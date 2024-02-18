using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNV.Collections.Generic
{
    public class MinHeap<T>
    {
        private readonly List<T> list;

        public int Count => list.Count;

        public MinHeap()
        {
            list = new List<T>();
        }

        public int Add(T item)
        {
            int index = list.BinarySearch(item);
            if (index < 0)
            {
                index = ~index;
            }
            list.Insert(index, item);
            return index;
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void Remove(T item)
        {
            list.Remove(item);
        }

        public T Peek()
        {
            return list[0];
        }

        public T Pop()
        {
            T item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public void Clear()
        {
            list.Clear();
        }
    }
}