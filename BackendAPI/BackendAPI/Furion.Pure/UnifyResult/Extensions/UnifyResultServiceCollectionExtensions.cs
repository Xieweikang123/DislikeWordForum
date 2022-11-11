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

using Furion.UnifyResult;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 规范化结果服务拓展
/// </summary>
[SuppressSniffer]
public static class UnifyResultServiceCollectionExtensions
{
    /// <summary>
    /// 添加规范化结果服务
    /// </summary>
    /// <param name="mvcBuilder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddUnifyResult(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.AddUnifyResult<RESTfulResultProvider>();

        return mvcBuilder;
    }

    /// <summary>
    /// 添加规范化结果服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUnifyResult(this IServiceCollection services)
    {
        services.AddUnifyResult<RESTfulResultProvider>();

        return services;
    }

    /// <summary>
    /// 添加规范化结果服务
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="mvcBuilder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddUnifyResult<TUnifyResultProvider>(this IMvcBuilder mvcBuilder)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        mvcBuilder.Services.AddUnifyResult<TUnifyResultProvider>();

        return mvcBuilder;
    }

    /// <summary>
    /// 添加规范化结果服务
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUnifyResult<TUnifyResultProvider>(this IServiceCollection services)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        // 添加配置
        services.AddConfigurableOptions<UnifyResultSettingsOptions>();

        // 是否启用规范化结果
        UnifyContext.EnabledUnifyHandler = true;

        // 添加规范化提供器
        services.AddUnifyProvider<TUnifyResultProvider>(string.Empty);

        // 添加成功规范化结果筛选器
        services.AddMvcFilter<SucceededUnifyResultFilter>();

        return services;
    }

    /// <summary>
    /// 替换默认的规范化结果
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUnifyProvider<TUnifyResultProvider>(this IServiceCollection services)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        return services.AddUnifyProvider<TUnifyResultProvider>(string.Empty);
    }

    /// <summary>
    /// 添加规范化提供器
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="services"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static IServiceCollection AddUnifyProvider<TUnifyResultProvider>(this IServiceCollection services, string providerName)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        providerName ??= string.Empty;

        var providerType = typeof(TUnifyResultProvider);

        // 添加规范化提供器
        services.TryAddSingleton(providerType, providerType);

        // 获取规范化提供器模型，不能为空
        var resultType = providerType.GetCustomAttribute<UnifyModelAttribute>().ModelType;

        // 创建规范化元数据
        var metadata = new UnifyMetadata
        {
            ProviderName = providerName,
            ProviderType = providerType,
            ResultType = resultType
        };

        // 添加或替换规范化配置
        UnifyContext.UnifyProviders.AddOrUpdate(providerName, _ => metadata, (_, _) => metadata);

        return services;
    }
}