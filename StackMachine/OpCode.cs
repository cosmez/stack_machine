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
        TRUE,  //pushes true on top of the stack
        FALSE, //pushes false on top of the stack
        NIL, //pushes nil on top of the stack
        
        POP, //pops a Value from the stack
        POPT, //pops (sizeof(type)) N length bytes, not used
        POP2, //pops 2 length bytes, not used
        POP4, //pops 4 length bytes, not used

        DUP, //duplicates the contents at the top of the stack
        ADD, //pops 2 values from the stack and then pushes the result
        ADD1, //pop 1  value, add 1 and push back result
        SUB, //pop2  value, substracts and push back result
        SUB1, //pop 1 value, subtract 1 and push back result
        MUL, //pops 2 values, multiply and then push back result
        DIV, //pops 2 values, divide and then push back result
        //booleans
        GT, 
        LT,

        STORE, //stores a new variable in the environment with the pop of the stack
        LOOKUP_LOCAL, //looks for a variable in the env and pushes it at the top of the stack
        LOAD_REFERENCE, //Gets a refenrece pointer from the stack and pushes the value at that position into the stack
        STORE_REFERENCE,
        MODULE, //module
        LOOKUP_GLOBAL, // globals do not exist, theyre just local to modules
        PRINT,
        DEBUG, //prints debug information
        JMP, //jump
        JMPCMP, //conditional jump
        CLOSURE, //create closure
        APP, //a function call
        RET, //return from function call

        //Arrays
        ARRAY_INIT, //inits and array in the heap
        ARRAY_GET, //pushes the i element from the array into the stack
        ARRAY_SET, //pops an index and value and puts the value into the n element of the array


        QUIT, //end the program
        JNE
    }





}
