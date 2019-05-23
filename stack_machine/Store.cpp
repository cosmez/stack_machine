#include "StackMachine.h"


namespace VM
{
	Store::Store() {}

	std::size_t Store::add(value_t value) {
		this->storage.push_back(value);
		return this->storage.size()-1;
	}

	void Store::remove(std::size_t reference) {
		auto it = this->storage.begin();
		advance(it, reference);
		this->storage.erase(it);
	}

	value_t Store::get(std::size_t reference) {
		return this->storage.at(reference);
	}

	void Store::print() {
		/*for (auto& value : this->storage) {
			printf("%d", value.get()->type);
		}*/
	}
}