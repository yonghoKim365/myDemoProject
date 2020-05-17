using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIModel : MonoBehaviour
{
    public Vector3 Scale = new Vector3(180, 180, 180);
    public Vector3 Rotation = new Vector3(0, 180, 0);
    
    public UnitAnimator UnitAnim = new UnitAnimator();
    public GameObject[] EffectObj;
    
    private int RenderQ = 3011;

    public void Init(Transform parent, UnitAnimator anim, GameObject[] effect, bool isReviseScale, bool isShadow, string panelName)
    {
        UnitAnim = anim;

        Transform unitTf = transform;
        unitTf.parent = parent;
        unitTf.localEulerAngles = Rotation;

        // 테이블에서 스케일과 포지션값을 읽어온다
        Partner.PartnerScaleInfo info = _LowDataMgr.instance.GetPartnerScaleInfo(gameObject.name, panelName);
        if (info == null)
        {
            Debug.Log(string.Format("{0} is null", gameObject.name));

            unitTf.localScale = Scale;
            unitTf.localPosition = Vector3.zero;
        }
        else
        {
            if (info.rotate_y == 0)  //0일경우 회전값을 강제로 돌려줘야함
                unitTf.localEulerAngles = Rotation;
            else
                unitTf.localEulerAngles = new Vector3(info.rotate_x, info.rotate_y, 0);

            if (info.scale == 0)
                unitTf.localScale = Scale;
            else
                unitTf.localScale = new Vector3(info.scale, info.scale, info.scale);

            unitTf.localPosition = new Vector3(info._x, info._y, 0);

        }

        if ( !string.IsNullOrEmpty(panelName) && panelName.Contains("TownPanel"))
        {
            if (unitTf.name.Contains("pc_f"))
            {
                unitTf.transform.localPosition = new Vector3(0, -12, 0);
            }
            else if (unitTf.name.Contains("pc_p"))
            {
                unitTf.transform.localPosition = new Vector3(0, -4, 0);
            }
            else if (unitTf.name.Contains("pc_d"))
            {
                unitTf.transform.localPosition = new Vector3(-1.93f, 6.39f, 0);
                unitTf.transform.localScale = new Vector3(190,190,190);

            }

        }




        if (effect != null)
        {
            GameObject readyPop = UIMgr.GetUI("UIPopup/ReadyPopup");
            UIPanel panel = null;
            if (readyPop != null)
            {
                panel = readyPop.GetComponent<UIPanel>();
            }
            else
            {
                UIBasePanel curUI = UIMgr.instance.GetCurPanel();
                if(curUI != null)
                    panel = curUI.gameObject.GetComponent<UIPanel>();
            }

            if (panel != null)
                RenderQ = panel.startingRenderQueue + 20;

            if (RenderQ <= 3020)
                RenderQ = 3030;

            if (effect[0] != null)//파트너의 경우 없을 수 있다.
            {
                CheckRenderQ(RenderQ, effect[0].transform);
                List<ParticleSystem> list = UIHelper.FindComponents<ParticleSystem>(effect[0].transform);
                ResettingParticle(parent, list, 0.4f);
            }


            if (effect[1] != null)//파트너의 경우 없을 수 있다.
            {
                CheckRenderQ(RenderQ, effect[1].transform);
                List<ParticleSystem> list = UIHelper.FindComponents<ParticleSystem>(effect[1].transform);
                ResettingParticle(parent, list, 0.4f);
            }
            
            if (effect[0] != null)
                effect[0].SetActive(true);
            if (effect[1] != null)
                effect[1].SetActive(true);
        }

        EffectObj = effect;

        //if (parent.parent != null && parent.parent.FindChild("Shadow") == null)
        if (isShadow && parent.parent != null && parent.parent.FindChild("Shadow") == null)
        {
            GameObject shadowGO = ResourceMgr.Instantiate<GameObject>("Etc/Shadow");
            shadowGO.transform.parent = unitTf;
            shadowGO.transform.localPosition = Vector3.zero;//new Vector3(0, 0.028f, 0f);
            shadowGO.transform.localRotation = Quaternion.Euler(new Vector3(-80, 180, -180));
            shadowGO.transform.localScale = Vector3.one;//new Vector3(1.5f, 1.5f, 1.5f);
            shadowGO.name = "Shadow";

            NGUITools.SetLayer(shadowGO, parent.gameObject.layer);
            SetRenderQueue q = shadowGO.AddComponent<SetRenderQueue>();
            q.renderQueue = 3001;

            shadowGO.transform.parent = parent.parent;
        }

        NGUITools.SetLayer(unitTf.gameObject, LayerMask.NameToLayer("UI"));//parent.gameObject.layer);
        NGUITools.SetChildLayer(unitTf, LayerMask.NameToLayer("UI"));
    }

    public void Init(string panelName, Transform parent, bool isReviseScale, bool UseUICamera = true)
    {
        Transform unitTf = transform;
        unitTf.parent = parent;

        if (UseUICamera)
            unitTf.localEulerAngles = Rotation;
        else
            unitTf.localEulerAngles = Vector3.zero;

        // 테이블에서 스케일과 포지션값을 읽어온다
        Partner.PartnerScaleInfo info = _LowDataMgr.instance.GetPartnerScaleInfo(gameObject.name, panelName);
        if (info == null)
        {
            Debug.Log(string.Format("{0} is null", gameObject.name));

            unitTf.localScale = Scale;
            unitTf.localPosition = Vector3.zero;
        }
        else
        {
            if (info.rotate_y == 0)  //0일경우 회전값을 강제로 돌려줘야함
                unitTf.localEulerAngles = Rotation;
            else
                unitTf.localEulerAngles = new Vector3(info.rotate_x, info.rotate_y, 0);

            //unitTf.localEulerAngles = new Vector3(info.rotate_x, info.rotate_y, 0);

            unitTf.localScale = new Vector3(info.scale, info.scale, info.scale);
            unitTf.localPosition = new Vector3(info._x, info._y, 0);
        }



        NGUITools.SetChildLayer(unitTf, LayerMask.NameToLayer("UI"));
    }

    /// <summary>
    /// 2개 실행 시킬때 사용.
    /// </summary>
    /// <param name="nowAniType">지금 실행할 애니</param>
    /// <param name="queueAniType">다음에 실행할 애니</param>
    /// <param name="crossTime">몇초?</param>
    public void CrossFadeAnimation(eAnimName nowAniType, eAnimName queueAniType, float crossTime=0.1f)
    {
        UnitAnim.PlayAnim(nowAniType);
        UnitAnim.PlayAnim(queueAniType, true, crossTime, true, true, false);
    }
    
    public void PlayAnim(eAnimName aniType)
    {
        if (UnitAnim != null && UnitAnim.Animation != null)
            UnitAnim.PlayAnim(aniType);
    }

    public GameObject PlayIntro(Transform effParent, float parScale)
    {
        GameObject effGo = PlayAniSoundAndEff(effParent, eAnimName.Anim_intro, parScale);
        CrossFadeAnimation(eAnimName.Anim_intro, eAnimName.Anim_idle);

        effParent.parent = transform;
        effParent.localPosition = Vector3.zero;
        effParent.localScale = Vector3.one;

        return effGo;
    }

    public GameObject PlayAniSoundAndEff(Transform parentTf, eAnimName anim, float particleSize, bool is2D=false)
    {
        if (UnitAnim == null)
            return null;

        UnitAnim.PlayAnimationSound(anim);
        UnitAnim.PlayAnimationVoice(anim);

        string effName = UnitAnim.GetAnimationEffect(anim);
        if (string.IsNullOrEmpty(effName))
            return null;

        string path = null;
        if (gameObject.name.Contains("par"))
        {
            if (gameObject.name.Contains("makgyeran"))
                path = "_PARTNER/mak_gyeran";
            else
            {
                string[] split = effName.Split('_');
                int length = 0, arr = 0;
                for (int i = 0; i < split.Length; i++)
                {
                    if (length < split[i].Length)//제일 긴 것이 이름일 것이다.
                    {
                        length = split[i].Length;
                        arr = i;
                    }
                }

                path = string.Format("_PARTNER/{0}", split[arr]);
            }

            if (gameObject.name.Contains("jopunghwa"))
            {
                SetWeaponEffectActive(false);
            }
        }
        else if(gameObject.name.Contains("pc"))
        {
            if(gameObject.name.Contains("pc_f") )
                path = string.Format("_PC/fighter");
            else if (gameObject.name.Contains("pc_p"))
                path = string.Format("_PC/pojol");
            else if (gameObject.name.Contains("pc_d"))
                path = string.Format("_PC/doctor");
        }

        Object obj = Resources.Load(string.Format("Effect/{0}/{1}", path, effName));
        GameObject effGo = null;
        if (obj != null)
        {
            effGo = Instantiate(obj) as GameObject;
            CheckRenderQ(RenderQ, effGo.transform);

            List<ParticleSystem> list = UIHelper.FindComponents<ParticleSystem>(effGo.transform);
            ResettingParticle(null, list, particleSize);
        }

        if (effGo == null)
            return null;

        NGUITools.SetChildLayer(effGo.transform, LayerMask.NameToLayer("UI"));
        effGo.SetActive(true);

        effGo.transform.parent = parentTf;
        effGo.transform.localPosition = Vector3.zero;
        effGo.transform.localScale = Vector3.one;
        effGo.transform.localEulerAngles = is2D ? new Vector3(10, 0, 0) : Vector3.zero;//Quaternion.Euler(Vector3.zero);
        
        return effGo;
    }

    public void SetWeaponEffectActive(bool isActive)
    {
        if (EffectObj[0] != null)
            EffectObj[0].SetActive(isActive);

        if (EffectObj[1] != null)
            EffectObj[1].SetActive(isActive);
    }

    /// <summary>
    /// 파티클 사이즈 재조정
    /// </summary>
    /// <param name="go">원하는 객체</param>
    void ResettingParticle(Transform parent, List<ParticleSystem> parList, float offScale)
    {
        float scaleX = 0;
        if (parent == null)
            scaleX = offScale;
        else
        {
            //float x = 2.5f * transform.localScale.x;//parent.localScale.x
            //scaleX = x * 0.0027f;
            scaleX = transform.lossyScale.x;
        }
        
        int listCount = parList.Count;
        for (int i = 0; i < listCount; i++)
        {
            ParticleSystem ps = parList[i];
            if (ps == null)
                continue;

            float startSize = ps.startSize * scaleX;
            //float startSpeed = ps.startSpeed * scaleX;//(scaleX + (scaleX * 0.1f));
            //float startLifeTime = ps.startLifetime * scaleX;//(scaleX+(scaleX*0.1f));
            //if (startSize < 0.1f)//보정수치.
            //    startSize = 0.1f;
            //if (startLifeTime < 0.7f)
            //    startLifeTime = 0.7f;
            //if (startSpeed < 0.2f)
            //    startSpeed = 0.2f;
            //if (startSpeed < 0.12f)
            //startSpeed = 0.12f;

            ps.startSize = startSize;
            //ps.startSpeed = startSpeed;
            //ps.startLifetime = startLifeTime;
        }
    }
    
    void CheckRenderQ(int renderQ, Transform parent)
    {
        for(int i=0; i < parent.childCount; i++)
        {
            Transform tf = parent.GetChild(i);
            if (tf == null)
                continue;

            AddRenderQ(renderQ, tf.gameObject);
            if (0 < tf.childCount)
                CheckRenderQ(renderQ, tf);
        }
    }

    void AddRenderQ(int renderQ, GameObject go)
    {
        
        if (go.particleSystem != null || go.GetComponent<MeshRenderer>() != null)
        {
            if(go.GetComponent<SetRenderQueue>() == null)
            {
                SetRenderQueue q = go.AddComponent<SetRenderQueue>();
                q.renderQueue = renderQ;
            }
        }
    }
    
    void OnEnable()
    {
        if (UnitAnim != null && !UnitAnim.IsPlaying(eAnimName.Anim_idle))
            PlayAnim(eAnimName.Anim_idle);
    }

    public bool IsPlaying(eAnimName anim)
    {
        if (UnitAnim != null && UnitAnim.IsPlaying(anim))
            return true;

        return false;
    }
}
