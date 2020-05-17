using UnityEngine;
using System.Collections;

public class UISummonInvenSlotLine : UIListGridItemPanelBase 
{
	public UISummonInvenSlot slotPrefab;

	public int maxPerLine = 5;
	public float cellWidth = 100.0f;

	public UISummonInvenSlot[] slots = null;

	public GameIDData[] data;

	protected override void initAwake ()
	{
		slots = new UISummonInvenSlot[maxPerLine];
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
				slots[i] = Instantiate(slotPrefab) as UISummonInvenSlot;
				slots[i].transform.parent = this.transform;
				slots[i].isInventorySlot = true;
				slots[i].checkCantUse = true;
				_v = slots[i].transform.localPosition;
				_v.x = i * cellWidth;
				_v.y = 0.0f;
				_v.z = 0.0f;
				slots[i].transform.localPosition = _v;
			}

			if(i < len && data[i] != null)
			{
				slots[i].setData(data[i], index * maxPerLine + i);
				slots[i].gameObject.SetActive(true);
			}
			else
			{
				slots[i].setData(null);
				slots[i].gameObject.SetActive(true);
			}

		}
	}
}
