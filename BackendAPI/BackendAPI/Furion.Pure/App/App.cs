// MIT License
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

using Furion.ConfigurableOptions;
using Furion.Reflection;
using Furion.Templates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Profiling;
using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Claims;

namespace Furion;

/// <summary>
/// 全局应用类
/// </summary>
[SuppressSniffer]
public static class App
{
    /// <summary>
    /// 私有设置，避免重复解析
    /// </summary>
    internal static AppSettingsOptions _settings;

    /// <summary>
    /// 应用全局配置
    /// </summary>
    public static AppSettingsOptions Settings => _settings ??= GetConfig<AppSettingsOptions>("AppSettings", true);

    /// <summary>
    /// 全局配置选项
    /// </summary>
    public static IConfiguration Configuration => InternalApp.Configuration;

    /// <summary>
    /// 获取Web主机环境，如，是否是开发环境，生产环境等
    /// </summary>
    public static IWebHostEnvironment WebHostEnvironment => InternalApp.WebHostEnvironment;

    /// <summary>
    /// 获取泛型主机环境，如，是否是开发环境，生产环境等
    /// </summary>
    public static IHostEnvironment HostEnvironment => InternalApp.HostEnvironment;

    /// <summary>
    /// 存储根服务，可能为空
    /// </summary>
    public static IServiceProvider RootServices => InternalApp.RootServices;

    /// <summary>
    /// 判断是否是单文件环境
    /// </summary>
    public static bool SingleFileEnvironment => string.IsNullOrWhiteSpace(Assembly.GetEntryAssembly().Location);

    /// <summary>
    /// 应用有效程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    /// <summary>
    /// 获取请求上下文
    /// </summary>
    public static HttpContext HttpContext => RootServices?.GetService<IHttpContextAccessor>()?.HttpContext;

    /// <summary>
    /// 获取请求上下文用户
    /// </summary>
    /// <remarks>只有授权访问的页面或接口才存在值，否则为 null</remarks>
    public static ClaimsPrincipal User => HttpContext?.User;

    /// <summary>
    /// 未托管的对象集合
    /// </summary>
    public static readonly ConcurrentBag<IDisposable> UnmanagedObjects;

    /// <summary>
    /// 解析服务提供器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProvider(Type serviceType)
    {
        // 处理控制台应用程序
        if (HostEnvironment == default) return RootServices;

        // 第一选择，判断是否是单例注册且单例服务不为空，如果是直接返回根服务提供器
        if (RootServices != null && InternalApp.InternalServices.Where(u => u.ServiceType == (serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition() : serviceType))
                                                                .Any(u => u.Lifetime == ServiceLifetime.Singleton)) return RootServices;

        // 第二选择是获取 HttpContext 对象的 RequestServices
        var httpContext = HttpContext;
        if (httpContext?.RequestServices != null) return httpContext.RequestServices;
        // 第三选择，创建新的作用域并返回服务提供器
        else if (RootServices != null)
        {
            var scoped = RootServices.CreateScope();
            UnmanagedObjects.Add(scoped);
            return scoped.ServiceProvider;
        }
        // 第四选择，构建新的服务对象（性能最差）
        else
        {
            var serviceProvider = InternalApp.InternalServices.BuildServiceProvider();
            UnmanagedObjects.Add(serviceProvider);
            return serviceProvider;
        }
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetService<TService>(IServiceProvider serviceProvider = default)
        where TService : class
    {
        return GetService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetService(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetService(type);
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TService GetRequiredService<TService>(IServiceProvider serviceProvider = default)
        where TService : class
    {
        return GetRequiredService(typeof(TService), serviceProvider) as TService;
    }

    /// <summary>
    /// 获取请求生存周期的服务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static object GetRequiredService(Type type, IServiceProvider serviceProvider = default)
    {
        return (serviceProvider ?? GetServiceProvider(type)).GetRequiredService(type);
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="path">配置中对应的Key</param>
    /// <param name="loadPostConfigure"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetConfig<TOptions>(string path, bool loadPostConfigure = false)
    {
        var options = Configuration.GetSection(path).Get<TOptions>();

        // 加载默认选项配置
        if (loadPostConfigure && typeof(IConfigurableOptions).IsAssignableFrom(typeof(TOptions)))
        {
            var postConfigure = typeof(TOptions).GetMethod("PostConfigure");
            if (postConfigure != null)
            {
                options ??= Activator.CreateInstance<TOptions>();
                postConfigure.Invoke(options, new object[] { options, Configuration });
            }
        }

        return options;
    }

    /// <summary>
    /// 获取选项
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptions<TOptions>(IServiceProvider serviceProvider = default)
        where TOptions : class, new()
    {
        return GetService<IOptions<TOptions>>(serviceProvider ?? RootServices)?.Value;
    }

    /// <summary>
    /// 获取选项
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptionsMonitor<TOptions>(IServiceProvider serviceProvider = default)
        where TOptions : class, new()
    {
        return GetService<IOptionsMonitor<TOptions>>(serviceProvider ?? RootServices)?.CurrentValue;
    }

    /// <summary>
    /// 获取选项
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns>TOptions</returns>
    public static TOptions GetOptionsSnapshot<TOptions>(IServiceProvider serviceProvider = default)
        where TOptions : class, new()
    {
        // 这里不能从根服务解析，因为是 Scoped 作用域
        return GetService<IOptionsSnapshot<TOptions>>(serviceProvider)?.Value;
    }

    /// <summary>
    /// 获取命令行配置
    /// </summary>
    /// <param name="args"></param>
    /// <param name="switchMappings"></param>
    /// <returns></returns>
    public static CommandLineConfigurationProvider GetCommandLineConfiguration(string[] args, IDictionary<string, string> switchMappings = null)
    {
        var commandLineConfiguration = new CommandLineConfigurationProvider(args, switchMappings);
        commandLineConfiguration.Load();

        return commandLineConfiguration;
    }

    /// <summary>
    /// 打印验证信息到 MiniProfiler
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="state">状态</param>
    /// <param name="message">消息</param>
    /// <param name="isError">是否为警告消息</param>
    public static void PrintToMiniProfiler(string category, string state, string message = null, bool isError = false)
    {
        if (!CanBeMiniProfiler()) return;

        // 打印消息
        var titleCaseCategory = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(category);
        var customTiming = MiniProfiler.Current?.CustomTiming(category, string.IsNullOrWhiteSpace(message) ? $"{titleCaseCategory} {state}" : message, state);
        if (customTiming == null) return;

        // 判断是否是警告消息
        if (isError) customTiming.Errored = true;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    static App()
    {
        // 未托管的对象
        UnmanagedObjects = new ConcurrentBag<IDisposable>();

        // 加载程序集
        var assObject = GetAssemblies();
        Assemblies = assObject.Assemblies;
        ExternalAssemblies = assObject.ExternalAssemblies;

        // 获取有效的类型集合
        EffectiveTypes = Assemblies.SelectMany(GetTypes);

        AppStartups = new ConcurrentBag<AppStartup>();
    }

    /// <summary>
    /// 应用所有启动配置对象
    /// </summary>
    internal static ConcurrentBag<AppStartup> AppStartups;

    /// <summary>
    /// 外部程序集
    /// </summary>
    internal static IEnumerable<Assembly> ExternalAssemblies;

    /// <summary>
    /// 获取应用有效程序集
    /// </summary>
    /// <returns>IEnumerable</returns>
    private static (IEnumerable<Assembly> Assemblies, IEnumerable<Assembly> ExternalAssemblies) GetAssemblies()
    {
        // 需排除的程序集后缀
        var excludeAssemblyNames = new string[] {
                "Database.Migrations"
            };

        // 读取应用配置
        var supportPackageNamePrefixs = Settings.SupportPackageNamePrefixs ?? Array.Empty<string>();

        IEnumerable<Assembly> scanAssemblies;

        // 获取入口程序集
        var entryAssembly = Assembly.GetEntryAssembly();

        // 非独立发布/非单文件发布
        if (!string.IsNullOrWhiteSpace(entryAssembly.Location))
        {
            var dependencyContext = DependencyContext.Default;

            // 读取项目程序集或 Furion 官方发布的包，或手动添加引用的dll，或配置特定的包前缀
            scanAssemblies = dependencyContext.RuntimeLibraries
               .Where(u =>
                      (u.Type == "project" && !excludeAssemblyNames.Any(j => u.Name.EndsWith(j))) ||
                      (u.Type == "package" && (u.Name.StartsWith(nameof(Furion)) || supportPackageNamePrefixs.Any(p => u.Name.StartsWith(p)))) ||
                      (Settings.EnabledReferenceAssemblyScan == true && u.Type == "reference"))    // 判断是否启用引用程序集扫描
               .Select(u => Reflect.GetAssembly(u.Name));
        }
        // 独立发布/单文件发布
        else
        {
            IEnumerable<Assembly> fixedSingleFileAssemblies = new[] { entryAssembly };

            // 扫描实现 ISingleFilePublish 接口的类型
            var singleFilePublishType = entryAssembly.GetTypes()
                                                .FirstOrDefault(u => u.IsClass && !u.IsInterface && !u.IsAbstract && typeof(ISingleFilePublish).IsAssignableFrom(u));
            if (singleFilePublishType != null)
            {
                var singleFilePublish = Activator.CreateInstance(singleFilePublishType) as ISingleFilePublish;

                // 加载用户自定义配置单文件所需程序集
                var nativeAssemblies = singleFilePublish.IncludeAssemblies();
                var loadAssemblies = singleFilePublish.IncludeAssemblyNames()
                                                .Select(u => Reflect.GetAssembly(u));

                fixedSingleFileAssemblies = fixedSingleFileAssemblies.Concat(nativeAssemblies)
                                                            .Concat(loadAssemblies);

                // 解决 Furion.Extras.ObjectMapper.Mapster 程序集不能加载问题
                try
                {
                    if (!fixedSingleFileAssemblies.Any(u => u.GetName().Name.Equals(ObjectMapperServiceCollectionExtensions.ASSEMBLY_NAME)))
                    {
                        fixedSingleFileAssemblies = fixedSingleFileAssemblies.Concat(new[] {
                            Reflect.GetAssembly(ObjectMapperServiceCollectionExtensions.ASSEMBLY_NAME) });
                    }
                }
                catch { }
            }
            else
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                // 提示没有正确配置单文件配置
                Console.WriteLine(TP.Wrapper("Deploy Console"
                    , "Single file deploy error."
                    , "##Exception## Single file deployment configuration error."
                    , "##Documentation## https://dotnetchina.gitee.io/furion/docs/singlefile"));
                Console.ResetColor();
            }

            // 通过 AppDomain.CurrentDomain 扫描，默认为延迟加载，正常只能扫描到 Furion 和 入口程序集（启动层）
            scanAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                    .Where(ass =>
                                            // 排除 System，Microsoft，netstandard 开头的程序集
                                            !ass.FullName.StartsWith(nameof(System))
                                            && !ass.FullName.StartsWith(nameof(Microsoft))
                                            && !ass.FullName.StartsWith("netstandard"))
                                    .Concat(fixedSingleFileAssemblies)
                                    .Distinct();
        }

        IEnumerable<Assembly> externalAssemblies = Array.Empty<Assembly>();

        // 加载 `appsetting.json` 配置的外部程序集
        if (Settings.ExternalAssemblies != null && Settings.ExternalAssemblies.Any())
        {
            foreach (var externalAssembly in Settings.ExternalAssemblies)
            {
                // 加载外部程序集
                var assemblyFileFullPath = Path.Combine(AppContext.BaseDirectory
                    , externalAssembly.EndsWith(".dll") ? externalAssembly : $"{externalAssembly}.dll");

                // 根据路径加载程序集
                var loadedAssembly = Reflect.LoadAssembly(assemblyFileFullPath);
                if (loadedAssembly == default) continue;
                var assembly = new[] { loadedAssembly };

                // 合并程序集
                scanAssemblies = scanAssemblies.Concat(assembly);
                externalAssemblies = externalAssemblies.Concat(assembly);
            }
        }

        // 处理排除的程序集
        if (Settings.ExcludeAssemblies != null && Settings.ExcludeAssemblies.Any())
        {
            scanAssemblies = scanAssemblies.Where(ass => !Settings.ExcludeAssemblies.Contains(ass.GetName().Name, StringComparer.OrdinalIgnoreCase));
        }

        return (scanAssemblies, externalAssemblies);
    }

    /// <summary>
    /// 加载程序集中的所有类型
    /// </summary>
    /// <param name="ass"></param>
    /// <returns></returns>
    private static IEnumerable<Type> GetTypes(Assembly ass)
    {
        var types = Array.Empty<Type>();

        try
        {
            types = ass.GetTypes();
        }
        catch
        {
            Console.WriteLine($"Error load `{ass.FullName}` assembly.");
        }

        return types.Where(u => u.IsPublic && !u.IsDefined(typeof(SuppressSnifferAttribute), false));
    }

    /// <summary>
    /// 判断是否启用 MiniProfiler
    /// </summary>
    /// <returns></returns>
    internal static bool CanBeMiniProfiler()
    {
        // 减少不必要的监听
        if (Settings.InjectMiniProfiler != true || HttpContext == null
            || !(HttpContext.Request.Headers.TryGetValue("request-from", out var value) && value == "swagger")) return false;

        return true;
    }

    /// <summary>
    /// 释放所有未托管的对象
    /// </summary>
    public static void DisposeUnmanagedObjects()
    {
        foreach (var dsp in UnmanagedObjects)
        {
            try
            {
                dsp?.Dispose();
            }
            finally { }
        }

        UnmanagedObjects.Clear();
    }
}