using System;
using System.Collections.Generic;

public class CharacterSorter
{
	public CharacterSorter ()
	{
	}
}


public class MonsterSorterByTransformPosX  : IComparer<Monster>
{
	public int Compare(Monster x, Monster y)
	{
		if(x.cTransform.position.x > y.cTransform.position.x) return 1;
		else if(x.cTransform.position.x < y.cTransform.position.x) return -1;		
		return 0;
	}	
}


public class MonsterSorterByPosX  : IComparer<Monster>
{
	public int Compare(Monster x, Monster y)
	{
		if(x.lineLeft > y.lineLeft) return 1;
		else if(x.lineLeft < y.lineLeft) return -1;		

		return x.stat.uniqueId.CompareTo(y.stat.uniqueId);
	}	
}
	

public class PlayerMonsterSorterByPosX  : IComparer<Monster>
{
	public int Compare(Monster x, Monster y)
	{
		if(x.lineRight < y.lineRight) return 1;
		else if(x.lineRight > y.lineRight) return -1;

		return x.stat.uniqueId.CompareTo(y.stat.uniqueId);
	}	
}



public class SorterByDistance  : IComparer<Monster>
{
	public int Compare(Monster x, Monster y)
	{
		if(x.distanceBetweenAttacker > y.distanceBetweenAttacker) return 1;//result = 1;
		else if(x.distanceBetweenAttacker < y.distanceBetweenAttacker) return -1;//result = -1;
		return x.stat.uniqueId.CompareTo(y.stat.uniqueId);
		//return result;
	}	

	// x, y가 입력 됐을때.
	// x.compareto(y) <= 1,2,3,4  낮은 순으로 정렬된다.
	// x > y = 1, x < y = -1  위와 같이 낮은 순으로 정렬된다.
}


public class SorterByDistanceFromHitPoint  : IComparer<Monster>
{
	public int Compare(Monster x, Monster y)
	{
		int result = x.distanceFromHitPoint.CompareTo(y.distanceFromHitPoint);

		if(result == 0) return x.stat.uniqueId.CompareTo(y.stat.uniqueId);
		else return result;
	}	
}

public class SorterByDistanceFromHitPointCharacter  : IComparer<Monster>
{
	public int Compare(Monster x, Monster y)
	{
		if(x.distanceFromHitPoint < y.distanceFromHitPoint) return 1;
		else if(x.distanceFromHitPoint > y.distanceFromHitPoint) return -1;

		return x.stat.uniqueId.CompareTo(y.stat.uniqueId);
	}	
}
		
public class SorterFloat  : IComparer<IFloat>
{
	public int Compare(IFloat x, IFloat y)
	{
		if(x > y) return 1;
		else if(x < y) return -1;
		return 0;
	}	
}		