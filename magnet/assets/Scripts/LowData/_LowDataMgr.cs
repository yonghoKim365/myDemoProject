using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

public class ActionInfo
{
    public uint idx;
    public uint name;
    public uint descrpition;
    public uint Icon;
    public float GlobalCooltime;
    public ushort range;
    public ushort effectCallNotiIdx;
    public byte needtarget;
    public byte camera;
    public float casttime;
    public float skillpass;
    public float hittime;
    public float mergetime;
    public float watingtime;
    public byte pabody;
    public byte emit;

    public uint callChainIdx;
    public float callChainTime;

    public float cooltime;

    public uint callCastingBuffIdx;
    public float CastingBuffDurationTime;
}

public class AbilityData
{
    public uint Idx;
    public ushort notiIdx;
    public string targetEffect;
    public byte skillType;
    public byte applyTarget; // 0==나만. 1==그외, 2==논타겟
    public byte eCount;
    public byte targetCount;
    public ushort radius;
    public ushort angle;
    public byte baseFactor;
    public float eventValue;
    public ushort availableCnt;
    public uint targetSound;
    public byte cameraShake;
    public float pushTime;
    public float pushpower;

    //추가되야 하는 데이터
    public float factorRate;
    public float factor;
    public float ignoreDef;

    //액션 TFT 추가
    public float durationTime;
    public uint rate;
    public uint callAbilityIdx; //호출 프로젝타일 인덱스
    public float diePower;
	public float stiffenTime;
    public float dieMinDistance;
    public float dieMaxDistance;

    //확실치 않은것들
    public uint callBuffIdx;
    public uint superArmorDmg;
    public uint superArmorRecovery;
}

public partial class _LowDataMgr : Immortal<_LowDataMgr>
{
    static Character charLowData = new Character();
    static Item itemLowData = new Item();
    public static Local stringLowData = new Local();
    static Resource resourceLowData = new Resource();
    static Partner partnerLowData = new Partner();
    static Mob monsterLowData = new Mob();
    static NpcData npcLowData = new NpcData();
	static NonInteractiveNpcData nonInteractiveNpcLowData = new NonInteractiveNpcData();
    static SkillTables skillLowData = new SkillTables();
    static Newbie NewbieData = new Newbie();
    static DungeonTable dungeonData = new DungeonTable();
    static Etc etcData = new Etc();
    static Level levelLowData = new Level();
    static Mail mailLowData = new Mail();
    static Quest questLowData = new Quest();
    static Enchant enchantLowData = new Enchant();
    static MissionTable missionLowData = new MissionTable();
    static Map mapLowData = new Map();
    static PartnerLevel partnerLvLowData = new PartnerLevel();
    static Shop shopLowData = new Shop();
    static Price priceLowData = new Price();
    static Vip vipLowData = new Vip();
    static Icon iconLowData = new Icon();
    static Formula FormulaLowData = new Formula();
    static Guild guildLowData = new Guild();
    static GatchaReward gatchaRewardLowData = new GatchaReward();
    static Loading loadingLowData = new Loading();
    static Title titleLowData = new Title();
    static Welfare welfareLowData = new Welfare();
    static Achievement achievementLowData = new Achievement();
    static ActiveReward activiteLowData = new ActiveReward();
    static PVP pvpAutoReward = new PVP();

    //List<System.Action> LoadBigLowDatas = new List<System.Action>(); 
    List<System.Action> LoadSmallLowDatas = new List<System.Action>(); 
	public Dictionary<string,TextAsset> textAssetDic = new Dictionary<string, TextAsset>();

	static public int loadedLowDataCnt;

    public void LoadLowDataAllData(System.Action<float, string> OnProgress)
    {
        //현재 버전의 시리얼라이즈 된 데이터로 저장된 checksum과 와 현재 json파일의 checksum을 비교하여 다를경우 새로 serialize하고 해당 파일을 읽자
		LoadSmallLowDatas.Add(charLowData.DeserializeData);
		LoadSmallLowDatas.Add(itemLowData.DeserializeData);
        LoadSmallLowDatas.Add(stringLowData.DeserializeData);
        LoadSmallLowDatas.Add(resourceLowData.DeserializeData);
        LoadSmallLowDatas.Add(partnerLowData.DeserializeData);
        LoadSmallLowDatas.Add(monsterLowData.DeserializeData);
        LoadSmallLowDatas.Add(npcLowData.DeserializeData);
		LoadSmallLowDatas.Add(nonInteractiveNpcLowData.DeserializeData);
        LoadSmallLowDatas.Add(NewbieData.DeserializeData);
        LoadSmallLowDatas.Add(dungeonData.DeserializeData);
        LoadSmallLowDatas.Add(etcData.DeserializeData);
        LoadSmallLowDatas.Add(levelLowData.DeserializeData);
        LoadSmallLowDatas.Add(mailLowData.DeserializeData);
        LoadSmallLowDatas.Add(questLowData.DeserializeData);
        LoadSmallLowDatas.Add(enchantLowData.DeserializeData);
        LoadSmallLowDatas.Add(missionLowData.DeserializeData);
        LoadSmallLowDatas.Add(mapLowData.DeserializeData);
        LoadSmallLowDatas.Add(partnerLvLowData.DeserializeData);
        LoadSmallLowDatas.Add(shopLowData.DeserializeData);
        LoadSmallLowDatas.Add(priceLowData.DeserializeData);
        LoadSmallLowDatas.Add(vipLowData.DeserializeData);
        LoadSmallLowDatas.Add(iconLowData.DeserializeData);
        LoadSmallLowDatas.Add(FormulaLowData.DeserializeData);
        LoadSmallLowDatas.Add(guildLowData.DeserializeData);
        LoadSmallLowDatas.Add(loadingLowData.DeserializeData);
        LoadSmallLowDatas.Add(titleLowData.DeserializeData);
        LoadSmallLowDatas.Add(welfareLowData.DeserializeData);
        LoadSmallLowDatas.Add(achievementLowData.DeserializeData);
        LoadSmallLowDatas.Add(activiteLowData.DeserializeData);
        LoadSmallLowDatas.Add(pvpAutoReward.DeserializeData);
		LoadSmallLowDatas.Add(gatchaRewardLowData.DeserializeData);
		LoadSmallLowDatas.Add(skillLowData.DeserializeData);
        // use asset bundle
        //LoadSmallLowDataAll ();
        //OnProgress (1f, "");

        // use txt file
        StartCoroutine(LoadLowDataUpdate(OnProgress));


		//StartCoroutine(LoadLowDataUpdate(OnProgress));


    }

	public void DeserializeBigLowDatas()
	{
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		sw.Start ();
		
		float time1 = sw.ElapsedMilliseconds / 1000f;

		gatchaRewardLowData.DeserializeData2 ();

		float time2 = sw.ElapsedMilliseconds / 1000f;

		UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> gatchaRewardLowData.Deserialize time : " + (time2 - time1));

		skillLowData.DeserializeData2 ();

		float time3 = sw.ElapsedMilliseconds / 1000f;

		UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> skillLowData.Deserialize time : " + (time3 - time2));
		
		sw.Stop ();
	}
    

//	void LoadBigLowDataAll(){
//		for (int i=0; i<LoadBigLowDatas.Count; i++) {
//			LoadBigLowDatas[i]();
//		}
//	}

	public IEnumerator LoadAssetBundle ()
	{
		#if UNITY_EDITOR
		LoadAssetBundleEditor();
		#else
		yield return StartCoroutine(LoadAssetBundleAndroid());
		#endif
		
		yield return null;
	}
	void LoadAssetBundleEditor(){
		#if UNITY_ANDROID
		string path = System.IO.Path.Combine ( Application.dataPath, "Resources/CreateTextAssetbundle/SerializeData.txt_a" );
		#elif UNITY_IPHONE
		string path = System.IO.Path.Combine ( Application.dataPath, "Resources/CreateTextAssetbundle/SerializeData.txt_i" );
		#endif
		
		WWW www = new WWW ("file://" + path);
		
		if (www != null) {
			
			AssetBundle asset = www.assetBundle;
			
			UnityEngine.Object[] objs = asset.LoadAll (typeof(UnityEngine.TextAsset));
			
			foreach (UnityEngine.Object obj in objs) {
				TextAsset t = (TextAsset)obj;
				//Debug.Log("t.name:"+t.name);
				
				_LowDataMgr.instance.textAssetDic.Add (t.name, t);
			}
			
			asset.Unload (false);
		}
		
		www.Dispose ();
	}
	
	IEnumerator LoadAssetBundleAndroid(){
		
		#if UNITY_ANDROID
		string path = "jar:file://" + Application.dataPath + "!/assets/SerializeData.txt_a";
		#elif UNITY_IPHONE
		string path = "jar:file://" + Application.dataPath + "!/assets/SerializeData.txt_i";
		#endif
		
		yield return StartCoroutine (LoadAssetBundleWWW (path, 1));
	}
	
	IEnumerator LoadAssetBundleWWW ( string path, int version )
	{
		while ( !Caching.ready )
		{
			yield return null;
		}
		
		using ( WWW www = WWW.LoadFromCacheOrDownload ( path, version ) )
		{
			yield return www;
			
			if ( www.error != null )
			{
				Debug.Log ( "error : " + www.error.ToString () );
			}
			else
			{
				AssetBundle asset = www.assetBundle;
				
				UnityEngine.Object[] objs = asset.LoadAll (typeof(UnityEngine.TextAsset));
				
				foreach (UnityEngine.Object obj in objs) {
					TextAsset t = (TextAsset)obj;
					//Debug.Log("t.name:"+t.name+", t.size:"+t.bytes.Length);
					
					_LowDataMgr.instance.textAssetDic.Add (t.name, t);
				}
			}
			
			www.assetBundle.Unload ( false );
		}
	}


	void LoadSmallLowDataAll(){
		
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		sw.Start ();
		
		loadedLowDataCnt = 0;
		for (int i=0; i<LoadSmallLowDatas.Count; i++) {
			LoadSmallLowDatas[i]();
		}
		
		UnityEngine.Debug.Log ("<color=green>[StopWatch]</color> LoadSmallLowDataAll :" + sw.ElapsedMilliseconds / 1000f); // 1 sec
		sw.Stop ();
	}

    //< 부하가 걸리는걸 막기위해 순서대로 로딩시켜줌
    IEnumerator LoadLowDataUpdate(System.Action<float, string> OnProgress)
    {
		// use asset buldle
		/*
		OnProgress(0f, string.Format(""));

		yield return new WaitForSeconds (0.1f);
		
		float displayProgress = 0;
		float loadCnt = 0;

		while (loadCnt < 100) {

			displayProgress = loadCnt / 100f;
			OnProgress(displayProgress, string.Format("File Loading..."));

			loadCnt++;
			yield return null;
		}

		OnProgress(1f, string.Format("File Loading..."));

		yield return null;
*/

		OnProgress(0f, string.Format(""));
		
		yield return new WaitForSeconds (0.1f);
		
		loadedLowDataCnt = 0;
		int LowDataMax = 94 + 1;//89 + 1; //83;
		for (int i=0; i<LoadSmallLowDatas.Count; i++) {
			LoadSmallLowDatas[i]();
		}

		float displayProgress = 0;
		float toProgress = 0;
		
		while (loadedLowDataCnt < LowDataMax)
		{
			//Debug.Log(" loadedLowDataCnt :"+loadedLowDataCnt);

			toProgress = (float)(loadedLowDataCnt + 1) / (float)LowDataMax;
			
			//while (displayProgress < toProgress)
			{
				//++displayProgress;
				displayProgress = Mathf.Lerp(displayProgress, toProgress, 0.5f);
				//displayProgress = (int)Mathf.Lerp((float)(displayProgress*0.01f), (float)(toProgress * 0.01f), 0.5f);
				float value = displayProgress;
				OnProgress(value, string.Format("File Loading..."));
			}
			
			if (toProgress >= 1)
			{
				OnProgress(1f, string.Format("File Loading..."));
				break;
			}
			
			yield return null;
		}
		
		yield return null;
	}

	public void LoadLowDataALLForTableCheck()
	{
		charLowData = new Character ();
		/*
		itemLowData = new Item();
		stringLowData = new Local();
		resourceLowData = new Resource();
		partnerLowData = new Partner();
		monsterLowData = new Mob();
		npcLowData = new NpcData();
		nonInteractiveNpcLowData = new NonInteractiveNpcData ();
		skillLowData = new SkillTables();
		NewbieData = new Newbie();
		dungeonData = new DungeonTable();
		etcData = new Etc();
		levelLowData = new Level();
		mailLowData = new Mail();
		questLowData = new Quest();
		enchantLowData = new Enchant();
		missionLowData = new MissionTable();
		mapLowData = new Map();
		partnerLvLowData = new PartnerLevel();
		shopLowData = new Shop();
		priceLowData = new Price();
		vipLowData = new Vip();
		iconLowData = new Icon();
		FormulaLowData = new Formula();
		guildLowData = new Guild();
		gatchaRewardLowData = new GatchaReward();
		loadingLowData = new Loading();
		titleLowData = new Title();
		welfareLowData = new Welfare();
		achievementLowData = new Achievement();
		activeLowData = new ActiveReward();
		pvpAutoRewardLowData = new PVP();
		*/
		charLowData.LoadLowDataForTableCheck ();
	}
	
	
	
	/*
    public string pathForDocumentsFile(string filename)
    { 
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring( 0, path.LastIndexOf( '/' ) );
            return Path.Combine( Path.Combine( path, "Documents" ), filename );
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath; 
            path = path.Substring(0, path.LastIndexOf( '/' ) ); 
            return Path.Combine (path, filename);
        } 
        else 
        {
            string path = Application.dataPath; 
            path = path.Substring(0, path.LastIndexOf( '/' ) );
            return Path.Combine (path, filename);
        }
    }
    */

    public T DeserializeData<T>(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();

        TextAsset data = Resources.Load(string.Format("SerializeData/{0}", fileName)) as TextAsset;
        if (data != null)
        {
            //Debug.Log(string.Format("SerializeData/{0} Loading", fileName));

            using (Stream s = new MemoryStream(data.bytes))
            {
                return (T)bf.Deserialize(s);
                //s.Close(); - using을 써서 안해도됨
            }
        }
        //StartCoroutine(LoadAsyncResource<T>(fileName, (data)=> { }));

        return default(T);
    }

	/* // use asset bundle
	public void DeserializeData<T>(string fileName, System.Action<T> callback)
	{
		// use txt file
		DeserializeFromAsset<T> (fileName, callback);
		//return default(T);
	}
	
	public void DeserializeFromAsset<T>(string fileName, System.Action<T> callback){
		
		if (textAssetDic.ContainsKey (fileName)) {
			TextAsset data = textAssetDic[fileName];
			
			BinaryFormatter bf = new BinaryFormatter();
			using (Stream s = new MemoryStream(data.bytes))
			{
				callback((T)bf.Deserialize(s));
				loadedLowDataCnt++;
			}
		}
	}
	*/

	//public T DeserializeDataAsync<T>(string fileName, System.Action<T> callback)
	public T DeserializeData<T>(string fileName, System.Action<T> callback)
	{
		// coroutine
		StartCoroutine(LoadAsyncResource<T>(fileName, callback));
		
		return default(T);
	}


	float lastElapseTime;
    public IEnumerator LoadAsyncResource<T>(string fileName, System.Action<T> callBack)
    {

        ResourceRequest resReq = Resources.LoadAsync(string.Format("SerializeData/{0}", fileName), typeof(TextAsset));

        while (!resReq.isDone) { 
            yield return null; 
        }

        BinaryFormatter bf = new BinaryFormatter();
        TextAsset data = resReq.asset as TextAsset;

        try
        {
            using (Stream s = new MemoryStream(data.bytes))
            {
                callBack((T)bf.Deserialize(s));
                loadedLowDataCnt++;

                //Debug.Log("loadedLowDataCnt :"+loadedLowDataCnt);
            }
        }
        catch
        {

            Debug.LogError(fileName);
        }
    } 



	//public void LoadData(string fileName, object obj, System.Action action)
	//{
	//    string md5string;
	//    string serializemd5string;
	
	//    serializemd5string = PlayerPrefs.GetString(string.Format("{0}_MD5", fileName), "");
	
	//    if (serializemd5string.Equals(""))
	//    {
	//        //시리얼라이즈된 데이터가 없다 - 시리얼라이즈 시작
	
	//        //다르면 새로 시리얼라이즈
	//        action();
	//        //charLowData.LoadLowData();
	
	//        string Path = string.Format("{0}/SerializeData", Application.persistentDataPath);
	
	//        if (Directory.Exists(Path) == false)
	//        {
	//            Directory.CreateDirectory(Path);
	//        }
	
	//        FileStream fs = new FileStream(string.Format( "{0}/{1}.txt", Path, fileName), FileMode.Create, FileAccess.Write);
	//        BinaryFormatter bf = new BinaryFormatter();
	//        bf.Serialize(fs, obj);
	//        fs.Close();
	
	//        BenchMark.Mark("MD5_START"+ fileName);
	//        PlayerPrefs.SetString(string.Format("{0}_MD5", fileName), checkMD5(string.Format("TestJson/{0}", fileName)));
	//        BenchMark.Mark("MD5_END"+ fileName);
	//        BenchMark.BenchTime("MD5_START"+ fileName, "MD5_END"+ fileName);
	//    }
	//    else
	//    {
	//        //시리얼라이즈된 데이터가 있다 - md5체크
	//        md5string = checkMD5(string.Format("TestJson/{0}", fileName));
	
	//        if (serializemd5string.Equals(md5string))
	//        {
	//            //같으면 그냥 사용
	//            BinaryFormatter bf = new BinaryFormatter();
	
	//            TextAsset data = Resources.Load(string.Format("SerializeData/{0}", fileName)) as TextAsset;
	//            if (data != null)
	//            {
	//                Stream s = new MemoryStream(data.bytes);
	//                charLowData = (CharacterTableLowData)bf.Deserialize(s);
	//                s.Close();
	//            }
	//        }
	//        else
	//        {
	//            //다르면 새로 시리얼라이즈
	//            charLowData.LoadLowData();
	
	//            FileStream fs = new FileStream(string.Format("Assets/Resources/SerializeData/{0}.txt", fileName), FileMode.Create, FileAccess.Write);
	//            BinaryFormatter bf = new BinaryFormatter();
	//            bf.Serialize(fs, charLowData);
	//            fs.Close();
	
	//            PlayerPrefs.SetString(string.Format("{0}_MD5", fileName), checkMD5(string.Format("TestJson/{0}", fileName)));
	//        }
	//    }
	//}
	
	//public string checkMD5(string filename)
	//{
	//    using (var md5 = MD5.Create())
	//    {
	//        TextAsset data = Resources.Load(filename) as TextAsset;
	//        if (data != null)
	//        {
	//            using (var s = new MemoryStream(data.bytes))
	//            {
	//                return System.Convert.ToBase64String(md5.ComputeHash(s));
	//            }
	//        }
	//    }
	
	//    return "";
	//}

	////////////////////////////////////////////////////////////////////////////////////////
	
	
	//Character정보 헬퍼들
	/// <summary>
	/// charLowData의 데이터를 가져옴
	/// </summary>
	/// <param name="idx"></param>
	/// <returns></returns>
	public Character.CharacterInfo GetCharcterData(uint idx)
    {
        if (charLowData.CharacterInfoDic.ContainsKey(idx))
        {
            return charLowData.CharacterInfoDic[idx];
        }

        return null;

    }

    public Quest GetQuestData()
    {
        return questLowData;
    }

    public List<Character.CharacterInfo> GetCharacterDataToList()
    {
        return charLowData.CharacterInfoDic.Values.ToList();
    }

    //뉴비데이터 가져오기
    public Newbie.NewbieInfo GetNewbieCharacterData(uint idx)
    {
        if (NewbieData.NewbieInfoDic.ContainsKey(idx))
        {
            return NewbieData.NewbieInfoDic[idx];
        }

        return null;

    }

    //아이템정보 헬퍼들
    public List<Item.EquipmentInfo> GetClassItemPartsToList(byte ClassIndex, byte PartList)
    {
        List<Item.EquipmentInfo> returnData = new List<Item.EquipmentInfo>();

        var enumerator = itemLowData.EquipmentInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.Class == ClassIndex && enumerator.Current.Value.UseParts == PartList)
            {
                returnData.Add(enumerator.Current.Value);
            }
        }

        return returnData;
    }

    public Item.ItemInfo GetUseItem(uint index)
    {
        if (itemLowData.ItemInfoDic.ContainsKey(index))
        {
            return itemLowData.ItemInfoDic[index];
        }
        return null;
    }

    public List<Item.CostumeInfo> GetClassCostumToList(byte ClassIndex)
    {
        List<Item.CostumeInfo> returnData = new List<Item.CostumeInfo>();

        var enumerator = itemLowData.CostumeInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.Class == ClassIndex)
            {
                returnData.Add(enumerator.Current.Value);
            }
        }

        return returnData;
    }

    public Item.CostumeInfo GetLowDataCostumeInfo(uint index)
    {
        if (itemLowData.CostumeInfoDic.ContainsKey(index))
        {
            return itemLowData.CostumeInfoDic[index];
        }

        return null;
    }

    public Item.EquipmentInfo GetLowDataEquipItemInfo(uint index)
    {
        if (itemLowData.EquipmentInfoDic.ContainsKey(index))
        {
            return itemLowData.EquipmentInfoDic[index];
        }

        //Debug.LogError("'EquipmentInfo' not found itemData = " + index);
        return null;
    }

	//장비의 ItemValueInfo를 리턴한다.
	public Item.ItemValueInfo GetLowDataEquipItemValueInfo(uint _id, int _nOption)
	{

		Item.EquipmentInfo nextEquipmentInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo(_id);

		uint index = nextEquipmentInfo.BasicOptionIndex;
		if (_nOption == 1){
			index = nextEquipmentInfo.OptionIndex2;
		}
		else if (_nOption == 2){
			index = nextEquipmentInfo.OptionIndex3;
		}
		else if (_nOption == 3){
			index = nextEquipmentInfo.OptionIndex4;
		}

		if (index <= 0) {
			return null;
		}
		return  _LowDataMgr.instance.GetLowDataItemValueInfo(index);

	}

    //스트링관련 헬퍼들
    public string GetStringUnit(uint idx)
    {
        if (stringLowData.StringUnitInfoDic.ContainsKey(idx))
        {
            return stringLowData.StringUnitInfoDic[idx].String;
        }

        return null;
    }

    public string GetStringItem(uint idx)
    {
        if (stringLowData.StringItemInfoDic.ContainsKey(idx))
        {
            return stringLowData.StringItemInfoDic[idx].String;
        }

        return null;
    }

    //사용할 수 없는 금칙어를 입력 했는지 검사
    public bool IsBanString(string str)
    {
        List<string> banList = new List<string>();
        string changeLower = str.ToLower();
        int count = stringLowData.BanInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Local.BanInfo banInfo = stringLowData.BanInfoList[i];
            if (!changeLower.Contains(banInfo.word.ToLower()))
                continue;

            //string banText = "";
            //for (int j = 0; j < banInfo.word.Length; j++)
            //{
            //    banText += "*";
            //}

            //str = Regex.Replace(
            //str,
            //Regex.Escape(banInfo.word),
            //banText,
            //RegexOptions.IgnoreCase);
            return true;
        }

        return false;
    }

    //사용할 수 없는 금칙어를 입력 했는지 검사 사용할수 없는 단어라면 x로 표기
    public bool IsBanString(ref string changeStr)
    {
        List<string> banList = new List<string>();
        string changeLower = changeStr.ToLower();
        int count = stringLowData.BanInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Local.BanInfo banInfo = stringLowData.BanInfoList[i];
            if (!changeLower.Contains(banInfo.word.ToLower() ))
                continue;

            string banText = "";
            for(int j=0; j < banInfo.word.Length; j++)
            {
                banText += "*";
            }

            changeStr = Regex.Replace(
            changeStr,
            Regex.Escape(banInfo.word),
            banText,
            RegexOptions.IgnoreCase);
        }
        
        return true;
    }

    //파트너 데이터 헬퍼
    public Partner.PartnerDataInfo GetPartnerInfo(uint idx)
    {
        if (partnerLowData.PartnerDataInfoDic.ContainsKey(idx))
        {
            return partnerLowData.PartnerDataInfoDic[idx];
        }

        return null;
    }

    /// <summary> 대분류 type를 중심으로 등급, 별로 파트너를 찾는다. </summary>
    //public Partner.PartnerDataInfo GetPartnerInfo(byte type, byte quality, byte star)
    //{
    //    List<Partner.PartnerDataInfo> list = GetPartnerDataToList();
    //    for(int i=0; i < list.Count; i++)
    //    {
    //        if (list[i].Type != type || list[i].Quality != quality || list[i].Star != star)
    //            continue;

    //        return list[i];
    //    }

    //    return null;
    //}

    public List<Partner.PartnerDataInfo> GetPartnerDataToList()
    {
        return partnerLowData.PartnerDataInfoDic.Values.ToList();
    }
    //파트너스케일
    public Partner.PartnerScaleInfo GetPartnerScaleInfo(string prefabName, string panelName )
    {
        if (string.IsNullOrEmpty(panelName))
            return null;

        Partner.PartnerScaleInfo scaleInfo = new Partner.PartnerScaleInfo();
        scaleInfo = null;
        for (int i = 0; i < partnerLowData.PartnerScaleInfoList.Count; i++)
        {
            Partner.PartnerScaleInfo info = partnerLowData.PartnerScaleInfoList[i];
            if (prefabName.Contains(info.prefab) && panelName.Contains(info.panel_name))
            {
                scaleInfo = info;
                
            }
               
        }
        return scaleInfo;
    }

    //스테이지 정보 헬퍼
    public DungeonTable.StageInfo GetStageInfo(uint stageID)
    {
        if (dungeonData.StageInfoDic.ContainsKey(stageID))
        {
            return dungeonData.StageInfoDic[stageID];
        }

        return null;
    }

    //스테이지 정보 헬퍼
    public List<DungeonTable.StageInfo> GetStageInfoList()
    {
        return dungeonData.StageInfoDic.Values.ToList();
    }

    //퀘스트 대화씬 데이터가져옴
    public List<Quest.QuestTalkSceneInfo> GetQuestTalk(uint sceneID)
    {
        List<Quest.QuestTalkSceneInfo> talkScene = new List<Quest.QuestTalkSceneInfo>();

        for (int i = 0; i < questLowData.QuestTalkSceneInfoList.Count; i++)
        {
            if (questLowData.QuestTalkSceneInfoList[i].SceneID == sceneID)
            {
                talkScene.Add(questLowData.QuestTalkSceneInfoList[i]);
            }
        }

        return talkScene;
    }

    //난투장 테이블 데이터 뱉어줌
    public DungeonTable.FreefightTableInfo GetLowDataDogFight(uint lowDataID)
    {
        DungeonTable.FreefightTableInfo lowData = null;
        if (dungeonData.FreefightTableInfoDic.TryGetValue(lowDataID, out lowData))
            return lowData;

        return null;
    }

    /// <summary> 임시용 난투장 테이블 </summary>
    public DungeonTable.FreefightTableInfo GetLowDataFreeFight(string sceneName)
    {
        List<DungeonTable.FreefightTableInfo> list = dungeonData.FreefightTableInfoDic.Values.ToList();
        for(int i=0; i < list.Count; i++)
        {
            if (!list[i].StageName.Equals(sceneName))
                continue;

            return list[i];
        }

        return null;
    }

    public List<DungeonTable.FreefightTableInfo> GetLowDataFreeFightList()
    {
        return dungeonData.FreefightTableInfoDic.Values.ToList();
    }

    public Mob.PropInfo GetPropInfo(uint idx)
    {
        if (monsterLowData.PropInfoDic.ContainsKey(idx))
        {
            return monsterLowData.PropInfoDic[idx];
        }

        return null;
    }

    public Mob.PropGroupInfo GetPropGroup(uint groupIdx)
    {
        if (monsterLowData.PropGroupInfoDic.ContainsKey(groupIdx))
        {
            return monsterLowData.PropGroupInfoDic[groupIdx];
        }

        return null;
    }
    
    //public Mob.PropInfo GetPropInfo(uint idx)
    //{
    //    List<Mob.PropInfo> list = new List<Mob.PropInfo>();

    //    for(int i=0;i< monsterLowData.PropInfoList.Count; i++)
    //    {
    //        list.Add(monsterLowData.PropInfoList[i]);
    //    }

    //    if(list.Count == 1)
    //    {
    //        float Range = Random.Range(0f, 1000f);

    //        if(Range <= list[0].rate*0.1f)
    //            return list[0];

    //        return null;
    //    }
    //    else if (list.Count > 1)
    //    {
    //        float Range = Random.Range(0f, 1000f);

    //        float min = 0f;
    //        float max = 0f;

    //        for (int i = 0; i < list.Count; i++)
    //        {
    //            min = max;
    //            max = min + (list[i].rate * 0.1f);
    //            if(Range <= max)
    //            {
    //                return list[i];
    //            }
    //        }
    //    }


    //    return null;
    //}

    //몬스터정보 헬퍼
    public Mob.MobInfo GetMonsterInfo(uint idx)
    {
        if (monsterLowData.MobInfoDic.ContainsKey(idx))
        {
            return monsterLowData.MobInfoDic[idx];
        }

        return null;
    }

    public List<Mob.MobInfo> GetMonsterDataToList()
    {
        return monsterLowData.MobInfoDic.Values.ToList();
    }

    //NPC정보 헬퍼
    public NpcData.NpcInfo GetNPCInfo(ushort idx)
    {
        if (npcLowData.NpcInfoDic.ContainsKey(idx))
        {
            return npcLowData.NpcInfoDic[idx];
        }

        return null;
    }

	public int getNumOfNonInteractiveNpc(){
		return nonInteractiveNpcLowData.NonInteractiveNpcInfoDic.Count;
	}

	public NonInteractiveNpcData.NonInteractiveNpcDataInfo  getNonInteractiveNpc(ushort idx){
		if (nonInteractiveNpcLowData.NonInteractiveNpcInfoDic.ContainsKey (idx)) {
			return nonInteractiveNpcLowData.NonInteractiveNpcInfoDic[idx];
		}
		return null;
	}

    public List<NpcData.NpcInfo> GetNPCDataToList()
    {
        return npcLowData.NpcInfoDic.Values.ToList();
    }


    //애니정보 헬퍼
    public Resource.AniInfo GetAniResourceData(uint idx)
    {
        if (resourceLowData.AniInfoDic.ContainsKey(idx))
        {
            return resourceLowData.AniInfoDic[idx];
        }

        Debug.LogWarning("can not find key in " + resourceLowData.AniInfoDic.GetType() + " : " + idx);
        return null;
    }

    public string GetAnimName(uint id)
    {
        Resource.AniInfo aniInfo = GetAniResourceData(id);
        if (null == aniInfo || id == 0)
            return string.Empty;

        if (string.IsNullOrEmpty(aniInfo.aniName))
            return string.Empty;

        return (aniInfo.aniName == "0" || aniInfo.aniName == "test") ? string.Empty : aniInfo.aniName;
    }

    //사운드 헬퍼
    public static string GetSoundName(uint id)
    {
        if (resourceLowData.SoundInfoDic.ContainsKey(id))
        {
            return resourceLowData.SoundInfoDic[id].soundFile;
        }

        return null;
    }


    /// <summary>
    /// Common시트에 지정되어 있는 스트링 빼옴
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetStringCommon(uint id)
    {
        if (stringLowData.StringCommonInfoDic.ContainsKey(id))
            return stringLowData.StringCommonInfoDic[id].String;

        return null;
    }

	public Local.StringLocalDialogInfo GetLocalDialogInfo(uint id)
	{
		if (stringLowData.StringLocalDialogInfoDic.ContainsKey (id))
			return stringLowData.StringLocalDialogInfoDic [id];
		
		return null;
	}
	
	
	/// <summary>
	/// StringAchieve 지정되어 있는 스트링 빼옴
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetStringAchievement(uint id)
    {
        if (stringLowData.StringAchievementInfoDic.ContainsKey(id))
            return stringLowData.StringAchievementInfoDic[id].String;

        return null;
    }
    /*
    /// <summary>
    /// 아이템 셋트 정보 뱉어준다.
    /// </summary>
    /// <param name="itemIndex">해당아이템의 아이디 값</param>
    /// <returns></returns>
    public Item.EquipmentSetInfo GetItemSetLowData(uint itemIndex)
    {
        Dictionary<uint, Item.EquipmentSetInfo> setDic = itemLowData.EquipmentSetInfoDic;
        var enumerator = setDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Item.EquipmentSetInfo lowData = enumerator.Current.Value;
            if (lowData.ItemIdx1.CompareTo(itemIndex) != 0
                && lowData.ItemIdx2.CompareTo(itemIndex) != 0
                && lowData.ItemIdx3.CompareTo(itemIndex) != 0
                && lowData.ItemIdx4.CompareTo(itemIndex) != 0
                && lowData.ItemIdx5.CompareTo(itemIndex) != 0
                && lowData.ItemIdx6.CompareTo(itemIndex) != 0)//아이디 다르면 무시.
                continue;

            return lowData;//같다면 줌
        }

        return null;
    }
    */

    public Item.EquipmentSetInfo GetItemSetLowData(uint itemIndex)
    {
        Item.EquipmentSetInfo info = null;
        if (!itemLowData.EquipmentSetInfoDic.TryGetValue(itemIndex, out info))
            Debug.LogError("not found EquipmentSetIfno error " + itemIndex);
        
        return info;
    }

    /// <summary> 클레스에 맞는 아이템 셋트 리스트. </summary>
    public List<Item.EquipmentSetInfo> GetItemSetLowDataList(int classIdx)
    {
        List<Item.EquipmentSetInfo> setList = new List<Item.EquipmentSetInfo>();

        Dictionary<uint, Item.EquipmentSetInfo> setDic = itemLowData.EquipmentSetInfoDic;
        var enumerator = setDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if(enumerator.Current.Value.Class != classIdx)
                continue;

            setList.Add(enumerator.Current.Value);
        }

        return setList;
    }

    public Resource.AniInfo SetAnimation(GameObject model, string path, Resource.AniInfo AniInfo)
    {
        //이것도 나중엔 어셋번들에서 읽어와야한다.. 지금은 그냥 Resource에서 읽기

        if (AniInfo.aniName.Equals("none"))
            return AniInfo;

        if (model == null)
        {
            Debug.LogError("SSSSSSSSSSSSSSSSSSSSS " + path+ " " + AniInfo.aniName);
            return null;
        }

        AnimationClip clip = model.animation.GetClip(AniInfo.aniName);

        if (clip != null)
        {
            //이미 있다 그냥리턴
            return AniInfo;
        }

        //해당 에니메이션을 찾아서 연결해준후 리턴해야한다
        //아 경로를 어케찾나 - 코스튬이름??
        clip = Resources.Load(string.Format("{0}/{1}", path, AniInfo.aniName)) as AnimationClip;

        if (clip == null)
        {
            Debug.LogWarning(string.Format("NotFound AnimationClip:{0}", AniInfo.aniName));
            return AniInfo;
        }
        model.animation.AddClip(clip, clip.name);
        return AniInfo;
    }

	public IEnumerator SetAnimationAsync(GameObject model, string path, Resource.AniInfo AniInfo, System.Action<Resource.AniInfo> callback)
	{
		//이것도 나중엔 어셋번들에서 읽어와야한다.. 지금은 그냥 Resource에서 읽기
		
		if (AniInfo.aniName.Equals ("none")) {
			callback(AniInfo);
			yield break;
		}
		
		AnimationClip clip = model.animation.GetClip(AniInfo.aniName);
		
		if (clip != null)
		{
			//이미 있다 그냥리턴
			callback(AniInfo);
			yield break;
		}
		
		//해당 에니메이션을 찾아서 연결해준후 리턴해야한다
		//아 경로를 어케찾나 - 코스튬이름??
		ResourceRequest resReq = Resources.LoadAsync (string.Format ("{0}/{1}", path, AniInfo.aniName), typeof(AnimationClip));
		while (!resReq.isDone) { 
			yield return null; 
		}

		clip = resReq.asset as AnimationClip;

		if (clip == null)
		{
			Debug.LogWarning(string.Format("NotFound AnimationClip:{0}", AniInfo.aniName));
			callback(AniInfo);
			yield break;
		}
		model.animation.AddClip(clip, clip.name);
		callback(AniInfo);

		yield return null; 
	}


    //인게임용 애니메이션 셋팅
    public Resource.AniInfo[] AniInfoSetting(string baseCostumePrefName, GameObject model, uint idx)
    {
        if (resourceLowData.UnitInfoDic.ContainsKey(idx))
        {
            Resource.UnitInfo unitInfo = resourceLowData.UnitInfoDic[idx];

            Resource.AniInfo[] animDatas = new Resource.AniInfo[(int)eAnimName.Anim_Max];

            //공용애니메이션 패스
            string CommonAnimationPath = "";
            //코스튬 스킬애니메이션 패스
            string SkillAnimationPath = "";
            if (baseCostumePrefName.Contains("pc_f"))
            {
                //권사일 경우
                CommonAnimationPath = string.Format("Character/Animations/pc_f_common");
                SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("pc_p"))
            {
                //포졸일 경우
                CommonAnimationPath = string.Format("Character/Animations/pc_p_common");
                SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("pc_d"))
            {
                //의사일경우 
                CommonAnimationPath = string.Format("Character/Animations/pc_d_common");
                SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("par_"))
            {
                //파트너일 경우
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
                SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("npc_"))
            {

                //몬스터일 경우1
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
                SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("m_"))
            {
                //몬스터일 경우
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
                SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }

            animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
            animDatas[(int)eAnimName.Anim_move] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniRun));
            animDatas[(int)eAnimName.Anim_walk] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.aniWalk));
            animDatas[(int)eAnimName.Anim_damage] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniHit01));
            animDatas[(int)eAnimName.Anim_die] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniDie01));
            animDatas[(int)eAnimName.Anim_stand] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniStand));
            animDatas[(int)eAnimName.Anim_down] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniDown));
            animDatas[(int)eAnimName.Anim_stun] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniStun));
            animDatas[(int)eAnimName.Anim_victory] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniVictory));
            animDatas[(int)eAnimName.Anim_battle_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.battleAniIdle01));

            animDatas[(int)eAnimName.Anim_lose] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFail));
            animDatas[(int)eAnimName.Anim_lose_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFailIdle));

            animDatas[(int)eAnimName.Anim_intro] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniSpecial));

            animDatas[(int)eAnimName.Anim_attack1] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.AniAttack01));
            animDatas[(int)eAnimName.Anim_attack2] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.AniAttack02));
            animDatas[(int)eAnimName.Anim_attack3] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.AniAttack03));
            animDatas[(int)eAnimName.Anim_attack4] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.AniAttack04));

            animDatas[(int)eAnimName.Anim_skill1] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill01));
            animDatas[(int)eAnimName.Anim_skill2] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill02));
            animDatas[(int)eAnimName.Anim_skill3] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill03));
            animDatas[(int)eAnimName.Anim_skill4] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill04));
            animDatas[(int)eAnimName.Anim_skill5] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill05));
            animDatas[(int)eAnimName.Anim_skill6] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill06));
            animDatas[(int)eAnimName.Anim_skill7] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill07));
            animDatas[(int)eAnimName.Anim_skill8] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.AniSkill08));

            animDatas[(int)eAnimName.Anim_Chain] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.AniChain));
            animDatas[(int)eAnimName.Anim_Extra] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.ExtraAni));

            animDatas[(int)eAnimName.Anim_intro_start] = SetAnimation(model, SkillAnimationPath, GetAniResourceData(unitInfo.aniIntroStart));
            return animDatas;
        }

        return null;
    }


    //나 타운유닛용 인포
    public Resource.AniInfo[] TownAniInfoSetting(string baseCostumePrefName, GameObject model, uint idx, bool isUI)
    {
        if (resourceLowData.UnitInfoDic.ContainsKey(idx))
        {
            Resource.UnitInfo unitInfo = resourceLowData.UnitInfoDic[idx];

            Resource.AniInfo[] animDatas = new Resource.AniInfo[(int)eAnimName.Anim_Max];

            //공용애니메이션 패스
            string CommonAnimationPath = "";
            //코스튬 스킬애니메이션 패스
            //string SkillAnimationPath = "";
            if (baseCostumePrefName.Contains("pc_f"))
            {
                //권사일 경우
                CommonAnimationPath = string.Format("Character/Animations/pc_f_common");
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("pc_p"))
            {
                //권사일 경우
                CommonAnimationPath = string.Format("Character/Animations/pc_p_common");
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("pc_d"))
            {
                //의사일경우 
                CommonAnimationPath = string.Format("Character/Animations/pc_d_common");
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("par_"))
            {
                //파트너일 경우
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("m_b_"))
            {
                //몬스터일 경우
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("npc_"))
            {
                //npc일경우
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }


            if (isUI)//다른(사용하지 않는) 애니메이션 추가안하려고 추가.
            {
                animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
            }
            else
            {
                animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
                animDatas[(int)eAnimName.Anim_move] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniRun));
                animDatas[(int)eAnimName.Anim_intro] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntro));
            }
            //animDatas[(int)eAnimName.Anim_walk] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniWalk));
            //animDatas[(int)eAnimName.Anim_victory] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniVictory));
            //animDatas[(int)eAnimName.Anim_battle_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.battleAniIdle01));
            //animDatas[(int)eAnimName.Anim_lose] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFail));
            ///animDatas[(int)eAnimName.Anim_lose_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFailIdle));

            return animDatas;
        }

        return null;
    }

    //프랍 에니메이션
    public Resource.AniInfo[] PropAniInfoSetting(string baseCostumePrefName, GameObject model, uint idx)
    {
        if (resourceLowData.UnitInfoDic.ContainsKey(idx))
        {
            Resource.UnitInfo unitInfo = resourceLowData.UnitInfoDic[idx];
            Resource.AniInfo[] animDatas = new Resource.AniInfo[(int)eAnimName.Anim_Max];

            string CommonAnimationPath = "";
            CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);

            animDatas[(int)eAnimName.Anim_die] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniDie01));
            return animDatas;
        }

        return null;
    }

	public IEnumerator UIAniInfoSettingAsync(string baseCostumePrefName, GameObject model, uint idx, byte state, System.Action<Resource.AniInfo[]> callback)
	{
		//System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		//sw.Start ();
		
		//Debug.Log (" UIAniInfoSetting start");
		
		if (resourceLowData.UnitInfoDic.ContainsKey(idx))
		{
			Resource.UnitInfo unitInfo = resourceLowData.UnitInfoDic[idx];
			
			Resource.AniInfo[] animDatas = new Resource.AniInfo[(int)eAnimName.Anim_Max];
			
			//공용애니메이션 패스
			string CommonAnimationPath = "";
			//코스튬 스킬애니메이션 패스
			//string SkillAnimationPath = "";
			if (baseCostumePrefName.Contains("pc_f"))
			{
				//권사일 경우
				CommonAnimationPath = string.Format("Character/Animations/pc_f_common");
				//SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
			}
			else if (baseCostumePrefName.Contains("pc_p"))
			{
				//권사일 경우
				CommonAnimationPath = string.Format("Character/Animations/pc_p_common");
				//SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
			}
			else if (baseCostumePrefName.Contains("pc_d"))
			{
				//의사일경우 
				CommonAnimationPath = string.Format("Character/Animations/pc_d_common");
				//SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
			}
			if (baseCostumePrefName.Contains("par_"))
			{
				//파트너일 경우
				CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
			}
			else if (baseCostumePrefName.Contains("m_b_"))
			{
				//몬스터일 경우s
				CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
				//SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
			}
			
			//Debug.Log (" UIAniInfoSetting , " + sw.ElapsedMilliseconds / 1000f);
			
			if (state == 1)//시작 연출용
			{
//				animDatas[(int)eAnimName.Anim_intro] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntro));
//				animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
//				animDatas[(int)eAnimName.Anim_intro_start] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntroStart));
//				animDatas[(int)eAnimName.Anim_intro_end] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntroEnd));

				yield return StartCoroutine( SetAnimationAsync (model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntro), 		(retVal)=>{
					animDatas[(int)eAnimName.Anim_intro] = retVal;
				}));
				yield return StartCoroutine( SetAnimationAsync (model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01),		(retVal)=>{
					animDatas[(int)eAnimName.Anim_idle] = retVal;
				}));
				yield return StartCoroutine( SetAnimationAsync (model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntroStart), (retVal)=>{
					animDatas[(int)eAnimName.Anim_intro_start] = retVal;
				}));
				yield return StartCoroutine( SetAnimationAsync (model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntroEnd), 	(retVal)=>{
					animDatas[(int)eAnimName.Anim_intro_end] = retVal;
				}));

			}
			else if(state == 2)//패배용
			{
				animDatas[(int)eAnimName.Anim_lose] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFail));
				animDatas[(int)eAnimName.Anim_lose_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFailIdle));
			}
			else if( state == 4)//승리용
			{
				animDatas[(int)eAnimName.Anim_victory] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniVictory));
				animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
			}
			else if(state == 5)//코스튬 장착 애니 추가
			{   //eAnimName.Anim_skill8 유저의 경우 강제로 이걸 코스튬 장착시 애니로 셋팅함.
				animDatas[(int)eAnimName.Anim_skill8] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.AniSkill08));
				animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
			}
			else//일반 적인
				animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
			
			
			//Debug.Log (" UIAniInfoSetting end, " + sw.ElapsedMilliseconds / 1000f);

			callback(animDatas);
		}
		
		//Debug.Log (" UIAniInfoSetting end, " + sw.ElapsedMilliseconds / 1000f);
		//sw.Stop ();
		
		yield return null;
	}

    //UI용 인포
    public Resource.AniInfo[] UIAniInfoSetting(string baseCostumePrefName, GameObject model, uint idx, byte state)
    {
		//System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
		//sw.Start ();

		//Debug.Log (" UIAniInfoSetting start");

        if (resourceLowData.UnitInfoDic.ContainsKey(idx))
        {
            Resource.UnitInfo unitInfo = resourceLowData.UnitInfoDic[idx];

            Resource.AniInfo[] animDatas = new Resource.AniInfo[(int)eAnimName.Anim_Max];

            //공용애니메이션 패스
            string CommonAnimationPath = "";
            //코스튬 스킬애니메이션 패스
            //string SkillAnimationPath = "";
            if (baseCostumePrefName.Contains("pc_f"))
            {
                //권사일 경우
                CommonAnimationPath = string.Format("Character/Animations/pc_f_common");
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("pc_p"))
            {
                //권사일 경우
                CommonAnimationPath = string.Format("Character/Animations/pc_p_common");
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("pc_d"))
            {
                //의사일경우 
                CommonAnimationPath = string.Format("Character/Animations/pc_d_common");
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            if (baseCostumePrefName.Contains("par_"))
            {
                //파트너일 경우
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }
            else if (baseCostumePrefName.Contains("m_b_"))
            {
                //몬스터일 경우s
                CommonAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
                //SkillAnimationPath = string.Format("Character/Animations/{0}", baseCostumePrefName);
            }

			//Debug.Log (" UIAniInfoSetting 20, " + sw.ElapsedMilliseconds / 1000f);
            
            if (state == 1)//시작 연출용
            {
                animDatas[(int)eAnimName.Anim_intro] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntro));
                animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
				animDatas[(int)eAnimName.Anim_intro_start] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntroStart));
				animDatas[(int)eAnimName.Anim_intro_end] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIntroEnd));
            }
            else if(state == 2)//패배용
            {
                animDatas[(int)eAnimName.Anim_lose] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFail));
                animDatas[(int)eAnimName.Anim_lose_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniFailIdle));
            }
            else if( state == 4)//승리용
            {
                animDatas[(int)eAnimName.Anim_victory] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniVictory));
                animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
            }
            else if(state == 5)//코스튬 장착 애니 추가
            {   //eAnimName.Anim_skill8 유저의 경우 강제로 이걸 코스튬 장착시 애니로 셋팅함.
                animDatas[(int)eAnimName.Anim_skill8] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.AniSkill08));
                animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));
            }
            else//일반 적인
                animDatas[(int)eAnimName.Anim_idle] = SetAnimation(model, CommonAnimationPath, GetAniResourceData(unitInfo.aniIdle01));


			//Debug.Log (" UIAniInfoSetting end, " + sw.ElapsedMilliseconds / 1000f);

            return animDatas;
        }

		//Debug.Log (" UIAniInfoSetting end, " + sw.ElapsedMilliseconds / 1000f);
		//sw.Stop ();

        return null;
    }
    /*
    //뉴비데이터 헬퍼
    public Item.EquipmentInfo GetDefaultItemInfo(uint charIdx, ePartType parts)
    {
        Newbie.NewbieInfo newbieData = null;
        //if (NewbieData.NewbieInfoDic.ContainsKey(charIdx))
        if (NewbieData.NewbieInfoDic.TryGetValue(charIdx, out newbieData))
        {
            //Newbie.NewbieInfo newbieData = NewbieData.NewbieInfoDic[charIdx];

            uint equipItemIdx = 0;

            switch (parts)
            {
                case ePartType.HELMET: equipItemIdx = newbieData.defaultItemidx1; break;
                case ePartType.CLOTH: equipItemIdx = newbieData.defaultItemidx2; break;
                case ePartType.WEAPON: equipItemIdx = newbieData.defaultItemidx3; break;
                case ePartType.SHOES: equipItemIdx = newbieData.defaultItemidx4; break;
                case ePartType.NECKLACE: equipItemIdx = newbieData.defaultItemidx5; break;
                case ePartType.RING: equipItemIdx = newbieData.defaultItemidx6; break;
            }

            if (equipItemIdx != 0)
            {
                return GetLowDataEquipItemInfo(equipItemIdx);
            }
        }

        Debug.LogError("not found newbieData error " + charIdx);
        return null;
    }
    */
    public Item.EquipmentInfo GetDefaultItemInfo(uint charIdx, ePartType parts)
    {
        List<Item.EquipmentSetInfo> list = GetItemSetLowDataList(UIHelper.GetClassType(charIdx));
        for(int i=0; i < list.Count; i++)
        {
            if (list[i].Default != 1)
                continue;

            uint equipItemIdx = 0;
            switch (parts)
            {
                case ePartType.HELMET: equipItemIdx = list[i].ItemIdx1; break;
                case ePartType.CLOTH: equipItemIdx = list[i].ItemIdx2; break;
                case ePartType.WEAPON: equipItemIdx = list[i].ItemIdx3; break;
                case ePartType.SHOES: equipItemIdx = list[i].ItemIdx4; break;
                case ePartType.NECKLACE: equipItemIdx = list[i].ItemIdx5; break;
                case ePartType.RING: equipItemIdx = list[i].ItemIdx6; break;
            }

            if (equipItemIdx != 0)
            {
                return GetLowDataEquipItemInfo(equipItemIdx);
            }
        }

        Debug.LogError("not found Defualt EquipmentSetInfo error " + charIdx);
        return null;
    }

    //스킬데이터 헬퍼
    public static SkillTables.SkillLevelInfo GetSkillLevelData(uint skillIdx, byte level)
    {

        for (int i = 0; i < skillLowData.SkillLevelInfoList.Count; i++)
        {
            if (skillLowData.SkillLevelInfoList[i].skillIdx == skillIdx && skillLowData.SkillLevelInfoList[i].skillLevel == (uint)level)
                return skillLowData.SkillLevelInfoList[i];
        }

        return null;
    }

    ////파트너버프스킬 
    //public static SkillTables.SkillLevelInfo GetByffSkillLevelData(uint BuffIdx)
    //{
    //    for (int i = 0; i < skillLowData.SkillLevelInfoList.Count; i++)
    //    {
    //        if (skillLowData.SkillLevelInfoList[i].callBuffIdx[0] == BuffIdx.ToString())
    //            return skillLowData.SkillLevelInfoList[i];
    //    }

    //    return null;
    //}

    /*
    public static NetData._SkillDataGrop GetSkillDataGrop(SkillData []skillData)
    {
        NetData._SkillDataGrop dataGroup = new NetData._SkillDataGrop();
        dataGroup.skillData = skillData;
        dataGroup.ActionDic = GetSkillActionDic(skillData);
        dataGroup.AbilityDic = GetSkillAbilityLevelData(skillData);
        
        return dataGroup;
    }
    */

    //평타데이터 까지 추가한거
    public static NetData._SkillDataGrop GetSkillDataGrop(SkillData[] normalAttackData, SkillData[] skillData)
    {
        NetData._SkillDataGrop dataGroup = new NetData._SkillDataGrop();
        dataGroup.normalAttackData = normalAttackData;
        dataGroup.skillData = skillData;

        dataGroup.ActionDic = new Dictionary<uint, ActionInfo>();
        dataGroup.AbilityDic = new Dictionary<uint, List<AbilityData>>();

        //dataGroup.ActionDic = GetSkillActionDic(skillData);
        SetSkillActionDic(ref dataGroup.ActionDic, normalAttackData);
        SetSkillActionDic(ref dataGroup.ActionDic, skillData);

        SetSkillAbilityLevelData(ref dataGroup.AbilityDic, normalAttackData);
        SetSkillAbilityLevelData(ref dataGroup.AbilityDic, skillData);
        //dataGroup.AbilityDic = GetSkillAbilityLevelData(skillData);

        /*
        if(BuffSkillData!=null)
        {
            dataGroup.buffData = BuffSkillData;
            //버프스킬의 경우 캐릭터/몬스터는 없기때문에 따로 체크
            SetSkillActionDic(ref dataGroup.ActionDic, BuffSkillData);
            SetSkillAbilityLevelData(ref dataGroup.AbilityDic, BuffSkillData);
        }
        */

        return dataGroup;
    }

    /// <summary> 보스 레이드용 스킬 데이터 </summary>
    public NetData._SkillDataGrop GetBossRaidSkillData(SkillData[] skillData)
    {
        NetData._SkillDataGrop dataGroup = new NetData._SkillDataGrop();

        dataGroup.skillData = skillData;

        dataGroup.ActionDic = new Dictionary<uint, ActionInfo>();
        dataGroup.AbilityDic = new Dictionary<uint, List<AbilityData>>();

        SetSkillActionDic(ref dataGroup.ActionDic, skillData);
        SetSkillAbilityLevelData(ref dataGroup.AbilityDic, skillData);

        return dataGroup;
    }

    public static SkillTables.ActionInfo GetSkillAction(uint skillIdx)
    {
        if (skillLowData.ActionInfoDic.ContainsKey(skillIdx))
        {
            return skillLowData.ActionInfoDic[skillIdx];
        }

        return null;
    }

    public static SkillTables.BuffInfo GetBuffData(uint buffIdx)
    {
        if (skillLowData.BuffInfoDic.ContainsKey(buffIdx))
        {
            return skillLowData.BuffInfoDic[buffIdx];
        }

        Debug.LogError(string.Format("not found 'BuffInfo' error {0}", buffIdx));
        return null;
    }

    public static bool SetSkillActionDic(ref Dictionary<uint, ActionInfo> dic, SkillData[] skillData)
    {
        for (int i = 0; i < skillData.Length; i++)
        {
            if (skillData[i] != null)
            {
                if (skillData[i]._SkillID != 0)
                {
                    if (skillLowData.ActionInfoDic.ContainsKey(skillData[i]._SkillID))
                    {
                        //넣기전에 스킬레벨 데이터까지 계산다해서 집어넣자
                        SkillTables.SkillLevelInfo levelInfo = GetSkillLevelData(skillData[i]._SkillID, skillData[i]._SkillLevel);
                        if (levelInfo == null)
                        {
                            ActionInfo applyAbility = new ActionInfo();
                            applyAbility.idx = skillLowData.ActionInfoDic[skillData[i]._SkillID].idx;
                            applyAbility.name = skillLowData.ActionInfoDic[skillData[i]._SkillID].name;
                            applyAbility.descrpition = skillLowData.ActionInfoDic[skillData[i]._SkillID].descrpition;
                            applyAbility.Icon = skillLowData.ActionInfoDic[skillData[i]._SkillID].Icon;
                            applyAbility.skillpass = 0;
                            //applyAbility.cooltime = skillLowData.ActionInfoDic[skillData[i]._SkillID].cooltime - levelInfo.cooltime;
                            applyAbility.GlobalCooltime = skillLowData.ActionInfoDic[skillData[i]._SkillID].GlobalCooltime;//skillLowData.ActionInfoDic[skillData[i]._SkillID].cooltime - levelInfo.cooltime;
                            applyAbility.range = skillLowData.ActionInfoDic[skillData[i]._SkillID].range;
                            applyAbility.effectCallNotiIdx = skillLowData.ActionInfoDic[skillData[i]._SkillID].effectCallNotiIdx;
                            applyAbility.needtarget = skillLowData.ActionInfoDic[skillData[i]._SkillID].needtarget;
                            applyAbility.camera = skillLowData.ActionInfoDic[skillData[i]._SkillID].camera;
                            applyAbility.callChainIdx = 0;
                            applyAbility.callChainTime = 0;
                            applyAbility.cooltime = 1f;

                            applyAbility.callCastingBuffIdx = 0;
                            applyAbility.CastingBuffDurationTime = 0f;

                            dic.Add(skillData[i]._SkillID, applyAbility);
                        }
                        else
                        {
                            ActionInfo applyAbility = new ActionInfo();
                            applyAbility.idx = skillLowData.ActionInfoDic[skillData[i]._SkillID].idx;
                            applyAbility.name = skillLowData.ActionInfoDic[skillData[i]._SkillID].name;
                            applyAbility.descrpition = skillLowData.ActionInfoDic[skillData[i]._SkillID].descrpition;
                            applyAbility.Icon = skillLowData.ActionInfoDic[skillData[i]._SkillID].Icon;
                            //applyAbility.cooltime = skillLowData.ActionInfoDic[skillData[i]._SkillID].cooltime - levelInfo.cooltime;
                            applyAbility.GlobalCooltime = skillLowData.ActionInfoDic[skillData[i]._SkillID].GlobalCooltime;//skillLowData.ActionInfoDic[skillData[i]._SkillID].cooltime - levelInfo.cooltime;
                            applyAbility.range = skillLowData.ActionInfoDic[skillData[i]._SkillID].range;
                            applyAbility.effectCallNotiIdx = skillLowData.ActionInfoDic[skillData[i]._SkillID].effectCallNotiIdx;
                            applyAbility.needtarget = skillLowData.ActionInfoDic[skillData[i]._SkillID].needtarget;
                            applyAbility.camera = skillLowData.ActionInfoDic[skillData[i]._SkillID].camera;
                            applyAbility.cooltime = levelInfo.cooltime;
                            applyAbility.skillpass = skillLowData.ActionInfoDic[skillData[i]._SkillID].skillpass;
                            //applyAbility.pushpower = skillLowData.ActionInfoDic[skillData[i]._SkillID].pushpower;
                            //applyAbility.skilltype = skillLowData.ActionInfoDic[skillData[i]._SkillID].skilltype;

                            applyAbility.callChainIdx = levelInfo.callChainIdx;
                            applyAbility.callChainTime = levelInfo.callChainTime;

                            applyAbility.callCastingBuffIdx = levelInfo.callCastingBuffIdx;
                            applyAbility.CastingBuffDurationTime = levelInfo.CastingBuffDurationTime;

                            if (!dic.ContainsKey(skillData[i]._SkillID))
                                dic.Add(skillData[i]._SkillID, applyAbility);
                            else
                            {
                                Debug.Log(string.Format("is already added skill index ( {0} ) error", skillData[i]._SkillID));
                            }
                        }

                    }
                }
            }
        }

        return true;
    }

    public static bool SetSkillAbilityLevelData(ref Dictionary<uint, List<AbilityData>> dic, SkillData[] skillData)
    {
        //따로 헬퍼함수로 뽑는데 이거 최적화 해줘야될듯
        for (int i = 0; i < skillLowData.AbilityInfoList.Count; i++)
        {
            for (int j = 0; j < skillData.Length; j++)
            {
                if (skillData[j] != null)
                {
                    if (skillData[j]._SkillID == 0)
                        continue;

                    if (skillLowData.AbilityInfoList[i].Idx == skillData[j]._SkillID)
                    {
                        if (!dic.ContainsKey(skillLowData.AbilityInfoList[i].Idx))
                            dic.Add(skillLowData.AbilityInfoList[i].Idx, new List<AbilityData>());

                        //해당 어빌리티를 계산해주자
                        SkillTables.SkillLevelInfo levelInfo = GetSkillLevelData(skillData[j]._SkillID, skillData[j]._SkillLevel);
                        if (levelInfo == null)
                        {
                            //해당 스킬레벨이 없으면 디폴트값으로 직접 ADD
                            AbilityData applyAbility = new AbilityData();
                            applyAbility.Idx = skillLowData.AbilityInfoList[i].Idx;
                            applyAbility.notiIdx = skillLowData.AbilityInfoList[i].notiIdx;
                            applyAbility.targetEffect = skillLowData.AbilityInfoList[i].targetEffect;
                            applyAbility.skillType = skillLowData.AbilityInfoList[i].skillType;
                            applyAbility.applyTarget = skillLowData.AbilityInfoList[i].applyTarget;
                            applyAbility.eCount = skillLowData.AbilityInfoList[i].eCount;
                            applyAbility.targetCount = skillLowData.AbilityInfoList[i].targetCount;
                            applyAbility.radius = skillLowData.AbilityInfoList[i].radius;
                            applyAbility.angle = skillLowData.AbilityInfoList[i].angle;
                            applyAbility.baseFactor = skillLowData.AbilityInfoList[i].baseFactor;
                            applyAbility.eventValue = skillLowData.AbilityInfoList[i].eventValue;

                            applyAbility.availableCnt = skillLowData.AbilityInfoList[i].availableCnt;
                            applyAbility.targetSound = skillLowData.AbilityInfoList[i].targetCount;
                            applyAbility.cameraShake = 0;
                            applyAbility.pushTime = skillLowData.AbilityInfoList[i].pushTime;
                            applyAbility.pushpower = skillLowData.AbilityInfoList[i].pushpower;

                            //추가되야 하는 데이터
                            applyAbility.factorRate = 1f;
                            applyAbility.factor = 0f;
                            applyAbility.ignoreDef = 0f;                           

                            //확실치 않은것들
                            applyAbility.callBuffIdx = 0;
                            applyAbility.superArmorDmg = 0;
                            applyAbility.superArmorRecovery = 0;

                            //액션 TFT추가
                            applyAbility.durationTime = 0;
                            applyAbility.rate = 0;
                            applyAbility.callAbilityIdx = 0;
                            applyAbility.stiffenTime = 0;
                            applyAbility.dieMinDistance = 0;
                            applyAbility.dieMaxDistance = 0;

                            Debug.Log(string.Format("skillLevel이 존재하지 않음 ( {0}-{1} )", skillData[j]._SkillID, skillData[j]._SkillLevel));

                            dic[skillLowData.AbilityInfoList[i].Idx].Add(applyAbility);
                        }
                        else
                        {
                            AbilityData applyAbility = new AbilityData();
                            applyAbility.Idx = skillLowData.AbilityInfoList[i].Idx;
                            applyAbility.notiIdx = skillLowData.AbilityInfoList[i].notiIdx;
                            applyAbility.targetEffect = skillLowData.AbilityInfoList[i].targetEffect;
                            applyAbility.skillType = skillLowData.AbilityInfoList[i].skillType;
                            applyAbility.applyTarget = skillLowData.AbilityInfoList[i].applyTarget;
                            applyAbility.eCount = skillLowData.AbilityInfoList[i].eCount;
                            applyAbility.targetCount = skillLowData.AbilityInfoList[i].targetCount;
                            applyAbility.radius = skillLowData.AbilityInfoList[i].radius;
                            applyAbility.angle = skillLowData.AbilityInfoList[i].angle;
                            applyAbility.baseFactor = skillLowData.AbilityInfoList[i].baseFactor;
                            applyAbility.eventValue = skillLowData.AbilityInfoList[i].eventValue;
                            applyAbility.availableCnt = skillLowData.AbilityInfoList[i].availableCnt;
                            applyAbility.targetSound = skillLowData.AbilityInfoList[i].targetCount;
                            applyAbility.cameraShake = skillLowData.AbilityInfoList[i].cameraShake;
                            applyAbility.pushTime = skillLowData.AbilityInfoList[i].pushTime;
                            applyAbility.pushpower = skillLowData.AbilityInfoList[i].pushpower;

                            try
                            {
                                if (levelInfo.callBuffIdx[applyAbility.notiIdx - 1] != null)
                                {
                                    uint.TryParse(levelInfo.callBuffIdx[applyAbility.notiIdx - 1].ToString(), out applyAbility.callBuffIdx);
                                }
                                else
                                {
                                    Debug.Log(string.Format("skillLevel데이터에 callBuffIdx 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                }
                            }
                            catch 
                            {
                                Debug.Log(string.Format("skillLevel데이터에 callBuffIdx 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                applyAbility.callBuffIdx = 0;
                            }

                            try
                            {
                                if (levelInfo.SuperArmorRecovery[applyAbility.notiIdx - 1] != null)
                                {
                                    uint.TryParse(levelInfo.SuperArmorRecovery[applyAbility.notiIdx - 1].ToString(), out applyAbility.superArmorRecovery);
                                }
                                else
                                {
                                    Debug.Log(string.Format("skillLevel데이터에 superArmorRecovery 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                }
                            }
                            catch
                            {
                                Debug.Log(string.Format("skillLevel데이터에 superArmorRecovery 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                applyAbility.superArmorRecovery = 0;
                            }

                            try
                            {
                                if (levelInfo.SuperArmorDamage[applyAbility.notiIdx - 1] != null)
                                {
                                    uint.TryParse(levelInfo.SuperArmorDamage[applyAbility.notiIdx - 1].ToString(), out applyAbility.superArmorDmg);
                                }
                                else
                                {
                                    Debug.Log(string.Format("skillLevel데이터에 SuperArmorDamage 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                }
                            }
                            catch
                            {
                                Debug.Log(string.Format("skillLevel데이터에 SuperArmorDamage 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                applyAbility.superArmorDmg = 0;
                            }


                            //액션 TFT추가
                            applyAbility.durationTime = levelInfo.durationTime;
                            applyAbility.rate = levelInfo.rate;
                            applyAbility.callAbilityIdx = levelInfo.callAbilityIdx;
                            applyAbility.stiffenTime = skillLowData.AbilityInfoList[i].stiffenTime;
                            applyAbility.dieMinDistance = skillLowData.AbilityInfoList[i].dieMinDistance;
                            applyAbility.dieMaxDistance = skillLowData.AbilityInfoList[i].dieMaxDistance;

                            //Debug.Log(skillData[j]._SkillID + "," + (applyAbility.notiIdx-1));

                            //Debug.Log(levelInfo.factorRate[applyAbility.notiIdx - 1].ToString());
                            //추가되야 하는 데이터
                            if (levelInfo.factorRate.Count <= applyAbility.notiIdx - 1)
                            {
                                Debug.Log(string.Format("LevelInfo.factorRate의 범위를 넘어갔음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                continue;
                            }

                            applyAbility.factor = levelInfo.factor;

                            if (levelInfo.ignoreDef.Count <= applyAbility.notiIdx - 1)
                            {
                                Debug.Log(string.Format("LevelInfo.ignoreDef의 범위를 넘어갔음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                                continue;
                            }

                            if (levelInfo.factorRate[applyAbility.notiIdx - 1] != null)
                            {
                                float.TryParse(levelInfo.factorRate[applyAbility.notiIdx - 1].ToString(), out applyAbility.factorRate);
                                //applyAbility.factorRate = float.Parse(levelInfo.factorRate[applyAbility.notiIdx - 1]);
                            }
                            else
                            {
                                Debug.Log(string.Format("skillLevel데이터에 factorRate가 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                            }

                            if (levelInfo.ignoreDef[applyAbility.notiIdx - 1] != null)
                            {
                                float.TryParse(levelInfo.ignoreDef[applyAbility.notiIdx - 1].ToString(), out applyAbility.ignoreDef);
                                //applyAbility.ignoreDef = float.Parse(levelInfo.ignoreDef[applyAbility.notiIdx - 1]);
                            }
                            else
                            {
                                Debug.Log(string.Format("skillLevel데이터에 ignoreDef가 존재하지 않음 ( {0}-{1}-{2} )", skillData[j]._SkillID, skillData[j]._SkillLevel, applyAbility.notiIdx));
                            }

                            dic[skillLowData.AbilityInfoList[i].Idx].Add(applyAbility);
                        }
                    }
                }
            }
        }

        return true;
    }

    public SkillTables.ActionInfo GetSkillActionLowData(uint skillID)
    {
        SkillTables.ActionInfo actionLowData = null;
        if (!skillLowData.ActionInfoDic.TryGetValue(skillID, out actionLowData))
        {
            Debug.LogError("not found actionInfo LowData id=" + skillID);
        }

        return actionLowData;
    }

    public static SkillTables.ProjectTileInfo GetSkillProjectTileData(uint projectileID)
    {
        if (skillLowData.ProjectTileInfoDic.ContainsKey(projectileID))
        {
            return skillLowData.ProjectTileInfoDic[projectileID];
        }

        return null;
    }


    //EtcData 헬퍼
    public static float G_AttackDelay()
    {
        /*
        if( etcData.EtcInfoDic.ContainsKey((int)EtcID.MonsterAttackDelay) )
        {
            float outFloat;
            float.TryParse(etcData.EtcInfoDic[(int)EtcID.MonsterAttackDelay].Value, out outFloat);

            return outFloat;
        }

        return 0f;
        */
        float outFloat = _LowDataMgr.instance.GetEtcTableValue<float>(EtcID.MonsterAttackDelay);
        return outFloat;
    }

    public static float G_AutoSkillDelay()
    {
        /*
        if (etcData.EtcInfoDic.ContainsKey((int)EtcID.AutoSkillDelay))
        {
            float outFloat;
            float.TryParse(etcData.EtcInfoDic[(int)EtcID.AutoSkillDelay].Value, out outFloat);

            return outFloat;
        }

        return 0f;
        */

        float outFloat = _LowDataMgr.instance.GetEtcTableValue<float>(EtcID.AutoSkillDelay);
        return outFloat;
    }

    public static int G_HitResist()
    {
        /*
        if (etcData.EtcInfoDic.ContainsKey((int)EtcID.HitResist))
        {
            int outInt;
            int.TryParse(etcData.EtcInfoDic[(int)EtcID.HitResist].Value, out outInt);

            return outInt;
        }

        return 0;
        */
        int outInt = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.HitResist);
        return outInt;
    }

    // StageDataTable에서 정보를 빼온다.
    public string GetStringStageData(uint id)
    {
        if (stringLowData.StageDataInfoDic.ContainsKey(id))
            return stringLowData.StageDataInfoDic[id].String;

        return null;
    }

    public string GetStringRndNickName(uint charLowDataIdx)
    {
        bool isMan = true;
        if (charLowDataIdx == 13000)
            isMan = false;

        List<Local.RandomNameInfo> rndList = stringLowData.RandomNameInfoDic.Values.ToList();
        //int rndArr = Random.Range(0, rndList.Count);

        string c_1 = rndList[Random.Range(0, rndList.Count)].Name1;//성
        string c_2 = null;
        if (isMan)
            c_2 = rndList[Random.Range(0, rndList.Count)].Name2;//남자이름
        else
            c_2 = rndList[Random.Range(0, rndList.Count)].Name3;//여자이름

        return c_1 + c_2;
    }

    //스킬 이름 빼오기
    public string GetStringSkillName(uint id)
    {
        if (stringLowData.StringSkillInfoDic.ContainsKey(id))
            return stringLowData.StringSkillInfoDic[id].String;

        return null;
    }

    /// <summary> 장비재료 던전 데이터 </summary>
    public DungeonTable.EquipInfo GetLowDataEquipBattle(byte id)
    {
        DungeonTable.EquipInfo lowData = null;
        if (!dungeonData.EquipInfoDic.TryGetValue(id, out lowData))
            Debug.LogError("error! can not find 'DungeonTable.EquipInfo', now find id = " + id);

        return lowData;
    }

    /// <summary> 경험치 던전 데이터 </summary>
    public DungeonTable.SkillInfo GetLowDataSkillBattle(byte id)
    {
        DungeonTable.SkillInfo skillData = null;
        if (!dungeonData.SkillInfoDic.TryGetValue(id, out skillData))
            Debug.LogError("error! can not find 'DungeonTable.SkillInfo', now find id = " + id);

        return skillData;
    }

    /*
    /// <summary> 골드 던전 데이터 </summary>
    /// <param name="id">던전 아이디(DungeonTable/GoldBattleInfo)</param>
    public DungeonTable.GoldBattleInfo GetLowDataGoldBattle(byte id)
    {
        DungeonTable.GoldBattleInfo glowData = null;
        if (!dungeonData.GoldBattleInfoDic.TryGetValue(id, out glowData))
            Debug.LogError("error! can not find 'GoldBattleInfo', now find id = " + id);

        return glowData;
    }
    */
    /*
    /// <summary> 경험치 던전 데이터 </summary>
    public DungeonTable.ExpBattleInfo GetLowDataExpBattle(byte id)
    {
        DungeonTable.ExpBattleInfo expData = null;
        if (!dungeonData.ExpBattleInfoDic.TryGetValue(id, out expData))
            Debug.LogError("error! can not find 'ExpBattleInfo', now find id = " + id);

        return expData;
    }
    */
    /// <summary> 캐릭터의 레벨 데이터(플레이어, 파트너) </summary>
    /// <param name="level"> 원하는 정보의 레벨 </param>
    public Level.LevelInfo GetLowDataCharLevel(uint level)
    {
        Level.LevelInfo lLowData = null;
        if (!levelLowData.LevelInfoDic.TryGetValue((byte)level, out lLowData))
            Debug.LogError("can not find 'LevelInfo' search level = " + level);

        return lLowData;
    }

    /// <summary> 파트너 레벨 데이터 </summary>
    public PartnerLevel.PartnerLevelInfo GetLowDataPartnerLevel(uint level)
    {
        PartnerLevel.PartnerLevelInfo lLowData = null;
        if (!partnerLvLowData.PartnerLevelInfoDic.TryGetValue((byte)level, out lLowData))
            Debug.LogWarning("can not find 'PartnerLevel.PartnerLevelInfo' search level = " + level);

        return lLowData;
    }

    /// <summary> 메일 테이블 읽음 </summary>
    public Mail.MailInfo GetLowDataMail(uint id)
    {
        Mail.MailInfo mailInfo = null;
        if (!mailLowData.MailInfoDic.TryGetValue((ushort)id, out mailInfo))
            Debug.LogError("can not find 'MailTable.MailTableInfo' error " + id);

        return mailInfo;
    }

    /// <summary> 상점 테이블 읽음 (상점패널에서사용 LogIdx_ui로 찾기) </summary>
    public Shop.ShopInfo GetLowDataShopById(uint id)
    {
        Shop.ShopInfo shopInfo = null;

        var loopCount = shopLowData.ShopInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            uint idx = shopLowData.ShopInfoList[i].LogIdx;
            if (id != idx)
                continue;
            shopInfo = shopLowData.ShopInfoList[i];
        }

        if (shopInfo == null)
            Debug.LogError("can not find 'Shop.ShopInfo' error " + id);

        return shopInfo;

    }

    ///<summary> 상점테이블에서 화폐타입 반환</summary>
    public byte GetShopCostType(uint Type)
    {
        byte costType = 0;

        var loopCount = shopLowData.ShopInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            uint shopType = shopLowData.ShopInfoList[i].Type;
            if (shopType == Type)
            {
                costType = shopLowData.ShopInfoList[i].costType;
                continue;
            }
        }

        return costType;
    }

    /// <summary> 상점 테이블 읽음 (길드패널에서  Shop_ui로 찾기) </summary>
    public int GetLowDataShopByIndex(uint id)
    {
        int count = 0;

        var loopCount = shopLowData.ShopInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            uint idx = shopLowData.ShopInfoList[i].Type;
            if (id != idx)
                continue;
            count++;
        }

        if (count == 0)
            Debug.LogError("can not find 'Shop.ShopInfo' error " + id);

        return count;
    }


    /// <summary> vip레벨 테이블 읽음 </summary>
    public Vip.VipLevelInfo GetLowDataVipLevel(uint id)
    {
        Vip.VipLevelInfo vipInfo = null;
        if (!vipLowData.VipLevelInfoDic.TryGetValue((ushort)id, out vipInfo))
            Debug.LogError("can not find 'Vip.VipLevelInfo' error " + id);

        return vipInfo;
    }

    /// <summary> 길드 테이블 읽음 </summary>
    public Guild.GuildInfo GetLowdataGuildInfo(uint lv)
    {
        Guild.GuildInfo GuildLowData = null;
        if (!guildLowData.GuildInfoDic.TryGetValue((uint)lv, out GuildLowData))
            Debug.LogError("can not find 'Guild.GuildInfo' search level = " + lv);

        return GuildLowData;
    }
    /// <summary> 길드기부 테이블 읽음 </summary>
    public Guild.DonateInfo GetLowdataGuildDonateInfo(uint id)
    {
        Guild.DonateInfo GuildLowData = null;
        if (!guildLowData.DonateInfoDic.TryGetValue((uint)id, out GuildLowData))
            Debug.LogError("can not find 'Guild.DonateInfo' search level = " + id);

        return GuildLowData;
    }
    /// <summary> 길드직위 테이블 읽음 </summary>
    public Guild.GuildPositionInfo GetLowdataGuildPositionInfo(uint id)
    {
        Guild.GuildPositionInfo GuildLowData = null;
        if (!guildLowData.GuildPositionInfoDic.TryGetValue((uint)id, out GuildLowData))
            Debug.LogError("can not find 'Guild.GuildPositionInfo' search level = " + id);

        return GuildLowData;
    }

    /// <summary> 길드축원 테이블 읽음 </summary>
    public Guild.GuildprayLevelInfo GetLowdataGuildPrayInfo(uint lv)
    {
        Guild.GuildprayLevelInfo GuildLowData = null;
        if (!guildLowData.GuildprayLevelInfoDic.TryGetValue((uint)lv, out GuildLowData))
            Debug.LogError("can not find 'Guild.GuildprayLevelInfo' search level = " + lv);

        return GuildLowData;
    }
    /// <summary> 길드상점 테이블 읽음 </summary>
    public Guild.GuildShopLevelInfo GetLowDataGuildShopLevel(uint lv)
    {
        Guild.GuildShopLevelInfo GuildLowData = null;
        if (!guildLowData.GuildShopLevelInfoDic.TryGetValue((uint)lv, out GuildLowData))
            Debug.LogError("can not find 'Guild.GuildShopLevelInfo' search level = " + lv);

        return GuildLowData;
    }
    /// <summary> 길드퀘스트 테이블 읽음 </summary>
    public Guild.GuildQuestInfo GetLowDataGuildQuest(uint id)
    {
        Guild.GuildQuestInfo GuildLowData = null;
        if (!guildLowData.GuildQuestInfoDic.TryGetValue((uint)id, out GuildLowData))
            Debug.LogError("can not find 'Guild.GuildQuestInfo' search level = " + id);

        return GuildLowData;
    }
    /// <summary> 길드상점 레벨업시 증가하는 물품수 </summary>
    //public int GetGuildShopItmeCount(uint type)
    //{
    //    //int cnt = 0;

    //    //Shop.ShopInfo shopInfo = null;
    //    //Shop.ShopInfo lvUpShopInfo = null;
    //    //if (!shopLowData.ShopInfoDic.TryGetValue((ushort)type, out shopInfo))
    //    //    Debug.LogError("can not find 'ShopTable.ShopTableInfo' error " + type);

    //    //if (!shopLowData.ShopInfoDic.TryGetValue((ushort)type+1, out lvUpShopInfo))
    //    //    Debug.LogError("can not find 'ShopTable.ShopTableInfo' error " + type);



    //    //return cnt;
    //}

    /// <summary> 리워드테이블에서 아이템리스트를 줌 /// </summary>
    public List<GatchaReward.RewardInfo> GetRewardItemGroupList(uint groupId)
    {
        List<GatchaReward.RewardInfo> List = new List<GatchaReward.RewardInfo>();

        var loopCount = gatchaRewardLowData.RewardInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            GatchaReward.RewardInfo gLowData = gatchaRewardLowData.RewardInfoList[i];
            if (gLowData.GatchIdx != groupId)
                continue;

            List.Add(gLowData);
        }

        if (List.Count == 0)
            Debug.LogError("can not find 'GatchaReward.RewardInfo' error " + groupId);

        return List;
    }

    /// <summary> 가차테이블 타입으로 찾기 </summary>
//    public GatchaReward.RewardInfo GetRewardInfoForType(int type)
//    {
//        var loopCount = gatchaRewardLowData.RewardInfoList.Count;
//        for (int i = 0; i < loopCount; i++)
//        {
//            GatchaReward.RewardInfo gLowData = gatchaRewardLowData.RewardInfoList[i];
//            if (gLowData.Type != type)
//                continue;
//
//            return gLowData;
//        }
//
//        return null;
//    }

    /// <summary> 가차테이블 에서 Type과 아이템인덱스를 비교해서 반환해줌 (가챠에서 파트너아이콘이없어서 쓸예정) </summary>
    public bool IsGetRewardType(int type, uint itemIdx)
    {
        var loopCount = gatchaRewardLowData.RewardInfoList.Count;
        List<GatchaReward.RewardInfo> list = new List<GatchaReward.RewardInfo>();
        for (int i = 0; i < loopCount; i++)
        {
            GatchaReward.RewardInfo gLowData = gatchaRewardLowData.RewardInfoList[i];
            if (gLowData.Type == type)
                list.Add(gLowData);
        }

        for (int i = 0; i < list.Count; i++)
        {
            GatchaReward.RewardInfo gLowData = list[i];
            if (gLowData.ItemIdx == itemIdx)
            {
                return true;    //같으면...
            }
        }
        return false;
    }

    ///<summary> fixedReward 테이블에서 RewardId로 리스트줌 </summary>
    public List<GatchaReward.FixedRewardInfo> GetFixedRewardItemGroupList(uint rewardId)
    {
        List<GatchaReward.FixedRewardInfo> List = new List<GatchaReward.FixedRewardInfo>();

        var loopCount = gatchaRewardLowData.FixedRewardInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            GatchaReward.FixedRewardInfo gLowData = gatchaRewardLowData.FixedRewardInfoList[i];
            if (gLowData.RewardId != rewardId)
                continue;

            List.Add(gLowData);
        }

        if (List.Count == 0)
            Debug.LogError("can not find 'GatchaReward.FixedRewardInfo' error " + rewardId);

        return List;
    }

    ///<summary> fixedReward 테이블에서 RewardId로 리스트줌 </summary>
    public GatchaReward.FixedRewardInfo GetFixedRewardItem(uint rewardId)
    {
        var loopCount = gatchaRewardLowData.FixedRewardInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            GatchaReward.FixedRewardInfo gLowData = gatchaRewardLowData.FixedRewardInfoList[i];
            if (gLowData.RewardId != rewardId)
                continue;

            return gLowData;
        }

        Debug.LogError("can not found 'GatchaReward.FixedRewardInfo' error " + rewardId);
        return null;
    }

    ///<summary> 가챠 확률아이템리스트 얻어옴 </summary>
    public List<GatchaReward.RewardInfo> GetGachaRewardItemLsit(uint type, int isNormal , bool isAll = false)
    {
        int GroupId = isNormal == 0 ? 180320 : 190320;  //가챠그룹아이디
        int GroupId_ = isNormal == 0 ? 181320 : 191320;

        List<GatchaReward.RewardInfo> List = new List<GatchaReward.RewardInfo>();

        var loopCount = gatchaRewardLowData.RewardInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            GatchaReward.RewardInfo gLowData = gatchaRewardLowData.RewardInfoList[i];
            if (gLowData.GatchIdx != GroupId && gLowData.GatchIdx != GroupId_)
                continue;

            if (!isAll && gLowData.Type != type)
                continue;

            List.Add(gLowData);
        }

        if (List.Count == 0)
            Debug.Log("can not find 'GatchaReward.RewardInfo' error " + type);

        return List;
    }

    /// <summary> vip스테이지 테이블 읽음 </summary>
    public List<Vip.VipStageInfo> GetLowDataVipStage(uint lv)
    {
        List<Vip.VipStageInfo> vipInfo = new List<Vip.VipStageInfo>();
        var loopCount = vipLowData.VipStageInfoList.Count;

        for (int i = 0; i < loopCount; i++)
        {
            Vip.VipStageInfo vLowData = vipLowData.VipStageInfoList[i];
            if (vLowData.VipGrade != lv)
                continue;

            vipInfo.Add(vLowData);
        }

        if (vipInfo.Count == 0)
            Debug.LogError("can not find 'VipTable.VipStageInfo' error " + lv);

        return vipInfo;
    }

    /// <summary> vip스테이지 테이블 에서 증가수치를 준다 </summary>
    public float GetVipStageValue(uint lv)
    {
        //Vip.VipStageInfo vipInfo = new Vip.VipStageInfo();
        var loopCount = vipLowData.VipStageInfoList.Count;

        float percent = 0;

        for (int i = 0; i < loopCount; i++)
        {
            Vip.VipStageInfo vLowData = vipLowData.VipStageInfoList[i];
            if (vLowData.VipGrade != lv)
                continue;

            percent = vLowData.addvalue;
        }

        return percent;
    }

    /// <summary> vip데이터 테이블 읽음 </summary>
    public List<Vip.VipDataInfo> GetLowDataVipData(uint id)
    {
        List<Vip.VipDataInfo> vipInfo = new List<Vip.VipDataInfo>();
        var loopCount = vipLowData.VipDataInfoList.Count;

        for (int i = 0; i < loopCount; i++)
        {
            Vip.VipDataInfo vLowData = vipLowData.VipDataInfoList[i];
            if (vLowData.VipGrade != id)
                continue;

            vipInfo.Add(vLowData);
        }

        if (vipInfo.Count == 0)
            Debug.LogError("can not find 'VipTable.VipDataInfo' error " + id);

        return vipInfo;

    }

    /// <summary> 가격 테이블 읽음 </summary>
    public List<Price.PriceInfo> GetLowDataPrice(uint Type)
    {
        List<Price.PriceInfo> priceInfo = new List<Price.PriceInfo>();
        var loopCount = priceLowData.PriceInfoList.Count;

        for (int i = 0; i < loopCount; i++)
        {
            Price.PriceInfo pLowData = priceLowData.PriceInfoList[i];
            if (pLowData.Type != Type)
                continue;

            priceInfo.Add(pLowData);
        }

        if (priceInfo.Count == 0)
            Debug.LogError("can not find 'PriceTable.PriceTableInfo' error " + Type);

        return priceInfo;
    }

    /// <summary> 가격 테이블 아이디에 맞게 하나 줌 </summary>
    public Price.PriceInfo GetLowDataPriceInfo(uint lowDataID)
    {
        int loopCount = priceLowData.PriceInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            Price.PriceInfo pLowData = priceLowData.PriceInfoList[i];
            if (pLowData.LogIdx != lowDataID)
                continue;

            return pLowData;
        }

        return null;
    }


    /// <summary> 아이템 벨류 테이블 줌 </summary>
    public Item.ItemValueInfo GetLowDataItemValueInfo(uint id)
    {
        Item.ItemValueInfo itemValueInfo = null;
        if (!itemLowData.ItemValueInfoDic.TryGetValue(id, out itemValueInfo))
            Debug.LogError("can not find 'ItemTableLowData.ItemValueInfo' error " + id);

        return itemValueInfo;
    }

    /// <summary> LevelTable의 마지막 레벨을 반환한다. </summary>
    public uint GetLevelTableMaxLevel()
    {
        byte maxLevel = 0;
        var enumerator = levelLowData.LevelInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            maxLevel = enumerator.Current.Key;
        }

        return maxLevel;
    }

    /// <summary> 보스 레이드 리스트 줌. (셋팅하는 용도)</summary>
    public List<DungeonTable.SingleBossRaidInfo> GetLowDataBossRaidList()
    {
        return dungeonData.SingleBossRaidInfoDic.Values.ToList();
    }

    //보스레이드 입장레벨...
    public DungeonTable.SingleBossRaidInfo GetSingleBossRaidLimitLevel(uint id)
    {
        DungeonTable.SingleBossRaidInfo singleValue = null;
        if (!dungeonData.SingleBossRaidInfoDic.TryGetValue(id, out singleValue))
            Debug.LogError("can not find 'ItemTableLowData.ItemValueInfo' error " + id);

        return singleValue;
    }

    /// <summary> 아이디에 맞는 보스 레이드 데이터</summary>
    public void RefLowDataBossRaid(uint id, ref DungeonTable.SingleBossRaidInfo bossRaid)
    {
        if (!dungeonData.SingleBossRaidInfoDic.TryGetValue(id, out bossRaid))
        {
            Debug.LogError("not found 'BossRaidLowData' error " + id);
        }
    }

    /// <summary>
    /// 던전 컨텐츠 오픈조건
    /// </summary>
    /// <param name="type"> 컨텐츠타입 </param>
    /// <returns></returns>
    public List<DungeonTable.ContentsOpenInfo> GetDungeonContentsOpenInfoList(byte type)
    {
        List<DungeonTable.ContentsOpenInfo> Info = new List<DungeonTable.ContentsOpenInfo>();

        for (int i = 0; i < dungeonData.ContentsOpenInfoList.Count; i++)
        {
            DungeonTable.ContentsOpenInfo LowData = dungeonData.ContentsOpenInfoList[i];
            if (LowData.ContentsType != type)
                continue;

            Info.Add(LowData);
        }

        return Info;
    }

    /// <summary> 테이블상 첫번째에 위치한 테이블 넘겨줌 </summary>
    public DungeonTable.ContentsOpenInfo GetFirstContentsOpenInfo(byte type)
    {
        for (int i = 0; i < dungeonData.ContentsOpenInfoList.Count; i++)
        {
            DungeonTable.ContentsOpenInfo LowData = dungeonData.ContentsOpenInfoList[i];
            if (LowData.ContentsType != type)
                continue;

            return LowData;
        }

        return null;
    }

    /// <summary> 오픈 조건 리스트 줌 </summary>
    public List<DungeonTable.ContentsOpenInfo> GetContentsOpenList()
    {
        return dungeonData.ContentsOpenInfoList;
    }

    /// <summary> 아이템 합성 정보를 그룹별, 직업에 맞게 정리해서 줌.</summary>
    public Dictionary<ushort, List<Item.fusionInfo>> GetLowDataItemFusion(byte classType)
    {
        Dictionary<ushort, List<Item.fusionInfo>> fusionDic = new Dictionary<ushort, List<Item.fusionInfo>>();
        if (itemLowData.fusionInfoDic == null)
            return fusionDic;

        var enumerator = itemLowData.fusionInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Item.fusionInfo lowData = enumerator.Current.Value;
            if (lowData.Id < SystemDefine.MinUseItemId)
            {
                Item.EquipmentInfo equipLowData = GetLowDataEquipItemInfo(lowData.Id);
                if (equipLowData == null || equipLowData.Class != classType)
                    continue;
            }

            List<Item.fusionInfo> list = null;
            if (fusionDic.TryGetValue(lowData.group, out list))
                list.Add(lowData);
            else
            {
                list = new List<Item.fusionInfo>();
                list.Add(lowData);
                fusionDic.Add(lowData.group, list);
            }
        }

        return fusionDic;
    }

    /// <summary> 마계의탑 데이터를 리스트로 준다</summary>
    public List<DungeonTable.TowerInfo> GetLowDataTowerList()
    {
        return dungeonData.TowerInfoList;
    }

    /// <summary> 마계의탑 데이터를 아이디에 맞게 준다 </summary>
    public DungeonTable.TowerInfo GetLowDataTower(uint lowDataID)
    {
        int loopCount = dungeonData.TowerInfoList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            DungeonTable.TowerInfo tLowData = dungeonData.TowerInfoList[i];
            if (tLowData.StageIndex != lowDataID)
                continue;

            return tLowData;
        }

        return null;
    }

    /// <summary> ETCTable에 있는 것들을 키값을 입력해서 찾아온다. ex) GetEtcTableValue<int>("intKey") </int></summary>
    public T GetEtcTableValue<T>(EtcID etcId)
    {
        object obj = 0;
        Etc.EtcInfo table = null;
        if (etcData.EtcInfoDic.TryGetValue((uint)etcId, out table))
        {
            if (typeof(T) == typeof(byte))
                obj = byte.Parse(table.Value);
            else if (typeof(T) == typeof(short))
                obj = short.Parse(table.Value);
            else if (typeof(T) == typeof(ushort))
                obj = ushort.Parse(table.Value);
            else if (typeof(T) == typeof(float))
                obj = float.Parse(table.Value);
            else if (typeof(T) == typeof(int))
                obj = int.Parse(table.Value);
            else if (typeof(T) == typeof(uint))
                obj = uint.Parse(table.Value);
            else if (typeof(T) == typeof(long))
                obj = long.Parse(table.Value);
            else if (typeof(T) == typeof(ulong))
                obj = ulong.Parse(table.Value);
            else if (typeof(T) == typeof(double))
                obj = double.Parse(table.Value);
            else if (typeof(T) == typeof(string))
                obj = table.Value;
        }

        return (T)obj;
    }


    /// <summary> 파트너 버프그룹 준다. </summary>
//    public SkillTables.BuffGroupInfo GetLowDataBuffGroup(byte partnerType, uint skillId)
//    {
//        int loopCount = skillLowData.BuffGroupInfoList.Count;
//        for (int i = 0; i < loopCount; i++)
//        {
//            SkillTables.BuffGroupInfo buffLowData = skillLowData.BuffGroupInfoList[i];
//            if (buffLowData.partnerType != partnerType)
//                continue;
//            else if (buffLowData.Indx != skillId)
//                continue;
//
//            return buffLowData;
//        }
//
//        return null;
//    }
    

    //일퀘와 업적리스트를 채워준다
    public void InitializeMission()
    {
        //NetData의 Mission관련 데이터 초기화
        NetData.instance._userInfo._MissionList.Clear();

        for (int i = 0; i < missionLowData.MissionInfoList.Count; i++)
        {
            NetData.Mission Mission = new NetData.Mission();
            Mission.InitMission(missionLowData.MissionInfoList[i]);

            NetData.instance._userInfo._MissionList.Add(Mission);
        }
    }

    /// <summary> 퀘스트 메인 데이터 </summary>
    public Quest.QuestInfo GetLowDataQuestData(uint id)
    {
        Quest.QuestInfo lowData = null;
        if (!questLowData.QuestInfoDic.TryGetValue(id, out lowData))
            Debug.LogError(string.Format("not found questLowData {0}", id));

        return lowData;
    }

    /// <summary> EnchantTable을 줌 </summary>
    public Enchant.EnchantInfo GetLowDataEnchant(uint id, int level)
    {
        int count = enchantLowData.EnchantInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Enchant.EnchantInfo enchantInfo = enchantLowData.EnchantInfoList[i];
            if (enchantInfo.Id != id)
                continue;

            if (enchantInfo.enchantCount != level)
                continue;

            return enchantInfo;
        }

        return null;
    }
    /*
    /// <summary> 인첸트 curLevel 부터 maxLevel까지 필요 금액 반환 </summary>
    public uint GetLowDataEnchantTotalCost(uint id, int curLevel, int maxLevel)
    {
        uint totalCost = 0;
        //int findLevel = curLevel;
        int count = enchantLowData.EnchantInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Enchant.EnchantInfo enchantInfo = enchantLowData.EnchantInfoList[i];
            if (enchantInfo.Id != id)
                continue;

            if (enchantInfo.enchantCount < curLevel)
                continue;

            ++curLevel;
            totalCost += enchantInfo.CostGold;

            if (maxLevel <= curLevel)//입력한 maxLevel보다 curLevel이 넘어가면 끝낸다.
                break;
        }

        return totalCost;
    }
    */
    /// <summary> 원하는 승급 데이터를 준다. </summary>
    public Enchant.EvolveInfo GetLowDataEvolve(uint id)
    {
        int maxCount = enchantLowData.EvolveInfoList.Count;
        for (int i = 0; i < maxCount; i++)
        {
            Enchant.EvolveInfo evolveInfo = enchantLowData.EvolveInfoList[i];
            if (evolveInfo.evolveId != id)
                continue;

            //if (!isPartner && evolveInfo.evolveCount != grade)//파트너는 이런거 검사안함
            //    continue;

            return evolveInfo;
        }

        return null;
    }

    /// <summary> 원하는 분해 데이터를 준다. </summary>
    public Enchant.BreakInfo GetLowDataBreak(uint id)
    {
        Enchant.BreakInfo breakInfo = null;
        if (!enchantLowData.BreakInfoDic.TryGetValue(id, out breakInfo))
            Debug.LogError(string.Format("not found error BreakTableInfo {0}", id));

        return breakInfo;
    }

    public Map.MapDataInfo GetMapData(uint MapID)
    {
        Map.MapDataInfo mapData = null;
        if( mapLowData.MapDataInfoDic.TryGetValue(MapID, out mapData) )
        {
            return mapData;
        }

        return null;
    }

    public Map.MapDataInfo GetMapData(string MapName)
    {
        var enumerator = mapLowData.MapDataInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.scene.Equals(MapName))
            {
                return enumerator.Current.Value;
            }
        }

        return null;
    }

    public string GetBGMFile(string MapName)
    {
        var enumerator = mapLowData.MapDataInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.scene.Equals(MapName))
            {
                return enumerator.Current.Value.Bgm;
            }
        }

        return "";
    }

    public void GetMapShadowData(string MapName, out float CharLightValue, out float ShadowStrength)
    {
        var enumerator = mapLowData.MapDataInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.scene.Equals(MapName))
            {
                CharLightValue = enumerator.Current.Value.CharLightIntensity;
                ShadowStrength = enumerator.Current.Value.ShadowStrength;
                return;
            }
        }

        //디폴트 값
        CharLightValue = 0.93f;
        ShadowStrength = 0.1f;
    }

    /// <summary> iconIdx에 맞는 아이콘 이름을 준다. (IconTable) </summary>
    public string GetLowDataIcon(uint iconIdx)
    {
        Icon.IconInfo info = null;
        if (iconLowData.IconInfoDic.TryGetValue(iconIdx, out info))
        {
            return info.Icon;
        }

        Debug.LogError(string.Format("not found IconIndex error {0}", iconIdx));
        return null;
    }

    


	
	/// <summary> 챕터 별보상 테이블 </summary>
    public List<DungeonTable.ChapterRewardInfo> GetLowDataChapterReward(byte chapter, byte chatType)
    {
        List<DungeonTable.ChapterRewardInfo> rewardList = new List<DungeonTable.ChapterRewardInfo>();

        int count = dungeonData.ChapterRewardInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            DungeonTable.ChapterRewardInfo info = dungeonData.ChapterRewardInfoList[i];
            if (info.ChapId != chapter || chatType != info.ChapType)//chatType == 1 노말, 2 하드
                continue;

            rewardList.Add(info);
        }

        return rewardList;
    }

    /// <summary> 파트너 타입으로 받기 </summary>
    public List<uint> GetLowDataPartnerIdForType(byte type)
    {
        List<uint> parList = new List<uint>();
        List<Partner.PartnerDataInfo> dataList = GetPartnerDataToList();
        bool isSearchEnd = false;

        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].Type != type)
            {
                if (isSearchEnd)
                    break;

                continue;
            }

            isSearchEnd = true;//테이블에는 순차적으로 나열되어있기때문에 이런식으로 사용함
            parList.Add(dataList[i].Id);
        }

        return parList;
    }

	public float GetLowDataBattlePoint(AbilityType _abilityType, float _value)
	{
		BattlePointType btType = BattlePointType.None;

		switch (_abilityType) {
			case AbilityType.HP: 						btType = BattlePointType.HP; break;
			case AbilityType.DAMAGE:					btType = BattlePointType.DAMAGE; break;
			case AbilityType.DEFENCE_IGNORE:			btType = BattlePointType.DEFENCE_IGNORE; break;
			case AbilityType.DRAIN_HP:					btType = BattlePointType.DRAIN_HP; break;
			case AbilityType.DAMAGE_DECREASE:			btType = BattlePointType.DAMAGE_DECREASE; break;
			case AbilityType.DAMAGE_DECREASE_RATE:		btType = BattlePointType.DAMAGE_DECREASE_RATE; break;
			case AbilityType.HIT_RATE:					btType = BattlePointType.HIT_RATE; break;
			case AbilityType.DODGE_RATE:				btType = BattlePointType.DODGE_RATE; break;
			case AbilityType.CRITICAL_RATE:				btType = BattlePointType.CRITICAL_RATE; break;
			case AbilityType.CRITICAL_RES:				btType = BattlePointType.CRITICAL_RES; break;
			case AbilityType.CRITICAL_DAMAGE:			btType = BattlePointType.CRITICAL_DAMAGE; break;
			case AbilityType.COOLTIME:					btType = BattlePointType.COOLTIME; break;
			case AbilityType.ATTACK_SPEED:				//btType = BattlePointType.ATTACK_SPEED; break;
			case AbilityType.EXP_UP:			
			case AbilityType.ALLSTAT_RATE:		
			case AbilityType.ATTACK_RANGE:		
			case AbilityType.ATTACK_ANGLE:		
			case AbilityType.MOVE_SPEED:		
			case AbilityType.DETECTED_RANGE:	
			case AbilityType.SUPERARMOR:		
			case AbilityType.SUPERARMOR_RECOVERY_TIME:
			case AbilityType.SUPERARMOR_RECOVERY_RATE:
			case AbilityType.SUPERARMOR_RECOVERY:		btType = BattlePointType.None; break;
		}


		Formula.battlepointInfo battleInfo = null;
		if (FormulaLowData.battlepointInfoDic.TryGetValue((uint)btType, out battleInfo))
		{
			//Debug.Log(string.Format("{0} * {1} = {2}", value, battleInfo.Value, value * battleInfo.Value) );
			return _value * battleInfo.Value;
		}
		
		return 0;
	}

    public float GetLowDataBattlePoint(BattlePointType type, float value)
    {
        Formula.battlepointInfo battleInfo = null;
        if (FormulaLowData.battlepointInfoDic.TryGetValue((uint)type, out battleInfo))
        {
            //Debug.Log(string.Format("{0} * {1} = {2}", value, battleInfo.Value, value * battleInfo.Value) );
            return value * battleInfo.Value;
        }

        return 0;
    }

    /// <summary> 로딩 테이블  </summary>
    public List<Loading.LoadingInfo> GetLowDataLoading(byte contentType)
    {
        List<Loading.LoadingInfo> list = new List<Loading.LoadingInfo>();

        if (contentType == 0)
            contentType = 99;

        int count = loadingLowData.LoadingInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Loading.LoadingInfo info = loadingLowData.LoadingInfoList[i];
            if (info.Type != contentType)
                continue;

            list.Add(info);
        }

        return list;
    }

    public List<Quest.QuestInfo> GetLowDataTutorialQuestInfo(byte questType)
    {
        List<Quest.QuestInfo> questList = null;


        return questList;
    }

    /// <summary> 칭호 리스트 </summary>
    public List<Title.TitleInfo> GetLowDataTitleList()
    {
        return titleLowData.TitleInfoDic.Values.ToList();
    }

    /// <summary> 칭호 이름 </summary>
    public Title.TitleInfo GetLowDataTitle(uint id)
    {
        Title.TitleInfo info = null;
        if (titleLowData.TitleInfoDic.TryGetValue(id, out info))
        {
            return info;
        }

        return null;
    }

    #region 임시 인트로 튜토리얼 데이터
    /// <summary> 튜토리얼 퀘스트 리스트 </summary>
    public Quest.TutorialInfo GetLowDataFirstTutorial(uint type, byte sort)
    {
        List<Quest.TutorialInfo> list = questLowData.TutorialInfoDic.Values.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Group != type || list[i].Sort != sort)
                continue;

            return list[i];
        }

        return null;
    }

    /// <summary> 튜토리얼 퀘스트 찾음 </summary>
    public Quest.TutorialInfo GetLowDataTutorial(uint id)
    {
        Quest.TutorialInfo info = null;
        if (!questLowData.TutorialInfoDic.TryGetValue(id, out info))
        {
            Debug.LogError("not found Tutorial Quest error id= " + id);
        }

        return info;
    }

    /// <summary> 튜토리얼에서 쓰는 서브 대화  </summary>
    public List<Quest.SubTalkSceneInfo> GetLowDataSubTalkInfo(uint id)
    {
        List<Quest.SubTalkSceneInfo> infoList = new List<Quest.SubTalkSceneInfo>();
        int count = questLowData.SubTalkSceneInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Quest.SubTalkSceneInfo info = questLowData.SubTalkSceneInfoList[i];
            if (info.SceneID != id)
                continue;

            infoList.Insert((int)info.Sequence - 1, info);
        }

        return infoList;
    }
    #endregion

    /// <summary> 튜토리얼 퀘스트 리스트 </summary>
    public Quest.MainTutorialInfo GetLowDataFirstMainTutorial(uint type, byte sort)
    {
        List<Quest.MainTutorialInfo> list = questLowData.MainTutorialInfoDic.Values.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Group != type || list[i].Sequence != sort)
                continue;

            return list[i];
        }

        return null;
    }

    /// <summary> 튜토리얼 퀘스트 찾음 </summary>
    public Quest.MainTutorialInfo GetLowDataMainTutorial(uint id)
    {
        Quest.MainTutorialInfo info = null;
        if (!questLowData.MainTutorialInfoDic.TryGetValue(id, out info))
        {
            Debug.LogError("not found Tutorial Quest error id= " + id);
        }

        return info;
    }

    /// <summary> 튜토리얼 퀘스트 sequnce에 맞는 녀석들 다 찾아서줌. 단 0일경우에는 리스트 통 </summary>
    public List<Quest.MainTutorialInfo> GetLowDataMainTutorialList(int sequence)
    {
        List<Quest.MainTutorialInfo> list = questLowData.MainTutorialInfoDic.Values.ToList();
        if (sequence == 0)
            return list;

        List<Quest.MainTutorialInfo> newList = new List<Quest.MainTutorialInfo>();
        for (int i=0; i < list.Count; i++)
        {
            if (list[i].Sequence != sequence)
                continue;

            newList.Add(list[i]);
        }

        return newList;
    }
    
    /// <summary> 칭호 네임 </summary>
    public string GetLowDataTitleName(uint id)
    {
        Local.StringTitleInfo info = null;
        if (stringLowData.StringTitleInfoDic.TryGetValue(id, out info))
        {
            return info.String;
        }

        return null;
    }
    /// <summary> 일일출석 보상id </summary>
    public uint GetLowDataDailyRewardId(uint id)
    {
        Welfare.DailyCheckInfo info = null;
        if (welfareLowData.DailyCheckInfoDic.TryGetValue(id, out info))
        {
            return info.RewardId;
        }

        return 0;
    }

    //Walfare테이블에서 혜택보상타입에 따른 List찾음
    public List<Welfare.WelfareInfo> GetLowDataWalfare(uint type)
    {
        List<Welfare.WelfareInfo> WelfareList = new List<Welfare.WelfareInfo>();
        for(int i=0;i< welfareLowData.WelfareInfoList.Count;i++)
        {
            Welfare.WelfareInfo info = welfareLowData.WelfareInfoList[i];
            if (info.Type != type)
                continue;

            WelfareList.Add(info);
        }
        return WelfareList;
    }

    //Welfare.EventCheck테이블을 가져온다
    public List<Welfare.EventCheckInfo> GetLowDataEventCheck(uint type)
    {
        List<Welfare.EventCheckInfo> WelfareList = new List<Welfare.EventCheckInfo>();
        for (int i = 0; i < welfareLowData.EventCheckInfoList.Count; i++)
        {
            Welfare.EventCheckInfo info = welfareLowData.EventCheckInfoList[i];
            if (info.Type != type)
                continue;

            WelfareList.Add(info);
        }

        return WelfareList;
    }
    //Welfare.ServerEvent테이블 가져오
    public Welfare.ServerEventInfo GetLowDataServerEvent(uint type, byte surverType)
    {
        Welfare.ServerEventInfo Info = new Welfare.ServerEventInfo();
        for(int i=0;i<welfareLowData.ServerEventInfoList.Count;i++)
        {
            Welfare.ServerEventInfo info = welfareLowData.ServerEventInfoList[i];
            if (info.Type != type && info.ServerType != surverType)
                continue;
            Info = info;
        }
        return Info;
    }

    /// <summary> 업적포인트 클리어값  </summary>
    public uint GetClearValue(uint id)
    {
        int count = achievementLowData.AchievementCategoryInfoList.Count;
        uint value = 0;
        for (int i = 0; i < count; i++)
        {
            Achievement.AchievementCategoryInfo info = achievementLowData.AchievementCategoryInfoList[i];
            if (info.Type != id)
                continue;

            value = info.Clearvalue;
        }

        return value;
    }


    /// <summary> 업적데이터  </summary>
    public List<Achievement.AchievementInfo> GetLowDataAchievementInfo(uint type)
    {
        List<Achievement.AchievementInfo> infoList = new List<Achievement.AchievementInfo>();
        int count = achievementLowData.AchievementInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.AchievementInfo info = achievementLowData.AchievementInfoList[i];
            if (info.Type != type)
                continue;

            infoList.Add(info);
        }

        return infoList;
    }
    /// <summary> 일일업적데이터리스트  </summary>
    public List<Achievement.DailyInfo> GetLowDataDaiylAchievementInfoList(uint type)
    {
        List<Achievement.DailyInfo> infoList = new List<Achievement.DailyInfo>();
        int count = achievementLowData.DailyInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.DailyInfo info = achievementLowData.DailyInfoList[i];
            if (info.Type != type)
                continue;

            infoList.Add(info);
        }

        return infoList;
    }
    /// <summary> 일일업적포인트 데이터리스트  </summary>
    public Achievement.DailyRewardInfo GetLowDataDaiylAchievementPointInof(uint type)
    {
        int count = achievementLowData.DailyRewardInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.DailyRewardInfo info = achievementLowData.DailyRewardInfoList[i];
            if (info.RewardRank == type)
            {
                return info;
            }
              
        }
        return null;
    }
    /// <summary> 일일업적데이터  </summary>
    public Achievement.DailyInfo GetLowDataDaiylAchievementInfo(uint type , uint level)
    {
        int count = achievementLowData.DailyInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.DailyInfo info = achievementLowData.DailyInfoList[i];
            if(info.Type == type && info.Phase == level)
            {
                return info;
            }

        }

        return null;
    }


    /// <summary> 업적데이터  </summary>
    public List<Achievement.AchievementInfo> GetLowDataAchievementInfoSubType(uint type, uint subType)
    {
        List<Achievement.AchievementInfo> infoList = new List<Achievement.AchievementInfo>();
        int count = achievementLowData.AchievementInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.AchievementInfo info = achievementLowData.AchievementInfoList[i];
            if (info.Type != type)
                continue;
            if (info.Subtype != subType)
                continue;

            infoList.Add(info);
        }

        return infoList;
    }


    /// <summary> 타입/서브타입/레벨에맞는 업적정보  </summary>
    public Achievement.AchievementInfo GetAchieveInfo(uint type, uint subTye, uint lv)
    {
       //Achievement.AchievementInfo AchieveInfo = new Achievement.AchievementInfo();
        int count = achievementLowData.AchievementInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.AchievementInfo info = achievementLowData.AchievementInfoList[i];
            if (info.Type != type)
                continue;
            if (info.Subtype != subTye)
                continue;
            if (info.Phase != lv)
                continue;

            return info;//AchieveInfo = 
        }

        Debug.LogError(string.Format("not found 'AchievementInfo' error type={0}, subType={1}, Lv={2}", type, subTye, lv) );
        return null;
        //return AchieveInfo;
    }


    /// <summary> 업적데이터  </summary>
    public Achievement.AchievementCategoryInfo GetLowDataAchievementInfoCategory(uint type)
    {
        Achievement.AchievementCategoryInfo Info = new Achievement.AchievementCategoryInfo();
        int count = achievementLowData.AchievementCategoryInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.AchievementCategoryInfo info = achievementLowData.AchievementCategoryInfoList[i];
            if (info.Type != type)
                continue;

            Info = info;
        }
        return Info;
    }

    /// <summary> 아이디에 맞는 업적정보  </summary>
    public Achievement.AchievementInfo GetLowDataAchievInfo(uint dataId)
    {
        int count = achievementLowData.AchievementInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Achievement.AchievementInfo info = achievementLowData.AchievementInfoList[i];
            if (info.Id != dataId)
                continue;

            return info;
        }

        return null;
    }

    /// <summary> 몬스터의 패턴리스트를 찾는다 </summary>
    public List<Mob.PattenInfo> GetLowDataPattenList(uint groupId)
    {
        List<Mob.PattenInfo> list = new List<Mob.PattenInfo>();

        int count = monsterLowData.PattenInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Mob.PattenInfo info = monsterLowData.PattenInfoList[i];
            if (info.id != groupId)
            {
                if (0 < list.Count)//아이디가 다른데 리스트에 이미 뭔가가 채워져 있으면 끝난거임
                    continue;
            }
            list.Add(info);
        }

        if (list.Count == 0)
            Debug.LogError("not found pattenInfo error group id = " + groupId);

        return list;
    }

    /// <summary> 콜로세움 정보를 찾는다 </summary>
    public DungeonTable.ColosseumInfo GetLowDataColosseumInfo(uint id)
    {
        DungeonTable.ColosseumInfo col = null;
        if( !dungeonData.ColosseumInfoDic.TryGetValue(id, out col))
        {
            Debug.LogError("not found ColosseumInfo error " + id);
        }

        return col;
    }

    /// <summary> 콜로세움 리스트 </summary>
    public List<DungeonTable.ColosseumInfo> GetLowDataColosseumList()
    {
        return dungeonData.ColosseumInfoDic.Values.ToList();
    }

    /// <summary> 멀티보스 레이드 데이터 </summary>
    public DungeonTable.MultyBossRaidInfo GetLowDataMultyBossInfo(uint id)
    {
        DungeonTable.MultyBossRaidInfo raidInfo = null;
        if (!dungeonData.MultyBossRaidInfoDic.TryGetValue(id, out raidInfo))
            Debug.LogError("not found MultyBossRaidInfo error " + id);

        return raidInfo;
    }

    /// <summary> 멀티보스 레이드 데이터 </summary>
    public List<DungeonTable.MultyBossRaidInfo> GetLowDataMultyBossInfoList(byte groupId)
    {
        List<DungeonTable.MultyBossRaidInfo> groupList = new List<DungeonTable.MultyBossRaidInfo>();
        List<DungeonTable.MultyBossRaidInfo> list = dungeonData.MultyBossRaidInfoDic.Values.ToList();
        for(int i=0; i < list.Count; i++)
        {
            if (list[i].Type != groupId)
                continue;

            groupList.Add(list[i] );
        }

        if(groupList.Count == 0)
            Debug.LogError("not found Group List MultyBossRaidInfo error " + groupId);

        return groupList;
    }

    /// <summary> 활동량 리스트 </summary>
    public List<ActiveReward.ActivePointInfo> GetLowDataActivePointList()
    {
        return activiteLowData.ActivePointInfoDic.Values.ToList();
    }

    /// <summary> 활동량 </summary>
    public ActiveReward.ActivePointInfo GetLowDataActivePoint(uint id)
    {
        ActiveReward.ActivePointInfo info = null;

        if ( !activiteLowData.ActivePointInfoDic.TryGetValue(id, out info))
        {
            Debug.LogError("not found ActivePointInfo error " + id);
        }

        return info;
    }

    /// <summary> 활동량 보상 목록 </summary>
    public List<ActiveReward.ActiveRewardInfo> GetLowDataActiveRewardList()
    {
        return activiteLowData.ActiveRewardInfoList;
    }
    
    /// <summary> 에러팝업 스트링 준다 </summary>
    public string GetLowDataErrorString(uint lowId)
    {
        Local.ErrorPopupInfo info = null;
        if (stringLowData.ErrorPopupInfoDic.TryGetValue(lowId, out info))
            return info.Description;

        Debug.LogError("not found ErrorPopupInfo error " + lowId);
        return null;
    }


	public int getNumOfSkillAction(){
		return skillLowData.ActionInfoDic.Count;
	}
	
	//	public static SkillTables.ActionInfo GetSkillActionByIndex(int index)
	//	{
	//		//return skillLowData.ActionInfoDic.
	//
	//		List<uint> keyList = new List<uint>(skillLowData.ActionInfoDic);
	//	}
	
	public List<uint> GetSkillActionInfoKeyList()
	{
		//return skillLowData.ActionInfoDic.
		
		List<uint> keyList = new List<uint>(skillLowData.ActionInfoDic.Keys);
		
		return keyList;
		
		//		for(int i=0;i<keyList.Count;i++){
		//			uint id = keyList[i];
		//			Debug.Log("actionInfo id:"+id);
		//		}
	}
	
	/// <summary>
	/// </summary>
	/// <returns>The skill ability info.</returns>
	/// <param name="skillId">Skill identifier.</param>
	public SkillTables.AbilityInfo getSkillAbilityInfo(uint skillId){
		for (int i=0; i<skillLowData.AbilityInfoList.Count; i++) {
			if (skillLowData.AbilityInfoList[i].Idx == skillId){
				return skillLowData.AbilityInfoList[i];
			}
		}
		
		return null;
	}

    /// <summary> 차관 자동 보상 목록 표기 </summary>
    public List<PVP.PVPAutoRewardInfo> GetPvpAutoRewardList()
    {
        return pvpAutoReward.PVPAutoRewardInfoList;
    }

    /// <summary> 아이템 획득처에 관한 리스트 준다. </summary>
    public List<Item.ContentsListInfo> GetLowDataContentsItemList(uint itemIdx)
    {
        List<Item.ContentsListInfo> list = new List<Item.ContentsListInfo>();
        for (int i = 0; i < itemLowData.ContentsListInfoList.Count; i++)
        {
            if (!itemLowData.ContentsListInfoList[i].ItemIdx.Equals(itemIdx))
            {
                if (0 < list.Count)
                    break;

                continue;
            }

            list.Add(itemLowData.ContentsListInfoList[i]);
        }

        return list;
    }

    /// <summary> 아이템 카테고리 리스트 넘겨줌</summary>
    public List<Item.CategoryListInfo> GetLowDataCategoryList()
    {
        return itemLowData.CategoryListInfoList;
    }

	public JSONObject getJsonObj(string tableName){
		TextAsset dataSub = Resources.Load("TestJson/"+tableName, typeof(TextAsset)) as TextAsset;
		StringReader srSub = new StringReader(dataSub.text);
		string strSrcSub = srSub.ReadToEnd();
		JSONObject StringUnitSub = new JSONObject(strSrcSub);

		return StringUnitSub;
	}

    /// <summary> 신분 리스트 </summary>
    public List<Character.StatusInfo> GetLowDataCharStatusList()
    {
        return charLowData.StatusInfoList;
    }

    /// <summary> 신분 </summary>
    public Character.StatusInfo GetLowDataCharStatus(uint id)
    {
        for(int i=0; i < charLowData.StatusInfoList.Count; i++)
        {
            if (!charLowData.StatusInfoList[i].Id.Equals(id))
                continue;

            return charLowData.StatusInfoList[i];
        }

        Debug.LogError("not found CharStatus Data error " + id);
        return null;
    }

    /// <summary> 스킬 셋트 정보 </summary>
    public SkillTables.SkillSetInfo GetLowDataSkillSet(uint id)
    {
        SkillTables.SkillSetInfo data = null;
        if (!skillLowData.SkillSetInfoDic.TryGetValue(id, out data ))
            Debug.LogError("not found skillSetIfno error " + id);

        return data;
    }
}

public static class EncryptHelper
{
    // SKC (Secure KeyCode)
    static byte SKC_BYTE;
    static short SKC_SHORT;
    static ushort SKC_USHORT;
    static int SKC_INT;
    static uint SKC_UINT;
    static long SKC_LONG;
    static ulong SKC_ULONG;

    static EncryptHelper()
    {
        SKC_BYTE = (byte)255;
        SKC_SHORT = (short)32124;
        SKC_USHORT = (ushort)45235;
        SKC_INT = (int)2147483214;
        SKC_UINT = (uint)2047483214;
        SKC_LONG = (long)2107483214;
        SKC_ULONG = (ulong)2147083214;

        //SKC_BYTE = (byte)Random.Range( 99, byte.MaxValue );
        //SKC_SHORT = (short)Random.Range( 9999, short.MaxValue );
        //SKC_USHORT = (ushort)Random.Range( 9999, ushort.MaxValue );
        //SKC_INT = (int)Random.Range( 9999, int.MaxValue );
        //SKC_UINT = (uint)Random.Range( 9999, int.MaxValue );
        //SKC_LONG = (long)Random.Range( 9999, int.MaxValue );
        //SKC_ULONG = (ulong)Random.Range( 9999, int.MaxValue );
    }

    public static byte SSecureBYTE(byte value) { return (byte)(value ^ SKC_BYTE); }
    public static byte GSecureBYTE(byte value) { return (byte)(value ^ SKC_BYTE); }

    public static short SSecureSHORT(short value) { return (short)(value ^ SKC_SHORT); }
    public static short GSecureSHORT(short value) { return (short)(value ^ SKC_SHORT); }

    public static ushort SSecureUSHORT(ushort value) { return (ushort)(value ^ SKC_USHORT); }
    public static ushort GSecureUSHORT(ushort value) { return (ushort)(value ^ SKC_USHORT); }

    public static int SSecureINT(int value) { return (int)(value ^ SKC_INT); }
    public static int GSecureINT(int value) { return (int)(value ^ SKC_INT); }

    public static uint SSecureUINT(uint value) { return (uint)(value ^ SKC_UINT); }
    public static uint GSecureUINT(uint value) { return (uint)(value ^ SKC_UINT); }

    public static long SSecureLONG(long value) { return (long)(value ^ SKC_LONG); }
    public static long GSecureLONG(long value) { return (long)(value ^ SKC_LONG); }

    public static ulong SSecureULONG(ulong value) { return (ulong)(value ^ SKC_ULONG); }
    public static ulong GSecureULONG(ulong value) { return (ulong)(value ^ SKC_ULONG); }
}


