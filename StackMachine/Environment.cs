﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StackMachine
{
    class Environment
    {
        Dictionary<string, int> Env { get; set; }
        public Environment? Parent { get; set; }
        Heap Store { get; set; }
		public Environment(Heap store)
        {
            this.Store = store;
            this.Env = new Dictionary<string, int>();
            this.Parent = null;
        }

        public Environment(Heap store, Environment parent) : this(store)
        {
            this.Parent = parent;
        }

        public void Add(string name, Value value)
        {
            var reference = this.Store.Add(value);
            this.Env.Add(name, reference);
        }

        public void Add(string name, int value) =>
            Add(name, new Value(value));
        public void Add(string name, bool value) =>
            Add(name, new Value(value));
        public void Add(string name, char value) =>
            Add(name, new Value(value));
        public void Add(string name, float value) =>
            Add(name, new Value(value));

        public Value Lookup(string name)
        {
            if (this.Env.ContainsKey(name))
                return this.Store[this.Env[name]];
            if (this.Parent != null)
                return this.Parent.Lookup(name);

            throw new Exception($"Variable {name} not found in environment");
        }

        public void Print()
        {
            foreach (var entry in Env)
            {
                Console.WriteLine($"{entry.Key} \t=\t {this.Store[entry.Value]}");
            }
        }


        /// <summary>
        /// How do we know when someone is now using an environment after a return?
        /// how do we garbage collect anonymous closure environments
        /// </summary>
        /// <param name="upvalues"></param>
        /// <returns></returns>
        public Environment ClosureEnvironment(ReadOnlySpan<string> upvalues)
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
