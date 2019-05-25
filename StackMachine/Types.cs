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
        //value types
        INT = 0,
        NUMBER,
        BOOL,
        CHAR,
        SYMBOL,
        NIL, //null
        STRING, //reference types
        CLOSURE, //closure
        PAIR,
        VECTOR,
        STRUCT,

        //Internal use only
        UPVALUE
    };

    /// <summary>
    /// This is the basic Value, it supports basic Value types and a CDR pointer to the next element
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct Value
    {
        [FieldOffset(0)]
        public ValueType type;
        //pointer or value of the first field
        [FieldOffset(sizeof(ValueType))]
        public int i32; 
        [FieldOffset(sizeof(ValueType))]
        public float fl;
        [FieldOffset(sizeof(ValueType))]
        public char c;
        [FieldOffset(sizeof(ValueType))]
        public bool b;

        public override String ToString()
        {
            switch (type)
            {
                case ValueType.INT:
                    return $"n: {i32}";
                case ValueType.BOOL:
                    return $"b: {b}";
                case ValueType.CHAR:
                    return $"c: {c}";
                case ValueType.NUMBER:
                    return $"f: {fl}";
                default:
                    return $"reference {type.ToString()} {i32}";
            }
        }
    };


    

}
