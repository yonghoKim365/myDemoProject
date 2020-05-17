using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class StringExtension
{
    /// <summary>
    /// ex ) "...".Split<int>(',',p=>int.Parse(p))
    /// </summary>
    public static List<T> Split<T>(this string source, char delimiter, Converter<string, T> func)
    {
         return source.Split(delimiter).Select(n => func(n) ).ToList();
    }

    /// <summary>
    /// ex ) "...".Split<int>(',')
    /// </summary>
    public static List<T> Split<T>(this string source, char delimiter)
    {
        return source.Split(delimiter).Select(n => (T)System.Convert.ChangeType(n, typeof(T))).ToList();
    }

    public static string GetBase64string(this string source)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(source);
        return Convert.ToBase64String(bytesToEncode);
    }

    // Server->Client String변환
    public static string GetUTF8string(this string source)
    {
        byte[] decodedBytes = Convert.FromBase64String(source);
        return Encoding.UTF8.GetString(decodedBytes);
    }
}
