using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StackMachine
{
    enum OpCode
    {
        PUSH = 1,
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
        LOOKUP, //looks for a variable in the env and pushes it at the top of the stack
        PRINT,
        DEBUG, //prints debug information
        JMP, //jump
        JMPCMP, //conditional jump
        BLOCK, //closure
        ENDBLOCK, //end closure
        APP, //a function call
        RET, //return from function call
        QUIT //end the program
    };

    enum ValueType
    {
        //value types
        INT = 0,
        NUMBER,
        BOOL,
        CHAR,
        SYMBOL,
        STRING, //reference types
        FUNCTION, //closure
        PAIR
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
            return $"Type: {type}, Value: [${i32}/${fl}/${c}]";
        }
    };


    

}
