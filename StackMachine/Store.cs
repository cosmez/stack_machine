using System;
using System.Collections.Generic;
using System.Text;

namespace StackMachine
{
    class Store
    {
		List<Value> storage { get; set; }

        public Store()
        {
            storage = new List<Value>();
        }
        
        public void Print()
        {

        }

        public int Add(Value value)
        {
            storage.Add(value);
            return storage.Count-1;
        }

        public Value Get(int reference)
        {
            return storage[reference];
        }
    }
}
