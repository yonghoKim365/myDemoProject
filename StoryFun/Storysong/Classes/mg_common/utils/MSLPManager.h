
#ifndef __MSLPMANAGER_H__
#define __MSLPMANAGER_H__

#include "SingletonBase.h"
#include "cocos2d.h"

#include "MSLPInterface.h"

USING_NS_CC;

    
class MSLPManager: public CSingletonBase<MSLPManager>
{

public:
    MSLPManager();
    ~MSLPManager();
    bool init();

public:
    
    bool    _isFinish;
    
    /**
     * 1권 알파벳 정보
     */
    int getAlphabetNum();
    
    /**
     * 일차 정보
     */
    int getDayNum();
    
    /**
     * 권 정보
     */
    int getBookNum();
    
    
    /**
     * Quiz 유형 D / W / R
     */
    int getQuizType();
    
    /**
     * Quest List
     */
    std::vector<int>* getQuestList();
    
    /**
     * 진도 시작
     */
    void beginProgress(int progress);
    
    /**
     * 진도 변경
     */
    void progress(int progress);
    
    /**
     * 이전 진도 정보 (최초 진입시 progress값은 -1)
     */
    int getProgress();
    
    /**
     * 진도 종료
     */
    void finishProgress(bool isEnd);
    
    /**
     * 다음 콘텐츠로 이동
     */
    void startNextContent();
    
    /**
     * 퀴즈 결과 전달
     * @param questIdx 퀴즈 Index
     * @param correct  퀴즈를 맞춘 경우 true, 그렇지 않으면 false
     */
    void testResult(int questIdx, bool isCorrect);
    
    /**
     * 연속된 다음 컨텐츠 유무 ( "y" or "n" )
     */
    bool getHasNext();
    
    /**
     * 이전에 완독한 적이 있는지 여부 ( "y" or "n" )
     */
    bool isFinished();
    
    /**
     * 포트폴리오 path
     */
    std::string getPortfolioPath();
    
    
    /**
     * 포트폴리오 저장
     */
    void finishPortfolio(int questIdx, std::string path);
};


#endif
