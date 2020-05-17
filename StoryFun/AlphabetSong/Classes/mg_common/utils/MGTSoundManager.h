#pragma once

#include "SingletonBase.h"
#include "AudioEngine.h"
#include "cocos2d.h"
#include <map>
#include <vector>
#include <string>
#include <functional>
#include <mutex>
#include "MGTDefines.h"
#include "MGTUtils.h"

class MGTSoundManager : public CSingletonBase<MGTSoundManager>
{
public:
	MGTSoundManager();
	~MGTSoundManager();
	bool init();
		
	// BGM
	int playBgm(const std::string & filePath, bool loop = true, bool stopPrevBgm = true, float volumeRatio = 1.0f, const std::function<void(int, const std::string&)> & callbackWhenFinished = nullptr);
	void stopBgm(int audioId);
	void stopBgm(const std::string & filePath);
	void stopAllBgms(const std::string & exceptFilePath = "");
	void pauseBgm(int audioId);
	void pauseBgm(const std::string & filePath);
	void pauseAllBgm();
	void resumeBgm(int audioId);
	void resumeBgm(const std::string & filePath);
	void resumeAllBgm();
	bool isBgmPlaying(int audioId);
	bool isBgmPlaying(const std::string & filePath);
	bool isAnyBgmPlaying();

	// Sound Effect
	int playEffect(const std::string & filePath, bool loop = false, float volumeRatio = 1.0f, const std::function<void(int, const std::string&)> & callbackWhenFinished = nullptr);
	void stopEffect(int audioId);
	void stopEffect(const std::string & filePath);
	void stopAllEffects();
	void pauseEffect(int audioId);
	void pauseEffect(const std::string & filePath);
	void pauseAllEffects();
	void resumeEffect(int audioId);
	void resumeEffect(const std::string & filePath);
	void resumeAllEffects();
    
    
    // Sound Narration
    int playNarration(const std::string & filePath, bool loop = false, float volumeRatio = 1.0f, const std::function<void(int, const std::string&)> & callbackWhenFinished = nullptr);
    void stopNarration(int audioId);
    void stopNarration(const std::string & filePath);
    void stopAllNarrations();
    void pauseNarration(int audioId);
    void pauseNarration(const std::string & filePath);
    void pauseAllNarrations();
    void resumeNarration(int audioId);
    void resumeNarration(const std::string & filePath);
    void resumeAllNarrations();
    bool isNarrationPlaying(int audioId);
    bool isNarrationPlaying(const std::string & filePath);
    bool isAnyNarrationPlaying();
    
    void stopAllSounds();
    
    
	// configration
	void setBgmOn(bool on);
	void setEffectOn(bool on);
    void setNarrationOn(bool on);
	void setBgmVolume(float volume);	
	void setEffectVolume(float volume);
    void setNarrationVolume(float volume);
    
    float getBgmVolume() { return _bgmVolume; };
    float getEffectVolume() { return _effectVolume; };
    float getNarrationVolume() { return _narrationVolume; };
    

	// preload functions
	void preloadAudioFile(const std::string & filePath);
	void preloadAudioFiles(const std::vector<std::string> & filePathVec);
	void uncacheAudioFile(const std::string & filePath);
	void uncacheAllAudioFiles();

private:
	// 사운드 파일 재생이 끝날 때 호출되는 콜백 함수
	void bgmPlayFinishedCallback(int audioId, const std::string & filePath);
	void effectPlayFinishedCallback(int audioId, const std::string & filePath);
    void narrationPlayFinishedCallback(int audioId, const std::string & filePath);
	
public:
	static const int	INVALID_AUDIO_ID;
	static const float	TIME_UNKNOWN;

private:
	bool		_isBgmOn;
	bool		_isEffectOn;
    bool		_isNarrationOn;
	float		_bgmVolume;
	float		_effectVolume;
    float		_narrationVolume;
	std::set<std::string> _preloadedAudioFileSet;
	
	// 속도 문제로 path 문자열 그대로 안쓰고 HASH값을 사용 (HASH 함수는 CocosDenshion::SimpleAudioEngine에 있는 함수를 카피해서 사용함)
	std::mutex							_mutexForBgm;
	std::map<int, unsigned int>			_bgmPathHashMap;	// pair(audio ID, path HASH)
	std::multimap<unsigned int, int>	_bgmAudioIdMMap;	// pair(path HASH, audio ID)
	std::map<int, std::function<void(int, const std::string &)>>	_bgmFinishCallbackMap;		// pair(audio ID, callback function when the play finished)

	std::mutex							_mutexForEffect;
	std::map<int, unsigned int>			_effectPathHashMap;	// pair(audio ID, path HASH)
	std::multimap<unsigned int, int>	_effectAudioIdMMap;	// pair(path HASH, audio ID)
	std::map<int, std::function<void(int, const std::string &)>>	_effectFinishCallbackMap;	// pair(audio ID, callback function when the play finished)
    
    
    std::mutex							_mutexForNarration;
    std::map<int, unsigned int>			_narrationPathHashMap;	// pair(audio ID, path HASH)
    std::multimap<unsigned int, int>	_narrationAudioIdMMap;	// pair(path HASH, audio ID)
    std::map<int, std::function<void(int, const std::string &)>>	_narrationFinishCallbackMap;	// pair(audio ID, callback function when the play finished)
};
