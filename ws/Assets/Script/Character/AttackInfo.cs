public class AttackInfo
{
	public Monster shooter;
	public MonsterStat stat = new MonsterStat(); // 실제 데미지 등에 적용할 스탯.
	public MonsterStat originalStat = new MonsterStat(); // 데미지 공식등에 적용 받지 않고 총알등이 생성될때 입력된 오리지널 수치.

	public int uniqueId;
	public bool isSkillType;

	public void clear()
	{
		shooter = null;
		uniqueId = -1;
		stat.reset();
		originalStat.reset();
	}
}
