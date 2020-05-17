using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public partial class Monster : MonoBehaviour//, ITweenableObject
{
	public const string head = "head";
	public const string body = "body";
	public const string weapon = "weapon";
	public const string ride_01 = "ride_01";

	protected static Vector2 _vector2Zero = Vector2.zero;
	protected static Vector2 _vector2One = Vector2.one;
	protected static Vector2 _vector3Zero = Vector3.zero;	
	
	protected static Quaternion _defaultQ = new Quaternion(0,0,0,0);	

	static readonly float[] DAMAGE_MOTION_DURATION = new float[]{0.08f, 0.12f, 0.16f};

	const float DAMAGE_PARTICLE_NUM_RATIO = 0.1f;
	const int DAMAGE_PARTICLE_MIN_NUM = 1;
	const int DAMAGE_PARTICLE_MAX_NUM = 8;

	protected const float MONSTER_DAMAGE_TIME = 0.02f;

	public const string SHOOT_HEADER_FIRST_ALPHABET = "a";
	public const string SHOOT_HEADER = "atk";
	public const string SKILL_HEAD = "sk_";
	
	public const string WALK = "walk";
	public const string BWALK = "bwalk";
	public const string MWALK = "mwalk";

	public const string SHOOT = "atk";
	
	public const string ATK = "atk";
	public const string ATK1 = "atk1";
	public const string ATK2 = "atk2";
	public const string ATK3 = "atk3";
	public const string ATK4 = "atk4";
	public const string ATK5 = "atk5";
	public const string ATK6 = "atk6";
	public const string ATK7 = "atk7";
	public const string ATK8 = "atk8";
	public const string ATK9 = "atk9";
	public const string ATK10 = "atk10";
	public const string ATK11 = "atk11";
	public const string ATK12 = "atk12";
	public const string ATK13 = "atk13";
	public const string ATK14 = "atk14";
	public const string ATK15 = "atk15";
	public const string ATK16 = "atk16";
	public const string ATK17 = "atk17";
	public const string ATK18 = "atk18";
	public const string ATK19 = "atk19";
	
	public const string SKILL_START = "sk_s";
	public const string SKILL_LOOP = "sk_l";
	public const string SKILL_END = "sk_e";
	
	public const string SKILL_NORMAL = "sk_n";
	public const string SKILL_FORWARD = "sk_f";
	
	public static readonly string[] ATK_IDS = new string[20]{"atk","atk","atk2","atk3","atk4","atk5","atk6","atk7","atk8","atk9","atk10","atk11","atk12","atk13","atk14","atk15","atk16","atk17","atk18","atk19"};

	public static Dictionary<string,int> ATK_INDEX = null;

	public const string DEAD = "die";
	public const string SCENE_DEAD = "scene_die";

	public const string NORMAL = "normal";
	public const string DEAD_MOTION = "d";	

	public const string NORMAL_LOBBY = "lobbynormal";

	public const string DEFAULT_NORMAL = "default_normal";

	public const string WIN = "win";

	public const string LOADING = "loading";

}
