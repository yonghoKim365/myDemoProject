using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 메모리 변조가 안되는 클래스
/// </summary>
/// <typeparam name="T"></typeparam>
public class SecurityValue<T>
{
    private T hash_value;
    private T real_value;

    static bool initKey = false;

    public void InitKey()
    {
        if (!initKey)
        {
            Byte_KEY = (Byte)UnityEngine.Random.Range(1, Byte.MaxValue);
            Int16_KEY = (Int16)UnityEngine.Random.Range(1, Int16.MaxValue);
            Int32_KEY = (Int32)UnityEngine.Random.Range(1, Int32.MaxValue);
            Int64_KEY = (Int64)UnityEngine.Random.Range(1, Int64.MaxValue);
            UInt16_KEY = (UInt16)UnityEngine.Random.Range(1, UInt16.MaxValue);
            UInt32_KEY = (UInt32)UnityEngine.Random.Range(1, UInt32.MaxValue);
            UInt64_KEY = (UInt64)UnityEngine.Random.Range(1, UInt64.MaxValue);

            Byte_KEY_H = (Byte)UnityEngine.Random.Range(1, Byte.MaxValue);
            Int16_KEY_H = (Int16)UnityEngine.Random.Range(1, Int16.MaxValue);
            Int32_KEY_H = (Int32)UnityEngine.Random.Range(1, Int32.MaxValue);
            Int64_KEY_H = (Int64)UnityEngine.Random.Range(1, Int64.MaxValue);
            UInt16_KEY_H = (UInt16)UnityEngine.Random.Range(1, UInt16.MaxValue);
            UInt32_KEY_H = (UInt32)UnityEngine.Random.Range(1, UInt32.MaxValue);
            UInt64_KEY_H = (UInt64)UnityEngine.Random.Range(1, UInt64.MaxValue);
            initKey = true;
        }
    }
    public T val
    {
        set
        {
            //리얼과 해쉬를 따로 저장해서
            real_value = (T)Encrypt(value);
            hash_value = (T)Encrypt(value, true);
        }
        get
        {
            object _val = Decrypt(real_value);
            object val2 = Decrypt(hash_value, true);

            //차후 동등 비교를 통해 검사한다.
            if (!_val.Equals(val2))
            {
                Debug.LogError("값이 변조 되었습니다");
                Application.Quit();
                _val = 0;
            }

            return (T)_val;
        }
    }

    /// <summary>
    /// 생성자에서 타입을 알아 논다
    /// </summary>
    /// <param name="value"></param>
    public SecurityValue(T value)
    {
        InitKey();
        typeCode = Convert.GetTypeCode(value);
        val = value;
    }

    TypeCode typeCode;

    static Byte Byte_KEY;
    static Int16 Int16_KEY;
    static Int32 Int32_KEY;
    static Int64 Int64_KEY;
    static UInt16 UInt16_KEY;
    static UInt32 UInt32_KEY;
    static UInt64 UInt64_KEY;

    static Byte Byte_KEY_H;
    static Int16 Int16_KEY_H;
    static Int32 Int32_KEY_H;
    static Int64 Int64_KEY_H;
    static UInt16 UInt16_KEY_H;
    static UInt32 UInt32_KEY_H;
    static UInt64 UInt64_KEY_H;

    /// <summary>
    /// 키를 알아 오는 함수
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    object GetKey(bool hash = false)
    {
        object result = null;
        switch (typeCode)
        {
            case TypeCode.Byte:
                result = hash ? Byte_KEY_H : Byte_KEY;
                break;
            case TypeCode.Int16:
                result = hash ? Int16_KEY_H : Int16_KEY;
                break;
            case TypeCode.Int32:
                result = hash ? Int32_KEY_H : Int32_KEY;
                break;
            case TypeCode.Int64:
                result = hash ? Int64_KEY_H : Int64_KEY;
                break;
            case TypeCode.UInt16:
                result = hash ? UInt16_KEY_H : UInt16_KEY;
                break;
            case TypeCode.UInt32:
                result = hash ? UInt32_KEY_H : UInt32_KEY;
                break;
            case TypeCode.UInt64:
                result = hash ? UInt64_KEY_H : UInt64_KEY;
                break;
            default:
                break;
        }

        return result;
    }

    object Encrypt(object _value, bool hash = false)
    {
        object result = null;
        switch (typeCode)
        {
            case TypeCode.Byte:
                result = Convert.ToByte(_value) ^ (byte)GetKey(hash);
                break;
            case TypeCode.Int16:
                result = Convert.ToInt16(_value) ^ (Int16)GetKey(hash);
                break;
            case TypeCode.Int32:
                result = Convert.ToInt32(_value) ^ (Int32)GetKey(hash);
                break;
            case TypeCode.Int64:
                result = Convert.ToInt64(_value) ^ (Int64)GetKey(hash);
                break;
            case TypeCode.UInt16:
                result = Convert.ToUInt16(_value) ^ (UInt16)GetKey(hash);
                break;
            case TypeCode.UInt32:
                result = Convert.ToUInt32(_value) ^ (UInt32)GetKey(hash);
                break;
            case TypeCode.UInt64:
                result = Convert.ToUInt64(_value) ^ (UInt64)GetKey(hash);
                break;
            default:
                break;
        }

        return result;
    }

    T Decrypt(T value, bool hash = false)
    {
        object result = null;

        switch (typeCode)
        {
            case TypeCode.Byte:
                result = Convert.ToByte(value) ^ (byte)GetKey(hash);
                break;
            case TypeCode.Int16:
                result = Convert.ToInt16(value) ^ (Int16)GetKey(hash);
                break;
            case TypeCode.Int32:
                result = Convert.ToInt32(value) ^ (Int32)GetKey(hash);
                break;
            case TypeCode.Int64:
                result = Convert.ToInt64(value) ^ (Int64)GetKey(hash);
                break;
            case TypeCode.UInt16:
                result = Convert.ToUInt16(value) ^ (UInt16)GetKey(hash);
                break;
            case TypeCode.UInt32:
                result = Convert.ToUInt32(value) ^ (UInt32)GetKey(hash);
                break;
            case TypeCode.UInt64:
                result = Convert.ToUInt64(value) ^ (UInt64)GetKey(hash);
                break;
            default:
                Debug.LogError(" 확인 할 수 없는 변수 타입 입니다. ");
                break;
        }

        return (T)result;
    }
}