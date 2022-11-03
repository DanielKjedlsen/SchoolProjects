using System;
using System.Diagnostics;
using System.Threading;

namespace RAD
{
    class Program
    {   
        static void Main(string[] args)
        {   
            // Set variables
            int numOfElementsInStream = 16777216; // (n)
            int logNumOfDifferentKeysInStream = 23; // (l)
            int countSketchSize = 12; // (t)

            // l for multiplyShift and multiplyModPrime is set using below function
            HashingMethods.lSet(Math.Min(63, logNumOfDifferentKeysInStream)); 

            // Generate stream of data
            IEnumerable <Tuple <ulong , int >> datastream = DataStream.CreateStream(numOfElementsInStream, logNumOfDifferentKeysInStream);

            // Running task 1
            Tests.TaskOne(datastream);

            // Running task 3
            Tests.TaskThree(datastream);

            // Running task 7
            Console.WriteLine("\nTASK 7");
            
            Tests.createCountSketch(countSketchSize, datastream);
    
        }
    }
}