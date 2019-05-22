/**
Environment implementation
Holds references to a store, the store is stored locally in a shared pointer
**/
#include "Environment.h"

namespace VM
{

	Environment::Environment(const std::shared_ptr<Store> store) {
		this->store = store;
	}

	void Environment::add(std::string name, std::any value, TYPE type) {
		auto reference = this->store->add(value, type);
		this->env.insert(std::pair<std::string, std::size_t>(name, reference));
	}

	/***
	 * Look up a variable name in the environment
	 * */
	Value* Environment::lookup(std::string name) {
		auto reference = (this->env).at(name);
		auto value = this->store->get(reference);
		return value;
	}

	void Environment::print() {
		for (auto& binding : this->env) {
			printf("%s = %d\n", binding.first.c_str(), binding.second);
		}
	}

}