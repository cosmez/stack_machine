#include <iostream>
#include <string>
#include <map>
#include  "Environment.h"
#include "Bytecode.h"
#pragma once
namespace VM
{
	enum OPCODE {
		OP_PUSH = 1,
		OP_POP,
		OP_DUP, //duplicates the contents at the top of the stack
		OP_ADD,
		OP_SUB,
		OP_MUL,
		OP_DIV,
		OP_GT,
		OP_LT,
		OP_STORE, //stores a new variable in the environment with the pop of the stack
		OP_LOOKUP, //looks for a variable in the env and pushes it at the top of the stack
		OP_PRINT,
		OP_DEBUG, //prints debug information
		OP_JMP, //jump
		OP_JMPCMP, //conditional jump
		BLOCK, //closure
		ENDBLOCK, //end closure
		OP_APP, //a function call
		OP_RET, //return from function call
		OP_QUIT //end the program
	};
#define STACK_MAX 32
	struct Stack {
		int content[STACK_MAX];
		int top;
	};




	class StackMachine
	{
	private:
		
		Stack* _stack;
		//program counter
		std::size_t _pc;
		//frame pointer
		std::size_t _fp;

	public:
		StackMachine();
		~StackMachine();
		void execute(Bytecode bytecode, Environment environment);
		void push(int value);
		int pop();
		int peek(std::size_t position);
		void print();
		int apply(int (*binary_func)(int, int));
		//stack functions
		/**
		 * math operations on integers
		 * Binary functions used by opcodes
		 * */
		int add_s(int a, int b);
		int mul_s(int a, int b);
		int sub_s(int a, int b);
		int div_s(int a, int b);
		int lgtn_s(int a, int b);
		int eq_s(int a, int b);
		int smtn_s(int a, int b);
	};
}