using UnityEngine;
using System.Collections;
using System;

public class Dkaghghk : MonoBehaviour {
	
	public static short fdsfas = 1000;
	public static short dsfafasfassfd = 1000;
	public static short adffffadfa = 1000;

	private static Xint _eCount = 0;

	public static int eCount
	{
		set
		{
			_eCount.Set(value);
		}
		get
		{
			return _eCount.Get();
		}
	}

	public static int securityLevel2_totalIndex = 5;
	public static int securityLevel2_baseIndex = 3;

	short _zl = 1000;
	short _zl2 = 1000;
	short _bzl = 1000;

	short _tempKey = 30;

	void Awake () {
		eCount = 0;
		fdsfas = (short)UnityEngine.Random.Range(1,500);
		dsfafasfassfd = (short)UnityEngine.Random.Range(1,200);
		adffffadfa = (short)UnityEngine.Random.Range(1000,4000);

		_tempKey = (short)UnityEngine.Random.Range(3,10);
		_zl = (short)(fdsfas ^ _tempKey);
		_zl2 = (short)(dsfafasfassfd ^ _tempKey);
		_bzl = (short)(adffffadfa ^ _tempKey);

		securityLevel2_totalIndex = UnityEngine.Random.Range(3,6);
		securityLevel2_baseIndex = UnityEngine.Random.Range(0,securityLevel2_totalIndex-1);
	}
	
	// Use this for initialization
	void Start () {
	}
	
	void Update () 
	{
		if(Time.frameCount % 1000 == 0){
			securityLevel2_baseIndex = UnityEngine.Random.Range(0,securityLevel2_totalIndex-1);
		}

		if(fdsfas != (short)(_zl ^ _tempKey))
		{
			++eCount;
		}

		if(dsfafasfassfd != (short)(_zl2 ^ _tempKey))
		{
			++eCount;
		}

		if(adffffadfa != (short)(_bzl ^ _tempKey))
		{
			++eCount;
		}


		fdsfas = (short)UnityEngine.Random.Range(1,500);
		dsfafasfassfd = (short)UnityEngine.Random.Range(1,200);
		adffffadfa = (short)UnityEngine.Random.Range(1000,4000);

		_tempKey = (short)UnityEngine.Random.Range(3,10);
		_zl = (short)(fdsfas ^ _tempKey);
		_zl2 = (short)(dsfafasfassfd ^ _tempKey);
		_bzl = (short)(adffffadfa ^ _tempKey);

		if(eCount > 0) 
		{
//			Time.timeScale = 0;
			Debug.LogError("errorCount : " + eCount);
//			Application.Quit();
		}		
		
	}
	

	private int _count = 0;
	// Update is called once per frame
	void FixedUpdate()
	{
		if(Time.timeScale == 0)
		{

			if(fdsfas != (short)(_zl ^ _tempKey))
			{
				++eCount;
			}
			
			if(dsfafasfassfd != (short)(_zl2 ^ _tempKey))
			{
				++eCount;
			}
			
			if(adffffadfa != (short)(_bzl ^ _tempKey))
			{
				++eCount;
			}


			fdsfas = (short)UnityEngine.Random.Range(1,500);
			dsfafasfassfd = (short)UnityEngine.Random.Range(1,200);
			adffffadfa = (short)UnityEngine.Random.Range(1000,4000);

			_tempKey = (short)UnityEngine.Random.Range(3,10);
			_zl = (short)(fdsfas ^ _tempKey);
			_zl2 = (short)(dsfafasfassfd ^ _tempKey);
			_bzl = (short)(adffffadfa ^ _tempKey);

			if(eCount > 0) 
			{
				Time.timeScale = 0;
//				Debug.LogError(errorCount);
			}
		}
	}
}







public struct Xint
{

	public short _zl;
	public short _zl2;
	public int _value;
	public int _eValue;

//	public int fakeValue;

	public Xint(int v)
	{
		_zl = 1000;
		_zl2 = 1000;
		_value = 0;
		_eValue = 0;
//		fakeValue = 0;

		Set(v);
	}
	public void Set(int v){
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;
		
		_value = v ^ _zl;
		_eValue = _value ^ _zl2;

//		fakeValue = v;
	}
	//private int _returnValue;
	//private int _returnKey;
	public int Get(){
		if( (_eValue ^ _zl2) != _value)
		{
			++Dkaghghk.eCount;
		}
//		else if( (_value^_key) != fakeValue)
//		{
//			++SecurityNumber.errorCount;
//		}
		
		Set(_value^_zl);

		return _value^_zl;
	}
	public static implicit operator Xint(int v)
	{
		return new Xint(v);
	}
	
	
	public static implicit operator int(Xint sValue)
	{	
			return sValue.Get();
	}



	public override string ToString (){return string.Format (""+(int)this);}
	public static Xint operator ++(Xint s1){return s1+=1;}
	public static Xint operator --(Xint s1){return s1-=1;}
	

	public static bool operator ==(Xint s1,Xint s2){ return s1.Get() == s2.Get(); }
	public static bool operator !=(Xint s1,Xint s2){ return s1.Get() != s2.Get(); }
	public static bool operator ==(int s1,Xint s2){ return s1 == s2.Get(); }
	public static bool operator !=(int s1,Xint s2){ return s1 != s2.Get(); }
	public static bool operator ==(Xint s1,int s2){ return s1.Get() == s2; }
	public static bool operator !=(Xint s1,int s2){ return s1.Get() != s2; }
	
	
	
	public static bool operator < (Xint s1,Xint s2){ return s1.Get() < s2.Get(); }
	public static bool operator > (Xint s1,Xint s2){ return s1.Get() > s2.Get(); }
	public static bool operator < (int s1,Xint s2){ return s1 < s2.Get(); }
	public static bool operator > (int s1,Xint s2){ return s1 > s2.Get(); }
	public static bool operator < (Xint s1,int s2){ return s1.Get() < s2; }
	public static bool operator > (Xint s1,int s2){ return s1.Get() > s2; }
	
	public static bool operator <= (Xint s1,Xint s2){ return s1.Get() <= s2.Get(); }
	public static bool operator >= (Xint s1,Xint s2){ return s1.Get() >= s2.Get(); }
	public static bool operator <= (int s1,Xint s2){ return s1 <= s2.Get(); }
	public static bool operator >= (int s1,Xint s2){ return s1 >= s2.Get(); }
	public static bool operator <= (Xint s1,int s2){ return s1.Get() <= s2; }
	public static bool operator >= (Xint s1,int s2){ return s1.Get() >= s2; }


	public int CompareTo(Xint sValue)
	{
		if( (_value - _zl) == (sValue._value - sValue._zl) )
		{
			return 0;
		}
		else if( (_value - _zl) <= (sValue._value - sValue._zl) )
		{
			return -1;
		}
		
		return 1;
	}


}


public struct Xfloat
{

	public short _zl;
	public short _zl2;
	public long _value;
	public long _eValue;

	public const long FACTOR = 1 << IFloat.SHIFT_AMOUNT;

	public Xfloat(int v)
	{
		_zl = 1000;
		_zl2 = 1000;
		_value = 0;
		_eValue = 0;

		Set(v);
	}

	public Xfloat(float v)
	{
		_zl = 1000;
		_zl2 = 1000;
		_value = 0;
		_eValue = 0;

		Set(v);
	}


	public Xfloat(IFloat v)
	{
		_zl = 1000;
		_zl2 = 1000;
		_value = 0;
		_eValue = 0;

		Set(v);
	}


	public void Set(Xfloat v){
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;;
		
		_value = v.GetCompareValue() + _zl;
		_eValue = _value + _zl2;
	}


	public void Set(IFloat v){
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;;
		
		_value = v.Value + _zl;
		_eValue = _value + _zl2;
	}


	
	public void Set(float v){
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;;

		_value = (long)Math.Round(v * FACTOR) + _zl;

		_eValue = _value + _zl2;

	}


	public void Set(int v){
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;;
		
		_value = ((long)v << IFloat.SHIFT_AMOUNT) + _zl;
		
		_eValue = _value + _zl2;

	}


	public void Set(Xint v){
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;;
		
		_value = ((long)v.Get() << IFloat.SHIFT_AMOUNT) + _zl;
		
		_eValue = _value + _zl2;
		
	}



	public long Value
	{
		set
		{
			_zl = Dkaghghk.fdsfas;
			_zl2 = Dkaghghk.dsfafasfassfd;;
			
			_value = value + _zl;
			
			_eValue = _value + _zl2;
		}
		get
		{
			if( (_eValue - _zl2) != _value)
			{
				++Dkaghghk.eCount;
			}
			
			long returnValue = _value - _zl;
			
			ApplyChangeKey();
			
			return returnValue;
		}
	}



	public static implicit operator Xfloat(float v){
		return new Xfloat(v);
	}

	public static implicit operator Xfloat(IFloat v){
		return new Xfloat(v);
	}

	public static implicit operator Xfloat(int v){
		return new Xfloat(v);
	}

	public static implicit operator Xfloat(Xint v){
		return new Xfloat(v.Get());
	}

//	public static implicit operator int(Xfloat src)
//	{
//		return (src.Value >> SHIFT_AMOUNT);
//	}




	public float Get(){
		if( (_eValue - _zl2) != _value)
		{
			++Dkaghghk.eCount;
		}

		float returnValue = (float)((double)(_value - _zl) / FACTOR);

		ApplyChangeKey();
		return returnValue;
	}


	public void ApplyChangeKey()
	{
		long originalValue = _value - _zl;

		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;;
		
		_value = originalValue + _zl;
		
		_eValue = _value + _zl2;

	}


	public long GetCompareValue()
	{
		if( (_eValue - _zl2) != _value)
		{
			++Dkaghghk.eCount;
		}

		long returnValue = _value - _zl;

		ApplyChangeKey();

		return returnValue;
	}


	public static implicit operator float(Xfloat sValue)
	{	
		return sValue.Get();
	}
	
	public string ToShortString(int underPoint = 0)
	{
		return (getShortFloat ((float)this , underPoint) + "");
	}
	
	public float getShortFloat(float num, int underPoint = 0)
	{
		if(underPoint == 0)
		{
			num = UnityEngine.Mathf.Round(num);
		}
		else
		{
			num = num * (float)(Math.Pow(  10 ,  underPoint ) );
			num = UnityEngine.Mathf.Round(num);
			num = num /  (float)(Math.Pow(  10 ,  underPoint ) );
		}
		
		return num;
	}	


	public int AsInt()
	{
		return (int)(Value >> IFloat.SHIFT_AMOUNT);
	}


	public float AsFloat()
	{
		return (float)((double)Value / FACTOR);
	}


	static long GetCompareValue(int intValue)
	{
		return ((long)intValue << IFloat.SHIFT_AMOUNT);
	}

	static long GetCompareValue(float floatValue)
	{
		return (long)Math.Round(floatValue * FACTOR);
	}


	static long GetCompareValue(Xint floatValue)
	{
		return (long)floatValue.Get() << IFloat.SHIFT_AMOUNT;
	}


	public override string ToString (){return string.Format (""+(float)this);}
	public static Xfloat operator ++(Xfloat s1){return s1+=1;}
	public static Xfloat operator --(Xfloat s1){return s1-=1;}
	
	public static bool operator ==(Xfloat s1,Xfloat s2){ return s1.GetCompareValue() == s2.GetCompareValue(); }
	public static bool operator !=(Xfloat s1,Xfloat s2){ return s1.GetCompareValue() != s2.GetCompareValue(); }
	public static bool operator ==(float s1,Xfloat s2){ return GetCompareValue(s1) == s2.GetCompareValue(); }
	public static bool operator !=(float s1,Xfloat s2){ return GetCompareValue(s1) != s2.GetCompareValue(); }
	public static bool operator ==(Xfloat s1,float s2){ return s1.GetCompareValue() == GetCompareValue(s2); }
	public static bool operator !=(Xfloat s1,float s2){ return s1.GetCompareValue() != GetCompareValue(s2); }
	public static bool operator ==(int s1,Xfloat s2){ return GetCompareValue(s1) == s2.GetCompareValue(); }
	public static bool operator !=(int s1,Xfloat s2){ return GetCompareValue(s1) != s2.GetCompareValue(); }
	public static bool operator ==(Xfloat s1,int s2){ return s1.GetCompareValue() == GetCompareValue(s2); }
	public static bool operator !=(Xfloat s1,int s2){ return s1.GetCompareValue() != GetCompareValue(s2); }	
	public static bool operator ==(IFloat s1,Xfloat s2){ return s1.Value == s2.GetCompareValue(); }
	public static bool operator !=(IFloat s1,Xfloat s2){ return s1.Value != s2.GetCompareValue(); }
	public static bool operator ==(Xfloat s1,IFloat s2){ return s1.GetCompareValue() == s2.Value; }
	public static bool operator !=(Xfloat s1,IFloat s2){ return s1.GetCompareValue() != s2.Value; }


	
	public static bool operator < (Xfloat s1,Xfloat s2){ return s1.GetCompareValue() < s2.GetCompareValue(); }
	public static bool operator > (Xfloat s1,Xfloat s2){ return s1.GetCompareValue() > s2.GetCompareValue(); }
	public static bool operator < (float s1,Xfloat s2){ return GetCompareValue(s1) < s2.GetCompareValue(); }
	public static bool operator > (float s1,Xfloat s2){ return GetCompareValue(s1) > s2.GetCompareValue(); }
	public static bool operator < (Xfloat s1,float s2){ return s1.GetCompareValue() < GetCompareValue(s2); }
	public static bool operator > (Xfloat s1,float s2){ return s1.GetCompareValue() > GetCompareValue(s2); }
	public static bool operator < (int s1,Xfloat s2){ return GetCompareValue(s1) < s2.GetCompareValue(); }
	public static bool operator > (int s1,Xfloat s2){ return GetCompareValue(s1) > s2.GetCompareValue(); }
	public static bool operator < (Xfloat s1,int s2){ return s1.GetCompareValue() < GetCompareValue(s2); }
	public static bool operator > (Xfloat s1,int s2){ return s1.GetCompareValue() > GetCompareValue(s2); }
	
	public static bool operator <= (Xfloat s1,Xfloat s2){ return s1.GetCompareValue() <= s2.GetCompareValue(); }
	public static bool operator >= (Xfloat s1,Xfloat s2){ return s1.GetCompareValue() >= s2.GetCompareValue(); }

	public static bool operator <= (float s1,Xfloat s2){ return GetCompareValue(s1) <= s2.GetCompareValue(); }
	public static bool operator >= (float s1,Xfloat s2){ return GetCompareValue(s1) >= s2.GetCompareValue(); }

	public static bool operator <= (Xfloat s1,float s2){ return s1.GetCompareValue() <= GetCompareValue(s2); }
	public static bool operator >= (Xfloat s1,float s2){ return s1.GetCompareValue() >= GetCompareValue(s2); }

	public static bool operator <= (int s1,Xfloat s2){ return GetCompareValue(s1) <= s2.GetCompareValue(); }
	public static bool operator >= (int s1,Xfloat s2){ return GetCompareValue(s1) >= s2.GetCompareValue(); }

	public static bool operator <= (Xfloat s1,int s2){ return s1.GetCompareValue() <= GetCompareValue(s2); }
	public static bool operator >= (Xfloat s1,int s2){ return s1.GetCompareValue() >= GetCompareValue(s2); }



	public static bool operator <= (Xfloat s1,IFloat s2){ return s1.GetCompareValue() <= s2.Value; }
	public static bool operator >= (Xfloat s1,IFloat s2){ return s1.GetCompareValue() >= s2.Value; }

	public static bool operator <= (IFloat s1,Xfloat s2){ return s1.Value <= s2.GetCompareValue(); }
	public static bool operator >= (IFloat s1,Xfloat s2){ return s1.Value >= s2.GetCompareValue(); }


	public static bool operator < (Xfloat s1,IFloat s2){ return s1.GetCompareValue() < s2.Value; }
	public static bool operator > (Xfloat s1,IFloat s2){ return s1.GetCompareValue() > s2.Value; }
	
	public static bool operator < (IFloat s1,Xfloat s2){ return s1.Value < s2.GetCompareValue(); }
	public static bool operator > (IFloat s1,Xfloat s2){ return s1.Value > s2.GetCompareValue(); }



	public static bool operator <= (Xfloat s1,Xint s2){ return s1.GetCompareValue() <= GetCompareValue(s2); }
	public static bool operator >= (Xfloat s1,Xint s2){ return s1.GetCompareValue() >= GetCompareValue(s2); }
	
	public static bool operator <= (Xint s1,Xfloat s2){ return GetCompareValue(s1) <= s2.GetCompareValue(); }
	public static bool operator >= (Xint s1,Xfloat s2){ return GetCompareValue(s1) >= s2.GetCompareValue(); }


	public static bool operator < (Xfloat s1,Xint s2){ return s1.GetCompareValue() < GetCompareValue(s2); }
	public static bool operator > (Xfloat s1,Xint s2){ return s1.GetCompareValue() > GetCompareValue(s2); }
	
	public static bool operator < (Xint s1,Xfloat s2){ return GetCompareValue(s1) < s2.GetCompareValue(); }
	public static bool operator > (Xint s1,Xfloat s2){ return GetCompareValue(s1) > s2.GetCompareValue(); }









	#region *
	public static Xfloat operator *(Xfloat one, Xfloat other)
	{
		one.Value = (one.Value * other.Value)  >> IFloat.SHIFT_AMOUNT;
		return one;
	}

	public static Xfloat operator *(Xfloat one, int other)
	{
		one.Value = (one.Value * ((long)other << IFloat.SHIFT_AMOUNT))  >> IFloat.SHIFT_AMOUNT;
		return one;
	}
	
	public static Xfloat operator *(int one, Xfloat other)
	{
		other.Value = (((long)one << IFloat.SHIFT_AMOUNT) * other.Value) >> IFloat.SHIFT_AMOUNT;
		return other;
	}
	
	
	public static Xfloat operator *(Xfloat one, float other)
	{
		one.Value = (one.Value * GetCompareValue(other)) >> IFloat.SHIFT_AMOUNT;
		return one;
	}
	
	public static Xfloat operator *(float one, Xfloat other)
	{
		other.Value = (GetCompareValue(one) * other.Value)  >> IFloat.SHIFT_AMOUNT;
		return other;
	}
	
	
	
	
	#endregion
	
	#region /
	public static Xfloat operator /(Xfloat one, Xfloat other)
	{
		if(other.Value == 0) return 0;
		one.Value = (one.Value  << IFloat.SHIFT_AMOUNT) / (other.Value);
		return one;
	}

	
	public static Xfloat operator /(Xfloat one, int other)
	{
		if(other == 0) return 0;
		one.Value = (one.Value  / (long) other ) ;
		return one;
	}
	
	public static Xfloat operator /(int one, Xfloat other)
	{
		if(other.Value == 0) return 0;
		other.Value = ( (long)one  << IFloat.SHIFT_AMOUNT << IFloat.SHIFT_AMOUNT) / (other.Value);
		return other;
	}
	
	
	
	public static Xfloat operator /(Xfloat one, float other)
	{
		if(other == 0) return 0;
		one.Value = (one.Value << IFloat.SHIFT_AMOUNT) / GetCompareValue(other) ;
		return one;
	}
	
	public static Xfloat operator /(float one, Xfloat other)
	{
		if(other.Value == 0) return 0;
		other.Value = ( GetCompareValue(one)  << IFloat.SHIFT_AMOUNT) / (other.Value);
		return other;
	}
	
	
	#endregion
	
	#region -
	public static Xfloat operator -(Xfloat one, Xfloat other)
	{
		one.Value = one.Value - other.Value;
		return one;
	}
	
	public static Xfloat operator -(Xfloat one, int other)
	{
		one.Value = one.Value - GetCompareValue(other);
		return one;
	}
	
	public static Xfloat operator -(int one, Xfloat other)
	{
		other.Value = GetCompareValue(one) - other.Value;
		return other;
	}


	public static Xfloat operator -(Xfloat one, Xint other)
	{
		one.Value = one.Value - GetCompareValue(other);
		return one;
	}
	
	public static Xfloat operator -(Xint one, Xfloat other)
	{
		other.Value = GetCompareValue(one) - other.Value;
		return other;
	}


	
	public static Xfloat operator -(Xfloat one, float other)
	{
		one.Value = one.Value - GetCompareValue(other);
		return one;
	}
	
	public static Xfloat operator -(float one, Xfloat other)
	{
		other.Value = GetCompareValue(one) - other.Value;
		return other;
	}
	
	
	#endregion
	
	#region +
	public static Xfloat operator +(Xfloat one, Xfloat other)
	{
		one.Value = one.Value + other.Value;
		return one;
	}
	
	public static Xfloat operator +(Xfloat one, int other)
	{
		one.Value = one.Value + GetCompareValue(other);
		return one;
	}
	
	public static Xfloat operator +(int one, Xfloat other)
	{
		other.Value = GetCompareValue(one) + other.Value;
		return other;
	}


	public static Xfloat operator +(Xfloat one, Xint other)
	{
		one.Value = one.Value + GetCompareValue(other);
		return one;
	}
	
	public static Xfloat operator +(Xint one, Xfloat other)
	{
		other.Value = GetCompareValue(one) + other.Value;
		return other;
	}

	
	public static Xfloat operator +(Xfloat one, float other)
	{
		one.Value = one.Value + GetCompareValue(other);
		return one;
	}
	
	public static Xfloat operator +(float one, Xfloat other)
	{
		other.Value = GetCompareValue(one) + other.Value;
		return other;
	}
	#endregion






	#region %
	public static Xfloat operator %(Xfloat one, Xfloat other)
	{
		one.Value = (one.Value) % (other.Value);
		return one;
	}
	
	public static Xfloat operator %(Xfloat one, int divisor)
	{
		return one % (Xfloat)divisor;
	}
	
	public static Xfloat operator %(int divisor, Xfloat one)
	{
		return (Xfloat)divisor % one;
	}
	#endregion


	#region <<
	public static Xfloat operator <<(Xfloat one, int amount)
	{
		one.Value = one.Value << amount;
		return one;
	}
	#endregion
	
	#region >>
	public static Xfloat operator >>(Xfloat one, int amount)
	{
		one.Value = one.Value >> amount;
		return one;
	}
	#endregion




	#region greatEqualThan

	public static bool greatEqualThan(Xfloat a, Xfloat b)
	{
		return (a >= b);
	}

	public static bool greatEqualThan(Xfloat a, IFloat b)
	{
		return ( a.GetCompareValue() >= b.Value  );	
	}
	
	public static bool greatEqualThan(IFloat a, Xfloat b)
	{
		return ( a.Value >= b.GetCompareValue()  );	
	}

	public static bool greatEqualThan(IFloat a, IFloat b)
	{
		return (a.Value >= b.Value);
	}


	public static bool greatEqualThan(float a, float b)
	{
		return ((long)Math.Round(a * FACTOR) >= (long)Math.Round(b * FACTOR));
	}


	public static bool greatEqualThan(Xfloat a, Xint b)
	{
		return (a >= b);
	}

	public static bool greatEqualThan(Xint a, Xfloat b)
	{
		return (a >= b);
	}

	public static bool greatEqualThan(float a, int b)
	{
		return ((long)Math.Round(a * FACTOR) >= ((long)b << IFloat.SHIFT_AMOUNT));
	}

	public static bool greatEqualThan(int a, float b)
	{
		return ( (long)(a << IFloat.SHIFT_AMOUNT) >= (long)Math.Round(b * FACTOR));
	}


	public static bool greatEqualThan(float a, Xint b)
	{
		return ((long)Math.Round(a * FACTOR) >= ((long)b.Get() << IFloat.SHIFT_AMOUNT));
	}


	public static bool greatEqualThan(Xint a, float b)
	{
		return ((long)(a.Get() << IFloat.SHIFT_AMOUNT) >= (long)Math.Round(b * FACTOR));
	}


	public static bool greatEqualThan(IFloat a, int b)
	{
		return (a.Value >= ((long)b << IFloat.SHIFT_AMOUNT));
	}

	public static bool greatEqualThan(int a, IFloat b)
	{
		return (((long)a << IFloat.SHIFT_AMOUNT) >= b.Value);
	}


	public static bool greatEqualThan(IFloat a, Xint b)
	{
		return (a.Value >= ((long)b.Get() << IFloat.SHIFT_AMOUNT));
	}
	
	public static bool greatEqualThan(Xint a, IFloat b)
	{
		return (((long)a.Get() << IFloat.SHIFT_AMOUNT) >= b.Value);
	}


	public static bool greatEqualThan(float a, Xfloat b)
	{
		return ((long)Math.Round(a * FACTOR) >= b.GetCompareValue());
	}

	public static bool greatEqualThan(Xfloat a, float b)
	{
		return (a.GetCompareValue() >= (long)Math.Round(b * FACTOR));
	}


	public static bool greatEqualThan(float a, IFloat b)
	{
		return ((long)Math.Round(a * FACTOR) >= b.Value);
	}
	
	public static bool greatEqualThan(IFloat a, float b)
	{
		return (a.Value >= (long)Math.Round(b * FACTOR));
	}

	#endregion


	#region greaterThan

	public static bool greaterThan(Xfloat a, Xfloat b)
	{
		return (a > b);
	}

	public static bool greaterThan(Xfloat a, IFloat b)
	{
		return ( a.GetCompareValue() > b.Value  );	
	}
	
	public static bool greaterThan(IFloat a, Xfloat b)
	{
		return ( a.Value > b.GetCompareValue()  );	
	}

	public static bool greaterThan(IFloat a, IFloat b)
	{
		return (a.Value > b.Value);
	}


	public static bool greaterThan(float a, float b)
	{
		return ((long)Math.Round(a * FACTOR) > (long)Math.Round(b * FACTOR));
	}
	
	public static bool greaterThan(Xfloat a, Xint b)
	{
		return (a > b);
	}
	
	public static bool greaterThan(Xint a, Xfloat b)
	{
		return (a > b);
	}


	public static bool greaterThan(float a, int b)
	{
		return ((long)Math.Round(a * FACTOR) > ((long)b << IFloat.SHIFT_AMOUNT));
	}

	public static bool greaterThan(int a, float b)
	{
		return ( ((long)a << IFloat.SHIFT_AMOUNT) > (long)Math.Round(b * FACTOR));	
	}




	public static bool greaterThan(float a, Xint b)
	{
		return ((long)Math.Round(a * FACTOR) > ((long)b.Get() << IFloat.SHIFT_AMOUNT));
	}

	public static bool greaterThan(Xint a, float b)
	{
		return (((long)a.Get() << IFloat.SHIFT_AMOUNT) > (long)Math.Round(b * FACTOR));
	}


	public static bool greaterThan(IFloat a, int b)
	{
		return (a.Value > ((long)b << IFloat.SHIFT_AMOUNT));	
	}

	public static bool greaterThan(int a, IFloat b)
	{
		return (((long)a << IFloat.SHIFT_AMOUNT) > b.Value );	
	}

	public static bool greaterThan(IFloat a, Xint b)
	{
		return (a.Value > ((long)b.Get() << IFloat.SHIFT_AMOUNT));	
	}
	
	public static bool greaterThan(Xint a, IFloat b)
	{
		return (((long)a.Get() << IFloat.SHIFT_AMOUNT) > b.Value );	
	}


	public static bool greaterThan(IFloat a, float b)
	{
		return (a.Value > (long)Math.Round(b * FACTOR));	
	}
	
	public static bool greaterThan(float a, IFloat b)
	{
		return ((long)Math.Round(a * FACTOR) > b.Value );	
	}



	public static bool greaterThan(float a, Xfloat b)
	{
		return ((long)Math.Round(a * FACTOR) > b.GetCompareValue());
	}


	public static bool greaterThan(Xfloat a, float b)
	{
		return ( a.GetCompareValue() > (long)Math.Round(b * FACTOR) );
	}


	#endregion


	#region lessEqualThan


	public static bool lessEqualThan(Xfloat a, Xfloat b)
	{
		return (a <= b);
	}


	public static bool lessEqualThan(Xfloat a, IFloat b)
	{
		return ( a.GetCompareValue() <= b.Value  );	
	}
	
	public static bool lessEqualThan(IFloat a, Xfloat b)
	{
		return ( a.Value <= b.GetCompareValue()  );	
	}

	public static bool lessEqualThan(IFloat a, IFloat b)
	{
		return (a.Value <= b.Value);
	}
	
	public static bool lessEqualThan(float a, float b)
	{
		return ((long)Math.Round(a * FACTOR) <=(long)Math.Round(b * FACTOR));
	}
	
	
	public static bool lessEqualThan(Xfloat a, Xint b)
	{
		return (a <= b);
	}
	
	public static bool lessEqualThan(Xint a, Xfloat b)
	{
		return (a <= b);
	}

	
	public static bool lessEqualThan(float a, int b)
	{
		return ((long)Math.Round(a * FACTOR) <= ((long)b << IFloat.SHIFT_AMOUNT));
	}

	public static bool lessEqualThan(int a, float b)
	{
		return ( ((long)a << IFloat.SHIFT_AMOUNT) <= (long)Math.Round(b * FACTOR));	
	}


	
	public static bool lessEqualThan(float a, Xint b)
	{
		return ((long)Math.Round(a * FACTOR) <= ((long)b.Get() << IFloat.SHIFT_AMOUNT));
	}

	public static bool lessEqualThan(Xint a, float b)
	{
		return (((long)a.Get() << IFloat.SHIFT_AMOUNT) <= (long)Math.Round(b * FACTOR));
	}


	public static bool lessEqualThan(IFloat a, int b)
	{
		return (a.Value <= ((long)b << IFloat.SHIFT_AMOUNT));
	}


	public static bool lessEqualThan(int a, IFloat b)
	{
		return (((long)a << IFloat.SHIFT_AMOUNT) <= b.Value );
	}

	public static bool lessEqualThan(IFloat a, Xint b)
	{
		return (a.Value <= ((long)b.Get() << IFloat.SHIFT_AMOUNT));
	}
	
	
	public static bool lessEqualThan(Xint a, IFloat b)
	{
		return (((long)a.Get() << IFloat.SHIFT_AMOUNT) <= b.Value );
	}

	public static bool lessEqualThan(float a, Xfloat b)
	{
		return ((long)Math.Round(a * FACTOR) <= b.GetCompareValue());
	}


	public static bool lessEqualThan(Xfloat a, float b)
	{
		return ( a.GetCompareValue() <= (long)Math.Round(b * FACTOR));
	}


	public static bool lessEqualThan(float a, IFloat b)
	{
		return ((long)Math.Round(a * FACTOR) <= b.Value);
	}
	
	
	public static bool lessEqualThan(IFloat a, float b)
	{
		return ( a.Value <= (long)Math.Round(b * FACTOR));
	}

	#endregion
	
	
	
	#region lessThan

	public static bool lessThan(Xfloat a, Xfloat b)
	{
		return (a < b);
	}

	public static bool lessThan(Xfloat a, IFloat b)
	{
		return ( a.GetCompareValue() < b.Value  );	
	}
	
	
	public static bool lessThan(IFloat a, Xfloat b)
	{
		return ( a.Value < b.GetCompareValue()  );	
	}


	public static bool lessThan(IFloat a, IFloat b)
	{
		return (a.Value < b.Value);
	}
	
	public static bool lessThan(float a, float b)
	{
		return ((long)Math.Round(a * FACTOR) < (long)Math.Round(b * FACTOR));
	}
	
	
	public static bool lessThan(Xfloat a, Xint b)
	{
		return (a < b);
	}
	
	
	public static bool lessThan(Xint a, Xfloat b)
	{
		return (a < b);
	}

	
	public static bool lessThan(float a, int b)
	{
		return ((long)Math.Round(a * FACTOR) < ((long)b << IFloat.SHIFT_AMOUNT));
	}

	public static bool lessThan(int a, float b)
	{
		return ( ((long)a << IFloat.SHIFT_AMOUNT) < (long)Math.Round(b * FACTOR));	
	}



	
	public static bool lessThan(float a, Xint b)
	{
		return ((long)Math.Round(a * FACTOR) < ((long)b.Get() << IFloat.SHIFT_AMOUNT));
	}


	public static bool lessThan(Xint a, float b)
	{
		return (((long)a.Get() << IFloat.SHIFT_AMOUNT) < (long)Math.Round(b * FACTOR));
	}


	
	public static bool lessThan(IFloat a, int b)
	{
		return (a.Value < ((long)b << IFloat.SHIFT_AMOUNT));
	}


	public static bool lessThan(int a, IFloat b)
	{
		return (((long)a << IFloat.SHIFT_AMOUNT) < b.Value );
	}


	public static bool lessThan(IFloat a, Xint b)
	{
		return (a.Value < ((long)b.Get() << IFloat.SHIFT_AMOUNT));
	}
	
	
	public static bool lessThan(Xint a, IFloat b)
	{
		return (((long)a.Get() * FACTOR) < b.Value );
	}

	public static bool lessThan(float a, Xfloat b)
	{
		return ((long)Math.Round(a * FACTOR) < b.GetCompareValue());
	}
	
	public static bool lessThan(Xfloat a, float b)
	{
		return ( a.GetCompareValue() < (long)Math.Round(b * FACTOR));
	}
	
	
	public static bool lessThan(float a, IFloat b)
	{
		return ((long)Math.Round(a * FACTOR) < b.Value);
	}
	
	public static bool lessThan(IFloat a, float b)
	{
		return ( a.Value < (long)Math.Round(b * FACTOR));
	}
	#endregion









	public int CompareTo(Xfloat sValue)
	{
		if( (_value - _zl) == (sValue._value - sValue._zl) )
		{
			return 0;
		}
		else if( (_value - _zl) <= (sValue._value - sValue._zl) )
		{
			return -1;
		}
		
		return 1;
	}
}



public struct Xbool
{
	public short _zl1;
	public short _zl2;
	public int _value1;
	public int _value2;

	public Xbool(bool v)
	{
		_zl1 = 1000;
		_zl2 = 1010;
		_value1 = 0;
		_value2 = 0;
		
		Set(v);
	}

	public void Set(bool v){

		_zl1 = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;
		
		_value1 = Dkaghghk.adffffadfa ^ _zl1;
		if(v){
			_value2 = (10000-Dkaghghk.adffffadfa) ^ _zl2;
		}else{
			_value2 = (100000-Dkaghghk.adffffadfa) ^ _zl2;
		}
	}

	public bool Get(){	
		
		if((_value1^_zl1)+(_value2^_zl2)==10000)
		{

			Set (true);

			return true;
		}
		else if((_value1^_zl1)+(_value2^_zl2)==100000)
		{

			Set (false);

			return false;
		}
		else
		{
			++Dkaghghk.eCount;
			//PerformanceManager.GetInstance().SetCheater();
			return false;
		}
	}
	
	public static implicit operator Xbool(bool v){
		return new Xbool(v);
	}
	
	//private static bool _returnValue;
	public static implicit operator bool(Xbool sValue)
	{	
		
		return sValue.Get();
	}
	
	public override string ToString (){return string.Format (""+(bool)this);}
	
	public static bool operator ==(Xbool s1,bool s2){
		return s1.Get() == s2; 
	}
	public static bool operator ==(bool s1,Xbool s2){
		return s1 == s2.Get(); 
	}
	public static bool operator ==(Xbool s1,Xbool s2){
		return s1.Get() == s2.Get (); 
	}
	public static bool operator !=(Xbool s1,bool s2){
		return s1.Get() != s2; 
	}
	public static bool operator !=(bool s1,Xbool s2){
		return s1 != s2.Get(); 
	}
	public static bool operator !=(Xbool s1,Xbool s2){
		return s1.Get() != s2.Get (); 
	}
}

















public class Xbool2
{
	static int baseIndex=10;
	public int _zl = 1000;
	public int _zl2 = 1010;
	public int[] _values1;
	public int[] _values2;
	
	//public int _baseKey = 100;
	
	private Xint totalNum = 20;
	
	public Xbool2(bool v)
	{
		
		totalNum = Dkaghghk.securityLevel2_totalIndex;
		baseIndex = Dkaghghk.securityLevel2_baseIndex;
		
		_values1 = new int[totalNum];
		_values2 = new int[totalNum];
		
		Set(v);
	}
	int Set_i;
	public void Set(bool v){
		baseIndex = Dkaghghk.securityLevel2_baseIndex;
		
		//_baseKey = (v)?SecurityNumber.boolKey:-SecurityNumber.boolKey;
		
		
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;
		

		CheckBaseIndex();
		
		_zl = Dkaghghk.fdsfas;
		_zl2 = Dkaghghk.dsfafasfassfd;
		
		_values1[baseIndex] = Dkaghghk.adffffadfa ^ _zl;
		if(v){
			_values2[baseIndex] = (10000-Dkaghghk.adffffadfa) ^ _zl2;
		}else{
			_values2[baseIndex] = (100000-Dkaghghk.adffffadfa) ^ _zl2;
		}
		
		SaveMoreDatas();
	}
	
	private void CheckBaseIndex(){
		if(baseIndex>=_values1.Length){
			baseIndex = _values1.Length-1;
		}
		if(baseIndex>=_values2.Length){
			baseIndex = _values2.Length-1;
		}
	}
	
	private void SaveMoreDatas(){
		int tmp1=0;
		for(Set_i=0;Set_i<_values1.Length;Set_i++){
			if(Set_i!=baseIndex){
				_values1[Set_i] = _values1[baseIndex];
				_values2[Set_i] = _values2[baseIndex];
			}
		}
	}
	//bool Get_ValueReturn;
	public bool Get(){
		CheckBaseIndex();
		
		if(IsIntergrity()==false){
			//PerformanceManager.GetInstance().SetCheater();
		}
		
		if((_values1[baseIndex]^_zl)+(_values2[baseIndex]^_zl2)==10000){
			Set (true);
		}else if((_values1[baseIndex]^_zl)+(_values2[baseIndex]^_zl2)==100000){
			Set (false);
		}else{
			//PerformanceManager.GetInstance().SetCheater();
		}
		
		
		if((_values1[baseIndex]^_zl)+(_values2[baseIndex]^_zl2)==10000){
			return true;
		}else if((_values1[baseIndex]^_zl)+(_values2[baseIndex]^_zl2)==100000){
			return false;
		}else{
			PerformanceManager.GetInstance().SetCheater();
			return false;
		}
		
		/*
		//Get_ValueReturn = false;
		if( (_encValues[baseIndex] ^ _key2) != _values[baseIndex])
		{
			++SecurityNumber.errorCount;
		}
		
		//Get_ValueReturn = ((_values[baseIndex] ^ _key) < 0)?false:true;
		_returnValue = _values[baseIndex];
		_returnKey = _key;
		
		//_baseKey = (Get_ValueReturn)?SecurityNumber.boolKey:-SecurityNumber.boolKey;
		_baseKey = (((_values[baseIndex] ^ _key) < 0)?false:true)?SecurityNumber.boolKey:-SecurityNumber.boolKey;
		_key = SecurityNumber.key;
		_key2 = SecurityNumber.key2;
		
		_values[baseIndex] = _baseKey  ^ _key;
		_encValues[baseIndex] = _values[baseIndex] ^ _key2;
		
		SaveMoreDatas();
		
		//return Get_ValueReturn; 
		return ((_returnValue ^ _returnKey) < 0)?false:true; 
		*/
	}
	int IsIntergrity_num,IsIntergrity_i;
	public bool IsIntergrity(){
		IsIntergrity_num = 0;
		for(IsIntergrity_i=0;IsIntergrity_i<_values1.Length;IsIntergrity_i++){
			if(_values1[baseIndex] == _values1[IsIntergrity_i] && _values2[baseIndex]==_values2[IsIntergrity_i]){
				IsIntergrity_num++;
			}
		}
		if(IsIntergrity_num>0 && totalNum>0 && IsIntergrity_num==totalNum){
			return true;
		}else{
			return false;
		}
	}
	
	public static explicit operator Xbool2(bool v){
		return new Xbool2(v);
	}
	
	public static implicit operator bool(Xbool2 sValue)
	{	
		
		return sValue.Get();
	}
	
	public override string ToString (){return string.Format (""+(bool)this);}
	
	public static bool operator ==(Xbool2 s1,bool s2){ return s1.Get() == s2; }
	public static bool operator ==(bool s1,Xbool2 s2){ return s1 == s2.Get(); }
	public static bool operator ==(Xbool2 s1,Xbool2 s2){	return s1.Get() == s2.Get (); }
	public static bool operator !=(Xbool2 s1,bool s2){ return s1.Get() != s2; }
	public static bool operator !=(bool s1,Xbool2 s2){ return s1 != s2.Get(); }
	public static bool operator !=(Xbool2 s1,Xbool2 s2){ return s1.Get() != s2.Get (); }




}




















public class XString
{
	string _fakeValue;
	byte[] _encValue;

	public XString(string v)
	{
		Set(v);
	}
	
	public void Set(string v)
	{
		_fakeValue = v;
		if(v != null) _encValue = Util.enc(v);
		else _encValue = null;
	}
	
	public string Get()
	{	
		if(_encValue == null)
		{
			return null;
		}

		string returnValue = Util.dec(_encValue);

		if(returnValue != _fakeValue)
		{
			++Dkaghghk.eCount;
		}

		return returnValue;
	}

	
	public static implicit operator XString(string v){
		return new XString(v);
	}
	
	//private static bool _returnValue;
	public static implicit operator string(XString sValue)
	{	
		return sValue.Get();
	}
	
	public override string ToString (){return string.Format (Util.dec(_encValue));}
	
	public static bool operator ==(XString s1,string s2){
		return s1.Get() == s2; 
	}
	public static bool operator ==(string s1,XString s2){
		return s1 == s2.Get(); 
	}
	public static bool operator ==(XString s1,XString s2){
		return s1.Get() == s2.Get (); 
	}



	public static bool operator !=(XString s1,string s2){
		return s1.Get() != s2; 
	}
	public static bool operator !=(string s1,XString s2){
		return s1 != s2.Get(); 
	}
	public static bool operator !=(XString s1,XString s2){
		return s1.Get() != s2.Get (); 
	}
}


