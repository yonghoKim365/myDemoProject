using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TempCoroutine : MonoBehaviour 
{
    private static TempCoroutine m_Instance = null;
    public static TempCoroutine instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(TempCoroutine)) as TempCoroutine;

                if (m_Instance == null)
                {
                    m_Instance = new GameObject(typeof(TempCoroutine).ToString(), typeof(TempCoroutine)).GetComponent<TempCoroutine>();

                    if (m_Instance == null)
                    {
                        Debug.LogError("Immortal Intance Init ERROR - " + typeof(TempCoroutine).ToString());
                    }
                }
            }
            return m_Instance;
        }
    }

    private struct KeyData
    {
        public string Key;
        public DateTime StartTime; 
        public Action Callback;

        public KeyData(string k, DateTime s, Action c)
        {
            Key = k;
            StartTime = s;
            Callback = c;
        }
    }

    private Dictionary<string, KeyData> CallDic = new Dictionary<string, KeyData>();

    public void FrameDelay(float delay, System.Action call)
    {
        StartCoroutine(_FrameDelay(delay, call));
    }
 
    public void NextFrame(Action call)
    {
        StartCoroutine(_NextFrame(call));
    }
    
    IEnumerator _FrameDelay(float delay, Action call)
    {
        yield return new WaitForSeconds(delay);

        call();
    }

    IEnumerator _NextFrame(System.Action call)
    {
        yield return new WaitForEndOfFrame();

        call();
    }


    /// <summary> 새롭게 추가한 함수. key값으로 데이터를 실행 </summary>
    public void KeyDelay(string key, float delay, Action call)
    {
        if (string.IsNullOrEmpty(key) || CallDic.ContainsKey(key))//이미 존제한다면
            return;

        CallDic.Add(key, new KeyData(key, DateTime.Now.AddSeconds(delay), call));
    }

    /// <summary> key값으로 데이터를 삭제 </summary>
    public void RemoveKeyDelay(string key)
    {
        if (!CallDic.ContainsKey(key))
            return;

        CallDic.Remove(key);
    }

    void LateUpdate()
    {
        if (CallDic.Count <= 0)
            return;

        var enume = CallDic.Values.GetEnumerator();
        while(enume.MoveNext() )
        {
            if (DateTime.Now < enume.Current.StartTime)
                continue;

            if (enume.Current.Callback != null)
                enume.Current.Callback();

            CallDic.Remove(enume.Current.Key);
            if (CallDic.Count <= 0)
                return;
        }
    }
}
