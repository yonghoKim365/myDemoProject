using System;

public class PartsUVAnimation
{
	const string MAIN_TEX = "_MainTex";

	public string targetName;

	public float xSpeed = 0.5f;
	public float ySpeed = -0.4f;
	public bool useLimit = true;
	public bool limitTypeIsMinus = false;
	public float limitValue = 1.0f;
	public string targetShaderProperty = MAIN_TEX;
	public bool isOnOffType = false;
	public bool useSharedMaterial = false;
	public TextureAnimation.ScrollType scrollType = TextureAnimation.ScrollType.Y_ONLY;

	public void setData(string data)
	{
		string[] temp = data.Split('@');

		targetName = temp[0];

		temp = temp[1].Split(',');

		float.TryParse(temp[0], out xSpeed);
		float.TryParse(temp[1], out ySpeed);
		useLimit = (temp[2] == "Y");
		limitTypeIsMinus = (temp[3] == "Y");
		float.TryParse(temp[4], out limitValue);
		targetShaderProperty = temp[5];
		isOnOffType = (temp[6] == "Y");
		useSharedMaterial = (temp[7] == "Y");

		switch(temp[8])
		{
		case "B":
			scrollType = TextureAnimation.ScrollType.BOTH;
			break;
		case "X":
			scrollType = TextureAnimation.ScrollType.X_ONLY;
			break;
		case "Y":
			scrollType = TextureAnimation.ScrollType.Y_ONLY;
			break;

		}
	}

	public void Apply(TextureAnimationLoopType ta)
	{
		ta.xSpeed = xSpeed;
		ta.ySpeed = ySpeed;
		ta.useLimit = useLimit;
		ta.limtTypeIsMinus = limitTypeIsMinus;
		ta.limit = limitValue;
		ta.targetShaderProperty = targetShaderProperty;
		ta.isOnOffType = isOnOffType;
		ta.isShareMaterial = useSharedMaterial;
		ta.scrollType = ta.scrollType;
	}



}

