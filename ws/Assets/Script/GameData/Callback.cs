public class Callback
{
	public delegate void Default();

	public delegate void Int(int param1);

	public delegate void Progress(float progress, bool isComplete);


	public Callback ()
	{
	}
}

