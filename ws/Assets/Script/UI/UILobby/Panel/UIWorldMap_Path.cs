using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class UIWorldMap : UIBase 
{
	UIWorldMapCheckPointButton[] _pb = null;


	Vector3 getCurrentPosition(int act, int nowStage)
	{
		if(act > GameManager.MAX_ACT)
		{
			act = GameManager.MAX_ACT;
			nowStage = 5;
		}

		_pb = null;

		switch(act)
		{
		case 1: _pb = mapRoad.cpAct1; break;
		case 2: _pb = mapRoad.cpAct2; break;
		case 3: _pb = mapRoad.cpAct3; break;
		case 4: _pb = mapRoad.cpAct4; break;
		case 5: _pb = mapRoad.cpAct5; break;
		case 6: _pb = mapRoad.cpAct6; break;
		case 7:
			Debug.Log("NEED ADD NEW ACT CASE !!!!!!!!!!!!!!!!!!!!!!!!!!!");
			break;
		}

		if(_pb != null && (nowStage > 0 && nowStage <= GameManager.MAX_ACT))
		{
			if(_pb[nowStage-1] != null)
			{
				_v = mapPlayer.cTransform.position;
				_v.x = _pb[nowStage-1].transform.position.x;
				_v.y = _pb[nowStage-1].transform.position.y + 9.34f;
				_v.z = 1839.891f;
				return _v; 
			}
		}


		// 아래는 안씀.

		switch(act)
		{
		case 1:
			switch(nowStage)
			{
			case 1:
				return new Vector3(-386,-176,0);//new Vector3(-409,-176,0);
				break;
			case 2:
				return new Vector3(-437,-37,0);
				break;
			case 3:
				return new Vector3(-332,107,0);
				break;
			case 4:
				return new Vector3(-231,-50,0);
				break;
			case 5:
				return new Vector3(-76,-128,0);
				break;
			}
			
			break;
		case 2:
			
			switch(nowStage)
			{
			case 1:
				return new Vector3(88,-33,0);
				break;
			case 2:
				return new Vector3(110,118,0);
				break;
			case 3:
				return new Vector3(262,18,0);
				break;
			case 4:
				return new Vector3(258,-178,0);
				break;
			case 5:
				return new Vector3(413,-67,0);
				break;
			}
			
			break;
		case 3:
			
			switch(nowStage)
			{
			case 1:
				return new Vector3(610,-155,0);
				break;
			case 2:
				return new Vector3(573,76,0);
				break;
			case 3:
				return new Vector3(775,-4,0);
				break;
			case 4:
				return new Vector3(805,-199,0);
				break;
			case 5:
				return new Vector3(921,-82,0);
				break;

			}
			
			break;
		case 4:
			
			switch(nowStage)
			{
			case 1:
				return new Vector3(1147,-173,0);
				break;
			case 2:
				return new Vector3(1138,-6,0);
				break;
			case 3:
				return new Vector3(1295,107,0);
				break;
			case 4:
				return new Vector3(1356,-83,0);
				break;
			case 5:
				return new Vector3(1564,-115,0);
				break;
			}
			
			break;
		case 5: 

			Debug.LogError("다시 설정 필요");

			switch(nowStage)
			{

			case 1:
				return new Vector3(1825,-116,0);
				break;
			case 2:
				return new Vector3(1677,8,0);
				break;
			case 3:
				return new Vector3(1834,101,0);
				break;
			case 4:
				return new Vector3(1975,4,0);
				break;
			}
			
			break;
		}

		return Vector3.zero;

	}
	
	
	/*
	private List<Vector3> _path = new List<Vector3>();

	List<Vector3> getPath(int act, int nextStage, float option)
	{
		_path.Clear();
		switch(act)
		{
		case 1:
			switch(nextStage)
			{
			case 2:
				_path.Add(new Vector3(-411,-180,0));
				_path.Add(new Vector3(-427,-130,0));
				_path.Add(new Vector3(-447,-88,0));
				_path.Add(new Vector3(-461,-33,0));
				break;
			case 3:
				_path.Add(new Vector3(-461,-33,0));
				_path.Add(new Vector3(-454,25,0));
				_path.Add(new Vector3(-419,71,0));
				_path.Add(new Vector3(-355,104,0));
				break;
			case 4:
				_path.Add(new Vector3(-355,104,0));
				_path.Add(new Vector3(-260,84,0));
				_path.Add(new Vector3(-236,23,0));
				_path.Add(new Vector3(-247,-43,0));
				break;
			case 5:
				_path.Add(new Vector3(-247,-43,0));
				_path.Add(new Vector3(-238,-134,0));
				_path.Add(new Vector3(-176,-153,0));
				_path.Add(new Vector3(-99,-119,0));
				break;
			}
			
			break;
		case 2:
			
			switch(nextStage)
			{
			case 1:
				_path.Add(new Vector3(-99,-119,0));
				_path.Add(new Vector3(-35,-118,0));
				_path.Add(new Vector3(4,-97,0));
				_path.Add(new Vector3(69,-25,0));
				break;
			case 2:
				_path.Add(new Vector3(69,-25,0));
				_path.Add(new Vector3(35,16,0));
				_path.Add(new Vector3(33,79,0));
				_path.Add(new Vector3(88,126,0));
				break;
			case 3:
				_path.Add(new Vector3(88,126,0));
				_path.Add(new Vector3(163,107,0));
				_path.Add(new Vector3(215,78,0));
				_path.Add(new Vector3(239,25,0));
				break;
			case 4:
				_path.Add(new Vector3(239,25,0));
				_path.Add(new Vector3(187,-63,0));
				_path.Add(new Vector3(156,-155,0));
				_path.Add(new Vector3(240,-174,0));
				break;
			case 5:
				_path.Add(new Vector3(240,-174,0));
				_path.Add(new Vector3(305,-165,0));
				_path.Add(new Vector3(338,-136,0));
				_path.Add(new Vector3(392,-65,0));
				break;
			}

			break;
		case 3:
			
			switch(nextStage)
			{
			case 1:
				_path.Add(new Vector3(392,-65,0));
				_path.Add(new Vector3(467,-75,0));
				_path.Add(new Vector3(493,-124,0));
				_path.Add(new Vector3(522,-184,0));
				_path.Add(new Vector3(586,-145,0));
				break;
			case 2:
				_path.Add(new Vector3(586,-145,0));
				_path.Add(new Vector3(624,-89,0));
				_path.Add(new Vector3(554,-54,0));
				_path.Add(new Vector3(513,8,0));
				_path.Add(new Vector3(556,90,0));
				break;
			case 3:
				_path.Add(new Vector3(556,90,0));
				_path.Add(new Vector3(641,91,0));
				_path.Add(new Vector3(706,54,0));
				_path.Add(new Vector3(758,9,0));
				break;
			case 4:
				_path.Add(new Vector3(758,9,0));
				_path.Add(new Vector3(745,-88,0));
				_path.Add(new Vector3(708,-156,0));
				_path.Add(new Vector3(784,-184,0));
				break;
			case 5:
				_path.Add(new Vector3(780,-184,0));
				_path.Add(new Vector3(847,-185,0));
				_path.Add(new Vector3(877,-150,0));
				_path.Add(new Vector3(901,-82,0));
				break;
			}
			
			break;
		case 4:
			
			switch(nextStage)
			{
			case 1:
				_path.Add(new Vector3(901,-82,0));
				_path.Add(new Vector3(951,-68,0));
				_path.Add(new Vector3(985,-102,0));
				_path.Add(new Vector3(985,-165,0));
				_path.Add(new Vector3(1022,-219,0));
				_path.Add(new Vector3(1073,-218,0));
				_path.Add(new Vector3(1128,-181,0));
				break;
			case 2:
				_path.Add(new Vector3(1128,-181,0));
				_path.Add(new Vector3(1157,-136,0));
				_path.Add(new Vector3(1157,-81,0));
				_path.Add(new Vector3(1111,-10,0));
				break;
			case 3:
				_path.Add(new Vector3(1111,-10,0));
				_path.Add(new Vector3(1092,68,0));
				_path.Add(new Vector3(1131,127,0));
				_path.Add(new Vector3(1206,153,0));
				_path.Add(new Vector3(1271,103,0));
				break;
			case 4:
				_path.Add(new Vector3(1271,103,0));
				_path.Add(new Vector3(1307,49,0));
				_path.Add(new Vector3(1381,48,0));
				_path.Add(new Vector3(1402,-14,0));
				_path.Add(new Vector3(1373,-54,0));
				_path.Add(new Vector3(1332,-89,0));
				break;
			case 5:
				_path.Add(new Vector3(1332,-89,0));
				_path.Add(new Vector3(1332,-171,0));
				_path.Add(new Vector3(1386,-200,0));
				_path.Add(new Vector3(1436,-154,0));
				_path.Add(new Vector3(1475,-106,0));
				_path.Add(new Vector3(1536,-127,0));
				break;
			}
			
			break;
		case 5: 
			
			switch(nextStage)
			{
			case 1:
				_path.Add(new Vector3(1536,-127,0));
				_path.Add(new Vector3(1632,-126,0));
				_path.Add(new Vector3(1696,-195,0));
				_path.Add(new Vector3(1759,-227,0));
				_path.Add(new Vector3(1818,-183,0));
				break;
			case 2:
				_path.Add(new Vector3(1825,-116,0));
				_path.Add(new Vector3(1774,-82,0));
				_path.Add(new Vector3(1713,-78,0));
				_path.Add(new Vector3(1665,-55,0));
				_path.Add(new Vector3(1677,8,0));
				break;
			case 3:
				_path.Add(new Vector3(1677,8,0));
				_path.Add(new Vector3(1708,55,0));
				_path.Add(new Vector3(1746,79,0));
				_path.Add(new Vector3(1834,101,0));
				break;
			case 4:
				_path.Add(new Vector3(1834,101,0));
				_path.Add(new Vector3(1898,68,0));
				_path.Add(new Vector3(1904,7,0));
				_path.Add(new Vector3(1919,-23,0));
				_path.Add(new Vector3(1975,4,0));
				break;
			case 5:
				_path.Add(new Vector3(1975,4,0));
				_path.Add(new Vector3(2036,-6,0));
				_path.Add(new Vector3(2085,-57,0));
				_path.Add(new Vector3(2071,-124,0));
				break;
			}
			
			break;
		}
		
		return _path;//CatmullRom.GetAllPoints(_path, option);;
	}
	*/

}