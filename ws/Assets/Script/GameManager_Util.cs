using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

sealed public partial class GameManager : MonoBehaviour {

	
	public static Vector2[] angleTable = new Vector2[361];
	//public static float[] angleCosTable = new float[361];
	//public static float[] angleSinTable = new float[361];

	public static IVector2[] fixedAngleTable = new IVector2[361];
	public static IVector3[] forwardTable = new IVector3[361];
	
	const int TOTAL_RANDOM_NUM = 1500;
	
	private static int[] _randoms = new int[TOTAL_RANDOM_NUM];
	private static float[] _randomFs = new float[TOTAL_RANDOM_NUM];
	private static int _randomIndex = 0;
	
	
	void setAngleData()
	{
		for(int i = 0; i < 361; ++i)
		{
			angleTable[i].x = Mathf.Cos(i*Mathf.Deg2Rad);
			angleTable[i].y = Mathf.Sin(i*Mathf.Deg2Rad);
			
			//angleCosTable[i] = Mathf.Cos(i*Mathf.Deg2Rad);
			//angleSinTable[i] = Mathf.Sin(i*Mathf.Deg2Rad);
		}	

		setFixedAngleData();
	}	

	
	void setFixedAngleData()
	{
		fixedAngleTable[0].x.Value = 4096;fixedAngleTable[0].y.Value = 0;
		fixedAngleTable[1].x.Value = 4095;fixedAngleTable[1].y.Value = 71;
		fixedAngleTable[2].x.Value = 4094;fixedAngleTable[2].y.Value = 143;
		fixedAngleTable[3].x.Value = 4090;fixedAngleTable[3].y.Value = 214;
		fixedAngleTable[4].x.Value = 4086;fixedAngleTable[4].y.Value = 286;
		fixedAngleTable[5].x.Value = 4080;fixedAngleTable[5].y.Value = 357;
		fixedAngleTable[6].x.Value = 4074;fixedAngleTable[6].y.Value = 428;
		fixedAngleTable[7].x.Value = 4065;fixedAngleTable[7].y.Value = 499;
		fixedAngleTable[8].x.Value = 4056;fixedAngleTable[8].y.Value = 570;
		fixedAngleTable[9].x.Value = 4046;fixedAngleTable[9].y.Value = 641;
		fixedAngleTable[10].x.Value = 4034;fixedAngleTable[10].y.Value = 711;
		fixedAngleTable[11].x.Value = 4021;fixedAngleTable[11].y.Value = 782;
		fixedAngleTable[12].x.Value = 4006;fixedAngleTable[12].y.Value = 852;
		fixedAngleTable[13].x.Value = 3991;fixedAngleTable[13].y.Value = 921;
		fixedAngleTable[14].x.Value = 3974;fixedAngleTable[14].y.Value = 991;
		fixedAngleTable[15].x.Value = 3956;fixedAngleTable[15].y.Value = 1060;
		fixedAngleTable[16].x.Value = 3937;fixedAngleTable[16].y.Value = 1129;
		fixedAngleTable[17].x.Value = 3917;fixedAngleTable[17].y.Value = 1198;
		fixedAngleTable[18].x.Value = 3896;fixedAngleTable[18].y.Value = 1266;
		fixedAngleTable[19].x.Value = 3873;fixedAngleTable[19].y.Value = 1334;
		fixedAngleTable[20].x.Value = 3849;fixedAngleTable[20].y.Value = 1401;
		fixedAngleTable[21].x.Value = 3824;fixedAngleTable[21].y.Value = 1468;
		fixedAngleTable[22].x.Value = 3798;fixedAngleTable[22].y.Value = 1534;
		fixedAngleTable[23].x.Value = 3770;fixedAngleTable[23].y.Value = 1600;
		fixedAngleTable[24].x.Value = 3742;fixedAngleTable[24].y.Value = 1666;
		fixedAngleTable[25].x.Value = 3712;fixedAngleTable[25].y.Value = 1731;
		fixedAngleTable[26].x.Value = 3681;fixedAngleTable[26].y.Value = 1796;
		fixedAngleTable[27].x.Value = 3650;fixedAngleTable[27].y.Value = 1860;
		fixedAngleTable[28].x.Value = 3617;fixedAngleTable[28].y.Value = 1923;
		fixedAngleTable[29].x.Value = 3582;fixedAngleTable[29].y.Value = 1986;
		fixedAngleTable[30].x.Value = 3547;fixedAngleTable[30].y.Value = 2048;
		fixedAngleTable[31].x.Value = 3511;fixedAngleTable[31].y.Value = 2110;
		fixedAngleTable[32].x.Value = 3474;fixedAngleTable[32].y.Value = 2171;
		fixedAngleTable[33].x.Value = 3435;fixedAngleTable[33].y.Value = 2231;
		fixedAngleTable[34].x.Value = 3396;fixedAngleTable[34].y.Value = 2290;
		fixedAngleTable[35].x.Value = 3355;fixedAngleTable[35].y.Value = 2349;
		fixedAngleTable[36].x.Value = 3314;fixedAngleTable[36].y.Value = 2408;
		fixedAngleTable[37].x.Value = 3271;fixedAngleTable[37].y.Value = 2465;
		fixedAngleTable[38].x.Value = 3228;fixedAngleTable[38].y.Value = 2522;
		fixedAngleTable[39].x.Value = 3183;fixedAngleTable[39].y.Value = 2578;
		fixedAngleTable[40].x.Value = 3138;fixedAngleTable[40].y.Value = 2633;
		fixedAngleTable[41].x.Value = 3091;fixedAngleTable[41].y.Value = 2687;
		fixedAngleTable[42].x.Value = 3044;fixedAngleTable[42].y.Value = 2741;
		fixedAngleTable[43].x.Value = 2996;fixedAngleTable[43].y.Value = 2793;
		fixedAngleTable[44].x.Value = 2946;fixedAngleTable[44].y.Value = 2845;
		fixedAngleTable[45].x.Value = 2896;fixedAngleTable[45].y.Value = 2896;
		fixedAngleTable[46].x.Value = 2845;fixedAngleTable[46].y.Value = 2946;
		fixedAngleTable[47].x.Value = 2793;fixedAngleTable[47].y.Value = 2996;
		fixedAngleTable[48].x.Value = 2741;fixedAngleTable[48].y.Value = 3044;
		fixedAngleTable[49].x.Value = 2687;fixedAngleTable[49].y.Value = 3091;
		fixedAngleTable[50].x.Value = 2633;fixedAngleTable[50].y.Value = 3138;
		fixedAngleTable[51].x.Value = 2578;fixedAngleTable[51].y.Value = 3183;
		fixedAngleTable[52].x.Value = 2522;fixedAngleTable[52].y.Value = 3228;
		fixedAngleTable[53].x.Value = 2465;fixedAngleTable[53].y.Value = 3271;
		fixedAngleTable[54].x.Value = 2408;fixedAngleTable[54].y.Value = 3314;
		fixedAngleTable[55].x.Value = 2349;fixedAngleTable[55].y.Value = 3355;
		fixedAngleTable[56].x.Value = 2290;fixedAngleTable[56].y.Value = 3396;
		fixedAngleTable[57].x.Value = 2231;fixedAngleTable[57].y.Value = 3435;
		fixedAngleTable[58].x.Value = 2171;fixedAngleTable[58].y.Value = 3474;
		fixedAngleTable[59].x.Value = 2110;fixedAngleTable[59].y.Value = 3511;
		fixedAngleTable[60].x.Value = 2048;fixedAngleTable[60].y.Value = 3547;
		fixedAngleTable[61].x.Value = 1986;fixedAngleTable[61].y.Value = 3582;
		fixedAngleTable[62].x.Value = 1923;fixedAngleTable[62].y.Value = 3617;
		fixedAngleTable[63].x.Value = 1860;fixedAngleTable[63].y.Value = 3650;
		fixedAngleTable[64].x.Value = 1796;fixedAngleTable[64].y.Value = 3681;
		fixedAngleTable[65].x.Value = 1731;fixedAngleTable[65].y.Value = 3712;
		fixedAngleTable[66].x.Value = 1666;fixedAngleTable[66].y.Value = 3742;
		fixedAngleTable[67].x.Value = 1600;fixedAngleTable[67].y.Value = 3770;
		fixedAngleTable[68].x.Value = 1534;fixedAngleTable[68].y.Value = 3798;
		fixedAngleTable[69].x.Value = 1468;fixedAngleTable[69].y.Value = 3824;
		fixedAngleTable[70].x.Value = 1401;fixedAngleTable[70].y.Value = 3849;
		fixedAngleTable[71].x.Value = 1334;fixedAngleTable[71].y.Value = 3873;
		fixedAngleTable[72].x.Value = 1266;fixedAngleTable[72].y.Value = 3896;
		fixedAngleTable[73].x.Value = 1198;fixedAngleTable[73].y.Value = 3917;
		fixedAngleTable[74].x.Value = 1129;fixedAngleTable[74].y.Value = 3937;
		fixedAngleTable[75].x.Value = 1060;fixedAngleTable[75].y.Value = 3956;
		fixedAngleTable[76].x.Value = 991;fixedAngleTable[76].y.Value = 3974;
		fixedAngleTable[77].x.Value = 921;fixedAngleTable[77].y.Value = 3991;
		fixedAngleTable[78].x.Value = 852;fixedAngleTable[78].y.Value = 4006;
		fixedAngleTable[79].x.Value = 782;fixedAngleTable[79].y.Value = 4021;
		fixedAngleTable[80].x.Value = 711;fixedAngleTable[80].y.Value = 4034;
		fixedAngleTable[81].x.Value = 641;fixedAngleTable[81].y.Value = 4046;
		fixedAngleTable[82].x.Value = 570;fixedAngleTable[82].y.Value = 4056;
		fixedAngleTable[83].x.Value = 499;fixedAngleTable[83].y.Value = 4065;
		fixedAngleTable[84].x.Value = 428;fixedAngleTable[84].y.Value = 4074;
		fixedAngleTable[85].x.Value = 357;fixedAngleTable[85].y.Value = 4080;
		fixedAngleTable[86].x.Value = 286;fixedAngleTable[86].y.Value = 4086;
		fixedAngleTable[87].x.Value = 214;fixedAngleTable[87].y.Value = 4090;
		fixedAngleTable[88].x.Value = 143;fixedAngleTable[88].y.Value = 4094;
		fixedAngleTable[89].x.Value = 71;fixedAngleTable[89].y.Value = 4095;
		fixedAngleTable[90].x.Value = 0;fixedAngleTable[90].y.Value = 4096;
		fixedAngleTable[91].x.Value = -71;fixedAngleTable[91].y.Value = 4095;
		fixedAngleTable[92].x.Value = -143;fixedAngleTable[92].y.Value = 4094;
		fixedAngleTable[93].x.Value = -214;fixedAngleTable[93].y.Value = 4090;
		fixedAngleTable[94].x.Value = -286;fixedAngleTable[94].y.Value = 4086;
		fixedAngleTable[95].x.Value = -357;fixedAngleTable[95].y.Value = 4080;
		fixedAngleTable[96].x.Value = -428;fixedAngleTable[96].y.Value = 4074;
		fixedAngleTable[97].x.Value = -499;fixedAngleTable[97].y.Value = 4065;
		fixedAngleTable[98].x.Value = -570;fixedAngleTable[98].y.Value = 4056;
		fixedAngleTable[99].x.Value = -641;fixedAngleTable[99].y.Value = 4046;
		fixedAngleTable[100].x.Value = -711;fixedAngleTable[100].y.Value = 4034;
		fixedAngleTable[101].x.Value = -782;fixedAngleTable[101].y.Value = 4021;
		fixedAngleTable[102].x.Value = -852;fixedAngleTable[102].y.Value = 4006;
		fixedAngleTable[103].x.Value = -921;fixedAngleTable[103].y.Value = 3991;
		fixedAngleTable[104].x.Value = -991;fixedAngleTable[104].y.Value = 3974;
		fixedAngleTable[105].x.Value = -1060;fixedAngleTable[105].y.Value = 3956;
		fixedAngleTable[106].x.Value = -1129;fixedAngleTable[106].y.Value = 3937;
		fixedAngleTable[107].x.Value = -1198;fixedAngleTable[107].y.Value = 3917;
		fixedAngleTable[108].x.Value = -1266;fixedAngleTable[108].y.Value = 3896;
		fixedAngleTable[109].x.Value = -1334;fixedAngleTable[109].y.Value = 3873;
		fixedAngleTable[110].x.Value = -1401;fixedAngleTable[110].y.Value = 3849;
		fixedAngleTable[111].x.Value = -1468;fixedAngleTable[111].y.Value = 3824;
		fixedAngleTable[112].x.Value = -1534;fixedAngleTable[112].y.Value = 3798;
		fixedAngleTable[113].x.Value = -1600;fixedAngleTable[113].y.Value = 3770;
		fixedAngleTable[114].x.Value = -1666;fixedAngleTable[114].y.Value = 3742;
		fixedAngleTable[115].x.Value = -1731;fixedAngleTable[115].y.Value = 3712;
		fixedAngleTable[116].x.Value = -1796;fixedAngleTable[116].y.Value = 3681;
		fixedAngleTable[117].x.Value = -1860;fixedAngleTable[117].y.Value = 3650;
		fixedAngleTable[118].x.Value = -1923;fixedAngleTable[118].y.Value = 3617;
		fixedAngleTable[119].x.Value = -1986;fixedAngleTable[119].y.Value = 3582;
		fixedAngleTable[120].x.Value = -2048;fixedAngleTable[120].y.Value = 3547;
		fixedAngleTable[121].x.Value = -2110;fixedAngleTable[121].y.Value = 3511;
		fixedAngleTable[122].x.Value = -2171;fixedAngleTable[122].y.Value = 3474;
		fixedAngleTable[123].x.Value = -2231;fixedAngleTable[123].y.Value = 3435;
		fixedAngleTable[124].x.Value = -2290;fixedAngleTable[124].y.Value = 3396;
		fixedAngleTable[125].x.Value = -2349;fixedAngleTable[125].y.Value = 3355;
		fixedAngleTable[126].x.Value = -2408;fixedAngleTable[126].y.Value = 3314;
		fixedAngleTable[127].x.Value = -2465;fixedAngleTable[127].y.Value = 3271;
		fixedAngleTable[128].x.Value = -2522;fixedAngleTable[128].y.Value = 3228;
		fixedAngleTable[129].x.Value = -2578;fixedAngleTable[129].y.Value = 3183;
		fixedAngleTable[130].x.Value = -2633;fixedAngleTable[130].y.Value = 3138;
		fixedAngleTable[131].x.Value = -2687;fixedAngleTable[131].y.Value = 3091;
		fixedAngleTable[132].x.Value = -2741;fixedAngleTable[132].y.Value = 3044;
		fixedAngleTable[133].x.Value = -2793;fixedAngleTable[133].y.Value = 2996;
		fixedAngleTable[134].x.Value = -2845;fixedAngleTable[134].y.Value = 2946;
		fixedAngleTable[135].x.Value = -2896;fixedAngleTable[135].y.Value = 2896;
		fixedAngleTable[136].x.Value = -2946;fixedAngleTable[136].y.Value = 2845;
		fixedAngleTable[137].x.Value = -2996;fixedAngleTable[137].y.Value = 2793;
		fixedAngleTable[138].x.Value = -3044;fixedAngleTable[138].y.Value = 2741;
		fixedAngleTable[139].x.Value = -3091;fixedAngleTable[139].y.Value = 2687;
		fixedAngleTable[140].x.Value = -3138;fixedAngleTable[140].y.Value = 2633;
		fixedAngleTable[141].x.Value = -3183;fixedAngleTable[141].y.Value = 2578;
		fixedAngleTable[142].x.Value = -3228;fixedAngleTable[142].y.Value = 2522;
		fixedAngleTable[143].x.Value = -3271;fixedAngleTable[143].y.Value = 2465;
		fixedAngleTable[144].x.Value = -3314;fixedAngleTable[144].y.Value = 2408;
		fixedAngleTable[145].x.Value = -3355;fixedAngleTable[145].y.Value = 2349;
		fixedAngleTable[146].x.Value = -3396;fixedAngleTable[146].y.Value = 2290;
		fixedAngleTable[147].x.Value = -3435;fixedAngleTable[147].y.Value = 2231;
		fixedAngleTable[148].x.Value = -3474;fixedAngleTable[148].y.Value = 2171;
		fixedAngleTable[149].x.Value = -3511;fixedAngleTable[149].y.Value = 2110;
		fixedAngleTable[150].x.Value = -3547;fixedAngleTable[150].y.Value = 2048;
		fixedAngleTable[151].x.Value = -3582;fixedAngleTable[151].y.Value = 1986;
		fixedAngleTable[152].x.Value = -3617;fixedAngleTable[152].y.Value = 1923;
		fixedAngleTable[153].x.Value = -3650;fixedAngleTable[153].y.Value = 1860;
		fixedAngleTable[154].x.Value = -3681;fixedAngleTable[154].y.Value = 1796;
		fixedAngleTable[155].x.Value = -3712;fixedAngleTable[155].y.Value = 1731;
		fixedAngleTable[156].x.Value = -3742;fixedAngleTable[156].y.Value = 1666;
		fixedAngleTable[157].x.Value = -3770;fixedAngleTable[157].y.Value = 1600;
		fixedAngleTable[158].x.Value = -3798;fixedAngleTable[158].y.Value = 1534;
		fixedAngleTable[159].x.Value = -3824;fixedAngleTable[159].y.Value = 1468;
		fixedAngleTable[160].x.Value = -3849;fixedAngleTable[160].y.Value = 1401;
		fixedAngleTable[161].x.Value = -3873;fixedAngleTable[161].y.Value = 1334;
		fixedAngleTable[162].x.Value = -3896;fixedAngleTable[162].y.Value = 1266;
		fixedAngleTable[163].x.Value = -3917;fixedAngleTable[163].y.Value = 1198;
		fixedAngleTable[164].x.Value = -3937;fixedAngleTable[164].y.Value = 1129;
		fixedAngleTable[165].x.Value = -3956;fixedAngleTable[165].y.Value = 1060;
		fixedAngleTable[166].x.Value = -3974;fixedAngleTable[166].y.Value = 991;
		fixedAngleTable[167].x.Value = -3991;fixedAngleTable[167].y.Value = 921;
		fixedAngleTable[168].x.Value = -4006;fixedAngleTable[168].y.Value = 852;
		fixedAngleTable[169].x.Value = -4021;fixedAngleTable[169].y.Value = 782;
		fixedAngleTable[170].x.Value = -4034;fixedAngleTable[170].y.Value = 711;
		fixedAngleTable[171].x.Value = -4046;fixedAngleTable[171].y.Value = 641;
		fixedAngleTable[172].x.Value = -4056;fixedAngleTable[172].y.Value = 570;
		fixedAngleTable[173].x.Value = -4065;fixedAngleTable[173].y.Value = 499;
		fixedAngleTable[174].x.Value = -4074;fixedAngleTable[174].y.Value = 428;
		fixedAngleTable[175].x.Value = -4080;fixedAngleTable[175].y.Value = 357;
		fixedAngleTable[176].x.Value = -4086;fixedAngleTable[176].y.Value = 286;
		fixedAngleTable[177].x.Value = -4090;fixedAngleTable[177].y.Value = 214;
		fixedAngleTable[178].x.Value = -4094;fixedAngleTable[178].y.Value = 143;
		fixedAngleTable[179].x.Value = -4095;fixedAngleTable[179].y.Value = 71;
		fixedAngleTable[180].x.Value = -4096;fixedAngleTable[180].y.Value = 0;
		fixedAngleTable[181].x.Value = -4095;fixedAngleTable[181].y.Value = -71;
		fixedAngleTable[182].x.Value = -4094;fixedAngleTable[182].y.Value = -143;
		fixedAngleTable[183].x.Value = -4090;fixedAngleTable[183].y.Value = -214;
		fixedAngleTable[184].x.Value = -4086;fixedAngleTable[184].y.Value = -286;
		fixedAngleTable[185].x.Value = -4080;fixedAngleTable[185].y.Value = -357;
		fixedAngleTable[186].x.Value = -4074;fixedAngleTable[186].y.Value = -428;
		fixedAngleTable[187].x.Value = -4065;fixedAngleTable[187].y.Value = -499;
		fixedAngleTable[188].x.Value = -4056;fixedAngleTable[188].y.Value = -570;
		fixedAngleTable[189].x.Value = -4046;fixedAngleTable[189].y.Value = -641;
		fixedAngleTable[190].x.Value = -4034;fixedAngleTable[190].y.Value = -711;
		fixedAngleTable[191].x.Value = -4021;fixedAngleTable[191].y.Value = -782;
		fixedAngleTable[192].x.Value = -4006;fixedAngleTable[192].y.Value = -852;
		fixedAngleTable[193].x.Value = -3991;fixedAngleTable[193].y.Value = -921;
		fixedAngleTable[194].x.Value = -3974;fixedAngleTable[194].y.Value = -991;
		fixedAngleTable[195].x.Value = -3956;fixedAngleTable[195].y.Value = -1060;
		fixedAngleTable[196].x.Value = -3937;fixedAngleTable[196].y.Value = -1129;
		fixedAngleTable[197].x.Value = -3917;fixedAngleTable[197].y.Value = -1198;
		fixedAngleTable[198].x.Value = -3896;fixedAngleTable[198].y.Value = -1266;
		fixedAngleTable[199].x.Value = -3873;fixedAngleTable[199].y.Value = -1334;
		fixedAngleTable[200].x.Value = -3849;fixedAngleTable[200].y.Value = -1401;
		fixedAngleTable[201].x.Value = -3824;fixedAngleTable[201].y.Value = -1468;
		fixedAngleTable[202].x.Value = -3798;fixedAngleTable[202].y.Value = -1534;
		fixedAngleTable[203].x.Value = -3770;fixedAngleTable[203].y.Value = -1600;
		fixedAngleTable[204].x.Value = -3742;fixedAngleTable[204].y.Value = -1666;
		fixedAngleTable[205].x.Value = -3712;fixedAngleTable[205].y.Value = -1731;
		fixedAngleTable[206].x.Value = -3681;fixedAngleTable[206].y.Value = -1796;
		fixedAngleTable[207].x.Value = -3650;fixedAngleTable[207].y.Value = -1860;
		fixedAngleTable[208].x.Value = -3617;fixedAngleTable[208].y.Value = -1923;
		fixedAngleTable[209].x.Value = -3582;fixedAngleTable[209].y.Value = -1986;
		fixedAngleTable[210].x.Value = -3547;fixedAngleTable[210].y.Value = -2048;
		fixedAngleTable[211].x.Value = -3511;fixedAngleTable[211].y.Value = -2110;
		fixedAngleTable[212].x.Value = -3474;fixedAngleTable[212].y.Value = -2171;
		fixedAngleTable[213].x.Value = -3435;fixedAngleTable[213].y.Value = -2231;
		fixedAngleTable[214].x.Value = -3396;fixedAngleTable[214].y.Value = -2290;
		fixedAngleTable[215].x.Value = -3355;fixedAngleTable[215].y.Value = -2349;
		fixedAngleTable[216].x.Value = -3314;fixedAngleTable[216].y.Value = -2408;
		fixedAngleTable[217].x.Value = -3271;fixedAngleTable[217].y.Value = -2465;
		fixedAngleTable[218].x.Value = -3228;fixedAngleTable[218].y.Value = -2522;
		fixedAngleTable[219].x.Value = -3183;fixedAngleTable[219].y.Value = -2578;
		fixedAngleTable[220].x.Value = -3138;fixedAngleTable[220].y.Value = -2633;
		fixedAngleTable[221].x.Value = -3091;fixedAngleTable[221].y.Value = -2687;
		fixedAngleTable[222].x.Value = -3044;fixedAngleTable[222].y.Value = -2741;
		fixedAngleTable[223].x.Value = -2996;fixedAngleTable[223].y.Value = -2793;
		fixedAngleTable[224].x.Value = -2946;fixedAngleTable[224].y.Value = -2845;
		fixedAngleTable[225].x.Value = -2896;fixedAngleTable[225].y.Value = -2896;
		fixedAngleTable[226].x.Value = -2845;fixedAngleTable[226].y.Value = -2946;
		fixedAngleTable[227].x.Value = -2793;fixedAngleTable[227].y.Value = -2996;
		fixedAngleTable[228].x.Value = -2741;fixedAngleTable[228].y.Value = -3044;
		fixedAngleTable[229].x.Value = -2687;fixedAngleTable[229].y.Value = -3091;
		fixedAngleTable[230].x.Value = -2633;fixedAngleTable[230].y.Value = -3138;
		fixedAngleTable[231].x.Value = -2578;fixedAngleTable[231].y.Value = -3183;
		fixedAngleTable[232].x.Value = -2522;fixedAngleTable[232].y.Value = -3228;
		fixedAngleTable[233].x.Value = -2465;fixedAngleTable[233].y.Value = -3271;
		fixedAngleTable[234].x.Value = -2408;fixedAngleTable[234].y.Value = -3314;
		fixedAngleTable[235].x.Value = -2349;fixedAngleTable[235].y.Value = -3355;
		fixedAngleTable[236].x.Value = -2290;fixedAngleTable[236].y.Value = -3396;
		fixedAngleTable[237].x.Value = -2231;fixedAngleTable[237].y.Value = -3435;
		fixedAngleTable[238].x.Value = -2171;fixedAngleTable[238].y.Value = -3474;
		fixedAngleTable[239].x.Value = -2110;fixedAngleTable[239].y.Value = -3511;
		fixedAngleTable[240].x.Value = -2048;fixedAngleTable[240].y.Value = -3547;
		fixedAngleTable[241].x.Value = -1986;fixedAngleTable[241].y.Value = -3582;
		fixedAngleTable[242].x.Value = -1923;fixedAngleTable[242].y.Value = -3617;
		fixedAngleTable[243].x.Value = -1860;fixedAngleTable[243].y.Value = -3650;
		fixedAngleTable[244].x.Value = -1796;fixedAngleTable[244].y.Value = -3681;
		fixedAngleTable[245].x.Value = -1731;fixedAngleTable[245].y.Value = -3712;
		fixedAngleTable[246].x.Value = -1666;fixedAngleTable[246].y.Value = -3742;
		fixedAngleTable[247].x.Value = -1600;fixedAngleTable[247].y.Value = -3770;
		fixedAngleTable[248].x.Value = -1534;fixedAngleTable[248].y.Value = -3798;
		fixedAngleTable[249].x.Value = -1468;fixedAngleTable[249].y.Value = -3824;
		fixedAngleTable[250].x.Value = -1401;fixedAngleTable[250].y.Value = -3849;
		fixedAngleTable[251].x.Value = -1334;fixedAngleTable[251].y.Value = -3873;
		fixedAngleTable[252].x.Value = -1266;fixedAngleTable[252].y.Value = -3896;
		fixedAngleTable[253].x.Value = -1198;fixedAngleTable[253].y.Value = -3917;
		fixedAngleTable[254].x.Value = -1129;fixedAngleTable[254].y.Value = -3937;
		fixedAngleTable[255].x.Value = -1060;fixedAngleTable[255].y.Value = -3956;
		fixedAngleTable[256].x.Value = -991;fixedAngleTable[256].y.Value = -3974;
		fixedAngleTable[257].x.Value = -921;fixedAngleTable[257].y.Value = -3991;
		fixedAngleTable[258].x.Value = -852;fixedAngleTable[258].y.Value = -4006;
		fixedAngleTable[259].x.Value = -782;fixedAngleTable[259].y.Value = -4021;
		fixedAngleTable[260].x.Value = -711;fixedAngleTable[260].y.Value = -4034;
		fixedAngleTable[261].x.Value = -641;fixedAngleTable[261].y.Value = -4046;
		fixedAngleTable[262].x.Value = -570;fixedAngleTable[262].y.Value = -4056;
		fixedAngleTable[263].x.Value = -499;fixedAngleTable[263].y.Value = -4065;
		fixedAngleTable[264].x.Value = -428;fixedAngleTable[264].y.Value = -4074;
		fixedAngleTable[265].x.Value = -357;fixedAngleTable[265].y.Value = -4080;
		fixedAngleTable[266].x.Value = -286;fixedAngleTable[266].y.Value = -4086;
		fixedAngleTable[267].x.Value = -214;fixedAngleTable[267].y.Value = -4090;
		fixedAngleTable[268].x.Value = -143;fixedAngleTable[268].y.Value = -4094;
		fixedAngleTable[269].x.Value = -71;fixedAngleTable[269].y.Value = -4095;
		fixedAngleTable[270].x.Value = 0;fixedAngleTable[270].y.Value = -4096;
		fixedAngleTable[271].x.Value = 71;fixedAngleTable[271].y.Value = -4095;
		fixedAngleTable[272].x.Value = 143;fixedAngleTable[272].y.Value = -4094;
		fixedAngleTable[273].x.Value = 214;fixedAngleTable[273].y.Value = -4090;
		fixedAngleTable[274].x.Value = 286;fixedAngleTable[274].y.Value = -4086;
		fixedAngleTable[275].x.Value = 357;fixedAngleTable[275].y.Value = -4080;
		fixedAngleTable[276].x.Value = 428;fixedAngleTable[276].y.Value = -4074;
		fixedAngleTable[277].x.Value = 499;fixedAngleTable[277].y.Value = -4065;
		fixedAngleTable[278].x.Value = 570;fixedAngleTable[278].y.Value = -4056;
		fixedAngleTable[279].x.Value = 641;fixedAngleTable[279].y.Value = -4046;
		fixedAngleTable[280].x.Value = 711;fixedAngleTable[280].y.Value = -4034;
		fixedAngleTable[281].x.Value = 782;fixedAngleTable[281].y.Value = -4021;
		fixedAngleTable[282].x.Value = 852;fixedAngleTable[282].y.Value = -4006;
		fixedAngleTable[283].x.Value = 921;fixedAngleTable[283].y.Value = -3991;
		fixedAngleTable[284].x.Value = 991;fixedAngleTable[284].y.Value = -3974;
		fixedAngleTable[285].x.Value = 1060;fixedAngleTable[285].y.Value = -3956;
		fixedAngleTable[286].x.Value = 1129;fixedAngleTable[286].y.Value = -3937;
		fixedAngleTable[287].x.Value = 1198;fixedAngleTable[287].y.Value = -3917;
		fixedAngleTable[288].x.Value = 1266;fixedAngleTable[288].y.Value = -3896;
		fixedAngleTable[289].x.Value = 1334;fixedAngleTable[289].y.Value = -3873;
		fixedAngleTable[290].x.Value = 1401;fixedAngleTable[290].y.Value = -3849;
		fixedAngleTable[291].x.Value = 1468;fixedAngleTable[291].y.Value = -3824;
		fixedAngleTable[292].x.Value = 1534;fixedAngleTable[292].y.Value = -3798;
		fixedAngleTable[293].x.Value = 1600;fixedAngleTable[293].y.Value = -3770;
		fixedAngleTable[294].x.Value = 1666;fixedAngleTable[294].y.Value = -3742;
		fixedAngleTable[295].x.Value = 1731;fixedAngleTable[295].y.Value = -3712;
		fixedAngleTable[296].x.Value = 1796;fixedAngleTable[296].y.Value = -3681;
		fixedAngleTable[297].x.Value = 1860;fixedAngleTable[297].y.Value = -3650;
		fixedAngleTable[298].x.Value = 1923;fixedAngleTable[298].y.Value = -3617;
		fixedAngleTable[299].x.Value = 1986;fixedAngleTable[299].y.Value = -3582;
		fixedAngleTable[300].x.Value = 2048;fixedAngleTable[300].y.Value = -3547;
		fixedAngleTable[301].x.Value = 2110;fixedAngleTable[301].y.Value = -3511;
		fixedAngleTable[302].x.Value = 2171;fixedAngleTable[302].y.Value = -3474;
		fixedAngleTable[303].x.Value = 2231;fixedAngleTable[303].y.Value = -3435;
		fixedAngleTable[304].x.Value = 2290;fixedAngleTable[304].y.Value = -3396;
		fixedAngleTable[305].x.Value = 2349;fixedAngleTable[305].y.Value = -3355;
		fixedAngleTable[306].x.Value = 2408;fixedAngleTable[306].y.Value = -3314;
		fixedAngleTable[307].x.Value = 2465;fixedAngleTable[307].y.Value = -3271;
		fixedAngleTable[308].x.Value = 2522;fixedAngleTable[308].y.Value = -3228;
		fixedAngleTable[309].x.Value = 2578;fixedAngleTable[309].y.Value = -3183;
		fixedAngleTable[310].x.Value = 2633;fixedAngleTable[310].y.Value = -3138;
		fixedAngleTable[311].x.Value = 2687;fixedAngleTable[311].y.Value = -3091;
		fixedAngleTable[312].x.Value = 2741;fixedAngleTable[312].y.Value = -3044;
		fixedAngleTable[313].x.Value = 2793;fixedAngleTable[313].y.Value = -2996;
		fixedAngleTable[314].x.Value = 2845;fixedAngleTable[314].y.Value = -2946;
		fixedAngleTable[315].x.Value = 2896;fixedAngleTable[315].y.Value = -2896;
		fixedAngleTable[316].x.Value = 2946;fixedAngleTable[316].y.Value = -2845;
		fixedAngleTable[317].x.Value = 2996;fixedAngleTable[317].y.Value = -2793;
		fixedAngleTable[318].x.Value = 3044;fixedAngleTable[318].y.Value = -2741;
		fixedAngleTable[319].x.Value = 3091;fixedAngleTable[319].y.Value = -2687;
		fixedAngleTable[320].x.Value = 3138;fixedAngleTable[320].y.Value = -2633;
		fixedAngleTable[321].x.Value = 3183;fixedAngleTable[321].y.Value = -2578;
		fixedAngleTable[322].x.Value = 3228;fixedAngleTable[322].y.Value = -2522;
		fixedAngleTable[323].x.Value = 3271;fixedAngleTable[323].y.Value = -2465;
		fixedAngleTable[324].x.Value = 3314;fixedAngleTable[324].y.Value = -2408;
		fixedAngleTable[325].x.Value = 3355;fixedAngleTable[325].y.Value = -2349;
		fixedAngleTable[326].x.Value = 3396;fixedAngleTable[326].y.Value = -2290;
		fixedAngleTable[327].x.Value = 3435;fixedAngleTable[327].y.Value = -2231;
		fixedAngleTable[328].x.Value = 3474;fixedAngleTable[328].y.Value = -2171;
		fixedAngleTable[329].x.Value = 3511;fixedAngleTable[329].y.Value = -2110;
		fixedAngleTable[330].x.Value = 3547;fixedAngleTable[330].y.Value = -2048;
		fixedAngleTable[331].x.Value = 3582;fixedAngleTable[331].y.Value = -1986;
		fixedAngleTable[332].x.Value = 3617;fixedAngleTable[332].y.Value = -1923;
		fixedAngleTable[333].x.Value = 3650;fixedAngleTable[333].y.Value = -1860;
		fixedAngleTable[334].x.Value = 3681;fixedAngleTable[334].y.Value = -1796;
		fixedAngleTable[335].x.Value = 3712;fixedAngleTable[335].y.Value = -1731;
		fixedAngleTable[336].x.Value = 3742;fixedAngleTable[336].y.Value = -1666;
		fixedAngleTable[337].x.Value = 3770;fixedAngleTable[337].y.Value = -1600;
		fixedAngleTable[338].x.Value = 3798;fixedAngleTable[338].y.Value = -1534;
		fixedAngleTable[339].x.Value = 3824;fixedAngleTable[339].y.Value = -1468;
		fixedAngleTable[340].x.Value = 3849;fixedAngleTable[340].y.Value = -1401;
		fixedAngleTable[341].x.Value = 3873;fixedAngleTable[341].y.Value = -1334;
		fixedAngleTable[342].x.Value = 3896;fixedAngleTable[342].y.Value = -1266;
		fixedAngleTable[343].x.Value = 3917;fixedAngleTable[343].y.Value = -1198;
		fixedAngleTable[344].x.Value = 3937;fixedAngleTable[344].y.Value = -1129;
		fixedAngleTable[345].x.Value = 3956;fixedAngleTable[345].y.Value = -1060;
		fixedAngleTable[346].x.Value = 3974;fixedAngleTable[346].y.Value = -991;
		fixedAngleTable[347].x.Value = 3991;fixedAngleTable[347].y.Value = -921;
		fixedAngleTable[348].x.Value = 4006;fixedAngleTable[348].y.Value = -852;
		fixedAngleTable[349].x.Value = 4021;fixedAngleTable[349].y.Value = -782;
		fixedAngleTable[350].x.Value = 4034;fixedAngleTable[350].y.Value = -711;
		fixedAngleTable[351].x.Value = 4046;fixedAngleTable[351].y.Value = -641;
		fixedAngleTable[352].x.Value = 4056;fixedAngleTable[352].y.Value = -570;
		fixedAngleTable[353].x.Value = 4065;fixedAngleTable[353].y.Value = -499;
		fixedAngleTable[354].x.Value = 4074;fixedAngleTable[354].y.Value = -428;
		fixedAngleTable[355].x.Value = 4080;fixedAngleTable[355].y.Value = -357;
		fixedAngleTable[356].x.Value = 4086;fixedAngleTable[356].y.Value = -286;
		fixedAngleTable[357].x.Value = 4090;fixedAngleTable[357].y.Value = -214;
		fixedAngleTable[358].x.Value = 4094;fixedAngleTable[358].y.Value = -143;
		fixedAngleTable[359].x.Value = 4095;fixedAngleTable[359].y.Value = -71;
		fixedAngleTable[360].x.Value = 4096;fixedAngleTable[360].y.Value = 0;
		
		
		forwardTable[0].x.Value = 0;forwardTable[0].z.Value = 4096;
		forwardTable[1].x.Value = 71;forwardTable[1].z.Value = 4095;
		forwardTable[2].x.Value = 143;forwardTable[2].z.Value = 4094;
		forwardTable[3].x.Value = 214;forwardTable[3].z.Value = 4090;
		forwardTable[4].x.Value = 286;forwardTable[4].z.Value = 4086;
		forwardTable[5].x.Value = 357;forwardTable[5].z.Value = 4080;
		forwardTable[6].x.Value = 428;forwardTable[6].z.Value = 4074;
		forwardTable[7].x.Value = 499;forwardTable[7].z.Value = 4065;
		forwardTable[8].x.Value = 570;forwardTable[8].z.Value = 4056;
		forwardTable[9].x.Value = 641;forwardTable[9].z.Value = 4046;
		forwardTable[10].x.Value = 711;forwardTable[10].z.Value = 4034;
		forwardTable[11].x.Value = 782;forwardTable[11].z.Value = 4021;
		forwardTable[12].x.Value = 852;forwardTable[12].z.Value = 4006;
		forwardTable[13].x.Value = 921;forwardTable[13].z.Value = 3991;
		forwardTable[14].x.Value = 991;forwardTable[14].z.Value = 3974;
		forwardTable[15].x.Value = 1060;forwardTable[15].z.Value = 3956;
		forwardTable[16].x.Value = 1129;forwardTable[16].z.Value = 3937;
		forwardTable[17].x.Value = 1198;forwardTable[17].z.Value = 3917;
		forwardTable[18].x.Value = 1266;forwardTable[18].z.Value = 3896;
		forwardTable[19].x.Value = 1334;forwardTable[19].z.Value = 3873;
		forwardTable[20].x.Value = 1401;forwardTable[20].z.Value = 3849;
		forwardTable[21].x.Value = 1468;forwardTable[21].z.Value = 3824;
		forwardTable[22].x.Value = 1534;forwardTable[22].z.Value = 3798;
		forwardTable[23].x.Value = 1600;forwardTable[23].z.Value = 3770;
		forwardTable[24].x.Value = 1666;forwardTable[24].z.Value = 3742;
		forwardTable[25].x.Value = 1731;forwardTable[25].z.Value = 3712;
		forwardTable[26].x.Value = 1796;forwardTable[26].z.Value = 3681;
		forwardTable[27].x.Value = 1860;forwardTable[27].z.Value = 3650;
		forwardTable[28].x.Value = 1923;forwardTable[28].z.Value = 3617;
		forwardTable[29].x.Value = 1986;forwardTable[29].z.Value = 3582;
		forwardTable[30].x.Value = 2048;forwardTable[30].z.Value = 3547;
		forwardTable[31].x.Value = 2110;forwardTable[31].z.Value = 3511;
		forwardTable[32].x.Value = 2171;forwardTable[32].z.Value = 3474;
		forwardTable[33].x.Value = 2231;forwardTable[33].z.Value = 3435;
		forwardTable[34].x.Value = 2290;forwardTable[34].z.Value = 3396;
		forwardTable[35].x.Value = 2349;forwardTable[35].z.Value = 3355;
		forwardTable[36].x.Value = 2408;forwardTable[36].z.Value = 3314;
		forwardTable[37].x.Value = 2465;forwardTable[37].z.Value = 3271;
		forwardTable[38].x.Value = 2522;forwardTable[38].z.Value = 3228;
		forwardTable[39].x.Value = 2578;forwardTable[39].z.Value = 3183;
		forwardTable[40].x.Value = 2633;forwardTable[40].z.Value = 3138;
		forwardTable[41].x.Value = 2687;forwardTable[41].z.Value = 3091;
		forwardTable[42].x.Value = 2741;forwardTable[42].z.Value = 3044;
		forwardTable[43].x.Value = 2793;forwardTable[43].z.Value = 2996;
		forwardTable[44].x.Value = 2845;forwardTable[44].z.Value = 2946;
		forwardTable[45].x.Value = 2896;forwardTable[45].z.Value = 2896;
		forwardTable[46].x.Value = 2946;forwardTable[46].z.Value = 2845;
		forwardTable[47].x.Value = 2996;forwardTable[47].z.Value = 2793;
		forwardTable[48].x.Value = 3044;forwardTable[48].z.Value = 2741;
		forwardTable[49].x.Value = 3091;forwardTable[49].z.Value = 2687;
		forwardTable[50].x.Value = 3138;forwardTable[50].z.Value = 2633;
		forwardTable[51].x.Value = 3183;forwardTable[51].z.Value = 2578;
		forwardTable[52].x.Value = 3228;forwardTable[52].z.Value = 2522;
		forwardTable[53].x.Value = 3271;forwardTable[53].z.Value = 2465;
		forwardTable[54].x.Value = 3314;forwardTable[54].z.Value = 2408;
		forwardTable[55].x.Value = 3355;forwardTable[55].z.Value = 2349;
		forwardTable[56].x.Value = 3396;forwardTable[56].z.Value = 2290;
		forwardTable[57].x.Value = 3435;forwardTable[57].z.Value = 2231;
		forwardTable[58].x.Value = 3474;forwardTable[58].z.Value = 2171;
		forwardTable[59].x.Value = 3511;forwardTable[59].z.Value = 2110;
		forwardTable[60].x.Value = 3547;forwardTable[60].z.Value = 2048;
		forwardTable[61].x.Value = 3582;forwardTable[61].z.Value = 1986;
		forwardTable[62].x.Value = 3617;forwardTable[62].z.Value = 1923;
		forwardTable[63].x.Value = 3650;forwardTable[63].z.Value = 1860;
		forwardTable[64].x.Value = 3681;forwardTable[64].z.Value = 1796;
		forwardTable[65].x.Value = 3712;forwardTable[65].z.Value = 1731;
		forwardTable[66].x.Value = 3742;forwardTable[66].z.Value = 1666;
		forwardTable[67].x.Value = 3770;forwardTable[67].z.Value = 1600;
		forwardTable[68].x.Value = 3798;forwardTable[68].z.Value = 1534;
		forwardTable[69].x.Value = 3824;forwardTable[69].z.Value = 1468;
		forwardTable[70].x.Value = 3849;forwardTable[70].z.Value = 1401;
		forwardTable[71].x.Value = 3873;forwardTable[71].z.Value = 1334;
		forwardTable[72].x.Value = 3896;forwardTable[72].z.Value = 1266;
		forwardTable[73].x.Value = 3917;forwardTable[73].z.Value = 1198;
		forwardTable[74].x.Value = 3937;forwardTable[74].z.Value = 1129;
		forwardTable[75].x.Value = 3956;forwardTable[75].z.Value = 1060;
		forwardTable[76].x.Value = 3974;forwardTable[76].z.Value = 991;
		forwardTable[77].x.Value = 3991;forwardTable[77].z.Value = 921;
		forwardTable[78].x.Value = 4006;forwardTable[78].z.Value = 852;
		forwardTable[79].x.Value = 4021;forwardTable[79].z.Value = 782;
		forwardTable[80].x.Value = 4034;forwardTable[80].z.Value = 711;
		forwardTable[81].x.Value = 4046;forwardTable[81].z.Value = 641;
		forwardTable[82].x.Value = 4056;forwardTable[82].z.Value = 570;
		forwardTable[83].x.Value = 4065;forwardTable[83].z.Value = 499;
		forwardTable[84].x.Value = 4074;forwardTable[84].z.Value = 428;
		forwardTable[85].x.Value = 4080;forwardTable[85].z.Value = 357;
		forwardTable[86].x.Value = 4086;forwardTable[86].z.Value = 286;
		forwardTable[87].x.Value = 4090;forwardTable[87].z.Value = 214;
		forwardTable[88].x.Value = 4094;forwardTable[88].z.Value = 143;
		forwardTable[89].x.Value = 4095;forwardTable[89].z.Value = 71;
		forwardTable[90].x.Value = 4096;forwardTable[90].z.Value = 0;
		forwardTable[91].x.Value = 4095;forwardTable[91].z.Value = -71;
		forwardTable[92].x.Value = 4094;forwardTable[92].z.Value = -143;
		forwardTable[93].x.Value = 4090;forwardTable[93].z.Value = -214;
		forwardTable[94].x.Value = 4086;forwardTable[94].z.Value = -286;
		forwardTable[95].x.Value = 4080;forwardTable[95].z.Value = -357;
		forwardTable[96].x.Value = 4074;forwardTable[96].z.Value = -428;
		forwardTable[97].x.Value = 4065;forwardTable[97].z.Value = -499;
		forwardTable[98].x.Value = 4056;forwardTable[98].z.Value = -570;
		forwardTable[99].x.Value = 4046;forwardTable[99].z.Value = -641;
		forwardTable[100].x.Value = 4034;forwardTable[100].z.Value = -711;
		forwardTable[101].x.Value = 4021;forwardTable[101].z.Value = -782;
		forwardTable[102].x.Value = 4006;forwardTable[102].z.Value = -852;
		forwardTable[103].x.Value = 3991;forwardTable[103].z.Value = -921;
		forwardTable[104].x.Value = 3974;forwardTable[104].z.Value = -991;
		forwardTable[105].x.Value = 3956;forwardTable[105].z.Value = -1060;
		forwardTable[106].x.Value = 3937;forwardTable[106].z.Value = -1129;
		forwardTable[107].x.Value = 3917;forwardTable[107].z.Value = -1198;
		forwardTable[108].x.Value = 3896;forwardTable[108].z.Value = -1266;
		forwardTable[109].x.Value = 3873;forwardTable[109].z.Value = -1334;
		forwardTable[110].x.Value = 3849;forwardTable[110].z.Value = -1401;
		forwardTable[111].x.Value = 3824;forwardTable[111].z.Value = -1468;
		forwardTable[112].x.Value = 3798;forwardTable[112].z.Value = -1534;
		forwardTable[113].x.Value = 3770;forwardTable[113].z.Value = -1600;
		forwardTable[114].x.Value = 3742;forwardTable[114].z.Value = -1666;
		forwardTable[115].x.Value = 3712;forwardTable[115].z.Value = -1731;
		forwardTable[116].x.Value = 3681;forwardTable[116].z.Value = -1796;
		forwardTable[117].x.Value = 3650;forwardTable[117].z.Value = -1860;
		forwardTable[118].x.Value = 3617;forwardTable[118].z.Value = -1923;
		forwardTable[119].x.Value = 3582;forwardTable[119].z.Value = -1986;
		forwardTable[120].x.Value = 3547;forwardTable[120].z.Value = -2048;
		forwardTable[121].x.Value = 3511;forwardTable[121].z.Value = -2110;
		forwardTable[122].x.Value = 3474;forwardTable[122].z.Value = -2171;
		forwardTable[123].x.Value = 3435;forwardTable[123].z.Value = -2231;
		forwardTable[124].x.Value = 3396;forwardTable[124].z.Value = -2290;
		forwardTable[125].x.Value = 3355;forwardTable[125].z.Value = -2349;
		forwardTable[126].x.Value = 3314;forwardTable[126].z.Value = -2408;
		forwardTable[127].x.Value = 3271;forwardTable[127].z.Value = -2465;
		forwardTable[128].x.Value = 3228;forwardTable[128].z.Value = -2522;
		forwardTable[129].x.Value = 3183;forwardTable[129].z.Value = -2578;
		forwardTable[130].x.Value = 3138;forwardTable[130].z.Value = -2633;
		forwardTable[131].x.Value = 3091;forwardTable[131].z.Value = -2687;
		forwardTable[132].x.Value = 3044;forwardTable[132].z.Value = -2741;
		forwardTable[133].x.Value = 2996;forwardTable[133].z.Value = -2793;
		forwardTable[134].x.Value = 2946;forwardTable[134].z.Value = -2845;
		forwardTable[135].x.Value = 2896;forwardTable[135].z.Value = -2896;
		forwardTable[136].x.Value = 2845;forwardTable[136].z.Value = -2946;
		forwardTable[137].x.Value = 2793;forwardTable[137].z.Value = -2996;
		forwardTable[138].x.Value = 2741;forwardTable[138].z.Value = -3044;
		forwardTable[139].x.Value = 2687;forwardTable[139].z.Value = -3091;
		forwardTable[140].x.Value = 2633;forwardTable[140].z.Value = -3138;
		forwardTable[141].x.Value = 2578;forwardTable[141].z.Value = -3183;
		forwardTable[142].x.Value = 2522;forwardTable[142].z.Value = -3228;
		forwardTable[143].x.Value = 2465;forwardTable[143].z.Value = -3271;
		forwardTable[144].x.Value = 2408;forwardTable[144].z.Value = -3314;
		forwardTable[145].x.Value = 2349;forwardTable[145].z.Value = -3355;
		forwardTable[146].x.Value = 2290;forwardTable[146].z.Value = -3396;
		forwardTable[147].x.Value = 2231;forwardTable[147].z.Value = -3435;
		forwardTable[148].x.Value = 2171;forwardTable[148].z.Value = -3474;
		forwardTable[149].x.Value = 2110;forwardTable[149].z.Value = -3511;
		forwardTable[150].x.Value = 2048;forwardTable[150].z.Value = -3547;
		forwardTable[151].x.Value = 1986;forwardTable[151].z.Value = -3582;
		forwardTable[152].x.Value = 1923;forwardTable[152].z.Value = -3617;
		forwardTable[153].x.Value = 1860;forwardTable[153].z.Value = -3650;
		forwardTable[154].x.Value = 1796;forwardTable[154].z.Value = -3681;
		forwardTable[155].x.Value = 1731;forwardTable[155].z.Value = -3712;
		forwardTable[156].x.Value = 1666;forwardTable[156].z.Value = -3742;
		forwardTable[157].x.Value = 1600;forwardTable[157].z.Value = -3770;
		forwardTable[158].x.Value = 1534;forwardTable[158].z.Value = -3798;
		forwardTable[159].x.Value = 1468;forwardTable[159].z.Value = -3824;
		forwardTable[160].x.Value = 1401;forwardTable[160].z.Value = -3849;
		forwardTable[161].x.Value = 1334;forwardTable[161].z.Value = -3873;
		forwardTable[162].x.Value = 1266;forwardTable[162].z.Value = -3896;
		forwardTable[163].x.Value = 1198;forwardTable[163].z.Value = -3917;
		forwardTable[164].x.Value = 1129;forwardTable[164].z.Value = -3937;
		forwardTable[165].x.Value = 1060;forwardTable[165].z.Value = -3956;
		forwardTable[166].x.Value = 991;forwardTable[166].z.Value = -3974;
		forwardTable[167].x.Value = 921;forwardTable[167].z.Value = -3991;
		forwardTable[168].x.Value = 852;forwardTable[168].z.Value = -4006;
		forwardTable[169].x.Value = 782;forwardTable[169].z.Value = -4021;
		forwardTable[170].x.Value = 711;forwardTable[170].z.Value = -4034;
		forwardTable[171].x.Value = 641;forwardTable[171].z.Value = -4046;
		forwardTable[172].x.Value = 570;forwardTable[172].z.Value = -4056;
		forwardTable[173].x.Value = 499;forwardTable[173].z.Value = -4065;
		forwardTable[174].x.Value = 428;forwardTable[174].z.Value = -4074;
		forwardTable[175].x.Value = 357;forwardTable[175].z.Value = -4080;
		forwardTable[176].x.Value = 286;forwardTable[176].z.Value = -4086;
		forwardTable[177].x.Value = 214;forwardTable[177].z.Value = -4090;
		forwardTable[178].x.Value = 143;forwardTable[178].z.Value = -4094;
		forwardTable[179].x.Value = 71;forwardTable[179].z.Value = -4095;
		forwardTable[180].x.Value = 0;forwardTable[180].z.Value = -4096;
		forwardTable[181].x.Value = -71;forwardTable[181].z.Value = -4095;
		forwardTable[182].x.Value = -143;forwardTable[182].z.Value = -4094;
		forwardTable[183].x.Value = -214;forwardTable[183].z.Value = -4090;
		forwardTable[184].x.Value = -286;forwardTable[184].z.Value = -4086;
		forwardTable[185].x.Value = -357;forwardTable[185].z.Value = -4080;
		forwardTable[186].x.Value = -428;forwardTable[186].z.Value = -4074;
		forwardTable[187].x.Value = -499;forwardTable[187].z.Value = -4065;
		forwardTable[188].x.Value = -570;forwardTable[188].z.Value = -4056;
		forwardTable[189].x.Value = -641;forwardTable[189].z.Value = -4046;
		forwardTable[190].x.Value = -711;forwardTable[190].z.Value = -4034;
		forwardTable[191].x.Value = -782;forwardTable[191].z.Value = -4021;
		forwardTable[192].x.Value = -852;forwardTable[192].z.Value = -4006;
		forwardTable[193].x.Value = -921;forwardTable[193].z.Value = -3991;
		forwardTable[194].x.Value = -991;forwardTable[194].z.Value = -3974;
		forwardTable[195].x.Value = -1060;forwardTable[195].z.Value = -3956;
		forwardTable[196].x.Value = -1129;forwardTable[196].z.Value = -3937;
		forwardTable[197].x.Value = -1198;forwardTable[197].z.Value = -3917;
		forwardTable[198].x.Value = -1266;forwardTable[198].z.Value = -3896;
		forwardTable[199].x.Value = -1334;forwardTable[199].z.Value = -3873;
		forwardTable[200].x.Value = -1401;forwardTable[200].z.Value = -3849;
		forwardTable[201].x.Value = -1468;forwardTable[201].z.Value = -3824;
		forwardTable[202].x.Value = -1534;forwardTable[202].z.Value = -3798;
		forwardTable[203].x.Value = -1600;forwardTable[203].z.Value = -3770;
		forwardTable[204].x.Value = -1666;forwardTable[204].z.Value = -3742;
		forwardTable[205].x.Value = -1731;forwardTable[205].z.Value = -3712;
		forwardTable[206].x.Value = -1796;forwardTable[206].z.Value = -3681;
		forwardTable[207].x.Value = -1860;forwardTable[207].z.Value = -3650;
		forwardTable[208].x.Value = -1923;forwardTable[208].z.Value = -3617;
		forwardTable[209].x.Value = -1986;forwardTable[209].z.Value = -3582;
		forwardTable[210].x.Value = -2048;forwardTable[210].z.Value = -3547;
		forwardTable[211].x.Value = -2110;forwardTable[211].z.Value = -3511;
		forwardTable[212].x.Value = -2171;forwardTable[212].z.Value = -3474;
		forwardTable[213].x.Value = -2231;forwardTable[213].z.Value = -3435;
		forwardTable[214].x.Value = -2290;forwardTable[214].z.Value = -3396;
		forwardTable[215].x.Value = -2349;forwardTable[215].z.Value = -3355;
		forwardTable[216].x.Value = -2408;forwardTable[216].z.Value = -3314;
		forwardTable[217].x.Value = -2465;forwardTable[217].z.Value = -3271;
		forwardTable[218].x.Value = -2522;forwardTable[218].z.Value = -3228;
		forwardTable[219].x.Value = -2578;forwardTable[219].z.Value = -3183;
		forwardTable[220].x.Value = -2633;forwardTable[220].z.Value = -3138;
		forwardTable[221].x.Value = -2687;forwardTable[221].z.Value = -3091;
		forwardTable[222].x.Value = -2741;forwardTable[222].z.Value = -3044;
		forwardTable[223].x.Value = -2793;forwardTable[223].z.Value = -2996;
		forwardTable[224].x.Value = -2845;forwardTable[224].z.Value = -2946;
		forwardTable[225].x.Value = -2896;forwardTable[225].z.Value = -2896;
		forwardTable[226].x.Value = -2946;forwardTable[226].z.Value = -2845;
		forwardTable[227].x.Value = -2996;forwardTable[227].z.Value = -2793;
		forwardTable[228].x.Value = -3044;forwardTable[228].z.Value = -2741;
		forwardTable[229].x.Value = -3091;forwardTable[229].z.Value = -2687;
		forwardTable[230].x.Value = -3138;forwardTable[230].z.Value = -2633;
		forwardTable[231].x.Value = -3183;forwardTable[231].z.Value = -2578;
		forwardTable[232].x.Value = -3228;forwardTable[232].z.Value = -2522;
		forwardTable[233].x.Value = -3271;forwardTable[233].z.Value = -2465;
		forwardTable[234].x.Value = -3314;forwardTable[234].z.Value = -2408;
		forwardTable[235].x.Value = -3355;forwardTable[235].z.Value = -2349;
		forwardTable[236].x.Value = -3396;forwardTable[236].z.Value = -2290;
		forwardTable[237].x.Value = -3435;forwardTable[237].z.Value = -2231;
		forwardTable[238].x.Value = -3474;forwardTable[238].z.Value = -2171;
		forwardTable[239].x.Value = -3511;forwardTable[239].z.Value = -2110;
		forwardTable[240].x.Value = -3547;forwardTable[240].z.Value = -2048;
		forwardTable[241].x.Value = -3582;forwardTable[241].z.Value = -1986;
		forwardTable[242].x.Value = -3617;forwardTable[242].z.Value = -1923;
		forwardTable[243].x.Value = -3650;forwardTable[243].z.Value = -1860;
		forwardTable[244].x.Value = -3681;forwardTable[244].z.Value = -1796;
		forwardTable[245].x.Value = -3712;forwardTable[245].z.Value = -1731;
		forwardTable[246].x.Value = -3742;forwardTable[246].z.Value = -1666;
		forwardTable[247].x.Value = -3770;forwardTable[247].z.Value = -1600;
		forwardTable[248].x.Value = -3798;forwardTable[248].z.Value = -1534;
		forwardTable[249].x.Value = -3824;forwardTable[249].z.Value = -1468;
		forwardTable[250].x.Value = -3849;forwardTable[250].z.Value = -1401;
		forwardTable[251].x.Value = -3873;forwardTable[251].z.Value = -1334;
		forwardTable[252].x.Value = -3896;forwardTable[252].z.Value = -1266;
		forwardTable[253].x.Value = -3917;forwardTable[253].z.Value = -1198;
		forwardTable[254].x.Value = -3937;forwardTable[254].z.Value = -1129;
		forwardTable[255].x.Value = -3956;forwardTable[255].z.Value = -1060;
		forwardTable[256].x.Value = -3974;forwardTable[256].z.Value = -991;
		forwardTable[257].x.Value = -3991;forwardTable[257].z.Value = -921;
		forwardTable[258].x.Value = -4006;forwardTable[258].z.Value = -852;
		forwardTable[259].x.Value = -4021;forwardTable[259].z.Value = -782;
		forwardTable[260].x.Value = -4034;forwardTable[260].z.Value = -711;
		forwardTable[261].x.Value = -4046;forwardTable[261].z.Value = -641;
		forwardTable[262].x.Value = -4056;forwardTable[262].z.Value = -570;
		forwardTable[263].x.Value = -4065;forwardTable[263].z.Value = -499;
		forwardTable[264].x.Value = -4074;forwardTable[264].z.Value = -428;
		forwardTable[265].x.Value = -4080;forwardTable[265].z.Value = -357;
		forwardTable[266].x.Value = -4086;forwardTable[266].z.Value = -286;
		forwardTable[267].x.Value = -4090;forwardTable[267].z.Value = -214;
		forwardTable[268].x.Value = -4094;forwardTable[268].z.Value = -143;
		forwardTable[269].x.Value = -4095;forwardTable[269].z.Value = -71;
		forwardTable[270].x.Value = -4096;forwardTable[270].z.Value = 0;
		forwardTable[271].x.Value = -4095;forwardTable[271].z.Value = 71;
		forwardTable[272].x.Value = -4094;forwardTable[272].z.Value = 143;
		forwardTable[273].x.Value = -4090;forwardTable[273].z.Value = 214;
		forwardTable[274].x.Value = -4086;forwardTable[274].z.Value = 286;
		forwardTable[275].x.Value = -4080;forwardTable[275].z.Value = 357;
		forwardTable[276].x.Value = -4074;forwardTable[276].z.Value = 428;
		forwardTable[277].x.Value = -4065;forwardTable[277].z.Value = 499;
		forwardTable[278].x.Value = -4056;forwardTable[278].z.Value = 570;
		forwardTable[279].x.Value = -4046;forwardTable[279].z.Value = 641;
		forwardTable[280].x.Value = -4034;forwardTable[280].z.Value = 711;
		forwardTable[281].x.Value = -4021;forwardTable[281].z.Value = 782;
		forwardTable[282].x.Value = -4006;forwardTable[282].z.Value = 852;
		forwardTable[283].x.Value = -3991;forwardTable[283].z.Value = 921;
		forwardTable[284].x.Value = -3974;forwardTable[284].z.Value = 991;
		forwardTable[285].x.Value = -3956;forwardTable[285].z.Value = 1060;
		forwardTable[286].x.Value = -3937;forwardTable[286].z.Value = 1129;
		forwardTable[287].x.Value = -3917;forwardTable[287].z.Value = 1198;
		forwardTable[288].x.Value = -3896;forwardTable[288].z.Value = 1266;
		forwardTable[289].x.Value = -3873;forwardTable[289].z.Value = 1334;
		forwardTable[290].x.Value = -3849;forwardTable[290].z.Value = 1401;
		forwardTable[291].x.Value = -3824;forwardTable[291].z.Value = 1468;
		forwardTable[292].x.Value = -3798;forwardTable[292].z.Value = 1534;
		forwardTable[293].x.Value = -3770;forwardTable[293].z.Value = 1600;
		forwardTable[294].x.Value = -3742;forwardTable[294].z.Value = 1666;
		forwardTable[295].x.Value = -3712;forwardTable[295].z.Value = 1731;
		forwardTable[296].x.Value = -3681;forwardTable[296].z.Value = 1796;
		forwardTable[297].x.Value = -3650;forwardTable[297].z.Value = 1860;
		forwardTable[298].x.Value = -3617;forwardTable[298].z.Value = 1923;
		forwardTable[299].x.Value = -3582;forwardTable[299].z.Value = 1986;
		forwardTable[300].x.Value = -3547;forwardTable[300].z.Value = 2048;
		forwardTable[301].x.Value = -3511;forwardTable[301].z.Value = 2110;
		forwardTable[302].x.Value = -3474;forwardTable[302].z.Value = 2171;
		forwardTable[303].x.Value = -3435;forwardTable[303].z.Value = 2231;
		forwardTable[304].x.Value = -3396;forwardTable[304].z.Value = 2290;
		forwardTable[305].x.Value = -3355;forwardTable[305].z.Value = 2349;
		forwardTable[306].x.Value = -3314;forwardTable[306].z.Value = 2408;
		forwardTable[307].x.Value = -3271;forwardTable[307].z.Value = 2465;
		forwardTable[308].x.Value = -3228;forwardTable[308].z.Value = 2522;
		forwardTable[309].x.Value = -3183;forwardTable[309].z.Value = 2578;
		forwardTable[310].x.Value = -3138;forwardTable[310].z.Value = 2633;
		forwardTable[311].x.Value = -3091;forwardTable[311].z.Value = 2687;
		forwardTable[312].x.Value = -3044;forwardTable[312].z.Value = 2741;
		forwardTable[313].x.Value = -2996;forwardTable[313].z.Value = 2793;
		forwardTable[314].x.Value = -2946;forwardTable[314].z.Value = 2845;
		forwardTable[315].x.Value = -2896;forwardTable[315].z.Value = 2896;
		forwardTable[316].x.Value = -2845;forwardTable[316].z.Value = 2946;
		forwardTable[317].x.Value = -2793;forwardTable[317].z.Value = 2996;
		forwardTable[318].x.Value = -2741;forwardTable[318].z.Value = 3044;
		forwardTable[319].x.Value = -2687;forwardTable[319].z.Value = 3091;
		forwardTable[320].x.Value = -2633;forwardTable[320].z.Value = 3138;
		forwardTable[321].x.Value = -2578;forwardTable[321].z.Value = 3183;
		forwardTable[322].x.Value = -2522;forwardTable[322].z.Value = 3228;
		forwardTable[323].x.Value = -2465;forwardTable[323].z.Value = 3271;
		forwardTable[324].x.Value = -2408;forwardTable[324].z.Value = 3314;
		forwardTable[325].x.Value = -2349;forwardTable[325].z.Value = 3355;
		forwardTable[326].x.Value = -2290;forwardTable[326].z.Value = 3396;
		forwardTable[327].x.Value = -2231;forwardTable[327].z.Value = 3435;
		forwardTable[328].x.Value = -2171;forwardTable[328].z.Value = 3474;
		forwardTable[329].x.Value = -2110;forwardTable[329].z.Value = 3511;
		forwardTable[330].x.Value = -2048;forwardTable[330].z.Value = 3547;
		forwardTable[331].x.Value = -1986;forwardTable[331].z.Value = 3582;
		forwardTable[332].x.Value = -1923;forwardTable[332].z.Value = 3617;
		forwardTable[333].x.Value = -1860;forwardTable[333].z.Value = 3650;
		forwardTable[334].x.Value = -1796;forwardTable[334].z.Value = 3681;
		forwardTable[335].x.Value = -1731;forwardTable[335].z.Value = 3712;
		forwardTable[336].x.Value = -1666;forwardTable[336].z.Value = 3742;
		forwardTable[337].x.Value = -1600;forwardTable[337].z.Value = 3770;
		forwardTable[338].x.Value = -1534;forwardTable[338].z.Value = 3798;
		forwardTable[339].x.Value = -1468;forwardTable[339].z.Value = 3824;
		forwardTable[340].x.Value = -1401;forwardTable[340].z.Value = 3849;
		forwardTable[341].x.Value = -1334;forwardTable[341].z.Value = 3873;
		forwardTable[342].x.Value = -1266;forwardTable[342].z.Value = 3896;
		forwardTable[343].x.Value = -1198;forwardTable[343].z.Value = 3917;
		forwardTable[344].x.Value = -1129;forwardTable[344].z.Value = 3937;
		forwardTable[345].x.Value = -1060;forwardTable[345].z.Value = 3956;
		forwardTable[346].x.Value = -991;forwardTable[346].z.Value = 3974;
		forwardTable[347].x.Value = -921;forwardTable[347].z.Value = 3991;
		forwardTable[348].x.Value = -852;forwardTable[348].z.Value = 4006;
		forwardTable[349].x.Value = -782;forwardTable[349].z.Value = 4021;
		forwardTable[350].x.Value = -711;forwardTable[350].z.Value = 4034;
		forwardTable[351].x.Value = -641;forwardTable[351].z.Value = 4046;
		forwardTable[352].x.Value = -570;forwardTable[352].z.Value = 4056;
		forwardTable[353].x.Value = -499;forwardTable[353].z.Value = 4065;
		forwardTable[354].x.Value = -428;forwardTable[354].z.Value = 4074;
		forwardTable[355].x.Value = -357;forwardTable[355].z.Value = 4080;
		forwardTable[356].x.Value = -286;forwardTable[356].z.Value = 4086;
		forwardTable[357].x.Value = -214;forwardTable[357].z.Value = 4090;
		forwardTable[358].x.Value = -143;forwardTable[358].z.Value = 4094;
		forwardTable[359].x.Value = -71;forwardTable[359].z.Value = 4095;
		forwardTable[360].x.Value = 0;forwardTable[360].z.Value = 4096;
		
		
	}

	/*
	public static int getRandomNum()
	{
		++_randomIndex;
		if(_randomIndex >= TOTAL_RANDOM_NUM) _randomIndex = 0;
		return _randoms[_randomIndex];
	}
	
	public static float getFloatRandomNum()
	{
		++_randomIndex;
		if(_randomIndex >= TOTAL_RANDOM_NUM) _randomIndex = 0;
		return _randomFs[_randomIndex];
	}	
	*/
	
	private void initCamera()
	{
		CharacterAttachedUI.gameViewCamera = gameCamera;
		gameCamera.gameObject.SetActive(false);
		tk2dCam.useTargetResolution = true;
		tk2dCam.UpdateCameraMatrix();
		tk2dCam.gameObject.SetActive(false);
		camRatioY = screenSize.y / Screen.height;

//		if(uiManager.runeStudio != null)
//		{
//			uiManager.runeStudio.resetter.setupCam.useTargetResolution = true;
//			uiManager.runeStudio.resetter.setupCam.UpdateCameraMatrix();
//			uiManager.runeStudio.resetter.setupCam.gameObject.SetActive(false);
//		}

	}	
	
	
//	void OnGUI()
//	{
//		if(showLog) GUI.Label(logRect, guiLog);
//	}	
	
	public void onCompleteBoxTransitionForwardComplete()
	{
	}
	
	public void onCompleteBoxTransitionBackwardComplete()
	{
		_isPaused = false;
	}
	

	
	
	public void showToast(string msg, int type)
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		//androidManager.activity.Call("showToast",msg,type);
#endif
	}
	
	

	
	public static void log(string str, string str2 = "")
	{
		return;
		/*
		if(_logString.Length > 270)
		{
			_logString = _logString.Substring(0, _logString.LastIndexOf("\n"));
		}
		
		_logString = str + " : (" + Time.realtimeSinceStartup +")" + str2 + "\n" + _logString;
		GameManager.me.logger.text = _logString;
		*/
	}	
	
	public void OnApplicationQuit()
	{
		if(AdPluginManager.useAdbrix) IgaworksUnityPluginAOS.Common.endSession();
		Application.Quit();
	}	
	

	public Collider overCheckCollider;
	private RaycastHit uiCheckHitInfo;

	//private Rectangle _touchRect = new Rectangle();

	private Vector2 _touchPositionV2;
	public bool uiOverCheck()
	{
		_touchPositionV2 = Util.screenPositionWithCamViewRect(Input.mousePosition);
		return ((_touchPositionV2.y >= 138 && _touchPositionV2.y <= 528) == false);
		//if(_touchRect.contains(Util.screenPositionWithCamViewRect(Input.mousePosition)))
		{

		}

		//if(Physics.Raycast(baseUICamera.ScreenPointToRay(Input.mousePosition), out uiCheckHitInfo))
		//{
			//Debug.Log("uiOverCheck : " + uiCheckHitInfo.collider.name + " : " + (uiCheckHitInfo.collider != overCheckCollider));
			//return (uiCheckHitInfo.collider != overCheckCollider);
		//}
		return true;
	}	



	public bool uiOverCheck(Vector2 pos)
	{
		_touchPositionV2 = Util.screenPositionWithCamViewRect(pos);
		return ((_touchPositionV2.y >= 138 && _touchPositionV2.y <= 528) == false);
		//if(_touchRect.contains(Util.screenPositionWithCamViewRect(Input.mousePosition)))
		{
			
		}
		
		//if(Physics.Raycast(baseUICamera.ScreenPointToRay(Input.mousePosition), out uiCheckHitInfo))
		//{
		//Debug.Log("uiOverCheck : " + uiCheckHitInfo.collider.name + " : " + (uiCheckHitInfo.collider != overCheckCollider));
		//return (uiCheckHitInfo.collider != overCheckCollider);
		//}
		return true;
	}




	private static Xfloat _timeScale = -100;
	public static float setTimeScale
	{
		set
		{
			_timeScale = value;
			Time.timeScale = value;
		}
	}

	
	public void clearMemory(float time = -1.0f)
	{
		StartCoroutine(clearMemoryCT(time));
	}

	IEnumerator clearMemoryCT(float time = -1.0f)
	{
		if(time < 0) yield return null;
		else yield return time;
		yield return Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}

	public static void checkAutoLandScape(){

		if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft && Screen.orientation!= ScreenOrientation.LandscapeLeft)
		{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
		else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight && Screen.orientation!= ScreenOrientation.LandscapeRight)
		{
			Screen.orientation = ScreenOrientation.LandscapeRight;
		}
	}


	public static void initDeviceOrientation()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait= false;
		Screen.autorotateToPortraitUpsideDown= false;
	}



	
	void OnDestroy()
	{
//		Debug.Log("OnDestroy GameManager");

		replayManager = null;

		networkManager = null;
		gameDataManager = null;
		soundManager = null;
		androidManager = null;
		resourceManager = null;
		info = null;

		effectManager = null;
		bulletManager = null;
		normalMapManager = null;
		cutSceneMapManager = null;
	
		clientDataLoader = null;
	
		mapManager = null;
	
		if(stageManager != null) stageManager.OnDestroy();
		
		stageManager = null;
		
		characterManager = null;
	
		uiManager = null;
	
		methodManager = null;
	
		gameCamera = null;
	
		gameCameraContainer = null;
	
		baseUICamera = null;
	
		inGameGUICamera = null;
	
		tk2dGameCamera = null;
	
		stage = null;
	
		assetPool = null;
	
		player = null;
	
		monster = null;
		mapObject = null;
	
		deletePool = null;

		me = null;

	}

	
	
}

