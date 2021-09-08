using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaRD3
{
    public class HuffmanNode<T> : IComparable
    {
        internal HuffmanNode(double probability, T value)
        {
            Probability = probability;
            LeftChild = RightChild = Parent = null;
            Value = value;
            IsLeaf = true;
        }

        internal HuffmanNode(HuffmanNode<T> leftchild, HuffmanNode<T> rightchild)
        {
            LeftChild = leftchild;
            RightChild = rightchild;
            Probability = leftchild.Probability + rightchild.Probability;
            leftchild.IsZero = true;
            rightchild.IsZero = false;
            leftchild.Parent = rightchild.Parent = this;
            IsLeaf = false;
        }

        internal HuffmanNode<T> LeftChild { get; set; }
        internal HuffmanNode<T> RightChild { get; set; }
        internal HuffmanNode<T> Parent { get; set; }
        internal T Value { get; set; }
        internal bool IsLeaf { get; set; }

        internal bool IsZero { get; set; }

        internal int Bit
        {
            get { return IsZero ? 0 : 1; }
        }

        internal bool IsRoot
        {
            get { return Parent == null; }
        }

        internal double Probability { get; set; }

        public int CompareTo(object obj)
        {
            return -Probability.CompareTo(((HuffmanNode<T>)obj).Probability);
        }
    }
}
