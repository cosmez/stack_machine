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
    class Interpreter
    {
        /// <summary>
        /// Program Counter
        /// </summary>
        private int PC { get; set; }
        /// <summary>
        /// Frame Pointer
        /// </summary>
		private int FP { get; set; }
        private int Top { get; set; }
        /// <summary>
        /// Current Environment
        /// </summary>
        private int CurrentEnv { get; set; }

        /// <summary>
        /// Environments.
        /// The current environment is retrieved by using PC (Function Location)
        /// as an index.
        /// </summary>
        private Dictionary<int, Environment> Envs { get; set; }

        private Environment Env {
            get { return Envs[CurrentEnv]; }
        }

        
		public Interpreter() 
        {
            PC = 0;
            FP = 0;
            Top = 0;
            Envs = new Dictionary<int, Environment>();
        }


        public void Execute(Bytecode bytecode, Environment rootEnv)
        {
            PC = 0;
            FP = 0;
            Span<Value> _stack = stackalloc Value[1000];
            Envs.Add(0, rootEnv); //global environment
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
                    Console.WriteLine($"OPCODE: {opCode.ToString()}");
                Console.ForegroundColor = ConsoleColor.White;
                PC++;
                switch (opCode)
                {
                    case OpCode.PUSH:
                        Push(_stack, bytecode.Bytecodes[PC++]);
                        break;
                    case OpCode.NIL:
                        Push(_stack, Value.Nil);
                        break;
                    case OpCode.FALSE:
                        Push(_stack, Value.False);
                        break;
                    case OpCode.TRUE:
                        Push(_stack, Value.True);
                        break;
                    case OpCode.POP:
                        Pop(_stack);
                        break;
                    case OpCode.DUP:
                        {
                            var value = Pop(_stack);
                            Push(_stack, value);
                            Push(_stack, value);
                            break;
                        }
                    case OpCode.PRINT:
                        {
                            var value = Pop(_stack);
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
                        Envs[CurrentEnv].Print();
                        Print();
                        break;
                    case OpCode.STORE:
                        {
                            string storeLiteral = bytecode.Symbols[bytecode.Bytecodes[PC++].i32];
                            var value = Pop(_stack);
                            Env.Add(storeLiteral, value);
                            break;
                        }
                    case OpCode.LOOKUP_LOCAL:
                        {
                            var lookupLiteral = bytecode.Symbols[bytecode.Bytecodes[PC++].i32];
                            var value = Env.Lookup(lookupLiteral);
                            Push(_stack, value);
                            break;
                        }
                    case OpCode.ADD:
                        {
                            var value1 = Pop(_stack);
                            var value2 = Pop(_stack);
                            if (value1.type == ValueType.INT && value2.type == ValueType.INT)
                                Push(_stack, value1.i32 + value2.i32);
                            else if (value1.type == ValueType.NUMBER && value2.type == ValueType.NUMBER)
                                Push(_stack, value1.fl + value2.fl);
                            else
                                throw new Exception("Invalid types for OP_ADD");
                            break;
                        }
                    case OpCode.ADD1:
                        {
                            var value1 = Pop(_stack);
                            if (value1.type == ValueType.INT) Push(_stack, value1.i32 + 1);
                            else if (value1.type == ValueType.NUMBER) Push(_stack, value1.fl + 1f);
                            else throw new Exception("Invalid types for OP_ADD");
                            break;
                        }
                    case OpCode.SUB:
                        {
                            var value1 = Pop(_stack);
                            var value2 = Pop(_stack);
                            if (value1.type == ValueType.INT && value2.type == ValueType.INT)
                                Push(_stack, value2.i32 - value1.i32);
                            else if (value1.type == ValueType.NUMBER && value2.type == ValueType.NUMBER)
                                Push(_stack,value2.fl - value1.fl);
                            else
                                throw new Exception("Invalid types for SUB"); ;
                            break;
                        }
                    case OpCode.SUB1:
                        {
                            var value = Pop(_stack);
                            if (value.type == ValueType.INT) Push(_stack, value.i32 - 1);
                            else if (value.type == ValueType.NUMBER) Push(_stack, value.fl - 1.0f);
                            else throw new Exception("Invalid types for SUB1");
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
                            var value = Pop(_stack);
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
                            var value = Pop(_stack);
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

                                | Env | Symbol | Ref |
                                |-----+--------+-----|
                                | E1  | a      |   7 |
                                | E2  | a      |   8 |
                                | E2  | b      |   9 |
                                | E3  | c      |  10 |
                                +-----+--------+-----+
                                OPCODE:
                                | Loc | Type     | Val |                             |
                                |-----+----------+-----+-----------------------------|
                                |   0 | CLOSURE  |  80 | closure points to PC        |
                                |   1 | NUMBER   |   3 | Closure captured references |
                                |   2 | UPVALUE  |   5 | Env Reference a             |
                                |   3 | UPVALUE  |   9 | Env Reference b             |
                                |   4 | UPVALUE  |  16 | Env Reference c             |
                                |  .. | ..       |  .. |                             |
                                |  .. | ..       |  .. |                             |
                                |   7 | INT      |  10 | Previous Environment a = 10 |
                                |   8 | INT      |  10 | New Environment a = 10      |
                                |   9 | INT      |  20 | b = 20                      |
                                |  10 | INT      |  30 | c = 30                      |
                                |  .. | ..       |  .. |                             |
                                |  80 | FUNCTION |  .. | Function Location           |
                                +-----+----------+-----+-----------------------------+
                             * */
                            var closure = new Value(ValueType.CLOSURE, bytecode.Bytecodes[PC++].i32); //get the closure
                            Push(_stack, closure);
                            //create new environment with upvalues
                            var closeUpValues = bytecode.Bytecodes[PC++];
                            //ReadOnlySpan<Value> arguments = stackalloc Value[closeUpValues.i32]; //get the number of upvalues
                            var upvalues =  new string[closeUpValues.i32];
                            for (int i = 0; i < closeUpValues.i32; i++) 
                            {
                                upvalues[i] = bytecode.Symbols[bytecode.Bytecodes[PC++].i32];
                            }
                            //clone the environment here
                            Envs.Add(closure.i32, rootEnv.ClosureEnvironment(upvalues));
                            
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
                            var closure = Pop(_stack);
                            if (closure.type != ValueType.CLOSURE)
                            {
                                throw new Exception($"Expected a Closure, got a {closure.type} instead");
                            }


                            Push(_stack, PC + 1); //push return address at the top of the stack
                            Push(_stack,CurrentEnv); //push current environment
                            

                            //store return address after frame pointer
                            //Push(new Value() { type = ValueType.INT, i32 = PC + 1 });
                            //point frame pointer to top of the stack
                            FP = Top;

                            //here we have to switch the environment

                            //move to procedure location
                            PC = closure.i32;
                            //move to closure env
                            CurrentEnv = PC;
                            break;
                        }
                    //function return, functions should always return something
                    case OpCode.RET:
                        {
                            //return value
                            var returnValue = Pop(_stack);

                            while (Top > FP) Pop(_stack);
                            CurrentEnv = Pop(_stack).i32; //previous environment
                            PC = Pop(_stack).i32; //return address
                            Push(_stack, returnValue);
                            break;
                        }
                    case OpCode.QUIT:
                        return;
                }

            }

        }

        Value Pop(Span<Value> Stack) => Stack[--Top];
        void Push(Span<Value> Stack, Value value) { Stack[Top++] = value;  }
        void Push(Span<Value> Stack, int value) { Push(Stack,new Value(value)); }
        void Push(Span<Value> Stack, bool value) { Push(Stack, new Value(value)); }
        void Push(Span<Value> Stack, char value) { Push(Stack, new Value(value)); }
        void Push(Span<Value> Stack, float value) { Push(Stack, new Value(value)); }


        void Print()
        {

        }

        //void execute(Bytecode bytecode, Environment environment);

    }
}
