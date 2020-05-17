using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class RacoonRegex
{
    /// <summary>
    /// 유저 아이디 유효성 검사.
    /// </summary>
    public static bool IsUserID(string username)
    {
        string pattern;

        // 1. 첫 번째 아이디는 문자만 가능
        // 2. 문자, 숫자 가능
        // 3. 4 ~ 14 자 가능
        pattern = @"^[a-zA-Z][a-zA-Z0-9]{3,13}$";

        Regex regex = new Regex(pattern);
        return regex.IsMatch(username);
    }

    /// <summary>
    /// 메일형식 검사.
    /// </summary>
    public static bool IsEmailCheck(string text)
    {
        string pattern;
        pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

        Regex regex = new Regex(pattern);

        //if (regex.IsMatch(text))
        //{
        //    Debug.LogWarning("Valid Email address ");
        //}
        //else
        //{
        //    Debug.LogWarning("Not a valid Email address ");
        //}
        return regex.IsMatch(text);
    }

    /// <summary>
    /// 특수문자가 포함되어 있는지 검사한다.
    /// </summary>
    public static bool ContainsSC(string text)
    {
        return Regex.IsMatch(text, "[^\\w\\._]");
    }

    //< 닉네임 체크
    public static bool IsNickName(string nickname)
    {
        string pattern;
        /*
        1. 문자 숫자 가능
        2. 2 ~ 8 자 가능
        */
        pattern = @"[a-zA-Z0-9ㄱ-ㅎㅏ-ㅣ가-힣一-龥あ-ゟァ-ヾ]{1,9}$";

        Regex regex = new Regex(pattern);
        return regex.IsMatch(nickname);
    }

    /// <summary>
    /// 문자열중에 특수문자를 지운다.
    /// </summary>
    public static string RemoveSC(string text)
    {
        return Regex.Replace(text, "[^\\w\\._]", "");
    }

    /// <summary>
    /// 자음과 모음이 포함되어 있는지 검사
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool CheckConsonantAndVowel(string str)
    {
        string pattern = "[ㄱ-ㅎㅏ-ㅣ]";

        Regex regex = new Regex(pattern);

        return regex.IsMatch(str);
    }

    /// <summary>
    /// 패스워드형식에러타입
    /// </summary>
    public enum PASSWORD_ERROR { NONE, LENGTH, KOREANSTR, }

    /// <summary>
    /// 패스워드 형식 검사
    /// </summary>
    /// <param name="strPW">패스워드</param>
    /// <param name="errtype">반환될 패스워드 에러</param>
    /// <returns>패스워드 형식에 일치하는지 여부</returns>
    public static bool IsPasswordDone(string strPW, ref PASSWORD_ERROR errtype)
    {
        // 6~12자 사이가 아니면 실패
        if (strPW.Length < 6 || strPW.Length > 12)
        {
            errtype = PASSWORD_ERROR.LENGTH;
            return false;
        }
        // 자음과 모음이 포함되어있으면 실패
        if (CheckConsonantAndVowel(strPW))
        {
            errtype = PASSWORD_ERROR.KOREANSTR;
            return false;
        }
        return true;
    }

    /// <summary>
    /// 스트립 슬래쉬
    /// </summary>
    /// <param name="InputTxt"></param>
    /// <returns></returns>
    public static string StripSlashes(string InputTxt)
    {
        // List of characters handled:
        // \000 null
        // \010 backspace
        // \011 horizontal tab
        // \012 new line
        // \015 carriage return
        // \032 substitute
        // \042 double quote
        // \047 single quote
        // \057 /
        // \134 backslash
        // \140 grave accent

        string Result = InputTxt;

        try
        {
            Result = System.Text.RegularExpressions.Regex.Replace(InputTxt, @"(\\)([\000\010\011\012\015\032\042\047\057\134\140])", "$2");
        }
        catch (System.Exception Ex)
        {
            // handle any exception here
            System.Console.WriteLine(Ex.Message);
        }

        return Result;
    }

}
