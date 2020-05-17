using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropItem
{
    public class Data
    {
        public uint itemId;
        public byte itemType;
        public uint itemLink;
        public int count;

        public Data(uint _itemId, byte _itemType, uint _itemLink, int _count)
        {
            itemId = _itemId;
            itemType = _itemType;
            itemLink = _itemLink;
            count = _count;
        }
    }

    //< 일단은 골드만 주는걸로 설정
    public static void DropAssets(Transform _from, Transform _target, int dropCnt, float gold, bool boss = false)
    {
        _target.GetComponent<MonoBehaviour>().StartCoroutine(SpawnPrefab("Coin_Drop_02", _from, _target, dropCnt++ * 0.1f, DropType.Gold, gold, boss));
    }

    public enum DropType { Gold, WoodBox, treasureChest, Exp }
    public static void DropItems(Transform _from, Transform _target, int dropCnt, DropType dtype, bool boss = false)
    {
	   _target.GetComponent<MonoBehaviour>().StartCoroutine(SpawnPrefab("Item_Drop_01", _from, _target, dropCnt++ * 0.1f, dtype, 1, boss));
    }

    static IEnumerator SpawnPrefab(string prefabName, Transform _from, Transform _target, float delay, DropType dtype, float gold, bool boss = false)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log(prefabName + "  " + _from.name);
        GameObject coinTrans = G_GameInfo.SpawnInGameObj(prefabName, _from.transform.position, _from.transform.localRotation);
        if (coinTrans == null)
            yield break;

        Debug.Log(prefabName + "  " + _from.name);

        Transform trGbox = coinTrans.transform.FindChild("GoldBox_001");
        Transform trWbox = coinTrans.transform.FindChild("woodbox_001");
        Transform trCoin = coinTrans.transform.FindChild("coin");
        switch (dtype)
        {
	        case DropType.treasureChest:
		        if (trGbox != null) trGbox.gameObject.SetActive(true);
		        if (trWbox != null) trWbox.gameObject.SetActive(false);
		        if (trCoin != null) trCoin.gameObject.SetActive(false);
		        break;
	        case DropType.WoodBox:
		        if (trGbox != null) trGbox.gameObject.SetActive(false);
		        if (trWbox != null) trWbox.gameObject.SetActive(true);
		        if (trCoin != null) trCoin.gameObject.SetActive(false);
		        break;
            case DropType.Exp:

                break;
	        case DropType.Gold:
	        default:
		        if (trGbox != null) trGbox.gameObject.SetActive(false);
		        if (trWbox != null) trWbox.gameObject.SetActive(false);
		        if (trCoin != null) trCoin.gameObject.SetActive(true);
                //SoundManager.instance.PlaySfxSound(eUISfx.DropGold, false);
		        break;
        }

        float itemAdd = dtype != DropType.Gold ? 3 : 0;
        Vector3 spawnedPos = coinTrans.transform.position;
        Vector3 addPos = Random.insideUnitSphere * (float)Random.Range(0 + itemAdd, 2 + itemAdd);

        addPos.y = 0;
        coinTrans.transform.position = spawnedPos + addPos;

        //< 각도를 랜덤으로 잡아줌
        coinTrans.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        
        if (dtype == DropType.Gold)
        {
	        //< 사이즈 조금더 보정해줌
	        coinTrans.transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
	        coinTrans.transform.localScale = Vector3.one * 0.4f;
        }

        TweenAsset asset = coinTrans.GetComponent<TweenAsset>();
        if (asset != null)
            asset.StartTween(_target, boss, gold, dtype);
    }
}