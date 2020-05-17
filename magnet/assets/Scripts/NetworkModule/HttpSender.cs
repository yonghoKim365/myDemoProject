using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

internal sealed class WWWData
{
    public bool retry = true;
    public WWW www;
    public string url;
    
    public int retryCount;
    public Action<string> recvFunc;
    public Action<string> failFunc;
}

public class HttpSender : MonoSingleton<HttpSender>
{
    private const int MAX_RETRY = 2;

    public void Get(string param, Action<string> recvFunc, Action<string> failFunc, bool retry = true)
    {
        WWWData wwwData = new WWWData();
        wwwData.retryCount = MAX_RETRY;
        wwwData.retry = retry;
        wwwData.recvFunc = recvFunc;
        wwwData.failFunc = failFunc;
        wwwData.www = new WWW(param);
        StartCoroutine(WaitForRequest(wwwData));
    }

    private IEnumerator WaitForRequest(WWWData wwwdata)
    {
        yield return wwwdata.www;
        // check for errors 

        if (wwwdata.www.error == null)
        {
            //Variant jsonstring = JSON.Load(wwwdata.www.text);
            wwwdata.recvFunc(wwwdata.www.text);
        }
        else
        {
            if (wwwdata.retry && wwwdata.retryCount > 0)
            {
                wwwdata.retryCount--;
                //Debug.Log("WWW Error: " + wwwdata.www.error + " Retry Count:" + wwwdata.retryCount);
                StartCoroutine(WaitForRequest(wwwdata));
            }
            else
            {
                //Debug.Log("WWW Error: " + wwwdata.www.error + " Retry Fail");
                wwwdata.failFunc(wwwdata.www.error);
            }
        }
    }    
}
