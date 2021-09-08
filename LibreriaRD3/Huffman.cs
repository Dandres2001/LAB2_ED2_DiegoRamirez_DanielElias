using System;
using System.Collections.Generic;
using LibreriaRD3;

namespace LibreriaRD3
{
    
     public class Huffman<T> : ICompressor<T> where T : IComparable
        {
            private readonly Dictionary<T, HuffmanNode<T>> _leafDictionary = new Dictionary<T, HuffmanNode<T>>();
            private readonly HuffmanNode<T> _root;

            public Huffman(IEnumerable<T> values)
            {
                var countsDictionary = new Dictionary<T, int>();
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
                return returnValue;
            }

            public T Decode(List<int> bitString, ref int position)
            {
                HuffmanNode<T> nodeCur = _root;
                while (!nodeCur.IsLeaf)
                {
                    if (position > bitString.Count)
                    {
                        throw new ArgumentException("Invalid bitstring in Decode");
                    }
                    nodeCur = bitString[position++] == 0 ? nodeCur.LeftChild : nodeCur.RightChild;
                }
                return nodeCur.Value;
            }

            public List<T> Decode(List<int> bitString)
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
