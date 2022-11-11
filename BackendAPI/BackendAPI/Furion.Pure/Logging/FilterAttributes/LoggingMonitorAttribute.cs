﻿// MIT License
//
// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Furion.DataValidation;
using Furion.FriendlyException;
using Furion.Logging;
using Furion.Templates;
using Furion.UnifyResult;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System;

/// <summary>
/// 强大的日志监听器
/// </summary>
/// <remarks>主要用于将请求的信息打印出来</remarks>
[SuppressSniffer, AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class LoggingMonitorAttribute : Attribute, IAsyncActionFilter, IOrderedFilter
{
    /// <summary>
    /// 过滤器排序
    /// </summary>
    private const int FilterOrder = -2000;

    /// <summary>
    /// 排序属性
    /// </summary>
    public int Order => FilterOrder;

    /// <summary>
    /// 日志 LogName
    /// </summary>
    /// <remarks>方便对日志进行过滤写入不同的存储介质中</remarks>
    internal const string LOG_CATEGORY_NAME = "System.Logging.LoggingMonitor";

    /// <summary>
    /// 日志上下文统一 Key
    /// </summary>
    private const string LOG_CONTEXT_NAME = "LoggingMonitor";

    /// <summary>
    /// 构造函数
    /// </summary>
    public LoggingMonitorAttribute()
        : this(new LoggingMonitorSettings())
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="settings"></param>
    internal LoggingMonitorAttribute(LoggingMonitorSettings settings)
    {
        Settings = settings;
    }

    /// <summary>
    /// 日志标题
    /// </summary>
    public string Title { get; set; } = "Logging Monitor";

    /// <summary>
    /// 是否记录返回值
    /// </summary>
    /// <remarks>bool 类型，默认输出</remarks>
    public object WithReturnValue { get; set; } = null;

    /// <summary>
    /// 设置返回值阈值
    /// </summary>
    /// <remarks>配置返回值字符串阈值，超过这个阈值将截断，默认全量输出</remarks>
    public object ReturnValueThreshold { get; set; } = null;

    /// <summary>
    /// 配置信息
    /// </summary>
    private LoggingMonitorSettings Settings { get; set; }

    /// <summary>
    /// 监视 Action 执行
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 判断是否是 Razor Pages
        var isPageDescriptor = context.ActionDescriptor is CompiledPageActionDescriptor;

        // 抛出不支持 Razor Pages 异常
        if (isPageDescriptor) throw new InvalidOperationException("The LoggingMonitorAttribute is not support the Razor Pages application.");

        // 获取控制器/操作描述器
        var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 获取请求方法
        var actionMethod = controllerActionDescriptor.MethodInfo;

        // 如果贴了 [SuppressMonitor] 特性则跳过
        if (actionMethod.IsDefined(typeof(SuppressMonitorAttribute), true))
        {
            _ = await next();
            return;
        }

        // 获取方法完整名称
        var methodFullName = controllerActionDescriptor.ControllerTypeInfo.FullName + "." + actionMethod.Name;

        // 只有方法没有贴有 [LoggingMonitor] 特性才判断全局，贴了特性优先级最大
        if (!actionMethod.IsDefined(typeof(LoggingMonitorAttribute), true))
        {
            // 解决通过 AddMvcFilter 的问题
            if (!Settings.IsMvcFilterRegister)
            {
                // 处理不启用但排除的情况
                if (!Settings.GlobalEnabled
                    && !Settings.IncludeOfMethods.Contains(methodFullName, StringComparer.OrdinalIgnoreCase))
                {
                    // 查找是否包含匹配，忽略大小写
                    _ = await next();
                    return;
                }

                // 处理启用但排除的情况
                if (Settings.GlobalEnabled
                    && Settings.ExcludeOfMethods.Contains(methodFullName, StringComparer.OrdinalIgnoreCase))
                {
                    _ = await next();
                    return;
                }
            }
        }

        // 获取全局 LoggingMonitorMethod 配置
        var monitorMethod = Settings.MethodsSettings.FirstOrDefault(m => m.FullName.Equals(methodFullName, StringComparison.OrdinalIgnoreCase));

        // 创建日志上下文
        var logContext = new LogContext();

        // 获取路由表信息
        var routeData = context.RouteData;
        var controllerName = routeData.Values["controller"];
        var actionName = routeData.Values["action"];
        var areaName = routeData.DataTokens["area"];
        logContext.Set($"{LOG_CONTEXT_NAME}.ControllerName", controllerName)
                  .Set($"{LOG_CONTEXT_NAME}.ControllerTypeName", controllerActionDescriptor.ControllerTypeInfo.Name)
                  .Set($"{LOG_CONTEXT_NAME}.ActionName", actionName)
                  .Set($"{LOG_CONTEXT_NAME}.ActionTypeName", actionMethod.Name)
                  .Set($"{LOG_CONTEXT_NAME}.AreaName", areaName);

        // 调用呈现链名称
        var displayName = controllerActionDescriptor.DisplayName;
        logContext.Set($"{LOG_CONTEXT_NAME}.DisplayName", displayName);

        // 获取 HttpContext 和 HttpRequest 对象
        var httpContext = context.HttpContext;
        var httpRequest = httpContext.Request;

        // 获取服务端 IPv4 地址
        var localIPv4 = httpContext.GetLocalIpAddressToIPv4();
        logContext.Set($"{LOG_CONTEXT_NAME}.LocalIPv4", localIPv4);

        // 获取客户端 IPv4 地址
        var remoteIPv4 = httpContext.GetRemoteIpAddressToIPv4();
        logContext.Set($"{LOG_CONTEXT_NAME}.RemoteIPv4", remoteIPv4);

        // 获取请求方式
        var httpMethod = httpContext.Request.Method;
        logContext.Set($"{LOG_CONTEXT_NAME}.RequestHttpMethod", httpMethod);

        // 获取请求的 Url 地址
        var requestUrl = Uri.UnescapeDataString(httpRequest.GetRequestUrlAddress());
        logContext.Set($"{LOG_CONTEXT_NAME}.RequestUrl", requestUrl);

        // 获取来源 Url 地址
        var refererUrl = Uri.UnescapeDataString(httpRequest.GetRefererUrlAddress());
        logContext.Set($"{LOG_CONTEXT_NAME}.RefererUrl", refererUrl);

        // 服务器环境
        var environmentName = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().EnvironmentName;
        logContext.Set($"{LOG_CONTEXT_NAME}.EnvironmentName", environmentName);

        // 客户端浏览器信息
        var userAgent = httpRequest.Headers["User-Agent"];
        logContext.Set($"{LOG_CONTEXT_NAME}.UserAgent", userAgent);

        // 获取方法参数
        var parameterValues = context.ActionArguments;

        // 获取授权用户
        var user = httpContext.User;

        // token 信息
        var authorization = httpRequest.Headers["Authorization"].ToString();
        logContext.Set($"{LOG_CONTEXT_NAME}.Authorization", authorization);

        // 计算接口执行时间
        var timeOperation = Stopwatch.StartNew();
        var resultContext = await next();
        timeOperation.Stop();
        logContext.Set($"{LOG_CONTEXT_NAME}.ElapsedMilliseconds", timeOperation.ElapsedMilliseconds);

        // 获取异常对象情况
        var exception = resultContext.Exception;
        if (exception == null)
        {
            // 解析存储的验证信息
            var validationFailedKey = nameof(DataValidationFilter) + nameof(ValidationMetadata);
            var validationMetadata = !httpContext.Items.ContainsKey(validationFailedKey)
                ? default
                : httpContext.Items[validationFailedKey] as ValidationMetadata;

            if (validationMetadata != null)
            {
                // 创建全局验证友好异常
                exception = new AppFriendlyException(SerializeObject(validationMetadata.ValidationResult), validationMetadata.OriginErrorCode)
                {
                    ErrorCode = validationMetadata.ErrorCode,
                    StatusCode = validationMetadata.StatusCode ?? StatusCodes.Status400BadRequest,
                    ValidationException = true
                };
            }
        }

        // 判断是否是验证异常
        var isValidationException = exception is AppFriendlyException friendlyException && friendlyException.ValidationException;

        var monitorItems = new List<string>()
        {
            $"##控制器名称## {controllerActionDescriptor.ControllerTypeInfo.Name}"
            , $"##操作名称## {actionMethod.Name}"
            , $"##路由信息## [area]: {areaName}; [controller]: {controllerName}; [action]: {actionName}"
            , $"##请求方式## {httpMethod}"
            , $"##请求地址## {requestUrl}"
            , $"##来源地址## {refererUrl}"
            , $"##浏览器标识## {userAgent}"
            , $"##客户端 IP 地址## {remoteIPv4}"
            , $"##服务端 IP 地址## {localIPv4}"
            , $"##服务端运行环境## {environmentName}"
            , $"##执行耗时## {timeOperation.ElapsedMilliseconds}ms"
        };

        // 添加 JWT 授权信息日志模板
        monitorItems.AddRange(GenerateAuthorizationTemplate(logContext, user, authorization));

        // 添加请求参数信息日志模板
        monitorItems.AddRange(GenerateParameterTemplate(logContext, parameterValues, actionMethod, httpRequest.Headers["Content-Type"]));

        // 判断是否启用返回值打印
        if (CheckIsSetWithReturnValue(WithReturnValue, monitorMethod))
        {
            // 添加返回值信息日志模板
            monitorItems.AddRange(GenerateReturnInfomationTemplate(logContext, resultContext, actionMethod, monitorMethod));
        }

        // 添加异常信息日志模板
        monitorItems.AddRange(GenerateExcetpionInfomationTemplate(logContext, exception, isValidationException));

        // 生成最终模板
        var monitor = TP.Wrapper(Title, displayName, monitorItems.ToArray());

        // 创建日志记录器
        var logger = httpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(LOG_CATEGORY_NAME);

        // 调用外部配置
        LoggingMonitorSettings.Configure?.Invoke(logger, logContext, resultContext);

        // 设置日志上下文
        logger.ScopeContext(logContext);

        // 写入日志，如果没有异常使用 LogInformation，否则使用 LogError
        if (exception == null) logger.LogInformation(monitor);
        else
        {
            // 如果不是验证异常，写入 Error
            if (!isValidationException) logger.LogError(exception, monitor);
            else
            {
                // 读取配置的日志级别并写入
                logger.Log(Settings.BahLogLevel, monitor);
            }
        }
    }

    /// <summary>
    /// 生成 JWT 授权信息日志模板
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="claimsPrincipal"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    private List<string> GenerateAuthorizationTemplate(LogContext logContext, ClaimsPrincipal claimsPrincipal, StringValues authorization)
    {
        var templates = new List<string>();

        if (!claimsPrincipal.Claims.Any()) return templates;

        templates.AddRange(new[]
        {
            $"━━━━━━━━━━━━━━━  授权信息 ━━━━━━━━━━━━━━━"
            , $"##JWT Token## {authorization}"
            , $""
        });

        // 遍历身份信息
        foreach (var claim in claimsPrincipal.Claims)
        {
            templates.Add($"##{claim.Type} ({(claim.ValueType.Replace("http://www.w3.org/2001/XMLSchema#", ""))})## {claim.Value}");
            logContext.Set($"{LOG_CONTEXT_NAME}.Authorization.{claim.Type}", claim.Value);
        }

        return templates;
    }

    /// <summary>
    /// 生成请求参数信息日志模板
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="parameterValues"></param>
    /// <param name="method"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    private List<string> GenerateParameterTemplate(LogContext logContext, IDictionary<string, object> parameterValues, MethodInfo method, StringValues contentType)
    {
        var templates = new List<string>();

        if (parameterValues.Count == 0) return templates;

        templates.AddRange(new[]
        {
            $"━━━━━━━━━━━━━━━  参数列表 ━━━━━━━━━━━━━━━"
            , $"##Content-Type## {contentType}"
            , $""
        });

        var parameters = method.GetParameters();
        foreach (var parameter in parameters)
        {
            var name = parameter.Name;
            _ = parameterValues.TryGetValue(name, out var value);
            var parameterType = parameter.ParameterType;

            object rawValue = default;

            // 文件类型参数
            if (value is IFormFile || value is List<IFormFile>)
            {
                // 单文件
                if (value is IFormFile formFile)
                {
                    var fileSize = Math.Round(formFile.Length / 1024D);
                    templates.Add($"##{name} ({parameterType.Name})## [name]: {formFile.FileName}; [size]: {fileSize}KB; [content-type]: {formFile.ContentType}");
                    logContext.Set($"{LOG_CONTEXT_NAME}.Parameter.{name}", $"File {{ name: {formFile.FileName}, size: {fileSize}, contentType: {formFile.ContentType} }}");
                    continue;
                }
                // 多文件
                else if (value is List<IFormFile> formFiles)
                {
                    for (var i = 0; i < formFiles.Count; i++)
                    {
                        var file = formFiles[i];
                        var size = Math.Round(file.Length / 1024D);
                        templates.Add($"##{name}[{i}] ({nameof(IFormFile)})## [name]: {file.FileName}; [size]: {size}KB; [content-type]: {file.ContentType}");
                        logContext.Set($"{LOG_CONTEXT_NAME}.Parameter.{name}[{i}]", $"File {{ name: {file.FileName}, size: {size}, contentType: {file.ContentType} }}");
                    }

                    continue;
                }
            }
            // 处理 byte[] 参数类型
            else if (value is byte[] byteArray)
            {
                templates.Add($"##{name} ({parameterType.Name})## [Length]: {byteArray.Length}");
                logContext.Set($"{LOG_CONTEXT_NAME}.Parameter.{name}", $"Byte[{byteArray.Length}]");
                continue;
            }
            // 处理基元类型，字符串类型和空值
            else if (parameterType.IsPrimitive || value is string || value == null) rawValue = value;
            // 其他类型统一进行序列化
            else
            {
                rawValue = SerializeObject(value);
            }

            templates.Add($"##{name} ({parameterType.Name})## {rawValue}");
            logContext.Set($"{LOG_CONTEXT_NAME}.Parameter.{name}", rawValue);
        }

        return templates;
    }

    /// <summary>
    /// 生成返回值信息日志模板
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="resultContext"></param>
    /// <param name="method"></param>
    /// <param name="monitorMethod"></param>
    /// <returns></returns>
    private List<string> GenerateReturnInfomationTemplate(LogContext logContext, ActionExecutedContext resultContext, MethodInfo method, LoggingMonitorMethod monitorMethod)
    {
        var templates = new List<string>();

        object returnValue = null;

        // 解析返回值
        if (UnifyContext.CheckVaildResult(resultContext.Result, out var data)) returnValue = data;

        // 获取最终呈现值（字符串类型）
        var displayValue = method.ReturnType == typeof(void)
            ? string.Empty
            : SerializeObject(returnValue);

        // 获取返回值阈值
        var threshold = CheckIsSetReturnValueThreshold(ReturnValueThreshold, monitorMethod);
        if (threshold > 0)
        {
            displayValue = displayValue[..(displayValue.Length > threshold ? threshold : displayValue.Length)];
        }

        templates.AddRange(new[]
        {
            $"━━━━━━━━━━━━━━━  返回信息 ━━━━━━━━━━━━━━━"
            , $"##原始类型## {method.ReturnType.FullName}"
            , $"##最终类型## {returnValue?.GetType()?.FullName}"
            , $"##最终返回值## {displayValue}"
        });
        logContext.Set($"{LOG_CONTEXT_NAME}.Return.Type", method.ReturnType.FullName)
                  .Set($"{LOG_CONTEXT_NAME}.Return.FinalType", returnValue?.GetType()?.FullName)
                  .Set($"{LOG_CONTEXT_NAME}.Return.FinalValue", displayValue);

        return templates;
    }

    /// <summary>
    /// 生成异常信息日志模板
    /// </summary>
    /// <param name="logContext"></param>
    /// <param name="exception"></param>
    /// <param name="isValidationException">是否是验证异常</param>
    /// <returns></returns>
    private List<string> GenerateExcetpionInfomationTemplate(LogContext logContext, Exception exception, bool isValidationException)
    {
        var templates = new List<string>();

        if (exception == null) return templates;

        // 处理不是验证异常情况
        if (!isValidationException)
        {
            templates.AddRange(new[]
            {
                $"━━━━━━━━━━━━━━━  异常信息 ━━━━━━━━━━━━━━━"
                , $"##类型## {exception.GetType().FullName}"
                , $"##消息## {exception.Message}"
                , $"##错误堆栈## {exception.StackTrace}"
            });
            logContext.Set($"{LOG_CONTEXT_NAME}.Exception.Type", exception.GetType().FullName)
                      .Set($"{LOG_CONTEXT_NAME}.Exception.Message", exception.Message)
                      .Set($"{LOG_CONTEXT_NAME}.Exception.StackTrace", exception.StackTrace);
        }
        else
        {
            var friendlyException = exception as AppFriendlyException;
            templates.AddRange(new[]
            {
                $"━━━━━━━━━━━━━━━  业务异常 ━━━━━━━━━━━━━━━"
                , $"##业务码## {friendlyException.ErrorCode}"
                , $"##业务码（原）## {friendlyException.OriginErrorCode}"
                , $"##业务消息## {friendlyException.ErrorMessage}"
            });
            logContext.Set($"{LOG_CONTEXT_NAME}.BusinessException.ErrorCode", friendlyException.ErrorCode)
                      .Set($"{LOG_CONTEXT_NAME}.BusinessException.OriginErrorCode", friendlyException.OriginErrorCode)
                      .Set($"{LOG_CONTEXT_NAME}.BusinessException.ErrorMessage", friendlyException.ErrorMessage);
        }

        return templates;
    }

    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static string SerializeObject(object obj)
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        try
        {
            return JsonSerializer.Serialize(obj, jsonSerializerOptions);
        }
        catch
        {
            return "<Error Serialize>";
        }
    }

    /// <summary>
    /// 检查是否开启启用返回值
    /// </summary>
    /// <param name="withReturnValue"></param>
    /// <param name="monitorMethod"></param>
    /// <returns></returns>
    private bool CheckIsSetWithReturnValue(object withReturnValue, LoggingMonitorMethod monitorMethod)
    {
        return withReturnValue == null
            ? (monitorMethod?.WithReturnValue ?? Settings.WithReturnValue)
            : Convert.ToBoolean(withReturnValue);
    }

    /// <summary>
    /// 检查是否设置返回值阈值
    /// </summary>
    /// <param name="returnValueThreshold"></param>
    /// <param name="monitorMethod"></param>
    /// <returns></returns>
    private int CheckIsSetReturnValueThreshold(object returnValueThreshold, LoggingMonitorMethod monitorMethod)
    {
        return returnValueThreshold == null
            ? (monitorMethod?.ReturnValueThreshold ?? Settings.ReturnValueThreshold)
            : Convert.ToInt32(returnValueThreshold);
    }
}