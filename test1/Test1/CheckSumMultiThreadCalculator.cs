// <copyright file="CheckSumMultiThreadCalculator.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace CheckSumCalculator;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// MultiThread check-sum calculator class.
/// </summary>
public static class CheckSumMultiThreadCalculator
{
    /// <summary>
    /// Method to run multi thread check-sum calculator.
    /// </summary>
    /// <param name="path">Path to directory or file.</param>
    /// <returns>Byte array.</returns>
    /// <exception cref="ArgumentException">Throws if path is invalid.</exception>
    public static async Task<byte[]> RunAsync(string path)
    {
        if (File.Exists(path))
        {
            return await CalculateFileSumAsync(path);
        }
        else if (Directory.Exists(path))
        {
            return await CalculateDirectorySumAsync(path);
        }

        throw new ArgumentException("Invalid path");
    }

    private static async Task<byte[]> CalculateDirectorySumAsync(string path)
    {
        var tasks = new List<Task<byte[]>>();

        foreach (var file in Directory.EnumerateFiles(path).OrderBy(f => f))
        {
            tasks.Add(CalculateFileSumAsync(file));
        }

        foreach (var dir in Directory.EnumerateDirectories(path).OrderBy(d => d))
        {
            tasks.Add(CalculateDirectorySumAsync(dir));
        }

        await Task.WhenAll(tasks);

        byte[] argument = tasks.SelectMany(t => t.Result).ToArray();

        DirectoryInfo directory = new (path);
        argument = [.. argument, .. MD5.HashData(Encoding.ASCII.GetBytes(directory.Name))];

        return MD5.HashData(argument);
    }

    private static async Task<byte[]> CalculateFileSumAsync(string path)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(path);

        return await Task.Run(() => md5.ComputeHash(stream));
    }
}
