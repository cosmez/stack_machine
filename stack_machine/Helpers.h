#pragma once

#include <string>

/***
 * Quick and Simple hash implementation
 * */
std::size_t hash(std::string str)
{
	return std::hash<std::string>{}(str);
}