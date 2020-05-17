
#ifndef __Common_Check_Correct__
#define __Common_Check_Correct__

#include "cocos2d.h"
#include "GAF.h"

USING_NS_CC;
USING_NS_GAF;

class Common_Check_Correct : public Node
{
public:
    
    Common_Check_Correct();
    ~Common_Check_Correct();
    
    static Common_Check_Correct* create(bool isCorrect);

    virtual bool init();
    virtual bool initWithCorrect(bool isCorrect);
    //override
    virtual void onEnter() override;
    virtual void onExit() override;

    CREATE_FUNC(Common_Check_Correct);
    
    virtual void onFinishSequence( GAFObject * object, const std::string& sequenceName );
    
public:
    
    GAFAsset* m_asset;
    GAFObject* m_mainObject;
    void show();
    void loop();
    void playGuide();
    void stopGuide();
};

#endif /* defined(__Common_Check_Correct__) */
