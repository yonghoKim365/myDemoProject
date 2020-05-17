#pragma once



#include <string>
#include <vector>
#include <cstdarg>
using namespace std;

#include "cocos2d.h"
USING_NS_CC;

class KStringUtil  {
public:
	static void tokenize(const string& str, vector<string>& tokens, const string& delimiters = ",");
	static std::string format(const std::string fmt_str, ...);
	static string replaceAll(const string &str, const string &pattern, const string &replace);
};