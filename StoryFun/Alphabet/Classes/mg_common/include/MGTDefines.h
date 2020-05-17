
#ifndef __MGTDEFINES_H__
#define __MGTDEFINES_H__

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// __NotificationCenter Observe Key
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Touch Priority on MGTPopup menu
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define MGTTOUCH_PRIORITY_POPUP      -129
#define MGTTOUCH_PRIORITY_POPUPMENU  -130



///////////////////////////////////////////////////////////////
// NARRATION
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_BGM_FINISHCALL                          "notification_bgm_finishcall"
#define MGTNOTIFICATION_EFFECT_FINISHCALL                       "notification_effect_finishcall"
#define MGTNOTIFICATION_NAR_FINISHCALL                          "notification_nar_finishcall"

///////////////////////////////////////////////////////////////
// VIDEO
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_VIDEO_FINISHCALL                        "notification_video_finishcall"
#define MGTNOTIFICATION_VIDEO_BUTTONCALL                        "notification_video_btncall"
#define MGTNOTIFICATION_VIDEO_INDEX_BUTTONCALL                  "notification_video_index_btncall"
#define MGTNOTIFICATION_FRAME_VIDEOVIEW_FINISHCALL              "notification_frame_videoview_finishcall"
///////////////////////////////////////////////////////////////
// CAMERA
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_PICTURE_FINISHCALL                        "notification_camera_picturefinishcall"
#define MGTNOTIFICATION_VIDEO_RECORDING_FINISHCALL                "notification_video_recordingfinishcall"
#define MGTNOTIFICATION_CAMERA_ONPAUSE_RECORDING_CALL            "notification_camera_onpause_recording_call"

///////////////////////////////////////////////////////////////
// VOICE RECORDING
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_VOICE_RECORDING_FINISHCALL                "notification_voice_recording_finishcall"
#define MGTNOTIFICATION_RECORDED_VOICE_FINISHCALL                 "notification_recorded_voice_finishcall"
// Info Close
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_INFO_CLOSECALL                           "notification_info_closecall"
// Guide Close
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_GUIDE_CLOSECALL                           "notification_guide_closecall"
// Tip Close
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_TIP_CLOSECALL                            "notification_tip_closecall"

// Reward Popup Close
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_REWARD_ITEM_COMPLETE_CALL              "notification_item_popup_complete_Call"
#define MGTNOTIFICATION_REWARD_POPUP_COMPLETE_CALL              "notification_reward_popup_complete_Call"

// MuteAlert Popup Close
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_MUTEALERT_CLOSECALL                      "notification_mutealert_closecall"

// MuteAlert Popup Skip
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_MUTEALERT_SKIPCALL                       "notification_mutealert_skipcall"

// MuteAlert Popup Volume Up
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_MUTEALERT_VOLUMEUPCALL                       "notification_mutealert_volumeupcall"



// LockAlert Popup Check
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_LOCKALERT_CHECKCALL                          "notification_lockalert_checkcall"

// PageEndAlert Popup Home
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_PAGEENDALERT_HOMECALL                        "notification_pageendalert_homecall"

// PageEndAlert Popup Replay
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_PAGEENDALERT_REPLAYCALL                      "notification_pageendalert_replaycall"

// PageEndAlert Popup Next
///////////////////////////////////////////////////////////////
#define MGTNOTIFICATION_PAGEENDALERT_NEXTCALL                        "notification_pageendalert_nextcall"

// Camera CAllBack
#define kNotification_ClickedCancelAtImagePickerView                "kNotification_Camera_ClickedCancelAtImagePickerView"
#define kNotification_ClickedUseAtImagePickerView                   "kNotification_Camera_ClickedUseAtImagePickerView"
#define kNotification_DismissImagePickerView                        "kNotification_Camera_DismissImagePickerView"
#define kNotification_SaveAfterChangeMenu                           "kNotification_Camera_ChangeMenuView"
#define kNotification_SaveCameraImagePath                           "kNotification_Camera_SaveFilePath"
namespace VIDEOSTYLE
{
    enum VIDEOSTYLE
    {
        kFLAGADDBTNNOTOUCH = 0,
        kFLAGADDBTNTOUCHSKIP = 1,
        kFLAGADDBTNTOUCHPAUSE = 2,
        
        kFLAGNOTOUCH = 100,
        kFLAGTOUCHSKIP = 101,
        kFLAGTOUCHPAUSE = 102,
        
        
    };
}
#endif // __MGTDEFINES_H__
