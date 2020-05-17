using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class BillboardTypeObject : MonoBehaviour 
{
	public enum Direction {up, down, left, right, forward, back};
	public bool reverse = false; 
	public Direction direction = Direction.up; 

#if UNITY_EDITOR
	public Camera cam;
#endif
	
	public Vector3 getDirection (Direction dir)
	{
		switch (dir)
		{
		case Direction.down:
			return Vector3.down; 
		case Direction.forward:
			return Vector3.forward; 
		case Direction.back:
			return Vector3.back; 
		case Direction.left:
			return Vector3.left; 
		case Direction.right:
			return Vector3.right; 
		}
		return Vector3.up; 		
	}

	Vector3 _targetPos;
	Vector3 _targetOrientation;

	void  Update ()
	{
#if UNITY_EDITOR
		if(cam != null)
		{
			_targetPos = transform.position + cam.transform.rotation * (reverse ? Vector3.forward : Vector3.back) ;
			_targetOrientation = cam.transform.rotation * getDirection(direction);
		}
		else 
#endif
			if(GameManager.me != null)
		{
			_targetPos = transform.position + GameManager.me.gameCamera.transform.rotation * (reverse ? Vector3.forward : Vector3.back) ;
			_targetOrientation = GameManager.me.gameCamera.transform.rotation * getDirection(direction);
		}
		
		transform.LookAt (_targetPos, _targetOrientation);
	}
}