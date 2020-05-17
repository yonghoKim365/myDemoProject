public class Well512Random
{
	uint[] state = new uint[16];
	uint index = 0;
	
	public Well512Random(uint nSeed)
	{
#if UNITY_EDITOR
		try
		{
			UnityEngine.Debug.Log("RANDOM SEED  : " + nSeed);
			Log.logError("====== SEED : " + nSeed);
		}
		catch(System.Exception e)
		{
		}
#endif

		uint s = nSeed;
		for (int i = 0; i < 16; i++)
		{
			state[i] = s;
			s += s + 73;
		}
	}


	public void init(uint nSeed)
	{

		#if UNITY_EDITOR
		Log.logError(fff++ + " range : " + rangeValue + "     " + GameManager.me.stageManager.playTime);
		UnityEngine.Debug.LogError("RANDOM SEED  : " + nSeed);
#else
//		if(GameManager.isDebugBuild)
//		{
//			Log.logError(fff++ + " range : " + rangeValue + "     " + GameManager.me.stageManager.playTime);
//		}
		#endif


		uint s = nSeed;
		for (int i = 0; i < 16; i++)
		{
			state[i] = s;
			s += s + 73;
		}
		index = 0;
		fff = 0;
		rangeValue = 0;
	}


	// max값 미만으로 나온다. 0~101로 지정했으면 0,100까지만 나온다.
	int fff = 0;
	int rangeValue = 0;
	public int Range(int minValue, int maxValue)
	{
		rangeValue = (int)((Next() % (maxValue - minValue)) + minValue);

#if UNITY_EDITOR
		Log.logError(fff++ + " range : " + rangeValue + "     " + GameManager.me.stageManager.playTime);
#else
//		if(GameManager.isDebugBuild)
//		{
//			Log.logError(fff++ + " range : " + rangeValue + "     " + GameManager.me.stageManager.playTime);
//		}
#endif


		return rangeValue;
	}

	internal uint Next(int minValue, int maxValue)
	{
		return (uint)((Next() % (maxValue - minValue)) + minValue);
	}
	
	public uint Next(uint maxValue)
	{
		return Next() % maxValue;
	}
	
	public uint Next()
	{
		uint a, b, c, d;
		
		a = state[index];
		c = state[(index + 13) & 15];
		b = a ^ c ^ (a << 16) ^ (c << 15);
		c = state[(index + 9) & 15];
		c ^= (c >> 11);
		a = state[index] = b ^ c;
		d = a ^ ((a << 5) & 0xda442d24U);
		index = (index + 15) & 15;
		a = state[index];
		state[index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);
		
		return state[index];
	}
}