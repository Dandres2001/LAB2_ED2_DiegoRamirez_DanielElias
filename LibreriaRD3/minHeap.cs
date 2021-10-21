using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaRD3
{
    public class minHeap<T>
    {
       
       
        public int Cont { get; private set; }
        public minHeap() : this(null) { }
        public minHeap(int capacidad) : this(capacidad, null) { }
        public minHeap(IComparer<T> comparador) : this(16, comparador) { }
        T[] heap;
        IComparer<T> comparador;
        public minHeap(int Capacidad, IComparer<T> Comparador)

        {
            this.comparador = (Comparador == null) ? Comparer<T>.Default : Comparador;
            this.heap = new T[Capacidad];
        }
        public void push(T v)
        {
            if (Cont >= heap.Length) Array.Resize(ref heap, Cont * 2);
            heap[Cont] = v;
            SiftUp(Cont++);
        }
        public T Pop()
        {
            var v = EncontrarUltimo();
            heap[0] = heap[--Cont];
            if (Cont > 0) SiftDown(0);
            return v;
        }
        void SiftUp(int indice)
        {
            var v = heap[indice];
            for (var n2 = indice / 2; indice > 0 && comparador.Compare(v, heap[n2]) > 0; indice = n2, n2 /= 2) heap[indice] = heap[n2];
            heap[indice] = v;
        }
        void SiftDown(int indice)
        {
            var v = heap[indice];
            for (var n2 = indice * 2; n2 < Cont; indice = n2, n2 *= 2)
            {
                if (n2 + 1 < Cont && comparador.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;
                if (comparador.Compare(v, heap[n2]) >= 0) break;
                heap[indice] = heap[n2];
            }
            heap[indice] = v;
        }
        public T EncontrarUltimo()
        {
            if (Cont > 0) return heap[0];
            throw new InvalidOperationException("There are no elements in the priority queue");
        }

    }
}
