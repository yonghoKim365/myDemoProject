using UnityEngine;
using System.Collections.Generic;

public class Immortal : MonoBehaviour
{
	protected void SetImmortal()
	{
		string name = gameObject.name;
		gameObject.name += "dummy";
		GameObject obj = GameObject.Find( name );
		if (obj != null)
		{
			DestroyImmediate( gameObject );
		}
		else
		{
			gameObject.name = name;
			DontDestroyOnLoad( gameObject );
		}
	}

	public void Awake()
	{
		//base.Awake();

		SetImmortal();
	}
}

public class Immortal<T> : MonoBehaviour where T : Immortal<T>
{
    private static T m_Instance = null;
	public static T instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = GameObject.FindObjectOfType( typeof( T ) ) as T;

				if (m_Instance == null)
				{
                    if (canCreate)
                        m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    //if (m_Instance == null)
                    //{
                    //    Debug.LogError( "Immortal Intance Init ERROR - " + typeof( T ).ToString() );
                    //}
				}
                else
				    m_Instance.Init();
			}
			return m_Instance;
		}
	}

    /// <summary> 싱글톤 객체별 Hierarchy상 위치를 자동으로 잡기 위한 속성 ex) Root/Category/Etc.. </summary>
    protected virtual string ImmortalHierarchyPath
    { 
        get { return string.Empty; }
    }

	public int InstanceID;
	public new Transform transform { get; private set; }
	public new GameObject gameObject { get; private set; }
    static bool canCreate = true;
    
	public virtual void Awake()
	{
        Init();
	}

	protected virtual void Init()
    {
        if (m_Instance == null)
        {
            transform = base.transform;
            gameObject = base.gameObject;
            InstanceID = GetInstanceID();

            m_Instance = this as T;
            DontDestroyOnLoad(base.gameObject);
            //m_Instance.Init();
        }
        else
        {
            if (m_Instance != this)
                DestroyImmediate(base.gameObject);
        }

        PositionInHierarchy(ImmortalHierarchyPath);
    }

    private void OnApplicationQuit()
    {
        canCreate = false;
        //m_Instance = null;
    }

    /// <summary> Hierarchy상에서의 위치를 조절 </summary>
    private void PositionInHierarchy(string hierarchyPath)
    {
        if (string.IsNullOrEmpty(hierarchyPath))
            return;

        string[] names = hierarchyPath.Split(new char[] { System.IO.Path.AltDirectorySeparatorChar });
        if (string.IsNullOrEmpty(names[0]))
            return;

        GameObject rootGO = GameObject.Find(names[0]);
        GameObject parentGO = rootGO ? rootGO : new GameObject(names[0], typeof(DontDestroyObject));

        for (int i = 1; i < names.Length; i++)
        {
            string findName = names[i];
            if (string.IsNullOrEmpty(findName))
                continue;

            Transform foundTrans = parentGO ? parentGO.transform.Find(findName) : null;            
            if (null == foundTrans)
                foundTrans = new GameObject(findName, typeof(DontDestroyObject)).transform;

            if (null != parentGO)
                foundTrans.parent = parentGO.transform;

            parentGO = foundTrans.gameObject;
        }

        if (null != parentGO)
            transform.parent = parentGO.transform;
    }
}
