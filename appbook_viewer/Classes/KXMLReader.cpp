#pragma warning(disable:4819)

#include "KXMLReader.h"

/* XML 파싱한다. */

/*생성자*/
KXMLReader::KXMLReader() {
	log("KXMLReader::KXMLReader()");
	mXmlData = "";
	mpFileUtils = FileUtils::getInstance();
}
/*소멸자*/
KXMLReader::~KXMLReader(){
	log("KXMLReader::~KXMLReader()");
	mpFileUtils->destroyInstance();
	mpFileUtils = nullptr;
}

KXMLReader * KXMLReader::_instance = nullptr;

/*KXMLReader getInstance return 한다.*/
KXMLReader * KXMLReader::getInstance(){
	if (_instance == nullptr) {
		_instance = new KXMLReader();
		_instance->initData();
	}
	return _instance;
}

/*KXMLReader Instance를 제거한다.*/
void KXMLReader::removeInstance() {
	if (_instance == nullptr) return;
	delete _instance;
	_instance = nullptr;
}

void KXMLReader::initData() {
	log("KXMLReader::initData()....");
}

/* 전체 페이지 수를 리턴한다. */
int KXMLReader::getPageCount(){
	return mPages.size();
}

/* @nPage 에 해당하는 페이지 정보를 리턴한다. */
HPAGE_INFO * KXMLReader::getPageReference(int nPage){
	if (nPage >= getPageCount()) return nullptr;
	return mPages.at(nPage);
}

/* @sID에 해당하는 팝업 정보를 리턴한다. */
HPOPUP_INFO * KXMLReader::getPopupReference(string sID){
	for (int i = 0; i < mPopups.size(); i++) {
		//log("id=%s", mPopups.at(i)->id.c_str());
		if (mPopups.at(i)->id == sID) return mPopups.at(i);
	}
	return nullptr;
}

/* 백터에 저장되어 있는 팝업중 @targetID 해당하는 위치값 리턴한다.  */
int KXMLReader::getPopupIndexFromTargetID(string targetID){
	for (int i = 0; i < mPopups.size(); i++) {
		if (mPopups.at(i)->id == targetID) return  i;
	}
	return -1;
}

/* XML에 정의된 팝업 수를 리턴한다.*/
int KXMLReader::getPopupCount(){
	return mPopups.size();
}

/* 벡터에 저장된 팝업중 @nIndex 해당되는 팝업정보 리턴한다. */
HPOPUP_INFO * KXMLReader::getPopupReference(int nIndex){
	if (nIndex >= getPopupCount()) return nullptr;
	return mPopups.at(nIndex);
}

/* 책에대한 프로젝트 정보를 리턴한다. */
STBOOK_INFO * KXMLReader::getBookInfo(){
	return &mBookInfo;
}

/* 책정보를 mBookInfo에 저장하게 한다.*/
bool KXMLReader::parseFileForBookInfo(){
	XMLError error = mDoc.Parse((const char *)mXmlData.c_str(), mXmlData.size());
	if (error != tinyxml2::XML_SUCCESS) {
		return false;
	}

	XMLElement * pRoot = mDoc.FirstChildElement();
	XMLElement * pElem = pRoot->FirstChildElement();
	while (pElem != NULL) {
		std::string sName = pElem->Name();
		if (sName == BOOK_INFO) {
			parseBookInfo(pElem);
		}
		pElem = pElem->NextSiblingElement();
	}
	return true;
}

/* XML에 있는 팝업 정보를 읽어 저장해 둔다. */
bool KXMLReader::parseFileOtherThanBookInfo(){
	XMLError error = mDoc.Parse((const char *)mXmlData.c_str(), mXmlData.size());
	if (error != tinyxml2::XML_SUCCESS) {
		return false;
	}
	//XMLError error = mDoc.LoadFile(filename);
	if (error != tinyxml2::XML_SUCCESS) {
		return false;
	}
	XMLElement * pRoot = mDoc.FirstChildElement();
	XMLElement * pElem = pRoot->FirstChildElement();
	while (pElem != NULL) {
		std::string sName = pElem->Name();
		if (sName == POPUPS) {
			parsePopups(pElem);
		}
		pElem = pElem->NextSiblingElement();
	}
	pElem = pRoot->FirstChildElement();
	while (pElem != NULL) {
		std::string sName = pElem->Name();
		if (sName == PAGES) {
			parsePages(pElem);
		}
		pElem = pElem->NextSiblingElement();
	}
	return true;
}

/* 책정보를 mBookInfo에 저장하게 한다.parseFileForBookInfo() 에서 호출 */
void KXMLReader::parseBookInfo(XMLElement * pElement) {

	XMLElement * pElem = pElement->FirstChildElement();
	while (pElem != NULL) {
		string sName = pElem->Name() == NULL ? "" : pElem->Name();
		string sValue = pElem->GetText() == NULL ? "" : pElem->GetText();
		if (sName == "version")					mBookInfo.version = atoi(sValue.c_str());
		else if (sName == "projectcode")		mBookInfo.projectcode = sValue;
		else if (sName == "title")				mBookInfo.title = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "width")				mBookInfo.width = atoi(sValue.c_str());
		else if (sName == "height")				mBookInfo.height = atoi(sValue.c_str());
		else if (sName == "backgroundImage")	mBookInfo.backgroundImage = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "backgroundSound")	mBookInfo.backgroundSound = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "transfereffect")		mBookInfo.transfereffect = sValue;
		else if (sName == "makeuser")			mBookInfo.makeuser = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "makedate")			mBookInfo.makedate = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "modifydate")			mBookInfo.modifydate = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "screenType")         mBookInfo.screenType = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "backgroundColor")    mBookInfo.backgroundColor = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "common") {
			XMLElement * pInnerElem = pElem->FirstChildElement();
			while (pInnerElem != NULL) {
				string sID = "";
				string sValue = "";
				for (const XMLAttribute* pattr = pInnerElem->FirstAttribute(); pattr != NULL; pattr = pattr->Next()) {
					string sInnerName = pattr->Name() == NULL ? "" : pattr->Name();
					string sInnerValue = pattr->Value() == NULL ? "" : pattr->Value();
					if (sInnerName == "id")		sID = sInnerValue;
					if (sInnerName == "file")	sValue = mpFileUtils->getSuitableFOpen(sInnerValue);
				}
				if (!sID.empty() && !sValue.empty()) {
					mBookInfo.mapCommon.insert(pair<string, string>(sID, sValue));
				}
				pInnerElem = pInnerElem->NextSiblingElement();
			}
		}
		pElem = pElem->NextSiblingElement();
	}
}

/*팝업 정보를 저장한다. */
void KXMLReader::parsePopups(XMLElement * pElement) {
	XMLElement * pElem = pElement->FirstChildElement();
	while (pElem != NULL) {
		HPOPUP_INFO * pPopupInfo = new HPOPUP_INFO();
		mPopups.pushBack(pPopupInfo);
		setPropertyTo_HPOPUP_INFO(pElem , pPopupInfo );
		parsePopupsInside(pElem, pPopupInfo );
		pElem = pElem->NextSiblingElement();
	}
}

/*해당 팝업의 하위 속성들을 저장해 둔다. */
void KXMLReader::setPropertyTo_HPOPUP_INFO(XMLElement * pElem, HPOPUP_INFO *pPopupInfo){
	for (const XMLAttribute* pattr = pElem->FirstAttribute(); pattr != NULL; pattr = pattr->Next()) {
		string sName = pattr->Name() == NULL ? "" : pattr->Name();
		string sValue = pattr->Value() == NULL ? "" : pattr->Value();
		if (sName == "id")						pPopupInfo->id = sValue;
		else if (sName == "width")				pPopupInfo->stBackground.width = atof(sValue.c_str());
		else if (sName == "height")				pPopupInfo->stBackground.height = atof(sValue.c_str());
		else if (sName == "opacity")			pPopupInfo->stBackground.opacity = atof(sValue.c_str());
		else if (sName == "backgroundColor")	pPopupInfo->stBackground.color = sValue;
		else if (sName == "image")				pPopupInfo->stBackground.image = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "albumthumby")		pPopupInfo->albumthumby = sValue;
		else if (sName == "albumthumbsrc")		pPopupInfo->albumthumbsrc = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "albumthumbsrc2")		pPopupInfo->albumthumbsrc2 = mpFileUtils->getSuitableFOpen(sValue);
	}
}

/* 해당 팝업의 하위 속성의 구조를 관리 한다. */
void KXMLReader::parsePopupsInside(XMLElement * pElement, HPOPUP_INFO * pPopupInfo ) {
	XMLElement * pElem = pElement->FirstChildElement();
	int nIndex = 0;
	while (pElem != NULL) {				
		STCONTENT_INFO * pContentInfo = new STCONTENT_INFO();
		pPopupInfo->vtContents.pushBack(pContentInfo);
		setPropertyTo_HPOPUP_INFO_Contents(pElem, pContentInfo);
		pElem = pElem->NextSiblingElement();
	}
}

/* 팝업 컨텐츠 저장 공통모듈  */
void KXMLReader::setPropertyTo_HPOPUP_INFO_Contents(XMLElement * pElem, STCONTENT_INFO *pContentInfo){
	setPropertyTo_Contents(pElem, pContentInfo);
}

/* XML 페이지 정보를 파싱한다. */
void KXMLReader::parsePages(XMLElement * pElement) {
	XMLElement * pElem = pElement->FirstChildElement();	
	while (pElem != NULL) {
		HPAGE_INFO * pPageInfo = new HPAGE_INFO();
		mPages.pushBack(pPageInfo);
		setPropertyTo_HPAGE_INFO(pElem, pPageInfo);
		
		parsePagesInside(pElem, pPageInfo);
		pElem = pElem->NextSiblingElement();
	}
}

/* 페이지 속성 정보를 저장한다. */
void KXMLReader::setPropertyTo_HPAGE_INFO(XMLElement * pElem, HPAGE_INFO * pPageInfo){
	
	for (const XMLAttribute* pattr = pElem->FirstAttribute(); pattr != NULL; pattr = pattr->Next()) {
		string sName = pattr->Name() == NULL ? "" : pattr->Name();
		string sValue = pattr->Value() == NULL ? "" : pattr->Value();
		string sValueForPlatform = mpFileUtils->getSuitableFOpen(sValue);
		
		if (sName == "opacity")						pPageInfo->stBackground.opacity = atof(sValue.c_str());
		else if (sName == "backgroundColor")		pPageInfo->stBackground.color = sValue;
		else if (sName == "backgroundImage")		pPageInfo->stBackground.image = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "width")					pPageInfo->stBackground.width = atof(sValue.c_str());
		else if (sName == "height")					pPageInfo->stBackground.height = atof(sValue.c_str());
		else if (sName == "backgroundSound")		pPageInfo->backgroundMusic = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "startaction")			pPageInfo->loadAction.name = sValue;
		else if (sName == "startactionparam")		pPageInfo->loadAction.param = sValue;
		else if (sName == "swiperight")				pPageInfo->swipeRight.name = sValue;
		else if (sName == "swiperightparam")		pPageInfo->swipeRight.param = sValue;
		else if (sName == "type")					pPageInfo->type = sValue;
		else if (sName == "pageno")					pPageInfo->pageno = sValue;
		else if (sName == "thumbnailImage")			pPageInfo->thumbnailImage = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "narration")				pPageInfo->narration = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "ptype")					pPageInfo->ptype = sValue;


	}
}

/* 페이지들의 하위 contents를 파싱한다. */
void KXMLReader::parsePagesInside(XMLElement * pElement, HPAGE_INFO * pPageInfo) {
	XMLElement * pElem = pElement->FirstChildElement();
	while (pElem != NULL) {
		string sName = pElem->Name();
		if (sName == "contents") {
			parsePagesInsideContents(pElem, pPageInfo);
		}
		pElem = pElem->NextSiblingElement();
	}
}

/* 단일 페이지 하위 contents를 파싱한다. */
void KXMLReader::parsePagesInsideContents(XMLElement * pElement, HPAGE_INFO * pPageInfo){
	XMLElement *pElem = pElement->FirstChildElement();
	int nIndex = 0;
	while (pElem != NULL) {
		string sName = pElem->Name();
		if (sName == "content") {
			STCONTENT_INFO * pContentInfo = new STCONTENT_INFO();
			pPageInfo->vtContents.pushBack(pContentInfo);
			setPropertyTo_HPAGE_INFO_Contents(pElem, pContentInfo);
		}
		pElem = pElem->NextSiblingElement();
	}
}

/* 페이지 컨텐츠 관리 */
void KXMLReader::setPropertyTo_HPAGE_INFO_Contents(XMLElement * pElement, STCONTENT_INFO *pContentInfo){
	setPropertyTo_Contents(pElement, pContentInfo);
}

/* 애니메이션 컨텐츠는 하위 프레임애니매이션 리스트를 별도로 저정한다. */
void KXMLReader::setPropertyTo_Contents_Animation(XMLElement * pElement, STCONTENT_INFO *pContentInfo){
	STANIMATION * pImageArray = new STANIMATION();
	for (const XMLAttribute* pattr = pElement->FirstAttribute(); pattr != NULL; pattr = pattr->Next()) {
		string sName = pattr->Name() == NULL ? "" : pattr->Name();
		string sValue = pattr->Value() == NULL ? "" : pattr->Value();
		if (sName == "width")		pImageArray->width = atoi(sValue.c_str());
		if (sName == "height")		pImageArray->height = atoi(sValue.c_str());
		if (sName == "src")			pImageArray->image = mpFileUtils->getSuitableFOpen(sValue);
	}
	pContentInfo->vtAnimation.pushBack(pImageArray);
}

/* 하나의 페이지 contents의 속성을 저장한다. */
void KXMLReader::setPropertyTo_Contents(XMLElement * pElem, STCONTENT_INFO *pContentInfo){
	for (const XMLAttribute* pattr = pElem->FirstAttribute(); pattr != NULL; pattr = pattr->Next()) {
		string sName = pattr->Name() == NULL ? "" : pattr->Name();
		string sValue = pattr->Value() == NULL ? "" : pattr->Value();
		if (sName == "type")				pContentInfo->type = sValue;
		else if (sName == "src")			pContentInfo->image = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "src2")			pContentInfo->selected = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "disabled")		pContentInfo->disabled = sValue;
		else if (sName == "x")				pContentInfo->x = atof(sValue.c_str());
		else if (sName == "y")				pContentInfo->y = atof(sValue.c_str());
		else if (sName == "width")			pContentInfo->width = atof(sValue.c_str());
		else if (sName == "height")			pContentInfo->height = atof(sValue.c_str());
		else if (sName == "action")			pContentInfo->action.name = sValue;
		else if (sName == "targetid")		pContentInfo->action.param = sValue;
		else if (sName == "showid")			pContentInfo->action.param2 = sValue;
		else if (sName == "id")				pContentInfo->id = sValue;
		else if (sName == "text")			pContentInfo->text = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "fontsize")		pContentInfo->fontsize = sValue;
		else if (sName == "fontcolor")		pContentInfo->fontcolor = sValue;
		else if (sName == "ptype")			pContentInfo->ptype = sValue;
		else if (sName == "delay")			pContentInfo->delay = sValue;
		else if (sName == "dx")				pContentInfo->dx = sValue;
		else if (sName == "dy")				pContentInfo->dy = sValue;
		else if (sName == "time")			pContentInfo->time = sValue;
		else if (sName == "visible")		pContentInfo->visible = sValue;
		else if (sName == "sound")		    pContentInfo->sound = sValue;
		else if (sName == "group")		    pContentInfo->group = sValue;
		else if (sName == "sort")		    pContentInfo->sort = sValue;
		else if (sName == "depth")		    pContentInfo->depth = sValue;
		else if (sName == "aniloop")		pContentInfo->aniloop = sValue;
		else if (sName == "soundloop")		pContentInfo->soundloop = sValue;
		else if (sName == "normaltype")		pContentInfo->normaltype = sValue;
		else if (sName == "rotate")			pContentInfo->rotate = sValue;
		else if (sName == "rotateloop")		pContentInfo->rotateloop = sValue;
		else if (sName == "rotatedu")		pContentInfo->rotatedu = sValue;
		else if (sName == "rotatean")		pContentInfo->rotatean = sValue;
		else if (sName == "scale")			pContentInfo->scale = sValue;
		else if (sName == "scaleloop")		pContentInfo->scaleloop = sValue;
		else if (sName == "scaledu")		pContentInfo->scaledu = sValue;
		else if (sName == "scaless")		pContentInfo->scaless = sValue;
		else if (sName == "fontfile")		pContentInfo->fontfile = sValue;
		else if (sName == "fontname")		pContentInfo->fontname = mpFileUtils->getSuitableFOpen(sValue);
		else if (sName == "outline")		pContentInfo->outline = sValue;
		else if (sName == "outlinecolor")	pContentInfo->outlinecolor = sValue;
		else if (sName == "outlinetick")	pContentInfo->outlinetick = sValue;
		else if (sName == "goto")			pContentInfo->gotopage = sValue;
		else if (sName == "playingcolor")	pContentInfo->playingcolor = sValue;
		else if (sName == "startani")   	pContentInfo->startani = sValue;
		else if (sName == "alpha")   	    pContentInfo->alpha = sValue;
	}

	if (pContentInfo->action.name == "GOTO")
	{
		if (pContentInfo->gotopage != "")
			pContentInfo->action.param = pContentInfo->gotopage;
	}
	if (pContentInfo->type == "animation" ) {
		XMLElement * pTempElement = pElem->FirstChildElement();
		while (pTempElement != NULL) {
			setPropertyTo_Contents_Animation(pTempElement, pContentInfo);
			pTempElement = pTempElement->NextSiblingElement();
		}
	}
}
