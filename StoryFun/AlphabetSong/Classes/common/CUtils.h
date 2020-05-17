//
//  CUtils.h
//  GafLib2Test
//
//   on 2015. 4. 23..
//
//

#ifndef GafLib2Test_CUtils_h
#define GafLib2Test_CUtils_h
#include "cocos2d.h"
#if(CC_TARGET_PLATFORM == CC_PLATFORM_ANDROID)
#include "platform/android/jni/JniHelper.h"
#endif
USING_NS_CC;

namespace timeUtil
{
    static inline void getTimeM(int& iMin, int& iSec)
    {
        time_t rawtime;
        time (&rawtime);
        static struct tm * timeinfo;
        timeinfo = localtime (&rawtime);
        
        iMin = timeinfo->tm_min;
        iSec = timeinfo->tm_sec;
    }
    static inline void getTimeH(int& iHour, int& iMin)
    {
        time_t rawtime;
        time (&rawtime);
        static struct tm * timeinfo;
        timeinfo = localtime (&rawtime);
        
        iHour = timeinfo->tm_hour;
        iMin = timeinfo->tm_min;
    }
    static inline void getDate(int& iYear, int&iMonth, int&iDay)
    {
        time_t rawtime;
        time (&rawtime);
        static struct tm * timeinfo;
        timeinfo = localtime (&rawtime);
        
        iYear = timeinfo->tm_year+1900;
        iMonth = timeinfo->tm_mon+1;
        iDay = timeinfo->tm_mday;
    }
    static inline void getDateTime(int& iYear, int&iMonth, int&iDay, int&iHour, int&iMin, int&iSec)
    {
        time_t rawtime;
        time (&rawtime);
        static struct tm * timeinfo;
        timeinfo = localtime (&rawtime);
        
        iYear = timeinfo->tm_year+1900;
        iMonth = timeinfo->tm_mon+1;
        iDay = timeinfo->tm_mday;
        iHour = timeinfo->tm_hour;
        iMin = timeinfo->tm_min;
        iSec = timeinfo->tm_sec;
    }
    
    
    
    static inline std::int64_t millisecondNow()
    {
        struct timeval now;
        gettimeofday(&now, NULL);
        return (now.tv_sec * 1000 + now.tv_usec / 1000);
    }
    
}

namespace textUtil
{
    static inline const char* addCommaText(double number)
    {
        if(number<1000)
        {
            return String::createWithFormat("%d",(int)number)->getCString();
        }
        static char result[20]="";
        char buf[20];
        int data[10] = {0};
        int a, c=number;
        int index=0;
        while(c) /* 뒤에서부터 3자리씩 잘라서 data에 저장.. */
        {
            a = c%1000;
            c = c/1000;
            data[index++] = a;
        }
        a = index-1;
        sprintf(buf,"%d", data[a]);
        strcpy(result,buf);
        
        for(--a ; a>=0; --a) /* data[]에 저장된 뒤쪽부터 result에 추가시킴.. */
        {
            sprintf(buf,",%03d", abs(data[a]));
            strcat(result,buf);
        }
        return String::createWithFormat("%s",result)->getCString();
    }
    
    static inline void replaceText(std::string& stringA, const std::string searchTxt, const std::string replaceText)
    {
        size_t size;
        while(true)
        {
            size = stringA.find(searchTxt);
            if(size==std::string::npos)
            {
                break;
            }
            stringA.replace(size,searchTxt.length(),replaceText);
        }
    }
}

#endif
