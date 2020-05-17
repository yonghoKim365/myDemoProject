using UnityEngine;
using System.Collections;

public class ModelUIViewer : MonoBehaviour
{

    public UILabel headmodel;
    public UILabel bodymodel;
    public UILabel weaponmodel;

    public GameObject ModelRoot;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            ModelRoot.transform.Rotate(Vector3.up, 2f, Space.World);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            ModelRoot.transform.Rotate(Vector3.up, -2f, Space.World);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            ModelRoot.transform.Rotate(Vector3.right, 2f, Space.World);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            ModelRoot.transform.Rotate(Vector3.right, -2f, Space.World);
        }
    }

    public void ModelLoad()
    {
        ModelReset();

        GameObject oriUnit = ResourceMgr.Load(string.Format("Character/Prefab/{0}", bodymodel.text)) as GameObject;
        if (oriUnit == null)
        {
            Debug.LogWarning("not found player model error! path = Character/Prefab/" + bodymodel.text);
            oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model")) as GameObject;
            GameObject _ErrorUnit = GameObject.Instantiate(oriUnit) as GameObject;
        }

        GameObject _myUnit = GameObject.Instantiate(oriUnit) as GameObject;

        ModelModifier Modifier = _myUnit.AddComponent<ModelModifier>();

        // 여기까지 베이스모델 로딩완료 머리/무기를 붙이자
        if (!headmodel.text.Equals(""))
        {
            Modifier.ModelApply(string.Format("Character/Prefab/{0}", headmodel.text));
        }

        if (!weaponmodel.text.Equals(""))
        {
            Modifier.ModelApply(string.Format("Character/Prefab/{0}", weaponmodel.text));
        }

        _myUnit.transform.parent = ModelRoot.transform;
        //_myUnit.layer = 8;
        NGUITools.SetLayer(_myUnit, 8);
        _myUnit.transform.localPosition = Vector3.zero;
        _myUnit.transform.localScale = new Vector3(180f, 180f, 180f);
        _myUnit.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

    }

    public void ModelReset()
    {
        if (ModelRoot.transform.childCount != 0)
        {
            Transform model = ModelRoot.transform.GetChild(0);
            if (model != null)
                Destroy(model.gameObject);
        }

        ModelRoot.transform.localRotation = Quaternion.identity;
    }

}
