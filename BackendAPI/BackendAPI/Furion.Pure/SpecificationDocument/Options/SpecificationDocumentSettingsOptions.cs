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
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Furion.SpecificationDocument;

/// <summary>
/// 规范化文档配置选项
/// </summary>
public sealed class SpecificationDocumentSettingsOptions : IConfigurableOptions<SpecificationDocumentSettingsOptions>
{
    /// <summary>
    /// 文档标题
    /// </summary>
    public string DocumentTitle { get; set; }

    /// <summary>
    /// 默认分组名
    /// </summary>
    public string DefaultGroupName { get; set; }

    /// <summary>
    /// 启用授权支持
    /// </summary>
    public bool? EnableAuthorized { get; set; }

    /// <summary>
    /// 格式化为V2版本
    /// </summary>
    public bool? FormatAsV2 { get; set; }

    /// <summary>
    /// 配置规范化文档地址
    /// </summary>
    public string RoutePrefix { get; set; }

    /// <summary>
    /// 文档展开设置
    /// </summary>
    public DocExpansion? DocExpansionState { get; set; }

    /// <summary>
    /// XML 描述文件
    /// </summary>
    public string[] XmlComments { get; set; }

    /// <summary>
    /// 分组信息
    /// </summary>
    public SpecificationOpenApiInfo[] GroupOpenApiInfos { get; set; }

    /// <summary>
    /// 安全定义
    /// </summary>
    public SpecificationOpenApiSecurityScheme[] SecurityDefinitions { get; set; }

    /// <summary>
    /// 配置 Servers
    /// </summary>
    public OpenApiServer[] Servers { get; set; }

    /// <summary>
    /// 隐藏 Servers
    /// </summary>
    public bool? HideServers { get; set; }

    /// <summary>
    /// 默认 swagger.json 路由模板
    /// </summary>
    public string RouteTemplate { get; set; }

    /// <summary>
    /// 配置安装第三方包的分组名
    /// </summary>
    public string[] PackagesGroups { get; set; }

    /// <summary>
    /// 启用枚举 Schema 筛选器
    /// </summary>
    public bool? EnableEnumSchemaFilter { get; set; }

    /// <summary>
    /// 启用标签排序筛选器
    /// </summary>
    public bool? EnableTagsOrderDocumentFilter { get; set; }

    /// <summary>
    /// 服务目录（修正 IIS 创建 Application 问题）
    /// </summary>
    public string ServerDir { get; set; }

    /// <summary>
    /// 配置规范化文档登录信息
    /// </summary>
    public SpecificationLoginInfo LoginInfo { get; set; }

    /// <summary>
    /// 启用 All Groups 功能
    /// </summary>
    public bool? EnableAllGroups { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    public void PostConfigure(SpecificationDocumentSettingsOptions options, IConfiguration configuration)
    {
        options.DocumentTitle ??= "Specification Api Document";
        options.DefaultGroupName ??= "Default";
        options.FormatAsV2 ??= false;
        //options.RoutePrefix ??= "api";    // 可以通过 UseInject() 配置，所以注释
        options.DocExpansionState ??= DocExpansion.List;

        // 加载项目注册和模块化/插件注释
        var frameworkPackageName = Reflect.GetAssemblyName(GetType());
        var projectXmlComments = App.Assemblies.Where(u => u.GetName().Name != frameworkPackageName).Select(t => t.GetName().Name);
        var externalXmlComments = App.ExternalAssemblies.Any() ? App.Settings.ExternalAssemblies.Select(u => u.EndsWith(".dll") ? u[0..^4] : u) : Array.Empty<string>();
        XmlComments ??= projectXmlComments.Concat(externalXmlComments).ToArray();

        GroupOpenApiInfos ??= new SpecificationOpenApiInfo[]
        {
                new SpecificationOpenApiInfo()
                {
                    Group=options.DefaultGroupName
                }
        };

        EnableAuthorized ??= true;
        if (EnableAuthorized == true)
        {
            SecurityDefinitions ??= new SpecificationOpenApiSecurityScheme[]
            {
                    new SpecificationOpenApiSecurityScheme
                    {
                        Id="Bearer",
                        Type= SecuritySchemeType.Http,
                        Name="Authorization",
                        Description="JWT Authorization header using the Bearer scheme.",
                        BearerFormat="JWT",
                        Scheme="bearer",
                        In= ParameterLocation.Header,
                        Requirement=new SpecificationOpenApiSecurityRequirementItem
                        {
                            Scheme=new OpenApiSecurityScheme
                            {
                                Reference=new OpenApiReference
                                {
                                    Id="Bearer",
                                    Type= ReferenceType.SecurityScheme
                                }
                            },
                            Accesses=Array.Empty<string>()
                        }
                    }
            };
        }

        Servers ??= Array.Empty<OpenApiServer>();
        HideServers ??= true;
        RouteTemplate ??= "swagger/{documentName}/swagger.json";
        PackagesGroups ??= Array.Empty<string>();
        EnableEnumSchemaFilter ??= true;
        EnableTagsOrderDocumentFilter ??= true;
        EnableAllGroups ??= false;
    }
}