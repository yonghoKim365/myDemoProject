using UnityEngine;
using System.Collections;

public class XRayCamera : MonoBehaviour 
{
 //   int m_SaveCullMask = 0;

	//// Use this for initialization
	//void Start () {
	//   CameraManager.instance.XRayComponent = this;
 //       if (Camera.main != null)
 //       {
 //           m_SaveCullMask = Camera.main.cullingMask;
 //           //Default
 //           //Map
 //           //EventTarget

 //           GameObject a_CdCamObj = new GameObject();
 //           if (a_CdCamObj != null)
 //           {
 //               Camera a_ChildCamObj = a_CdCamObj.AddComponent<Camera>();

 //               a_ChildCamObj.name = "ChildCamera";
 //               a_ChildCamObj.tag = "Untagged";
 //               a_ChildCamObj.clearFlags = CameraClearFlags.Nothing;    //"Don't Clear"

 //               //a_ChildCamObj.cullingMask = -1;  //모든 레이어를 보이게 하고
 //               //a_ChildCamObj.cullingMask = ~(1 << LayerMask.NameToLayer("Unit"));  //Unit만 제외
 //               a_ChildCamObj.cullingMask = 0;
 //               a_ChildCamObj.cullingMask |= 1 << LayerMask.NameToLayer("Default");
 //               a_ChildCamObj.cullingMask |= 1 << LayerMask.NameToLayer("Map");
 //               a_ChildCamObj.cullingMask |= 1 << LayerMask.NameToLayer("EventTarget");
 //               // 잠시 수정
 //               a_ChildCamObj.cullingMask |= 1 << LayerMask.NameToLayer("Obstacle");


 //               a_ChildCamObj.depth = 1;

 //               a_ChildCamObj.projectionMatrix = Camera.main.projectionMatrix;
 //               a_ChildCamObj.fieldOfView = Camera.main.fieldOfView;
 //               a_ChildCamObj.nearClipPlane = Camera.main.nearClipPlane;
 //               a_ChildCamObj.farClipPlane = Camera.main.farClipPlane;
 //               a_ChildCamObj.rect = Camera.main.rect;

 //               a_ChildCamObj.transform.parent = Camera.main.transform;

 //               a_ChildCamObj.transform.localPosition = Vector3.zero;
 //               a_ChildCamObj.transform.localRotation = Quaternion.identity;
 //               a_ChildCamObj.transform.localScale = Vector3.one;
 //           }

 //           Camera.main.cullingMask = 0;
 //           Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Unit");  //Unit만 추가하기...
 //           //Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Foliage");
 //       }//if(Camera.main != null)


 //   }

 //   public void DefualtCameraMode()
 //   {
 //       if (Camera.main != null)
 //       {
 //           Camera.main.cullingMask = m_SaveCullMask;

 //           Camera[] a_Coms = Camera.main.GetComponentsInChildren<Camera>(true);
 //           foreach (Camera a_Unit in a_Coms)
 //           {
 //               if (a_Unit == Camera.main)
 //                   continue;

 //               if (a_Unit.gameObject.name == "ChildCamera")
 //               {
 //                   a_Unit.gameObject.SetActive(false);
 //               }
 //           }
 //       }
 //   }

 //   public void XRaCameraMode()
 //   {
 //       if (Camera.main != null)
 //       {
 //           Camera.main.cullingMask = 0;
 //           Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Unit");  //Unit만 추가하기...
 //           //Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Foliage");

 //           Camera[] a_Coms = Camera.main.GetComponentsInChildren<Camera>(true);
 //           foreach (Camera a_Unit in a_Coms)
 //           {
 //               if (a_Unit == Camera.main)
 //                   continue;

 //               if (a_Unit.gameObject.name == "ChildCamera")
 //               {
 //                   a_Unit.gameObject.SetActive(true);
 //               }
 //           }
 //       }
 //   }
}
