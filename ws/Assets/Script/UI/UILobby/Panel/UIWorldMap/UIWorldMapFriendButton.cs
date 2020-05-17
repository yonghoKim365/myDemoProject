using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWorldMapFriendButton : MonoBehaviour 
{
	public UIButton btn;
	public PhotoDownLoader[] face = new PhotoDownLoader[4];
	public UITexture[] faceTexture = new UITexture[4];
	public UISprite[] spFaceFrame = new UISprite[4];

	public UISprite spHard;

	void Awake()
	{
		UIEventListener.Get(gameObject).onClick = onClick;
	}

	Vector3 _v;

	void onClick(GameObject go)
	{
		if(GameManager.me.uiManager.uiMenu.uiWorldMap.nowPlayingWalkAnimation) return;
		if(GameManager.me.uiManager.uiMenu.uiWorldMap.stageClearRewardPopup.gameObject.activeSelf) return;

		if(data.Count == 1)
		{
			GameManager.me.uiManager.uiMenu.uiWorldMap.friendList.gameObject.SetActive(false);

			_v = GameManager.me.uiManager.uiMenu.uiWorldMap.camWorldCamera.WorldToScreenPoint(transform.position);
			GameManager.me.uiManager.uiMenu.uiWorldMap.friendDetailButton.show(data[0], _v);
		}
		else if(data.Count > 1)
		{
			_v = GameManager.me.uiManager.uiMenu.camera.WorldToScreenPoint(transform.position);
			GameManager.me.uiManager.uiMenu.uiWorldMap.friendList.draw(data, _v);
		}
		//EpiServer.instance.sendGetFriendDetail(data[0].userId);
	}


	void setPhoto(int count)
	{
		for(int i = 0; i < 4; ++i)
		{
			face[i].gameObject.SetActive(i < count);
		}

//		yield return new WaitForSeconds(0.1f);

		for(int i = 0; i < count; ++i)
		{
			face[i].init(epi.GAME_DATA.appFriendDic[data[i].userId].image_url);
			face[i].down(epi.GAME_DATA.appFriendDic[data[i].userId].image_url);
		}
	}

	P_FriendDataWorldMapSorter _sort = new P_FriendDataWorldMapSorter();

	public List<P_FriendData> data;

	private int _setPhotoIndex = -1;

	public void setData(List<P_FriendData> fdList)
	{
		data = fdList;

		data.Sort(_sort);

		int count = fdList.Count;

		if(count > 0)
		{
			spHard.enabled = (fdList[0].receivedFP == WSDefine.YES);
		}

		switch(count)
		{
		case 1:
			spFaceFrame[1].gameObject.SetActive(false);
			spFaceFrame[2].gameObject.SetActive(false);
			spFaceFrame[3].gameObject.SetActive(false);

			_setPhotoIndex = 1;

			break;
		case 2:
			spFaceFrame[1].transform.localPosition = new Vector3(12,-12);
			spFaceFrame[1].gameObject.SetActive(true);
			spFaceFrame[2].gameObject.SetActive(false);
			spFaceFrame[3].gameObject.SetActive(false);

			_setPhotoIndex = 2;

			break;
		case 3:

			spFaceFrame[1].transform.localPosition = new Vector3(8,-8);
			spFaceFrame[2].transform.localPosition = new Vector3(16,-16);

			spFaceFrame[1].gameObject.SetActive(true);
			spFaceFrame[2].gameObject.SetActive(true);
			spFaceFrame[3].gameObject.SetActive(false);

			_setPhotoIndex = 3;

			break;
		default:
			spFaceFrame[1].transform.localPosition = new Vector3(4,-4);
			spFaceFrame[2].transform.localPosition = new Vector3(8,-8);
			spFaceFrame[3].transform.localPosition = new Vector3(12,-12);

			spFaceFrame[1].gameObject.SetActive(true);
			spFaceFrame[2].gameObject.SetActive(true);
			spFaceFrame[3].gameObject.SetActive(true);

			_setPhotoIndex = 1;
			break;
		}

		setPhoto(_setPhotoIndex);//StartCoroutine(setPhoto(_setPhotoIndex));
		setPosition(data[0].maxAct, data[0].maxStage);
	}


	public void refreshPhoto()
	{
		setPhoto(_setPhotoIndex);//StartCoroutine(setPhoto(_setPhotoIndex));
	}


	public void refreshInfo()
	{
		if(data.Count > 0)
		{
			spHard.enabled = (data[0].receivedFP == WSDefine.YES);

			UIWorldMapFriendListPanel[] l = GameManager.me.uiManager.uiMenu.uiWorldMap.friendList.GetComponentsInChildren<UIWorldMapFriendListPanel>();

			for(int i = l.Length - 1; i >= 0; --i)
			{
				if(l[i].data != null)
				{
					for(int j = data.Count - 1; j >= 0; --j)
					{
						if(l[i].data.userId == data[j].userId)
						{
							l[i].data = data[j];
							l[i].refresh();
						}
					}
				}
			}
		}
	}



	Vector3 pos = new Vector3();
	void setPosition(int act, int stage)
	{
		pos.x = 1000; pos.y = 151; pos.z = 70.0f;

		switch(act)
		{
		case 1:

			switch(stage)
			{
			case 1: pos.x = -327; pos.y = -183;
				break;
			case 2: pos.x = -384; pos.y = -38;
				break;
			case 3: pos.x = -283; pos.y = 141;
				break;
			case 4: pos.x = -175; pos.y = -52;
				break;
			case 5: pos.x = -28; pos.y = -159;
				break;
			}

			break;
		case 2:

			switch(stage)
			{
			case 1: pos.x = 145; pos.y = -36;
				break;
			case 2: pos.x = 154; pos.y = 155;
				break;
			case 3: pos.x = 316; pos.y = 18;
				break;
			case 4: pos.x = 324; pos.y = -213;
				break;
			case 5: pos.x = 419; pos.y = -138;
				break;
			}

			break;


		case 3:

			switch(stage)
			{
			case 1: pos.x = 615; pos.y = -220;
				break;
			case 2: pos.x = 612; pos.y = 121;
				break;
			case 3: pos.x = 828; pos.y = 0;
				break;
			case 4: pos.x = 737; pos.y = -199;
				break;
			case 5: pos.x = 1003; pos.y = -166;
				break;
			}
			break;


		case 4:
			
			switch(stage)
			{
			case 1: pos.x = 1095; pos.y = -179;
				break;
			case 2: pos.x = 1192; pos.y = -10;
				break;
			case 3: pos.x = 1351; pos.y = 108;
				break;
			case 4: pos.x = 1411; pos.y = -102;
				break;
			case 5: pos.x = 1574; pos.y = -188;
				break;
			}
			break;


		case 5:

			switch(stage)
			{
			case 1: pos.x = 1910; pos.y = -195;
				break;
			case 2: pos.x = 1766; pos.y = 10;
				break;
			case 3: pos.x = 1913; pos.y = 134;
				break;
			case 4: pos.x = 2007; pos.y = -66;
				break;
			case 5: pos.x = 2095; pos.y = -197;
				break;
			}

			break;
		case 6:
			switch(stage)
			{
			case 1: pos.x = 2314; pos.y = -180;
				break;
			case 2: pos.x = 2297; pos.y = 46;
				break;
			case 3: pos.x = 2430; pos.y = -46;
				break;
			case 4: pos.x = 2512; pos.y = -176;
				break;
			case 5: pos.x = 2648; pos.y = 2;
				break;
			}
			break;
		default:

			if(act > GameManager.MAX_ACT)
			{
				pos.x = 2961; pos.y = -180; // act6에 미리 도착해있는 친구 위치. 열리지 않은 해당 액트 중앙에 위치시켜 준다.
			}

			break;
		}

		transform.localPosition = pos;
	}


}
