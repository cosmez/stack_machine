using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StackMachine
{
    /// <summary>
    /// OPCodes retrieved from the ByteCodes
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

                            Console.WriteLine($"{value}");
                            break;
                        }
                    case OpCode.DEBUG:
                        environment.Print();
                        Print();
                        break;
                    case OpCode.STORE:
                        {
                            string storeLiteral = bytecode.Symbols[bytecode.Bytecodes[PC++].i32];
                            environment.Add(storeLiteral, Pop());
                            break;
                        }
                    case OpCode.LOOKUP:
                        {
                            var lookupLiteral = bytecode.Symbols[bytecode.Bytecodes[PC++].i32];
                            Push(environment.Lookup(lookupLiteral));
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
                    //function call
                    case OpCode.APP:
                        {
                            //store return address after frame pointer
                            Push(new Value() { type = ValueType.INT, i32 = PC + 1 });
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
