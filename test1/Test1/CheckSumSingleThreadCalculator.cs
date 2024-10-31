// <copyright file="CheckSumSingleThreadCalculator.cs" company="IlyaSotnikov">
// Copyright (c) IlyaSotnikov. All rights reserved.
// </copyright>

namespace CheckSumCalculator;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Singlethread check-sum calculator class.
/// </summary>
public static class CheckSumSingleThreadCalculator
{
    /// <summary>
    /// Method to run single thread check-sum calculator.
    /// </summary>
    /// <param name="path">Path to directory or file.</param>
    /// <returns>Byte array.</returns>
    /// <exception cref="ArgumentException">Throws if path is invalid.</exception>
    public static byte[] Run(string path)
    {
        if (File.Exists(path))
        {
            return CalculateFileSum(path);
        }
        else if (Directory.Exists(path))
        {
            return CalculateDirectorySum(path);
        }

        throw new ArgumentException("Invalid path");
    }

    private static byte[] CalculateDirectorySum(string path)
    {
        byte[] argument = [];

        foreach (var file in Directory.EnumerateFiles(path).OrderBy(f => f))
        {
            argument = [.. argument, .. CalculateFileSum(file)];
        }

        foreach (var dir in Directory.EnumerateDirectories(path).OrderBy(d => d))
        {
            argument = [.. argument, .. CalculateDirectorySum(dir)];
        }

        DirectoryInfo directory = new (path);
        argument = [.. argument, .. MD5.HashData(Encoding.ASCII.GetBytes(directory.Name))];

        return MD5.HashData(argument);
    }

    private static byte[] CalculateFileSum(string path)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(path);

        return md5.ComputeHash(stream);
    }
}
