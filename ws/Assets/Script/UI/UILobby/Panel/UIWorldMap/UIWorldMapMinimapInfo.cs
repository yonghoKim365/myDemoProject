using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWorldMapMinimapInfo : MonoBehaviour 
{
	public UISprite spSimbol;

	Stack<GameObject> list = new Stack<GameObject>();
	
	public void hide()
	{
		while(list.Count > 0)
		{
			GameObject.Destroy( list.Pop() );
		}
		list.Clear();
	}


	public GameObject goStartPoint;
	public GameObject goEndPoint;

	float mapStartX;
	float mapEndX;
	float startX;
	float width;

	private Vector3 _v;
	public void init(  string type, float xPos)
	{
		UISprite sp = Instantiate(spSimbol) as UISprite;
		sp.gameObject.SetActive(true);
		sp.spriteName = type;
		sp.MakePixelPerfect();
		sp.transform.parent = transform;
		_v = goStartPoint.transform.position;
		_v.x = startX + width * ((xPos - mapStartX)/(mapEndX - mapStartX));
		sp.transform.position = _v;
		list.Push(sp.gameObject);
	}

	public void setStage(RoundData rd)
	{
		startX = goStartPoint.transform.position.x;
		width = goEndPoint.transform.position.x - goStartPoint.transform.position.x;

		mapStartX = rd.mapStartEndPosX[0];
		mapEndX = rd.mapStartEndPosX[1];

		init(CharacterMinimapPointer.PLAYER, rd.playerStartPosX);
		

		if(rd.heroMonsters.Length > 0)
		{
			foreach(StageMonsterData data in rd.heroMonsters)
			{
				if(data.attr == null)
				{
					init(CharacterMinimapPointer.E_HERO,data.posX);
				}
			}
		}

		if(rd.unitMonsters != null)
		{
			foreach(StageMonsterData data in rd.unitMonsters)
			{
				init(CharacterMinimapPointer.E_UNIT, data.posX);
			}		
		}

		if(rd.decoObject != null)
		{
			foreach(StageMonsterData data in rd.decoObject)
			{
				_v.x = data.posX;
				init(CharacterMinimapPointer.OBJECT, data.posX);
			}				
		}


		if(rd.blockObject != null)
		{
			foreach(StageMonsterData data in rd.blockObject)
			{
				if(data.attr == "0" || data.attr == "1") // 적.
				{
					init(CharacterMinimapPointer.DESTROY, data.posX);
				} 
				else // 아군.
				{
					init(CharacterMinimapPointer.OBJECT, data.posX);
				}
			}	
		}

		
		else if(rd.mode == RoundData.MODE.ARRIVE)
		{
			if(rd.chaser != null)
			{
				init(CharacterMinimapPointer.CHASER, rd.chaser.posX);
			}	
			
			if(rd.protectObject != null)
			{
				init(CharacterMinimapPointer.PROTECT, rd.playerStartPosX - 150.0f);
			}

			init(CharacterMinimapPointer.DISTANCE, rd.targetPos);
		}
		else if(rd.mode == RoundData.MODE.PROTECT)
		{
			foreach(StageMonsterData data in rd.protectObject)
			{
				init(CharacterMinimapPointer.PROTECT, data.posX);
			}
		}
		else if(rd.mode == RoundData.MODE.DESTROY)
		{
			if(rd.chaser != null)
			{
				init(CharacterMinimapPointer.CHASER, rd.chaser.posX);
			}
			
			foreach(StageMonsterData data in rd.destroyObject)
			{
				init(CharacterMinimapPointer.DESTROY, data.posX);
			}
		}
	}
}
