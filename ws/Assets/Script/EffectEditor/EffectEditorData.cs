using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
[System.Serializable]
public class EffectEditorData : MonoBehaviour
{
	public float effectResizeRatio = 1.0f;

	[SerializeField]
	public float scaleFactor = 100;

	public float unitX = 500;
	public float enemyX = 1000;

	// 기존이 되는 모델링.
	[SerializeField]
	public GameObject modeling;


	[SerializeField]
	public List<EffectAniEditData> data = new List<EffectAniEditData>();

	public static bool containAniData(string aniName, List<EffectAniEditData> d)
	{
		foreach(EffectAniEditData ed in d)
		{
			if(aniName == ed.aniName) return true;
		}

		return false;
	}


	public static EffectAniEditData getAniData(string aniName, List<EffectAniEditData> d)
	{
		foreach(EffectAniEditData ed in d)
		{
			if(aniName == ed.aniName) return ed;
		}
		
		return null;
	}


	public void SetDirty ()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
		#endif
	}

	//atk 같은 개별 애니메이션의 데이터들.
	[System.Serializable]
	public class EffectAniEditData
	{
		public string aniName = string.Empty;

		// 한 애니메이션이 작동될때 뿌려지는 애니메이션 내용.
		[SerializeField]
		public AniData aniData = new AniData();

		public int atkType = 1;

		// 해당 애니메이션이 동작할때 
		[SerializeField]
		public EffectEditBulletData bulletData = new EffectEditBulletData();
	}


	[System.Serializable]
	public class EffectEditBulletData
	{
		public bool use = false;

		[SerializeField]
		public List<int> actionFrame = new List<int>();

		[SerializeField]
		public List<Vector3> shotPoint = new List<Vector3>();

		[SerializeField]
		public List<string> targetTransform = new List<string>();

		[SerializeField]
		public EffectEditBulletDetailData bulletEffect = new EffectEditBulletDetailData();

		public int attackType = 0;

		public static string[] atkTypeList = new string[]
		{
			"0.없음",
			"1.근접단일",
			"2.근접범위",
			"3.직선발사 단일",
			"4.직선발사 범위",
			"5.직선발사 관통",
			"6.곡선발사 범위",
			"7.낙하 범위",
			"8.즉시 범위",
			"9.순간 위치고정",
			"10.지속 위치고정",
			"11.곡선지속위치고정",
			"12.지속 캐릭터붙임",
			"13.시한폭탄",
			"14.지뢰",
			"15.체인라이트닝",
			"16.화면전체",
			"17.메테오",
			"18.유도탄"
		};

		[SerializeField]
		List<int> op0 = new List<int>(7);
		[SerializeField]
		List<int> op1 = new List<int>(7);
		[SerializeField]
		List<int> op2 = new List<int>(7);
		[SerializeField]
		List<int> op3 = new List<int>(7);
		[SerializeField]
		List<int> op4 = new List<int>(7);
		[SerializeField]
		List<int> op5 = new List<int>(7);
		[SerializeField]
		List<int> op6 = new List<int>(7);
		[SerializeField]
		List<int> op7 = new List<int>(7);
		[SerializeField]
		List<int> op8 = new List<int>(7);
		[SerializeField]
		List<int> op9 = new List<int>(7);
		[SerializeField]
		List<int> op10 = new List<int>(7);
		[SerializeField]
		List<int> op11 = new List<int>(7);
		[SerializeField]
		List<int> op12 = new List<int>(7);
		[SerializeField]
		List<int> op13 = new List<int>(7);
		[SerializeField]
		List<int> op14 = new List<int>(7);
		[SerializeField]
		List<int> op15 = new List<int>(7);
		[SerializeField]
		List<int> op16 = new List<int>(7);
		[SerializeField]
		List<int> op17 = new List<int>(7);
		[SerializeField]
		List<int> op18 = new List<int>(7);

		[SerializeField]
		public bool setAtkOption = false;

		public List<int> atkTypeOptions(int index)
		{
			List<int> returnList = null;

			switch(index)
			{
			case 0: returnList = op0; break;
			case 1: returnList = op1; break;
			case 2: returnList = op2; break;
			case 3: returnList = op3; break;
			case 4: returnList = op4; break;
			case 5: returnList = op5; break;
			case 6: returnList = op6; break;
			case 7: returnList = op7; break;
			case 8: returnList = op8; break;
			case 9: returnList = op9; break;
			case 10: returnList = op10; break;
			case 11: returnList = op11; break;
			case 12: returnList = op12; break;
			case 13: returnList = op13; break;
			case 14: returnList = op14; break;
			case 15: returnList = op15; break;
			case 16: returnList = op16; break;
			case 17: returnList = op17; break;
			case 18: returnList = op18; break;
		
			}

			if(returnList.Count < 8)
			{
				for(int i = 0; i < 8; ++i)
				{
					returnList.Add(0);
				}
			}

			return returnList;

		}

	}


	[System.Serializable]
	public class EffectEditBulletDetailData
	{
		public enum Type
		{
			Particle, Object, Indie // E, P, IE
		}

		public enum DestroyEffType
		{
			Normal, BulletRotation
		}

		[SerializeField]
		public Type type = Type.Particle;

		[SerializeField]
		public GameObject effect;

		[SerializeField]
		public GameObject goHitEffect;
		
		[SerializeField]
		public GameObject goDestroyEffect;

		[SerializeField]
		public DestroyEffType destroyOption = DestroyEffType.Normal;

		[SerializeField]
		public GameObject goGroundEffect;

		[SerializeField]
		public ElectricEffect chainLighting;

		public bool attachedToParent = true;

		public bool useOption = true;
	}


}
