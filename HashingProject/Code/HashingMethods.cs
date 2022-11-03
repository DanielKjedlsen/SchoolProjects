using System;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Generic;

namespace RAD
{
    public class HashingMethods
    {
        private static int l;
        public static void lSet(int v)
        {
            l = v;
        }

        public static ulong multiplyShiftHashing(ulong x, List<ValueType> randomParameters)
        {
            Debug.Assert(randomParameters.Count == 1);
            ulong a = (ulong)randomParameters[0];
            return (a*x)>>(64-l);
        }

        public static ulong multiplyModPrimeHashing(ulong x, List<ValueType> randomParameters)
        {
            Debug.Assert(randomParameters.Count == 2);
            BigInteger val = ((BigInteger)x * (BigInteger)randomParameters[0] + (BigInteger)randomParameters[1]);
            BigInteger p = new BigInteger(Math.Pow(2, 89)) - 1;
            BigInteger y = (val&p)+(val>>89);
            if (y>=p) y-=p;
            y = y&((BigInteger)Math.Pow(2,l)-1);
            return (ulong)y;
        }

        public static BigInteger fourUniversalHashing(ulong x, List<ValueType> randomParameters)
        {
            Debug.Assert(randomParameters.Count == 4);
            BigInteger y = (BigInteger)randomParameters[3];
            BigInteger p = new BigInteger(Math.Pow(2,89)) - 1;
            for (int i = 2; i >= 0; --i)
            {
                y = y*x + (BigInteger)randomParameters[i];
                y = (y&p) + (y>>89);
            }
            if (y >= p)
                y = y - p;
            return y;
        }

        public static Tuple <int , int> countSketchHashing(ulong x, Func<ulong,List<ValueType>, BigInteger> hashFunction, List<ValueType> randomParameters, int t)
        {
            BigInteger m = (BigInteger)Math.Pow(2,t);
            BigInteger g = (BigInteger)hashFunction(x, randomParameters);
            int h = (int)(g&(m-1));
            BigInteger b = (BigInteger)g>>(88);
            int s = (int)(1 - 2*b);
            return Tuple.Create(h, s);

        } 
    }
}