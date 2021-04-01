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

    


    

}
