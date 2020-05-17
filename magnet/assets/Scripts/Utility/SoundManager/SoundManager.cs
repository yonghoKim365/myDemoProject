using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eUISfx
{
    //Click = 0,//클릭 사운드
    //Close=1,//닫기 사운드
    //DropGold = 2,//골드 드랍 사운드
    //GetGold = 3,//골드 획득 사운드
    //GetExp = 4,//획득 경험치 사운드
    //LevelUp = 5,//레벨업 사운드
    //GetRewardCard = 6,//카드획득 사운드
    //ComposePartner = 7,//파트너합성 사운드
    
    UI_button_open_close	= 100,//대부분의 버튼음(초기화 버튼 제외)
    UI_cos_skill_up			= 101,//코스튬 스킬 강화
    UI_cos_upgrade			= 102,//코스튬 승급
    UI_dark_tower_choice	= 103,//마계의 탑 선택
    //UI_dark_tower_step		= 104,//마계의 탑 선택 (예비입니다. 작업 X)
    UI_fighting_power_up	= 105,//전투력 상승
    UI_gatcha_normal_drop	= 106,//일반 뽑기 상자 떨어지고 열림 애니까지
    UI_gatcha_normal_open	= 107,//일반 뽑기 확인 (판 회전)
    UI_gatcha_rare_drop		= 108,//고급 뽑기 상자 떨어지고 열림 애니까지
    UI_gatcha_rare_open		= 109,//고급 뽑기 확인 (판 회전)
    UI_item_compose			= 110,//아이템 합성
    UI_item_enchant_upgrade	= 111,//아이템 강화 / 승급
    UI_item_equip			= 112,//아이템 장착
    UI_item_off				= 113,//아이템 벗기
    UI_item_regret			= 114,//아이템 분해
    UI_jewel_equip			= 115,//보석 장착
    UI_opening_shine  		= 116,//로그인에서 게임로고가 최초 반짝일 
    UI_opening_signboard	= 117,//오프닝 간판 박힐 때
    UI_par_buff_choice		= 118,//파트너 버프 선택
    UI_par_buff_enchant		= 119,//파트너 버프 강화
    UI_par_grow_up_01		= 120,//파트너 성장(경험치 오를때)
    UI_par_grow_up_02		= 121,//파트너 성장(레벨업 할때)
    UI_par_upgrade_01		= 122,//파트너 승급(별 박힐 때)
    UI_par_upgrade_02		= 123,//파트너 승급(등급이 오를 때)
    UI_quest_conversation	= 124,//퀘스트 대화창(텍스트 입력될 때)
    UI_reset_warn_negation	= 125,//모든 초기화 버튼 / 각종 부족(에너지/골드/원보/재료) 알림 팝업 / 선행퀘스트 진행필요 팝업

    UI_achive_attendence_alarm	=126,//업적 달성 알림음 / 혜택 달성음
    UI_gatcha_normal_ten		=127,//일반뽑기 10회 드랍부터 오픈까지
    UI_gatcha_rare_ten			=128,//고급뽑기 10회 드랍부터 오픈까지
    UI_incre_number				=129,//상점에서 개수 변화 효과음( +, - )
    UI_item_upgrade_02			=130,//아이템 승급 순간
    //UI_magnet_games				=131,//회사 로고(최초 게임 실행)
    UI_mail_delete				=132,//(메일삭제 -> 삭제 하시겠습니까? 예 누르는 순간 출력[버튼음은 삭제])
    UI_multi_select				=133,//분해할 때 한 번에 여러 아이템을 체크할 때 나는 효과음
    UI_par_enchant				=134,//파트너 강화 효과음

    UI_revival				= 200,//부활
    UI_boss_revial			= 201,//보스 출현
    UI_gold_get				= 202,//골드 획득
    UI_reward_choice		= 203,//보상 선택(스테이지 클리어 후 보상 선택)
    UI_reward_popup			= 204,//소탕/퀘스트 보상 등의 팝업으로 출력되는 보상들
    UI_reward_preview		= 205,//보상 미리보기(클리어시 보상 미리보기 화면임)
    UI_victory_star			= 206,//승리 화면 별박힘 소리
    UI_count_exp_dungeon	= 207,//골드 던전 카운트 다운
    UI_count_gold_dungeon	= 208,//경험치 던전 카운트 다운
    UI_count_pvp            = 209,//전투UI 차관 카운트
    UI_reward_hide          = 210,//전투UI 아이템이 감춰지고 섞일때 나오는 효과음

    Success = 309,//성공 사운드
    Fail = 310,//실패 사운드
    Forced = 311,//강제 종료
}

public enum eBGMSfx//코드상 강제 실행 BGM들
{
    ReViveStay = 312,//부활 대기
}

/// <summary>
/// 추후엔 전부 어셋번들로 읽어오고 동시플레이 관련 수정해야한다
/// </summary>
public class SoundManager : Immortal<SoundManager>
{
    //일단 SoundHelper을 제거하기위한 임시 데이터
    //public static List<string> UISoundList = new List<string>();


    private Dictionary<uint, AudioClip> SoundList = new Dictionary<uint, AudioClip>();
    private Dictionary<string, AudioClip> BGMList = new Dictionary<string, AudioClip>();

    private AudioSource BgmAudio;

    protected override void Init()
    {
        base.Init();
        BgmAudio = gameObject.AddComponent<AudioSource>();
        BgmAudio.loop = true;
    }

    public void Clean()
    {
        var enumerator = SoundList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Resources.UnloadAsset(enumerator.Current.Value);            
        }

        SoundList.Clear();

        //var enumerator2= BGMList.GetEnumerator();
        //while (enumerator2.MoveNext())
        //{
        //    Resources.UnloadAsset(enumerator2.Current.Value);
        //}

        //BGMList.Clear();
    }

    void Start()
    {
        /*
        UISoundList = new List<string>() {
          "UI_main",
          "UI_back",
          "Success",
          "Fail",
          "UI_Moneydrop",
          "UI_Moneyget",
          "Exp",
          "Level_up",
          "GetRewardCard",
          "Gatcha_Succes"
       };
        */
    }

    public void PlayBgmSound(string FileName)
    {
        if (SceneManager.instance.IsPlayBgm == false)
            return;

        AudioClip clip = null;

        if (!BGMList.ContainsKey(FileName))
        {
            //없으면 파일이름을 알아온후 로드
            //string soundName = _LowDataMgr.GetSoundName(soundID);
            //if (soundName == null)
            //    return;

            clip = Resources.Load(string.Format("Sound/{0}", FileName)) as AudioClip;

            if(clip != null)
            {
                BGMList.Add(FileName, clip);
            }
        }
        else
        {
            clip = BGMList[FileName];
        }

        //NGUITools.PlaySound(clip);
        SetBgmVolume(SceneManager.instance.optionData.SoundVolume);
        PlayBgmSoundClip(clip);
    }

    public void PlayBgmSound(uint soundID)
    {
        if (SceneManager.instance.IsPlayBgm == false)
            return;

        AudioClip clip = null;

        if (!SoundList.ContainsKey(soundID))
        {
            //없으면 파일이름을 알아온후 로드
            string soundName = _LowDataMgr.GetSoundName(soundID);
            if (soundName == null)
                return;

            clip = Resources.Load(string.Format("Sound/{0}", soundName)) as AudioClip;
        }
        else
        {
            clip = SoundList[soundID];
        }

        //NGUITools.PlaySound(clip);
        PlayBgmSoundClip(clip);
    }

    public void PlaySfxSound(uint soundID)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
        {
            //NGUITools.soundVolume = 0;
            return;
        }

        AudioClip clip = null;

        if (!SoundList.ContainsKey(soundID))
        {
            //없으면 파일이름을 알아온후 로드
            string soundName = _LowDataMgr.GetSoundName(soundID);
            if (soundName == null)
                return;

            clip = Resources.Load(string.Format("Sound/{0}", soundName)) as AudioClip;
        }
        else
        {
            clip = SoundList[soundID];
        }

        NGUITools.PlaySound(clip);
        //UIMgr.instance.PlayBgmSound(clip);
    }

    public void PlayVoiceSound(uint soundID)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
        {
            //NGUITools.soundVolume = 0;
            return;
        }

        AudioClip clip = null;

        if (!SoundList.ContainsKey(soundID))
        {
            //없으면 파일이름을 알아온후 로드
            string soundName = _LowDataMgr.GetSoundName(soundID);
            if (soundName == null)
                return;

            clip = Resources.Load(string.Format("Sound/Voice/{0}", soundName)) as AudioClip;
        }
        else
        {
            clip = SoundList[soundID];
        }

        NGUITools.PlaySound(clip);
        //UIMgr.instance.PlayBgmSound(clip);
    }

    public void PlaySfxUnitSound(uint soundID, AudioSource source, Transform effectTransform, bool ForcePlay = false)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
        {
            //NGUITools.soundVolume = 0;
            return;
        }

        AudioClip clip = null;

        if(!ForcePlay)
        {
            PlayerController pctrl = G_GameInfo.PlayerController;

            if (pctrl == null)
                return;

            if (pctrl.Leader == null)
                return;

            if (Vector3.Distance(pctrl.Leader.transform.position, effectTransform.position) >= 22)
                return;
        }

        if (!SoundList.ContainsKey(soundID))
        {
            //없으면 파일이름을 알아온후 로드
            string soundName = _LowDataMgr.GetSoundName(soundID);
            if (soundName == null)
                return;

            clip = Resources.Load(string.Format("Sound/{0}", soundName)) as AudioClip;
        }
        else
        {
            clip = SoundList[soundID];
        }

        source.PlayOneShot(clip);

        //NGUITools.PlaySound(clip);


        //UIMgr.instance.PlayBgmSound(clip);
    }

    public void PlayUIClip(AudioClip audio)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
            return;

        NGUITools.PlaySound(audio);
    }

    public void PlayUnitVoiceSound(uint soundID, AudioSource source, Transform effectTransform, bool ForcePlay = false)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
        {
            //NGUITools.soundVolume = 0;
            return;
        }

        AudioClip clip = null;

        if (!ForcePlay)
        {
            PlayerController pctrl = G_GameInfo.PlayerController;

            if (pctrl == null)
                return;

            if (pctrl.Leader == null)
                return;

            if (Vector3.Distance(pctrl.Leader.transform.position, effectTransform.position) >= 22)
                return;
        }

        if (!SoundList.ContainsKey(soundID))
        {
            //없으면 파일이름을 알아온후 로드
            string soundName = _LowDataMgr.GetSoundName(soundID);
            if (soundName == null)
                return;

            clip = Resources.Load(string.Format("Sound/Voice/{0}", soundName)) as AudioClip;
        }
        else
        {
            clip = SoundList[soundID];
        }

        source.PlayOneShot(clip);

        //NGUITools.PlaySound(clip);


        //UIMgr.instance.PlayBgmSound(clip);
    }

    public void PlaySfxSound(uint soundID, Transform effectTransform)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
        {
            //NGUITools.soundVolume = 0;
            return;
        }

        AudioClip clip = null;

        PlayerController pctrl = G_GameInfo.PlayerController;

        if (pctrl == null)
            return;

        if (pctrl.Leader == null)
            return;

        if (Vector3.Distance(pctrl.Leader.transform.position, effectTransform.position) >= 22)
            return;

        if (!SoundList.ContainsKey(soundID))
        {
            //없으면 파일이름을 알아온후 로드
            string soundName = _LowDataMgr.GetSoundName(soundID);
            if (soundName == null)
                return;

            clip = Resources.Load(string.Format("Sound/{0}", soundName)) as AudioClip;
        }
        else
        {
            clip = SoundList[soundID];
        }
        
        NGUITools.PlaySound(clip);
        
        
        //UIMgr.instance.PlayBgmSound(clip);
    }

    public void PlaySfxSound(eUISfx sound, bool isStopBGM)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
        {
            //NGUITools.soundVolume = 0;
            return;
        }
        
        AudioClip clip = null;
        if (!SoundList.ContainsKey((uint)sound))
        {
            //없으면 파일이름을 알아온후 로드
            string soundName = _LowDataMgr.GetSoundName((uint)sound);
            if (soundName == null)
                return;

            clip = Resources.Load(string.Format("Sound/{0}", soundName)) as AudioClip;
        }
        else
        {
            clip = SoundList[(uint)sound];
        }

        if (isStopBGM)
        {
            //float runTime = clip.length;
            PlayBgmSoundClip(null);
            //TempCoroutine.instance.FrameDelay(runTime, () => {
            //    SceneManager.instance.CurrentStateBase().PlayMapBGM(Application.loadedLevelName);
            //});
        }

        NGUITools.PlaySound(clip);
        
    }
    
    public void PlaySfxSound(string soundName)
    {
        if (SceneManager.instance.IsPlaySoundFx == false)
        {
            //NGUITools.soundVolume = 0;
            return;
        }
        
        if (string.IsNullOrEmpty(soundName) || soundName.Contains("0"))
            return;

        AudioClip audio = Resources.Load(string.Format("Sound/{0}", soundName)) as AudioClip;
        NGUITools.PlaySound(audio);
    }

    /// <summary>
    /// BGM의 경우에는 계속 실행되어야 해서 UIMgr이 들고다니게 했다.
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBgmSoundClip(AudioClip clip)
    {
        //기존의 BGM은 종료
        if (BgmAudio.clip != null)
        {

            //만약 동일한 실행을 한다면 무시
            if (BgmAudio.clip == clip && BgmAudio.isPlaying)
                return;
        }

        if (BgmAudio.isPlaying)
        {
            //음악을 플레이 중일경우
            /*
            if (AudioFade)
            {
                //페이드 중일경우
                StopCoroutine(FadeOutAudio(clip));
                StopCoroutine(FadeInAudio(clip));
            }
            */

            StartCoroutine(FadeOutAudio(clip));
            //StartCoroutine(FadeInAudio(clip));
        }
        else
        {
            StartCoroutine(FadeInAudio(clip));
        }
    }

    //볼륨이 1일때만 적용됨 추후 볼륨조절 추가되면 수정해야함
    float FadeTime = 1;
    IEnumerator FadeOutAudio(AudioClip clip)
    {
        //yield return null;

        BgmAudio.volume = 1f;
        float startVolume = SceneManager.instance.optionData.SoundVolume;//1f;

        while (BgmAudio.volume > 0)
        {
            yield return null;
            BgmAudio.volume -= startVolume * Time.deltaTime / FadeTime;
            //Debug.Log(BgmAudio.volume);
            yield return null;
        }

        BgmAudio.Stop();

        if (clip != null)
            StartCoroutine(FadeInAudio(clip));
    }

    IEnumerator FadeInAudio(AudioClip clip)
    {
        //yield return null;

        BgmAudio.clip = clip;

        BgmAudio.volume = 0f;
        float startVolume = 1f;

        BgmAudio.Play();

        while (BgmAudio.volume < /*1*/SceneManager.instance.optionData.SoundVolume)
        {
            yield return null;
            BgmAudio.volume += startVolume * Time.deltaTime / FadeTime;
            //Debug.Log(BgmAudio.volume);
            yield return null;
        }
    }

    public void SetBgmVolume(float volume)
    {
        BgmAudio.volume = volume;
    }

    //public bool IsPlayBgm()
    //{
    //    return BgmAudio.isPlaying;
    //}


    public void BGMStop()
    {
        BgmAudio.Stop();
    }

    public void BGMPlay()
    {
        BgmAudio.Play();
    }
}
