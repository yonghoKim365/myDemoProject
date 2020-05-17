using System;
using System.Collections.Generic;

/// <summary>
/// 캐싱을 위한 클래스
/// </summary>
public class Cache<TKey, TValue> where TValue : new()
{
    private readonly Dictionary<TKey, TValue> cachedDic;

    public Cache()
    {
        cachedDic = new Dictionary<TKey, TValue>();
    }

    public TValue Get(TKey key)
    {
        if (cachedDic.ContainsKey(key))
            return cachedDic[key];

        //var value = new TValue();
        //_dict[key] = value;
        //return value;

        return default(TValue);
    }

    public void Put(TKey key, TValue value)
    {
        if (!cachedDic.ContainsKey(key))
            cachedDic[key] = value;
    }

    public void Clear()
    {
        foreach (TValue value in cachedDic.Values)
        {
            IDisposable disposable = value as IDisposable;
            if (null != disposable)
                disposable.Dispose();
        }
        cachedDic.Clear();
    }

    public string GetDebugInfo()
    {
        string str = string.Empty;

        foreach (KeyValuePair<TKey, TValue> pair in cachedDic)
        {
            str += "[ " + pair.Key.ToString() + " : " + pair.Value.ToString() + " ]";
        }

        return str;
    }
}