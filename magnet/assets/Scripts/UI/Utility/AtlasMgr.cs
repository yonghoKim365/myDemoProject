using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolAtlasType
{
    Skill,
    EquipItem,
    LoadAni,
    
    Img,
    Bod,
    Loading,
    Btn,

    Max,
    //MaxNum = Max+4
}

public enum LoadAtlasType//읽어오는 아틀라스들 후에 파괴도 되는 녀석들.
{
    //Img_0,
    NewCommon,
    Face,
    UseItem,
    Item,
    Shard,
    GuildMark,
    Buff,
    Skill,

    Equip,//아틀라스 이름 빼오는 목적이 아님.
    Equip_P,
    Equip_F,
    Equip_D,

    Skill_P,
    Skill_F,
    Skill_D,
}

public class AtlasMgr : Immortal<AtlasMgr> {
    /*
	public static Dictionary<string , UIAtlas> AtlaseDic = new Dictionary<string, UIAtlas>();

    // TODO : 임시코드
    public UIAtlas unitAtlas, unitAtlas2;
    public UIAtlas equipmentAtlas;
    public UIAtlas commonAtlas;
    */

    private Dictionary<UIAtlas, int> AtlasList = new Dictionary<UIAtlas, int>();
    private Dictionary<UITexture, int> TextureList = new Dictionary<UITexture, int>();
    private Dictionary<PoolAtlasType, UIAtlas> PoolAtlas = new Dictionary<PoolAtlasType, UIAtlas>();

    public Texture2D[] LightMapDatas;

    private string[] IgnoreAtlas = new string[(int)PoolAtlasType.Max];
    
    public override void Awake()
    {
        base.Awake();

        IgnoreAtlas[(int)PoolAtlasType.Img] = "Img";
        IgnoreAtlas[(int)PoolAtlasType.Bod] = "Bod";
        IgnoreAtlas[(int)PoolAtlasType.Loading] = "LoginAndSelectHero";
        IgnoreAtlas[(int)PoolAtlasType.Btn] = "Btn";
    }

    /// <summary> 캐릭터 선택시 사용. </summary>
    public void Load()
    {
        ClearJobType();
        PoolAtlas.Clear();

        for (PoolAtlasType i = PoolAtlasType.Skill; i < PoolAtlasType.Max; i++)
        {
            IgnoreAtlas[(int)i] = GetLoadAtlas(i).name;
        }
    }

    /// <summary> 캐릭터 변경시? </summary>
    void ClearJobType()
    {
        PoolAtlasType[] types = new PoolAtlasType[] {
            PoolAtlasType.EquipItem,
            PoolAtlasType.LoadAni,
            PoolAtlasType.Skill
        };

        for (int i = 0; i < types.Length; i++)
        {
            UIAtlas atlas = null;
            if (!PoolAtlas.TryGetValue(types[i], out atlas))
                continue;

            atlas.gameObject.SetActive(false);
            Resources.UnloadAsset(atlas.texture);
            Resources.UnloadAsset(atlas.spriteMaterial);
            atlas = null;

            PoolAtlas.Remove(types[i]);
        }

        PoolAtlas.Clear();
    }

    public List<UIAtlas> SaveAtlasList = new List<UIAtlas>();
    public List<UITexture> SaveTextureList = new List<UITexture>();

    public void SavePanelData(UIBasePanel basePanel)
    {
        /*
        if (basePanel.name.Contains("LoadingTipPanel"))
            return;

        List<UIAtlas> sps = basePanel.GetUseAtlas();
        List<UITexture> texts = basePanel.GetUseTexture();

        for (int i = 0; i < sps.Count; i++)
        {
            bool isAdd = true;
            for (int j = 0; j < IgnoreAtlas.Length; j++)
            {
                if (string.IsNullOrEmpty(IgnoreAtlas[j]))
                    continue;

                if (IgnoreAtlas[j].Equals(sps[i].name))
                {
                    isAdd = false;
                    break;
                }
            }

            if (!isAdd)
                continue;

            int count = 0;
            if (!AtlasList.TryGetValue(sps[i], out count))
                AtlasList.Add(sps[i], 1);
            else
                AtlasList[sps[i]] = count + 1;

            //Debug.Log(string.Format("<color=green>Add Atlas={0}</color>", sps[i].name));
        }

        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i] == null || texts[i].mainTexture == null)
                continue;
            else if (texts[i].mainTexture.name.Equals("PlayerRender"))
                continue;

            int count = 0;
            if (!TextureList.TryGetValue(texts[i], out count))
                TextureList.Add(texts[i], 1);
            else
                TextureList[texts[i]] = count + 1;

            //Debug.Log(string.Format("<color=green>Add Texture={0}</color>", texts[i].mainTexture.name));
        }

        if( !basePanel.IsPopup)
            StartCoroutine("LastFrame", new object[] { SaveAtlasList, SaveTextureList } );
        */
    }
    /*
    public void DeleteAllPanelData()
    {
        StopAllCoroutines();

        var atlas = AtlasList.GetEnumerator();
        while (atlas.MoveNext())
        {
            UIAtlas at = atlas.Current.Key;
            if (at == null)
                continue;

            //at.gameObject.SetActive(false);
            Resources.UnloadAsset(at.texture);
            Resources.UnloadAsset(at.spriteMaterial);
            //Debug.Log(string.Format("<color=green>Atlas UnLoadAsset = {0}</color>", at.name));
            at = null;
        }

        var texture = TextureList.GetEnumerator();
        while (texture.MoveNext())
        {
            UITexture tt = texture.Current.Key;
            if (tt == null)
                continue;

            //tt.gameObject.SetActive(false);
            Resources.UnloadAsset(tt.mainTexture);
            //Debug.Log(string.Format("<color=yellow>Texture UnLoadAsset = {0}</color>", tt.name));
            tt = null;
        }

        AtlasList.Clear();
        TextureList.Clear();

        SaveAtlasList.Clear();
        SaveTextureList.Clear();
    }
    */
    public void DeletePanelData(UIBasePanel basePanel)
    {
        /*
        if (basePanel.name.Contains("LoadingTipPanel"))
            return;

        //List<UIAtlas> atlasList = basePanel.GetUseAtlas();
        //List<UITexture> txtureList = basePanel.GetUseTexture();
        SaveAtlasList.AddRange(basePanel.GetUseAtlas());
        SaveTextureList.AddRange(basePanel.GetUseTexture() );
        */
    }

    IEnumerator LastFrame(object[] objs)
    {
        yield return new WaitForEndOfFrame();

        List<UIAtlas> atlasList = (List<UIAtlas>)objs[0];
        List<UITexture> txtureList = (List<UITexture>)objs[1];
        for (int i = 0; i < atlasList.Count; i++)
        {
            int count = 0;
            if (AtlasList.TryGetValue(atlasList[i], out count))
            {
                --count;
                if (count <= 0)
                {
                    //atlasList[i].gameObject.SetActive(false);

                    Resources.UnloadAsset(atlasList[i].texture);
                    Resources.UnloadAsset(atlasList[i].spriteMaterial);
                    //Debug.Log(string.Format("<color=green>Atlas UnLoadAsset = {0}</color>", atlasList[i].name) );
                    AtlasList.Remove(atlasList[i]);
                    atlasList[i] = null;
                    atlasList.RemoveAt(i);
                    --i;
                }
                else
                    AtlasList[atlasList[i]] = count;
            }
        }

        for (int i = 0; i < txtureList.Count; i++)
        {
            if (txtureList[i] == null || txtureList[i].mainTexture == null)
                continue;

            int count = 0;
            if (TextureList.TryGetValue(txtureList[i], out count))
            {
                --count;
                if (count <= 0)
                {
                    //txtureList[i].gameObject.SetActive(false);
                    Resources.UnloadAsset(txtureList[i].mainTexture);
                    TextureList.Remove(txtureList[i]);
                    
                    //Debug.Log(string.Format("<color=yellow>Texture UnLoadAsset = {0}</color>", txtureList[i].name));
                    //txtureList[i] = null;
                    txtureList.RemoveAt(i);
                    --i;
                }
                else
                    TextureList[txtureList[i]] = count;
            }
        }

        atlasList.Clear();
        txtureList.Clear();
        yield return null;
    }

    /// <summary> 아틀라스 타입에 맞게 반환해준다. 없으면 로드 후 보냄. </summary>
    public UIAtlas GetLoadAtlas(PoolAtlasType type)
    {
        UIAtlas atlas = null;
        if( !PoolAtlas.TryGetValue(type, out atlas) )
        {
            string atlasName = GetAtlasNameForType(type);
            atlas = Resources.Load(string.Format("UI/Atlases/{0}", atlasName), typeof(UIAtlas) ) as UIAtlas;

            if (atlas == null)
                Debug.LogError(string.Format("atlas not found error. path = 'UI/Atlases/{0}'", atlasName));
            else
                PoolAtlas.Add(type, atlas);
        }

        return atlas;
    }

    /// <summary> 아틀라스 읽어온다. </summary>
    public UIAtlas GetLoadAtlas(LoadAtlasType type)
    {
        string atlasName = GetAtlasNameForType(type);
        UIAtlas atlas = Resources.Load(string.Format("UI/Atlases/{0}", atlasName), typeof(UIAtlas)) as UIAtlas;

        if (atlas == null)
            Debug.LogError(string.Format("atlas not found error. path = 'UI/Atlases/{0}'", atlasName));
        else
        {
            if (!atlasName.Equals("Img") && !atlasName.Equals("Bod"))//이 두개는 놔둔다.
            {
                int count = 1;
                if (AtlasList.TryGetValue(atlas, out count))
                    AtlasList[atlas] = count + 1;
                else
                    AtlasList.Add(atlas, count);
            }
        }

        return atlas;
    }

    /// <summary> 내 직업군에 맞는지 검사 </summary>
    public UIAtlas GetEquipAtlasForCharIdx(uint charIdx)
    {
        if (NetData.instance.GetUserInfo()._userCharIndex.Equals(charIdx))
            return GetLoadAtlas(PoolAtlasType.EquipItem);
        else
        {
            LoadAtlasType type_0;
            switch (charIdx)
            {
                case 11000:
                    type_0 = LoadAtlasType.Equip_F;
                    break;

                case 12000:
                    type_0 = LoadAtlasType.Equip_P;
                    break;

                default:
                    type_0 = LoadAtlasType.Equip_D;
                    break;
            }

            return GetLoadAtlas(type_0);
        }
    }

    /// <summary> 내 직업군에 맞는지 검사 </summary>
    public UIAtlas GetSkillAtlasForCharIdx(uint charIdx)
    {
        LoadAtlasType type_0;
        switch (charIdx)
        {
            case 11000:
                type_0 = LoadAtlasType.Skill_F;
                break;

            case 12000:
                type_0 = LoadAtlasType.Skill_P;
                break;

            default:
                type_0 = LoadAtlasType.Skill_D;
                break;
        }

        return GetLoadAtlas(type_0);
    }

    /// <summary> 내 직업군에 맞는지 검사 </summary>
    public UIAtlas GetEquipAtlasForClassId(byte classId)
    {
        uint myCharId = NetData.instance.GetUserInfo()._userCharIndex;
        byte myClassId = 0;
        if (myCharId.Equals(11000))
            myClassId = 1;
        else if (myCharId.Equals(12000))
            myClassId = 2;
        else
            myClassId = 3;

        if (myClassId == classId)
            return GetLoadAtlas(PoolAtlasType.EquipItem);
        else
        {
            LoadAtlasType type_0;
            switch (classId)
            {
                case 1:
                    type_0 = LoadAtlasType.Equip_F;
                    break;

                case 2:
                    type_0 = LoadAtlasType.Equip_P;
                    break;

                default:
                    type_0 = LoadAtlasType.Equip_D;
                    break;
            }

            return GetLoadAtlas(type_0);
        }
    }

    /// <summary> 아틀라스 이름 찾기 외부에서 필요하면 public으로 변경해도 상관없음. </summary>
    private string GetAtlasNameForType(PoolAtlasType type)
    {
        string atlasName = "";
        switch (type)
        {
            case PoolAtlasType.LoadAni:
            case PoolAtlasType.EquipItem:
            case PoolAtlasType.Skill:
                if (NetData.instance.GetUserInfo() == null)//null일 수 있음.
                    break;

                uint charIdx = NetData.instance.GetUserInfo().GetCharIdx();
                if (charIdx == 11000)//권
                    atlasName = "F";
                else if (charIdx == 12000)//명
                    atlasName = "P";
                else if (charIdx == 13000)//협
                    atlasName = "D";

                switch (type)
                {
                    case PoolAtlasType.LoadAni:
                        atlasName = string.Format("Ani{0}", atlasName);
                        break;
                    case PoolAtlasType.EquipItem:
                        atlasName = string.Format("Item{0}", atlasName);
                        break;
                    case PoolAtlasType.Skill:
                        atlasName = string.Format("Skill{0}", atlasName);
                        break;
                }

                break;

            case PoolAtlasType.Img:
                atlasName = "Img";
                break;
            case PoolAtlasType.Bod:
                atlasName = "Bod";
                break;
            case PoolAtlasType.Loading:
                atlasName = "LoginAndSelectHero";
                break;
            case PoolAtlasType.Btn:
                atlasName = "Btn";
                break;
        }

        return atlasName;
    }

    /// <summary> 아틀라스 이름 찾기 외부에서 필요하면 public으로 변경해도 상관없음. </summary>
    private string GetAtlasNameForType(LoadAtlasType type)
    {
        string atlasName = "";
        switch (type)
        {
            case LoadAtlasType.NewCommon:
                atlasName = "NewCommon";
                break;
            case LoadAtlasType.UseItem:
                atlasName = "UseItem";
                break;
            case LoadAtlasType.Face:
                atlasName = "Face";
                break;
            case LoadAtlasType.Item:
                atlasName = "Item";
                break;
            case LoadAtlasType.Shard:
                atlasName = "Piece";
                break;
            case LoadAtlasType.GuildMark:
                atlasName = "Guildmark";
                break;
            case LoadAtlasType.Buff:
                atlasName = "Buff";
                break;
            case LoadAtlasType.Skill:
                atlasName = "Skill";
                break;

            case LoadAtlasType.Equip_F:
                atlasName = "ItemF";
                break;
            case LoadAtlasType.Equip_P:
                atlasName = "ItemP";
                break;
            case LoadAtlasType.Equip_D:
                atlasName = "ItemD";
                break;

            case LoadAtlasType.Skill_F:
                atlasName = "SkillF";
                break;
            case LoadAtlasType.Skill_P:
                atlasName = "SkillP";
                break;
            case LoadAtlasType.Skill_D:
                atlasName = "SkillD";
                break;
        }

        return atlasName;
    }

    /*
    public void DeletePanelData(UIBasePanel basePanel)
    {
        List<UISprite> sps = basePanel.GetUseSprite();
        List<UITexture> texts = basePanel.GetUseTexture();

        for (int i = 0; i < sps.Count; i++)
        {
            if (sps[i] == null || sps[i].atlas == null)
                continue;

            int count = 0;
            if (AtlasList.TryGetValue(sps[i].atlas, out count))
            {
                count -= 1;
                if (count <= 0)
                {
                    Resources.UnloadAsset(sps[i].atlas.texture);
                    Resources.UnloadAsset(sps[i].atlas.spriteMaterial);
                    AtlasList.Remove(sps[i].atlas);
                }
            }
            else
            {
                Resources.UnloadAsset(sps[i].atlas.texture);
                Resources.UnloadAsset(sps[i].atlas.spriteMaterial);
            }
        }

        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i] == null || texts[i].mainTexture == null)
                continue;
            
            int count = 0;
            if (TextureList.TryGetValue(texts[i].mainTexture, out count))
            {
                count -= 1;
                if (count <= 0)
                {
                    Resources.UnloadAsset(texts[i].mainTexture);
                    TextureList.Remove(texts[i].mainTexture);
                }
            }
            else
                Resources.UnloadAsset(texts[i].mainTexture);
        }

    }
    */
    /*
    UIAtlas NunitAtlas, NunitAtlas2;
    void Start () {
		//AtlaseDic.Clear();

        // TODO : 임시코드
        //AtlaseDic.Add("Unit_Ref", NunitAtlas);
        //AtlaseDic.Add( "EquipItem_Ref", equipmentAtlas );
        //AtlaseDic.Add( "NCommon_Ref", commonAtlas );

        //NunitAtlas = (Instantiate(unitAtlas.gameObject) as GameObject).GetComponent<UIAtlas>();
        //NunitAtlas2 = (Instantiate(unitAtlas2.gameObject) as GameObject).GetComponent<UIAtlas>();

        //NunitAtlas.transform.parent = this.transform;
        //NunitAtlas2.transform.parent = this.transform;

	}
	public void AddAtlaseDic(string key , UIAtlas val)
	{
		if(AtlaseDic.ContainsKey(key))
			AtlaseDic.Remove(key);
		AtlaseDic.Add(key,val);
	}

	public static UIAtlas GetAtlases(byte itemtype)
	{

        switch ((GK_AtlasType)itemtype)
		{
			case GK_AtlasType.EquipItem :   return AtlaseDic["EquipItem_Ref"];
            case GK_AtlasType.ItemData:     return AtlaseDic["NCommon_Ref"]; 
			case GK_AtlasType.Build :       return AtlaseDic["Build_Ref"]; 
			case GK_AtlasType.Ability :     return AtlaseDic["Etc_Ref"]; 
			case GK_AtlasType.Gacha :       return AtlaseDic["Etc_Ref"]; 
			case GK_AtlasType.Avatar :      return AtlaseDic["Etc_Ref"]; 
			case GK_AtlasType.UnitData :    return AtlaseDic["Unit_Ref"]; 
			case GK_AtlasType.RuneData :    return AtlaseDic["Rune_Ref"];
            case GK_AtlasType.InGame:       return AtlaseDic["InGame_Ref"];
		}
		
		return null;
	}

    public static UIAtlas GetAtlas(string name)
    {
        return AtlaseDic.ContainsKey( name ) ? AtlaseDic[name] : null;
    }

    public void SetUnitTex(UISprite sprite, string texname)
    {
        if (sprite.atlas == null)
            sprite.atlas = NunitAtlas;

        UISpriteData data = sprite.atlas.GetSprite(texname);
        if (data == null)
        {
            if (sprite.atlas == unitAtlas || sprite.atlas == NunitAtlas)
            {
                sprite.atlas = null;
                sprite.atlas = NunitAtlas2;
            }
            else
            {
                sprite.atlas = null;
                sprite.atlas = NunitAtlas;
            }
        }

        sprite.spriteName = texname;
    }
    */
}
