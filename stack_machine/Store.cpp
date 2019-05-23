#include "StackMachine.h"


namespace VM
{
	Store::Store() {}

	std::size_t Store::add(value_t value) {
		auto ptr = std::make_shared<value_t>();
		ptr->i32 = value.i32;
		ptr->type = value.type;
		this->storage.push_back(ptr);
		return this->storage.size()-1;
	}

	void Store::remove(std::size_t reference) {
		auto it = this->storage.begin();
		advance(it, reference);
		this->storage.erase(it);
	}

	value_t* Store::get(std::size_t reference) {
		return this->storage.at(reference).get();
	}

	void Store::print() {
		/*for (auto& value : this->storage) {
			printf("%d", value.get()->type);
		}*/
	}
}