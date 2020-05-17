using UnityEngine;
using System.Collections;

public class CheckAtlas : MonoBehaviour {

    public PoolAtlasType Type;
    public bool IsAtlas = true;//false일시 강제 Texture로인식

    void Awake()
    {
        if (IsAtlas)
        {
            UIAtlas atlas = AtlasMgr.instance.GetLoadAtlas(Type);
            if (atlas == null)//문제가 심각하다.
                return;

            UISprite sp = GetComponent<UISprite>();
            if (sp == null)//문제가 심각하다.
            {
                Debug.LogError( string.Format("is object( '{0}' ) not sprite error", gameObject.name) );
                return;
            }
            
            sp.atlas = atlas;
        }

        Destroy(this);//스크립트 쓸모없으니 삭제.
    }
}
