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

using Furion.Reflection;
using Furion.Templates.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Furion.SensitiveDetection;

/// <summary>
/// 脱敏词汇（脱敏）提供器（默认实现）
/// </summary>
[SuppressSniffer]
public class SensitiveDetectionProvider : ISensitiveDetectionProvider
{
    /// <summary>
    /// 分布式缓存
    /// </summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="distributedCache"></param>
    public SensitiveDetectionProvider(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    /// <summary>
    /// 分布式缓存键
    /// </summary>
    private const string DISTRIBUTED_KEY = "SENSITIVE:WORDS";

    /// <summary>
    /// 返回所有脱敏词汇
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<string>> GetWordsAsync()
    {
        // 读取缓存数据
        var wordsCached = await _distributedCache.GetStringAsync(DISTRIBUTED_KEY);
        if (wordsCached != null) return wordsCached.Split(new[] { "\r\n", "|" }, StringSplitOptions.RemoveEmptyEntries).Select(u => u.Trim());

        var entryAssembly = Reflect.GetEntryAssembly();

        // 解析嵌入式文件流
        byte[] buffer;
        using (var readStream = entryAssembly.GetManifestResourceStream($"{Reflect.GetAssemblyName(entryAssembly)}.sensitive-words.txt"))
        {
            buffer = new byte[readStream.Length];
            await readStream.ReadAsync(buffer.AsMemory(0, buffer.Length));
        }

        var content = Encoding.UTF8.GetString(buffer);

        // 缓存数据
        await _distributedCache.SetStringAsync(DISTRIBUTED_KEY, content);

        // 取换行符分割字符串
        var words = content.Split(new[] { "\r\n", "|" }, StringSplitOptions.RemoveEmptyEntries).Select(u => u.Trim());

        return words;
    }

    /// <summary>
    /// 判断脱敏词汇是否有效（支持自定义算法）
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task<bool> VaildedAsync(string text)
    {
        // 空字符串和空白字符不验证
        if (string.IsNullOrWhiteSpace(text)) return true;

        // 查找脱敏词汇出现次数和位置
        var foundSets = await FoundSensitiveWordsAsync(text);

        return foundSets.Count == 0;
    }

    /// <summary>
    /// 替换敏感词汇
    /// </summary>
    /// <param name="text"></param>
    /// <param name="transfer"></param>
    /// <returns></returns>
    public async Task<string> ReplaceAsync(string text, char transfer = '*')
    {
        if (string.IsNullOrWhiteSpace(text)) return default;

        // 查找脱敏词汇出现次数和位置
        var foundSets = await FoundSensitiveWordsAsync(text);

        // 如果没有敏感词则返回原字符串
        if (foundSets.Count == 0) return text;

        var stringBuilder = new StringBuilder(text);

        // 循环替换
        foreach (var kv in foundSets)
        {
            for (var i = 0; i < kv.Value.Count; i++)
            {
                for (var j = 0; j < kv.Key.Length; j++)
                {
                    var tempIndex = GetSensitiveWordIndex(kv.Value, i, sensitiveWordLength: kv.Key.Length);

                    // 设置替换的字符
                    stringBuilder[tempIndex + j] = transfer;
                }
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 查找脱敏词汇
    /// </summary>
    /// <param name="text"></param>
    private async Task<Dictionary<string, List<int>>> FoundSensitiveWordsAsync(string text)
    {
        // 支持读取配置渲染
        var realText = text.Render();

        // 获取词库
        var sensitiveWords = await GetWordsAsync();

        var stringBuilder = new StringBuilder(realText);
        var tempStringBuilder = new StringBuilder();

        // 记录脱敏词汇出现位置和次数
        int findIndex;
        var foundSets = new Dictionary<string, List<int>>();

        // 遍历所有脱敏词汇并查找字符串是否包含
        foreach (var sensitiveWord in sensitiveWords)
        {
            // 重新填充目标字符串
            tempStringBuilder.Clear();
            tempStringBuilder.Append(stringBuilder);

            // 查询查找至结尾
            while (tempStringBuilder.ToString().IndexOf(sensitiveWord) > -1)
            {
                if (foundSets.ContainsKey(sensitiveWord) == false)
                {
                    foundSets.Add(sensitiveWord, new List<int>());
                }

                findIndex = tempStringBuilder.ToString().IndexOf(sensitiveWord);
                foundSets[sensitiveWord].Add(findIndex);

                // 删除从零开始，长度为 findIndex + sensitiveWord.Length 的字符串
                tempStringBuilder.Remove(0, findIndex + sensitiveWord.Length);
            }
        }

        return foundSets;
    }

    /// <summary>
    /// 获取敏感词索引
    /// </summary>
    /// <param name="list"></param>
    /// <param name="count"></param>
    /// <param name="sensitiveWordLength"></param>
    /// <returns></returns>
    private static int GetSensitiveWordIndex(List<int> list, int count, int sensitiveWordLength)
    {
        // 用于返回当前敏感词的第 count 个的真实索引
        var sum = 0;
        for (var i = 0; i <= count; i++)
        {
            if (i == 0)
            {
                sum = list[i];
            }
            else
            {
                sum += list[i] + sensitiveWordLength;
            }
        }
        return sum;
    }
}