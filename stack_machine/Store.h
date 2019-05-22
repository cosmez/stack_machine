#pragma once
#include <any>
#include <vector>
#include <memory>

namespace VM
{
	enum TYPE {
		TYPE_INT = 0,
		TYPE_NUMBER,
		TYPE_CHAR,
		TYPE_STRING,
		TYPE_SYMBOL,
		TYPE_FUNCTION, //closure
		TYPE_PAIR
	};

	struct Value {
		TYPE type;
		std::any data;
	};


	///TODO: this implementation is good at accesing and creating new elements, but not at removing them
	///look for a better data structure
	class Store {
	private:
		std::vector<std::shared_ptr<Value>> storage;
	public:
		Store();
		void print();
		std::size_t add(const std::any value, TYPE type);
		void remove(std::size_t reference);
		Value* get(std::size_t reference);
	};
}