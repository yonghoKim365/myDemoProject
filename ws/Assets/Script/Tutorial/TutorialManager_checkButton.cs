using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// 버튼 체크를 통해 튜토리얼을 진행 시킨다.
public partial class TutorialManager : MonoBehaviour 
{
	bool check1(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,false,false);
				setArrowAndDim(567,107);
			}
			return false;
			break;
		case 2:
			if(go == uiManager.uiMenu.uiLobby.btnStart.gameObject)
			{
				++subStep;
				openDialog(200,300,false,false,true);
				uiTutorial.setArrowAndDim(787,93);
				return true;
			}
			return false;
			break;
		case 3:
			if(go == uiManager.uiMenu.uiWorldMap.btnStartEpicStage.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(360,400,false,true);
				uiTutorial.setDimPosition(261,418);
				return true;
			}
			return false;
			break;
		case 4:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				setArrowAndDim(687,120);
				return true;
			}
			return false;
			break;
		case 5:
			if(go == uiManager.uiMenu.uiWorldMap.stageDetail.btnStartRound.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);
			}
			return false;
			break;
		case 6:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial(nowTutorialId);
			}
			return false;
			break;
		case 7: // 완료 팝업 클릭.
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				++subStep;
				closeTutorial();

				uiTutorial.hide();
				uiManager.uiMenu.uiWorldMap.stageDetail.onStartRound(null);
				uiManager.uiMenu.uiWorldMap.stageDetail.btnStartRound.gameObject.SetActive(true);
			}
			return false;
			break;
		}
		
		return true;
	}
	
	
	
	
	
	
	//		2	탐험 게임모드 (섬멸)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 섬멸(KILLEMALL) 게임모드
	bool check2_8(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial(nowTutorialId);
			}
			return false;
			break;
		case 2:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				++subStep;
				closeTutorial();
				uiTutorial.hide();
				waitStartBattle = false;
			}
			return false;
			break;
		}
		
		return true;
	}
	
	
	
	//			9	탐험 게임모드 (아이템획득)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 아이템획득(GETITEM) 게임모드
	bool check9(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				//uiTutorial.hide();
				StartCoroutine( uiVisibleAnimation(2,2,uiManager.uiPlay.lbRoundLeftNum.gameObject));
				openDialog(200,300,false,true);
				// 남은 아이템 갯수가 깜빡거림.
			}
			return false;
			break;
		case 2:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial(nowTutorialId);
			}
			return false;
			break;
		case 3:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				waitStartBattle = false;

				closeTutorial();
			}
			return false;
			break;
		}
		return true;
	}
	
	//			10	추격몬스터	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 추격몬스터 존재
	bool check10(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,300,false,true);
			}
			return false;
			break;
		case 2:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial(nowTutorialId);
			}
			return false;
			break;
		case 3:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				waitStartBattle = false;
				closeTutorial();
			}
			return false;
			break;
		}
		return true;
	}
	
	

	bool check13(GameObject go)
	{
		switch(subStep)
		{
			/*
		case 1:
			//2	월드맵			[뒤로가기]	O					

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				// 뒤로가기에 화살표.
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(34,542,false);
			}
			return false;
			break;
		case 2:
			//3	메인 페이지			[미션]	O	하단에, 미션 버튼 보이도록				미션 버튼 임시활성

			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				// 미션 버튼에 화살표 // 미션 버튼 임시 활성.
				uiManager.uiMenu.uiLobby.btnMission.gameObject.SetActive(true);

				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1070,345+25,false);
				return true;
			}
			return false;
			break;
		case 3:
			//4	미션 페이지					하단에, 미션 항목이 보이도록		"미션페이지에 들어오셨습니다. [5CD1E5]첫 스테이지 클리어 미션[-]을 완료하셨네요."		
			if(go == uiManager.uiMenu.uiLobby.btnMission.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,100,true,true);
				return true;
			}
			return false;
			break;
			*/

		case 1:
			//2		완료		[완료미션 보상]				"멋지게 미션을 완료하셨으니 보상을 받으셔야죠? [5CD1E5]보상 받기[-]버튼을 눌러보세요."		미션 버튼 영구활성
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,100,false,false);


				UIMissionListSlotPanel[] slots = uiManager.uiMenu.uiMission.list.listGrid.gameObject.GetComponentsInChildren<UIMissionListSlotPanel>();
				if(slots != null)
				{
					foreach(UIMissionListSlotPanel slot in slots)
					{
						if(slot.btnReceive.gameObject.name == "M_EP_001_01")
						{
							Vector3 lpos = slot.transform.localPosition;
							lpos.x += 872 - 149;
							lpos.y -= 34;
							setArrowAndDimToInventorySlot(lpos, true);
							break;
						}
					}
				}
			}
			return false;
			break;

		case 2:
			if(go.name == "M_EP_001_01")
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial("T13");
				return true;
			}
			return false;
			break;

		case 3:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendRewardTutorial("T13");
			}
			return false;
			break;
			
		case 4:
			
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				uiTutorial.hide();
				isTutorialMode = false;
				closeTutorial();
			}
			return false;
			break;
		}
		
		return true;
	}

	
	
	
	
	
	

	
	bool check15(GameObject go)
	{
		switch(subStep)
		{
			/*
		case 1: // 라운드 다음 누르면...
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1064,500,false);
			}
			return false;
			break;
		case 2: // 정보창 닫으면...
			
			if(go == uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
		case 3: // 백버튼 누르면...
			
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;

				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1060,350);
				uiManager.uiMenu.uiLobby.btnFriend.gameObject.SetActive(true);
				return true;
			}
			
			return false;
			break;
			
		case 4:
			// 
			if(go == uiManager.uiMenu.uiLobby.btnFriend.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);
				return true;
			}
			return false;
			break;

		case 5:


			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;
			*/
		case 1: //7번 입력.
			//친구와 [86E57F]'우정포인트'[-]를 주고 받을 수 있답니다. 이 우정포인트는 [86E57F]'우정보너스'[-]를 받는 데 필요해요.
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;
			
		case 2:
			//자아, 우정보너스를 받아볼까요? 아직 우정포인트가 없으시니 제가 도와드릴게요.
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(913,205-35);
			}
			
			return false;
			break;
			
		case 3:
			
			if(go == uiManager.uiMenu.uiFriend.btnReceiveBonus.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				return true;
			}
			
			return false;
			break;
			
		case 4:
			
			return false;
			break;
			
			
		case 5:
			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;

		case 6:
			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;

		case 7:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				
				EpiServer.instance.sendRewardTutorial("T15");
				
				isTutorialMode = false;
				nowTutorialId = "";
				return false;
			}
			
			return false;
			break;

		case 8:
			
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				uiTutorial.hide();
				closeTutorial();
				++subStep;
			}
			return false;
			break;
		}

		return true;
	}
	



	/*
	bool check17(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			//2				[다음]			O	도전모드를 열심히 플레이하시면 소환룬, 스킬룬, 히어로장비 등 아주 다양한 보상을 받으실 수 있습니다.

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;
		case 2:
			//3				[도전]	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(790,94);
				uiManager.uiMenu.uiWorldMap.btnStartChallenge.gameObject.SetActive(true);
			}
			return false;
			break;

		case 3:
			//4	도전모드창			[다음]			O	도전모드를 플레이하시면 획득한 점수 또는 시간에 따라서 랭크(★)를 부여받게 되는데요.		
			if(go == uiManager.uiMenu.uiWorldMap.btnStartChallenge.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);
				return true;
			}
			return false;
			break;
			
		case 4:
			///5				[다음]			O	별 셋(★★★) 클리어를 하시면 보상을 세 개까지 획득하실 수 있습니다.		
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);

			}
			return false;
			break;

		case 5:
			///6
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);
				
			}
			return false;
			break;
			
		case 6:
			//			[다음]			O	필요한 아이템이 보상아이템으로 지정되었다면 기회를 놓치지 마세요!	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);
			}
			
			return false;
			break;
			
		case 7:
			//8				[도전하기]	O			

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(566,115);
				uiManager.uiMenu.uiWorldMap.btnStartChallenge.gameObject.SetActive(true);
			}
			
			return false;
			break;
			
		case 8:
			if(go == uiManager.popupChallenge.btnStart.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial("T17");
				return false;
			}
			
			return false;
			break;

		case 9:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				isTutorialMode = false;
				nowTutorialId = "";
//				GameManager.me.uiManager.popupChallenge.startChallenge();
			}
			return false;
			break;

		}
		
		
		
		return true;
	}

	
	
	
	
	
	//			18	도전 게임모드 설명 (무한질주)	도전모드 & 전투 스타트 직전 & 무한질주 게임모드
	bool check18_20(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,300,false,true);
			}
			return false;
			break;
		case 2:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial(nowTutorialId);
			}
			return false;
			break;
		case 3:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				waitStartBattle = false;
				isTutorialMode = false;
				nowTutorialId = "";
			}
			return false;
			break;
		}
		return true;
	}
	*/


	bool check24(GameObject go)
	{
		
		// 처음부터....
		switch(subStep)
		{
			/*
		case 1:
			// 2 			[다음]			O	대회 결과에 따라 상위등급으로 올라갈수록, 혜택도 다양해져요.	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;
		case 2:
			// 3				[챔피언십]	O 에 화살표.
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(565,100);
				return true;
			}
			
			return false;
			break;

		case 3:
			//4	챔피언십 페이지			[다음]			O	"여기에서 챔피언십 대회가 열려요. 챔피언십 대회에서는 [5CD1E5]최대 15명이 그룹을 지어 1:1로 경기를 플레이합니다. 승리하면 승점을 얻어요."		

			if(go == uiManager.uiMenu.uiWorldMap.btnChampionship.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog (200,400,true,true);
				return true;
			}
			return false;
			break;
			*/
		case 1:
			//5				[다음]			O	대회는 일주일동안 진행이 되며, 매주 월요일에 새롭게 배치되어요.	O		
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog (200,490,false,false);
				uiTutorial.showBigSizeCharacter();
			}
			
			return false;
			break;
			
		case 2:
			//6				[다음]			O	전주 순위에 따라 상위 등급으로 올라가거나, 하위등급으로 떨어질 수도 있으니 주의해 주세요.	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog (200,490,false,false);
				uiTutorial.showBigSizeCharacter();
			}
			
			return false;
			break;

		case 3:
			//7				[다음]			O	챔피언십 등급이 높을수록 특별한 혜택을 드리고 있어요! 자세한 설명은 물음표 버튼을 눌러보세요. 자, 용사님의 힘을 보여주실 시간이에요!	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog (200,490,false,false);
				uiTutorial.showBigSizeCharacter();
			}
			
			return false;
			break;
			
		case 4:
			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteRewardTutorial("T24");
			}
			
			return false;
			break;
			
		case 5:
			
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				uiTutorial.hide();
				closeTutorial();
				++subStep;
			}
			return false;
			break;
		}
		
		return true;
	}

	
	
	/*
	 * [구버전]
	bool check24(GameObject go)
	{
		
		// 처음부터....
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;
		case 2:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				//월드맵 뒤로가기 화살표.
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
		case 3:
			
			// 월드맵 뒤로가기.
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				++subStep;
				
				uiTutorial.hide();
				uiManager.uiMenu.uiWorldMap.btnChampionship.gameObject.SetActive(true);
				uiTutorial.setArrowAndDim(827,106);
				return true;
			}
			
			return false;
			break;
			
		case 5:
			
			if(go == uiManager.uiMenu.uiWorldMap.btnChampionship.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog (200,400,true,true);
				return true;
			}
			return false;
			break;
			
		case 6:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog (200,490,false,false);
//				uiTutorial.showVerticalDim(new Vector3(604,0,0));
			}
			
			return false;
			break;
			
		case 7:
			
			if(go.name == "0")
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(569,224);
				return true;
			}
			
			return false;
			break;
			
		case 8:
			
			if(go == uiManager.popupChampionshipAttack.btnAttack.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial("T24");
			}
			
			return false;
			break;
			
		case 9:
			
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				uiTutorial.hide();
				isTutorialMode = false;
				nowTutorialId = "";
				++subStep;

				GameManager.me.uiManager.popupChampionshipAttack.onClickAttack(null);
			}
			return false;
			break;
		}

		return true;
	}
	
	*/





	bool check37(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 카일리 선택 버튼.

				openDialog(200,400,false,true,true);

				setArrowAndDim(102,430);
				uiTutorial.hideArrowWithoutCircle();

			}
			return false;
			break;
			
		case 2:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,false,true,true);

				setArrowAndDim(102,430);
				uiTutorial.hideArrowWithoutCircle();

			}
			return false;
			break;
			
		case 3:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteRewardTutorial("T37");
			}
			return false;
			break;
			
		case 4:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				//GameManager.me.uiManager.uiMenu.uiHero.btnSelectCharacter[UIHero.KILEY].isEnabled = true;
				uiTutorial.hide();
				closeTutorial();
				++subStep;
			}

			return false;
			break;
		}
		
		
		return true;
	}








	bool check44(GameObject go)
	{
		switch(subStep)
		{
		case 1: // 라운드 다음 누르면...
			////2				라운드 정보창 [닫기]	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1064,500,false);
			}
			return false;
			break;
		case 2: // 정보창 닫으면...
			//3	월드맵			[뒤로가기]	O			
			if(go == uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
		case 3: // 백버튼 누르면...
			//4	메인 페이지			[소환룬]		소환룬 버튼 가리지 않는 위치		왼쪽에 '소환룬' 버튼을 누르세요.		소환룬 버튼 임시활성
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,false,false);
				uiTutorial.setArrowAndDim(72,352);
				uiManager.uiMenu.uiLobby.btnSummon.gameObject.SetActive( true );
				return true;
			}
			
			return false;
			break;
			
		case 4:
			//	5	소환룬 페이지			[다음]			O	이 곳이 [5CD1E5]소환룬을 장착하고 관리[-]할 수 있는 페이지입니다. 새로운 소환수를 만나볼까요?		
			if(go == uiManager.uiMenu.uiLobby.btnSummon.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				return true;
			}
			return false;
			break;
			
		case 5:
			// 6				[상점]	O					
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1019,142);
			}
			
			return false;
			break;
			
		case 6:
			//7	상점			[다음] (200,400)			O	이곳에서 [5CD1E5]소환룬을 구입[-]할 수 있어요.
			if(go == uiManager.uiMenu.uiSummon.btnOpenShop.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				return true;
			}
			
			return false;
			break;
			
		case 7:
			//			8				[다음] (200,400)			O	최대 S등급까지 나올 수 있는 [5CD1E5]프리미엄 소환 룬[-]을 구입해 볼게요.
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;
			
		case 8:
//			9				[프리미엄 소환룬 구입] (358,410)	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(358,410);

			}
			
			return false;
			break;
			
		case 9:
			// endprocess에서 처리!
			if(go.name == "P")
			{
				++subStep;
				uiTutorial.hide();
				return true;
			}
			return false;
			break;
		case 10:
			return false;
			break;			

		case 11:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				//상점			[뒤로가기]	O			소환룬 페이지로 돌아갑니다.
				openDialog(200,400,false,false);
				uiTutorial.setArrowAndDim(34,542,false);
			}
			return false;
			break;

		case 12:
			//소환룬 페이지			[다음]			O	소환룬의 레어등급은 [86E57F]'D<C<B<A<S'[-] 가 있답니다. 방금 구입한 C등급 소환룬을 [5CD1E5]'강화'[-]해 볼게요.
			if(go == GameManager.me.uiManager.popupShop.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
				return true;
			}

			return false;
			break;

		case 13:
			//14 소환룬 페이지			[다음]			O	탐험모드에서 경험치를 얻어도 레벨업을 할 수 있지만, 재료룬을 사용한 [5CD1E5]'강화'[-]로 더 많은 경험치를 얻을 수 있어요.	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}

			return false;
			break;

		case 14:

			//15				소환룬 선택 (UN21900101)	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				foreach(UISummonInvenSlot slot in uiManager.uiMenu.uiSummon.invenList.listGrid.GetComponentsInChildren<UISummonInvenSlot>())
				{
					if(slot.data != null)
					{
						if(slot.data.serverId.Contains("UN21900101"))// && slot.isChecked == false)
						{
							Vector3 v = slot.transform.localPosition;
							v.y = slot.transform.parent.localPosition.y;
							setArrowAndDimToInventorySlot(v, true);
							slot.name = "UN21900101";
							break;
						}
					}
				}
			}
			return false;
			break;


		case 15:
			// 16	소환룬 상세 팝업			강화 버튼	O			
			if(go.name == "UN21900101")
			{
				++subStep;
				uiTutorial.hide();

				uiTutorial.setArrowAndDim(757,123);
				return true;
			}
			
			return false;
			break;

		case 16:
			//17	강화 UI						O	강화를 하려면 [86E57F]재료 소환룬[-]이 필요해요. 가방에 있는 D급 소환룬을 강화 재료로 사용해 볼까요?
			if(go == GameManager.me.uiManager.popupSummonDetail.btnReinfoce.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				return true;
			}
			
			return false;
			break;

		case 17:
			//18				재료 선택 (UN10100101)	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				foreach(UISummonInvenSlot slot in uiManager.uiMenu.uiSummon.invenList.listGrid.GetComponentsInChildren<UISummonInvenSlot>())
				{
					if(slot.data != null)
					{
						if(slot.data.serverId.Contains("UN10100101"))// && slot.isChecked == false)
						{
							Vector3 v = slot.transform.localPosition;
							v.y = slot.transform.parent.localPosition.y;
							setArrowAndDimToInventorySlot(v, true);
							slot.name = "UN10100101";
							break;
						}
					}
				}
			}

			return false;
			break;
			
			
		case 18:
			//19				[강화]	O			
			if(go.name == "UN10100101")
			{
				++subStep;
				uiTutorial.hide();				
				uiTutorial.setArrowAndDim(935,168);
				return true;
			}
			
			return false;
			break;

		case 19:
			if(go == GameManager.me.uiManager.uiMenu.uiSummon.reinforcePanel.btnReinforce.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				return true;
			}
			return false;
			break;

		case 20:
			//21							O	[86E57F]합성을 하면 무조건 상위 레어등급의 룬[-]을 얻을 수 있답니다. 자아, 소환룬을 장착해봐요.
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);
			}
			return false;
			break;

		case 21:
			//22				[장착]	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				TutorialManager.uiTutorial.setArrowAndDim(898,129);
			}

			return false;
		case 22:
				
			if(go == GameManager.me.uiManager.popupSummonDetail.btnPutOn.gameObject)
			{
	
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(878,180-40);
				return true;
			}
			return false;
			break;

		case 23:
			if(go == GameManager.me.uiManager.uiMenu.uiSummon.nowSlots[4].btn.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true,true);
				uiTutorial.showBigSizeCharacter();
				return true;
			}
			return false;
		case 24:
			//21				[강화]	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();				
				EpiServer.instance.sendCompleteRewardTutorial("T44");
			}
			
			return false;
			break;


		case 25:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				TutorialManager.uiTutorial.tutorialEndCircleEffect.show();
				++subStep;
				uiTutorial.hide();
				waitStartBattle = false;
				closeTutorial();
			}
			return false;
			break;


		}
		
		
		return true;
		
	}








	bool check45(GameObject go)
	{
		switch(subStep)
		{
		case 1: // 정보창 닫으면...
			//2	월드맵			[뒤로가기]	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
		case 2: // 백버튼 누르면...
			//3	메인 페이지			[스킬룬]		스킬룬 버튼 가리지 않는 위치		왼쪽의 '스킬룬' 버튼을 눌러주세요.		스킬룬 버튼 임시활성
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,false,false);
				uiTutorial.setArrowAndDim(72,213);
				uiManager.uiMenu.uiLobby.btnSkill.gameObject.SetActive(true);
				return true;
			}
			
			return false;
			break;
			
		case 3:
			//	4	스킬룬 페이지			[다음]			O	이곳이 [5CD1E5]스킬룬을 장착하고 관리[-]할 수 있는 페이지랍니다. 조금 전에 보상으로 받은 C등급 스킬을 장착해 봐요.
			if(go == uiManager.uiMenu.uiLobby.btnSkill.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				return true;
			}
			return false;
			break;
			
		case 4:
			// 			스킬룬 선택 (SK_2110_1)	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				foreach(UISkillInvenSlot slot in uiManager.uiMenu.uiSkill.invenList.listGrid.GetComponentsInChildren<UISkillInvenSlot>())
				{
					if(slot.data != null)
					{
						if(slot.data.serverId.Contains("SK_2110_1"))// && slot.isChecked == false)
						{
							Vector3 v = slot.transform.localPosition;
							v.y = slot.transform.parent.localPosition.y;
							setArrowAndDimToInventorySlot(v, true);
							slot.name = "OK";
							break;
						}
					}
				}
			}
			
			return false;
			break;
			
		case 5:
			//6	스킬룬 상세 팝업			[장착]	O
			if(go.name == "OK")
			{
				++subStep;
				uiTutorial.hide();
				
				// 장착 버튼에 화살표...
				uiTutorial.setArrowAndDim(901,129);
				return true;
			}
			
			return false;
			break;
			
		case 6:
			// 7	장착UI			3번째 스킬룬 슬롯	O
			if(go == GameManager.me.uiManager.popupSkillPreview.btnPutOn.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(772,161);
				return true;
			}
			
			return false;
			break;
			
		case 7:
			// 8	스킬룬 페이지						O	굉장해요! 그리고 하나 더, 스킬룬도 [86E57F]강화[-]가 가능하다는 사실! 한번 강화해 볼까요?	O	
			if(go == GameManager.me.uiManager.uiMenu.uiSkill.nowSlots[2].btn.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
				return true;
			}
			
			return false;
			break;
			
		case 8:		
			//9	스킬룬 페이지			3번째 장착 스킬룬 선택	O			

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(772,161);
				return true;
			}
			
			return false;
			break;
			
		case 9:			
			//10	스킬룬 상세 팝업			[강화]	O			
			if(go == GameManager.me.uiManager.uiMenu.uiSkill.nowSlots[2].btn.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(755,126);
				return true;
			}
			
			return false;
			break;

		case 10:			
			// 11						적당히 하단에 표시	O	재료로 사용되는 룬은 레어등급이 높을수록 더 많은 경험치를 얻을 수 있으며, 최대 5개 재료 룬을 한번에 사용할 수도 있답니다.		
			if(go == GameManager.me.uiManager.popupSkillPreview.btnReinfoce.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				return true;
			}
			
			return false;
			break;

		case 11:
			//12							O	또! 소환룬과 마찬가지로 [5CD1E5]20레벨 스킬룬이 2개[-] 있으면 [86E57F]합성[-]도 할 수 있어요. 자,  D등급 스킬룬을 재료로 선택해 보세요.		
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;

		case 12:
			//13	강화 UI			재료 스킬룬 선택 (SK_1105_1)	O					
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				foreach(UISkillInvenSlot slot in uiManager.uiMenu.uiSkill.invenList.listGrid.GetComponentsInChildren<UISkillInvenSlot>())
				{
					if(slot.data != null)
					{
						if(slot.data.serverId.Contains("SK_1105_1"))// && slot.isChecked == false)
						{
							Vector3 v = slot.transform.localPosition;
							v.y = slot.transform.parent.localPosition.y;
							setArrowAndDimToInventorySlot(v, true);
							slot.name = "SK_1105_1";
							break;
						}
					}
				}


			}
			
			return false;
			break;

		case 13:
			// 14		완료		[강화]	O					"스킬룬 버튼 영구활성
			if(go.name == "SK_1105_1")
			{
				++subStep;
				uiTutorial.hide();
				
				// 장착 버튼에 화살표...
				uiTutorial.setArrowAndDim(930,160);
				return true;
			}
			
			return false;
			break;

		case 14:
			//15							O	[86E57F]스킬 충전[-], 알고 계세요? [5CD1E5]스킬 버튼을 꾸욱[-] 누르면 강력한 스킬효과가 발동해요!		
			if(go == GameManager.me.uiManager.uiMenu.uiSkill.reinforcePanel.btnReinforce.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				return true;
			}
			
			return false;
			break;
		case 15:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				
				EpiServer.instance.sendCompleteRewardTutorial("T45");
				
				isTutorialMode = false;
				nowTutorialId = "";
				return false;
			}
			
			return false;
			break;
			
		case 16:
			
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				uiTutorial.hide();
				closeTutorial();
				++subStep;
			}
			return false;
			break;

		}
		
		
		return true;
		
	}





	bool check46(GameObject go)
	{
		switch(subStep)
		{
		case 1: // 정보창 닫으면...
			//2 월드맵			[뒤로가기]	O			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
			
		case 2: // 백버튼 누르면...
			// 3
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,false,false);
				uiTutorial.setArrowAndDim(72,357,false);
				uiManager.uiMenu.uiLobby.btnHero.gameObject.SetActive( true );
				return true;
			}
			
			return false;
			break;
			
		case 3:
			//	4	히어로 페이지			[다음]			O	이 곳은 [5CD1E5]히어로 페이지[-]입니다. 제가 드린 새로운 장비를 장착해 보겠습니다.
			if(go == uiManager.uiMenu.uiLobby.btnHero.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				return true;
			}
			return false;
			break;
			
		case 4:
			//5 인벤 장비 선택 (LEO_BD1_1_10) 이 시점에 장착중인 BD ID 저장	O			
			
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				foreach(UIHeroInventorySlot slot in uiManager.uiMenu.uiHero.invenList.listGrid.GetComponentsInChildren<UIHeroInventorySlot>())
				{
					if(slot.data != null)
					{
						if(slot.data.serverId.Contains("LEO_RD2_11_1"))
						{
							Vector3 v = slot.transform.localPosition;
							v.y = slot.transform.parent.localPosition.y;
							setArrowAndDimToInventorySlot(v, true);
							slot.name = "OK";
							break;
						}
					}
				}
			}
			
			return false;
			break;
			
		case 5:
			// 6	장비 상세 팝업			[장착] (795,179)	O			
			if(go.name == "OK")
			{
				++subStep;
				uiTutorial.hide();
				
				// 장착 버튼에 화살표...
				uiTutorial.setArrowAndDim(899,129);
				return true;
			}
			
			return false;
			break;
			
		case 6:
			// 히어로 페이지						O	히어로 장비도 룬과 동일한 방식으로 [86E57F]강화[-]가 가능합니다. 교체된 장비를 재료로 사용해 볼까요?
			if(go == GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.btnWear.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				return true;
			}
			
			return false;
			break;

		case 7:

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				uiTutorial.setArrowAndDim(832,147);
			}

			break;

		case 8:
			//9	장비 상세 팝업			[강화] (868,170)	O			
			// 18				[강화] (789,131)	O			
			if(go ==  GameManager.me.uiManager.uiMenu.uiHero.slotCategory[3].btn.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				
				// 장착 버튼에 화살표...
				uiTutorial.setArrowAndDim(754,129);
				return true;
			}
			
			return false;
			break;

		case 9:

			if(go ==  GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.btnReinfoce.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				
				
				foreach(UIHeroInventorySlot slot in uiManager.uiMenu.uiHero.invenList.listGrid.GetComponentsInChildren<UIHeroInventorySlot>())
				{
					if(slot.data != null)
					{
						if(slot.data.serverId.Contains("LEO_RD1_11"))
//						if(slot.data.serverId.Contains("LEO_HD5_11_10_77")) //LEO_HD5_11_10_77
						{
							Vector3 v = slot.transform.localPosition;
							v.y = slot.transform.parent.localPosition.y;
							setArrowAndDimToInventorySlot(v, true);
							slot.name = "FUCK";
							break;
						}
					}
				}

				return true;
			}
			
			return false;
			break;

		case 10:
			// 6	장비 상세 팝업			[장착] (795,179)	O			
			if(go.name == "FUCK")
			{
				++subStep;
				uiTutorial.hide();
				
				// 장착 버튼에 화살표...
				uiTutorial.setArrowAndDim(931, 163);
				return true;
			}
			
			return false;
			break;

		case 11:
			// 18				[강화] (789,131)	O			
			if(go == GameManager.me.uiManager.uiMenu.uiHero.reinforcePanel.btnReinforce.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				return true;
			}
			
			return false;
			break;

		case 12:
			if(go == GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				
				EpiServer.instance.sendCompleteTutorial("T46");
				return true;
			}
			
			return false;
			break;

		case 13:

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}

			return false;

			break;

		case 14:
			//15							O	히어로 장비와 히어로 레벨에 소홀하시면 절대 안됩니다!	O

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}

			return false;
			break;

		case 15:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				
				EpiServer.instance.sendRewardTutorial("T46");
				
				isTutorialMode = false;
				nowTutorialId = "";
			}
			
			return false;
			break;
			
		case 16:
			
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				TutorialManager.uiTutorial.tutorialEndCircleEffect.show();
				uiTutorial.hide();
				closeTutorial();
				++subStep;
			}
			return false;
			break;
			
		}
		
		
		return true;
	}




	bool check47(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;
		case 2:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;
		case 3:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				subStep = 6;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;
		case 6:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;

		case 7:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
				uiTutorial.showBigSizeCharacter();
			}
			return false;
			break;

		case 8:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteRewardTutorial("T47");
			}
			return false;
			break;
			
		case 9:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				uiTutorial.hide();
				closeTutorial();
				++subStep;
			}
			
			return false;
			break;
		}
		
		
		return true;
	}





	bool check48(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			//2				라운드 정보창 [닫기]	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 라운드 정보창 닫기에 화살표.
				uiTutorial.setArrowAndDim(1064,500,false);
			}
			return false;
			break;
			//	3	월드맵 페이지			[뒤로가기]	O	
		case 2:
			
			if(go == uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 소환룬 버튼에 화살표.
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
		case 3:
			//4	메인 페이지			[상점]	O	
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1051,163);
				uiManager.uiMenu.uiLobby.btnShop.gameObject.SetActive(true);
				return true;
			}
			
			return false;
			break;
			
			
		case 4:
			if(go == uiManager.uiMenu.uiLobby.btnShop.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(355,390);
				GameManager.me.uiManager.popupShop.showItemShop();
				return false;
			}
			return false;
			break;
			
			
		case 5:
			if(go.name == "P")
			{
				++subStep;
				uiTutorial.hide();
				return true;
			}
			
			return false;
			break;

		case 6:
			return false;
			break;

		case 8:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial("T48");
				return true;
			}
			
			return false;
			break;
			
//		case 9:
//			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
//			{
//				uiTutorial.hide();
//				isTutorialMode = false;
//				nowTutorialId = "";
//				++subStep;
//			}
//			return false;
//			break;
		}
		return true;
	}




	bool check49(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			//2				라운드 정보창 [닫기]	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 라운드 정보창 닫기에 화살표.
				uiTutorial.setArrowAndDim(1064,500,false);
			}
			return false;
			break;
			//	3	월드맵 페이지			[뒤로가기]	O	
		case 2:
			
			if(go == uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 소환룬 버튼에 화살표.
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
		case 3:
			//4	메인 페이지			[상점]	O	
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1051,163);
				uiManager.uiMenu.uiLobby.btnShop.gameObject.SetActive(true);
				return true;
			}
			
			return false;
			break;
			
			
		case 4:
			if(go == uiManager.uiMenu.uiLobby.btnShop.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(568,390);
				GameManager.me.uiManager.popupShop.showItemShop();
				return false;
			}
			return false;
			break;
			
			
		case 5:
			if(go.name == "P")
			{
				++subStep;
				uiTutorial.hide();
				return true;
			}
			
			return false;
			break;
			
		case 6:
			return false;
			break;
			
		case 8:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial("T49");
				return true;
			}
			
			return false;
			break;
			
//		case 9:
//			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
//			{
//				uiTutorial.hide();
//				isTutorialMode = false;
//				nowTutorialId = "";
//				++subStep;
//			}
//			return false;
//			break;
		}
		return true;
	}





	bool check50(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			//2				라운드 정보창 [닫기]	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 라운드 정보창 닫기에 화살표.
				uiTutorial.setArrowAndDim(1064,500,false);
			}
			return false;
			break;
			//	3	월드맵 페이지			[뒤로가기]	O	
		case 2:
			
			if(go == uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 소환룬 버튼에 화살표.
				uiTutorial.setArrowAndDim(34,542,false);
				return true;
				
			}
			return false;
			break;
			
		case 3:
			//4	메인 페이지			[상점]	O	
			if(go == uiManager.uiMenu.uiWorldMap.btnBack.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(1051,163);
				uiManager.uiMenu.uiLobby.btnShop.gameObject.SetActive(true);
				return true;
			}
			
			return false;
			break;
			
			
		case 4:
			if(go == uiManager.uiMenu.uiLobby.btnShop.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				uiTutorial.setArrowAndDim(780,390);
				GameManager.me.uiManager.popupShop.showItemShop();
				return false;
			}
			return false;
			break;
			
			
		case 5:
			if(go.name == "P")
			{
				++subStep;
				uiTutorial.hide();
				return true;
			}
			
			return false;
			break;
			
		case 6:
			return false;
			break;
			
		case 8:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteTutorial("T50");
				return true;
			}
			
			return false;
			break;
			
//		case 9:
//			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
//			{
//				uiTutorial.hide();
//				isTutorialMode = false;
//				nowTutorialId = "";
//				++subStep;
//			}
//			return false;
//			break;
		}
		return true;
	}




	bool check51(GameObject go)
	{
		switch(subStep)
		{
			/*
		case 1:
			//2				[이계 던전]	O			 '이계 던전'[-] 화면으로 이동해주세요.	O	
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				// 던전 버튼에 화살표.
				uiTutorial.setArrowAndDim(346,96);
			}
			return false;
			break;
			*/
		case 1:
			//	2	이계 던전 페이지			[다음]			O	[86E57F]이계 던전[-]에서는 주어진 시간 동안 25곳에서 나타나는 적을 쓰러뜨리며, 멀리 이동해야 해요.					
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();

				uiTutorial.hide();
				openDialog(200,400,true,true);

			}
			return false;
			break;
			
		case 2:
			//3				[다음]			O	이계 던전에 참여한 모든 용사님들은 물리친 적의 수, 이동 거리, 클리어 단계에 따라 점수를 부여받아요. 점수에 따라 받는 보상도 달라집니다.		
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;
			
			
		case 3:
			//4				[다음]			O	지급되는 보상에는 [5CD1E5][참여 보상][-]과 [5CD1E5][주간 보상][-] 두 가지가 있어요.		
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			return false;
			break;
			
			
		case 4:
			// 5				[다음]			O	[5CD1E5][주간 보상][-]은 일주일에 한 번만 받을 수 있답니다. 기록된 점수는 매주 월요일마다 초기화된다는 사실도 기억해 주세요.		

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}
			
			return false;
			break;
			
		case 5:
			//6			[다음]			O	이제 이계 던전에 도전할 준비가 되셨나요?		

			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				openDialog(200,400,true,true);
			}

			return false;
			break;
			
		case 6:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				EpiServer.instance.sendCompleteRewardTutorial("T51");
				return true;
			}
			
			return false;
			break;

		case 7:
			if(go == uiTutorial.rewardPopup.btnClose.gameObject)
			{
				uiTutorial.hide();
				closeTutorial();
				++subStep;
			}
			
			return false;
			break;

		}
		return true;
	}







	bool check52(GameObject go)
	{
		switch(subStep)
		{
		case 1:
			if(go == uiTutorial.btnDialog.gameObject || go == uiTutorial.btnBigDialog.gameObject || go == uiTutorial.btnOKDialog.gameObject)
			{
				++subStep;
				uiTutorial.hide();
				GameManager.me.uiManager.uiMenu.uiSummon.tutorialComposePanel.gameObject.SetActive(false);
				EpiServer.instance.sendCompleteTutorial(nowTutorialId);
			}
			return false;
			break;
		case 2:
			return false;
			break;
		}
		
		return true;
	}








}

