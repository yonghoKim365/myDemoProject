using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterPosition
{
	static List<Monster> _units = new List<Monster>();
	static Queue<Monster> bSize = new Queue<Monster>();
	static Queue<Monster> mSize = new Queue<Monster>();
	static Queue<Monster> sSize = new Queue<Monster>();


	public static bool hasOutsidePosition = false;


	// 0.72    1.0f    1.2f
	public static float defaultZoomSize = 0.72f;

	bool _isMyLobby = true;

	int _bigUnitNum = 0;


	public void changePosition (Monster[] monsters, bool isMyLobby = true)
	{
		_isMyLobby = isMyLobby;

		hasOutsidePosition = false;
		_units.Clear();

		for(int i = 0; i < 5; ++i)
		{
			if(monsters[i] != null)
			{
				_units.Add(monsters[i]);
				monsters[i].monsterUISlotIndex = i;
			}
		}

		int totalUnit = _units.Count;

		if(totalUnit <= 0) return;
		if(totalUnit > 2) _units.Sort(Monster.sortByUISize);

		_bigUnitNum = 0;

		switch(totalUnit)
		{
		case 1:
			setNum1();
			break;
		case 2:
			setNum2();
			break;
		case 3:
			setNum3();
			break;
		case 4:
			setNum4();
			break;
		case 5:
			setNum5();
			break;
		}

		//0.72    1.0f    1.2f

		if(_bigUnitNum >= 3)
		{
			defaultZoomSize = 1.2f;
		}
		else
		{
			if(hasOutsidePosition)
			{
				defaultZoomSize = 1.0f;
			}
			else
			{
				defaultZoomSize = 0.72f;
			}
		}

		bSize.Clear();
		mSize.Clear();
		sSize.Clear();

		_units.Clear();
	}


	void setPosition(params string[] data)
	{
		for(int i = 0; i < data.Length; ++i)
		{
			setPosition(_units[i],data[i]);
		}
	}

	void setPosition(Monster mon, string posId)
	{
		GameManager.info.lobbyPosition[posId].setPosition(mon.cTransform.parent.transform);

//		Debug.LogError(mon.resourceId + "   " + posId);

		if(hasOutsidePosition == false && (posId.StartsWith("L") || posId.StartsWith("R")))
		{
			hasOutsidePosition = true;
		}
	}


	void setHeroPosition(string posId)
	{
		if(_isMyLobby == false)
		{
			GameManager.info.lobbyPosition[posId].setPosition(GameManager.me.uiManager.uiVisitingLobby.hero.cTransform.parent.transform);
		}
		else
		{
			GameManager.info.lobbyPosition[posId].setPosition(GameManager.me.uiManager.uiMenu.uiLobby.hero.cTransform.parent.transform);
		}
	}

	void setNum1()
	{
//		[소환수 1마리]			
		if(_units[0].monsterUISlotIndex <= 2)
		{
//			0~2	대	CL4	HR1
//			0~2	중	HL2	HR1
//			0~2	소	HL1	HR2


			switch(checkMonsterSize(_units[0]))
			{
			case MonsterSize.Big:
				setPosition(_units[0],"CL4");
				setHeroPosition("HR1");
				break;
			case MonsterSize.Medium:
				setPosition(_units[0],"HL2");
				setHeroPosition("HR1");
				break;
			case MonsterSize.Small:
				setPosition(_units[0],"HL1");
				setHeroPosition("HR2");
				break;
			}
		}
		else
		{
//			3~4	대	CR4	HL1
//				3~4	중	HR2	HL1
//					3~4	소	HR1	HL2


			switch(checkMonsterSize(_units[0]))
			{
			case MonsterSize.Big:
				setPosition(_units[0],"CR4");
				setHeroPosition("HL1");
				break;
			case MonsterSize.Medium:
				setPosition(_units[0],"HR2");
				setHeroPosition("HL1");
				break;
			case MonsterSize.Small:
				setPosition(_units[0],"HR1");
				setHeroPosition("HL2");
				break;
			}
		}
	}

	void setNum2()
	{
		setHeroPosition("HC1");

		if(checkMonsterSize(_units[0]) == MonsterSize.Big)
		{
			if(checkMonsterSize(_units[1]) == MonsterSize.Big)
			{
				//대	대  CL4	CR4	HC1
				setPosition("CL4","CR4");
			}
			else if(checkMonsterSize(_units[1]) == MonsterSize.Medium)
			{
				//대	중	CL4	CR3	HC1
				setPosition("CL4","CR3");
			}
			else if(checkMonsterSize(_units[1]) == MonsterSize.Small)
			{
				//대	소	CL4	CR2	HC1
				setPosition("CL4","CR2");
			}
		}
		else if(checkMonsterSize(_units[0]) == MonsterSize.Medium)
		{
//			중	대	CL3	CR4	HC1
//			중	중	CL3	CR3	HC1
//			중	소	CL3	CR2	HC1

			if(checkMonsterSize(_units[1]) == MonsterSize.Big)
			{
				setPosition("CL3","CR4");
			}
			else if(checkMonsterSize(_units[1]) == MonsterSize.Medium)
			{
				setPosition("CL3","CR3");
			}
			else if(checkMonsterSize(_units[1]) == MonsterSize.Small)
			{
				setPosition("CL3","CR2");
			}
		}

		else if(checkMonsterSize(_units[0]) == MonsterSize.Small)
		{
//			소	대	CL2	CR4	HC1
//			소	중	CL2	CR3	HC1
//			소	소	CL2	CR2	HC1
			if(checkMonsterSize(_units[1]) == MonsterSize.Big)
			{
				setPosition("CL2","CR4");
			}
			else if(checkMonsterSize(_units[1]) == MonsterSize.Medium)
			{
				setPosition("CL2","CR3");
			}
			else if(checkMonsterSize(_units[1]) == MonsterSize.Small)
			{

				setPosition("CL2","CR2");
			}
		}

	}

	void setNum3()
	{
		//대	대	대	L4	CC2	R4	HC1
		if( 
		   checkMonsterSize(_units[0]) == MonsterSize.Big && 
		   checkMonsterSize(_units[1]) == MonsterSize.Big && 
		   checkMonsterSize(_units[2]) == MonsterSize.Big)
		{
			_bigUnitNum = 3;
			setPosition("L4","CC2","R4");
			setHeroPosition("HC1");
		}
		//대	대	중	L4	R4	CL2	HR2
		else if( 
		   checkMonsterSize(_units[0]) == MonsterSize.Big && 
		   checkMonsterSize(_units[1]) == MonsterSize.Big && 
		   checkMonsterSize(_units[2]) == MonsterSize.Medium)
		{
			setPosition("L4","CC2","R4");
			setHeroPosition("HC1");
		}
//		대	대	소	L4	R4	CC1	HC3
		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Big && 
		        checkMonsterSize(_units[1]) == MonsterSize.Big && 
		        checkMonsterSize(_units[2]) == MonsterSize.Small)
		{
			setPosition("L4","R4","CC1");
			setHeroPosition("HC3");
		}
//		대	중	중	CC2	CL3	CR3	HC1

		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Big && 
		        checkMonsterSize(_units[1]) == MonsterSize.Medium && 
		        checkMonsterSize(_units[2]) == MonsterSize.Medium)
		{
			setPosition("CC2","CL3","CR3");
			setHeroPosition("HC1");
		}
//		대	중	소	CC2	CL3	CR2	HC1
		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Big && 
		        checkMonsterSize(_units[1]) == MonsterSize.Medium && 
		        checkMonsterSize(_units[2]) == MonsterSize.Small)
		{
			setPosition("CC2","CL3","CR2");
			setHeroPosition("HC1");
		}
//		대	소	소	CC2	CL2	CR2	HC1
		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Big && 
		        checkMonsterSize(_units[1]) == MonsterSize.Small && 
		        checkMonsterSize(_units[2]) == MonsterSize.Small)
		{
			setPosition("CC2","CL2","CR2");
			setHeroPosition("HC1");
		}
		//		중	중	중	L4	CL2	R3	HR1
		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Medium && 
		        checkMonsterSize(_units[1]) == MonsterSize.Medium && 
		        checkMonsterSize(_units[2]) == MonsterSize.Medium)
		{
			setPosition("L4","CL2","R3");
			setHeroPosition("HR1");
		}

		//중	중	소	CL3	CR3	CC1	HC3
		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Medium && 
		        checkMonsterSize(_units[1]) == MonsterSize.Medium && 
		        checkMonsterSize(_units[2]) == MonsterSize.Small)
		{
			setPosition("CL3","CR3","CC1");
			setHeroPosition("HC3");
		}
//		중	소	소	L3	CR1	R2	HL1
		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Medium && 
		        checkMonsterSize(_units[1]) == MonsterSize.Small && 
		        checkMonsterSize(_units[2]) == MonsterSize.Small)
		{
			setPosition("L3","CR1","R2");
			setHeroPosition("HL1");
		}
		//		소	소	소	CL3	CC1	CR3	HC3
		else if( 
		        checkMonsterSize(_units[0]) == MonsterSize.Small && 
		        checkMonsterSize(_units[1]) == MonsterSize.Small && 
		        checkMonsterSize(_units[2]) == MonsterSize.Small)
		{
			setPosition("CL3","CC1","CR3");
			setHeroPosition("HC3");
		}
	}


	void setNum4()
	{
		int big = 0, medium = 0, small = 0;

		for(int i = 0; i < 4; ++i)
		{
			switch(checkMonsterSize(_units[i]))
			{
			case MonsterSize.Big: ++big; bSize.Enqueue(_units[i]); break;
			case MonsterSize.Medium: ++medium; mSize.Enqueue(_units[i]); break;	 
			case MonsterSize.Small: ++small; sSize.Enqueue(_units[i]); break;
			}
		}

		_bigUnitNum = big;

		if(big == 4)
		{
//			[소환수 대 4마리]	
//			
//			"N번째대 유닛"	위치
//				1	L4
//				2	CL4
//				3	CR4
//				4	R4
//					
//				히어로	HC1

			setPosition("L4","CL4","CR4","R4");
			setHeroPosition("HC1");
		}
		else if(big == 3)
		{
//			[소환수 대 3마리 + 나머지 1]	
//			
//			"N번째 대 유닛"	위치
//				1	L4
//				2	CC2
//				3	R4
//					
//				히어로	HL1
//					
//				나머지	위치
//				중	CR2
//				소	CR1

			setPosition(bSize.Dequeue(),"L4");
			setPosition(bSize.Dequeue(),"CC2");
			setPosition(bSize.Dequeue(),"R4");

			setHeroPosition("HL1");

			if(medium == 1) setPosition(mSize.Dequeue(),"CR2");
			else setPosition(sSize.Dequeue(),"CR1");
		}
		else if(big == 2)
		{
//			[소환수 대 2마리 + 나머지 2]		
//			
//			"N번째//대 유닛"	위치	
//				1	L4	
//					2	R4	
//					
//					히어로	HC3	
//					
//					나머지	위치	위치
//					중중	CL2	CR2
//					중소	CL2	CR1
//					소소	CL1	CR1


			setPosition(bSize.Dequeue(),"L4");
			setPosition(bSize.Dequeue(),"R4");
			
			setHeroPosition("HC3");

			if(medium == 2)
			{
				//중중	CL2	CR2
				setPosition(mSize.Dequeue(),"CL2");
				setPosition(mSize.Dequeue(),"CR2");
			}
			else if(medium == 1)
			{
				//중소	CL2	CR1
				setPosition(mSize.Dequeue(),"CL2");
				setPosition(sSize.Dequeue(),"CR1");
			}
			else
			{
				//소소	CL2	CR2
				setPosition(sSize.Dequeue(),"CL1");
				setPosition(sSize.Dequeue(),"CR1");
			}
		}
		else if(big == 1)
		{
			//[소환수 대 1마리 + 나머지 3]
			/*
			"N번째대 유닛"	위치		
			1	CL5		
					
			히어로	HR1		
			
			나머지	1번째	2번째	3번째
			중	L3	CL2	R3
			소	L2	CL1	R2
			*/

			setPosition(bSize.Dequeue(),"CL5");
			setHeroPosition("HR1");

			int i = 0;
			while(mSize.Count > 0)
			{
				if(i == 0) setPosition(mSize.Dequeue(),"L3");
				else if(i == 1) setPosition(mSize.Dequeue(),"CL2");
				else if(i == 2) setPosition(mSize.Dequeue(),"R3");
				++i;
			}

			while(sSize.Count > 0)
			{
				if(i == 0) setPosition(sSize.Dequeue(),"L2");
				else if(i == 1) setPosition(sSize.Dequeue(),"CL1");
				else if(i == 2) setPosition(sSize.Dequeue(),"R2");
				++i;
			}
		}
		else if(medium == 4)
		{
//			[소환수 중 4마리]	
//			
//			"N번째 중 유닛"	위치
//			1	L3
//			2	CL2
//			3	CR2
//			4	R3
//					
//			히어로	HC3

			setPosition("L3","CL2","CR2","R3");
			setHeroPosition("HC3");
		}
		else if(medium == 3 && small == 1)
		{
//			[소환수 중 3마리 + 소 1]	
//			
//			"N번째 중 유닛"	위치
//				1	L3
//				2	CL4
//				3	R3
//					
//				히어로	HR2
//				소	CL1


			setPosition(mSize.Dequeue(),"L3");
			setPosition(mSize.Dequeue(),"CL4");
			setPosition(mSize.Dequeue(),"R3");

			setHeroPosition("HR2");

			setPosition(sSize.Dequeue(),"CL1");
		}
		else if(medium == 2 && small == 2)
		{
//				[소환수 중 2마리 + 소 2]	
//			
//				"N번째중 유닛"	위치
//				1	L3
//				2	R3
//					
//				히어로	HC2
//				소1	CL2
//				소2	CR2

			setPosition(mSize.Dequeue(),"L3");
			setPosition(mSize.Dequeue(),"R3");

			setHeroPosition("HC2");

			setPosition(sSize.Dequeue(),"CL2");
			setPosition(sSize.Dequeue(),"CR2");

		}
		else if(medium == 1 && small == 3)
		{
			//[소환수 중 1마리 + 소 3]

//			"N번째중 유닛"	위치
//			1	CR4
//					
//			히어로	HL2
//			소1	L2
//			소2	CC1
//			소3	R2
			setPosition(mSize.Dequeue(),"CR4");

			setHeroPosition("HL2");

			setPosition(sSize.Dequeue(),"L2");
			setPosition(sSize.Dequeue(),"CC1");
			setPosition(sSize.Dequeue(),"R2");
		}
		else if(small == 4)
		{

//			히어로	HC3
//			소1	CL3
//			소2	CL1
//			소3	CR1
//			소4	CR3

			setPosition("CL3","CL1","CR1","CR3");
			setHeroPosition("HC3");
		}
	}

	void setNum5()
	{
		int big = 0, medium = 0, small = 0;
		
		for(int i = 0; i < 5; ++i)
		{
			switch(checkMonsterSize(_units[i]))
			{
			case MonsterSize.Big: ++big; bSize.Enqueue(_units[i]); break;
			case MonsterSize.Medium: ++medium; mSize.Enqueue(_units[i]); break;	 
			case MonsterSize.Small: ++small; sSize.Enqueue(_units[i]); break;
			}
		}	

		_bigUnitNum = big;

		if(big == 5)
		{
//			[소환수 대 5마리]	
//			
//			1	L2
//				2	L6
//					3	CC2
//					4	R6
//					5	R2

//					
//					히어로	HC1
			setPosition("L2","L6","CC2","R6","R2");	
			setHeroPosition("HC1");
		}
		else if(big == 4)
		{
//			[소환수 대 4마리 + 나머지 1]	
//			
//			1	L2
//				2	L6
//					3	R6
//					4	R2

//				
//				나머지	위치
//				중	CL2
//				소	CL1
//				
//				히어로	HR2

			setPosition(bSize.Dequeue(),"L2");
			setPosition(bSize.Dequeue(),"L6");
			setPosition(bSize.Dequeue(),"R6");
			setPosition(bSize.Dequeue(),"R2");

			if(mSize.Count > 0) setPosition(mSize.Dequeue(),"CL2");
			else if(sSize.Count > 0) setPosition(sSize.Dequeue(),"CL1");

			setHeroPosition("HR2");

		}
		else if(big == 3)
		{
//			[소환수 대 3마리 + 나머지 2]		
//			
//			1	L4
//				2	CC2
//					3	R4

//				
//				나머지	유닛1	유닛2
//				중	CL3	CR3
//				소	CL2	CR2
//				
//				히어로	HC1	

			setPosition(bSize.Dequeue(),"L4");
			setPosition(bSize.Dequeue(),"CC2");
			setPosition(bSize.Dequeue(),"R4");

			int i = 0;
			while(mSize.Count > 0)
			{
				if(i == 0) setPosition(mSize.Dequeue(),"CL3");
				else setPosition(mSize.Dequeue(),"CR3");
				++i;
			}

			//i = 0;
			while(sSize.Count > 0)
			{
				if(i == 0) setPosition(sSize.Dequeue(),"CL2");
				else setPosition(sSize.Dequeue(),"CR2");
				++i;
			}

			setHeroPosition("HC1");


		}
		else if(big == 2)
		{
//			[소환수 대 2마리 + 나머지 3]				
//			
//				1	L6			
//				2	R6			
//				
//			중	L2	HR1	R2
//				소	L1	CR1	R1

//				
//				히어로	HL2			

			setPosition(bSize.Dequeue(),"L6");
			setPosition(bSize.Dequeue(),"R6");

			int i = 0;
			while(mSize.Count > 0)
			{
				if(i == 0) setPosition(mSize.Dequeue(),"L2");
				else if(i == 1) setPosition(mSize.Dequeue(),"HR1");
				else setPosition(mSize.Dequeue(),"R2");
				++i;
			}
			
//			i = 0;
			while(sSize.Count > 0)
			{
				if(i == 0) setPosition(sSize.Dequeue(),"L2");
				else if(i == 1) setPosition(sSize.Dequeue(),"CR1");
				else setPosition(sSize.Dequeue(),"R2");
				++i;
			}
			
			setHeroPosition("HL2");

		}
		else if(big == 1)
		{
//			[소환수 대 1마리 + 나머지 4]					
//			
//			대유닛	CC2				
//				
//			중중	 L4	CL3  	R4	CR3
//			중소	 L4	CL2  	R4	CR2
//			소소	 L2	CL2 	R2	CR2
//					
//			히어로	HC1			
			setPosition(bSize.Dequeue(),"CC2");

			_units.Clear();

			while(mSize.Count > 0)
			{
				_units.Add(mSize.Dequeue());
			}

			while(sSize.Count > 0)
			{
				_units.Add(sSize.Dequeue());
			}


			if( checkMonsterSize(_units[0]) == MonsterSize.Medium && checkMonsterSize(_units[1]) == MonsterSize.Medium)
			{
				setPosition(_units[0],"L4");
				setPosition(_units[1],"CL3");
			}
			else if( checkMonsterSize(_units[0]) == MonsterSize.Medium && checkMonsterSize(_units[1]) == MonsterSize.Small)
			{
				setPosition(_units[0],"L4");
				setPosition(_units[1],"CL2");
			}
			else if( checkMonsterSize(_units[1]) == MonsterSize.Medium && checkMonsterSize(_units[0]) == MonsterSize.Small)
			{
				setPosition(_units[1],"L4");
				setPosition(_units[0],"CL2");
			}
			else if( checkMonsterSize(_units[0]) == MonsterSize.Small && checkMonsterSize(_units[1]) == MonsterSize.Small)
			{
				setPosition(_units[0],"L2");
				setPosition(_units[1],"CL2");
			}


			if( checkMonsterSize(_units[2]) == MonsterSize.Medium && checkMonsterSize(_units[3]) == MonsterSize.Medium)
			{
				setPosition(_units[2],"R4");
				setPosition(_units[3],"CR3");				
			}
			else if( checkMonsterSize(_units[2]) == MonsterSize.Medium && checkMonsterSize(_units[3]) == MonsterSize.Small)
			{
				setPosition(_units[2],"R4");
				setPosition(_units[3],"CR2");							
			}
			else if( checkMonsterSize(_units[3]) == MonsterSize.Medium && checkMonsterSize(_units[2]) == MonsterSize.Small)
			{
				setPosition(_units[3],"R4");
				setPosition(_units[2],"CR2");							
			}
			else if( checkMonsterSize(_units[2]) == MonsterSize.Small && checkMonsterSize(_units[3]) == MonsterSize.Small)
			{
				setPosition(_units[2],"R2");
				setPosition(_units[3],"CR2");								
			}


			setHeroPosition("HC1");

		}
		else if(medium == 5)
		{
//			[소환수 중 5마리]		
//			L4
//				CL3
//					CC1
//					CR3
//					R4

//				
//				히어로	HL2	
			setPosition("L4","CL3","CC1","CR3","R4");	
			setHeroPosition("HC3");

		}
		else if(medium == 4)
		{
//			[소환수 중 4마리 + 소 1마리]		
//			
//			1	L4
//				2	CL3
//					3	CR3
//					4	R4

//				
//				소유닛	C1	
//				히어로	HC3	

			setPosition(mSize.Dequeue(),"L4");
			setPosition(mSize.Dequeue(),"CL3");
			setPosition(mSize.Dequeue(),"CR3");
			setPosition(mSize.Dequeue(),"R4");
			
			setPosition(sSize.Dequeue(),"CC1");

			setHeroPosition("HC3");

		}
		else if(medium == 3)
		{
//			[소환수 중 3마리 + 소 2마리]		
//			
//			"N번째 중 유닛"	위치
//				1	L4
//					2	CL3
//					3	R4
//					
//					히어로	HR2
//					
//					
//					"N번째 소 유닛"	위치
//					1	CC1
//					2	CR2


			setPosition(mSize.Dequeue(),"L4");
			setPosition(mSize.Dequeue(),"CL3");
			setPosition(mSize.Dequeue(),"R4");
			
			setHeroPosition("HC3");
			
			setPosition(sSize.Dequeue(),"CC1");
			setPosition(sSize.Dequeue(),"CR3");

		}
		else if(medium == 2)
		{
//			[소환수 중 2마리 + 소 3마리]		
//			"N번째 중 유닛"	위치
//				1	L4
//					2	R4
//					
//					
//					히어로	HC3
//					
//					
//					"N번째 소 유닛"	위치
//					1	CL3
//					2	CC1
//					3	CR3

			setPosition(mSize.Dequeue(),"L4");
			setPosition(mSize.Dequeue(),"R4");

			setHeroPosition("HC3");

			setPosition(sSize.Dequeue(),"CL3");
			setPosition(sSize.Dequeue(),"CC1");
			setPosition(sSize.Dequeue(),"CR3");

		}
		else if(medium == 1)
		{
//			[소환수 중 1마리 + 소 4마리]	
//			
//			"N번째
//소 유닛"	위치
//				1	L2
//					2	CL3
//					3	CC1
//					4	CR3
//					
//					중유닛	R3
//					히어로	HC3


					
			setPosition(sSize.Dequeue(),"L2");
			setPosition(sSize.Dequeue(),"CL3");
			setPosition(sSize.Dequeue(),"CC1");
			setPosition(sSize.Dequeue(),"CR3");

			setPosition(mSize.Dequeue(),"R3");
			setHeroPosition("HC3");
		}
		else
		{
//			[소환수 소 5마리]	
//			
//				"N번째
//				소 유닛"	위치
//			1	L2
//				2	CL2
//					3	CC1
//					4	CR2
//					5	R2
//					
//					히어로	HC3


			setPosition("L2","CL2","CC1","CR2","R2");
			setHeroPosition("HC3");
		}
	}


	enum MonsterSize
	{
		Big, Medium, Small
	}


	MonsterSize checkMonsterSize(Monster mon)
	{
		if(GameManager.info.modelData[ mon.resourceId ].lobbySize == ModelData.LobbySize.Big)
		{
			return MonsterSize.Big;
		}
		else if(GameManager.info.modelData[ mon.resourceId ].lobbySize == ModelData.LobbySize.Medium)
		{
			return MonsterSize.Medium;
		}

		return MonsterSize.Small;
	}

}

