using UnityEngine;
using System.Collections;

public class UIBigSizePopup : UISystemPopupBase 
{
	public PhotoDownLoader photo;

	public UILabel lbText;

	public UIScrollView scrollView;

	public GameObject spLoading;

	protected override void awakeInit ()
	{
		base.awakeInit ();
		photo.isLockMemoryUnload = true;
	}

	bool _isImageType = false;

	public override void show (PopupData pd, string msg)
	{
		base.show (pd, "");

		scrollView.ResetPosition();

		if( string.IsNullOrEmpty(msg) == false && msg.Length > 5 && (msg.Substring(0,4).ToLower().StartsWith("http") ))
		{
			_isImageType = true;
			photo.gameObject.SetActive(true);
			lbText.enabled = false;

			spLoading.gameObject.SetActive( true );

			StartCoroutine(startDownload(msg));
		}
		else
		{
			spLoading.gameObject.SetActive( false );

			_isImageType = false;
			photo.gameObject.SetActive(false);
			lbText.enabled = true;
			lbText.text = msg;
		}

		btnYes.gameObject.SetActive(string.IsNullOrEmpty(pd.closeLink) == false);
	}


	IEnumerator startDownload(string msg)
	{
		float timeLimit = 0;
		if(useScaleTween && ani != null )
		{
			while(ani.isPlaying && timeLimit < 1.5f)
			{
				timeLimit += 0.1f;
				yield return Util.ws01;
			}
		}
		yield return Util.ws01;

		photo.init(msg);
		photo.down(msg);

		float timeout = 10.0f;
		while(timeout > 0 && ( photo.mainTexture == null || photo.mainTexture.enabled == false ) )
		{ 
			timeout -= 0.05f;
			yield return new WaitForSeconds(0.05f);
		}

		spLoading.gameObject.SetActive( false );
	}






}
