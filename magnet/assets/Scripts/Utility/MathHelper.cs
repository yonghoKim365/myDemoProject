using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 현재 게임 전용 계산관련 도움용 클래스
/// </summary>
public class MathHelper
{
    public static Vector3 UI_Center_Pos = new Vector3( ( Screen.width * 0.5f ), ( Screen.height * 0.5f ), 0 );
    public static Rect ScreenRect = new Rect( 0, 0, Screen.width, Screen.height );

    public static Color TabBaseColor = new Color32( 227, 154, 13, 255 );

    static public Vector3 WorldToUIPosition(Vector3 worldPos)
    {
        if (SkillEventMgr.Live && SkillEventMgr.instance.IsFocusEnable)
            return SkillEventMgr.instance.FocusingCam.Mycamera.WorldToScreenPoint(worldPos) - UI_Center_Pos;

        if (Camera.main == null)
            return Vector3.zero;

        return Camera.main.WorldToScreenPoint( worldPos ) - UI_Center_Pos;
    }

    static public Vector3 WorldUIToUIPosition(Vector3 worldPos)
    {
        return UIMgr.instance.UICamera.camera.WorldToScreenPoint( worldPos ) - UI_Center_Pos;
    }

    static public Vector3 WorldToUIPosition(Vector3 worldobject, Transform uitr, float depth)
    {
        Vector3 pos = Camera.main.WorldToViewportPoint( worldobject );

        uitr.position = UIMgr.instance.UICamera.camera.ViewportToWorldPoint( pos );
        pos = uitr.localPosition;
        pos.z = depth;

        return pos;
    }

    static public Quaternion GetYRotation(Vector3 Dest, Transform Start, float delta = 1)
    {
        Quaternion rot = ( Dest == Start.position ) ? Start.rotation : Quaternion.Lerp( Start.rotation, Quaternion.LookRotation( Dest - Start.position ), delta );
        rot.eulerAngles = new Vector3( 0, rot.eulerAngles.y, 0 );

        return rot;
    }

    static public bool InAngle(Vector3 CasterDir, Vector3 CasterPos, Vector3 TargetPos, float Angle)
    {
        if (Angle == 360)
            return true;

        float Ac = Mathf.Acos(Vector3.Dot(CasterDir.normalized, (TargetPos - CasterPos).normalized));
        Ac = (float.IsNaN(Ac)) ? 1 : Ac;

        float Rad = 180f / Mathf.PI;
        if (Ac == 1 || (Ac * Rad) <= Angle / 2f)
            return true;

        return false;
    }

    public static Vector3 PlanarDirection(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        return new Vector3( direction.x, 0.0f, direction.z );
    }

    public static void RotateY(ref Vector3 v, float angle)
    {
        //float sin = Mathf.Sin(angle);
        //float cos = Mathf.Cos(angle);

        //float tx = v.x;
        //float tz = v.z;
        //v.x = (cos * tx) + (sin * tz);
        //v.z = (cos * tz) - (sin * tx);

        v = Quaternion.AngleAxis( angle, Vector3.up ) * v;
    }

    /// <summary>
    /// 공격 가능 여부 판단
    /// </summary>
    /// <param name="dist">자신과 타겟과의 거리</param>
    /// <param name="attackRange">자신의 공격가능거리</param>
    /// <param name="forward">자신의 현재 정면</param>
    /// <param name="angle">공격가능각도</param>
    /// <param name="myRadius">나의 radius</param>
    /// <param name="targetRadius">타겟의 radius</param>
    public static bool IsInRange(Vector3 dist, float attackRange, Vector3 forward, float angle, float myRadius = 0f, float targetRadius = 0f)
    {
        // 높낮이 무시
        dist.y = 0;
        forward.y = 0;

        return ( dist.magnitude - targetRadius ) <= ( attackRange + myRadius )
            && Vector3.Angle( dist.normalized, forward ) <= ( angle * 0.5001f );
    }

    public static bool IsInRange(Vector3 dist, float attackRange, float myRadius = 0f, float targetRadius = 0f)
    {
        return ( dist.magnitude - targetRadius ) <= ( attackRange + myRadius );
    }
    static public string TimeToString(ulong timesecond, bool bhideMin = false, bool bhideSec = false)
    {
        //return Application.systemLanguage == SystemLanguage.Korean ? TimeToString(timesecond, "일", "시간", "분", "초",bhideMin,bhideSec) : TimeToString(timesecond, "day", "h", "m", "s",bhideMin,bhideSec);
        return TimeToString( timesecond, "일", "시간", "분", "초", bhideMin, bhideSec );
    }
    static public string TimeToString(ulong timesecond, string strDay, string strHour, string strMinute, string strSecond, bool bhideMin = false, bool bhideSec = false)
    {
        ulong day = timesecond / ( 60 * 60 * 24 );
        ulong hour = ( timesecond % ( 60 * 60 * 24 ) ) / ( 60 * 60 );
        ulong minute = ( timesecond % ( 60 * 60 ) ) / 60;
        ulong second = timesecond % 60;

        string strDate = "";
        bool isSpace = false;
        if (0 < day)
        {
            strDate += string.Format( "{0}{1}", day, strDay );
            isSpace = true;
        }
        if (0 < hour)
        {
            if (isSpace == true)
                strDate += string.Format( " {0}{1}", hour, strHour );
            else
                strDate += string.Format( "{0}{1}", hour, strHour );

            isSpace = true;
        }
        if (bhideMin == false)
        {
            if (0 < minute)
            {
                if (isSpace == true)
                    strDate += string.Format( " {0}{1}", minute, strMinute );
                else
                    strDate += string.Format( "{0}{1}", minute, strMinute );

                isSpace = true;
            }
        }
        if (bhideSec == false)
        {
            if (0 == second)
            {
                if (strDate == "")
                {
                    if (isSpace == true)
                        strDate += string.Format( " {0}{1}", second, strSecond );
                    else
                        strDate += string.Format( "{0}{1}", second, strSecond );

                    isSpace = true;
                }

            }
            else if (0 < second)
            {
                if (isSpace == true)
                    strDate += string.Format( " {0}{1}", second, strSecond );
                else
                    strDate += string.Format( "{0}{1}", second, strSecond );

                isSpace = true;
            }

        }

        return strDate;
    }

    static public string Date_Day(ulong second_)
    {
        ulong day = second_ / ( 60 * 60 * 24 );

        string strDate = "";
        if (0 < day)
        {
            strDate += string.Format( "{0}{1}", day, "Day" );
        }
        else strDate = Date_Hour( second_ );

        return strDate;
    }

    static public string Date_Hour(ulong second_)
    {
        ulong hour = ( second_ % ( 60 * 60 * 24 ) ) / ( 60 * 60 );

        string strDate = "";
        if (0 < hour)
        {
            strDate += string.Format( "{0}{1}", hour, "Hour" );
        }
        else strDate = Date_Minite( second_ );
        return strDate;
    }

    static string Date_Minite(ulong second_)
    {
        ulong minute = ( second_ % ( 60 * 60 ) ) / 60;

        string strDate = "";
        if (0 < minute)
        {
            strDate += string.Format( "{0}{1}", minute, "Min" );
        }
        else strDate = Date_Second( second_ );
        return strDate;
    }

    static string Date_Second(ulong second_)
    {
        ulong second = second_ % 60;

        string strDate = "";
        if (0 < second)
        {
            strDate += string.Format( "{0}{1}", second, "Sec" );
        }
        return strDate;
    }
    static public string GetStringFormat(string des, string src)
    {
        string tmpstr = "";
        string resultstr = des;
        for (int i=0; i < src.Split( ',' ).Length; i++)
        {
            tmpstr = "{" + i.ToString() + "}";
            resultstr = resultstr.Replace( tmpstr, src.Split( ',' )[i] );
        }
        return resultstr;
    }
    
    //static Dictionary<uint , uint> resulttimedic = new Dictionary<uint, uint>();
    
    static public string StarGradeToString(byte grade, byte awaken)
    {
        return awaken == 0 ? "n_star_" + grade : "n_star_" + grade + "_g";
    }
    static public string ClassimgToSring(byte type)
    {
        if (type == (byte)ProfessionType.n_icon_unittype_01)
            return ProfessionType.n_icon_unittype_01.ToString();
        else if (type == (byte)ProfessionType.n_icon_unittype_03)
            return ProfessionType.n_icon_unittype_03.ToString();
        else if (type == (byte)ProfessionType.n_icon_unittype_04)
            return ProfessionType.n_icon_unittype_04.ToString();

        return "";
    }
    static public string PropertiToString(byte type)
    {
        if (type == (byte)UnitPropertiType.n_icon_attribute_01)
            return UnitPropertiType.n_icon_attribute_01.ToString();
        else if (type == (byte)UnitPropertiType.n_icon_attribute_02)
            return UnitPropertiType.n_icon_attribute_02.ToString();
        else if (type == (byte)UnitPropertiType.n_icon_attribute_03)
            return UnitPropertiType.n_icon_attribute_03.ToString();
        else if (type == (byte)UnitPropertiType.n_icon_attribute_04)
            return UnitPropertiType.n_icon_attribute_04.ToString();
        else if (type == (byte)UnitPropertiType.n_icon_attribute_05)
            return UnitPropertiType.n_icon_attribute_05.ToString();
        else if (type == (byte)UnitPropertiType.n_icon_attribute_06)
            return UnitPropertiType.n_icon_attribute_06.ToString();
        return "";
    }
}
