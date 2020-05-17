using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebViewMgr : MonoBehaviour 
{
    static List<WebViewObj> WebViews = new List<WebViewObj>();

    public static void CreateWebViewMgr(string url, WebViewPosData data, System.Action<UniWebView> EndCB)
    {
        GameObject obj = Instantiate(new GameObject()) as GameObject;
        WebViewObj newObj = obj.AddComponent<WebViewObj>();
        newObj.Setup(url, data, EndCB);
        WebViews.Add(newObj);
    }

    public static void Clear()
    {
        for(int i=0; i<WebViews.Count; i++)
            Destroy(WebViews[i].gameObject);

        WebViews.Clear();
    }

    //void OnGUI()
    //{
        //if (GUI.Button(new Rect(0, 0, 200, 200), "Open"))
        //{
        //    WebViewMgr.CreateWebViewMgr("http://ts-img.four33.co.kr:10011/_test/Agreement/999/999/999/lostk1.html",
        //        new WebViewPosData(
        //            (int)(UniWebViewHelper.screenHeight * 0.15f),
        //            (int)(UniWebViewHelper.screenWidth * 0.02f), 
        //            (int)(UniWebViewHelper.screenHeight * 0.2f),
        //            (int)(UniWebViewHelper.screenWidth * 0.51f)));

        //    WebViewMgr.CreateWebViewMgr("http://ts-img.four33.co.kr:10011/_test/Agreement/999/999/999/lostk1.html",
        //        new WebViewPosData(
        //            (int)(UniWebViewHelper.screenHeight * 0.15f),
        //            (int)(UniWebViewHelper.screenWidth * 0.51f), 
        //            (int)(UniWebViewHelper.screenHeight * 0.2f),
        //            (int)(UniWebViewHelper.screenWidth * 0.02f)));
        //}
    //}
}
