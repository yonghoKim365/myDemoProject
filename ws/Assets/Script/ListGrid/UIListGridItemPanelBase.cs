using UnityEngine;
using System.Collections;

public abstract class UIListGridItemPanelBase : MonoBehaviour
{
	public bool isPoolingSlot = true;

	public UIListGridItemPanelBase ()
	{
	}

	public Transform cachedTransform;

	void Awake()
	{
		cachedTransform = this.transform;
		initAwake();
	}

	protected abstract void initAwake();

	public int index = 0;
	protected int _idx = 0;

	public void setIndex(int idx)
	{
		_idx = idx;
		index = idx;
	}

	public abstract void setPhotoLoad();

	public abstract void setData(object obj);

	public virtual void refreshPanel()
	{
	}

}

