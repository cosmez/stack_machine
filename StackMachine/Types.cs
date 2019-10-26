using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StackMachine
{
    enum OpCode
    {
        NOP = 0,
        PUSH,
        TRUE,  //pushes nil on top of the stack
        FALSE, //pushes false on top of the stack
        NIL, //pushes nil on top of the stack
        PUSH_ARRAY,
        POP,
        POPT, //pops (sizeof(type)) N length bytes
        POP2, //pops 2 length bytes
        POP4, //pops 4 length bytes

        DUP, //duplicates the contents at the top of the stack
        ADD,
        ADD1,
        SUB,
        SUB1,
        MUL,
        DIV,
        GT,
        LT,
        STORE, //stores a new variable in the environment with the pop of the stack
        LOOKUP_LOCAL, //looks for a variable in the env and pushes it at the top of the stack
        MODULE, //module
        LOOKUP_GLOBAL, // globals do not exist, theyre just local to modules
        PRINT,
        DEBUG, //prints debug information
        JMP, //jump
        JMPCMP, //conditional jump
        CLOSURE, //create closure
        APP, //a function call
        RET, //return from function call
        QUIT, //end the program
        JNE
    };

    enum ValueType
    {
        NIL = 0,
        INT,
        NUMBER,
        BOOL,
        CHAR,
        SYMBOL,
        STRING, //reference types
        CLOSURE, //closure
        PAIR,
        ARRAY,
        STRUCT,

        //Internal use only
        UPVALUE
    };

    /// <summary>
    /// This is the basic Value, it supports basic Value types and a CDR pointer to the next element
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    readonly struct Value
    {
        [FieldOffset(0)]
        public readonly ValueType type;
        //pointer or value of the first field
        [FieldOffset(sizeof(ValueType))]
        public readonly int i32; 
        [FieldOffset(sizeof(ValueType))]
        public readonly float fl;
        [FieldOffset(sizeof(ValueType))]
        public readonly char c;
        [FieldOffset(sizeof(ValueType))]
        public readonly bool b;

        private static readonly Value nilValue = new Value(ValueType.NIL, 0);
        private static readonly Value trueValue = new Value(true);
        private static readonly Value falseValue = new Value(false);
        public static ref readonly Value Nil => ref nilValue;
        public static ref readonly Value True => ref trueValue;
        public static ref readonly Value False => ref falseValue;


        ///why this() ?
        ///https://github.com/dotnet/roslyn/issues/7323
        public Value(ValueType type,  int value) : this()
        {
            this.i32 = value;
            this.type = type;
        }

        public Value(int value) : this()
        {
            this.i32 = value;
            this.type = ValueType.NUMBER;
        }

        public Value(float value) : this()
        {
            this.fl = value;
            this.type = ValueType.NUMBER;
        }

        public Value(bool value) : this()
        {
            this.b  = value;
            this.type = ValueType.BOOL;
        }

        public Value(char value) : this()
        {
            this.c = value;
            this.type = ValueType.CHAR;
        }

        public readonly override String ToString()
        {
            return type switch
            {
                ValueType.INT => $"n: {i32}",
                ValueType.BOOL => $"b: {b}",
                ValueType.CHAR => $"c: {c}",
                ValueType.NUMBER => $"f: {fl}",
                _ => $"reference {type.ToString()} {i32}",
            };
        }
    };


    

}
