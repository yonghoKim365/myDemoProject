using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckObstacle : MonoBehaviour
{
    // ======Ray 체크 Alpha빼기 ============

    public GameObject pc;

    private GameObject obstacleObj; //충돌체
    //private Renderer objRenderers;   //충돌체의 Renderer

    private List<Renderer> ListObjRenderers;   //충돌체의 Renderer
    private List<Material> ListObjMaterials;   //충돌체의 Material
    private Material[] objMaterials_;

    public void SetPC(PlayerController playerCtlr)
    {
        pc = playerCtlr.Leader.gameObject;
    }

    bool IsHitObj;

    void LateUpdate()
    {
         if (SceneManager.instance.IsShowLoadingPanel())    //로딩중
            return;

        // 타운에서 내 캐릭터 위치
        if (SceneManager.instance.GetState<TownState>().enabled)
        {
            if (SceneManager.instance.GetState<TownState>().MyHero == null)
                return;
         
            pc = SceneManager.instance.GetState<TownState>().MyHero.gameObject;

        }
        // 인게임에서의 내 캐릭터 위치 
        else if (G_GameInfo.PlayerController != null)
        {
            pc = G_GameInfo.PlayerController.Leader.gameObject;
        }
       
        else
            return; // 캐릭터 선택창

        if (Camera.main == null)
            return;

        if (pc != null)

        {
            Vector3 vy = new Vector3(0, 5, 0);

            // 캐릭터위치 
            Vector3 targetPosition = pc.transform.position;


            // 충돌체의 콜라이더안에 들어가게되면 체크를 못해주기때문에 시작위치를 높게잡아줘야함 
            Ray ray = new Ray(Camera.main.transform.position + vy, targetPosition - Camera.main.transform.position - vy);

            RaycastHit hit;
            int layermask = 1 << 20;

            // 20번 레이어만 체크
            if (Physics.Raycast(ray, out hit, 100, layermask))
            {

                IsHitObj = true;
                obstacleObj = hit.transform.gameObject;

                // 카메라가 오브젝트에 들어가있는지도 체크? (마을에서 )
                 //if(Camera.main.transform. )


                if (ListObjRenderers != null)
                {
                    for (int i = 0; i < ListObjRenderers.Count; i++)
                    {
                        // 연속으로 체크될때 , 앞에서 체크된 알파값을 복구해줘야 하므로 비교해준다
                        if (ListObjRenderers[0] != obstacleObj.GetComponentInParent<Renderer>())
                        {
                            // 이전의 충돌체 알파값은 1f로 복구시켜주고 
                            // 새로 체크한 obstacleObj 의 값을 넣어줘야함 
                            StopCoroutine("FadeOutAlpha");

                            // 자식이 있으면 자식의 렌더러도 넣어줘야함
                            if (obstacleObj.transform.childCount >= 1)
                            {
                                for (int k = 0; k < obstacleObj.transform.childCount; k++)
                                {
                                    if (obstacleObj.transform.GetChild(k).GetComponent<Renderer>() != null)
                                        ListObjRenderers.Add(obstacleObj.transform.GetChild(k).GetComponent<Renderer>());
                                }
                            }
                            // 전부 알파 복구해준다
                            for (int j = 0; j < ListObjRenderers.Count; j++)
                            {
                                objMaterials_ = ListObjRenderers[j].materials;
                                FadeInAlpha(objMaterials_);
                            }
                            break;

                        }
                    }
                }

                ListObjRenderers = new List<Renderer>();
                // 자식이 존재한다면 자식의 랜더러도 넣어줘야함
                if (obstacleObj.transform.childCount >= 1)
                {
                    // 부모의 렌더러
                    //objRenderers = obstacleObj.GetComponentInParent<MeshRenderer>();
                    //ListObjRenderers.Add(objRenderers);
                    if(obstacleObj.GetComponentInParent<Renderer>()!=null)
                    {
                        ListObjRenderers.Add(obstacleObj.GetComponentInParent<Renderer>());
                    }
                    //if(obstacleObj.GetComponentInParent<MeshRenderer>() !=null)
                    //{
                    //    ListObjRenderers.Add(obstacleObj.GetComponentInParent<MeshRenderer>());

                    //}
                    //else if(obstacleObj.GetComponentInParent<SkinnedMeshRenderer>() != null)
                    //{
                    //    ListObjRenderers.Add(obstacleObj.GetComponentInParent<SkinnedMeshRenderer>());
                    //}

                    for (int i = 0; i < obstacleObj.transform.childCount; i++)
                    {
                        //자식의 렌더러 넣어줌
                        if (obstacleObj.transform.GetChild(i).GetComponent<MeshRenderer>() == null)
                        {
                            if(obstacleObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() != null)
                                ListObjRenderers.Add(obstacleObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>());
                        }
                        else
                        {
                            if (obstacleObj.transform.GetChild(i).GetComponent<Renderer>() != null)
                            {
                                ListObjRenderers.Add(obstacleObj.transform.GetChild(i).GetComponent<MeshRenderer>());

                            }
                        }
                    }
                }
                // 자식이 없을경우
                else
                {
                    if (obstacleObj.GetComponentInParent<Renderer>() != null)
                        ListObjRenderers.Add(obstacleObj.GetComponentInParent<Renderer>());
                    //objRenderers = obstacleObj.GetComponentInParent<MeshRenderer>();
                    //ListObjRenderers.Add(objRenderers);
                }


                ListObjMaterials = new List<Material>();
                for (int i = 0; i < ListObjRenderers.Count; i++)
                {
                    // 렌더러의 머테리얼 뽑아내기 
                    if (ListObjRenderers[i] == null)
                        continue;

                    objMaterials_ = ListObjRenderers[i].materials;
                    for (int j = 0; j < objMaterials_.Length; j++)
                    {
                        ListObjMaterials.Add(objMaterials_[j]);
                    }
                    

                }

                // 머테리얼들의 알파값을 빼준다
                StartCoroutine("FadeOutAlpha", ListObjMaterials);


            }
            else if (IsHitObj)
            {

                StopCoroutine("FadeOutAlpha");
                //Debug.Log("코루틴 종료");


                //자식유무 다시확인
                for (int i = 0; i < ListObjRenderers.Count; i++)
                {
                    if (ListObjRenderers[i] == null)
                        break;
                    objMaterials_ = ListObjRenderers[i].materials;
                    FadeInAlpha(objMaterials_); //전부 복구
                }


                // 리스트 삭제 
                ListObjRenderers.Clear();
                ListObjMaterials.Clear();

                IsHitObj = false;


            }


            Debug.DrawRay(ray.origin, ray.direction * 500, Color.red);

        }



    }

    IEnumerator FadeOutAlpha(List<Material> mat)
    {
        //Debug.Log("코루틴ing");

        // 알파값은 서서히빠지게.. 
        for (int i = 0; i < mat.Count; i++)
        {
            Color col = mat[i].color;
            float a = 0.05f;

            if (mat[i].color.a > 0.25)
            {
                col.a -= a;
                mat[i].color = col;
            }


            //Debug.Log(string.Format("{0}번째 material 코루틴ing", i.ToString()));
        }

        yield return new WaitForSeconds(0.01f);

    }

    public void FadeInAlpha(Material[] mat)
    {
        // 복구되는건 한번에

        for (int i = 0; i < mat.Length; i++)
        {
            Color col = mat[i].color;
            col.a = 1.0f;
            mat[i].color = col;
        }

        


    }



    //#if UNITY_EDITTER

    //void OnGUI()
    //{
    //    if (G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
    //    {

    //        GUI.Label(new Rect(70, 70, 100, 100), " mat1.alpha: " + mattt[0].color.a);
    //        GUI.Label(new Rect(70, 100, 100, 100), " mat2.alpha: " + mattt[1].color.a);

    //    }
    //}
    //#endif

}


