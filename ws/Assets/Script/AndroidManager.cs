using UnityEngine;
using System.Collections;
 
public class AndroidManager : MonoBehaviour
{
    private static AndroidManager _instance;
 
    public string androidLog = "No Log";

	/*
	 * 
	 * 
	 * http://www.slideshare.net/williamyang3910/unitekorea2013-protecting-your-android-content-21713675

// Add code to print out the key hash
  try {
  PackageInfo info = getPackageManager().getPackageInfo("com.yourpckg.yourname", PackageManager.GET_SIGNATURES);
  for (Signature signature : info.signatures) {
  MessageDigest md = MessageDigest.getInstance("SHA");
  md.update(signature.toByteArray());
  Log.e("MY KEY HASH:", Base64.encodeToString(md.digest(), Base64.DEFAULT));
      }
  } catch (NameNotFoundException e) {

  } catch (NoSuchAlgorithmException e) {

  }
  
#if UNITY_ANDROID && !UNITY_EDITOR	
	
    // 유니티가 동작하는 액티비티를 저장하는 변수
    public AndroidJavaObject activity;
 
    void Awake()
    {
        // 현재 실행 중인 유니티 액티비티를 가져와서 변수에 저장
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }
 
    // 자바 클래스로부터 전달받은 로그를 보여줍니다.
    void AndroidLog(string newAndroidLog)
    {
        androidLog = newAndroidLog;
    }
 
    public static AndroidManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(AndroidManager)) as AndroidManager;
                if (_instance == null)
                {
                    _instance = new GameObject("AndroidManager").AddComponent<AndroidManager>();
                }
            }
 
            return _instance;
        }
    }
	
#endif

*/
	
}