using UnityEngine;
using System.Collections;

public class UISpecialPackPopup : UIPopupBase 
{
	public UILabel lbRibonTitle;

	public UILabel lbTitle;
	public UILabel lbsubTitle;

	public UILabel lbBottomLine;

	public UISpecialPackSlot slot1;
	public UISpecialPackSlot slot2;

	public UISpecialPackSlot[] slots;
//	public UISpecialPackSlot slot2;
//	public UISpecialPackSlot slot3;
//	public UISpecialPackSlot slot4;



	protected override void awakeInit ()
	{

	}


	public void setData(P_Package[] data){

		string[] txts = data[0].desc.Split('/');

		if (data[0].category == SpecialPackage.SPECIAL)
		{
			slot1.gameObject.SetActive(false);
			slot2.gameObject.SetActive(false);
			for(int i=0;i<slots.Length;i++){
				if (slots[i] != null){
					slots[i].gameObject.SetActive(false);
				}
			}

			for(int i=0;i<data.Length;i++){
				if (slots[i] != null){
					slots[i].gameObject.SetActive(true);
				}
			}

			if (data.Length == 2){
				slots[0].gameObject.transform.localPosition = new Vector3(-135, 163, 0);
				slots[1].gameObject.transform.localPosition = new Vector3(135, 163, 0);
			}
			else if (data.Length == 4){
				slots[0].gameObject.transform.localPosition = new Vector3(-405, 163, 0);
				slots[1].gameObject.transform.localPosition = new Vector3(-135, 163, 0);
				slots[2].gameObject.transform.localPosition = new Vector3(135, 163, 0);
				slots[3].gameObject.transform.localPosition = new Vector3(405, 163, 0);
			}

			//data[0].imageUrl = "http://office.linktomorrow.com/common/windsoul/specialsample.png";
			//data[1].imageUrl = "http://office.linktomorrow.com/common/windsoul/specialsample.png";

			lbRibonTitle.text = txts[0];
			lbTitle.text = txts[1];
			
			if( string.IsNullOrEmpty(data[0].endDate) == false)
			{
				string endDate = data[0].endDate;
				endDate = endDate.Substring(4,2) + Util.getUIText("MONTH") + " " + endDate.Substring(6,2) +  Util.getUIText("DAY");
				lbsubTitle.text = txts[2].Replace("{0}", data[0].buyCount.ToString()).Replace("{1}", endDate)  ;
			}
			else
			{
				lbsubTitle.text = txts[2].Replace("{0}", data[0].buyCount.ToString());
			}

			lbBottomLine.text = txts[txts.Length-1];

			for(int i=0;i<data.Length;i++){
				slots[i].setData(data[i], hide);

				if(slots[i].isImageType == false){
					string[] packageText = data[i].desc.Split('/');

					if (slots[i].lbItemTitle != null){
						slots[i].lbItemTitle.text = packageText[3];
					}
					if (slots[i].lbRubyCount != null){
						slots[i].lbRubyCount.text = packageText[4];
					}
					if (slots[i].lbDescription != null){
						slots[i].lbDescription.text = Util.stringJoin("\n",packageText,5, packageText.Length-2);
					}
				}

			}
		}

		show ();

		for(int i=0;i<slots.Length;i++){
			if (slots[i] != null)slots[i].loadImage();
		}

	}


	public void setData(P_Package data1)
	{
		string[] txts = data1.desc.Split('/');
		string[] txts2 = null;
		string[] temp = null;


		switch(data1.category)
		{
		case SpecialPackage.RUNE:

			//0 역전! 패자부활 소환룬 특별강화 패키지!/
			//1 용사님의 재도전을 응원하며!/
			//2 소환팩/

			//3 프리미엄/
			//4 소환룬 5개/

			//5 +/
			//6 루비 150개/
			//7 30,000 골드/
			//8 BONUS!!/
			//9 해당 팝업을 통해서 월 1회 구매가 가능한 상품입니다.

			lbRibonTitle.text = txts[0];
			lbTitle.text = txts[1];

			lbBottomLine.text = txts[txts.Length-1];

			slot1.setData(data1, hide);

			slot1.lbItemTitle.text = txts[2];

			if(slot1.isImageType == false)
			{
				slot1.lbRubyCount.text = txts[3] + "\n" + txts[4];
				slot1.lbDescription.text = Util.stringJoin("\n",txts,5, txts.Length-3);

				_v = slot1.lbDescription.transform.localPosition;
				_v.y = slot1.lbRubyCount.transform.localPosition.y - slot1.lbRubyCount.printedSize.y * slot1.lbRubyCount.transform.localScale.y;
				slot1.lbDescription.transform.localPosition = _v;
			}

			slot1.spItemPriceIcon.enabled = (txts[txts.Length-2].Contains("BONUS"));
			
			if(slot1.spItemPriceIcon.enabled)
			{
				_v = slot1.spItemPriceIcon.transform.localPosition;
				_v.y = slot1.lbDescription.transform.localPosition.y - slot1.lbDescription.printedSize.y - 5f;
				slot1.spItemPriceIcon.transform.localPosition = _v;
			}

			break;
		}

		show ();

		if(slot1 != null) slot1.loadImage();

	}

}
