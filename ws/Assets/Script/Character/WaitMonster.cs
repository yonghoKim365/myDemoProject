using System;
using UnityEngine;

sealed public class WaitMonster
{
	public WaitMonster ()
	{
	}
	
	
	private IVector3 _v;
	private float waitLine = 0.0f;
	private UnitData _waitUnitData;
	
	public void setData(IVector3 pos, float linePos, UnitData unitData)
	{
		_v = pos;
		_waitUnitData = unitData;
		waitLine = linePos;
	}
	
	
	public void update()
	{
		if(GameManager.me.characterManager.playerMonsterRightLine >= waitLine)
		{
			Monster summonMon = GameManager.me.mapManager.addMonsterToStage(null, null, false, null, _waitUnitData.id, _v, _waitUnitData);
			_v.y = 0.5f;
			GameManager.info.effectData[UnitSlot.getSummonEffectByRare(_waitUnitData.rare)].getEffect(-1000,_v, null, null, summonMon.summonEffectSize);
			_waitUnitData = null;
			GameManager.me.characterManager.removeAndSetWaitMonster(this);
		}
	}
	
}

