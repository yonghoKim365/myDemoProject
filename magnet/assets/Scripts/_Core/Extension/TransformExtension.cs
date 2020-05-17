using UnityEngine;
using System.Collections;

public static class TransformExtension
{
    public static Transform SetX(this Transform t, float x)
    {
        Vector3 vector3 = t.position;
        vector3.x = x;
        t.position = vector3;
        return t;
    }

    public static Transform SetY(this Transform t, float y)
    {
        Vector3 vector3 = t.position;
        vector3.y = y;
        t.position = vector3;
        return t;
    }

    public static Transform SetZ(this Transform t, float z)
    {
        Vector3 vector3 = t.position;
        vector3.z = z;
        t.position = vector3;
        return t;
    }

    public static Transform SetLocalX(this Transform t, float x)
    {
        Vector3 vector3 = t.localPosition;
        vector3.x = x;
        t.localPosition = vector3;
        return t;
    }

    public static Transform SetLocalY(this Transform t, float y)
    {
        Vector3 vector3 = t.localPosition;
        vector3.y = y;
        t.localPosition = vector3;
        return t;
    }

    public static Transform SetLocalZ(this Transform t, float z)
    {
        Vector3 vector3 = t.localPosition;
        vector3.z = z;
        t.localPosition = vector3;
        return t;
    }

    public static Transform SetLocalScaleX(this Transform t, float x)
    {
        Vector3 vector3 = t.localScale;
        vector3.x = x;
        t.localScale = vector3;
        return t;
    }

    public static Transform SetLocalScaleY(this Transform t, float y)
    {
        Vector3 vector3 = t.localScale;
        vector3.y = y;
        t.localScale = vector3;
        return t;
    }

    public static Transform SetLocalScaleZ(this Transform t, float z)
    {
        Vector3 vector3 = t.localScale;
        vector3.z = z;
        t.localScale = vector3;
        return t;
    }

    public static Transform FindTransform(this Transform t, string name)
	{
		Transform dt = t.Find(name);
		if (dt)
			return dt;
		else
		{
			foreach (Transform child in t)
			{
				dt = FindTransform(child, name);
				if (dt)
					return dt;
			}
		}
		return null;
	}

    public static Transform DestroyChildren(this Transform t, bool immediate = true)
    {
        // Unity4.5.5f1 이슈
        //  : 이 함수가 Awake()안에서 불리면, Crash됨..

        if (t.childCount > 0)
        {
            int childcount = t.childCount;
            for (int i = 0; i < childcount; ++i)
            {
                Transform go = t.GetChild(0);
                if (go != null)
                    Object.DestroyImmediate(go.gameObject);
            }
        }

        return t;
    }

    /// <summary>
    /// 부모를 설정한다.
    /// </summary>
    /// <param name="parent">부모가 될 대상</param>
    /// <param name="doInit">자식으로 편입후, identity 적용</param>
    public static void AttachTo(this Transform t, Transform parent, bool doInit = true)
    {
        t.parent = parent;
        if (doInit)
        {
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
        }
    }

    public static void AttachTo(this Transform t, Transform parent, Vector3 newLocalPos, Vector3 newLocalScale, Quaternion newLocalRot)
    {
        t.parent = parent;
        t.localPosition = newLocalPos;
        t.localScale = newLocalScale;
        t.localRotation = newLocalRot;
    }
}
