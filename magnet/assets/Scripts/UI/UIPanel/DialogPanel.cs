using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogPanel : UIBasePanel {
    
    
    UIBasePanel beforePanel = null;
    public TypewriterEffect typewriterEffect = null;
	public GameObject cameraParent;

	enum labelT { MyName, TargetName, MTitle, TalkerName, Talk, Btn, }
	public UILabel[] Labels;

	public string[] beforeModel;

	public GameObject talkGroup;
    public override void Init()
    {
	    base.Init();

		beforeModel = new string[2];
		beforeModel[0] = "";
		beforeModel[1] = "";
		
	}	
	
	public override void LateInit()
    {
	    base.LateInit();

    }

	public void setVisible(bool b){
		talkGroup.SetActive (b);
	}


	public void setDialog(int textId){

		if (talkGroup.activeInHierarchy == false) {
			setVisible(true);
		}

		if (cameraParent.activeInHierarchy == false) {
			cameraParent.SetActive(true);
		}

		//string talk = _LowDataMgr.instance.GetStringCommon((uint)textId);
		//string talk = _LowDataMgr.instance.GetLocalDialogInfo ((uint)textId).String;
		//typewriterEffect.ResetToBeginning(SceneManager.instance.IsPlaySoundFx, string.Format("{0}", talk));

		DialogUpdate (_LowDataMgr.instance.GetLocalDialogInfo ((uint)textId));
	}
    
	void DialogUpdate(Local.StringLocalDialogInfo info)
	{

		string Name = info.NPCNameRIGHT;
		
		//string talkername = "";
		
		if(Name.Equals("0"))
		{
			Name = "";
		}
		
		if (Name.Contains("PLAYER"))
		{
			Name = Name.Replace("PLAYER", NetData.instance.Nickname);
		}
		//오른쪽
		if (string.IsNullOrEmpty(Name) || info.TalkPosition == 0)
		{
			Labels[(int)labelT.MyName].gameObject.SetActive(false);
		}
		else{
			Labels[(int)labelT.MyName].gameObject.SetActive(true);
			Labels[(int)labelT.MyName].text = Name;
		}

		if (info.TalkPosition == 2) // right
		{
			Labels[(int)labelT.MyName].depth = 5;
		}
		else
		{
			Labels[(int)labelT.MyName].depth = -5;
		}
		
		Name = info.NPCNameLEFT;
		
		if (Name.Equals("0"))
		{
			Name = "";
		}
		

		if (Name.Contains("PLAYER"))
		{
			Name = Name.Replace("PLAYER", NetData.instance.Nickname);
		}
		//왼쪽
		
		if (string.IsNullOrEmpty(Name) || info.TalkPosition == 0)
		{
			Labels[(int)labelT.TargetName].gameObject.SetActive(false);
		}
		else{
			Labels[(int)labelT.TargetName].gameObject.SetActive(true);
			Labels[(int)labelT.TargetName].text = Name;
		}

		if (info.TalkPosition == 1) // left
		{
			Labels[(int)labelT.TargetName].depth = 5;
		}
		else
		{
			Labels[(int)labelT.TargetName].depth = -5;
		}
		
		string talk = info.String;
		if (talk.Contains("\\\\\n"))
			talk = talk.Replace("\\\\\n", "\n");
		else if (talk.Contains("\\\\n"))
			talk = talk.Replace("\\\\n", "\n");
		else if (talk.Contains("\\\n"))
			talk = talk.Replace("\\\n", "\n");
		else if (talk.Contains("\\n"))
			talk = talk.Replace("\\n", "\n");
		
		typewriterEffect.ResetToBeginning(SceneManager.instance.IsPlaySoundFx, string.Format("{0}", talk));

	}

	
	
	//안드로이드 뒤로가기 키가 눌리면 꺼지게 해줘야해서 추가함
	public override void Close()
	{
		base.Close();
	}
	
	public override void OnDestroy()
	{
		base.OnDestroy();
		
		if(beforePanel != null)
		{
			beforePanel.Show();
		}
	}
	
	public override PrevReturnType Prev()
	{
		return base.Prev();
	}
	
	
}
