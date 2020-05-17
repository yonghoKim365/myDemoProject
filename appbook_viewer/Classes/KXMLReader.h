#pragma once

#include "cocos2d.h"
#include "../../cocos2d/external/tinyxml2/tinyxml2.h"
#include "CCFileUtils.h"
#include "Page.h"
#include <string>

USING_NS_CC;
using namespace tinyxml2;


#define BOOK_INFO "book_info"
#define POPUPS    "popups"
#define PAGES	  "pages"

class KXMLReader  {
public:
	KXMLReader();
	~KXMLReader();
public:
	static KXMLReader* getInstance();
	static void removeInstance();

	bool parseFileForBookInfo();
	bool parseFileOtherThanBookInfo();
	
	/* XML에 정의된 Width */
	int getContentWidth() {
		return mBookInfo.width;
	}
	/* XML에 정의된 Height*/
	int getContentHeight() {
		return mBookInfo.height;
	}

	void setXmlData(const std::string&  aData) {
		mXmlData = aData;
	}
private:
	
	std::string mXmlData;
	
private:
	static KXMLReader * _instance;
	tinyxml2::XMLDocument mDoc;
	void initData();
	void parseBookInfo(XMLElement * pElement);
	void parsePopups(XMLElement * pElement);
	void parsePopupsInside(XMLElement * pElement, HPOPUP_INFO * pPopupInfo );
	void parsePages(XMLElement * pElement);
	void parsePagesInside(XMLElement * pElement,  HPAGE_INFO * pPageInfo);
	void parsePagesInsideContents(XMLElement * pElement, HPAGE_INFO * pPageInfo);

public:
	int getPageCount();
	HPAGE_INFO * getPageReference(int nPage);
	HPOPUP_INFO * getPopupReference(string sID);
	int getPopupIndexFromTargetID(string targetID);
	STBOOK_INFO * getBookInfo();
	int getPopupCount();
	HPOPUP_INFO * getPopupReference(int nIndex);
private:
	STBOOK_INFO mBookInfo;           /*book 정보 관리*/
	Vector<HPOPUP_INFO *>  mPopups;  /*페이지 정보 관리*/
	Vector<HPAGE_INFO *>   mPages;   /*팝업정보 관리*/

private:
	FileUtils * mpFileUtils;

private:
	void setPropertyTo_HPOPUP_INFO(XMLElement * pElem, HPOPUP_INFO *pPopupInfo );
	void setPropertyTo_HPOPUP_INFO_Contents(XMLElement * pElem, STCONTENT_INFO *pContentInfo);

	void setPropertyTo_HPAGE_INFO(XMLElement * pElem, HPAGE_INFO * pPageInfo);
	void setPropertyTo_HPAGE_INFO_Contents(XMLElement * pElem, STCONTENT_INFO *pContentInfo);

	void setPropertyTo_Contents(XMLElement * pElem, STCONTENT_INFO *pContentInfo);
	
	void setPropertyTo_Contents_Animation(XMLElement * pElem, STCONTENT_INFO *pContentInfo);

};