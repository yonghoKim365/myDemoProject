using UnityEngine;
using System.Collections;

public class UIHeroInvenSlotLine : UIListGridItemPanelBase 
{
	public UIHeroInventorySlot slotPrefab;

	public int maxPerLine = 5;
	public float cellWidth = 100.0f;

	public UIHeroInventorySlot[] slots = null;

	public GameIDData[] data;

	protected override void initAwake ()
	{
		slots = new UIHeroInventorySlot[maxPerLine];
		for(int i = 0; i < maxPerLine; ++i)
		{
			slots[i] = null;
		}
	}

	private Vector3 _v;

	public override void setPhotoLoad ()
	{

	}

	public override void setData (object obj)
	{
		data = (GameIDData[])obj;

		int len = data.Length;

		for(int i = 0; i < maxPerLine; ++i)
		{
			if(slots[i] == null)
			{
				slots[i] = Instantiate(slotPrefab) as UIHeroInventorySlot;
				slots[i].transform.parent = this.transform;
				_v = slots[i].transform.localPosition;
				_v.x = i * cellWidth;
				_v.y = 0.0f;
				_v.z = 0.0f;
				slots[i].transform.localPosition = _v;
			}

			if(i < len && data[i] != null)
			{
				slots[i].setData(data[i],true,index * maxPerLine + i);
				slots[i].gameObject.SetActive(true);
			}
			else
			{
				slots[i].setData(null,true);
				slots[i].gameObject.SetActive(true);
			}
		}
	}
}
