using System;
using System.Collections.Generic;
using System.Text;

namespace StackMachine
{
    class Bytecode
    {
        public Value[] Bytecodes;
        public string[] Symbols;
        public bool IsDebugging;

        public Dictionary<int, string> DebugInfo { get; set; }
        public Bytecode(Value[] bytecode, string[] symbols, bool isDebugging = false, Dictionary<int, string> debugInfo = null)
        {
            this.Bytecodes = bytecode;
            this.Symbols = symbols;
            IsDebugging = isDebugging;
            this.DebugInfo = debugInfo;
        }
    }

    class BytecodeWriter
    {
        public List<Value> Bytecode;
        List<string> Symbols;
        Dictionary<string, int> Labels;
        Dictionary<int, string> undefinedLabels;
        const int PLACEHOLDER_LABEL = 0;
        //debugging information
        public bool IsDebugging { get; set; } = false;
        public Dictionary<int, string> DebugInfo { get; set; }  //maps a string to a instruction number

        public BytecodeWriter(bool debugging)
        {
            Bytecode = new List<Value>();
            Symbols = new List<string>();
            Labels = new Dictionary<string, int>();
            undefinedLabels = new Dictionary<int, string>();
            Labels.Add("INIT", PLACEHOLDER_LABEL); //reserved
            DebugInfo = new Dictionary<int, string>();
            IsDebugging = debugging;
        }


        private void Add(OpCode opCode) =>
            Bytecode.Add(new Value(ValueType.INT, (int)opCode));

        private void Add(int value) =>
            Bytecode.Add(new Value(ValueType.INT, value));

        private void Add(float value) =>
            Bytecode.Add(new Value(ValueType.INT, value));

        private void Add(char value) =>
            Bytecode.Add(new Value(ValueType.INT, value));

        private void Add(bool value) =>
            Bytecode.Add(new Value(ValueType.INT, value));

        private void AddNil() =>
            Bytecode.Add(new Value(ValueType.NIL, 0));

        /// <summary>
        /// Adds A Symbol
        /// </summary>
        /// <param name="value"></param>
        private void Add(string symbol)
        {
            if (!Symbols.Contains(symbol)) Symbols.Add(symbol);
            Bytecode.Add(new Value(ValueType.SYMBOL, Symbols.IndexOf(symbol)) );
        }



        /// <summary>
        /// Adds Debugging information to the current instruction
        /// </summary>
        /// <param name="content"></param>
        private void Debug(string content, int pad = 1)
        {
            if (this.IsDebugging)
            {
                if (DebugInfo.ContainsKey(Bytecode.Count))
                {
                    DebugInfo[Bytecode.Count] += $", {content}";
                } else DebugInfo.Add(Bytecode.Count, $"{(pad > 0 ? '\t' : ' ')}{content}");
            }
        }

        public void Push(int value) {
            Debug($"PUSH {value}");
            Add(OpCode.PUSH);
            Add(value);
        }

        public void Push(string value)
        {
            Debug($"PUSH '{value}");
            Add(OpCode.PUSH);
            Add(value);
        }

        public void Push(float value)
        {
            Debug($"PUSH {value}");
            Add(OpCode.PUSH);
            Add(value);
        }

        public void Push(bool value)
        {
            Debug($"PUSH {value}");
            Add(OpCode.PUSH);
            Add(value);
        }

        public void Push(char value)
        {
            Debug($"PUSH {value}");
            Add(OpCode.PUSH);
            Add(value);
        }

        public void Nil()
        {
            Debug($"PUSH NIL"); 
            Add(OpCode.PUSH);
            AddNil();
        }

        public void Pop()
        {
            Debug("POP");
            Add(OpCode.POP);
        }

        public void Dup()
        {
            Debug("DUP");
            Add(OpCode.DUP);
        }
        public void Add()
        {
            Debug("ADD");
            Add(OpCode.ADD);
        }
        public void Sub()
        {
            Debug("SUB");
            Add(OpCode.SUB);
        }

        public void Store(string name)
        {
            Debug($"STORE {name}");
            Add(OpCode.STORE);
            if (!Symbols.Contains(name)) Symbols.Add(name);
            Add(Symbols.IndexOf(name));
        }

        public void LookupLocal(string name)
        {
            Debug($"LOCAL {name}");
            Add(OpCode.LOOKUP_LOCAL);
            Add(Symbols.IndexOf(name));
        }


        public void Label(string name)
        {
            Debug($"{name}:", pad: 0);
            if (Labels.ContainsKey(name))
                Labels[name] = Bytecode.Count;
            else
                Labels.Add(name, Bytecode.Count);
        }

        /// <summary>
        /// jumps of different types
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="label"></param>
        public void Jump(OpCode opCode,string label)
        {
            Debug($"{opCode} {label}");
            Add(opCode);
            if (!Labels.ContainsKey(label))
            {
                Labels.Add(label, PLACEHOLDER_LABEL);
                undefinedLabels.Add(Bytecode.Count, label);
            }
            Add(Labels[label]);
        }

        public void Jump(string label) =>
            Jump(OpCode.JMP, label);

        public void JumpNotZero(string label) =>
            Jump(OpCode.JNE, label);

        public void JumpCond(string label) =>
            Jump(OpCode.JMPCMP, label);

        public void App()
        {
            Debug("APP");
            Add(OpCode.APP);
            Add(0);
        }
            
    


        /// <summary>
        /// Closure, first class functions
        /// </summary>
        /// <param name="name"></param>
        public void Closure(string function, params string[] upvalues)
        {
            Jump(OpCode.CLOSURE, function);
            Add(upvalues.Length);
            foreach (var symbol in upvalues)
            {
                Bytecode.Add(new Value(ValueType.UPVALUE, Symbols.IndexOf(symbol)));
            }
        }

        public void Ret()
        {
            Debug("RET");
            Add(OpCode.RET);
        }
        public void Print()
        {
            Debug("PRINT");
            Add(OpCode.PRINT);
        }

        public void Debug()
        {
            Debug("DEBUG");
            Add(OpCode.DEBUG);
        }




        public void Quit()
        {
            Debug("QUIT");
            Add(OpCode.QUIT);
        }





        public Bytecode Compile()
        {
            for (int i = 0; i < Bytecode.Count; i++)
            {
                //global variables and methods can be used before being defined
                //check for those that were never defined
                if ((OpCode)Bytecode[i].i32 == OpCode.CLOSURE ||
                    (OpCode)Bytecode[i].i32 == OpCode.JMPCMP  ||
                    (OpCode)Bytecode[i].i32 == OpCode.JMP)
                {
                    //symbol reference
                    int labelReference = Bytecode[++i].i32;
                    if (labelReference == PLACEHOLDER_LABEL) //placeholder label
                    {
                        string name = undefinedLabels[i];
                        int newReference = Labels[name];
                        if (newReference != 0)
                        {
                            Bytecode[i] = new Value(ValueType.INT, newReference);
                            undefinedLabels.Remove(i);
                        }
                    }
                }
            }

            return new Bytecode(
                bytecode: Bytecode.ToArray(), 
                symbols: Symbols.ToArray(), 
                isDebugging: IsDebugging, 
                debugInfo: DebugInfo);
        }

    };
}
