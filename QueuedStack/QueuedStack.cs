using System;
using System.Collections.Generic;

[assembly: CLSCompliant (true)]
namespace Kaos.Collections
{
    public class QueuedStack<T>
    {
        private readonly List<T> data;
        public int Height { get; private set; }

        public QueuedStack()
        {
            data = new List<T>();
        }


        public T this[int index] { get { return data[index]; } }
        public int Count { get { return data.Count; } }
        public T Peek() { return data[Height-1]; }
        public T Next() { return data[Height]; }


        public void Clear()
        {
            Height = 0;
            data.Clear();
        }


        public void Enqueue (T item)
        {
            data.Add (item);
        }

        public T Pop()
        {
            T result = data[Height-1];
            --Height;
            data.RemoveAt (Height);
            return result;
        }


        public void Push()
        {
            if (Height >= Count)
                throw new InvalidOperationException ("Queue is empty.");
            ++Height;
        }

        public void Push (T item)
        {
            if (Height < Count)
                data[Height] = item;
            else
                data.Add (item);
            ++Height;
        }


        public void RemoveAt (int index)
        {
            data.RemoveAt (index);
            if (Height > data.Count)
                Height = data.Count;
        }
    }
}
