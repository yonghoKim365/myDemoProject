using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public class TweenAsset : MonoBehaviour 
{
    public bool IsDropCoin;
    public Transform target;
    float JumpingHeight = 2f;
    float JumpingDuration = 0.5f;

    private GameObject CoinEff;
    private List<Material> Materials;
    private List<float> MaterialAlphas;
    //private float EffOriAlpha;

    //< 루트모션 셋팅한다.
    //KRootMotionRM rootMotion;
    void Awake()
    {
        //rootMotion = gameObject.GetComponent<KRootMotionRM>();
        //if (rootMotion == null)
        //    rootMotion = gameObject.AddComponent<KRootMotionRM>();

        //AssetbundleLoader.GetRMCurves("coin", (dic) =>
        //{
        //    rootMotion.Init(dic, transform, this.gameObject.animation, this.transform.FindChild("Bip001"));
        //});
    }

    void InitTweenAsset()
    {
        if (IsDropCoin && CoinEff == null)
        {
            Materials = new List<Material>();
            MaterialAlphas = new List<float>();

            CoinEff = UIHelper.CreateEffectInGame(transform, "Fx_IN_coin_02", false);
            int childCount = CoinEff.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform tf = CoinEff.transform.GetChild(i);
                for (int j = 0; j < tf.childCount; j++)
                {
                    if (tf.GetChild(j).renderer == null || tf.GetChild(j).renderer.material == null)
                        continue;

                    Materials.Add(tf.GetChild(j).renderer.material);
                    MaterialAlphas.Add(tf.GetChild(j).renderer.material.GetColor("_TintColor").a);
                }

                if (tf.renderer == null || tf.renderer.material == null)
                    continue;

                Materials.Add(tf.renderer.material);
                MaterialAlphas.Add(tf.renderer.material.GetColor("_TintColor").a);
            }

            CoinEff.SetActive(false);
        }
    }

    float gold;
    public void StartTween(Transform _target, bool boss, float _gold, DropItem.DropType type)
    {
        target = _target;
        gold = _gold;

        if (type == DropItem.DropType.Gold)
        {
            //< 스타트할 애니를 랜덤으로 구한다
            int idx = !boss ? Random.Range(1, 5) : Random.Range(2, 5);

            //string startAni = "Coin_Drop_Start_0" + idx.ToString();
            animation.Play("coin");
            //animation.PlayQueued("Coin_Drop_Idle_01");

            //< 루트모션도 같이 실행해준다.
            //if (startAni != "Coin_Drop_Start_01")
            //    rootMotion.Play(animation[startAni], true);
            float time = animation["coin"].length;
            StartCoroutine(EatAnimEffect(transform, target, 0.6f, time));
        }
        else
            StartCoroutine("AlphaAction", type == DropItem.DropType.WoodBox);
        //< 사운드 실행
        //SoundHelper.PlaySfxSound(250, 1.5f);
    }
    
    IEnumerator AlphaAction(bool isWood)
    {
        yield return new WaitForSeconds(0.5f);

        Renderer ren = null;
        if (!isWood)
        {
            SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
            ren = mesh.renderer;
        }
        else
        {
            MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
            ren = mesh.renderer;
        }

        if (ren != null)
        {
            float alpha = 1f;
            while(0 < alpha)
            {
                alpha -= (1f * Time.deltaTime);
                for(int i=0; i < ren.materials.Length; i++)
                {
                    if (ren.materials[i] == null)
                        continue;

                    if(isWood)
                    {
                        Color c = isWood ? ren.materials[i].color : ren.materials[i].GetColor("_AddColor");
                        c.a = alpha;
                        ren.materials[i].color = c;
                    }
                    else
                    {
                        Color c = ren.materials[i].GetColor("_AddColor");
                        c.a = alpha;
                        ren.materials[i].SetColor("_AddColor", c);
                    }
                    
                }

                yield return null;
            }
        }

        yield return null;
    }

    /*
    /// <summary>
    /// 코인이 점핑 애니메이션을 하게 한다.
    /// </summary>
    IEnumerator JumpingAnimEffect(Transform coinTrans)
    {
        float JumpVelY = JumpingHeight;
        float GravityY = (JumpVelY * 2f) / JumpingDuration;
        float jumpTime = 0f;
        float delta;

        Vector3 destPos = coinTrans.position;
        Vector3 pos = coinTrans.position;

        while (jumpTime <= JumpingDuration)
        {
            pos = coinTrans.position;

            delta = Time.deltaTime;

            pos.y       += JumpVelY * delta;
            JumpVelY    -= GravityY * delta;
            jumpTime    += delta;

            coinTrans.position = pos;

            yield return null;
        }

        coinTrans.position = destPos;
    }
    */
    /// 코인 섭취시 대상으로 이동시키는 애니메이션.
    IEnumerator EatAnimEffect(Transform coinTrans, Transform target, float duration, float delay)
    {
        //yield return StartCoroutine( MoveToPosition(coinTrans, (Quaternion.AngleAxis( Random.Range(0, 360), Vector3.up ) * coinTrans.forward)+ coinTrans.position, delay - 0.33f) );
        if (CoinEff == null)
            InitTweenAsset();

        CoinEff.SetActive(true);
        SkinnedMeshRenderer meshRender = coinTrans.FindChild("coin024").GetComponent<SkinnedMeshRenderer>();
        Color color = meshRender.material.GetColor("_AddColor");
        color.a = 1f;
        meshRender.material.SetColor("_AddColor", color);

        for (int i = 0; i < CoinEff.transform.childCount; i++)
        {
            Transform tf = CoinEff.transform.GetChild(i);
            for (int j = 0; j < tf.childCount; j++)
            {
                NcUvAnimation ncUv = tf.GetChild(j).GetComponent<NcUvAnimation>();
                NcCurveAnimation ncCurve = tf.GetChild(j).GetComponent<NcCurveAnimation>();
                if (ncUv == null)
                    continue;

                ncCurve.enabled = true;
                ncUv.enabled = true;
            }

            NcUvAnimation _ncUv = tf.GetComponent<NcUvAnimation>();
            NcCurveAnimation _ncCurve = tf.GetComponent<NcCurveAnimation>();
            if (_ncUv == null)
                continue;

            _ncCurve.enabled = true;
            _ncUv.enabled = true;
        }

        for (int i=0; i < Materials.Count; i++)
        {
            Color c = Materials[i].GetColor("_TintColor");
            c.a = MaterialAlphas[i];
            Materials[i].SetColor("_TintColor", c);
        }

		EventListner.instance.TriggerEvent("COIN_LOOTING", gold); 
        //for(int i=0; i < CoinEffs.Length; i++)
        //{
        //    Renderer render = CoinEffs[i].renderer;
        //    Color partiC = render.material.GetColor("_TintColor");
        //    partiC.a = EffOriAlpha;
        //    render.material.SetColor("_TintColor", partiC);
        //}
        
        yield return new WaitForSeconds(delay + 0.4f);

        animation.Play("coin_stop");
        //< UI위치로 변경
        //coinTrans.parent = UIMgr.instance.UICamera.camera.transform;
        //Vector3 spawnedPos = MathHelper.WorldToUIPosition(coinTrans.position);
        //coinTrans.transform.localPosition = new Vector3(spawnedPos.x, spawnedPos.y, -270);
        //coinTrans.transform.localScale = new Vector3(90, 90, 1);
        //coinTrans.transform.localRotation = Quaternion.identity;
        //NGUITools.SetLayer(coinTrans.gameObject, 8);
        /*
        //coinTrans.position = Vector3.zero;
        Vector3 upPos = new Vector3(0, 1f, 0);
        //< 현재 위치에서부터 목표 위치까지 이동시킨다.
        //Vector3 f3 = target.position - coinTrans.position;
        float moveSpeed = 0;
        while (true)
        {
            Vector3 targetPos = target.position;
            targetPos.y += 1f;
            moveSpeed += 2f * Time.fixedDeltaTime;
            //if (10f < moveSpeed)
            //    moveSpeed = 10f;
            
            int count = coinTrans.childCount-1;
            for (int i=0; i < coinTrans.childCount; i++)
            {
                if (!coinTrans.GetChild(i).name.Contains("aniBone"))
                    continue;
                else if (Vector3.Distance(coinTrans.GetChild(i).position, targetPos) < 1f)
                {
                    coinTrans.GetChild(i).GetComponent<TweenPosition>().enabled = false;
                    --count;
                    continue;
                }

                TweenPosition tweenPos = coinTrans.GetChild(i).GetComponent<TweenPosition>();
                if (!tweenPos.enabled)
                {
                    tweenPos.to = target.position;
                    tweenPos.from = coinTrans.GetChild(i).position;
                    tweenPos.ResetToBeginning();
                    tweenPos.PlayForward();
                }
                //Vector3 offset = targetPos - coinTrans.GetChild(i).position;
                //coinTrans.GetChild(i).position += (offset.normalized * (moveSpeed*Time.fixedDeltaTime) );
            }

            if (count <= 0)
                break;

            //< 프레임을 좀더 딜레이 걸어준다.
            yield return null;
        }
        */

        for (int i = 0; i < CoinEff.transform.childCount; i++)
        {
            Transform tf = CoinEff.transform.GetChild(i);
            for(int j=0; j < tf.childCount; j++)
            {
                NcUvAnimation ncUv = tf.GetChild(j).GetComponent<NcUvAnimation>();
                NcCurveAnimation ncCurve = tf.GetChild(j).GetComponent<NcCurveAnimation>();
                if (ncUv == null )
                    continue;

                ncCurve.enabled = false;
                ncUv.enabled = false;
            }

            NcUvAnimation _ncUv = tf.GetComponent<NcUvAnimation>();
            NcCurveAnimation _ncCurve = tf.GetComponent<NcCurveAnimation>();
            if (_ncUv == null)
                continue;

            _ncCurve.enabled = false;
            _ncUv.enabled = false;
        }

        float alpha = 1f;//, alpha2 = EffOriAlpha;
        while (0 < alpha)
        {
            alpha -= (2f * Time.deltaTime);
            //alpha2 -= (1f * Time.deltaTime);

            Color c = meshRender.material.GetColor("_AddColor");
            c.a = alpha;
            meshRender.material.SetColor("_AddColor", c);

            for(int i=0; i < MaterialAlphas.Count; i++)
            {
                //if (MaterialAlphas[i] < alpha)
                //    continue;
                
                Color partiC = Materials[i].GetColor("_TintColor");
                if (partiC.a <= 0)
                    continue;

                partiC.a -= 2f * Time.deltaTime;//alpha;
                Materials[i].SetColor("_TintColor", partiC);
            }

            yield return null;
        }

         //EventListner.instance.TriggerEvent("COIN_LOOTING", gold); 
        
        yield return new WaitForEndOfFrame();

        //NGUITools.SetLayer(coinTrans.gameObject, 0);
        if (PoolManager.Pools.ContainsKey("InGameObj"))
            PoolManager.Pools["InGameObj"].Despawn(transform);
    }
    /*
    /// <summary>
    /// 정해진 위치까지만 이동하는 Coroutine
    /// </summary> 
    IEnumerator MoveToPosition(Transform srcTrans, Vector3 target, float duration)
    { 
        Vector3 startPos = srcTrans.position;
        float time = 0;
        float t = 0;

        while (t < 1f)
        {
            srcTrans.position = Vector3.Lerp( startPos, target, easeInCirc( 0, 1, t ) );

            t = Mathf.Clamp(time / duration, 0, 1);

            time += Time.deltaTime;

            yield return null;
        }
    }    
    */
    /// <summary>
    /// 정해진 시간만큼 정해진 타겟까지 이동하는 Coroutine
    /// </summary>
    IEnumerator MoveToTarget(Transform srcTrans, float duration)
    {
        //Vector3 TargetPos = target.position;//G_GameInfo.GameInfo.HudPanel.CoinEffRoot.localPosition;
        Vector3 upPos = new Vector3(0, 1f, 0);
        //< 현재 위치에서부터 목표 위치까지 이동시킨다.
        float MoveSpeed = 2f;
        while (true)
        {
            //Vector3 dic = target.position - srcTrans.transform.position;
            //if (dic.magnitude < 2)
            Vector3 targetPos = target.position;
            targetPos.y += 1f;
            if (Vector3.Distance(srcTrans.transform.position, targetPos) < 1f)
                break;

            Vector3 look = (targetPos - srcTrans.transform.position).normalized;
            srcTrans.Translate(look * (MoveSpeed * Time.fixedDeltaTime), Space.World);
            MoveSpeed += Time.fixedDeltaTime * 1.5f;
            //if (MoveSpeed > 3)
            //    break;//MoveSpeed = 3;

            //< 프레임을 좀더 딜레이 걸어준다.
            yield return null;
        }

        //G_GameInfo.SpawnEffect("Fx_GoldDrop_01", 1, null, null, Vector3.one, (effect) =>
        //{
        //    effect.transform.parent = UIMgr.instance.UICamera.camera.transform;
        //    effect.transform.localPosition = TargetPos;
        //    effect.transform.localScale = Vector3.one * 3;
        //    NGUITools.SetLayer(effect.gameObject, 8);

        //    //< 사운드 실행
        //    //SoundHelper.PlaySfxSound(251, 1.5f);
        //});

        EventListner.instance.TriggerEvent("COIN_LOOTING", gold);

        //Vector3 startPos = srcTrans.position;
        //float time = 0;
        //float t = 0;
        //while (t < 1f)
        //{
        //    if (null == target)
        //        yield break;

        //    srcTrans.position = Vector3.Lerp(startPos, target.position, easeInCirc(0, 1, t));

        //    t = Mathf.Clamp(time / duration, 0, 1);

        //    time += Time.deltaTime;

        //    yield return null;
        //}
    }

    private float easeInCirc(float start, float end, float value){
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}
}
