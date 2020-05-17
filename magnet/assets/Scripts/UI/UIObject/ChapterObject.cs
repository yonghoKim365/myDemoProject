using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChapterObject : MonoBehaviour {

    public List<GameObject> StageObjs;
    public UITexture Background;
    public UITexture BackgroundLine;        // 스테이지마다 연결된 길

    public GameObject InvenItemPrefab;

    private bool IsLevelHard;
    private InvenItemSlotObject[] FirstRewardSlot;
    private InvenItemSlotObject[] BasickRewardSlot;

    void Awake()
    {
        CheckStageObjs();

        IsLevelHard = gameObject.name.Contains("Hard") ? true : false;

        if (IsLevelHard)
        {
            FirstRewardSlot = new InvenItemSlotObject[3];
            BasickRewardSlot = new InvenItemSlotObject[3];

            for (int i = 0; i < StageObjs.Count; i++)
            {
                GameObject slotGo = Instantiate(InvenItemPrefab) as GameObject;
                Transform tf = slotGo.transform;
                tf.parent = StageObjs[i].transform.FindChild("FirstReward/icon").transform;
                tf.localPosition = Vector3.zero;
                tf.localScale = Vector3.one;

                FirstRewardSlot[i] = slotGo.GetComponent<InvenItemSlotObject>();
                FirstRewardSlot[i].EmptySlot();


                GameObject slotGo_ = Instantiate(InvenItemPrefab) as GameObject;
                Transform tf_ = slotGo_.transform;
                tf_.parent = StageObjs[i].transform.FindChild("BasicReward/icon").transform;
                tf_.localPosition = Vector3.zero;
                tf_.localScale = Vector3.one;

                BasickRewardSlot[i] = slotGo_.GetComponent<InvenItemSlotObject>();
                BasickRewardSlot[i].EmptySlot();
            }
        }
    }

    public void CheckStageObjs()
    {
        if (StageObjs == null || StageObjs.Count == 0 || StageObjs[0] == null)
        {
            if (StageObjs == null)
                StageObjs = new List<GameObject>();

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform stage = transform.FindChild(string.Format("{0}", i));
                if (stage == null)
                    continue;

                StageObjs.Add(stage.gameObject);
            }

        }
    }

    /// <summary> 스테이지 셋팅 </summary>
    /// <param name="list">스테이지 데이터</param>
    /// <param name="callBack">스테이지 클릭시 호출할 콜백 함수</param>
    public void SetChapter(List<ChapterPanel.DataStage> list, System.Action<ChapterPanel.DataStage> callBack, string chapterName)
    {
	   gameObject.name = chapterName;
	   SetStage(list, callBack);
    }

    /// <summary>
    /// Instantiate를 계속하지 않을려고 추가한 함수.
    /// Stage의 Transform의 정보, 배경이미지를 바꿔준다.
    /// </summary>
    /// <param name="targetChapter"></param>
    public void ChangeLocalData(ChapterObject targetChapter)
    {
        targetChapter.CheckStageObjs();

        for (int i = 0; i < StageObjs.Count; i++)
        {
            Transform tf = StageObjs[i].transform;

            //하드는 3개이므로 ,, 
            if (i >= targetChapter.StageObjs.Count)
            {
                tf.gameObject.SetActive(false);
                continue;
            }

            tf.gameObject.SetActive(true);
            Transform targetTf = targetChapter.StageObjs[i].transform;

            tf.localPosition = targetTf.localPosition;
            tf.localScale = targetTf.localScale;
            tf.rotation = targetTf.rotation;
        }

        Background.mainTexture = targetChapter.Background.mainTexture;
        Background.color = targetChapter.Background.color;
        BackgroundLine.mainTexture = targetChapter.BackgroundLine.mainTexture;
        BackgroundLine.width = targetChapter.BackgroundLine.width;
        BackgroundLine.height = targetChapter.BackgroundLine.height;
        BackgroundLine.gameObject.transform.localPosition = targetChapter.BackgroundLine.gameObject.transform.localPosition;
    }

    /// <summary>
    /// 스테이지 셋팅
    /// </summary>
    /// <param name="list">스테이지 정보 리스트</param>
    /// <param name="callBack">클릭시 호출될 콜백함수</param>
    void SetStage(List<ChapterPanel.DataStage> list, System.Action<ChapterPanel.DataStage> callBack)
    {         
        IsLevelHard = list[0]._StageLowData.StageId >= 10000 ? true : false;

        for (int i = 0; i < StageObjs.Count; i++)
        {
            if (i >= list.Count)
                continue;

	        Transform tf = StageObjs[i].transform;
	        ChapterPanel.DataStage data = list[i];

            TweenAlpha tween;
            if (tf.FindChild("iconeff").gameObject.GetComponent<TweenAlpha>() == null)
            {
                tf.FindChild("iconeff").gameObject.AddComponent<TweenAlpha>();
            }
            tween = tf.FindChild("iconeff").gameObject.GetComponent<TweenAlpha>();

            tween.transform.gameObject.SetActive(false);

            //하드모드는 최초보상과 기본보상이존재..
            if (IsLevelHard)
            {
                GatchaReward.FixedRewardInfo basicInfo = _LowDataMgr.instance.GetFixedRewardItem(list[i]._StageLowData.FixedReward);
                GatchaReward.FixedRewardInfo firstInfo = _LowDataMgr.instance.GetFixedRewardItem(list[i]._StageLowData.FirstReward);

                if (basicInfo != null)
                    BasickRewardSlot[i].SetLowDataItemSlot(basicInfo.ItemId, basicInfo.ItemCount);
                if (firstInfo != null)
                    FirstRewardSlot[i].SetLowDataItemSlot(firstInfo.ItemId, firstInfo.ItemCount);

                GameObject fisrtRewardRecieve = tf.FindChild("FirstReward/Receive").gameObject;
                fisrtRewardRecieve.SetActive(list[i].State == 1);

            }



            Quest.QuestInfo quest = QuestManager.instance.GetCurrentQuest();
            if (quest!=null ) //현재 퀘스트가 모험입장쿠ㅔ인가?
            {
                if(quest.type == 1 ||quest.type ==2)
                {
                    if (list[i].TableID == quest.ParamId)
                    {
                        tween.style = UITweener.Style.PingPong;
                        tween.from = 0.2f;
                        tween.to = 1f;
                        tween.transform.gameObject.SetActive(true);
                    }
                }
               
            }
            //클리어 등급 셋팅
            int childCount = tf.FindChild("ClearGrade").childCount;
            for (int j = 0; j < childCount; j++)
            {
                Transform gradeTf = tf.FindChild(string.Format("ClearGrade/{0}", j)).GetChild(0);
                if (gradeTf == null)
                    break;

                if (j < data.TotalClearGrade)
                    gradeTf.gameObject.SetActive(true);
                else
                    gradeTf.gameObject.SetActive(false);
            }
        
            ////스테이지 버튼 처리. 난이도 체크 확인하면 지우고 밑에걸로..
            bool isEnable = data.State != -1 ? true : false;

            tf.FindChild("icon_on").gameObject.SetActive(isEnable);
            tf.FindChild("icon_off").gameObject.SetActive(!isEnable);

            string namePath = (isEnable ? "icon_on" : "icon_off") + "/name_d4";
            UILabel nameLbl = tf.FindChild(namePath).GetComponent<UILabel>();
            nameLbl.text = VerticalText(data.StageName);
            
            UIEventTrigger uiTri = tf.GetComponent<UIEventTrigger>();
            if (uiTri == null)
                continue;
            
            EventDelegate.Set(uiTri.onClick, delegate () { callBack(data); });
        }
    }

    string VerticalText(string stageName)
    {
        string newStr = "";
        char[] arr = stageName.ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == '[' || arr[i] == ']')
                continue;

            if(string.IsNullOrEmpty(newStr) )
                newStr = string.Format("{0}", arr[i]);
            else
                newStr += string.Format("\n{0}", arr[i]);
        }

        return newStr;
    }
}
