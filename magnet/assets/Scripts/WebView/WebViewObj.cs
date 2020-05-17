using UnityEngine;
using System.Collections;

public class WebViewPosData
{
    public WebViewPosData(int _top, int _left, int _bottom, int _right)
    {
        top = _top;
        left = _left;
        bottom = _bottom;
        right = _right;
    }

    public int top;
    public int left;
    public int bottom;
    public int right;
}

public class WebViewObj : MonoBehaviour {

    private UniWebView _webView;
    System.Action<UniWebView> EndCB;

    WebViewPosData CreatePos;
    public void Setup(string url, WebViewPosData _data, System.Action<UniWebView> _EndCB)
    {
        EndCB = _EndCB;

        _webView = this.gameObject.AddComponent<UniWebView>();
        _webView.OnReceivedMessage += OnReceivedMessage;
        _webView.OnLoadComplete += OnLoadComplete;
        _webView.OnWebViewShouldClose += OnWebViewShouldClose;
        _webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;

        _webView.InsetsForScreenOreitation += InsetsForScreenOreitation;

        //< URL 설정
        CreatePos = _data;
        _webView.url = url;

        //< 기기의 백키로 꺼지는걸 막기위함
        _webView.backButtonEnable = false;
        _webView.toolBarShow = false;

        //< 로드
        _webView.Load();
    }

    public void ChangeUrl(string url)
    {
        _webView.url = url;
        _webView.Load();
    }

    //< 처음 셋팅하는부분(UI)
    UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
    {
        if (orientation == UniWebViewOrientation.Portrait)
        {
            return new UniWebViewEdgeInsets(CreatePos.top, CreatePos.left, CreatePos.bottom, CreatePos.right);
        }
        else
        {
            return new UniWebViewEdgeInsets(CreatePos.top, CreatePos.left, CreatePos.bottom, CreatePos.right);
        }
    }

    //< 로드가 완료되었을시 콜백
    void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (success)
        {
            webView.Show();

            if (EndCB != null)
                EndCB(_webView);
        }
        else
        {
            Debug.Log("Something wrong in webview loading: " + errorMessage);
        }
    }

    //< 웹뷰에서 입력받았을시 받는 메시지콜백
    void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        Debug.Log(message.rawMessage);
    }

    void OnEvalJavaScriptFinished(UniWebView webView, string result)
    {
        Debug.Log("js result: " + result);
    }

    bool OnWebViewShouldClose(UniWebView webView)
    {
        if (webView == _webView)
        {
            _webView = null;
            return true;
        }
        return false;
    }
}
