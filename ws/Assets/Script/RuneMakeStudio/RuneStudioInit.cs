using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RuneStudioInit : MonoBehaviour 
{
	public GameObject root;
	public GameObject menuRoot;
	public c_tk2dGameCamera setupCam;

	public RuneStudioMain studioMain;

	public PlayMakerFSM[] unitMakeFSM;
	public PlayMakerFSM[] unitPreMakeFSM;
	public PlayMakerFSM[] reinforceFSM;
	public PlayMakerFSM[] composeFSM;

	public PlayMakerFSM[] evolveSSFSM;
	public PlayMakerFSM[] evolveSFSM;
	public PlayMakerFSM[] evolveAFSM;
	public PlayMakerFSM[] evolveBFSM;
	public PlayMakerFSM[] evolveCFSM;

	public PlayMakerFSM[] transcendFSM;

	public Transform[] reinforceTfs;
	public Transform[] unitMakeTfs;
	public Transform[] unitPreMakeTfs;
	public Transform[] composeTfs;
	public Transform[] transcendTfs;

	public Transform[] evolveSSTfs;
	public Transform[] evolveSTfs;
	public Transform[] evolveATfs;
	public Transform[] evolveBTfs;
	public Transform[] evolveCTfs;

	public ParticleSystem[] reinforcePs;
	public ParticleSystem[] unitMakePs;
	public ParticleSystem[] unitPreMakePs;
	public ParticleSystem[] composePs;
	public ParticleSystem[] transendPs;

	public ParticleSystem[] evolveSSPs;
	public ParticleSystem[] evolveSPs;
	public ParticleSystem[] evolveAPs;
	public ParticleSystem[] evolveBPs;
	public ParticleSystem[] evolveCPs;




	public SimpleRotater detailPopupSimpleRotater;


	public void clear()
	{
		if(studioMain == null) studioMain = gameObject.GetComponent<RuneStudioMain>();
		studioMain.resetter = this;

		if(root == null) root = gameObject.transform.FindChild("StudioRoot").gameObject;

		if(studioMain.cardStudio256 == null || studioMain.cardStudio256.Length < 6) studioMain.cardStudio256 = new GameObject[6];
		if(studioMain.cardStudio512 == null || studioMain.cardStudio512.Length < 6) studioMain.cardStudio512 = new GameObject[6];
		if(studioMain.skillIconRareBg == null || studioMain.skillIconRareBg.Length < 6) studioMain.skillIconRareBg = new GameObject[6];

		if(studioMain.composeController == null || studioMain.composeController.Length < 4) studioMain.composeController = new PlayMakerFSM[4];
		if(studioMain.evolveController == null || studioMain.evolveController.Length < 5) studioMain.evolveController = new PlayMakerFSM[5];

		if(studioMain.rootEvolve == null || studioMain.rootEvolve.Length < 5) studioMain.rootEvolve = new GameObject[5];

		if(studioMain.evolutionCardFrame == null || studioMain.evolutionCardFrame.Length < 5) studioMain.evolutionCardFrame = new UICardFrame[5];
		if(studioMain.evolutionRotationAni == null || studioMain.evolutionRotationAni.Length < 5) studioMain.evolutionRotationAni = new Animation[5];
		if(studioMain.evolveMovablePanelParent == null || studioMain.evolveMovablePanelParent.Length < 5) studioMain.evolveMovablePanelParent = new Transform[5];

		UICamera[] uic = root.GetComponentsInChildren<UICamera>(true);

		for(int i = uic.Length - 1; i >= 0; --i)
		{
			DestroyImmediate(uic[i]);
		}

		Transform[] trees = root.GetComponentsInChildren<Transform>(true);

		for(int i = trees.Length - 1; i >= 0; --i)
		{
			Transform t = trees[i];
			if(t == null || t == root.transform) continue;
			switch(t.name)
			{
			case "01 CameraSetup":
			case "02 CameraContainer":
			case "GameCameraContainer":
			case "SkillMake":
			case "BtnSkillMake_special":
			case "summon_infobg":
				GameObject.DestroyImmediate(t.gameObject,true);
				break;

			case "skillIcon_Card_d_studiobg":
				studioMain.skillIconRareBg[0] = t.gameObject;
				break;

			case "skillIcon_Card_c_studiobg":
				studioMain.skillIconRareBg[1] = t.gameObject;
				break;

			case "skillIcon_Card_b_studiobg":
				studioMain.skillIconRareBg[2] = t.gameObject;
				break;

			case "skillIcon_Card_a_studiobg":
				studioMain.skillIconRareBg[3] = t.gameObject;
				break;

			case "skillIcon_Card_s_studiobg":
				studioMain.skillIconRareBg[4] = t.gameObject;
				break;

			case "skillIcon_Card_ss_studiobg":
				studioMain.skillIconRareBg[5] = t.gameObject;
				break;

			case "spSkillIconCHECK":
				studioMain.spSkillIcon = t.gameObject.GetComponent<UISprite>();
				break;

			case "spCardFrameCHECK":
				studioMain.spSkillIconFrame = t.gameObject.GetComponent<UISprite>();
				break;



			case "ReinforceSlots":
				if(t.parent.name == "Studio")
				{
					studioMain.reinforceSlotContainer = t.gameObject;

					UIChallengeItemSlot[] rt = studioMain.reinforceSlotContainer.GetComponentsInChildren<UIChallengeItemSlot>(true);

					foreach(UIChallengeItemSlot rtt in rt)
					{
						switch(rtt.gameObject.name)
						{
						case "itemslot0":
							studioMain.reinforceSlots[0] = rtt;
							break;
						case "itemslot1":
							studioMain.reinforceSlots[1] = rtt;
							break;
						case "itemslot2":
							studioMain.reinforceSlots[2] = rtt;
							break;
						case "itemslot3":
							studioMain.reinforceSlots[3] = rtt;
							break;
						case "itemslot4":
							studioMain.reinforceSlots[4] = rtt;
							break;
						}
					}

				}
				break;

			case "UnitContainer256Stage":
				studioMain.tfUnit256Container = t;
				break;

			case "UnitContainer512Stage":
				studioMain.tfUnit512Container = t;

				detailPopupSimpleRotater = t.GetComponent<SimpleRotater>();

				break;

			case "Camera256":
				if(t.parent.name == "99 Studio") studioMain.cam256 = t.gameObject.camera;
				break;
			case "Camera512":
				if(t.parent.name == "99 Studio")
				{
					studioMain.cam512 = t.gameObject.camera;
				}
				break;

			case "UnitMake":
				if(t.parent.name == "StudioRoot") studioMain.rootUnitMake = t.gameObject;
				break;

			case "UnitMakeVer2":
				if(t.parent.name == "StudioRoot") studioMain.rootPreUnitMake = t.gameObject;
				break;


			case "BtnUnitCompose_D":
				studioMain.composeController[0] = t.GetComponent<PlayMakerFSM>();
				break;
			case "BtnUnitCompose_C":
				studioMain.composeController[1] = t.GetComponent<PlayMakerFSM>();
				break;
			case "BtnUnitCompose_B":
				studioMain.composeController[2] = t.GetComponent<PlayMakerFSM>();
				break;
			case "BtnUnitCompose_A":
				studioMain.composeController[3] = t.GetComponent<PlayMakerFSM>();
				break;

			case "card1frame":
				if(t.parent.name == "moveCard") studioMain.composeCardFrame1 = t.GetComponent<UICardFrame>();
				break;

			case "card2frame":
				if(t.parent.name == "moveCard") studioMain.composeCardFrame2 = t.GetComponent<UICardFrame>();
				break;


			case "RuneCompose":
				if(t.parent.name == "StudioRoot") studioMain.rootCompose = t.gameObject; t.gameObject.SetActive(false);
				break;

			case "SSEvolusion":
				if(t.parent.name == "StudioRoot") studioMain.rootEvolve[4] = t.gameObject; t.gameObject.SetActive(false);
				break;

			case "SEvolusion":
				if(t.parent.name == "StudioRoot") studioMain.rootEvolve[3] = t.gameObject; t.gameObject.SetActive(false);
				break;

			case "AEvolusion":
				if(t.parent.name == "StudioRoot") studioMain.rootEvolve[2] = t.gameObject; t.gameObject.SetActive(false);
				break;

			case "BEvolusion":
				if(t.parent.name == "StudioRoot") studioMain.rootEvolve[1] = t.gameObject; t.gameObject.SetActive(false);
				break;

			case "CEvolusion":
				if(t.parent.name == "StudioRoot") studioMain.rootEvolve[0] = t.gameObject; t.gameObject.SetActive(false);
				break;


			case "PartsUpgrade":
				if(t.parent.name == "StudioRoot") studioMain.rootReinforce = t.gameObject;
				break;

			case "ChallengeResult":

				GameObject.DestroyImmediate(t.gameObject,true);

				/*
				studioMain.rootChallengeResult = t.gameObject;

				studioMain.challengeResultContainer = t.gameObject;
				clearChallengeTree(t);
				studioMain.challengeController = t.GetComponentsInChildren<PlayMakerFSM>(true);

				foreach(PlayMakerFSM fff in studioMain.challengeController)
				{
					switch(fff.name)
					{
					case "result_icon01":
						studioMain.challengeResultSlotFSM[0] = fff;
						break;
					case "result_icon02":
						studioMain.challengeResultSlotFSM[1] = fff;
						break;
					case "result_icon03":
						studioMain.challengeResultSlotFSM[2] = fff;
						break;
					case "result_icon04":
						studioMain.challengeResultSlotFSM[3] = fff;
						break;
					case "result_icon05":
						studioMain.challengeResultSlotFSM[4] = fff;
						break;
					case "result_icon06":
						studioMain.challengeResultSlotFSM[5] = fff;
						break;
					case "result_icon07":
						studioMain.challengeResultSlotFSM[6] = fff;
						break;
					case "result_icon08":
						studioMain.challengeResultSlotFSM[7] = fff;
						break;
					case "result_icon09":
						studioMain.challengeResultSlotFSM[8] = fff;
						break;
					}
				}

				foreach(PlayMakerFSM sfsm in studioMain.challengeResultSlotFSM)
				{
					Transform[] zzz = sfsm.GetComponentsInChildren<Transform>(true);
					for(int z = zzz.Length - 1; z >= 0; --z)
					{
						if(zzz[z].name == "result_icon")
						{
							GameObject.DestroyImmediate(zzz[z].gameObject, true);
							break;
						}
					}

				}
				*/


				break;
			case "base":
				if(t.parent.name == "08 MENU")
				{
					GameObject.DestroyImmediate(t.gameObject,true);
				}
				break;
			case "08 MENU":

				menuRoot = t.gameObject;

				t.gameObject.SetActive(true);
				Camera c = t.gameObject.GetComponent<Camera>();
				if(c != null) DestroyImmediate(c,true);

				UICamera c1 = t.gameObject.GetComponent<UICamera>();
				if(c1 != null) DestroyImmediate(c1,true);

				UIPanel c2 = t.gameObject.GetComponent<UIPanel>();
				if(c2 != null) DestroyImmediate(c2,true);
				break;

			case "BtnHeropartsUpgrade":

				studioMain.reinforceStarter = t.GetComponent<PlayMakerFSM>();

				PlayMakerFSM[] rins = t.GetComponents<PlayMakerFSM>();

				studioMain.reinforceController = rins;

				foreach(PlayMakerFSM r in rins)
				{
					r.enabled = false;
				}

				break;
			case "BtnUnitMake_special":

				studioMain.makeStarter = t.GetComponent<PlayMakerFSM>();

				PlayMakerFSM[] rm = t.GetComponents<PlayMakerFSM>();
				
				studioMain.makeController = rm;

				foreach(PlayMakerFSM r in rm)
				{
					r.enabled = false;
				}
				break;

			case "BtnUnitMake_NEW":
//				studioMain.makeStarter = t.GetComponent<PlayMakerFSM>();
//				studioMain.makeStarter.enabled = false;
				break;


			case "BtnTranscendence":
				
				studioMain.transcendStarter = t.GetComponent<PlayMakerFSM>();
				
				PlayMakerFSM[] tm = t.GetComponents<PlayMakerFSM>();
				
				studioMain.reinforceController = tm;
				
				foreach(PlayMakerFSM r in tm)
				{
					r.enabled = false;
				}
				
				break;



			case "BtnSSEvolution":
				studioMain.evolveController[4] = t.GetComponent<PlayMakerFSM>();
				studioMain.evolveController[4].enabled = false;
				break;
			case "BtnSEvolution":
				studioMain.evolveController[3] = t.GetComponent<PlayMakerFSM>();
				studioMain.evolveController[3].enabled = false;
				break;
			case "BtnAEvolution":
				studioMain.evolveController[2] = t.GetComponent<PlayMakerFSM>();
				studioMain.evolveController[2].enabled = false;
				break;
			case "BtnBEvolution":
				studioMain.evolveController[1] = t.GetComponent<PlayMakerFSM>();
				studioMain.evolveController[1].enabled = false;
				break;
			case "BtnCEvolution":
				studioMain.evolveController[0] = t.GetComponent<PlayMakerFSM>();
				studioMain.evolveController[0].enabled = false;
				break;







			case "reset":
				if(t.parent.name == "08 MENU")
				{
					GameObject.DestroyImmediate(t.gameObject,true);
				}
				break;



			case "main":

				if(t.parent.parent.parent.name == "PartsUpgrade")
				{
					studioMain.reinforceMoveablePanelParent = t;
				}
				else if(t.parent.parent.parent.name == "UnitMake")
				{
					studioMain.moveablePanelParent = t;
				}
				else if(t.parent.parent.parent.name == "RuneCompose")
				{
					studioMain.composeMovablePanelParent = t;
				}
				else if(t.parent.parent.parent.name == "Transcendence")
				{
					studioMain.transcendMovablePanelParent = t;

					PlayMakerFSM df = t.parent.GetComponent<PlayMakerFSM>();
					if(df != null)
					{
						//DestroyImmediate(df);
					}

					if(t.parent.animation != null)
					{
						DestroyImmediate(t.parent.animation);
					}

				}

				if(t.parent.name == "PanelDummy")
				{
					Transform[] dt = t.gameObject.GetComponentsInChildren<Transform>(true);
					for(int j = dt.Length - 1; j >= 0; --j)
					{
						if(dt[j] == null || dt[j] == t) continue;
						GameObject.DestroyImmediate(dt[j].gameObject,true);
					}
				}
				break;

			case "Card_d_studio256": studioMain.cardStudio256[0] = t.gameObject; break;
			case "Card_c_studio256": studioMain.cardStudio256[1] = t.gameObject; break;
			case "Card_b_studio256": studioMain.cardStudio256[2] = t.gameObject; break;
			case "Card_a_studio256": studioMain.cardStudio256[3] = t.gameObject; break;
			case "Card_s_studio256": studioMain.cardStudio256[4] = t.gameObject; break;
			case "Card_SS_studio256": studioMain.cardStudio256[5] = t.gameObject; break;

			case "Card_d_studio512": studioMain.cardStudio512[0] = t.gameObject; break;
			case "Card_c_studio512": studioMain.cardStudio512[1] = t.gameObject; break;
			case "Card_b_studio512": studioMain.cardStudio512[2] = t.gameObject; break;
			case "Card_a_studio512": studioMain.cardStudio512[3] = t.gameObject; break;
			case "Card_s_studio512": studioMain.cardStudio512[4] = t.gameObject; break;
			case "Card_SS_studio512": studioMain.cardStudio512[5] = t.gameObject; break;

			case "studio10_container": studioMain.card10Container = t.gameObject; studioMain.card10Container.SetActive(false); break;

			case "studio2_container": studioMain.card2Container = t.gameObject; studioMain.card2Container.SetActive(false); break;

			case "studio2_1": studioMain.card2BgSlot[0] = t.GetComponent<StudioCardBgSlot>(); break;
			case "studio2_2": studioMain.card2BgSlot[1] = t.GetComponent<StudioCardBgSlot>(); break;

			case "EvolveSourceContainer": studioMain.evolveSourceBgSlot = t.GetComponent<StudioCardBgSlot>(); t.gameObject.SetActive(false);  break;

			case "ss_evolutionSourceFrame": studioMain.evolutionCardFrame[4] = t.GetComponent<UICardFrame>(); break;
			case "s_evolutionSourceFrame": studioMain.evolutionCardFrame[3] = t.GetComponent<UICardFrame>(); break;
			case "a_evolutionSourceFrame": studioMain.evolutionCardFrame[2] = t.GetComponent<UICardFrame>(); break;
			case "b_evolutionSourceFrame": studioMain.evolutionCardFrame[1] = t.GetComponent<UICardFrame>(); break;
			case "c_evolutionSourceFrame": studioMain.evolutionCardFrame[0] = t.GetComponent<UICardFrame>(); break;

			case "SSevolutionAnimationContainer": studioMain.evolutionRotationAni[4] = t.animation; break;
			case "SevolutionAnimationContainer": studioMain.evolutionRotationAni[3] = t.animation; break;
			case "AevolutionAnimationContainer": studioMain.evolutionRotationAni[2] = t.animation; break;
			case "BevolutionAnimationContainer": studioMain.evolutionRotationAni[1] = t.animation; break;
			case "CevolutionAnimationContainer": studioMain.evolutionRotationAni[0] = t.animation; break;


			case "SSEvolveMoveable": studioMain.evolveMovablePanelParent[4] = t; break;
			case "SEvolveMoveable": studioMain.evolveMovablePanelParent[3] = t; break;
			case "AEvolveMoveable": studioMain.evolveMovablePanelParent[2] = t; break;
			case "BEvolveMoveable": studioMain.evolveMovablePanelParent[1] = t; break;
			case "CEvolveMoveable": studioMain.evolveMovablePanelParent[0] = t; break;

			case "Transcendence": studioMain.rootTranscend = t.gameObject; t.gameObject.SetActive(false); break;

			case "startdoor": 

				if(t.parent.name == "10 EffectZoneBottom")
				{
					studioMain.transcendStartDoor = t.gameObject; 
				}
				break;

			case "black_background": studioMain.blackBackground = t.gameObject ;break;


			case "runestone": 

				if(t.parent.name == "runestone")
				{
					studioMain.evolutionRuneRenderer = t.gameObject.GetComponent<Renderer>() ;break;
				}
				break;

			case "parts_icon01":
				studioMain.reinforceRenderingSlot[0] = t.renderer;
				studioMain.reinforceRenderingSlot[0].sharedMaterial.SetTextureScale("_diffuse_alpha",new Vector2(0.342f, 0.4f));
				studioMain.reinforceRenderingSlot[0].sharedMaterial.SetTextureOffset("_diffuse_alpha",new Vector2(0, 0.5f));
				break;
			case "parts_icon02":
				studioMain.reinforceRenderingSlot[1] = t.renderer;
				studioMain.reinforceRenderingSlot[1].sharedMaterial.SetTextureScale("_diffuse_alpha",new Vector2(0.342f, 0.4f));
				studioMain.reinforceRenderingSlot[1].sharedMaterial.SetTextureOffset("_diffuse_alpha",new Vector2(0.336f, 0.498f));
				break;
			case "parts_icon03":
				studioMain.reinforceRenderingSlot[2] = t.renderer;
				studioMain.reinforceRenderingSlot[2].sharedMaterial.SetTextureScale("_diffuse_alpha",new Vector2(0.342f, 0.4f));
				studioMain.reinforceRenderingSlot[2].sharedMaterial.SetTextureOffset("_diffuse_alpha",new Vector2(0.688f, 0.5f));
				break;
			case "parts_icon04":
				studioMain.reinforceRenderingSlot[3] = t.renderer;
				studioMain.reinforceRenderingSlot[3].sharedMaterial.SetTextureScale("_diffuse_alpha",new Vector2(0.342f, 0.4f));
				studioMain.reinforceRenderingSlot[3].sharedMaterial.SetTextureOffset("_diffuse_alpha",new Vector2(0, 0.114f));
				break;
			case "parts_icon05":
				studioMain.reinforceRenderingSlot[4] = t.renderer;
				studioMain.reinforceRenderingSlot[4].sharedMaterial.SetTextureScale("_diffuse_alpha",new Vector2(0.342f, 0.4f));
				studioMain.reinforceRenderingSlot[4].sharedMaterial.SetTextureOffset("_diffuse_alpha",new Vector2(0.336f, 0.114f));
				break;

			case "compose_summon_infobg":
				//t.gameObject.SetActive(false);
				GameObject.DestroyImmediate(t.gameObject,true);
				break;

			case "planeexplosion_001":

				if(studioMain.megaMorpeAnim == null)
				{
					studioMain.megaMorpeAnim = t.GetComponent<MegaMorphAnim>();
				}

				break;

			}

			if(t == null || t.gameObject == null) continue;

			if(t.parent == studioMain.reinforceMoveablePanelParent)
			{
				GameObject.DestroyImmediate(t.gameObject,true);
			}

			if(t.parent == studioMain.transcendMovablePanelParent)
			{
				GameObject.DestroyImmediate(t.gameObject,true);
			}


			if(t == null || t.gameObject == null) continue;

			int sIndex = 0;
			if(t.name.Contains("studio10_"))
			{
				string[] fuck = t.name.Split('_');
				int.TryParse(fuck[1], out sIndex);
				if(sIndex > 0)
				{
					studioMain.card10BgSlot[sIndex-1] = t.GetComponent<StudioCardBgSlot>();
				}
			}

			if(t.name.Contains("card_output"))
			{
				if(t.parent.name == "10card_ouput")
				{
					int.TryParse(t.name.Substring(t.name.Length - 2), out sIndex);
					if(sIndex > 0)
					{
						studioMain.card10Cover[sIndex-1] = t.GetComponent<StudioCardCoverSlot>();
						studioMain.card10Cover[sIndex-1].rare = 0;
					}
				}
			}

		}









		UIWidget[] wg = menuRoot.GetComponentsInChildren<UIWidget>(true);

		for(int i = wg.Length - 1; i >= 0; --i)
		{
			if(wg[i] == null) continue;
			GameObject.DestroyImmediate(wg[i].gameObject,true);
		}

		UIButton[] ub = menuRoot.GetComponentsInChildren<UIButton>(true);
		
		for(int i = ub.Length - 1; i >= 0; --i)
		{
			DestroyImmediate(ub[i],true);
		}


		BoxCollider[] b = menuRoot.GetComponentsInChildren<BoxCollider>(true);
		
		for(int i = b.Length - 1; i >= 0; --i)
		{
			DestroyImmediate(b[i],true);
		}

		// 카메라 초기화...
		Camera[] cams = root.GetComponentsInChildren<Camera>(true);
		
		List<Camera> dc = new List<Camera>();

		for(int i = 0; i < cams.Length; ++i)
		{
			if(dc.Contains(cams[i]) == false && cams[i].name.Contains("256") == false && cams[i].name.Contains("512") == false)
			{
				dc.Add(cams[i]);
			}
		}
		
		setupCam.cams = dc.ToArray();


		reinforceTfs = studioMain.rootReinforce.GetComponentsInChildren<Transform>(true);
		unitMakeTfs = studioMain.rootUnitMake.GetComponentsInChildren<Transform>(true);
		unitPreMakeTfs = studioMain.rootPreUnitMake.GetComponentsInChildren<Transform>(true);
		composeTfs = studioMain.rootCompose.GetComponentsInChildren<Transform>(true);
		transcendTfs = studioMain.rootTranscend.GetComponentsInChildren<Transform>(true);

		evolveSSTfs = studioMain.rootEvolve[4].GetComponentsInChildren<Transform>(true);
		evolveSTfs = studioMain.rootEvolve[3].GetComponentsInChildren<Transform>(true);
		evolveATfs = studioMain.rootEvolve[2].GetComponentsInChildren<Transform>(true);
		evolveBTfs = studioMain.rootEvolve[1].GetComponentsInChildren<Transform>(true);
		evolveCTfs = studioMain.rootEvolve[0].GetComponentsInChildren<Transform>(true);

		reinforcePs = studioMain.rootReinforce.GetComponentsInChildren<ParticleSystem>(true);
		unitMakePs = studioMain.rootUnitMake.GetComponentsInChildren<ParticleSystem>(true);
		unitPreMakePs = studioMain.rootPreUnitMake.GetComponentsInChildren<ParticleSystem>(true);
		composePs = studioMain.rootCompose.GetComponentsInChildren<ParticleSystem>(true);

		evolveSSPs = studioMain.rootEvolve[4].GetComponentsInChildren<ParticleSystem>(true);
		evolveSPs = studioMain.rootEvolve[3].GetComponentsInChildren<ParticleSystem>(true);
		evolveAPs = studioMain.rootEvolve[2].GetComponentsInChildren<ParticleSystem>(true);
		evolveBPs = studioMain.rootEvolve[1].GetComponentsInChildren<ParticleSystem>(true);
		evolveCPs = studioMain.rootEvolve[0].GetComponentsInChildren<ParticleSystem>(true);

		transendPs = studioMain.rootTranscend.GetComponentsInChildren<ParticleSystem>(true);


		PlayMakerFSM[] fsm = menuRoot.GetComponentsInChildren<PlayMakerFSM>(true);
		
		for(int i = fsm.Length - 1; i >= 0; --i)
		{
			PlayMakerFSM t = fsm[i];
			if(t == null || t.gameObject == menuRoot) continue;
		}

		reinforceFSM = studioMain.rootReinforce.GetComponentsInChildren<PlayMakerFSM>(true);
		unitMakeFSM = studioMain.rootUnitMake.GetComponentsInChildren<PlayMakerFSM>(true);
		unitPreMakeFSM = studioMain.rootPreUnitMake.GetComponentsInChildren<PlayMakerFSM>(true);
		composeFSM = studioMain.rootCompose.GetComponentsInChildren<PlayMakerFSM>(true);

		evolveSSFSM = studioMain.rootEvolve[4].GetComponentsInChildren<PlayMakerFSM>(true);
		evolveSFSM = studioMain.rootEvolve[3].GetComponentsInChildren<PlayMakerFSM>(true);
		evolveAFSM = studioMain.rootEvolve[2].GetComponentsInChildren<PlayMakerFSM>(true);
		evolveBFSM = studioMain.rootEvolve[1].GetComponentsInChildren<PlayMakerFSM>(true);
		evolveCFSM = studioMain.rootEvolve[0].GetComponentsInChildren<PlayMakerFSM>(true);

		transcendFSM = studioMain.rootTranscend.GetComponentsInChildren<PlayMakerFSM>(true);

	}


	Dictionary<Transform, ObjectDefaultInformation> _objectDefaultInfo = new Dictionary<Transform, ObjectDefaultInformation>();
	Dictionary<PlayMakerFSM, bool> _fsmActive = new Dictionary<PlayMakerFSM, bool>();

	public void reset(RuneStudioMain.Type type)
	{
		if(type == RuneStudioMain.Type.Reinforce)
		{
			foreach(PlayMakerFSM kv in studioMain.reinforceController)
			{
				kv.enabled = false;
			}

			reset2 (reinforceTfs, reinforcePs, reinforceFSM);
		}
		else if(type == RuneStudioMain.Type.UnitMake)
		{

			foreach(PlayMakerFSM kv in studioMain.makeController)
			{
				kv.enabled = false;
			}

			reset2 (unitMakeTfs, unitMakePs, unitMakeFSM);
			reset2 (unitPreMakeTfs, unitPreMakePs, unitPreMakeFSM);
		}
		else if(type == RuneStudioMain.Type.Evolve)
		{
			foreach(PlayMakerFSM kv in studioMain.evolveController)
			{
				kv.enabled = false;
			}

			reset2 (evolveSSTfs, evolveSSPs, evolveSSFSM);
			reset2 (evolveSTfs, evolveSPs, evolveSFSM);
			reset2 (evolveATfs, evolveAPs, evolveAFSM);
			reset2 (evolveBTfs, evolveBPs, evolveBFSM);
			reset2 (evolveCTfs, evolveCPs, evolveCFSM);	

		}
		else if(type == RuneStudioMain.Type.Compose)
		{

			foreach(PlayMakerFSM kv in studioMain.composeController)
			{
				kv.enabled = false;
			}
			
			reset2 (composeTfs, composePs, composeFSM);

		}
		else if(type == RuneStudioMain.Type.Transcend)
		{
			
			foreach(PlayMakerFSM kv in studioMain.transcendController)
			{
				kv.enabled = false;
			}
			
			reset2 (transcendTfs, transendPs, transcendFSM);
			
		}


	}


	void reset2(Transform[] tfs, ParticleSystem[] ps, PlayMakerFSM[] fsms)
	{
		if(tfs != null)
		{
			ObjectDefaultInformation info;
			foreach(Transform tf in tfs)
			{
				info = _objectDefaultInfo[tf];
				tf.gameObject.SetActive(info.isActive);
				tf.localPosition = info.localPosition;
				tf.localScale = info.localScale;
				tf.localRotation = info.localRotation;
			}
		}

		if(ps != null)
		{
			foreach(ParticleSystem p in ps)
			{
				p.Stop();
				p.Clear();
				p.Play();
			}
		}

		if(fsms != null)
		{
			foreach(PlayMakerFSM f in fsms)
			{
				f.enabled = _fsmActive[f];
			}
		}
	}



	void resetTransform(Transform[] resetTarget)
	{
		if(resetTarget != null)
		{
			foreach(Transform tf in resetTarget)
			{
				if(tf == null) continue;
				ObjectDefaultInformation info = new ObjectDefaultInformation();
				info.isActive = tf.gameObject.activeSelf;
				info.localPosition = tf.localPosition;
				info.localScale = tf.localScale;
				info.localRotation = tf.localRotation;
				_objectDefaultInfo[tf] = info;
			}
		}
	}

	void resetFSM(PlayMakerFSM[] resetTarget)
	{
		if(resetTarget != null)
		{
			foreach(PlayMakerFSM f in resetTarget)
			{
				if(f == null) continue;
				_fsmActive[f] = f.enabled;
			}
		}
	}



	void Awake()
	{
		resetTransform(reinforceTfs);
		resetTransform(unitMakeTfs);
		resetTransform(unitPreMakeTfs);

		resetTransform(evolveSSTfs);
		resetTransform(evolveSTfs);
		resetTransform(evolveATfs);
		resetTransform(evolveBTfs);
		resetTransform(evolveCTfs);

		resetTransform(composeTfs);
		resetTransform(transcendTfs);

		resetFSM(unitMakeFSM);
		resetFSM(unitPreMakeFSM);
		resetFSM(reinforceFSM);
		resetFSM(composeFSM);
		resetFSM(transcendFSM);

		resetFSM(evolveSSFSM);
		resetFSM(evolveSFSM);
		resetFSM(evolveAFSM);
		resetFSM(evolveBFSM);
		resetFSM(evolveCFSM);


		if(Application.isPlaying)
		{
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.characterRotate.container = detailPopupSimpleRotater.transform;
			GameManager.me.uiManager.popupSummonDetail.characterRotate.container = detailPopupSimpleRotater.transform;
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.sampleContainer = detailPopupSimpleRotater.transform;
			GameManager.me.uiManager.popupSummonDetail.sampleContainer = detailPopupSimpleRotater.transform;
			
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.simpleRotater = detailPopupSimpleRotater;
			GameManager.me.uiManager.popupSummonDetail.rotater = detailPopupSimpleRotater;

			studioMain.goMakeCompleteGuidePanel = GameManager.me.uiManager.goMakeCompleteGuidePanel;
		}

	}

}



public struct ObjectDefaultInformation
{
	public bool isActive;
	public Vector3 localPosition;
	public Vector3 localScale;
	public Quaternion localRotation;
}



