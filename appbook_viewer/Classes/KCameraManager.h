#pragma once

#include "cocos2d.h"
#include "CCFileUtils.h"
#include "ui/CocosGUI.h"
#include <string>
USING_NS_CC;

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
#include "jni.h"
#include "jni/JniHelper.h"
#define KCAMERA_JAVA_REFERENCE_CLASS "com/kid/KCameraManager"
#endif

class KCameraManagerDelegate {
public:
	virtual void onRefreshUI(std::string filename) = 0;
};


class KCameraManager  {

public:
	static KCameraManager* getInstance();
	static void removeInstance();

private:
	static KCameraManager * _instance;
	KCameraManagerDelegate * delegate;

public:
	void setDelegate(KCameraManagerDelegate * delegate);
	void setActionTargetName(std::string aTargetName);
	void setActionTargetLayer(Layer * pLayer);

	Node * getActionTargetNode();
	Layer * getActionTargetLayer();
	Sprite * getSprite();
	std::string getAndroidTargetPath();
	void setTextFieldObject(ui::TextField* aTextField);
	ui::TextField* getTextFieldObject();
	void determineFullpathnameForPicture(std::string sProjectcode);
	std::string getFullpathnameForPicture();
private:
	void initData();
	std::string sActionTargetName;
	Layer * mpActionTargetLayer;
	Sprite * mpSprite;
	std::string mAndroidTargetPath;
	ui::TextField* mTextField;
	std::string mFileFullPath;

#if (CC_TARGET_PLATFORM != CC_PLATFORM_WIN32) 
private:
	jobject mRefJObj;
#endif

public:
	void requestPicture(std::string sMessage);
	void refreshUI(std::string aFilename);
};