using UnityEngine;
using System;
using System.Collections.Generic;

public class NetProcess : UIBasePanel {

    public BoxCollider Collider;
    public TweenRotation TweenRot;
    
    struct ProcessData
    {
        public string Name;
        public DateTime StartTime;
    }

    private List<ProcessData> DataList = new List<ProcessData>();

    public override void LateInit()
    {
        base.LateInit();

        string proName = (string)parameters[0];

        if (!CheckedProcessName(proName))//이미 실행중. 무시함
            return;

        StartProcess(proName);
    }

    //중복 체크
    public bool CheckedProcessName(string proName)
    {
        int loopCount = DataList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            ProcessData checkData = DataList[i];
            if (checkData.Name.Equals(proName) )
                return false;
        }

        return true;
    }

    void StartProcess(string proName)
    {
        //if(DataList.Count <= 0 )
        //    gameObject.SetActive(true);
        
        ProcessData data;
        data.Name = proName;
        data.StartTime = DateTime.Now.AddSeconds(10);//10초 이상!
        DataList.Add(data);
    }

    public void EndProcess(string proName)
    {
        int loopCount = DataList.Count;
        for(int i=0; i < loopCount; i++)
        {
            ProcessData data = DataList[i];
            if (!data.Name.Contains(proName))
                continue;

            //Debug.Log(string.Format("End NetProcess={0} RunTime={1}", proName, DateTime.Now - data.StartTime));
            DataList.Remove(data);
            break;
        }

        if (0 < DataList.Count)//혹시 데이터가 쌓였다면 다시 실행 해준다.
        {
            //ProcessData reData = DataList[0];
            //StartProcess(reData.Name);
        }
        else
            Hide();
    }

    public override void Hide()
    {
        if (DataList.Count <= 0)
            base.Hide();
    }

    void LateUpdate()
    {
        for (int i = 0; i < DataList.Count; i++)
        {
            ProcessData data = DataList[i];
            if (DateTime.Now < data.StartTime)
                continue;
            
            DataList.Remove(data);
            Hide();
            break;
        }
    }

    public int GetProcessCount()
    {
        if (DataList.Count <= 0)
            return 0;

        return DataList.Count;
    }

    public override void HideEvent()
    {
        base.HideEvent();
        SceneManager.instance.HideRoot();
    }
}
