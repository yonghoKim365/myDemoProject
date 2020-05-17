using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class WindSoulConfig : MonoBehaviour {

	[SerializeField]
	public DebugManager debugManager;

	[SerializeField]
	public ResourceManager resourceManager;

	[SerializeField]
	public UnitSkillCamMaker unitcamMaker;

	public void SetDirty ()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
		#endif
	}

}
