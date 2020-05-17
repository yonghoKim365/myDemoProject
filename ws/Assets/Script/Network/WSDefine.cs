public class WSDefine
{
	public const string HANDICAP_TYPE_UNIT = "1";
	public const string HANDICAP_TYPE_SKILL = "2";
	public const string HANDICAP_TYPE_BOTH = "3";

	public const string NORMAL_HEADER = "일반";
	public const string PREMIUM_HEADER = "프리";

	public const int SYSTEM_CHECK_MODE_SERVICE = 0;
	public const int SYSTEM_CHECK_MODE_MAINTENANCE = 1;
	public const int SYSTEM_CHECK_MODE_UPDATE_CLIENT = 2;

	public const string GAME_SUCCESS = "SUCCESS";
	public const string GAME_FAILED = "FAILED";
	public const string GAME_GIVEUP = "GIVEUP";

	public const string NETWORK_ERROR = "ToC_ERROR";

	public const string RUBY = "RUBY";
	public const string GOLD = "GOLD";
	public const string EXP = "EXP";
	public const string ENERGY = "ENERGY";
	public const string RUNESTONE = "RSTONE";

	public const string MSG_TYPE_INVITE_REWARD = "INVITE_REWARD";
	public const string MSG_TYPE_GIFT = "GIFT";

	public const string GIFT_TYPE_ENERGY = "ENERGY";
	public const string GIFT_TYPE_GOLD = "GOLD";
	public const string GIFT_TYPE_RUBY = "RUBY";
	public const string GIFT_TYPE_FRIENDPOINT = "FRPNT";
//	public const string GIFT_TYPE_RUNESTONE = "RUNESTONE";
	public const string GIFT_TYPE_ITEM = "ITEM";

	public const string ITEM_TYPE_EQUIPMENT = "EQUIPMENT";
	public const string ITEM_TYPE_SKILLRUNE = "SKILLRUNE";
	public const string ITEM_TYPE_UNITRUNE = "UNITRUNE";

	public const string REWARD_TYPE_ENERGY_SHORT = "ENGY";
	public const string REWARD_TYPE_ENERGY = "ENERGY";
	public const string REWARD_TYPE_GOLD = "GOLD";
	public const string REWARD_TYPE_RUBY = "RUBY";
	public const string REWARD_TYPE_FRIENDPOINT = "FRPNT";
	public const string REWARD_TYPE_TICKET = "TICKET";
	public const string REWARD_TYPE_ITEM = "ITEM";
	public const string REWARD_TYPE_EXP = "EXP";
	public const string REWARD_TYPE_GACHA = "GACHA";
	public const string REWARD_TYPE_RUNE = "RUNE";
	public const string REWARD_TYPE_RUNESTONE = "RSTONE";


	public const string ICON_RUBY = "img_icn_cash";
	public const string ICON_EXP = "icn_exp";
	public const string ICON_GOLD = "img_icn_gamemoney";
	public const string ICON_ENERGY = "img_icn_energe";
	public const string ICON_TICKET = "img_icn_cashicn_ticket";
	public const string ICON_HART = "icn_socialpoint";
	public const string ICON_RUNE = "img_reward_item_rune";
	public const string ICON_RUNESTONE = "img_icn_runestone";

	public const string ICON_REWARD_ANIMAL = "img_reward_item_animal";
	public const string ICON_REWARD_ENERGY = "img_reward_item_energy";
	public const string ICON_REWARD_EQUIP = "img_reward_item_equip";
	public const string ICON_REWARD_EXP = "img_reward_item_exp";
	public const string ICON_REWARD_GOLD = "img_reward_item_gold";
	public const string ICON_REWARD_RUBY = "img_reward_item_ruby";
	public const string ICON_REWARD_RUNE = "img_reward_item_rune";
	public const string ICON_REWARD_RUNESTONE = "img_reward_item_runestone";

	public const string MISSION_KIND_MAIN = "MAIN";
	public const string MISSION_KIND_SUB = "SUB";
	public const string MISSION_KIND_EVENT = "EVENT";
	public const string MISSION_KIND_PLAY = "PLAY";


	public const int LEAGUE_BRONZE = 1;
	public const int LEAGUE_SILVER = 2;
	public const int LEAGUE_GOLD = 3;
	public const int LEAGUE_MASTER = 4;
	public const int LEAGUE_PLATINUM = 5;
	public const int LEAGUE_LEGEND = 6;

	public const int LAST_PLAY_STATUS_NONE = 0;
	public const int LAST_PLAY_STATUS_EPIC = 1;
	public const int LAST_PLAY_STATUS_HELL = 2;
	public const int LAST_PLAY_STATUS_CHAMPIONSHIP = 3;
	public const int LAST_PLAY_STATUS_FRIENDPVP = 4;

	public const int WIN = 1;
	public const int LOSE = 0;
	public const int INVALID = -1;
	public const int GIVE_UP = 2;
	public const int NOT_CONTINUE = 3;

	public const int DOING = 0;
	public const int CLEAR = 1;
	public const int CLOSE = 2;

	public const int REFRESH				= -1;

	public const int TRUE					= 1;
	public const int FALSE					= 0;
	public const int YES					= 1;
	public const int NO					= 0;
	public const int SUCCESS				= 1;
	public const int FAIL					= 0;
	public const int KAKAO_GUESTID_LENGTH 	= 18;

	// MODE
	public const int MODE_NONE				= 0;
	public const int MODE_EPIC				= 1;
	public const int MODE_CHALLENGE		= 2;
	public const int MODE_CHAMPIONSHIP		= 3;
	
	// CHALLENGE MODE
	public const int CHALLENGE_RUSH		= 1;
	public const int CHALLENGE_SURVIVE		= 2;
	public const int CHALLENGE_HUNT		= 3;

	
	// CHAMPIONSHIP STATUS
	public const int CHAMPIONSHIP_OPEN		= 1; 
	public const int CHAMPIONSHIP_CLOSE	= 2; 


	// MAKE, COMPOSE MODE
//	public const int MACOM_NORMAL			= 0;
//	public const int MACOM_PREMIUM			= 1;
//	public const int MACOM_SPECIAL			= 2;
	
	// MISSION STATE
	public const int MISSION_OPEN			= 0;
	public const int MISSION_CLEAR			= 1;
	public const int MISSION_CLOSE			= 2;
	
	// MESSAGE
	public const string ADMIN_ID				= "admin";
	public const int MESSAGE_EXPIRED		= 240; // 2주 보관
	
	public const int MAX_INCREASING_ENERGY	= 5;
	public const int TIME_INCREASING_ENERGY = 180;
	
	public const string DEFAULT_SELECT_HERO = "LEO";




	public static string getItemIconByRewardCode(string code)
	{
		switch(code)
		{
		case REWARD_TYPE_ENERGY_SHORT:
		case REWARD_TYPE_ENERGY:
			return ICON_ENERGY;

		case REWARD_TYPE_GOLD:
			return ICON_GOLD;

		case REWARD_TYPE_RUBY:
			return ICON_RUBY;

		case REWARD_TYPE_FRIENDPOINT:
			return ICON_HART;

		case REWARD_TYPE_TICKET:
			return ICON_TICKET;

		case REWARD_TYPE_EXP:
			return ICON_EXP;

		case REWARD_TYPE_RUNESTONE:
			return ICON_RUNESTONE;

		case REWARD_TYPE_RUNE:
		case REWARD_TYPE_ITEM:
			return ICON_RUNE;
		}

		return "";
	}

}

