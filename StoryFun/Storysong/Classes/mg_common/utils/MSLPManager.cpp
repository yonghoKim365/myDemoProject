
#include "MSLPManager.h"

MSLPManager::MSLPManager():
_isFinish(false)
{
    init();
}

MSLPManager::~MSLPManager()
{
    
}

bool MSLPManager::init()
{
    
    return true;
}

int MSLPManager::getAlphabetNum()
{
    int ret = 0;
    
    std::string currentAlphabet = MSLPInterface::getInstance()->getAlphabet();
    std::transform(currentAlphabet.begin(), currentAlphabet.end(), currentAlphabet.begin(), tolower);
    
    log("currentAlphabet: %s", currentAlphabet.c_str());
    
    std::string alphabets[26] = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
    
    for (int i = 0; i<26; i++)
    {
        std::string alphabet = alphabets[i];
        
        if (alphabet == currentAlphabet)
        {
            ret = i+1;
            break;
        }
    }
    return ret;
}

int MSLPManager::getDayNum()
{
    int ret = 0;
    ret = MSLPInterface::getInstance()->getDayNum();
    return ret;
}

int MSLPManager::getBookNum()
{
    int ret = 0;
    ret = MSLPInterface::getInstance()->getBookNum();
    return ret;
}


int MSLPManager::getQuizType()
{
    int ret = 0;
    std::string type = MSLPInterface::getInstance()->getQuizType();
    std::transform(type.begin(), type.end(), type.begin(), tolower);
    
    if (type == "d")
    {
        ret = 0;
    }
    else if (type == "w")
    {
        ret = 1;
    }
    else if (type == "r")
    {
        ret = 2;
    }
    
    return ret;
}

std::vector<int>* MSLPManager::getQuestList()
{
    return MSLPInterface::getInstance()->getQuestList();
}

void MSLPManager::beginProgress(int progress)
{
    MSLPInterface::getInstance()->beginProgress(progress);
}

void MSLPManager::progress(int progress)
{
    MSLPInterface::getInstance()->progress(progress);
}

int MSLPManager::getProgress()
{
    int ret = 0;
    ret = MSLPInterface::getInstance()->getProgress();
    return ret;
}

void MSLPManager::finishProgress(bool isEnd)
{
    MSLPInterface::getInstance()->finishProgress(isEnd);
}

void MSLPManager::startNextContent()
{
    MSLPInterface::getInstance()->startNextContent();
}

void MSLPManager::testResult(int questIdx, bool isCorrect)
{
    MSLPInterface::getInstance()->testResult(questIdx, isCorrect);
}

bool MSLPManager::getHasNext()
{
    bool ret = false;
    std::string str = MSLPInterface::getInstance()->getHasNext();
    
    if (str == "y")
    {
        ret = true;
    }
    
    return ret;
}

bool MSLPManager::isFinished()
{
    bool ret = MSLPInterface::getInstance()->isFinished();
    return ret;
}

std::string MSLPManager::getPortfolioPath()
{
    std::string ret = "";
    ret = MSLPInterface::getInstance()->getPortfolioPath();
    return ret;
}

void MSLPManager::finishPortfolio(int questIdx, std::string path)
{
    MSLPInterface::getInstance()->finishPortfolio(questIdx, path);
}
