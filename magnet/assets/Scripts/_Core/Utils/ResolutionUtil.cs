using UnityEngine;
using System.Collections;

public class ResolutionUtil : MonoBehaviour
{
    public bool donDestroy = false;
    public Resolution originalRes;
    public Camera blackCamera;

    bool isShow = false;

    void Awake()
    {
        if (donDestroy)
            DontDestroyOnLoad(gameObject);

        originalRes = Screen.currentResolution;
    }

    void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUILayout.Label("현재 해상도 : " + Screen.currentResolution.width + " : " + Screen.currentResolution.height, GUILayout.ExpandWidth(true));
        if (Camera.main)
            GUILayout.Label("MainCamRect : " + Camera.main.rect, GUILayout.ExpandWidth(true));

        isShow = GUILayout.Toggle(isShow, "Resolution Control" , GUILayout.Width(200), GUILayout.Height(50f));

        if (!isShow)
            return;

        Rect guiPos = new Rect(0, 100, 200, 50);
        if (GUI.Button(guiPos, "Original"))
        {
            Screen.SetResolution(originalRes.width, originalRes.height, true);
        }

        guiPos.y += 50;
        if (GUI.Button(guiPos, "16:9"))
        {
            int calcHeight = (int)((9f / 16f) * (float)originalRes.width);
            Screen.SetResolution(originalRes.width, calcHeight, true);
            Debug.LogWarning(originalRes.width + " : " + calcHeight);
        }

        guiPos.y += 50;
        if (GUI.Button(guiPos, "4:3"))
        {
            int calcWidth = (int)((4f / 3f) * (float)originalRes.height);
            Screen.SetResolution(calcWidth, originalRes.height, true);
            Debug.LogWarning(calcWidth + " : " + originalRes.height);
        }

        guiPos.y += 50;
        if (GUI.Button(guiPos, "AdjustCam 1280:720"))
        {
            AdjuestCamRect(Camera.allCameras, 1280f, 720f);
        }
        guiPos.y += 50;
        if (GUI.Button(guiPos, "AdjustCam 1280:800"))
        {
            AdjuestCamRect(Camera.allCameras, 1280f, 800f);
        }
        guiPos.y += 50;
        if (GUI.Button(guiPos, "AdjustCam 1024:768"))
        {
            AdjuestCamRect(Camera.allCameras, 1024f, 768f);
        }
        guiPos.y += 50;
        if (GUI.Button(guiPos, "Add BlackCam"))
        {
            if (null != blackCamera)
                DestroyImmediate(blackCamera.gameObject);

            blackCamera = CreateBlackCamera(-3);
        }
        guiPos.y += 50;
        if (GUI.Button(guiPos, "ResetAll"))
        {
            if (null != blackCamera)
                DestroyImmediate(blackCamera.gameObject);

            AdjuestCamRect(Camera.allCameras, Screen.width, Screen.height);
        }

        guiPos.y += 50;
        if (GUI.Button(guiPos, "1/2"))
        {
            Screen.SetResolution(originalRes.width / 2, originalRes.height / 2, true);
        }
        guiPos.y += 50;
        if (GUI.Button(guiPos, "1/3"))
        {
            Screen.SetResolution(originalRes.width / 3, originalRes.height / 3, true);
        }
        guiPos.y += 50;
        if (GUI.Button(guiPos, "1/4"))
        {
            Screen.SetResolution(originalRes.width / 4, originalRes.height / 4, true);
        }
    }

    /// <summary>
    /// 카메라 Viewport 설정
    /// </summary>
    /// <param name="camera">설정할 대상 카메라</param>
    /// <param name="targetAspect">바꾸려는 화면 비율</param>
    public static float AdjuestCamRect(Camera camera, float targetWidth = 1280f, float targetHeight = 720f)
    {
        // 16 : 9 = 1.7777
        // 16 : 10 = 1.6
        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;

        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        return scaleHeight;
    }

    public static float AdjuestCamRect(Camera[] cameras, float targetWidth = 1280f, float targetHeight = 720f)
    {
        float scaleHeight = 0f;
        foreach (Camera cam in cameras)
        {
            if (null != cam)
                scaleHeight = AdjuestCamRect(cam, targetWidth, targetHeight);
        }

        return scaleHeight;
    }

    public static Camera CreateBlackCamera(int depth = -10, bool dontDestroy = true)
    {
        GameObject backCamObj = new GameObject("BlackCamera", typeof(Camera));

        backCamObj.camera.clearFlags = CameraClearFlags.SolidColor;
        backCamObj.camera.backgroundColor = Color.black;
        backCamObj.camera.cullingMask = 0;
        backCamObj.camera.depth = depth;

        if (dontDestroy)
            DontDestroyOnLoad(backCamObj);

        return backCamObj.camera;
    }
}
