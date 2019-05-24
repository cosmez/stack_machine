#pragma once
#include <string>
#include <map>
#include  "Environment.h"
#include "Bytecode.h"

namespace VM
{


	enum OPCODE {
		OP_PUSH = 1,
		OP_POP,
		OP_POPT, //pops (sizeof(type)) N length bytes
		OP_POP2, //pops 2 length bytes
		OP_POP4, //pops 4 length bytes
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

	enum TYPE {
		//value types
		TYPE_INT = 0,
		TYPE_NUMBER,
		TYPE_BOOL,
		TYPE_CHAR,
		TYPE_SYMBOL,
		TYPE_STRING, //reference types
		TYPE_FUNCTION, //closure
		TYPE_PAIR
	};

	struct value_t {
		TYPE type;
		union {
			int i32;
			float fl;
			char c;
			std::size_t car; //non specialized type
		};
		std::size_t cdr; //pointer to the next location value
	};

	typedef std::vector<value_t> stack_t;


	class Store {
	private:
		std::vector<value_t> storage;
	public:
		Store();
		void print();
		std::size_t add(value_t value);
		void remove(std::size_t reference);
		value_t get(std::size_t reference);
	};


	class Environment
	{
	private:
		std::map<std::string, std::size_t> env;
		std::shared_ptr<Store> store;
	public:
		Environment(const std::shared_ptr<Store> store);
		void add(std::string name, value_t value);
		value_t lookup(std::string name);
		void print();
	};

	struct Bytecode {
		std::vector<std::size_t> Instructions;
		std::vector<std::string> Strings;
	};

	class BytecodeWriter {
	public:
		std::vector<std::size_t> Bytecode;
		std::vector<std::string> Symbols;

		BytecodeWriter();

		void Push(value_t value);
		void Pop();
		void Dup();

		void Store(std::string name);
		void Lookup(std::string name);

		///write a label to jump back to it
		void Label(std::string name);
		void App(std::string name); //call a function
		void Ret(); //return

		//print & debug
		void Print();
		void Debug();

		//math opcodes
		void Add();
		void Sub();
	};

	class StackMachine
	{
	private:
		
		stack_t _stack;
		//program counter
		std::size_t _pc;
		//frame pointer
		std::size_t _fp;

	public:
		StackMachine();
		~StackMachine();
		void execute(Bytecode bytecode, Environment environment);
		void push(value_t value);
		value_t pop();
		value_t peek(std::size_t position);
		void top(std::size_t position);
		std::size_t top();
		void print();
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