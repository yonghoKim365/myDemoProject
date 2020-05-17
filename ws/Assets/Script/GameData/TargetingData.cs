using System;

sealed public class TargetingData
{
/*	
[타게팅타입] [속성1,2]

* 타게팅타입
   - 0 없음 : 타게팅 조작 및 영역이 존재하지 않는 (예: 직선 발사형 or 아군전체 대상 등)
   - 1 고정 : 타게팅 조작 없이 타겟영역이 고정된 방식 (예: 히어로 전방 N미터)
   - 2 지정 : 플레이어가 직접 타겟영역을 지정하는 방식
   - 3 자동 : 타게팅 조작 없이 특정조건에 해당하는 대상을 자동으로 선택하는 방식

* 타게팅속성1 : 타게팅타입에 따른 서브속성
   → 타입0 : 없음 / 
     타입1 : 히어로위치 기준 전방거리 (CM)
     타입2 : 타게팅지정 최소거리 (CM, 히어로위치 기준)
   → 타입3
       1 : 히어로와 가장 가까운 적(아군) 유닛
       2 : 적(아군) 중 HP가 가장 낮은 유닛
       
* 타게팅속성2
   → 타입0 : 없음 / 
     타입1 : 없음 / 
     타입2 : 타게팅지정 최대거리 (CM, 히어로위치 기준)	
*/	
	
/*	

	* 해당 스킬이 발사되는 위치를 지정하는 방식 (누구에게 쏠건지) 및 타게팅 표시 UI 방식을 지정

<0 : 없음>  // none. 0개.
  - 특별한 타겟이 존재하지 않으며, 타게팅 데칼 없음

<1 : 히어로 전방 A미터, B지름>  // fixed. 2개.
  - 바닥에 뭔가가 떨어지거나 생성되는 스킬에 적용
  - 타게팅(차징)중 히어로 전방 A미터에 B지름 형태의 (원형 마법진모양의) 타게팅 데칼을 표시함
  - 설정된 지름의 크기에 따라서 원형태의 마법진 이미지의 크기를 변경해서 표시함
  - B지름이 400을 넘어가면 타원으로 (일단;) 늘려서 표시

<2 : A번 조건에  맞는 유닛을 자동조준 >
  - 조건 1: 히어로와 가장 가까운 적(아군)
  - 조건2 : HP가 가장 낮은 적(아군)
  - 해당 유닛의 바닥에 타게팅 데칼 표시 (그림자위에 표시)

<3 : 전방 직선 발사>
  - 직선 관통발사 같은 형태의 공격에 적용
  - 히어로 바로앞 바닥에 (화살표 모양같은) 타게팅 데칼 표시
*/ 
	
	public const int NONE = 0;
	public const int FIXED_1 = 1;
	public const int AUTOMATIC_2 = 2;
	public const int FORWARD_LINEAR_3 = 3;
	
	public const int AUTOMATIC_CLOSE_TARGET_1 = 1;
	public const int AUTOMATIC_LOWEST_HP_TARGET_2 = 2;
	
	public int[] attr;
	
	public static TargetingData getTargetingData(int type, params object[] args)
	{
		int len = 0;
		switch(type)
		{
		case FIXED_1:
			len = 2;
			break;
		case AUTOMATIC_2:
			len = 1;
			break;
		}
		
		TargetingData td = new TargetingData();
		
		td.attr = new int[len];
		for(int i = 0; i < len; ++i)
		{
			Util.parseObject(args[i],out td.attr[i], true, 0);	
		}
		
		return td;
	}
	
	
	public static bool isTargetPlayer(Skill.TargetType targetType, bool isPlayerSide)
	{
		if(targetType == Skill.TargetType.ENEMY) // 타겟팅 타입이 적일때.
		{
			if(isPlayerSide) return false;
			else return true;
		}
		else // 타겟팅 타입이 우리편일때.
		{
			if(isPlayerSide) return true;
			else return false;
		}	
	}
	
	
	public static Monster getAutomaticTarget(Monster attacker, Skill.TargetType targetingType, Xint[] subTargingType, BaseSkillData.CheckTargetingType targetingTypeChecker = null)
	{
		if(subTargingType[0] == TargetingData.AUTOMATIC_CLOSE_TARGET_1) // 히어로와 가장 가까운 타겟.
		{
			if(targetingType == Skill.TargetType.ENEMY)
			{
				return GameManager.me.characterManager.getCloseEnemyTarget(attacker.isPlayerSide, attacker, targetingTypeChecker, (subTargingType[1] == 0));
			}
			else
			{
				return GameManager.me.characterManager.getCloseTeamTarget(attacker.isPlayerSide, attacker, targetingTypeChecker, (subTargingType[1] == 0));
			}
		}
		else if(subTargingType[0] == TargetingData.AUTOMATIC_LOWEST_HP_TARGET_2) // 가장 에너지가 적은 녀석. 
		{
			if(targetingType == Skill.TargetType.ENEMY)
			{
				return GameManager.me.characterManager.getLowestHPEnemyTarget(attacker.isPlayerSide, targetingTypeChecker, (subTargingType[1] == 0));
			}
			else
			{
				return GameManager.me.characterManager.getLowestHPTeamTarget(attacker.isPlayerSide, attacker, targetingTypeChecker, (subTargingType[1] == 0));
			}
		}		
		
		return null;
	}	
	
	
}
