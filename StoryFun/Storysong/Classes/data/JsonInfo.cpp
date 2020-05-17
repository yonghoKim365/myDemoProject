#include "JsonInfo.h"

JsonInfo * JsonInfo::obj = NULL;

JsonInfo::JsonInfo() {}
JsonInfo::~JsonInfo(){}


JsonInfo * JsonInfo::create()
{
	if (obj == NULL)
		obj = new JsonInfo();

	return obj;
}

void JsonInfo::releaseInstance()
{
	if (obj != NULL)
		delete obj;
}

GAFAsset * JsonInfo::getGafAsset(int index)
{
	std::string gafPath = trackExampleList.at(index);

	return GAFAsset::create(gafPath.c_str(), nullptr);
}

void JsonInfo::setStartStep(int step)
{
	startStep = step;
}

int JsonInfo::getStartStep()
{
	return startStep;
}