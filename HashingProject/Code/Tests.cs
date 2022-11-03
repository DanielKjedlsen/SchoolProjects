using System;
using System.Diagnostics;
using System.Threading;
using System.Numerics;
using System.Collections.Generic;

namespace RAD
{
    public static class Tests
    {
        public static void TaskOne(IEnumerable <Tuple <ulong , int >> datastream)
        {
            Console.WriteLine("TASK ONE");
            // Random variables a and a,b are chosen at random at put into lists
            List<ValueType> aRandom = new List<ValueType>();
            aRandom.Add(Randomizer.getRandomUlong());
  
            List<ValueType> abRandom = new List<ValueType>();
            abRandom.Add(Randomizer.getRandomBigInt());
            abRandom.Add(Randomizer.getRandomBigInt());


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ulong sumMultiplyShiftHashing = 0;
            foreach (Tuple<ulong, int> elm in datastream)
            {
                sumMultiplyShiftHashing += HashingMethods.multiplyShiftHashing(elm.Item1, aRandom);
            }
            Console.WriteLine("\nmultiply-shift hashing sum: " + sumMultiplyShiftHashing as string);
            stopwatch.Stop();
            Console.WriteLine("\nElapsed Time for multiply-shift hashing is {0} ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();


            stopwatch.Start();
            ulong sumMultiplyModPrimeHashing = 0;
            foreach (Tuple<ulong, int> elm in datastream)
            {
                sumMultiplyModPrimeHashing += HashingMethods.multiplyModPrimeHashing(elm.Item1, abRandom);
            }
            Console.WriteLine("\nmultiply-mod-prime hashing sum: " + sumMultiplyModPrimeHashing as string);
            stopwatch.Stop();
            Console.WriteLine("\nElapsed Time for multiply-mod-prime hashing is {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public static void TaskThree(IEnumerable <Tuple <ulong , int >> datastream)
        {
            Console.WriteLine("\nTASK THREE");
            // Random variables a and a,b are chosen at random at put into lists
            List<ValueType> aRandom = new List<ValueType>();
            aRandom.Add(Randomizer.getRandomUlong());
  
            List<ValueType> abRandom = new List<ValueType>();
            abRandom.Add(Randomizer.getRandomBigInt());
            abRandom.Add(Randomizer.getRandomBigInt());

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            HashTable hashTableFirst = new HashTable(HashingMethods.multiplyShiftHashing, aRandom);
            Console.WriteLine("\nKvadratsum calculated using multiplyShiftHashing:");
            stopwatch.Start();
            calcKvadratsum(hashTableFirst, datastream);
            stopwatch.Stop();
            Console.WriteLine("\nElapsed Time for calculating second moment using multiply-shift hashing is {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            
            stopwatch.Start();
            HashTable hashTableSecond = new HashTable(HashingMethods.multiplyModPrimeHashing, abRandom);
            Console.WriteLine("\nKvadratsum calculated using multiplyModPrimeHashing:");
            stopwatch.Start();
            calcKvadratsum(hashTableSecond, datastream);
            stopwatch.Stop();
            Console.WriteLine("\nElapsed Time for calculating second moment using multiply-mod-prime hashing is {0} ms", stopwatch.ElapsedMilliseconds);
        }


        private static void calcKvadratsum(HashTable hashTable, IEnumerable <Tuple <ulong , int >> datastream)
        {
            foreach (Tuple<ulong, int> elm in datastream)
            {
                hashTable.increment(elm.Item1, elm.Item2);
            }
            long kvadratsum = 0;
            Dictionary<ulong, Node>.KeyCollection keys = hashTable.LookupTable.Keys;  
            foreach (ulong key in keys)  
            {  
                Node curr = hashTable.LookupTable[key];
                while (curr != null)
                {
                    kvadratsum += curr.value^2;
                    curr = curr.next;
                }
            }  
            Console.WriteLine(kvadratsum);
        }

        public static void createCountSketch(int t, IEnumerable <Tuple <ulong , int >> datastream)
        {
            BigInteger m = (BigInteger)Math.Pow(2, t);
            List<long> sketch = new List<long>();
            List<ValueType> param = new List<ValueType>();
            param.Add(Randomizer.getRandomBigInt());
            param.Add(Randomizer.getRandomBigInt());
            param.Add(Randomizer.getRandomBigInt());
            param.Add(Randomizer.getRandomBigInt());
            for (int i = 0; i<m; ++i)
                sketch.Add(0);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (Tuple<ulong, int> elm in datastream)
            {
                ulong x = elm.Item1;
                Tuple<int, int> hashes = HashingMethods.countSketchHashing(x, HashingMethods.fourUniversalHashing, param, t);
                sketch[hashes.Item1] += (long)(elm.Item2 * hashes.Item2);
            }
            estimateSecondMoment(sketch);
            stopwatch.Stop();
            Console.WriteLine("\nElapsed Time for Count Sketch creation + Second Moment Estimation is {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public static long estimateSecondMoment(List<long> countSketch)
        {
            int size = countSketch.Count;
            long kvadratsum = 0;
            for (int i = 0; i < size; ++i)
            {
                kvadratsum += (long)Math.Pow(countSketch[i], 2);
            }
            //Console.WriteLine("\nEstimated second moment (kvadratsum): {0}", kvadratsum);
            return kvadratsum;
        }
    }
}