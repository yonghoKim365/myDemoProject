#include "KGlobalManager.h"

/*Global ���� ����*/

KGlobalManager * KGlobalManager::_instance = nullptr;
KGlobalManager::KGlobalManager() {
    /*���(true) �б�(false) ��� ����*/
	bAutoPlay = false;
    shouldNarrationType = false;
    mAudioEngineID = -11111;
}

KGlobalManager::~KGlobalManager() {

}

KGlobalManager* KGlobalManager::getInstance() {
    if (_instance == nullptr) {
	   _instance = new KGlobalManager();
    }
    return _instance;
}

void KGlobalManager::removeInstance() {
    if (_instance == nullptr) return;
    delete _instance;
    _instance = nullptr;
}

/*���(true) �б�(false) ��� ����*/
void KGlobalManager::setAutoPlay(bool aGoNext) {
	bAutoPlay = aGoNext;
}
/*���(true) �б�(false) ��� ���� return*/
bool KGlobalManager::getAutoPlay() {
	return bAutoPlay;
}

/* Narration System : false / User : true */
void KGlobalManager::setNarration(bool aGoNext) {
    shouldNarrationType = aGoNext;
}

/* Narration System : false / User : true return*/
bool KGlobalManager::getNarration() {
    return shouldNarrationType;
}

void KGlobalManager::setCurrentAudio(int nAudioEngineID) {
    mAudioEngineID = nAudioEngineID;
}

int KGlobalManager::getCurrentAudio() {
    return mAudioEngineID;
}