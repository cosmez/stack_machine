#include "StackMachine.h"

namespace VM
{

	StackMachine::StackMachine() {
		this->_pc = 0;
		this->_fp = 0;
	}

	StackMachine::~StackMachine() {

	}

	void StackMachine::execute(Bytecode bytecode, Environment environment)
	{
		while (this->_pc < bytecode.Instructions.size())
		{
			printf("Bytecode: pc: %d\t instr: %d\n", this->_pc,bytecode.Instructions[this->_pc]);
			auto OPCODE = static_cast<int>(bytecode.Instructions[this->_pc++]);
			switch (OPCODE)
			{
				case OP_PUSH:
				{
					value_t value;
					value.i32 = static_cast<int> (bytecode.Instructions[this->_pc++]);
					value.type = TYPE_INT;
					this->push(value);
					break;
				}
				case OP_POP:
				{
					pop();
					break;
				}
				case OP_DUP:
				{
					auto value = pop();
					this->push(value);
					this->push(value);
					break;
				}
				case OP_PRINT:
				{
					auto value = pop();
					printf("%d\n", value.i32);
					break;
				}
				case OP_DEBUG:
					this->print();
					environment.print();
					break;
				case OP_STORE:
				{
					const std::string storeLiteral = bytecode.Strings[bytecode.Instructions[this->_pc++]];
					environment.add(storeLiteral, pop());
					break;
				}
				case OP_LOOKUP:
				{
					auto lookupLiteral = bytecode.Strings[bytecode.Instructions[this->_pc++]];
					this->push(environment.lookup(lookupLiteral));
					break;
				}
				case OP_ADD:
				{
					auto value1 = pop();
					auto value2 = pop();
					if (value1.type == TYPE_INT && value2.type == TYPE_INT)
					{
						value_t result{ TYPE_INT, value1.i32 + value2.i32 };
						this->push(result);
					}
					else if (value1.type == TYPE_NUMBER && value2.type == TYPE_NUMBER)
					{
						value_t result{ TYPE_NUMBER, value1.fl + value2.fl };
						this->push(result);
					}
					else {
						perror("Invalid types for OP_ADD");
					}
					break;
				}
				case OP_SUB:
				{
					auto value2 = pop();
					auto value1 = pop();
					if (value1.type == TYPE_INT && value2.type == TYPE_INT)
					{
						value_t result{ TYPE_INT, value1.i32 - value2.i32 };
						this->push(result);
					}
					else if (value1.type == TYPE_NUMBER && value2.type == TYPE_NUMBER)
					{
						value_t result{ TYPE_NUMBER, value1.fl - value2.fl };
						this->push(result);
					}
					else {
						perror("Invalid types for OP_SUB");
					}
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
					auto value = pop();
					if (value.i32) {
						this->_pc = _newPC;
					}
					break;
				}
				//function call
				case OP_APP:
				{
					//point frame pointer to top of the stack
					this->_fp = this->top();
					//store return address after frame pointer
					value_t value;
					value.type = TYPE_SYMBOL;
					value.ref = this->_pc + 1;
					this->push(value);

					//here we have to switch the environment
					//move to procedure location
					this->_pc = bytecode.Instructions[this->_pc]; 

					//push
					break;
				}
				//function return, functions should always return something
				case OP_RET:
				{
					//here we have to restore the environment
					auto returnValue = pop();
					this->_pc = peek(this->_fp).ref; //move the pc to previous location
					this->top(this->_fp); //return to previous location in stack
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
	void StackMachine::push(value_t value)
	{
		_stack.push_back(value);
	}


	void StackMachine::top(std::size_t position)
	{
		_stack.resize(position);
	}

	std::size_t StackMachine::top()
	{
		return _stack.size();
	}



	/**
	 * pop the stack contents
	 * */
	value_t StackMachine::pop()
	{
		auto value = _stack[_stack.size() - 1];
		_stack.pop_back();
		return value;
	}


	value_t StackMachine::peek(std::size_t position)
	{
		return this->_stack[position];
	}


	/**
	 * Print the contents of the stack
	 * */
	void StackMachine::print() {
		
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
