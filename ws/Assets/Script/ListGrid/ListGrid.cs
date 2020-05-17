using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ListGrid : MonoBehaviour {

	Vector3 _v;

	////// 페이지 드래그용 
	// 드래그가 끝난후...
	public bool moveAfterDragFinish = false;
	public float pageMoveStrength = 20.0f;
	public float pageMoveOffset = 70.0f;


	// 페이지 단위 슬롯들은 그 라인안에 또 아이템을 포함할 수 있다. 이걸 방지하기 위함.
	// 체크 되어있으면 풀링을 할때 하위 자식들은 제외한다.
	public bool checkItemPoolChild = false;

	public int upperDummyNum = 1;

	public enum Direction
	{
		vertical, horizontal
	}

	public Direction direction = Direction.vertical;

	public int itemPerLine = 1;

	public float xOffset = 0.0f;
	public float yOffset = 0.0f;

	public float cellWidth = 100.0f;
	public float cellHeight = 100.0f;

	// 재료가 될 프리펩.
	public UIListGridItemPanelBase listItem;

	public List<UIListGridItemPanelBase> itemList = new List<UIListGridItemPanelBase>();

	public List<object> dataList;
	public int dataCount = 0;

	public UIPanel panel = null;

	public SpringPanel springPanel = null;

	Transform panelTf;

	private float panelPrevPosition = 0;

	public int dummyNum = 2;

	private enum ListDrawStatus
	{
		idle, start
	}
	
	ListDrawStatus _listDrawStatus = ListDrawStatus.idle;
	
	public UIScrollView dragScrollView = null;


	public Callback.Int callbackAfterDrawFinalItem = null;


	void Awake()
	{
		dragScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);

		if(dragScrollView != null && moveAfterDragFinish)
		{
			dragScrollView.onCustomDragFinished = onDragFinished;
		}

		panel = NGUITools.FindInParents<UIPanel>(gameObject);
		panelTf = panel.transform;
	}

	void OnEnable()
	{
		dragScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);	

		if(dragScrollView != null && moveAfterDragFinish)
		{
			dragScrollView.onCustomDragFinished = onDragFinished;
		}

		if(panel == null)
		{
			panel = NGUITools.FindInParents<UIPanel>(gameObject);
			panelTf = panel.transform;
		}

		if(springPanel == null && panel != null)
		{
			springPanel = panel.GetComponent<SpringPanel>();
		}
	}


	void onDragFinished ()
	{
		dragScrollView.DisableSpring();

		// 일단 좌우 페이지 이동만 만들어본다...
		_v = dragScrollView.transform.localPosition;

		int nowIndex = checkFirstIndex();

		float f = _v.x % cellWidth;

//		Debug.LogError("_v: " + _v + "    f: " + f + "   " + _panelMoveDirection);
//
		if(_v.x < -(dataCount - 1) * cellWidth - 20.0f) return;
		else if(_v.x > 5) return;

		if(_panelMoveDirection == MoveDirection.Left)
		{
			if(f < -pageMoveOffset)
			{
				_v.x -= (cellWidth + f);
			}
			else
			{
				_v.x -= f;
			}

		}
		else if(_panelMoveDirection == MoveDirection.Right)
		{
			f = cellWidth + f;

			if(f < pageMoveOffset)
			{
				_v.x -= f;
			}
			else
			{
				_v.x += cellWidth - f;
			}
		}


		springPanel = SpringPanel.Begin(dragScrollView.gameObject, _v, pageMoveStrength);
	}


	void OnDisable()
	{
		if(springPanel == null && panel != null)
		{
			springPanel = panel.GetComponent<SpringPanel>();
		}
	}

	// 화면에 그릴 최대 아이템 숫자.
	// 그릴 아이템 데이터가 화면보다 작으면 그대로.
	// 넘치면 화면을 꽉 채울만큼의 양만 그린다.
	public int totalItemNumber = 0;


	private int _index = 0;
	public int index
	{
		get{return _index;}
		set{
			if(_index == value)return;

//#if UNITY_EDITOR
//			Debug.Log("set index: " + value);
//			Debug.Log("dataList.Count: " + dataCount);
//			Debug.Log("itemList.Count: " + itemList.Count);
//#endif

			if(value<0)
			{
				return;
			}

			if(value > dataCount)// - itemList.Count + dummyNum)
			{
				return;
			}

			if(value > dataCount - itemList.Count)
			{
				value= dataCount - itemList.Count;
				if(value < 0) value = 0;
			}

			_index = value;
		}
	}



	// 리스트에 자료를 넣고 최초 1번 그린다.
	void reposition(bool isResetPosition)
	{
		index = checkFirstIndex();

//		Debug.Log("reposition index " + index);

		int i= 0;

		foreach(UIListGridItemPanelBase item in itemList)
		{
			_v = item.cachedTransform.localPosition;

			if(direction == Direction.vertical)
			{
				_v.x = 0.0f; 
				_v.y = -(index+i)*cellHeight; 
				_v.z = 0.0f;
			}
			else
			{
				_v.x = (index+i)*cellWidth; 
				_v.y = 0.0f; 
				_v.z = 0.0f;
			}

			item.cachedTransform.localPosition = _v;
			setItemData(item);
			++i;
		}

		StartCoroutine( updateItemPositionRightNow(isResetPosition) );



		needAllImageLoading = true;
	}


	IEnumerator updateItemPositionRightNow(bool isResetPosition)
	{
		if(itemList == null || dataList == null) yield break;
		if(panel == null)yield break;
		
		float startPosition = (direction == Direction.vertical)?panelTf.localPosition.y:panelTf.localPosition.x;
		
		int itemCount = itemList.Count;


		int redrawCount = 1;

		if(isResetPosition == false)
		{
			dragScrollView.DisableSpring();

			if(direction == Direction.vertical)
			{
				float itemBottomLine = startPosition - (dataCount * cellHeight);

				// 다시 그린 위치의 가장 하단이 실제 패널 그릴 영역의 맨 아래보다 높으면 아래로 내려서 맞춰주어야한다.
				if(itemBottomLine > -dragScrollView.panel.finalClipRegion.w)
				{
					_v.x = 0; _v.z = 0;
					_v.y = -dragScrollView.panel.finalClipRegion.w - itemBottomLine;
					dragScrollView.MoveRelative(_v);
				}
				else if(startPosition > 2)
				{
					_v.x = 0; _v.y = -2; _v.z = 0;
					dragScrollView.MoveRelative(_v);
				}
			}
			else
			{
				
			}

			// 위치를 다시 잡지 않고 그냥 그릴때는 오류를 방지하기위해 추가로 몇번 더 간격을 주며 다시 그려준다.
			++redrawCount;
		}

		yield return null;



		for(int i = 0; i < redrawCount ; ++i)
		{
			startPosition = (direction == Direction.vertical)?panelTf.localPosition.y:panelTf.localPosition.x;
			
			foreach(UIListGridItemPanelBase item in itemList)
			{
				if(direction == Direction.vertical)
				{
					if(item.cachedTransform.localPosition.y + startPosition > cellHeight)
					{
						//					Debug.LogError("item.cachedTransform.localPosition.y + startPosition > cellHeight : " + (item.cachedTransform.localPosition.y + startPosition > cellHeight));
						
						if(item.cachedTransform.localPosition.y - (cellHeight*itemCount) > -(dataCount)*cellHeight)
						{
							//						Debug.Log("item.cachedTransform.localPosition.y - (cellHeight*itemCount) > -(dataCount)*cellHeight : " + (item.cachedTransform.localPosition.y - (cellHeight*itemCount) > -(dataCount)*cellHeight));
							
							_v.x = 0;
							_v.y = item.cachedTransform.localPosition.y - (cellHeight*itemCount);
							_v.z = 0;
							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
					
					if(item.cachedTransform.localPosition.y + startPosition < -cellHeight*(itemCount-(dummyNum-1)) )
					{
						//					Debug.Log("item.cachedTransform.localPosition.y + startPosition < -cellHeight*(itemCount-(dummyNum-1))  : " + (item.cachedTransform.localPosition.y + startPosition < -cellHeight*(itemCount-(dummyNum-1)) ));
						
						if(item.cachedTransform.localPosition.y + (cellHeight*itemCount) <= 0)
						{
							//						Debug.Log("item.cachedTransform.localPosition.y + (cellHeight*itemCount) <= 0 : " + (item.cachedTransform.localPosition.y + (cellHeight*itemCount) <= 0));
							
							_v.x = 0;
							_v.y = item.cachedTransform.localPosition.y + (cellHeight*itemCount);
							_v.z = 0;
							
							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
					
				}
				else
				{
					
					if(item.cachedTransform.localPosition.x + startPosition > cellWidth*(itemCount-(dummyNum-1)))
					{
						if(item.cachedTransform.localPosition.x - (cellWidth*itemCount) >= 0)
						{
							_v.x = item.cachedTransform.localPosition.x - (cellWidth*itemCount);
							_v.y = 0;
							_v.z = 0;
							
							//						Debug.Log(_v);
							
							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
					
					if(item.cachedTransform.localPosition.x + startPosition < -cellWidth)
					{
						if(item.cachedTransform.localPosition.x + (cellWidth*itemCount) < (dataCount)*cellWidth)
						{
							_v.x = item.cachedTransform.localPosition.x + (cellWidth*itemCount);
							_v.y = 0;
							_v.z = 0;
							
							//						Debug.Log(_v);
							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
					
				}
			}

			yield return Util.ws01;

		}

	}
	


	private UILabel endLineDummy = null;


	private bool _firstResetDraw = false;

	// 최초 진입점.
	public void setData(UIScrollView scrollView, List<object> list,bool isResetPos, int startIndex = -1)
	{
		dataCount = 0;

		_firstResetDraw = isResetPos;

		if(gameObject.activeInHierarchy == false)
		{
			Debug.LogError("Active GameObject First!");
			return;
		}

		if(listItem==null)
		{
			Debug.LogError("listItem is Null");
			return;
		}

		itemList.Clear();
		dataList = list;
		dataCount = dataList.Count;

		itemClear();

		if(isResetPos)
		{
			panel.transform.localPosition = Vector3.zero;
			panel.clipOffset = Vector2.zero;

			if(springPanel != null)
			{
				springPanel.target = Vector3.zero;
				springPanel.enabled = false;
			}
		}

		if(direction == Direction.vertical) panelPrevPosition = panelTf.localPosition.y;
		else panelPrevPosition = panelTf.localPosition.x;

		int itemMax;
		
		if(direction == Direction.vertical)
		{
			itemMax = Mathf.CeilToInt(panel.finalClipRegion.w/cellHeight)+dummyNum;
		}
		else
		{
			itemMax = Mathf.CeilToInt(panel.finalClipRegion.z/cellWidth)+dummyNum;
		}
		
		totalItemNumber = Mathf.Min(itemMax, dataCount);

		createItemPanel();

		if(isResetPos)
		{
			_index = 0;
			scrollView.ResetPosition();
		}

		if(startIndex > -1)
		{
			if(direction == Direction.vertical)
			{
				scrollView.MoveAbsolute(new Vector3(0,cellHeight*startIndex,0));
			}
			else
			{
				scrollView.MoveAbsolute(new Vector3(-cellWidth*startIndex,0,0));
			}
		}

		reposition(isResetPos);


		_listDrawStatus = ListDrawStatus.start;


	}






	// 필요한 아이템을 리스트에 올려놓는다.. 이제 얘들을 돌려쓸거다...
	private void createItemPanel()
	{
//		if(endLineDummy == null)
//		{
//			endLineDummy = (new GameObject("endline")).AddComponent<UILabel>();
//			endLineDummy.fontSize = 1;
//			endLineDummy.text = ".";
//			endLineDummy.color = new Color(0,0,0,0.1f);
//			endLineDummy.transform.parent = transform;
//		}

//		if(direction == Direction.vertical)
//		{
//			endLineDummy.transform.localPosition = new Vector3(0, -(dataList.Count)*cellHeight, 0);
//		}
//		else
//		{
//			endLineDummy.transform.localPosition = new Vector3((dataList.Count)*cellWidth, 0, 0);
//		}

			
		for(int i = 0; i < totalItemNumber; ++i)
		{
			UIListGridItemPanelBase item = getItem();

			_v.x = 0; _v.y = 0; _v.z = 0;

			// 마지막 얘는 리스트 끝으로. 그래야 ngui panel 스크롤 영역이 정해진다.
			if(i == totalItemNumber-1)
			{
				if(direction == Direction.vertical)
				{
					_v.y = -(dataCount-1)*cellHeight;
				}
				else
				{
					_v.x = (dataCount-1)*cellWidth;
				}
			}
			// 나머지는 그냥 순서대로...
			else
			{
				if(direction == Direction.vertical)
				{
					_v.y = -i*cellHeight;
				}
				else
				{
					_v.x = i*cellWidth;
				}
			}


			item.cachedTransform.localPosition = _v;
			itemList.Add(item);

			setItemData(item);
		}
	}




	public void setAllItemImgLoad()
	{
		foreach(UIListGridItemPanelBase item in itemList)
		{
			setItemImgLoad(item);
		}
	}

	private void setItemImgLoad(UIListGridItemPanelBase item)
	{
		if(item.gameObject.activeSelf==true)
		{
			item.setPhotoLoad();
		}
	}

	public int positionIndex = 0;

	private void setItemData(UIListGridItemPanelBase item)
	{
		if(direction == Direction.vertical)
		{
			positionIndex = -(int)(item.cachedTransform.localPosition.y/cellHeight);
		}
		else
		{
			positionIndex = (int)(item.cachedTransform.localPosition.x/cellWidth);
		}

		if(item.gameObject.activeInHierarchy==true)
		{
#if UNITY_EDITOR
			item.name = positionIndex + "";
#endif
			if(dataCount > positionIndex && positionIndex >= 0 && item.index != positionIndex)
			{
				item.setIndex(positionIndex);
				item.setData(dataList[positionIndex]);
				setItemImgLoad(item);

#if UNITY_EDITOR
//				Debug.LogError("positionIndex : "  + positionIndex);
#endif

				if(callbackAfterDrawFinalItem != null && positionIndex >= dataCount -1)
				{
					if(_firstResetDraw == false)
					{
						callbackAfterDrawFinalItem(positionIndex);
					}
					else
					{
						_firstResetDraw = false;
					}
				}
			}
		}
	}




	private int checkFirstIndex()
	{
		if(panel == null) return 0;

		if(direction == Direction.vertical)
		{
			return Mathf.FloorToInt((panelTf.localPosition.y)/cellHeight);
		}
		else
		{
			return Mathf.FloorToInt((panelTf.localPosition.x)/cellWidth);
		}
	}


	public Bounds bound;
	public Vector3 constraint;

	private void updateItemPosition()
	{
		if(itemList == null || dataList == null) return;

		if(panel == null)return;

		bound = dragScrollView.bounds;
		constraint = dragScrollView.panel.CalculateConstrainOffset(bound.min, bound.max);

		float startPosition = (direction == Direction.vertical)?panelTf.localPosition.y:panelTf.localPosition.x;

		int itemCount = itemList.Count;

		foreach(UIListGridItemPanelBase item in itemList)
		{
			if(direction == Direction.vertical)
			{
				if(_panelMoveDirection == MoveDirection.Up)
				{
					if(item.cachedTransform.localPosition.y + startPosition > cellHeight)
					{
						if(item.cachedTransform.localPosition.y - (cellHeight*itemCount) > -(dataCount)*cellHeight)
						{
							_v.x = 0;
							_v.y = item.cachedTransform.localPosition.y - (cellHeight*itemCount);
							_v.z = 0;
							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
				}
				else if(_panelMoveDirection == MoveDirection.Down)
				{
					if(item.cachedTransform.localPosition.y + startPosition < -cellHeight*(itemCount-(dummyNum-1)) )
					{
						if(item.cachedTransform.localPosition.y + (cellHeight*itemCount) <= 0)
						{
							_v.x = 0;
							_v.y = item.cachedTransform.localPosition.y + (cellHeight*itemCount);
							_v.z = 0;
							
							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
				}
			}
			else
			{
				if(_panelMoveDirection == MoveDirection.Right)
				{
					if(item.cachedTransform.localPosition.x + startPosition > cellWidth*(itemCount-(dummyNum-1)))
					{
						if(item.cachedTransform.localPosition.x - (cellWidth*itemCount) >= 0)
						{
							_v.x = item.cachedTransform.localPosition.x - (cellWidth*itemCount);
							_v.y = 0;
							_v.z = 0;

//							Debug.Log(_v);

							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
				}
				else if(_panelMoveDirection == MoveDirection.Left)
				{
					if(item.cachedTransform.localPosition.x + startPosition < -cellWidth * upperDummyNum)
					{
						if(item.cachedTransform.localPosition.x + (cellWidth*itemCount) < (dataCount)*cellWidth)
						{
							_v.x = item.cachedTransform.localPosition.x + (cellWidth*itemCount);
							_v.y = 0;
							_v.z = 0;

//							Debug.Log(_v);
							item.cachedTransform.localPosition = _v;
							setItemData(item);
						}
					}
				}
			}
		}
	}









	// 패널이 움직였는지 본다.
	private bool checkPanelMovement()
	{
		if(direction == Direction.vertical && Mathf.Abs(panelTf.localPosition.y - panelPrevPosition) >=1)
		{
			if(panelPrevPosition - panelTf.localPosition.y > 0)
			{
				_panelMoveDirection = MoveDirection.Down;
			}
			else 
			{
				_panelMoveDirection = MoveDirection.Up; 
			}


			panelPrevPosition = panelTf.localPosition.y;
			return true;
		}
		else if(direction == Direction.horizontal && Mathf.Abs(panelTf.localPosition.x - panelPrevPosition) >=1)
		{

			if(panelPrevPosition - panelTf.localPosition.x > 0)
			{
				_panelMoveDirection = MoveDirection.Left;
			}
			else 
			{
				_panelMoveDirection = MoveDirection.Right; 
			}

			panelPrevPosition = panelTf.localPosition.x;
			return true;
		}

		return false;
	}


	public enum MoveDirection
	{
		Up,Down,Left,Right
	}

	MoveDirection _panelMoveDirection;

	bool needAllImageLoading = false;


	void Update () 
	{

		if(panel == null ) return;

		if(_listDrawStatus==ListDrawStatus.start)
		{
			if(checkPanelMovement()) // 패널이 움직였음...
			{
				// 영역을 벗어난 녀석이 있는지 보고 위치 조정.
				updateItemPosition();
				if(dragScrollView != null) dragScrollView.UpdateScrollbars(true);
				needAllImageLoading = true;
			}
			else if(needAllImageLoading)
			{
				setAllItemImgLoad();
				needAllImageLoading = false;
			}
		}
	}


	//============================================================= //


	UIListGridItemPanelBase[] itemPool;
	
	int itemPoolIndex = 0;
	int itemItemPoolLength = 0;
	UIListGridItemPanelBase getItem()
	{
		UIListGridItemPanelBase item;

		if(itemPoolIndex < itemItemPoolLength)
		{
			item = itemPool[itemPoolIndex];
			item.gameObject.SetActive(true);
			++itemPoolIndex;
		}
		else
		{
			item = Instantiate(listItem) as UIListGridItemPanelBase;
			item.cachedTransform = item.transform;
			item.transform.parent = transform;
		}

		item.index = -1000;

		return item;
	}
	
	public void itemClear(bool destroy = false)
	{
		if(destroy)
		{
			Transform[] tList = this.gameObject.GetComponentsInChildren<Transform>();
			//Debug.LogWarning("==============item Count "+tList.Length);
			foreach(Transform t in tList)
			{
				if(t.gameObject!=this.gameObject)
				{
					t.gameObject.SetActive(true);
					GameObject.Destroy(t.gameObject);
				}
			}
		}
		else
		{
			for(int i = 0; i < itemItemPoolLength; ++i)
			{
				itemPool[i].gameObject.SetActive(true);
			}
			
			itemPool = this.gameObject.GetComponentsInChildren<UIListGridItemPanelBase>();

			if(checkItemPoolChild)
			{
				List<UIListGridItemPanelBase> temp = new List<UIListGridItemPanelBase>();

				for(int i = itemPool.Length -1; i >= 0; --i)
				{
					if(itemPool[i].isPoolingSlot)
					{
						temp.Add(itemPool[i]);
					}
				}

				itemPool = temp.ToArray();
				temp.Clear();
			}

			itemPoolIndex = 0;
			itemItemPoolLength = itemPool.Length;

			for(int i = 0; i < itemItemPoolLength; ++i)
			{
				itemPool[i].gameObject.SetActive(false);
			}
		}
	}


	public void refreshPanel()
	{
		if(itemList != null)
		{
			int len = itemList.Count;

			for(int i = 0; i < len; ++i)
			{
				itemList[i].setPhotoLoad();
				itemList[i].refreshPanel();
			}
		}
	}


}