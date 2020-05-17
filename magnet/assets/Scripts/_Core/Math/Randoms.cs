using UnityEngine;
using System.Collections;

/// <summary>
/// Platform별마다 rand()가 다를 수 있기때문에, 같은 Random 생성을 위해서 작성한 클래스.
/// </summary>
public class Randoms 
{
    public class Well512
    {
        static uint[] state = new uint[16];
        static uint index = 0;

        static Well512()
        {
            System.Random random = new System.Random((int)System.DateTime.Now.Ticks);

            for (int i = 0; i < 16; i++)
            {
                state[i] = (uint)random.Next();
            }
        }

        public static bool SetSeed(uint[] seeds)
        {
            if (null == seeds || seeds.Length < 16)
            {
                return false;
            }

            state = seeds;
            index = 0;

            return true;
        }

        internal static uint Next(int minValue, int maxValue)
        {
            return (uint)((Next() % (maxValue - minValue)) + minValue);
        }

        public static uint Next(uint maxValue)
        {
            return Next() % maxValue;
        }

        public static uint Next()
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
}
