public class MonsterStat
{

	public int uniqueId = -1;

	public Xint baseLevel = 0;

	// 원래 속도의 x배로 빨라진 값이 이 캐릭터의 기본 이동/공격 애니메이션 속도다.
	public Xfloat originalAtkSpeedRate = 1.0f;
	public Xfloat originalMoveSpeedRate = 1.0f;

	public Xfloat atkRange = 0;
	public Xfloat atkSpeed = 0;

	public Xfloat atkPhysic = 0;
	public Xfloat atkMagic = 0;


	public Xfloat defPhysic = 0;
	public Xfloat defMagic = 0;

	public Xfloat spRecovery = 0;
	public Xfloat mpRecovery = 0;

	public Xfloat speed = 0;

	public Xfloat summonSpPercent = 0;
	public Xfloat unitHpUp = 0;
	public Xfloat unitDefUp = 0;
	public Xfloat skillMpDiscount = 0;
	public Xfloat skillAtkUp = 0; 
	public Xfloat skillUp = 0;
	public Xfloat skillTimeUp = 0;

	public Xint reinforceLevel = 1;
	public Xint maxHp = 0;

	public Xint skillTargetLevel = 1;

	public Monster.TYPE monsterType = Monster.TYPE.NONE;
	
	public PlayerType playerType = PlayerType.NotPlayer;



	public float getSkillAtkUp(BaseSkillData hsd)
	{
		switch(playerType)
		{
		case PlayerType.NotPlayer:
			return skillAtkUp;
			break;
		case PlayerType.Player:
			return GameManager.me.player.skillAtkUp(hsd);
			break;
		case PlayerType.PVPPlayer:
			return GameManager.me.pvpPlayer.skillAtkUp(hsd);
			break;
		}
		return skillAtkUp;
	}



	public float getSkillUp(BaseSkillData hsd)
	{
		switch(playerType)
		{
		case PlayerType.NotPlayer:
			return skillUp;
			break;
		case PlayerType.Player:
			return GameManager.me.player.skillUp(hsd);
			break;
		case PlayerType.PVPPlayer:
			return GameManager.me.pvpPlayer.skillUp(hsd);
			break;
		}
		return skillUp;
	}




	public float getSkillTimeUp(BaseSkillData hsd)
	{
		switch(playerType)
		{
		case PlayerType.NotPlayer:
			return skillTimeUp;
			break;
		case PlayerType.Player:
			return GameManager.me.player.skillTimeUp(hsd);
			break;
		case PlayerType.PVPPlayer:
			return GameManager.me.pvpPlayer.skillTimeUp(hsd);
			break;
		}
		return skillTimeUp;
	}


	public enum PlayerType
	{
		NotPlayer, Player, PVPPlayer
	}

	public static void copyTo(ref MonsterStat from, ref MonsterStat to)
	{
		to.originalAtkSpeedRate.Set( from.originalAtkSpeedRate );
		to.originalMoveSpeedRate.Set( from.originalMoveSpeedRate );

		to.atkRange.Set( from.atkRange );
		to.atkSpeed.Set( from.atkSpeed );
		to.atkPhysic.Set( from.atkPhysic );
		to.atkMagic.Set( from.atkMagic );
		to.defPhysic.Set( from.defPhysic );
		to.defMagic.Set( from.defMagic );

		to.spRecovery.Set( from.spRecovery );
		to.mpRecovery.Set( from.mpRecovery );

		to.speed.Set( from.speed );
		
		to.summonSpPercent.Set( from.summonSpPercent);
		to.unitHpUp.Set( from.unitHpUp);
		to.unitDefUp.Set( from.unitDefUp);
		to.skillMpDiscount.Set( from.skillMpDiscount);
		to.skillAtkUp.Set( from.skillAtkUp); 
		to.skillUp.Set( from.skillUp);
		to.skillTimeUp.Set( from.skillTimeUp );

		to.monsterType = from.monsterType;

		to.playerType = from.playerType;

		to.baseLevel.Set( from.baseLevel );

		to.reinforceLevel.Set( from.reinforceLevel );
		to.maxHp.Set( from.maxHp );

		to.skillTargetLevel.Set( from.skillTargetLevel );


		//return clone;
	}

	public void reset()
	{
		originalAtkSpeedRate.Set(1);
		originalMoveSpeedRate.Set(1);

		atkRange.Set( 0 );
		atkSpeed.Set( 0 );
		atkPhysic.Set( 0 );
		atkMagic.Set( 0 );
		defPhysic.Set( 0 );
		defMagic.Set( 0 );
		
		spRecovery.Set( 0 );
		mpRecovery.Set( 0 );
		
		speed.Set( 0 );
		
		summonSpPercent.Set( 0 );
		unitHpUp.Set( 0 );
		unitDefUp.Set( 0 );
		skillMpDiscount.Set( 0 );
		skillAtkUp.Set( 0 );
		skillUp.Set( 0 );
		skillTimeUp.Set( 0 );

		monsterType = Monster.TYPE.NONE;

		playerType = PlayerType.NotPlayer;

		baseLevel.Set( 0 );

		reinforceLevel.Set( 1 );
		maxHp.Set( 0 );

	}


	public Xfloat changeAnimationSpeed(Xfloat applySpeed, Xfloat originalSpeed)
	{
		return applySpeed / originalSpeed;
	}
}