#ifndef __KJSONDataManager_H__
#define __KJSONDataManager_H__

#include "cocos2d.h"
#include "json/document.h"
#include <map>

using namespace std;

#define JSON_CAMERA_SECTION "camera"

class KJSONDataManager 
{
public:
	static KJSONDataManager* getInstance();
	static void removeInstance();
	static std::string sTargetName;
private:
	static KJSONDataManager * _instance;
	void initData();
	rapidjson::Document mDocument;

	void saveData();

private:
	std::map<std::string, std::string> mCameraMap;
public:
	std::string getCameraData( std::string sKey);
	void setCameraData( std::string sKey,std::string sValue);



};

#endif // __KJSONDataManager_H__
