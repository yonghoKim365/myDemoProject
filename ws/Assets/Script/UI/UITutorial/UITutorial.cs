using UnityEngine;
using System.Collections;

public class UITutorial : MonoBehaviour 
{
	public TutorialEndCircleEffect tutorialEndCircleEffect;


	public Transform dim;
	public UISprite spDim;
	public UIPopupTutorial popupTutorial;

	public Transform verticalDim;
	public UISprite spVerticalDim;

	public UITutorialSkillGuide skillGuide;

	public GameObject goDialogPanel, goBigDialogPanel;

	public UILabel lbDialog, lbPopupText, lbBigDialog;

	public UIButton btnDialog, btnDialogYes, btnDialogNo, btnBigDialog, btnOKDialog;

	public GameObject goDialogYesNoButtonContainer;

	public UISprite spBtn;

	public UISprite spBackground;

	public UITutorialRewardPopup rewardPopup;


	public UISprite[] spPrepareRewardIcon;
	public UILabel[] lbPrepareRewardLabel;
	public UILabel lbPrepareRewardSpecial;
	public UISprite spPrepareRewardSpecialCover;


	public void showBigSizeCharacter()
	{
		goDialogPanel.gameObject.SetActive(false);
		goBigDialogPanel.gameObject.SetActive(true);
		_nextButtonVisible = true;
	}


	Vector3 _v;

	public enum ButtonType
	{
		None, Continue, Close
	}

	void Awake()
	{
		UIEventListener.Get(btnDialog.gameObject).onClick = onClickDialog;
		UIEventListener.Get(btnDialogYes.gameObject).onClick = onClickDialog;
		UIEventListener.Get(btnDialogNo.gameObject).onClick = onClickDialog;

		UIEventListener.Get(btnBigDialog.gameObject).onClick = onClickDialog;

		UIEventListener.Get(btnOKDialog.gameObject).onClick = onClickDialog;

		hide();
	}


	public void showDim(bool isVisible)
	{
		spBackground.gameObject.SetActive(false);
		gameObject.SetActive(true);
		dim.gameObject.SetActive(isVisible);
		verticalDim.gameObject.SetActive(false);
	}

	public void setDimPosition(float x, float y, float alpha = 20.0f)
	{
		showDim(true);
		gameObject.SetActive(true);
		_v = dim.localPosition;
		_v.x = x;
		_v.y = y;
		dim.localPosition = _v;
		spDim.color = new Color(1,1,1,alpha * 0.01f);
	}

	public void openDialog(string textId, float posX, float posY)
	{
		gameObject.SetActive(true);
		goDialogPanel.SetActive(true);
		lbDialog.text = Util.getText(textId);
		lbBigDialog.text = lbDialog.text;
		_v = goDialogPanel.transform.localPosition;
		_v.x = posX;
		_v.y = posY;
		goDialogPanel.transform.localPosition = _v;
		if(dim.gameObject.activeSelf == false) spBackground.gameObject.SetActive(true);
		btnDialog.gameObject.SetActive(false);
		btnBigDialog.gameObject.SetActive(false);
		goDialogYesNoButtonContainer.SetActive(false);

		yesAction = null;
		noAction = null;
	}


	public void setYesNoCallback(PopupData.PopupAction yes, PopupData.PopupAction no)
	{
		yesAction = yes;
		noAction = no;
	}


	string _renderText = "";
	int _renderTextIndex = -1;
	int _renderTextLength = -1;
	bool _nextButtonVisible = false;
	public void openTutorialDialog(float posX, float posY, bool nextButtonVisible, string id, int step, params string[] strs)
	{
		openTutorialDialog(posX, posY, Util.getTutorialText(id,step,strs),nextButtonVisible);

		SoundManager.instance.playTutorialVoice(step);
	}

	public void openTutorialDialog(float posX, float posY, string text, bool nextButtonVisible)
	{
		if(goDialogPanel.activeSelf) blockCloseDialog = true;
		gameObject.SetActive(true);
		goDialogPanel.SetActive(true);
		lbDialog.text = "";
		lbBigDialog.text = "";
		_renderText = text;
		
		_v = goDialogPanel.transform.localPosition;
		_v.x = posX;
		_v.y = posY;
		goDialogPanel.transform.localPosition = _v;
		if(dim.gameObject.activeInHierarchy == false)
		{
			spBackground.gameObject.SetActive(true);
		}

		setDialogButtonText(true);
		btnDialog.gameObject.SetActive(false);
		btnBigDialog.gameObject.SetActive(false);

		goDialogYesNoButtonContainer.SetActive(false);
		_renderTextIndex = 0;
		_renderTextLength = _renderText.Length;
		_nextButtonVisible = nextButtonVisible;
	}



	void FixedUpdate()
	{
		if(_renderTextIndex > -1 && _renderTextIndex <= _renderTextLength)
		{
			if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
			{
				_renderTextIndex = _renderTextLength;

				if(Input.GetMouseButtonUp(0) == false)
				{
					TutorialManager.instance.skipThisTime = true;
				}
//				if(Input.GetMouseButtonUp(0))
//				{
//					if(_nextButtonVisible)
//					{
//						btnDialog.gameObject.SetActive(true);
//						_nextButtonVisible = false;
//					}
//				}
			}

			string str = _renderText.Substring(0,_renderTextIndex);

			int colorStartIndex = str.LastIndexOf("[");
			int colorLastIndex = str.LastIndexOf("]");

			++_renderTextIndex;
			if(_renderTextIndex > _renderTextLength)
			{
				_renderTextIndex = -1;

				if(_nextButtonVisible)
				{
					btnDialog.gameObject.SetActive(true);
					btnBigDialog.gameObject.SetActive(true);
					_nextButtonVisible = false;
				}
			}

			if(colorStartIndex >= 0 && colorLastIndex < colorStartIndex)
			{
				FixedUpdate();
			}
			else
			{
				lbDialog.text = str;
				lbBigDialog.text = str;
			}
		}
	}





	public void setDialogButtonText(bool isConinue)
	{
		if(isConinue) spBtn.spriteName = "img_text_tutorial_next";
		else spBtn.spriteName = "img_title_close2";
	}


	public bool blockCloseDialog = false;




	void onClickDialog(GameObject go)
	{
		gameObject.SetActive(true);
		if(blockCloseDialog == false)
		{
			goDialogPanel.SetActive(false);
			goBigDialogPanel.SetActive(false);
		}
		_renderTextIndex = -1;
	}

	PopupData.PopupAction yesAction;
	PopupData.PopupAction noAction;

	void onClickYes(GameObject go)
	{
		gameObject.SetActive(true);
		if(blockCloseDialog == false) goDialogPanel.SetActive(false);
		_renderTextIndex = -1;
		if(yesAction != null) yesAction();

	}

	void onClickNo(GameObject go)
	{
		gameObject.SetActive(true);
		if(blockCloseDialog == false) goDialogPanel.SetActive(false);
		_renderTextIndex = -1;
		if(noAction != null) noAction();

	}




	public void hideDialog()
	{
		_renderTextIndex = -1;

		//if(GameManager.me != null) SoundManager.instance.stopTutorialVoice();

		goDialogPanel.SetActive(false);
		spBackground.gameObject.SetActive(false);
		goBigDialogPanel.SetActive(false);
		btnOKDialog.gameObject.SetActive(false);
	}


	public void openTutorialPopup(string textId, float posX, float posY)
	{
		gameObject.SetActive(true);
		lbPopupText.text = Util.getText(textId);
		_v = popupTutorial.transform.localPosition;
		_v.x = posX;
		_v.y = posY;
		popupTutorial.transform.localPosition = _v;
		popupTutorial.show();
	}

	public Transform tfArrow;
	public Transform tfArrowCircle;
	public Transform tfHand;

	public void showHand(float posX, float posY)
	{
		gameObject.SetActive(true);
		_v = tfHand.localPosition;
		_v.x = posX;
		_v.y = posY;
		tfHand.localPosition = _v;
		tfHand.gameObject.SetActive(true);
	}

	public void hideHand()
	{
		tfHand.gameObject.SetActive(false);
	}


	public UISprite spArrow;

	Quaternion _q;


	public void setArrowAndDim(float posX, float posY, bool isDown = true)
	{
		showArrow(posX, posY, isDown);
		setDimPosition(posX, posY + (isDown?-80:+50));
	}


	public void showArrow(float posX, float posY, bool isDown = true)
	{
		gameObject.SetActive(true);
		_v = tfArrow.localPosition;
		_v.x = posX;
		_v.y = posY;

		if(isDown == false)
		{
			_v.y -= 120.0f;
			tfArrow.localPosition = _v;

			_v = tfArrowCircle.localPosition;
			_v.y = -122.0f;
			tfArrowCircle.localPosition = _v;

		}
		else
		{
			_v.y -= 20.0f;
			tfArrow.localPosition = _v;

			_v = tfArrowCircle.localPosition;
			_v.y = -351.0f;
			tfArrowCircle.localPosition = _v;
		}


		spArrow.gameObject.SetActive(true);
		tfArrow.gameObject.SetActive(true);



		_q = spArrow.transform.localRotation;

		if(isDown)
		{
			_v.x = 0; _v.y = 0; _v.z = 0;
		}
		else
		{
			_v.x = 0; _v.y = 0; _v.z = 180;
		}

		_q.eulerAngles = _v;
		spArrow.transform.localRotation = _q;
	}
	
	public void hideArrow()
	{
		tfArrow.gameObject.SetActive(false);
	}

	public void hideArrowWithoutCircle()
	{
		spArrow.gameObject.SetActive(false);
	}


	public void hide()
	{
		_nextButtonVisible = false;
		_renderTextIndex = -1;
		hideArrow();
		hideHand();
		popupTutorial.hide();
		hideDialog();
		gameObject.SetActive(false);
		showDim(false);
		spBackground.gameObject.SetActive(false);

		goBigDialogPanel.SetActive(false);

		skillGuide.gameObject.SetActive(false);

		if(rewardPopup.gameObject.activeSelf) rewardPopup.hide();

		verticalDim.gameObject.SetActive(false);
		yesAction = null;
		noAction = null;

		TutorialManager.instance.skipThisTime = false;
	}


	public void showVerticalDim(Vector3 pos, int size = 1866)
	{
		verticalDim.gameObject.SetActive(true);
		verticalDim.localPosition = pos;
		spVerticalDim.width = size;
	}


}
