#include <string>
#include <vector>
#include <any>
#include "Store.h"
#pragma once

namespace VM
{
	struct Bytecode {
		std::vector<size_t> Instructions;
		std::vector<std::string> Strings;
	};

	class BytecodeWriter {
	public:
		std::vector<std::size_t> Bytecode;
		std::vector<std::string> Symbols;
		
		BytecodeWriter();

		void Push(std::any value, TYPE type);
		void Pop();
		void Dup();
		void Debug();
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
}