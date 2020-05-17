
using UnityEngine;
using System;

public struct IFloat
{
	public long Value;

	public const int SHIFT_AMOUNT = 12; //12 is 4096

	public const long One = 1 << SHIFT_AMOUNT;
	public const int OneI = 1 << SHIFT_AMOUNT;
	public static IFloat OneF = IFloat.Create(1, true);
	public static IFloat HalfF = IFloat.Create(2048, false);


	public const long FACTOR = 1 << SHIFT_AMOUNT;
	
	public static IFloat n1 = 1;

	public static IFloat n2 = 2;

	public static IFloat dot004 = IFloat.Create(16,false);

	public static IFloat dot01 = IFloat.Create(41,false);
	public static IFloat dot02 = IFloat.Create(82,false);
	public static IFloat dot05 = IFloat.Create(205,false);
	public static IFloat dot1 = IFloat.Create(410,false);
	public static IFloat dot11 = IFloat.Create(451,false);
	public static IFloat dot15 = IFloat.Create(614,false);
	public static IFloat dot2 = IFloat.Create(819,false);
	public static IFloat dot3 = IFloat.Create(1229,false);
	public static IFloat dot35 = IFloat.Create(1434,false);
	public static IFloat dot4 = IFloat.Create(1638,false);
	public static IFloat dot5 = IFloat.Create(2048,false);
	public static IFloat dot55 = IFloat.Create(2253,false);
	public static IFloat dot6 = IFloat.Create(2458,false);
	public static IFloat dot65 = IFloat.Create(2663,false);
	public static IFloat dot7 = IFloat.Create(2867,false);
	public static IFloat dot8 = IFloat.Create(3277,false);
	public static IFloat dot9 = IFloat.Create(3686,false);

	public static IFloat dot998 = IFloat.Create(4088,false);

	public static IFloat n1dot05 = IFloat.Create(4301,false);
	public static IFloat oneDot1 = IFloat.Create(4506,false);
	public static IFloat oneDot2 = IFloat.Create(4915,false);
	public static IFloat oneDot3 = IFloat.Create(5325,false);
	public static IFloat oneDot4 = IFloat.Create(5734,false);
	public static IFloat oneDot5 = IFloat.Create(6144,false);
	public static IFloat twoDot2 = IFloat.Create(9011,false);
	public static IFloat twoDot5 = IFloat.Create(10240,false);

	public static IFloat n3 = 3;

	public static IFloat n10 = 10;
	public static IFloat n100 = 100;
	public static IFloat n1000 = 1000;

	public static IFloat Zero = 0;

	#region Constructors
	// Float Constructor
	public IFloat(float FloatValue)
	{
		Value = (long)Math.Round((double)FloatValue * FACTOR);
	}

	public IFloat(int IntValue)
	{
		Value = (long)IntValue << SHIFT_AMOUNT;
	}

	public IFloat(Xfloat xFloatValue)
	{
		Value = xFloatValue.GetCompareValue();
	}

	public IFloat(IFloat FloatValue)
	{

		Value = FloatValue.Value;
	}

	public void Set(Xfloat xFloatValue)
	{
		Value = xFloatValue.GetCompareValue();
	}

	public void Set(float FloatValue)
	{
		Value = (long)Math.Round(FloatValue * FACTOR);
	}

	public void Set(int IntValue)
	{
		Value = (long)IntValue << SHIFT_AMOUNT;
	}

	public void Set(IFloat IFloatValue)
	{
		Value = IFloatValue.Value;
	}

	public static IFloat Create(Xfloat FloatValue)
	{
		IFloat iFloat;
		iFloat.Value = FloatValue.GetCompareValue();
		return iFloat;
	}

	public static IFloat Create(float FloatValue)
	{
		IFloat iFloat;
		iFloat.Value = (long)Math.Round(FloatValue * FACTOR);
		return iFloat;
	}
	
	// Integer Constructor
	public static IFloat Create(int IntValue)
	{
		IFloat iFloat;
		iFloat.Value = (long)IntValue << SHIFT_AMOUNT;
		return iFloat;
	}


	public static IFloat Create(long StartingValue, bool UseMultiple)
	{
		IFloat IFloat;
		IFloat.Value = StartingValue;
		if (UseMultiple)
		{
			IFloat.Value = IFloat.Value << SHIFT_AMOUNT;
		}
			
		return IFloat;
	}



	#endregion
	
	#region Values

	public float AsFloat()
	{
		return (float)((double)Value / FACTOR);
	}

	public int AsInt()
	{
		return (int)(Value >> SHIFT_AMOUNT);
	}

	#endregion


	public IFloat Inverse
	{
		get { return IFloat.Create(-this.Value, false); }
	}



	#region FromParts
	/// <summary>
	/// Create a fixed-int number from parts.  For example, to create 1.5 pass in 1 and 500.
	/// </summary>
	/// <param name="PreDecimal">The number above the decimal.  For 1.5, this would be 1.</param>
	/// <param name="PostDecimal">The number below the decimal, to three digits.  
	/// For 1.5, this would be 500. For 1.005, this would be 5.</param>
	/// <returns>A fixed-int representation of the number parts</returns>
	public static IFloat FromParts(int PreDecimal, int PostDecimal)
	{
		IFloat f = IFloat.Create(PreDecimal, true);
		if (PostDecimal != 0)
			f.Value += (IFloat.Create(PostDecimal) / 1000).Value;
		
		return f;
	}
	#endregion





	#region Operators
	
	#region *
	public static IFloat operator *(IFloat one, IFloat other)
	{
		one.Value = (one.Value * other.Value) >> SHIFT_AMOUNT;
		return one;
	}


	public static IFloat operator *(IFloat aIFloat, Xfloat bXfloat)
	{
		aIFloat.Value = (aIFloat.Value * bXfloat.GetCompareValue()) >> SHIFT_AMOUNT;
		return aIFloat;
	}
	
	public static IFloat operator *(Xfloat aXfloat, IFloat bIFloat)
	{
		bIFloat.Value = (aXfloat.GetCompareValue() * bIFloat.Value) >> SHIFT_AMOUNT;
		return bIFloat;
	}


	public static IFloat operator *(IFloat aIFloat, int bInt)
	{
		return aIFloat * (IFloat)bInt;
	}
	
	public static IFloat operator *(int aInt, IFloat bIFloat)
	{
		return bIFloat * (IFloat)aInt;
	}



	public static IFloat operator *(IFloat aIFloat, Xint bInt)
	{
		return aIFloat * (IFloat)bInt.Get();
	}
	
	public static IFloat operator *(Xint aInt, IFloat bIFloat)
	{
		return bIFloat * (IFloat)aInt.Get();
	}





	public static IFloat operator *(IFloat aIFloat, float bfloat)
	{
		return aIFloat * (IFloat)bfloat;
	}
	
	public static IFloat operator *(float afloat, IFloat bIFloat)
	{
		return (IFloat)afloat * bIFloat;
	}




	#endregion
	
	#region /
	public static IFloat operator /(IFloat one, IFloat other)
	{
		if(other.Value == 0) return 0;
		one.Value = (one.Value << SHIFT_AMOUNT) / (other.Value);
		return one;
	}

	public static IFloat operator /(IFloat one, Xfloat other)
	{
		if(other.Value == 0) return 0;
		one.Value = (one.Value << SHIFT_AMOUNT) / (other.GetCompareValue());
		return one;
	}

	public static IFloat operator /(Xfloat one, IFloat other)
	{
		if(other.Value == 0) return 0;
		other.Value = (one.GetCompareValue() << SHIFT_AMOUNT) / (other.Value);
		return other;
	}

	public static IFloat operator /(IFloat one, int other)
	{
		if(other == 0) return 0;
		one.Value = (one.Value ) / ((long)other ) ;
		return one;
	}
	
	public static IFloat operator /(int one, IFloat other)
	{
		if(other.Value == 0) return 0;
		other.Value = ( (long)one << SHIFT_AMOUNT << SHIFT_AMOUNT) / (other.Value);
		return other;
	}


	public static IFloat operator /(Xint one, IFloat other)
	{
		int v = one;
		if(v == 0) return 0;
		other.Value = ( (long)one.Get() << SHIFT_AMOUNT << SHIFT_AMOUNT) / (other.Value);
		return other;
	}


	public static IFloat operator /(IFloat one, float other)
	{
		if(other == 0) return 0;
		one.Value = (one.Value << SHIFT_AMOUNT) / ((IFloat)other).Value ;
		return one;
	}
	
	public static IFloat operator /(float one, IFloat other)
	{
		if(other.Value == 0) return 0;
		other.Value = (  ((IFloat)one).Value << SHIFT_AMOUNT) / (other.Value);
		return other;
	}


	#endregion
	
	#region -
	public static IFloat operator -(IFloat one, IFloat other)
	{
		one.Value = one.Value - other.Value;
		return one;
	}
	
	public static IFloat operator -(IFloat aIFloat, int bInt)
	{
		return aIFloat - (IFloat)bInt;
	}
	
	public static IFloat operator -(int aInt, IFloat bIFloat)
	{
		return (IFloat)aInt - bIFloat;
	}

	public static IFloat operator -(IFloat one, Xfloat other)
	{
		one.Value = one.Value - other.GetCompareValue();
		return one;
	}

	public static IFloat operator -(Xfloat one, IFloat other)
	{
		other.Value = one.GetCompareValue() - other.Value;
		return other;
	}

	public static IFloat operator -(IFloat aIFloat, float bInt)
	{
		return aIFloat - (IFloat)bInt;
	}
	
	public static IFloat operator -(float aInt, IFloat bIFloat)
	{
		return (IFloat)aInt - bIFloat;
	}


	#endregion
	
	#region +
	public static IFloat operator +(IFloat one, IFloat other)
	{
		one.Value = one.Value + other.Value;
		return one;
	}
	
	public static IFloat operator +(IFloat aIFloat, int bInt)
	{
		return aIFloat + (IFloat)bInt;
	}
	
	public static IFloat operator +(int aInt, IFloat bIFloat)
	{
		return (IFloat)aInt + bIFloat;
	}


	public static IFloat operator +(IFloat one, Xfloat other)
	{
		one.Value = one.Value + other.GetCompareValue();
		return one;
	}


	public static IFloat operator +(Xfloat one, IFloat other)
	{
		other.Value = one.GetCompareValue() + other.Value;
		return other;
	}


	public static IFloat operator +(IFloat aIFloat, float bInt)
	{
		return aIFloat + (IFloat)bInt;
	}
	
	public static IFloat operator +(float aInt, IFloat bIFloat)
	{
		return (IFloat)aInt + bIFloat;
	}

	#endregion



	#region %
	public static IFloat operator %(IFloat one, IFloat other)
	{
		one.Value = (one.Value) % (other.Value);
		return one;
	}
	
	public static IFloat operator %(IFloat one, int divisor)
	{
		return one % (IFloat)divisor;
	}
	
	public static IFloat operator %(int divisor, IFloat one)
	{
		return (IFloat)divisor % one;
	}
	#endregion










	#region ==
	public static bool operator ==(IFloat one, IFloat other)
	{
		return one.Value == other.Value;
	}

	public static bool operator ==(float one, IFloat other)
	{
		return ((IFloat)one).Value == other.Value;
	}

	public static bool operator ==(IFloat one, float other)
	{
		return one.Value == ((IFloat)other).Value;
	}


	public static bool operator ==(int one, IFloat other)
	{
		return ((IFloat)one).Value == other.Value;
	}
	
	public static bool operator ==(IFloat one, int other)
	{
		return one.Value == ((IFloat)other).Value;
	}

	#endregion
	
	#region !=
	public static bool operator !=(IFloat one, IFloat other)
	{
		return one.Value != other.Value;
	}

	public static bool operator !=(float one, IFloat other)
	{
		return ((IFloat)one).Value != other.Value;
	}
	
	public static bool operator !=(IFloat one, float other)
	{
		return one.Value != ((IFloat)other).Value;
	}


	public static bool operator !=(int one, IFloat other)
	{
		return ((IFloat)one).Value != other.Value;
	}
	
	public static bool operator !=(IFloat one, int other)
	{
		return one.Value != ((IFloat)other).Value;
	}

	#endregion


	#region >=

	public static bool operator >=(IFloat one, IFloat other)
	{
		return one.Value >= other.Value;
	}

	public static bool operator >=(IFloat one, int other)
	{
		return one.Value >= (long)other << SHIFT_AMOUNT;
	}

	public static bool operator >=(int one, IFloat other)
	{
		return (long)one << SHIFT_AMOUNT >= other.Value;
	}

	public static bool operator >=(float one, IFloat other)
	{
		return ((IFloat)one).Value >= other.Value;
	}
	
	public static bool operator >=(IFloat one, float other)
	{
		return one.Value >= ((IFloat)other).Value;
	}

	#endregion
	
	#region <=
	public static bool operator <=(IFloat one, IFloat other)
	{
		return one.Value <= other.Value;
	}

	public static bool operator <=(IFloat one, int other)
	{
		return one.Value <= (long)other << SHIFT_AMOUNT;
	}
	
	public static bool operator <=(int one, IFloat other)
	{
		return (long)one << SHIFT_AMOUNT <= other.Value;
	}

	public static bool operator <=(float one, IFloat other)
	{
		return ((IFloat)one).Value  <= other.Value;
	}
	
	public static bool operator  <= (IFloat one, float other)
	{
		return one.Value  <= ((IFloat)other).Value;
	}

	#endregion



	#region <
	public static bool operator <(IFloat one, IFloat other)
	{
		return one.Value < other.Value;
	}

	public static bool operator <(IFloat one, int other)
	{
		return one.Value < (long)other << SHIFT_AMOUNT;
	}
	
	public static bool operator <(int one, IFloat other)
	{
		return (long)one << SHIFT_AMOUNT < other.Value;
	}

	public static bool operator <(float one, IFloat other)
	{
		return ((IFloat)one).Value  < other.Value;
	}
	
	public static bool operator < (IFloat one, float other)
	{
		return one.Value  < ((IFloat)other).Value;
	}

	#endregion
	
	#region >
	public static bool operator >(IFloat one, IFloat other)
	{
		return one.Value > other.Value;
	}


	public static bool operator >(IFloat one, int other)
	{
		return one.Value > (long)other << SHIFT_AMOUNT;
	}
	
	
	public static bool operator >(int one, IFloat other)
	{
		return (long)one << SHIFT_AMOUNT > other.Value;
	}

	public static bool operator >(float one, IFloat other)
	{
		return ((IFloat)one).Value > other.Value;
	}
	
	public static bool operator > (IFloat one, float other)
	{
		return one.Value  > ((IFloat)other).Value;
	}

	#endregion
	
	#region <<
	public static IFloat operator <<(IFloat one, int amount)
	{
		one.Value = one.Value << amount;
		return one;
	}
	#endregion
	
	#region >>
	public static IFloat operator >>(IFloat one, int amount)
	{
		one.Value = one.Value >> amount;
		return one;
	}
	#endregion










	#region Casting

	public static implicit operator IFloat(Xfloat src)
	{
		return IFloat.Create(src);
	}

	public static implicit operator IFloat(Xint src)
	{
		return IFloat.Create(src.Get());
	}

//	public static implicit operator int(IFloat src)
//	{
//		return (int)(src.Value >> SHIFT_AMOUNT);
//	}
	
	public static implicit operator IFloat(int src)
	{
		return IFloat.Create(src);
	}

	public static implicit operator float(IFloat src)
	{
		return src.AsFloat();
	}
	
	public static implicit operator IFloat(float src)
	{
		return IFloat.Create(src);
	}
	#endregion








	
	#region PI, DoublePI
	public static IFloat PI = IFloat.Create(12868, false); //PI x 2^12
	public static IFloat TwoPIF = PI * 2; //radian equivalent of 260 degrees
	public static IFloat PIOver180F = PI / (IFloat)180; //PI / 180
	public static IFloat HalfPI = PI/2;

	#endregion
	
	
	#region Sqrt
	
	public static IFloat Sqrt(IFloat f)
	{
		byte numberOfIterations = 8;
		if(f.Value > 0)
		{
		if (f.Value > 0x64000)
			{
			numberOfIterations = 12;
			}
			else if (f.Value > 0x3e8000)
			{
			numberOfIterations = 16;
			}
		}
		else if (f.Value < 0) //NaN in Math.Sqrt
		{
			throw new ArithmeticException("Input Error");
		}
		else if (f.Value == 0)
		{
			return IFloat.Zero;
		}

		long k = f.Value + IFloat.n1.Value >> 1;
		
		for (int i = 0; i < numberOfIterations; ++i)
		{
			if(k == 0) break;
			k = (k + (  (f.Value << IFloat.SHIFT_AMOUNT) / k)) >> 1;
		}
		
		f.Value = k;
		
		if (k < 0)
		{
			throw new ArithmeticException("Overflow");
		}

		return f;

	}
	#endregion





	#region Sin
	public static IFloat Sin(IFloat i)
	{
		IFloat j = (IFloat)0;
		for (; i < 0; i += IFloat.Create(25736, false)) ;
		if (i > IFloat.Create(25736, false))
			i %= IFloat.Create(25736, false);
		IFloat k = (i * IFloat.Create(10, false)) / IFloat.Create(714, false);
		if (i != 0 && i != IFloat.Create(6434, false) && i != IFloat.Create(12868, false) &&
		    i != IFloat.Create(19302, false) && i != IFloat.Create(25736, false))
			j = (i * IFloat.Create(100, false)) / IFloat.Create(714, false) - k * IFloat.Create(10, false);
		if (k <= IFloat.Create(90, false))
			return sin_lookup(k, j);
		if (k <= IFloat.Create(180, false))
			return sin_lookup(IFloat.Create(180, false) - k, j);
		if (k <= IFloat.Create(270, false))
			return sin_lookup(k - IFloat.Create(180, false), j).Inverse;
		else
			return sin_lookup(IFloat.Create(360, false) - k, j).Inverse;
	}
	
	private static IFloat sin_lookup(IFloat i, IFloat j)
	{
		if (j > 0 && j < IFloat.Create(10, false) && i < IFloat.Create(90, false))
			return IFloat.Create(SIN_TABLE[i.Value], false) +
				((IFloat.Create(SIN_TABLE[i.Value + 1], false) - IFloat.Create(SIN_TABLE[i.Value], false)) /
				 IFloat.Create(10, false)) * j;
		else
			return IFloat.Create(SIN_TABLE[i.Value], false);
	}
	
	private static int[] SIN_TABLE = {
		0, 71, 142, 214, 285, 357, 428, 499, 570, 641, 
		711, 781, 851, 921, 990, 1060, 1128, 1197, 1265, 1333, 
		1400, 1468, 1534, 1600, 1665, 1730, 1795, 1859, 1922, 1985, 
		2048, 2109, 2170, 2230, 2290, 2349, 2407, 2464, 2521, 2577, 
		2632, 2686, 2740, 2793, 2845, 2896, 2946, 2995, 3043, 3091, 
		3137, 3183, 3227, 3271, 3313, 3355, 3395, 3434, 3473, 3510, 
		3547, 3582, 3616, 3649, 3681, 3712, 3741, 3770, 3797, 3823, 
		3849, 3872, 3895, 3917, 3937, 3956, 3974, 3991, 4006, 4020, 
		4033, 4045, 4056, 4065, 4073, 4080, 4086, 4090, 4093, 4095, 
		4096
	};
	#endregion
	
	private static IFloat mul(IFloat F1, IFloat F2)
	{
		return F1 * F2;
	}
	
	#region Cos, Tan, Asin
	public static IFloat Cos(IFloat i)
	{
		return Sin(i + IFloat.Create(6435, false));
	}
	
	public static IFloat Tan(IFloat i)
	{
		return Sin(i) / Cos(i);
	}
	
	public static IFloat Asin(IFloat F)
	{
		bool isNegative = F < 0;
		F = Abs(F);
		
		if (F > IFloat.OneF)
			throw new ArithmeticException("Bad Asin Input:" + F.AsFloat());
		
		IFloat f1 = mul(mul(mul(mul(IFloat.Create(145103 >> IFloat.SHIFT_AMOUNT, false), F) -
		                        IFloat.Create(599880 >> IFloat.SHIFT_AMOUNT, false), F) +
		                    IFloat.Create(1420468 >> IFloat.SHIFT_AMOUNT, false), F) -
		                IFloat.Create(3592413 >> IFloat.SHIFT_AMOUNT, false), F) +
			IFloat.Create(26353447 >> IFloat.SHIFT_AMOUNT, false);
		IFloat f2 = PI / IFloat.Create(2, true) - (Sqrt(IFloat.OneF - F) * f1);
		
		return isNegative ? f2.Inverse : f2;
	}
	#endregion
	
	#region ATan, ATan2
	public static IFloat Atan(IFloat F)
	{
		return Asin(F / Sqrt(IFloat.OneF + (F * F)));
	}
	
	public static IFloat Atan2(IFloat F1, IFloat F2)
	{
		if (F2.Value == 0 && F1.Value == 0)
			return (IFloat)0;
		
		IFloat result = (IFloat)0;
		if (F2 > 0)
			result = Atan(F1 / F2);
		else if (F2 < 0)
		{
			if (F1 >= 0)
				result = (PI - Atan(Abs(F1 / F2)));
			else
				result = (PI - Atan(Abs(F1 / F2))).Inverse;
		}
		else
			result = (F1 >= 0 ? PI : PI.Inverse) / IFloat.Create(2, true);
		
		return result;
	}
	#endregion
	
	#region Abs
	public static IFloat Abs(IFloat F)
	{
		if (F < 0)
			return F.Inverse;
		else
			return F;
	}
	#endregion




	#region Misc
	
	public bool IsZero()
	{
		return (Value == 0);
	}


	public override bool Equals(object obj)
	{
		if (obj is IFloat)
		{
			return ((IFloat)obj).Value == this.Value;
		}
		else
		{
			return false;
		}
	}


	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}
	
	public override string ToString()
	{
		return AsFloat().ToString();
	}

	public int CompareTo(IFloat sValue)
	{
		if(Value == sValue.Value)
		{
			return 0;
		}
		else if(Value <= sValue.Value)
		{
			return -1;
		}

		return 1;
	}


	public static void tryParse(string str, out IFloat value)
	{
		float temp = 0.0f;
		float.TryParse(str, out temp);
		value = temp;
	}


	#endregion

	#endregion

}

public struct IVector2
{
	public IFloat x;
	public IFloat y;

	#region Constructors
	public static implicit operator IVector2(Vector2 iv)
	{
		IVector2 v;
		v.x = iv.x;
		v.y = iv.y;
		return v;
	}



	public static IVector2 Create(IFloat X, IFloat Y)
	{
		IVector2 v;
		v.x = X;
		v.y = Y;
		return v;
	}
	
	public static IVector2 Create(int x, int y)
	{
		return IVector2.Create(IFloat.Create(x), IFloat.Create(y));
	}
	
	public static IVector2 Create(float x, float y)
	{
		return IVector2.Create(IFloat.Create(x), IFloat.Create(y));
	}
	
	public static IVector2 Zero
	{
		get { return IVector2.Create(IFloat.Create(0), IFloat.Create(0)); }
	}
	
	
	public Vector2 AsVector2()
	{
		return new Vector2(x.AsFloat(), y.AsFloat());
	}
	
	#endregion
	
	#region Vector Operations
	
	#region +
	public static IVector2 operator +(IVector2 one, IVector2 other)
	{
		return IVector2.Create(one.x + other.x, one.y + other.y);
	}
	
	#endregion
	
	#region -
	public static IVector2 operator -(IVector2 one, IVector2 other)
	{
		return IVector2.Create(one.x - other.x, one.y - other.y);
	}
	
	#endregion
	
	#region SqrMagnitude
	public IFloat SqrMagnitude()
	{
		return (x * x + y * y);
	}
	#endregion
	
	#region Magnitude
	public IFloat Magnitude()
	{
		return IFloat.Sqrt(x * x + y * y);
	}
	#endregion
	
	#region Normalize
	public IVector2 Normalize()
	{
		IFloat length = Magnitude();
		return IVector2.Create(x / length, y / length);
	}
	#endregion
	
	#region isZero
	public bool IsZero()
	{
		return (x.IsZero() && y.IsZero());
	}
	#endregion
	
	#endregion
	
	public override string ToString()
	{
		return "("+ x + ", " + y + ")";
	}
}

public struct IVector3
{
	public IFloat x;
	public IFloat y;
	public IFloat z;
	
	#region Constructors

	public IVector3(Vector3 v)
	{
		x = v.x;
		y = v.y;
		z = v.z;
	}

	public IVector3(float X, float Y, float Z)
	{
		x = X;
		y = Y;
		z = Z;
	}


	public static IVector3 Create(IFloat X, IFloat Y, IFloat Z)
	{
		IVector3 v;
		v.x = X;
		v.y = Y;
		v.z = Z;
		return v;
	}
	
	public static IVector3 Create(Vector3 v)
	{
		return IVector3.Create(IFloat.Create(v.x), IFloat.Create(v.y), IFloat.Create(v.z));
	}
	
	public static IVector3 Create(float x, float y, float z)
	{
		return IVector3.Create(IFloat.Create(x), IFloat.Create(y), IFloat.Create(z));
	}
	
	public static IVector3 zero
	{
		get { return IVector3.Create(IFloat.Create(0), IFloat.Create(0), IFloat.Create(0)); }
	}
	
	public Vector3 AsVector3()
	{
		return new Vector3(x.AsFloat(), y.AsFloat(), z.AsFloat());
	}

	#endregion

	#region Vector Operations

	public static implicit operator Vector3(IVector3 sValue)
	{	
		return sValue.AsVector3();
	}

	public static implicit operator IVector3(Vector3 sValue)
	{	
		return IVector3.Create(sValue);
	}


	#region +
	public static IVector3 operator +(IVector3 one, IVector3 other)
	{
		return IVector3.Create(one.x + other.x, one.y + other.y, one.z + other.z);
	}

	public static IVector3 operator +(IVector3 one, Vector3 other)
	{
		return IVector3.Create(one.x + other.x, one.y + other.y, one.z + other.z);
	}

	public static IVector3 operator +(Vector3 one, IVector3 other)
	{
		return IVector3.Create(one.x + other.x, one.y + other.y, one.z + other.z);
	}

	
	#endregion
	
	#region -
	public static IVector3 operator -(IVector3 one, IVector3 other)
	{
		return IVector3.Create(one.x - other.x, one.y - other.y, one.z - other.z);
	}


	public static IVector3 operator -(Vector3 one, IVector3 other)
	{
		return IVector3.Create(one.x - other.x, one.y - other.y, one.z - other.z);
	}

	public static IVector3 operator -(IVector3 one, Vector3 other)
	{
		return IVector3.Create(one.x - other.x, one.y - other.y, one.z - other.z);
	}

	
	#endregion
	
	#region *
	public static IVector3 operator *(IVector3 aIVector, int bInt)
	{
		IVector3 iVector;
		iVector.x = aIVector.x * bInt;
		iVector.y = aIVector.y * bInt;
		iVector.z = aIVector.z * bInt;
		return iVector;
	}
	
	public static IVector3 operator *(int aInt, IVector3 bIVector)
	{
		IVector3 iVector;
		iVector.x = bIVector.x * aInt;
		iVector.y = bIVector.y * aInt;
		iVector.z = bIVector.z * aInt;
		return iVector;
	}
	
	public static IVector3 operator *(IVector3 aIVector, IFloat bIFloat)
	{
		IVector3 iVector;
		iVector.x = aIVector.x * bIFloat;
		iVector.y = aIVector.y * bIFloat;
		iVector.z = aIVector.z * bIFloat;
		return iVector;
	}
	
	public static IVector3 operator *(IFloat aIFloat, IVector3 bIVector)
	{
		IVector3 iVector;
		iVector.x = bIVector.x * aIFloat;
		iVector.y = bIVector.y * aIFloat;
		iVector.z = bIVector.z * aIFloat;
		return iVector;
	}


	public static IVector3 operator *(IVector3 aIVector, Xfloat bIFloat)
	{
		IVector3 iVector;
		iVector.x = aIVector.x * bIFloat;
		iVector.y = aIVector.y * bIFloat;
		iVector.z = aIVector.z * bIFloat;
		return iVector;
	}
	
	public static IVector3 operator *(Xfloat aIFloat, IVector3 bIVector)
	{
		IVector3 iVector;
		iVector.x = bIVector.x * aIFloat;
		iVector.y = bIVector.y * aIFloat;
		iVector.z = bIVector.z * aIFloat;
		return iVector;
	}



	public static IVector3 operator *(IVector3 aIVector, float bIFloat)
	{
		IVector3 iVector;
		iVector.x = aIVector.x * bIFloat;
		iVector.y = aIVector.y * bIFloat;
		iVector.z = aIVector.z * bIFloat;
		return iVector;
	}
	
	public static IVector3 operator *(float aIFloat, IVector3 bIVector)
	{
		IVector3 iVector;
		iVector.x = bIVector.x * aIFloat;
		iVector.y = bIVector.y * aIFloat;
		iVector.z = bIVector.z * aIFloat;
		return iVector;
	}


	public static IVector3 operator /(IVector3 aIVector, IFloat bIFloat)
	{
		IVector3 iVector;
		iVector.x = aIVector.x / bIFloat;
		iVector.y = aIVector.y / bIFloat;
		iVector.z = aIVector.z / bIFloat;
		return iVector;
	}

	public static bool operator ==(IVector3 av, IVector3 bv)
	{
		if(av.x != bv.x) return false;
		if(av.y != bv.y) return false;
		if(av.z != bv.z) return false;

		return true;
	}


	public static bool operator !=(IVector3 av, IVector3 bv)
	{
		if(av.x == bv.x) return false;
		if(av.y == bv.y) return false;
		if(av.z == bv.z) return false;
		
		return true;
	}


	#endregion
	
	#region SqrMagnitude
	public IFloat SqrMagnitude()
	{
		return (x * x + y * y + z * z);
	}
	#endregion
	
	#region Magnitude
	public IFloat Magnitude()
	{
		return IFloat.Sqrt(x * x + y * y + z * z);
	}
	#endregion
	
	#region isZero
	public bool IsZero()
	{
		return (x.IsZero() && y.IsZero() && z.IsZero());
	}
	#endregion
	

	public static IVector3 lerp(IVector3 from, IVector3 to, IFloat step)
	{
		return ((to - from) * step + from);
	}




	public static IFloat Dot (IVector3 lhs, IVector3 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
	}


	public static IVector3 Cross (IVector3 lhs, IVector3 rhs)
	{
		return new IVector3 (lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
	}


	public static IVector3 up
	{
		get
		{
			return new IVector3 (0, 1, 0);
		}
	}

	public static IVector3 Normalize (IVector3 v)
	{
		IFloat mag = v.Magnitude();
		if (mag == 0) return IVector3.zero;
		return v/mag;
	}



	public override string ToString()
	{
		return "(" + x + ", " + y + ", " + z + ")";
	}
	#endregion

	public static IVector3 forward = new IVector3(0,0,1);

}



















public struct IQuaternion
{
	public IFloat x;
	public IFloat y;
	public IFloat z;
	public IFloat w;

	public IQuaternion(Quaternion v)
	{
		x = v.x;
		y = v.y;
		z = v.z;
		w = v.w;
	}

	public static implicit operator Quaternion(IQuaternion sValue)
	{	
		return (new Quaternion(sValue.x, sValue.y, sValue.z, sValue.w));
	}
	
	public static implicit operator IQuaternion(Quaternion sValue)
	{	
		return new IQuaternion(sValue);
	}


	public IVector3 eulerAngles
	{
		get
		{
			return (new Quaternion(x, y, z, w)).eulerAngles;
		}
		set
		{
			Quaternion q = new Quaternion();
			q.eulerAngles = value;
			x = q.x;
			y = q.y;
			z = q.z;
			w = q.w;
		}
	}


	public static IFloat Dot(IQuaternion a, IQuaternion b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
	}


//	static IFloat angleCalcValue = 114.59f;
//
//	public static IFloat Angle(IQuaternion a, IQuaternion b)
//	{
//		IFloat f = Dot(a, b);
//		f = Mathf.Acos(MathUtil.min(MathUtil.abs(f), 1));
//		return f * angleCalcValue;
//	}
}


















public static class FixedEasing
{
	// Adapted from source : http://www.robertpenner.com/easing/
	
	public static IFloat Ease(IFloat linearStep, IFloat acceleration, EasingType type)
	{
		IFloat easedStep = acceleration > 0 ? EaseIn(linearStep, type) : 
			acceleration < 0 ? EaseOut(linearStep, type) : 
				(IFloat) linearStep;
		
		return FixedMathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
	}
	
	public static IFloat EaseIn(IFloat linearStep, EasingType type)
	{
		switch (type)
		{
		case EasingType.Step:       return linearStep < IFloat.dot5 ? 0 : 1;
		case EasingType.Linear:     return linearStep;
		case EasingType.Sine:       return Sine.EaseIn(linearStep);
		case EasingType.Quadratic:  return Power.EaseIn(linearStep, 2);
		case EasingType.Cubic:      return Power.EaseIn(linearStep, 3);
		case EasingType.Quartic:    return Power.EaseIn(linearStep, 4);
		case EasingType.Quintic:    return Power.EaseIn(linearStep, 5);
		}
		throw new NotImplementedException();
	}
	
	public static IFloat EaseOut(IFloat linearStep, EasingType type)
	{
		switch (type)
		{
		case EasingType.Step:       return linearStep < IFloat.dot5 ? 0 : 1;
		case EasingType.Linear:     return (IFloat)linearStep;
		case EasingType.Sine:       return Sine.EaseOut(linearStep);
		case EasingType.Quadratic:  return Power.EaseOut(linearStep, 2);
		case EasingType.Cubic:      return Power.EaseOut(linearStep, 3);
		case EasingType.Quartic:    return Power.EaseOut(linearStep, 4);
		case EasingType.Quintic:    return Power.EaseOut(linearStep, 5);
		}
		throw new NotImplementedException();
	}
	
	public static IFloat EaseInOut(IFloat linearStep, EasingType easeInType, EasingType easeOutType)
	{
		return linearStep < IFloat.dot5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
	}
	public static IFloat EaseInOut(IFloat linearStep, EasingType type)
	{
		switch (type)
		{
		case EasingType.Step:       return linearStep < IFloat.dot5 ? 0 : 1;
		case EasingType.Linear:     return (IFloat)linearStep;
		case EasingType.Sine:       return Sine.EaseInOut(linearStep);
		case EasingType.Quadratic:  return Power.EaseInOut(linearStep, 2);
		case EasingType.Cubic:      return Power.EaseInOut(linearStep, 3);
		case EasingType.Quartic:    return Power.EaseInOut(linearStep, 4);
		case EasingType.Quintic:    return Power.EaseInOut(linearStep, 5);
		}
		throw new NotImplementedException();
	}
	
	static class Sine
	{
		public static IFloat EaseIn(IFloat s)
		{
			return (IFloat)Math.Sin(s * FixedMathHelper.HalfPi - FixedMathHelper.HalfPi) + 1;
		}
		public static IFloat EaseOut(IFloat s)
		{
			return (IFloat)Math.Sin(s * FixedMathHelper.HalfPi);
		}
		public static IFloat EaseInOut(IFloat s)
		{
			return (IFloat)(Math.Sin(s * FixedMathHelper.Pi - FixedMathHelper.HalfPi) + 1) / 2;
		}
	}
	
	static class Power
	{
		public static IFloat EaseIn(IFloat s, int power)
		{
			return MathUtil.pow(s, power);
		}
		public static IFloat EaseOut(IFloat s, int power)
		{
			int sign = power % 2 == 0 ? -1 : 1;
			return (sign * (MathUtil.pow(s - 1, power) + sign));
		}
		public static IFloat EaseInOut(IFloat s, int power)
		{
			s *= 2;
			if (s < 1) return EaseIn(s, power) / 2;
			int sign = power % 2 == 0 ? -1 : 1;
			return (sign / IFloat.n2 * (MathUtil.pow(s - 2, power) + sign * 2));
		}
	}
}



public static class FixedMathHelper
{
	public static IFloat Pi = (IFloat)Math.PI;
	public static IFloat HalfPi = (IFloat)(Math.PI / 2);
	
	public static IFloat Lerp(IFloat from, IFloat to, IFloat step)
	{
		return (IFloat)((to - from) * step + from);
	}



}
//}
