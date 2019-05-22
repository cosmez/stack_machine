// stack_machine.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "StackMachine.h"



namespace VM
{

	StackMachine::StackMachine() {
		this->_pc = 0;
		this->_fp = 0;
		this->_stack = new Stack();
	}

	StackMachine::~StackMachine() {
		delete this->_stack;
	}

	void StackMachine::execute(Bytecode bytecode, Environment environment)
	{
		while (this->_pc < bytecode.Instructions.size())
		{
			printf("Bytecode: pc: %d\t instr: %d\n", this->_pc,bytecode.Instructions[this->_pc]);
			switch (bytecode.Instructions[this->_pc++])
			{
				case OP_PUSH:
					this->push(bytecode.Instructions[this->_pc++]);
					break;
				case OP_POP:
					this->pop();
					break;
				case OP_DUP:
				{
					auto value = this->pop();
					this->push(value);
					this->push(value);
					break;
				}
				case OP_PRINT:
				{
					auto value = this->pop();
					printf("%d\n", value);
					break;
				}
				case OP_DEBUG:
					this->print();
					environment.print();
					break;
				case OP_STORE:
				{
					const std::string storeLiteral = bytecode.Strings[bytecode.Instructions[this->_pc++]];
					environment.add(storeLiteral, this->pop(), TYPE_INT);
					break;
				}
				case OP_LOOKUP:
				{
					const auto lookupLiteral = bytecode.Strings[bytecode.Instructions[this->_pc++]];
					const auto lookupVal = environment.lookup(lookupLiteral);
					this->push(std::any_cast<int>(lookupVal->data));
					break;
				}
				case OP_ADD:
					this->push(this->pop() + this->pop());
					break;
				case OP_SUB:
				{
					const auto val2 = this->pop();
					this->push(this->pop() - val2);
					break;
				}
				//always jump to next value
				case OP_JMP:
					this->_pc = bytecode.Instructions[this->_pc++];
					break;
				//conditional jump, jumps to next address if the top of the stack is true
				case OP_JMPCMP: 
				{
					auto _newPC = bytecode.Instructions[this->_pc++];
					if (this->pop()) {
						this->_pc = _newPC;
					}
					break;
				}
				//function call
				case OP_APP:
				{
					//here we have to switch the environment
					this->_fp = this->_stack->top; //point frame pointer to top of the stack
					this->push(this->_pc); //store return address after frame pointer
					this->_pc = bytecode.Instructions[this->_pc++]; //push current location
					//push
					break;
				}
				//function return, functions should always return something
				case OP_RET:
				{
					//here we have to restore the environment
					auto returnValue = this->pop();
					this->_pc = this->peek(this->_fp); //move the pc to previous location
					this->_stack->top = this->_fp; //return to previous location in stack
					this->push(returnValue);
					break;
				}
				case OP_QUIT:
					return;
					break;
			}

		}
	}

	/***
	 * pushes the stack content
	 * */
	void StackMachine::push(int value)
	{
		_stack->content[_stack->top++] = value;
	}

	/***
 * pushes the stack content
 * */
	int StackMachine::peek(std::size_t position)
	{
		return _stack->content[position];
	}


	/**
	 * pop the stack contents
	 * */
	int StackMachine::pop()
	{
		return _stack->content[--_stack->top];
	}


	/**
	 * Print the contents of the stack
	 * */
	void StackMachine::print() {
		printf("[");
		for (int i = 0; i < STACK_MAX; i++) {
			if (i == _stack->top) {
				printf("<");
			}
			printf("%d ", _stack->content[i]);

		}
		printf("]\n");
	}


	/**
	 * Pops 2 elements from the stack, apply the function and push it back
	 * */
	int StackMachine::StackMachine::apply(int (*binary_func)(int, int)) {
		int val1 = pop();
		int val2 = pop();
		int sum = binary_func(val1, val2);
		push(sum);
		return sum;
	}

	/**
	 * Binary functions used by opcodes
	 * */
	int StackMachine::add_s(int a, int b) { return a + b; }
	int StackMachine::mul_s(int a, int b) { return a * b; }
	int StackMachine::sub_s(int a, int b) { return a - b; }
	int StackMachine::div_s(int a, int b) { return a / b; }
	int StackMachine::lgtn_s(int a, int b) { return a > b; }
	int StackMachine::eq_s(int a, int b) { return a == b; }
	int StackMachine::smtn_s(int a, int b) { return a < b; }


}
