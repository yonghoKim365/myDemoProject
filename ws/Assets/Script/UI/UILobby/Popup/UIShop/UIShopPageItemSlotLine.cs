using UnityEngine;
using System.Collections;

public class UIShopPageItemSlotLine : UIListGridItemPanelBase 
{
	public UIShopItemSlot slotPrefab;

	public int maxPerLine = 3;
	public float cellWidth = 100.0f;

	public UIShopItemSlot[] slots = null;

	public float xOffset = 150.0f;

	public P_Product[] data;

	protected override void initAwake ()
	{
		slots = new UIShopItemSlot[maxPerLine];
		for(int i = 0; i < maxPerLine; ++i)
		{
			slots[i] = null;
		}
	}

	private Vector3 _v;

	public override void setPhotoLoad ()
	{
		if(slots != null)
		{
			for(int i = 0; i < slots.Length; ++i)
			{
				slots[i].setPhotoLoad();
			}
		}
	}

	public override void setData (object obj)
	{
		data = (P_Product[])obj;

		int len = data.Length;

		for(int i = 0; i < maxPerLine; ++i)
		{
			if(slots[i] == null)
			{
				slots[i] = Instantiate(slotPrefab) as UIShopItemSlot;
				slots[i].transform.parent = this.transform;
				slots[i].scrollView.scrollView = GameManager.me.uiManager.popupShop.equipScrollView;

				_v = slots[i].transform.localPosition;
				_v.x = i * cellWidth + xOffset;
				_v.y = 0.0f;
				_v.z = 0.0f;
				slots[i].transform.localPosition = _v;
			}

			if(i < len && data[i] != null)
			{
				slots[i].setData( data[i] );
				slots[i].gameObject.SetActive(true);
			}
			else
			{
				slots[i].setData(null);
				slots[i].gameObject.SetActive(true);
			}

			slots[i].isPoolingSlot = false;
		}
	}
}
