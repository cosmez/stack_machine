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
 * [X] Symbols
 * [X] Chars
 * [ ] Function Parameters
 * [ ] Closures
 * [ ] Cons cells
 * [ ] Lists
 * */

    class Program
    {
        static void Main(string[] args)
        {

            var compiler = new BytecodeWriter(debugging: true);
            
            compiler.Quit();
            var bytecode = compiler.Compile();

            var interpreter = new Interpreter();
            var store = new Store();
            var env = new Environment(store);
            interpreter.Execute(bytecode, env);
        }
    }
}
