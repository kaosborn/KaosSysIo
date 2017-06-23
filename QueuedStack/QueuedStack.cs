using System;
using System.Collections.Generic;

[assembly: CLSCompliant (true)]
namespace Kaos.SysIo.Collections
{
    /// <summary>
    /// QueuedStack represents a Last-In-First-Out data structure with a queue of pending elements.
    /// </summary>
    /// <remarks>
    /// QueuedStack is implemented as a single array with the stack immediately followed by the queue.
    /// When the API overlaps that of the BCL Stack class, the semantics are identical.
    /// If only the stack API is utilized, its behavior if functionally equivalent to that of the BCL Stack class.
    /// </remarks>
    /// <typeparam name="T">Specifies the type of elements in the queued stack.</typeparam>
    public class QueuedStack<T>
    {
        private readonly List<T> data;

        /// <summary>Get the number of elements in the stack.</summary>
        public int Count { get; private set; }


        /// <summary>Initialize a new QueuedStack instance that is empty.</summary>
        public QueuedStack()
        {
            data = new List<T>();
        }


        /// <summary>Get the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index] { get { return data[index]; } }


        /// <summary>Get the sum of elements in the stack and queue.</summary>
        public int TotalCount { get { return data.Count; } }


        /// <summary>Returns the element at the top of the stack without removing it.</summary>
        /// <returns>The element at the top of the stack.</returns>
        public T Peek() { return data[Count-1]; }


        /// <summary>Returns the element at the beginning of the queue without moving it onto the stack.</summary>
        /// <returns>The first element in the queue.</returns>
        public T Head() { return data[Count]; }


        /// <summary>Removes all elements from the QueuedStack.</summary>
        public void Clear()
        {
            Count = 0;
            data.Clear();
        }


        /// <summary>Append to the queue of pending elements.</summary>
        /// <param name="item">The element to append to the queue.</param>
        public void Enqueue (T item)
        {
            data.Add (item);
        }


        /// <summary>Removes and returns the element at the top of the stack.</summary>
        /// <returns>The element removed from the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">When the stack is empty.</exception>
        public T Pop()
        {
            if (Count == 0)
                throw new InvalidOperationException ("The stack is empty.");
            T result = data[Count-1];
            --Count;
            data.RemoveAt (Count);
            return result;
        }


        /// <summary>Appends the element at the bottom of the queue onto the stack.</summary>
        /// <exception cref="InvalidOperationException">When the queue is empty.</exception>
        public void Push()
        {
            if (Count >= data.Count)
                throw new InvalidOperationException ("The queue is empty.");
            ++Count;
        }


        /// <summary>Appends the specified element onto the stack.</summary>
        /// <param name="item">The element to push onto the stack.</param>
        public void Push (T item)
        {
            if (Count < data.Count)
                data[Count] = item;
            else
                data.Add (item);
            ++Count;
        }


        /// <summary>Removes the element at the specified index of the queued stack.</summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <exception cref="InvalidOperationException">When index is out of range.</exception>
        public void RemoveAt (int index)
        {
            if (index < 0 || index >= data.Count)
                throw new ArgumentOutOfRangeException ("Index must be between 0 and number of elements.");

            data.RemoveAt (index);
            if (Count > data.Count)
                Count = data.Count;
        }
    }
}
