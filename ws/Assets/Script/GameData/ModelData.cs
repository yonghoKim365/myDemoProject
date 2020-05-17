using UnityEngine;
using System.Collections.Generic;

sealed public class ModelData
{

	public const string OUTLINE_RARE_NAME = "_linethick";
	public const string OUTLINE_NAME = "_line";

	public const int WITH_OUTLINE = 0;
	public const int WITH_RARELINE = 1;
	public const int WITHOUT_OUTLINE = 2;

	public const string eye = "_eye";
	public const string mouth = "_mouth";

	public string fileName = "";
	public int scale = 100; 
	public string shader = "Unlit/Texture";
	public bool hasCollider = true;
	public float px = 0.0f;
	public float py = 0.0f;
	public float pz = 0.0f;
	public float sx = 1.0f;
	public float sy = 1.0f;
	public float sz = 1.0f;

	public enum MergeType
	{
		All, Parts, Map, None
	}
	public MergeType mergeWithoutAtlas = MergeType.None;

	public float shadowSize = 0.0f;

	public float summonEffectSize = 1.0f;

	public float effectSize = 1.0f;

	public float width = 0.0f;
	public float height = 0.0f;

	public float shotScale = 0.0f;

	public float damageRange = 100.0f;
	public bool isOnce = false;

	public bool hasDeleteTime = true;

	public string[] textures = null;

	public float poseTime = 0.0f;

	// 카드 프레임 안에 고정될 y 위치.
	public float shotYPos = 0.0f;


	public float lobbyEffectSize = 1.0f;


	public enum ModelType
	{
		Normal, Weapon
	}

	public ModelType modelType = ModelType.Normal;

	public enum LobbySize
	{
		Small, Medium, Big
	}

	public LobbySize lobbySize = LobbySize.Medium;

	public Color[] particleColors;

	public bool useDefaultColor = false;
	public Color defaultColor;

	public int particleColorLength = 0;


	public bool useRimShader = false;

	
}