using System;
using System.Collections.Generic;
using System.Text;

namespace StackMachine
{
    class Environment
    {
        Dictionary<string, int> Env { get; set; }
        public Environment Parent { get; set; }
        Store Store { get; set; }
		public Environment(Store store)
        {
            this.Store = store;
            this.Env = new Dictionary<string, int>();
            this.Parent = null;
        }

        public Environment(Store store, Environment parent) : this(store)
        {
            this.Parent = parent;
        }

        public void Add(string name, Value value)
        {
            var reference = this.Store.Add(value);
            this.Env.Add(name, reference);
        }

        public Value Lookup(string name)
        {
            if (this.Env.ContainsKey(name))
                return this.Store.Get(this.Env[name]);
            if (this.Parent != null)
                return this.Parent.Lookup(name);

            throw new Exception($"Variable {name} not found in environment");
        }

        public void Print()
        {
            foreach (var entry in Env)
            {
                Console.WriteLine($"{entry.Key} \t=\t {this.Store.Get(entry.Value)}");
            }
        }


        /// <summary>
        /// How do we know when someone is now using an environment after a return?
        /// how do we garbage collect anonymous closure environments
        /// </summary>
        /// <param name="upvalues"></param>
        /// <returns></returns>
        public Environment ClosureEnvironment(params string[] upvalues)
        {
            Environment env = new Environment(Store);
            foreach (var value in upvalues)
            {
                env.Add(value, Lookup(value));
            }
            env.Parent = this;
            return env;
        }
    }
}
