using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl
{
    public enum CallRegexConditionTypes
    {
        all,
        any,
        xor
    }

    public enum DialPlanPriorities
    {
        First = 4,
        High = 3,
        Normal = 2,
        Low = 1
    }

    public enum CallActionTypes
    {
        Ring_Ready,
        Answer,
        Sleep,
        Hangup,
        Attended_Transfer,
        Break,
        Acl,
        CheckAcl,
        DB,
        Deflect,
        Echo,
        Enum,
        Eval,
        Event,
        Execute_Extension,
        Export,
        Info,
        MkDir,
        Presence,
        Privacy,
        ReadDigits,
        Redirect,
        Respond,
        SendDisplay,
        ToneDetect,
        StopToneDetect,
        CurrentTime,
        System,
        WaitForSilence,
        Chat,
        Log,
        Session_LogLevel,
        FIFO_In,
        FIFO_Out,
        SetJitterBuffer,
        Set_Audio_Level,
        Set_Global,
        Set_Channel_Name,
        Set_User,
        Set_Zombie_Exec,
        Set_Verbose_Events,
        BridgeToExtension,
        BridgeToMultipleExtensions,
        BridgeOutGateway,
        Voicemail,
        Bridge_Export,
        EavesDrop,
        Intercept,
        IVR,
        Soft_Hold,
        Three_Way,
        Transfer,
        Transfer_To_Context,
        Transfer_To_Call_Extension,
        Set,
        UnSet,
        Conference_Set_Auto_Outcall,
        JoinConference,
        KickFromConference,
        PlayAudioFile,
        PlayAudioFileEndlessly,
        PlayAndGetDigits,
        GenTones,
        PlayAndDetectSpeech,
        Play_FSV,
        Record,
        Record_Session,
        Stop_Record_Session,
        Say,
        Schedule_Broadcast,
        Speak,
        Stop_Displace,
        Bind_Digit_Action,
        Clear_Digit_Aciton,
        Digit_Action_Set_Realm,
        Bind_Meta_App,
        UnBind_Meta_App,
        Detect_Speech,
        Detect_Speech_Grammar,
        Detect_Speech_Grammar_On,
        Detect_Speech_Grammar_Off,
        Detect_Speech_Grammar_OffAll,
        Detect_Speech_No_Grammar,
        Detect_Speech_Param,
        Detect_Speech_Pause,
        Detect_Speech_Resume,
        Detect_Speech_Start_Input_Timers,
        Detect_Speech_Stop,
        Flush_DTMF,
        Queue_DTMF,
        Send_DTMF,
        Start_DTMF,
        Stop_DTMF,
        Start_DTMF_Generate,
        Stop_DTMF_Generate,
        Javascript,
        Lua,
        Park,
        Valet_Park,
        Schedule_Hangup,
        Schedule_Transfer
    }

    public enum BindDigitTargetLegs
    {
        aleg,
        peer,
        both
    }

    public enum BindDigitsEventLegs
    {
        peer,
        self,
        both
    }

    public enum BroadcastLegs
    {
        aleg,
        bleg,
        both
    }

    public enum CallPrivacyTypes
    {
        No,
        Yes,
        Name,
        Full,
        Number
    }

    public enum DTMFQueueDelays
    {
        Half_Second = 'w',
        Full_Second = 'W',
        None
    }

    public enum LogLevels
    {
        Console = 0,
        Alert = 1,
        Crit = 1,
        Err = 3,
        Warning = 4,
        Notice = 5,
        Info = 6,
        Debug = 7
    }

    public enum MetaAppLegTypes
    {
        a,
        b,
        ab
    }

    public enum MetaAppFlags
    {
        Respond_A = 'a',
        Respond_B = 'b',
        Respond_Opposite = 'o',
        Respond_Same = 's',
        Execute_Inline = 'i',
        OneTimeUse = '1'
    }

    public enum SayTypes
    {
        NUMBER,
        ITEMS,
        PERSONS,
        MESSAGES,
        CURRENCY,
        TIME_MEASUREMENT,
        CURRENT_DATE,
        CURRENT_TIME,
        CURRENT_DATE_TIME,
        TELEPHONE_NUMBER,
        TELEPHONE_EXTENSION,
        URL,
        IP_ADDRESS,
        EMAIL_ADDRESS,
        POSTAL_ADDRESS,
        ACCOUNT_NUMBER,
        NAME_SPELLED,
        NAME_PHONETIC,
        SHORT_DATE_TIME
    }

    public enum SayMethods
    {
        N_A,
        PRONOUNCED,
        ITERATED,
        COUNTED
    }

    public enum SayGenders
    {
        FEMININE,
        MASCULINE,
        NEUTER
    }
}
