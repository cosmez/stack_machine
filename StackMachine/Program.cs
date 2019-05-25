using System;
using System.Collections.Generic;

namespace StackMachine
{
    class Program
    {
        static void Main(string[] args)
        {

            var compiler = new BytecodeWriter(debugging: true);
            compiler.Push(10);
            compiler.Store("a");
            compiler.Push(20);
            compiler.Store("b");
            compiler.Push(30);
            compiler.Store("c");

            compiler.Closure("func1", new string[] { "a", "b", "c" });


            compiler.Label("func1");
            compiler.LookupLocal("a");
            compiler.Print();
            compiler.Ret();


            compiler.Quit();
            var bytecode = compiler.Compile();
            

            var interpreter = new Interpreter();
            var store = new Store();
            var env = new Environment(store);
            interpreter.Execute(bytecode, env);
        }
    }
}
