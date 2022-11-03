using System;
using System.Collections.Generic;

namespace RAD 
{
    public class Node 
    {
        public ulong key;
        public int value;
        public Node? next;
        public Node(ulong x, int v) 
        {
            key = x;
            value = v;
        }
    }
    
    public class HashTable
    {

        private Func<ulong,List<ValueType>, ulong> HashFunction;
        private List<ValueType> randomParameters;

        public Dictionary<ulong, Node> LookupTable;

        public HashTable(Func<ulong,List<ValueType>, ulong> HashFunction, List<ValueType> randomParameters) {
            this.HashFunction = HashFunction;
            this.randomParameters = randomParameters;
            LookupTable = new Dictionary<ulong, Node>();
        }
        
        public long get(ulong x)
        {
            ulong hashcode = HashFunction(x, randomParameters);
            Node curr;
            LookupTable.TryGetValue(hashcode, out curr);
            while(curr != null) 
            {
                if (curr.key == x)
                    return curr.value;
                curr = curr.next;
            }
            return 0;
        }

        public void set(ulong x, int v)
        {
            ulong hashcode = HashFunction(x, randomParameters);
            Node curr; 
            if (!LookupTable.TryGetValue(hashcode, out curr))
            {
                Node node = new Node(x, v);
                LookupTable.Add(hashcode, node);
                return;
            }
            while (curr.next != null && curr.key != x)
            {
                curr = curr.next;
            }
            if (curr.key == x)
                curr.value = v;
            else
                curr.next = new Node(x, v);
        }

        public void increment(ulong x, int d)
        {
            ulong hashcode = HashFunction(x, randomParameters);
            Node curr; 
              if (!LookupTable.TryGetValue(hashcode, out curr))
            {
                Node node = new Node(x, d);
                LookupTable.Add(hashcode, node);
                return;
            }
            while (curr.next != null && curr.key != x)
            {
                curr = curr.next;
            }
            if (curr.key == x)
                curr.value += d;
            else
                curr.next = new Node(x, d);
        }
    }
}