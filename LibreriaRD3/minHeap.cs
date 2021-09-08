using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaRD3
{

        public class minHeap<T> where T : IComparable
        {
            protected List<T> HeapList = new List<T>();

            public virtual int Count
            {
                get { return HeapList.Count; }
            }

            public virtual void Add(T val)
            {
                HeapList.Add(val);
                SetAtPosition(HeapList.Count - 1, val);
                UpHeap(HeapList.Count - 1);
            }

            public virtual T Peek()
            {
                if (HeapList.Count == 0)
                {
                    throw new IndexOutOfRangeException("Peeking at an empty priority queue");
                }

                return HeapList[0];
            }

            public virtual T Pop()
            {
                if (HeapList.Count == 0)
                {
                    throw new IndexOutOfRangeException("Popping an empty priority queue");
                }

                T valRet = HeapList[0];

                SetAtPosition(0, HeapList[HeapList.Count - 1]);
                HeapList.RemoveAt(HeapList.Count - 1);
                DownHeap(0);
                return valRet;
            }

            protected virtual void SetAtPosition(int i, T val)
            {
                HeapList[i] = val;
            }

            protected bool RightExists(int i)
            {
                return RightChildIndex(i) < HeapList.Count;
            }

            protected bool LeftExists(int i)
            {
                return LeftChildIndex(i) < HeapList.Count;
            }

            protected int ParentIndex(int i)
            {
                return (i - 1) / 2;
            }

            protected int LeftChildIndex(int i)
            {
                return 2 * i + 1;
            }

            protected int RightChildIndex(int i)
            {
                return 2 * (i + 1);
            }

            protected T ArrayValue(int i)
            {
                return HeapList[i];
            }

            protected T Parent(int i)
            {
                return HeapList[ParentIndex(i)];
            }

            protected T Left(int i)
            {
                return HeapList[LeftChildIndex(i)];
            }

            protected T Right(int i)
            {
                return HeapList[RightChildIndex(i)];
            }

            protected void Swap(int i, int j)
            {
                T valHold = ArrayValue(i);
                SetAtPosition(i, HeapList[j]);
                SetAtPosition(j, valHold);
            }

            protected void UpHeap(int i)
            {
                while (i > 0 && ArrayValue(i).CompareTo(Parent(i)) > 0)
                {
                    Swap(i, ParentIndex(i));
                    i = ParentIndex(i);
                }
            }

            protected void DownHeap(int i)
            {
                while (i >= 0)
                {
                    int ic = -1;

                    if (RightExists(i) && Right(i).CompareTo(ArrayValue(i)) > 0)
                    {
                        ic = Left(i).CompareTo(Right(i)) < 0 ? RightChildIndex(i) : LeftChildIndex(i);
                    }
                    else if (LeftExists(i) && Left(i).CompareTo(ArrayValue(i)) > 0)
                    {
                        ic = LeftChildIndex(i);
                    }

                    if (ic >= 0 && ic < HeapList.Count)
                    {
                        Swap(i, ic);
                    }

                    i = ic;
                }
            }
        }

    }

