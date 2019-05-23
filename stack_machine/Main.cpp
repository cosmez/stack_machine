#include "StackMachine.h"

/***
 * TODO:
 * [X] Basic Stack Operations
 * [X] Math Operations
 * [X] Basic Environment
 * [X] Grow Environment
 * [X] Bytecode OP CODES
 * [X] Interpreter
 * [X] Symbols
 * [x] Functions
 * [ ] Bytecode Writer
 * [ ] Debug Information
 * [ ] Untyped Stack
 * [ ] Symbols
 * [ ] Chars
 * [ ] Closures
 * */


int main()
{
	using namespace VM;

	Bytecode bytecode{ {
		OP_PUSH, 10,   //0.
		OP_PUSH, 5,    //2.
		OP_PUSH, 7,    //4.
		OP_PUSH, 9,    //6.
		OP_POP,        //8.
		OP_ADD,        //9.
		OP_DEBUG,      //10.
		OP_STORE, 0,   //11. val1 = top of the stack.
		OP_PUSH, 17,   //13. 
		OP_LOOKUP, 0,  //15. top of the stack = val1
		OP_ADD,        //17. 
		OP_DEBUG,      //18. 
		OP_PUSH, 1,    //19
		OP_SUB,        //21
		OP_DUP,        //22
		OP_PRINT,      //23
		OP_DUP,        //24
		OP_JMPCMP, 18, //25  jump to 18
		OP_APP, 30,    //27
		OP_QUIT,       //28
		BLOCK,         //29 function block, capture environment ? (val1)
		OP_PUSH, 10,   //30
		OP_PRINT,      //32
		OP_PUSH, 0,    //33 value to return
		OP_RET,        //35 return 0
		ENDBLOCK       //36
	}, {
		"val1", "print10"
	} };

	
	StackMachine stack;
	const auto shared_store = std::make_shared<Store>();

	//global store for now
	Environment env(shared_store);

	stack.execute(bytecode, env);

	return 0;
}