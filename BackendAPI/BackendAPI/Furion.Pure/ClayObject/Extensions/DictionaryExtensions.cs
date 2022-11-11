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

using Furion.Extensions;
using System.Reflection;
using System.Text.Json;

namespace Furion.ClayObject.Extensions;

/// <summary>
/// 字典类型拓展类
/// </summary>
[SuppressSniffer]
public static class DictionaryExtensions
{
    /// <summary>
    /// 将对象转成字典
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static IDictionary<string, object> ToDictionary(this object input)
    {
        if (input == null) return default;

        // 处理本就是字典类型
        if (input.GetType().HasImplementedRawGeneric(typeof(IDictionary<,>)))
        {
            var dicInput = ((IDictionary)input);

            var dic = new Dictionary<string, object>();
            foreach (var key in dicInput.Keys)
            {
                dic.Add(key.ToString(), dicInput[key]);
            }

            return dic;
        }

        // 处理粘土对象
        if (input is Clay clay && clay.IsObject)
        {
            var dic = new Dictionary<string, object>();
            foreach (KeyValuePair<string, dynamic> item in (dynamic)clay)
            {
                dic.Add(item.Key, item.Value is Clay v ? v.ToDictionary() : item.Value);
            }
            return dic;
        }

        // 处理 JSON 类型
        if (input is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
        {
            return jsonElement.ToObject() as IDictionary<string, object>;
        }

        // 剩下的当对象处理
        var properties = input.GetType().GetProperties();
        var fields = input.GetType().GetFields();
        var members = properties.Cast<MemberInfo>().Concat(fields.Cast<MemberInfo>());

        return members.ToDictionary(m => m.Name, m => GetValue(input, m));
    }

    /// <summary>
    /// 将对象转字典类型，其中值返回原始类型 Type 类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static IDictionary<string, Tuple<Type, object>> ToDictionaryWithType(this object input)
    {
        if (input == null) return default;

        // 处理本就是字典类型
        if (input.GetType().HasImplementedRawGeneric(typeof(IDictionary<,>)))
        {
            var dicInput = ((IDictionary)input);

            var dic = new Dictionary<string, Tuple<Type, object>>();
            foreach (var key in dicInput.Keys)
            {
                var value = dicInput[key];
                var tupleValue = value == null ?
                    new Tuple<Type, object>(typeof(object), value) :
                    new Tuple<Type, object>(value.GetType(), value);

                dic.Add(key.ToString(), tupleValue);
            }

            return dic;
        }

        var dict = new Dictionary<string, Tuple<Type, object>>();

        // 获取所有属性列表
        foreach (var property in input.GetType().GetProperties())
        {
            dict.Add(property.Name, new Tuple<Type, object>(property.PropertyType, property.GetValue(input, null)));
        }

        // 获取所有成员列表
        foreach (var field in input.GetType().GetFields())
        {
            dict.Add(field.Name, new Tuple<Type, object>(field.FieldType, field.GetValue(input)));
        }

        return dict;
    }

    /// <summary>
    /// 获取成员值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    private static object GetValue(object obj, MemberInfo member)
    {
        if (member is PropertyInfo info)
            return info.GetValue(obj, null);

        if (member is FieldInfo info1)
            return info1.GetValue(obj);

        throw new ArgumentException("Passed member is neither a PropertyInfo nor a FieldInfo.");
    }
}