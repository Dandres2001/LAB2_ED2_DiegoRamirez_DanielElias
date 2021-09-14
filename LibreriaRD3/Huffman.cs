using System;
using System.Collections.Generic;
using LibreriaRD3;

namespace LibreriaRD3
{
    
     public class Huffman<T> : ICompressor<T> where T : IComparable
        {
            public Dictionary<T, HuffmanNode<T>> _leafDictionary = new Dictionary<T, HuffmanNode<T>>();
        public Dictionary<T, int> countsDictionary = new Dictionary<T, int>();
        private readonly HuffmanNode<T> _root;

            public Huffman(IEnumerable<T> values)
            {
  
                var priorityQueue = new minHeap<HuffmanNode<T>>();
                int valueCount = 0;

                foreach (T value in values)
                {
                    if (!countsDictionary.ContainsKey(value))
                    {
                        countsDictionary[value] = 0;
                    }
                    countsDictionary[value]++;
                    valueCount++;
                }

                foreach (T value in countsDictionary.Keys)
                {
                    var node = new HuffmanNode<T>((double)countsDictionary[value] / valueCount, value);
                    priorityQueue.Add(node);
                    _leafDictionary[value] = node;
                }

                while (priorityQueue.Count > 1)
                {
                    HuffmanNode<T> leftSon = priorityQueue.Pop();
                    HuffmanNode<T> rightSon = priorityQueue.Pop();
                    var parent = new HuffmanNode<T>(leftSon, rightSon);
                    priorityQueue.Add(parent);
                }

                _root = priorityQueue.Pop();
                _root.IsZero = false;
            }

            public List<int> Encode(T value)
            {
                var returnValue = new List<int>();
                Encode(value, returnValue);
                return returnValue;
            }

            public void Encode(T value, List<int> encoding)
            {
                if (!_leafDictionary.ContainsKey(value))
                {
                    throw new ArgumentException("Invalid value in Encode");
                }
                HuffmanNode<T> nodeCur = _leafDictionary[value];
                var reverseEncoding = new List<int>();
                while (!nodeCur.IsRoot)
                {
                    reverseEncoding.Add(nodeCur.Bit);
                    nodeCur = nodeCur.Parent;
                }

                reverseEncoding.Reverse();
                encoding.AddRange(reverseEncoding);
            }

            public List<int> Encode(IEnumerable<T> values)
            {
                var returnValue = new List<int>();

                foreach (T value in values)
                {
                    Encode(value, returnValue);
                }

            //decodeascii(Encodeascii(returnValue));
            
            return returnValue;
            }

        public List<string> Encodeascii(List<int> values)
        {
            while (values.Count % 8 != 0)
            {
                values.Add(0);

            }
            var returvalue = new List<string>();
            //var bytearray = new List<int>();
            string bytearray = ""; 

            while (values != null)
            {

                for (int i = 0; i < 8; i++)
                {
                    if (values.Count > i)
                    {
                        bytearray += values[i];
                    }
                    //if (values.Count < 8)
                    //{
                    //   for (int  j = bytearray.Length; j < 8; j++)
                    //    {
                    //        bytearray += "0";

                    //    }
                    //}
                }
                if (values.Count > 8)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        values.RemoveAt(0);
                    }
                }
                else
                {
                    values = null;

                }

                var encEncoder = System.Text.Encoding.GetEncoding(28591);

                string str = encEncoder.GetString(getbytesfrombinarystrin(bytearray));
                bytearray = "";

                    
                returvalue.Add(str);
                 
            }
      
            return returvalue;
        } 

        public List<int> decodeascii(List<string> value, int largo)
        {
            //int i = 0;
            string current = "";
            byte[] bytearray;
            var returvalue = new List<int>();
            while (value != null)
            {
                current = value[0];
                value.RemoveAt(0);
                bytearray = System.Text.Encoding.GetEncoding(28591).GetBytes(current);
               
                for (int i = 0; i < bytearray.Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                       returvalue.Add((bytearray[i] & 0x80) > 0 ? 1 : 0);
                        bytearray[i] <<= 1;
                    }
                }
                current = "";

                if (value.Count == 0)
                {
                    value = null;
                }

            }
            var returvalue2 = new List<int>();
            for (int i=0; i < largo; i++)
            {

                returvalue2.Add(returvalue[i]);


            }
            return returvalue2;
        }

        public Byte[] getbytesfrombinarystrin(string binary)
        {
            var list = new List<Byte>();
         
                list.Add(Convert.ToByte(binary, 2));
         
            return list.ToArray();
        }

        public Byte[] ConvertToByte(string binary)
        {
            var list = new List<Byte>();

            list.Add(Convert.ToByte(binary));

            return list.ToArray();
        }
        public T Decode(List<int> bitString, ref int position)
            {
                HuffmanNode<T> nodeCur = _root;
            try
            {
                while (!nodeCur.IsLeaf)
                {
                    if (position > bitString.Count)
                    {
                        throw new ArgumentException("Invalid bitstring in Decode");
                    }
                    nodeCur = bitString[position++] == 0 ? nodeCur.LeftChild : nodeCur.RightChild;
                }
            }
            catch
            {

            }
                return nodeCur.Value;
            }

            public List<T> Decode(List<int> bitString) //este es  el que se llama
            {
                int position = 0;
                var returnValue = new List<T>();

                while (position != bitString.Count)
                {
                    returnValue.Add(Decode(bitString, ref position));
                }
                return returnValue;
            }
        }
    
}
