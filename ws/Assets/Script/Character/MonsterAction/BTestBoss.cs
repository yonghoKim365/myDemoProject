using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class BTestBoss : CharacterAction
{
	public BTestBoss ()
	{
	}
	
	private CharacterManager _cm;
	
	sealed public override void init(Monster monster)
	{
		base.init(monster);
		
		_cm = GameManager.me.characterManager;
	}
	
	
	sealed public override void startAction ()
	{
		base.startAction ();
	}



	BTestGroupData[] groups = null;
	int nowGroupIndex = 0;

	public override void setData (object data)
	{
		string d = (string)data;
		string[] g = d.Split('/');

		groups = new BTestGroupData[g.Length];

		for(int i = 0; i < g.Length; ++i)
		{
			groups[i] = new BTestGroupData();
			groups[i].init(g[i].Split(','));
		}

		nowGroupIndex = 0;
	}



	float _delay = 0.0f;
	sealed public override void doMotion()
	{
		_delay += GameManager.globalDeltaTime;

		if(groups == null || nowGroupIndex >= groups.Length)
		{
			return;
		}

		if(canCreateMonster())
		{
//			Debug.LogError("nowGroupIndex : " + nowGroupIndex);

			if(groups[nowGroupIndex].create(veryRightMonsterPositionX) == false)
			{
				++nowGroupIndex;
			}
		}
	}	


	float veryRightMonsterPositionX = 0.0f;

	bool canCreateMonster()
	{
		veryRightMonsterPositionX = -1.0f;

		if(_cm.monsters.Count == 0)
		{
			veryRightMonsterPositionX = GameManager.me.player.cTransformPosition.x + 1200.0f;
			return true;
		}
		else
		{
			for(int i = _cm.monsters.Count - 1; i >= 0; --i)
			{
				if(_cm.monsters[i].hp > 0 && _cm.monsters[i].cTransformPosition.x > veryRightMonsterPositionX)
				{
					veryRightMonsterPositionX = _cm.monsters[i].cTransformPosition.x;
				}
			}
			
			return (veryRightMonsterPositionX > 0 && veryRightMonsterPositionX - GameManager.me.player.cTransformPosition.x < 1200.0f);
		}
	}




	public override void clear ()
	{
		base.clear ();
		_cm = null;
	}
}


public class BTestGroupData
{
	string[] units;
	public float offset = 0.0f;
	public int createGroupNum = 0;

	int _unitIndex = 2;
	int _unitLastIndex = 0;

	int nowIndex = 0;

	public void init(string[] data)
	{
		units = data;
		float.TryParse(data[0], out offset);
		int.TryParse(data[1], out createGroupNum);

		_unitLastIndex = units.Length;
		_unitIndex = 2;
	}



	public bool create(float posX)
	{
		if(nowIndex < createGroupNum)
		{
//			Debug.Log("nowIndex : " + nowIndex + "  of " + createGroupNum);

			for(int i = 0; i < 2; ++i)
			{
				if(_unitIndex >= _unitLastIndex)
				{
					_unitIndex = 2;
				}

//				Debug.Log("       _unitIndex : " + _unitIndex);

				createSummon(units[_unitIndex], posX+offset, (i==0)?-100.0f:100.0f);
				++_unitIndex;
			}

			++nowIndex;
			return true;
		}
		return false;
	}


	void createSummon(string unitId, float posX, float posZ)
	{
		Vector3 _v = new Vector3();
		_v.x = posX;
		_v.z = posZ;

		Monster summonMon = GameManager.me.mapManager.addMonsterToStage(null, null, false, null, unitId,_v);
		_v.y = 0.5f;
		//GameManager.info.effectData[UnitSlot.SUMMON_EFFECT_ENEMY].getEffect(_v, null, null, summonMon.shadowSize); 
	}
}


