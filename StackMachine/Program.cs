using System;
using System.Buffers;
using System.Collections.Generic;

namespace StackMachine
{
    class Program
    {
        /// <summary>
        /// TODO: Re-read contents in README.cs
        /// </summary>
        static void Main()
        {
            
            var compiler = new BytecodeWriter(debugging: true);

            compiler.Push(10);
            compiler.StoreReference();
            compiler.Store("ptr");
            compiler.LookupLocal("ptr");
            compiler.LoadReference();
            compiler.Print();

            /*compiler.Push(10);
            compiler.Store("a");
            compiler.Push(20);
            compiler.Store("b");
            compiler.Push(30);
            compiler.Store("c");

            compiler.Closure("func1", new string[] { "a", "b", "c" });
            compiler.Store("clos1");
            compiler.LookupLocal("clos1");
            compiler.App();
            compiler.Quit();

            compiler.Label("func1");
            compiler.LookupLocal("a");
            compiler.Dup(); //return a contents
            compiler.Print();
            compiler.Ret();*/
            compiler.Quit();


            var bytecode = compiler.Compile();

            MemoryPool<Value> pool = MemoryPool<Value>.Shared;
            var rental = pool.Rent(minBufferSize: 1024);
            var interpreter = new Interpreter();
            var store = new Heap(rental.Memory);
            var env = new Environment(store);
            interpreter.Execute(bytecode, env, store);
        }
    }
}
