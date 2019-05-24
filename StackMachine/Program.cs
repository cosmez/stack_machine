using System;
using System.Collections.Generic;

namespace StackMachine
{
    /***
 * TODO:
 * [X] Basic Stack Operations
 * [X] Math Operations
 * [X] Basic Environment
 * [X] Grow Environment
 * [X] Bytecode OP CODES
 * [X] Interpreter
 * [X] Symbols
 * [X] Functions
 * [X] Bytecode Writer
 * [X] Debug Information
 * [X] Untyped Stack
 * [ ] Symbols
 * [ ] Chars
 * [ ] Closures
 * */

    class Program
    {
        static void Main(string[] args)
        {

            var compiler = new BytecodeWriter(debugging: true);
            compiler.Push(10);
            compiler.Push(5);
            compiler.Push(7);
            compiler.Push(9);
            compiler.Pop();
            compiler.Add();
            compiler.Debug();
            compiler.Store("val1");
            compiler.Push(17);
            compiler.Lookup("val1");
            compiler.Add();
            compiler.Label("LOOP");
            compiler.Debug();
            compiler.Push(1);
            compiler.Sub();
            compiler.Dup();
            compiler.Print();
            compiler.Dup();
            compiler.JumpNotZero("LOOP");
            compiler.App("FUN1");
            compiler.Quit();
            compiler.Label("FUN1");
            compiler.Block();
            compiler.Push(10);
            compiler.Print();
            compiler.Push(0);
            compiler.Ret();
            compiler.EndBlock();
            var bytecode = compiler.Compile();



            var interpreter = new Interpreter();
            var store = new Store();
            var env = new Environment(store);
            interpreter.Execute(bytecode, env);
        }
    }
}
