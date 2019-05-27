using System;
using System.Collections.Generic;
using System.Text;

namespace StackMachine
{
    class Store
    {
        public Memory<Value> Memory { get; }
        int _idx = 0;

        public Store(int initialMemory = 5000)
        {
            Memory = new Value[initialMemory];
            _idx = 0;
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
            Add(new Value { type = ValueType.INT, i32 = value });
        public int Add(float value) =>
            Add(new Value { type = ValueType.INT, fl = value });
        public int Add(bool value) =>
            Add(new Value { type = ValueType.INT, b = value });
        public int Add(char value) =>
            Add(new Value { type = ValueType.INT, c = value });
        public int Add(ValueType type, int value) =>
            Add(new Value { type = type, i32 = value });

        public int Add(Span<Value> values)
        {
            foreach (var value in values)
            {
                values[_idx++] = value;
            }
            return _idx-1;
        }

        public Value this[int reference]
        {
            get
            {
                return Memory.Span[reference];
            }
        }

        public Value Get(int reference)
        {
            return Memory.Span[reference];
        }
    }
}
