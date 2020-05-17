#include "KJSONDataManager.h"

#include "cocostudio/DictionaryHelper.h"

USING_NS_CC;

using namespace cocostudio;
#include "json/document.h"
#include "json/filestream.h"
#include "json/prettywriter.h"
#include "json/stringbuffer.h"


KJSONDataManager * KJSONDataManager::_instance = nullptr;

std::string			KJSONDataManager::sTargetName = "aa.json";

KJSONDataManager * KJSONDataManager::getInstance(){
	if (_instance == nullptr) {
		_instance = new KJSONDataManager();
		_instance->initData();
	}
	return _instance;
}
void KJSONDataManager::removeInstance() {
	if (_instance == nullptr) return;
	delete _instance;
	_instance = nullptr;
}


void KJSONDataManager::initData() {
	std::string contentStr = FileUtils::getInstance()->getStringFromFile(sTargetName);
	mDocument.Parse<0>(contentStr.c_str());
	if (mDocument.HasParseError()) {
		log("There is no data in the json data..");
		return;
	}
	//데이터가 있으면 map에 데이터를 넣자..
	if (mDocument.HasMember(JSON_CAMERA_SECTION) && mDocument[JSON_CAMERA_SECTION].IsArray()) {
		const rapidjson::Value & arr = mDocument[JSON_CAMERA_SECTION];

		
		for (int i = 0; i < arr.Size(); i++) {
			const rapidjson::Value & v = arr[i];
			for (rapidjson::Value::ConstMemberIterator iter = v.MemberBegin(); iter != v.MemberEnd(); iter++) {
				//log("name=%s value=%s", iter->name.GetString(), iter->value.GetString());
				mCameraMap.insert(std::pair<std::string, std::string>(iter->name.GetString(), iter->value.GetString()));
			}
		}
	}

	/*std::map<std::string, std::string>::iterator mapiter;
	for (mapiter = mCameraMap.begin(); mapiter != mCameraMap.end(); mapiter++) {
		log("first=%s", mapiter->first.c_str());
		log("second=%s", mapiter->second.c_str());
		log("---------------------------");
	}
*/


}

std::string  KJSONDataManager::getCameraData(std::string sKey){
	std::map<std::string, std::string>::iterator iter;

	iter = mCameraMap.find(sKey);
	if (iter != mCameraMap.end()) {
		return iter->second;
	}
	return "";
}

void KJSONDataManager::setCameraData(std::string sKey, std::string sValue){
	mCameraMap[sKey] = sValue;
	saveData();
}

void KJSONDataManager::saveData() {

	//mDocument.RemoveAllMembers();
	mDocument.SetObject();

	rapidjson::Value::AllocatorType & allocator = mDocument.GetAllocator();

	rapidjson::Value cameraArray(rapidjson::kArrayType);
	
	for (auto mapiter = mCameraMap.begin(); mapiter != mCameraMap.end(); mapiter++) {
		rapidjson::Value mapItem(rapidjson::kObjectType);
		mapItem.AddMember(rapidjson::Value(mapiter->first.c_str(),allocator)
			,rapidjson::Value(mapiter->second.c_str(), allocator), allocator);
		cameraArray.PushBack(mapItem, allocator);
		//log("first=%s", mapiter->first.c_str());
		//log("second=%s", mapiter->second.c_str());
		//log("---------------------------");
	}
	mDocument.AddMember(rapidjson::Value(JSON_CAMERA_SECTION, allocator), cameraArray, allocator);

	rapidjson::StringBuffer strbuf;
	rapidjson::Writer<rapidjson::StringBuffer> writer(strbuf);
	mDocument.Accept(writer);

	log("jsonData= %s", strbuf.GetString());
	FileUtils::getInstance()->writeStringToFile(strbuf.GetString(), sTargetName);
}