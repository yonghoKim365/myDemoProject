#include "MGTSoundManager.h"

#ifndef AUTO_LOCK
#define AUTO_LOCK(mtx)	std::lock_guard<std::mutex> lock(mtx)
#endif

using namespace cocos2d;
using namespace experimental;

const int MGTSoundManager::INVALID_AUDIO_ID = AudioEngine::INVALID_AUDIO_ID;
const float	MGTSoundManager::TIME_UNKNOWN = AudioEngine::TIME_UNKNOWN;

static unsigned int _hash(const char *key)
{
	unsigned int len = strlen(key);
	const char *end = key + len;
	unsigned int hash;

	for (hash = 0; key < end; key++)
	{
		hash *= 16777619;
		hash ^= (unsigned int)(unsigned char)toupper(*key);
	}
	return (hash);
}

MGTSoundManager::MGTSoundManager()
: _isBgmOn(true)
, _isEffectOn(true)
, _bgmVolume(1.0f)
, _effectVolume(1.0f)
{
	AudioEngine::lazyInit();
    init();
}

MGTSoundManager::~MGTSoundManager()
{
	uncacheAllAudioFiles();
}

bool MGTSoundManager::init()
{
	/* USER_IMPLEMENTATION_BEGIN */
	// 공유 메모리로 부터 배경음, 효과음의 ON/OFF 여부와 각각의 볼륨값을 셋팅 해준다. (참고로 볼륨 최대값은 1.0f)
	// init 함수를 부르지 않을 것이라면 아래 네개의 함수를 직접 불러줘도 된다.
	bool isBgmOn = true;
	bool isEffectOn = true;
	float bgmVolume = 1.0f;
	float effectVolume = 1.0f;
    float narrationVolume = 1.0f;
	/* USER_IMPLEMENTATION_END */
	
	setBgmOn(isBgmOn);
	setEffectOn(isEffectOn);
    setNarrationOn(isEffectOn);
    
	setBgmVolume(bgmVolume);
	setEffectVolume(effectVolume);
    setNarrationVolume(narrationVolume);
	
	return true;
}

#pragma mark - BGM

int MGTSoundManager::playBgm(const std::string & filePath, bool loop, bool stopPrevBgm, float volumeRatio, const std::function<void(int, const std::string&)> & callbackWhenFinished)
{
	if (!_isBgmOn)
		return INVALID_AUDIO_ID;

    std::string name = MGTUtils::getInstance()->stringTokenizer(filePath, "/", false);
    
	auto hash = _hash(filePath.c_str());
	
	// 같은 BGM 파일이 시간차를 두고 재생 되는 것을 방지한다.
	{
		_mutexForBgm.lock();
		auto it = _bgmAudioIdMMap.find(hash);
		if (it != _bgmAudioIdMMap.end())
		{
			_mutexForBgm.unlock();
			int audioId = it->second;
			
			if (stopPrevBgm)
				stopAllBgms(filePath);

			return audioId;
		}
		_mutexForBgm.unlock();
	}
	
	// 이미 재생 중인 BGM이 있는 경우 모두 정지시킨다. (단, 파라미터로 넘어온 filePath와 같은 파일이라면 그냥 리턴한다.)
	if (stopPrevBgm)
		stopAllBgms();
	
	if (filePath == "")
		return INVALID_AUDIO_ID;

	int audioId = AudioEngine::play2d(filePath, loop, _bgmVolume * volumeRatio);
	if (audioId != INVALID_AUDIO_ID)
	{
		AudioEngine::setFinishCallback(audioId, CC_CALLBACK_2(MGTSoundManager::bgmPlayFinishedCallback, this));

		AUTO_LOCK(_mutexForBgm);
		_bgmAudioIdMMap.insert(std::pair<unsigned int, int>(hash, audioId));
		_bgmPathHashMap.insert(std::pair<int, unsigned int>(audioId, hash));

		if (callbackWhenFinished)
			_bgmFinishCallbackMap.insert(std::pair<int, std::function<void(int, const std::string &)>>(audioId, callbackWhenFinished));
	}

	return audioId;
}

void MGTSoundManager::stopBgm(int audioId)
{
	AudioEngine::stop(audioId);

	bgmPlayFinishedCallback(audioId, "");
}

void MGTSoundManager::stopBgm(const std::string & filePath)
{
	auto hash = _hash(filePath.c_str());

	std::multimap<unsigned int, int>::iterator it;
	
	// mutex 처리 때문데 multimap의 equal_range를 사용하지 않고 하나씩 처리함
	while (true)
	{
		_mutexForBgm.lock();
		it = _bgmAudioIdMMap.find(hash);

		if (it == _bgmAudioIdMMap.end())
		{
			_mutexForBgm.unlock();
			break;
		}
		_mutexForBgm.unlock();

		stopBgm(it->second);
	}	
}

void MGTSoundManager::stopAllBgms(const std::string & exceptFilePath)
{
	if (exceptFilePath == "")
	{
		std::multimap<unsigned int, int>::iterator it;

		while (true)
		{
			_mutexForBgm.lock();
			it = _bgmAudioIdMMap.begin();

			if (it == _bgmAudioIdMMap.end())
			{
				_mutexForBgm.unlock();
				break;
			}
			_mutexForBgm.unlock();

			stopBgm(it->second);
		}
	}
	else
	{
		std::vector<int> audioIdToStopVec;
		int hash = _hash(exceptFilePath.c_str());

		_mutexForBgm.lock();
		for (auto & it : _bgmAudioIdMMap)
		{			
			if (it.first != hash)
				audioIdToStopVec.push_back(it.second);
		}
		_mutexForBgm.unlock();

		for (auto it : audioIdToStopVec)
			stopBgm(it);
	}
}

void MGTSoundManager::pauseBgm(int audioId)
{
	AudioEngine::pause(audioId);
}

void MGTSoundManager::pauseBgm(const std::string & filePath)
{
	auto hash = _hash(filePath.c_str());

	AUTO_LOCK(_mutexForBgm);

	auto range = _bgmAudioIdMMap.equal_range(hash);
	for (auto it = range.first; it != range.second; ++it)
		pauseBgm(it->second);
}

void MGTSoundManager::pauseAllBgm()
{
	AUTO_LOCK(_mutexForBgm);

	for (auto & it : _bgmAudioIdMMap)
		pauseBgm(it.second);
}

void MGTSoundManager::resumeBgm(int audioId)
{
	AudioEngine::resume(audioId);
}

void MGTSoundManager::resumeBgm(const std::string & filePath)
{
	auto hash = _hash(filePath.c_str());

	AUTO_LOCK(_mutexForBgm);

	auto range = _bgmAudioIdMMap.equal_range(hash);
	for (auto it = range.first; it != range.second; ++it)
		resumeBgm(it->second);
}

void MGTSoundManager::resumeAllBgm()
{
	AUTO_LOCK(_mutexForBgm);

	for (auto & it : _bgmAudioIdMMap)
		resumeBgm(it.second);
}

bool MGTSoundManager::isBgmPlaying(int audioId)
{
	return AudioEngine::getState(audioId) == AudioEngine::AudioState::PLAYING;
}

bool MGTSoundManager::isBgmPlaying(const std::string & filePath)
{
	auto hash = _hash(filePath.c_str());

	AUTO_LOCK(_mutexForBgm);

	auto range = _bgmAudioIdMMap.equal_range(hash);
	for (auto it = range.first; it != range.second; ++it)
	{
		if (AudioEngine::getState(it->second) == AudioEngine::AudioState::PLAYING)
			return true;
	}

	return false;
}

bool MGTSoundManager::isAnyBgmPlaying()
{
	AUTO_LOCK(_mutexForBgm);

	for (auto & it : _bgmAudioIdMMap)
	{
		if (AudioEngine::getState(it.second) == AudioEngine::AudioState::PLAYING)
			return true;
	}

	return false;
}

#pragma mark - Effect

int MGTSoundManager::playEffect(const std::string & filePath, bool loop, float volumeRatio, const std::function<void(int, const std::string&)> & callbackWhenFinished)
{
	if (!_isEffectOn)
		return INVALID_AUDIO_ID;

	if (filePath == "")
		return INVALID_AUDIO_ID;

	int audioId = AudioEngine::play2d(filePath, loop, _effectVolume * volumeRatio);
	if (audioId != INVALID_AUDIO_ID)
	{
		AudioEngine::setFinishCallback(audioId, CC_CALLBACK_2(MGTSoundManager::effectPlayFinishedCallback, this));
		auto hash = _hash(filePath.c_str());

		AUTO_LOCK(_mutexForEffect);
		_effectAudioIdMMap.insert(std::pair<unsigned int, int>(hash, audioId));
		_effectPathHashMap.insert(std::pair<int, unsigned int>(audioId, hash));

		if (callbackWhenFinished)
			_effectFinishCallbackMap.insert(std::pair<int, std::function<void(int, const std::string &)>>(audioId, callbackWhenFinished));
	}

	return audioId;
}

void MGTSoundManager::stopEffect(int audioId)
{
	AudioEngine::stop(audioId);
	
	effectPlayFinishedCallback(audioId, "");
}

void MGTSoundManager::stopEffect(const std::string & filePath)
{
	auto hash = _hash(filePath.c_str());

	std::multimap<unsigned int, int>::iterator it;

	// mutex 처리 때문데 multimap의 equal_range를 사용하지 않고 하나씩 처리함
	while (true)
	{
		_mutexForEffect.lock();
		it = _effectAudioIdMMap.find(hash);

		if (it == _effectAudioIdMMap.end())
		{
			_mutexForEffect.unlock();
			break;
		}
		_mutexForEffect.unlock();

		stopEffect(it->second);
	}
}

void MGTSoundManager::stopAllEffects()
{
	std::multimap<unsigned int, int>::iterator it;

	while (true)
	{
		_mutexForEffect.lock();
		it = _effectAudioIdMMap.begin();

		if (it == _effectAudioIdMMap.end())
		{
			_mutexForEffect.unlock();
			break;
		}
		_mutexForEffect.unlock();

		stopEffect(it->second);
	}
}

void MGTSoundManager::pauseEffect(int audioId)
{
	AudioEngine::pause(audioId);
}

void MGTSoundManager::pauseEffect(const std::string & filePath)
{
	auto hash = _hash(filePath.c_str());

	AUTO_LOCK(_mutexForEffect);

	auto range = _effectAudioIdMMap.equal_range(hash);
	for (auto it = range.first; it != range.second; ++it)
		pauseEffect(it->second);
}

void MGTSoundManager::pauseAllEffects()
{
	AUTO_LOCK(_mutexForEffect);

	for (auto & it : _effectAudioIdMMap)
		pauseEffect(it.second);
}

void MGTSoundManager::resumeEffect(int audioId)
{
	AudioEngine::resume(audioId);
}

void MGTSoundManager::resumeEffect(const std::string & filePath)
{
	auto hash = _hash(filePath.c_str());

	AUTO_LOCK(_mutexForEffect);

	auto range = _effectAudioIdMMap.equal_range(hash);
	for (auto it = range.first; it != range.second; ++it)
		resumeEffect(it->second);
}

void MGTSoundManager::resumeAllEffects()
{
	AUTO_LOCK(_mutexForEffect);

	for (auto & it : _effectAudioIdMMap)
		resumeEffect(it.second);
}


#pragma mark - Narration

int MGTSoundManager::playNarration(const std::string & filePath, bool loop, float volumeRatio, const std::function<void(int, const std::string&)> & callbackWhenFinished)
{
    if (!_isNarrationOn)
        return INVALID_AUDIO_ID;
    
    if (filePath == "")
        return INVALID_AUDIO_ID;
    
    int audioId = AudioEngine::play2d(filePath, loop, _narrationVolume * volumeRatio);
    if (audioId != INVALID_AUDIO_ID)
    {
        AudioEngine::setFinishCallback(audioId, CC_CALLBACK_2(MGTSoundManager::narrationPlayFinishedCallback, this));
        auto hash = _hash(filePath.c_str());
        
        AUTO_LOCK(_mutexForNarration);
        _narrationAudioIdMMap.insert(std::pair<unsigned int, int>(hash, audioId));
        _narrationPathHashMap.insert(std::pair<int, unsigned int>(audioId, hash));
        
        if (callbackWhenFinished)
            _narrationFinishCallbackMap.insert(std::pair<int, std::function<void(int, const std::string &)>>(audioId, callbackWhenFinished));
    }
    
    return audioId;
}

void MGTSoundManager::stopNarration(int audioId)
{
    AudioEngine::stop(audioId);
    
    narrationPlayFinishedCallback(audioId, "");
}

void MGTSoundManager::stopNarration(const std::string & filePath)
{
    auto hash = _hash(filePath.c_str());
    
    std::multimap<unsigned int, int>::iterator it;
    
    // mutex 처리 때문데 multimap의 equal_range를 사용하지 않고 하나씩 처리함
    while (true)
    {
        _mutexForNarration.lock();
        it = _narrationAudioIdMMap.find(hash);
        
        if (it == _narrationAudioIdMMap.end())
        {
            _mutexForNarration.unlock();
            break;
        }
        _mutexForNarration.unlock();
        
        stopNarration(it->second);
    }
}

void MGTSoundManager::stopAllNarrations()
{
    std::multimap<unsigned int, int>::iterator it;
    
    while (true)
    {
        _mutexForNarration.lock();
        it = _narrationAudioIdMMap.begin();
        
        if (it == _narrationAudioIdMMap.end())
        {
            _mutexForNarration.unlock();
            break;
        }
        _mutexForNarration.unlock();
        
        stopNarration(it->second);
    }
}

void MGTSoundManager::pauseNarration(int audioId)
{
    AudioEngine::pause(audioId);
}

void MGTSoundManager::pauseNarration(const std::string & filePath)
{
    auto hash = _hash(filePath.c_str());
    
    AUTO_LOCK(_mutexForNarration);
    
    auto range = _narrationAudioIdMMap.equal_range(hash);
    for (auto it = range.first; it != range.second; ++it)
        pauseNarration(it->second);
}

void MGTSoundManager::pauseAllNarrations()
{
    AUTO_LOCK(_mutexForNarration);
    
    for (auto & it : _narrationAudioIdMMap)
        pauseNarration(it.second);
}

void MGTSoundManager::resumeNarration(int audioId)
{
    AudioEngine::resume(audioId);
}

void MGTSoundManager::resumeNarration(const std::string & filePath)
{
    auto hash = _hash(filePath.c_str());
    
    AUTO_LOCK(_mutexForNarration);
    
    auto range = _narrationAudioIdMMap.equal_range(hash);
    for (auto it = range.first; it != range.second; ++it)
        resumeNarration(it->second);
}

void MGTSoundManager::resumeAllNarrations()
{
    AUTO_LOCK(_mutexForNarration);
    
    for (auto & it : _narrationAudioIdMMap)
        resumeNarration(it.second);
}

bool MGTSoundManager::isNarrationPlaying(int audioId)
{
    return AudioEngine::getState(audioId) == AudioEngine::AudioState::PLAYING;
}

bool MGTSoundManager::isNarrationPlaying(const std::string & filePath)
{
    auto hash = _hash(filePath.c_str());
    
    AUTO_LOCK(_mutexForNarration);
    
    auto range = _narrationAudioIdMMap.equal_range(hash);
    for (auto it = range.first; it != range.second; ++it)
    {
        if (AudioEngine::getState(it->second) == AudioEngine::AudioState::PLAYING)
            return true;
    }
    
    return false;
}

bool MGTSoundManager::isAnyNarrationPlaying()
{
    AUTO_LOCK(_mutexForNarration);
    
    for (auto & it : _narrationAudioIdMMap)
    {
        if (AudioEngine::getState(it.second) == AudioEngine::AudioState::PLAYING)
            return true;
    }
    
    return false;
}



void MGTSoundManager::stopAllSounds()
{
    stopAllBgms();
    stopAllEffects();
    stopAllNarrations();
}

#pragma mark - Cofiguration

void MGTSoundManager::setBgmOn(bool on)
{
    /* USER_IMPLEMENTATION_BEGIN */
    // 이 함수를 호출하기 전 또는 후에 DB 파일에 설정 정보를 쓰도록 했다면 여기는 그냥 넘어가세요.
    // 자신의 DB 파일에 파라미터로 넘어온 on 값에 따라 설정값 저장.
    // 따로 저장해두지 않으면 앱을 재실행 할 때마다 BGM이 켜져 있게 됩니다.
    /* USER_IMPLEMENTATION_END */
    
    _isBgmOn = on;
    if (!_isBgmOn)
        stopAllBgms();
}

void MGTSoundManager::setEffectOn(bool on)
{
    /* USER_IMPLEMENTATION_BEGIN */
    // 이 함수를 호출하기 전 또는 후에 DB 파일에 설정 정보를 쓰도록 했다면 여기는 그냥 넘어가세요.
    // 자신의 DB 파일에 파라미터로 넘어온 on 값에 따라 설정값 저장.
    // 따로 저장해두지 않으면 앱을 재실행 할 때마다 효과음이 켜져 있게 됩니다.
    /* USER_IMPLEMENTATION_END */
    
    _isEffectOn = on;
    if (!_isEffectOn)
        stopAllEffects();
}

void MGTSoundManager::setNarrationOn(bool on)
{
    /* USER_IMPLEMENTATION_BEGIN */
    // 이 함수를 호출하기 전 또는 후에 DB 파일에 설정 정보를 쓰도록 했다면 여기는 그냥 넘어가세요.
    // 자신의 DB 파일에 파라미터로 넘어온 on 값에 따라 설정값 저장.
    // 따로 저장해두지 않으면 앱을 재실행 할 때마다 효과음이 켜져 있게 됩니다.
    /* USER_IMPLEMENTATION_END */
    
    _isNarrationOn = on;
    if (!_isNarrationOn)
        stopAllNarrations();
}

void MGTSoundManager::setBgmVolume(float volume)
{
    if (volume < 0.0f)
        volume = 0.0f;
    else if (volume > 1.0f)
        volume = 1.0f;
    
    /* USER_IMPLEMENTATION_BEGIN */
    // 이 함수를 호출하기 전 또는 후에 DB 파일에 설정 정보를 쓰도록 했다면 여기는 그냥 넘어가세요.
    // 자신의 DB 파일에 파라미터로 넘어온 volume 값에 따라 설정값 저장.
    // 따로 저장해두지 않으면 앱을 재실행 할 때마다 BGM의 볼륨이 최대로 설정됩니다.
    /* USER_IMPLEMENTATION_END */
    
    _bgmVolume = volume;
    for (auto & it : _bgmAudioIdMMap)
        AudioEngine::setVolume(it.second, _bgmVolume);
}

void MGTSoundManager::setEffectVolume(float volume)
{
    if (volume < 0.0f)
        volume = 0.0f;
    else if (volume > 1.0f)
        volume = 1.0f;
    
    /* USER_IMPLEMENTATION_BEGIN */
    // 이 함수를 호출하기 전 또는 후에 DB 파일에 설정 정보를 쓰도록 했다면 여기는 그냥 넘어가세요.
    // 자신의 DB 파일에 파라미터로 넘어온 volume 값에 따라 설정값 저장.
    // 따로 저장해두지 않으면 앱을 재실행 할 때마다 BGM의 볼륨이 최대로 설정됩니다.
    /* USER_IMPLEMENTATION_END */
    
    _effectVolume = volume;
    for (auto & it : _effectAudioIdMMap)
        AudioEngine::setVolume(it.second, _effectVolume);
}

void MGTSoundManager::setNarrationVolume(float volume)
{
    if (volume < 0.0f)
        volume = 0.0f;
    else if (volume > 1.0f)
        volume = 1.0f;
    
    /* USER_IMPLEMENTATION_BEGIN */
    // 이 함수를 호출하기 전 또는 후에 DB 파일에 설정 정보를 쓰도록 했다면 여기는 그냥 넘어가세요.
    // 자신의 DB 파일에 파라미터로 넘어온 volume 값에 따라 설정값 저장.
    // 따로 저장해두지 않으면 앱을 재실행 할 때마다 BGM의 볼륨이 최대로 설정됩니다.
    /* USER_IMPLEMENTATION_END */
    
    _narrationVolume = volume;
    for (auto & it : _narrationAudioIdMMap)
        AudioEngine::setVolume(it.second, _narrationVolume);
}


#pragma mark - Preload Function

void MGTSoundManager::preloadAudioFile(const std::string & filePath)
{
    auto it = _preloadedAudioFileSet.find(filePath);
    if (it != _preloadedAudioFileSet.end())
        return;
    
    _preloadedAudioFileSet.insert(filePath);
    
    AudioEngine::preload(filePath);
}

void MGTSoundManager::preloadAudioFiles(const std::vector<std::string> & filePathVec)
{
    for (auto it : filePathVec)
        preloadAudioFile(it);
}

void MGTSoundManager::uncacheAudioFile(const std::string & filePath)
{
    if (filePath == "")
        return;
    
    stopBgm(filePath);
    stopEffect(filePath);
    stopNarration(filePath);
    AudioEngine::uncache(filePath);
    auto it = _preloadedAudioFileSet.find(filePath);
    if (it != _preloadedAudioFileSet.end())
        _preloadedAudioFileSet.erase(it);
}

void MGTSoundManager::uncacheAllAudioFiles()
{
    stopAllBgms();
    stopAllEffects();
    stopAllNarrations();
    AudioEngine::uncacheAll();
    _preloadedAudioFileSet.clear();
}


#pragma mark - Sound finished callback

void MGTSoundManager::bgmPlayFinishedCallback(int audioId, const std::string & filePath)
{
    AUTO_LOCK(_mutexForBgm);
    
    auto callbackIt = _bgmFinishCallbackMap.find(audioId);
    if (callbackIt != _bgmFinishCallbackMap.end())
    {
        if (filePath != "")
            callbackIt->second(audioId, filePath);
        _bgmFinishCallbackMap.erase(callbackIt);
    }
    
    auto hashIt = _bgmPathHashMap.find(audioId);
    if (hashIt != _bgmPathHashMap.end())
    {
        auto range = _bgmAudioIdMMap.equal_range(hashIt->second);
        for (auto it = range.first; it != range.second; ++it)
        {
            if (it->second == audioId)
            {
                _bgmAudioIdMMap.erase(it);
                break;
            }
        }
        
        _bgmPathHashMap.erase(hashIt);
    }
    
    cocos2d::Value file(filePath);
    __NotificationCenter::getInstance()->postNotification(MGTNOTIFICATION_BGM_FINISHCALL, (Ref*)&file);
}

void MGTSoundManager::effectPlayFinishedCallback(int audioId, const std::string & filePath)
{
    AUTO_LOCK(_mutexForEffect);
    
    auto callbackIt = _effectFinishCallbackMap.find(audioId);
    if (callbackIt != _effectFinishCallbackMap.end())
    {
        if (filePath != "")
            callbackIt->second(audioId, filePath);
        _effectFinishCallbackMap.erase(callbackIt);
    }
    
    auto hashIt = _effectPathHashMap.find(audioId);
    if (hashIt != _effectPathHashMap.end())
    {
        auto range = _effectAudioIdMMap.equal_range(hashIt->second);
        for (auto it = range.first; it != range.second; ++it)
        {
            if (it->second == audioId)
            {
                _effectAudioIdMMap.erase(it);
                break;
            }
        }
        
        _effectPathHashMap.erase(hashIt);
    }
    
    cocos2d::Value file(filePath);
    __NotificationCenter::getInstance()->postNotification(MGTNOTIFICATION_EFFECT_FINISHCALL, (Ref*)&file);
}

void MGTSoundManager::narrationPlayFinishedCallback(int audioId, const std::string & filePath)
{
    AUTO_LOCK(_mutexForNarration);
    
    auto callbackIt = _narrationFinishCallbackMap.find(audioId);
    if (callbackIt != _narrationFinishCallbackMap.end())
    {
        if (filePath != "")
            callbackIt->second(audioId, filePath);
        _narrationFinishCallbackMap.erase(callbackIt);
    }
    
    auto hashIt = _narrationPathHashMap.find(audioId);
    if (hashIt != _narrationPathHashMap.end())
    {
        auto range = _narrationAudioIdMMap.equal_range(hashIt->second);
        for (auto it = range.first; it != range.second; ++it)
        {
            if (it->second == audioId)
            {
                _narrationAudioIdMMap.erase(it);
                break;
            }
        }
        
        _narrationPathHashMap.erase(hashIt);
    }
    
    cocos2d::Value file(filePath);
    __NotificationCenter::getInstance()->postNotification(MGTNOTIFICATION_NAR_FINISHCALL, (Ref*)&file);
}


