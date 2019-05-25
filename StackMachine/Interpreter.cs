using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StackMachine
{
    /// <summary>
    /// OPCodes retrieved from the ByteCodes
    /// TODO: how do we pass arguments in a efficient way
    /// </summary>
    class Interpreter : Stack<Value>
    {
        /// <summary>
        /// Program Counter
        /// </summary>
        private int PC { get; set; }
        /// <summary>
        /// Frame Pointer
        /// </summary>
		private int FP { get; set; }


        
		public Interpreter()
        {
            PC = 0;
            FP = 0;
        }

        public Interpreter(IEnumerable<Value> collection) : base(collection)
        {
        }

        public void Execute(Bytecode bytecode, Environment environment)
        {
            var globalEnvironment = environment;
            while (PC < bytecode.Bytecodes.Length)
            {
                Thread.Sleep(100);
                var opCode = (OpCode)bytecode.Bytecodes[PC].i32;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"{PC.ToString("X8")} ");
                Console.ForegroundColor = ConsoleColor.Green;
                if (bytecode.IsDebugging && bytecode.DebugInfo.ContainsKey(PC))
                    Console.WriteLine(bytecode.DebugInfo[PC]);
                else
                    Console.WriteLine("OPCODE: {opCode.ToString()}");
                Console.ForegroundColor = ConsoleColor.White;
                PC++;
                switch (opCode)
                {
                    case OpCode.PUSH:
                        {
                            Push(bytecode.Bytecodes[PC++]);
                            break;
                        }
                    case OpCode.POP:
                        {
                            Pop();
                            break;
                        }
                    case OpCode.DUP:
                        {
                            var value = Pop();
                            Push(value);
                            Push(value);
                            break;
                        }
                    case OpCode.PRINT:
                        {
                            var value = Pop();
                            if (value.type == ValueType.INT || value.type == ValueType.NUMBER
                                || value.type == ValueType.CHAR || value.type == ValueType.BOOL)
                            {
                                Console.WriteLine($"{value}");
                            }
                            else if (value.type == ValueType.SYMBOL)
                            {
                                Console.WriteLine($"{bytecode.Symbols[value.i32]}");
                            }
                            break;
                        }
                    case OpCode.DEBUG:
                        environment.Print();
                        Print();
                        break;
                    case OpCode.STORE:
                        {
                            string storeLiteral = bytecode.Symbols[bytecode.Bytecodes[PC++].i32];
                            var value = Pop();
                            environment.Add(storeLiteral, value);
                            break;
                        }
                    case OpCode.LOOKUP_LOCAL:
                        {
                            var lookupLiteral = bytecode.Symbols[bytecode.Bytecodes[PC++].i32];
                            var value = environment.Lookup(lookupLiteral);
                            Push(value);
                            break;
                        }
                    case OpCode.ADD:
                        {
                            var value1 = Pop();
                            var value2 = Pop();
                            if (value1.type == ValueType.INT && value2.type == ValueType.INT)
                            {
                                var result = new Value { type = ValueType.INT, i32 = value1.i32 + value2.i32 };
                                Push(result);
                            }
                            else if (value1.type == ValueType.NUMBER && value2.type == ValueType.NUMBER)
                            {
                                var result = new Value { type = ValueType.NUMBER, fl = value1.fl + value2.fl };
                                Push(result);
                            }
                            else
                            {
                                throw new Exception("Invalid types for OP_ADD");
                            }
                            break;
                        }
                    case OpCode.ADD1:
                        {
                            var value1 = Pop();
                            if (value1.type == ValueType.INT)
                            {
                                var result = new Value { type = ValueType.INT, i32 = value1.i32 + 1  };
                                Push(result);
                            }
                            else if (value1.type == ValueType.NUMBER)
                            {
                                var result = new Value { type = ValueType.NUMBER, fl = value1.fl + 1f };
                                Push(result);
                            }
                            else
                            {
                                throw new Exception("Invalid types for OP_ADD");
                            }
                            break;
                        }
                    case OpCode.SUB:
                        {
                            var value1 = Pop();
                            var value2 = Pop();
                            if (value1.type == ValueType.INT && value2.type == ValueType.INT)
                            {
                                var result = new Value { type = ValueType.INT, i32 = value2.i32 - value1.i32 };
                                Push(result);
                            }
                            else if (value1.type == ValueType.NUMBER && value2.type == ValueType.NUMBER)
                            {
                                var result = new Value { type = ValueType.NUMBER, fl = value2.fl - value1.fl };
                                Push(result);
                            }
                            else
                            {
                                throw new Exception("Invalid types for SUB");
                            }
                            break;
                        }
                    case OpCode.SUB1:
                        {
                            var value = Pop();
                            if (value.type == ValueType.INT)
                            {
                                var result = new Value { type = ValueType.INT, i32 = value.i32 - 1 };
                                Push(result);
                            }
                            else if (value.type == ValueType.NUMBER)
                            {
                                var result = new Value { type = ValueType.NUMBER, fl = value.fl - 1 };
                                Push(result);
                            }
                            else
                            {
                                throw new Exception("Invalid types for SUB1");
                            }
                            break;
                        }
                    //always jump to next value
                    case OpCode.JMP:
                        PC = bytecode.Bytecodes[PC].i32;
                        break;
                    //conditional jump, jumps to next address if the top of the stack is true
                    case OpCode.JMPCMP:
                        {
                            var _newPC = bytecode.Bytecodes[PC++].i32;
                            var value = Pop();
                            if ((value.type == ValueType.BOOL && value.b) ||
                                (value.type == ValueType.INT && value.i32 == 0))
                            {
                                PC = _newPC;
                            }
                            break;
                        }
                    //conditional jump, jumps to next address if the top of the stack is false
                    case OpCode.JNE:
                        {
                            var _newPC = bytecode.Bytecodes[PC++].i32;
                            var value = Pop();
                            if (value.b)
                            {
                                PC = _newPC;
                            }
                            break;
                        }
                    case OpCode.CLOSURE:
                        {
                            /*
                                A Closure has to capture variables from the current Environment creating a copy at the time the Closure was created.
                                this means that every captured variable will be a new reference in the Store. 

                                The new Environment needs to to point to the new Store references, maybe an OpCode could be used to hint
                                the interpreter to only capture the variables it needs because capturing everything will be expensive 
                                on huge environments.

                                |-------------+--------------+---|
                                | Env E1      |              |   |
                                | var a = 10; |              |   |
                                |             | ------------ |   |
                                |             | Env E1       |   |
                                |             | var c = 30;  |   |
                                |             | var b = 20;  |   |
                                +-------------+--------------+---+

                                Symbol Table:
                                | Loc | Symbol |
                                |-----+--------|
                                |   5 | a      |
                                |   9 | b      |
                                |  16 | c      |
                                +-----+--------+
                                Environment:
                                | Env | Symbol | Ref |
                                |-----+--------+-----|
                                | E1  | a      |   7 |
                                | E2  | a      |   8 |
                                | E2  | b      |   9 |
                                | E3  | c      |  10 |
                                +-----+--------+-----+
                                Store:
                                | Loc | Type     | Val |                             |
                                |-----+----------+-----+-----------------------------|
                                |   0 | CLOSURE  | 002 | closure points to PC        |
                                |   1 | NUMBER   |   3 | Closure captured references |
                                |   2 | UPVALUE  |   5 | Env Reference a             |
                                |   3 | UPVALUE  |   9 | Env Reference b             |
                                |   4 | UPVALUE  |  16 | Env Reference c             |
                                |  .. | ..       |  .. |                             |
                                |   7 | INT      |  10 | Previous Environment a = 10 |
                                |   8 | INT      |  10 | New Environment a = 10      |
                                |   9 | INT      |  20 | b = 20                      |
                                |  10 | INT      |  30 | c = 30                      |
                                +-----+----------+-----+-----------------------------+
                             * */
                            var closure = new Value() { type = ValueType.CLOSURE, i32 = PC++ }; //get the closure
                            var closeUpValues = bytecode.Bytecodes[PC++];
                            var arguments = new Value[closeUpValues.i32]; //get the number of upvalues
                            for (int i = arguments.Length-1; i >= 0; i--) 
                            {
                                Push(bytecode.Bytecodes[PC++]);  //push upvalues
                            }

                            Push(closeUpValues);
                            Push(closure);
                            break;
                        }
                    //the function itself should be retrieved from the stack
                    //the closure contains all the necessary information to execute
                    //what we need after the application is the number of arguments to POP
                    case OpCode.APP:
                        {
                            //number of arguments to get
                            int argumentCount = bytecode.Bytecodes[PC].i32;
                            //where to we store the arguments?
                            //using the stack would be inefficient

                            //closure
                            var closure = Pop();
                            if (closure.type != ValueType.CLOSURE)
                            {
                                throw new Exception($"Expected a Closure, got a {closure.type} instead");
                            }
                            var upValueCount = Pop();
                            if (upValueCount.type != ValueType.INT)
                            {
                                throw new Exception("Expected a number of upvalues");
                            }

                            //clone the environment here
                            string[] upvalues = new string[upValueCount.i32];
                            for (int i = 0; i < upValueCount.i32; i++)
                            {
                                upvalues[i] = bytecode.Symbols[Pop().i32];
                            }


                            //main environment becomes parent
                            environment = environment.ClosureEnvironment(upvalues);


                            //push return address at the top of the stack
                            Push(new Value() { type = ValueType.INT, i32 = closure.i32 });
                            

                            //store return address after frame pointer
                            //Push(new Value() { type = ValueType.INT, i32 = PC + 1 });
                            //point frame pointer to top of the stack
                            FP = this.Count;

                            //here we have to switch the environment

                            //move to procedure location
                            PC = bytecode.Bytecodes[PC].i32;
                            break;
                        }
                    //function return, functions should always return something
                    case OpCode.RET:
                        {
                            //here we have to restore the environment
                            environment = environment.Parent;
                            var returnValue = Pop();
                            while (Count > FP) Pop();
                            PC = Pop().i32;
                            Push(returnValue);
                            break;
                        }
                    case OpCode.QUIT:
                        return;
                }

            }

        }

        void Print()
        {

        }

        //void execute(Bytecode bytecode, Environment environment);

    }
}
