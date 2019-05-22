#pragma once
#include <memory>
#include <map>
#include <string>
#include "Store.h"

namespace VM
{
	class Environment
	{
	private:
		std::map<std::string, std::size_t> env;
		std::shared_ptr<Store> store;
	public:
		Environment(const std::shared_ptr<Store> store);
		void add(std::string name, std::any value, TYPE type);
		Value* lookup(std::string name);
		void print();
	};

}