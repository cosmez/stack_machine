using System;
using System.Collections.Generic;
using System.Text;

namespace StackMachine
{
    /// <summary>
    /// This is our dynamic memory allocator, right now the implementation is really naive.
    /// it just pushes items by using an autoincremental index and does no gabarge collection
    /// </summary>
    public class Heap
    {
        public Memory<Value> Memory { get; }
        int _idx = 0;

        public Heap(Memory<Value> memory)
        {
            this.Memory = memory;
        }
        
        public void Print()
        {

        }


        public int Add(Value value)
        {
            Memory.Span[_idx] = value;
            return _idx++;
        }

        public int Add(int value) =>
            Add(new Value(value));
        public int Add(float value) =>
            Add(new Value(value));
        public int Add(bool value) =>
            Add(new Value(value));
        public int Add(char value) =>
            Add(new Value(value));


        public int AddArray(Span<Value> values)
        {
            values[_idx++] = new Value(ValueType.ARRAY, values.Length);
            foreach (var value in values)
            {
                values[_idx++] = value;
            }
            return _idx-1;
        }

        /// <summary>
        /// Returns an element from the Heap at x coordinate
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public Value this[int reference]
        {
            get
            {
                return Memory.Span[reference];
            }
        }

        /// <summary>
        /// Returns an element from the Heap at x coordinate
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public Value Get(int reference)
        {
            return Memory.Span[reference];
        }
    }
}
