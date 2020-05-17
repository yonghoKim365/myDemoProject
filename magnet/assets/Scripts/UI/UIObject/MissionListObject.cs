using UnityEngine;
using System.Collections;

public class MissionListObject : MonoBehaviour {

    public UILabel Title, Description;
    public UILabel LockDesc;
    /*
    public void SetupMission(MissionData _mdata)
    {
	   Title.text = _mdata.Title;
	   Description.text = _mdata.Description;
    }
    */
    public void SetupMission(string title, string description)
    {
        Title.text = title;
        Description.text = description;
    }

    public void SetLock(string str)
    {
        LockDesc.text = str;
    }
}
