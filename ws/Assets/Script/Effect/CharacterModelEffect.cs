using System;

sealed public class CharacterModelEffect : Monster
{
	public CharacterModelEffect ()
	{
	}

//	sealed public override void deadComplete()
	public override void dead (bool useSound)
	{
		
	}
	
	public override string state {
		get {
			return _state;
		}
		set {
			_state = value;
		}
	}
}

