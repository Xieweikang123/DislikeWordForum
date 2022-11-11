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

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Furion.DataValidation;

/// <summary>
/// 验证信息元数据
/// </summary>
public sealed class ValidationMetadata
{
    /// <summary>
    /// 验证结果
    /// </summary>
    /// <remarks>返回字典或字符串类型</remarks>
    public object ValidationResult { get; internal set; }

    /// <summary>
    /// 异常消息
    /// </summary>
    public string Message { get; internal set; }

    /// <summary>
    /// 验证状态
    /// </summary>
    public ModelStateDictionary ModelState { get; internal set; }

    /// <summary>
    /// 错误码
    /// </summary>
    public object ErrorCode { get; internal set; }

    /// <summary>
    /// 错误码（没被复写过的 ErrorCode ）
    /// </summary>
    public object OriginErrorCode { get; internal set; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int? StatusCode { get; internal set; }

    /// <summary>
    /// 首个错误属性
    /// </summary>
    public string FirstErrorProperty { get; internal set; }

    /// <summary>
    /// 首个错误消息
    /// </summary>
    public string FirstErrorMessage { get; internal set; }

    /// <summary>
    /// 额外数据
    /// </summary>
    public object Data { get; internal set; }
}