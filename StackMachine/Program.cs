using System;
using System.Collections.Generic;

namespace StackMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var bytecode = new Bytecode()
            {
                Instructions = new List<int>
                {
                    (int)OpCode.PUSH, 10,   //0.
		            (int)OpCode.PUSH, 5,    //2.
		            (int)OpCode.PUSH, 7,    //4.
		            (int)OpCode.PUSH, 9,    //6.
		            (int)OpCode.POP,        //8.
		            (int)OpCode.ADD,        //9.
		            (int)OpCode.DEBUG,      //10.
		            (int)OpCode.STORE, 0,   //11. val1 = top of the stack.
		            (int)OpCode.PUSH, 17,   //13. 
		            (int)OpCode.LOOKUP, 0,  //15. top of the stack = val1
		            (int)OpCode.ADD,        //17. 
		            (int)OpCode.DEBUG,      //18. 
		            (int)OpCode.PUSH, 1,    //19
		            (int)OpCode.SUB,        //21
		            (int)OpCode.DUP,        //22
		            (int)OpCode.DUP,      //23
		            (int)OpCode.DUP,        //24
		            (int)OpCode.JMPCMP, 18, //25  jump to 18
		            (int)OpCode.APP, 30,    //27
		            (int)OpCode.QUIT,       //29
		            (int)OpCode.BLOCK,         //30 function block, capture environment ? (val1)
		            (int)OpCode.PUSH, 10,   //31
		            (int)OpCode.PRINT,      //33
		            (int)OpCode.PUSH, 0,    //35 value to return
		            (int)OpCode.RET,        //37 return 0
		            (int)OpCode.ENDBLOCK       //38
                },
                Strings = new List<string> { "val1" }
            };

            var interpreter = new Interpreter();
            var store = new Store();
            var env = new Environment(store);
            interpreter.Execute(bytecode, env);
        }
    }
}
