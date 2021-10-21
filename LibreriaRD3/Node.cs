using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaRD3

{
    class HuffmanNode
    {
        public byte _Byte;
        public int hijoIzq, hijoDer;
        public HuffmanNode() { }
        public HuffmanNode(byte _byte, int left, int right)
        {
            this._Byte = _byte; this.hijoIzq = left; this.hijoDer = right;
        }
    }
    class Node : IComparable<Node>
    {

        public Node HijoIzq, HijoDer;
        public int id;
        public bool isByte;
        public byte _Byte;
        public int frecuencias;
        public Node() { }
        public Node(byte _byte, int frecuencia, Node hijoIzq, Node hijoDer, bool isbyte)
        {
            this._Byte = _byte; this.frecuencias = frecuencia; this.HijoIzq = hijoIzq; this.HijoDer = hijoDer; this.isByte = isbyte;
        }

        public int CompareTo(Node nodo)
        {
            return (this.frecuencias > nodo.frecuencias) ? -1 : ((this.frecuencias == nodo.frecuencias) ? 0 : 1);
        }
    }
}
