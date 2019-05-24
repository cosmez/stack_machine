using System;
using System.Collections.Generic;
using System.Text;

namespace StackMachine
{
    class Environment
    {
        Dictionary<string, int> Env { get; set; }
        Store Store { get; set; }
		public Environment(Store store)
        {
            this.Store = store;
            this.Env = new Dictionary<string, int>();
        }
        public void Add(string name, Value value)
        {
            var reference = this.Store.Add(value);
            this.Env.Add(name, reference);
        }

        public Value Lookup(string name)
        {
            return this.Store.Get(this.Env[name]);
        }

        public void Print()
        {
            foreach (var entry in Env)
            {
                Console.WriteLine($"{entry.Key} \t=\t {entry.Value}");
            }
        }
    }
}
