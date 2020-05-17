public class MapObjectInfo
{
	public string id = "";
	public float x = 0.0f;
	public float y = 0.0f;
	
	public bool hasRandomX = false;
	public float rx = 0.0f;
	
	public string groupId = "NONE";
	
	public float getXPos()
	{
		if(rx < 0)
		{
			return UnityEngine.Random.Range(x + rx, x);
		}
		else
		{
			return UnityEngine.Random.Range(x, x + rx);
		}
	}
}