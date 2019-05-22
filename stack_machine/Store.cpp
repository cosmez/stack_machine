#include "Store.h"


namespace VM
{
	Store::Store() {}


	std::size_t Store::add(const std::any value, TYPE type) {
		auto ptr = std::make_shared<Value>();
		ptr->data = value;
		ptr->type = type;
		this->storage.push_back(ptr);
		return this->storage.size()-1;
	}

	void Store::remove(std::size_t reference) {
		auto it = this->storage.begin();
		advance(it, reference);
		this->storage.erase(it);
	}

	Value* Store::get(std::size_t reference) {
		return this->storage.at(reference).get();
	}

	void Store::print() {
		/*for (auto& value : this->storage) {
			printf("%d", value.get()->type);
		}*/
	}
}