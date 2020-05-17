using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RuneStudioMain : MonoBehaviour 
{
	public const string REINFORCE_START = "weapon_upgrade_start";

	public const string REINFORCE_CLOSE = "card_levelup_close";

	public const string REINFORCE_OPEN = "card_levelup_open";

	public const string REINFORCE_END = "weapon_upgrade_end";

	public const string MAKE_START = "gatcha_summon";


	public const string MAKE_D = "card_d_on";
	public const string MAKE_C = "card_c_on";
	public const string MAKE_B = "card_b_on";
	public const string MAKE_A = "card_a_on";
	public const string MAKE_S = "card_s_on";


	public const string MAKE_5CARD = "5card_open";
	public const string MAKE_10CARD = "10card_open";

	public PlayMakerFSM makeStarter;
	public PlayMakerFSM transcendStarter;
	public PlayMakerFSM reinforceStarter;


	public void playState(string fsmName)
	{
		if(type == Type.Reinforce)
		{
			foreach(PlayMakerFSM fsm in reinforceController)
			{
				if(fsm.FsmName == fsmName)
				{
					fsm.enabled = true;
					return;
				}
			}
		}
		else if(type == Type.UnitMake)
		{

			foreach(PlayMakerFSM fsm in makeController)
			{
				if(fsm.FsmName == fsmName)
				{
//					Debug.Log(" playstate : " + fsmName);
					fsm.enabled = true;
					return;
				}
			}
		}
	}

}
