using RenderHeads.Media.AVProVideo;
using System;
using UnityEngine;

public struct NNNVideoConfig
{
    public string Path;         //path以StreamingAsset为根目录的相对路径
    public bool Loop;           //当前视频是否loop播放(如果是playList的视频最好不要在过程中loop，这样会导致playlist不会正常结束)

    //Transition相关
    public float CutoutTime;    //播放结束前切出时间，为0则播放完成时切出
    public bool OverrideTransition;
    public PlaylistMediaPlayer.Transition TransitionType;   //过渡类型
    public float TransitionDur;                             //过渡时长
    public PlaylistMediaPlayer.Easing.Preset EasingType;    //变换类型

    public TransparencyMode TransparencyType;                             //是否是透明视频 Opaque：否，Transparency:是

    public NNNVideoConfig(string path, bool loop = false, float cutoutTime = 0, bool overrideTransition = false
        , PlaylistMediaPlayer.Transition transitionType = PlaylistMediaPlayer.Transition.Black, float transitionDur = 1
        , PlaylistMediaPlayer.Easing.Preset easingType = PlaylistMediaPlayer.Easing.Preset.Linear, TransparencyMode transparency = TransparencyMode.Opaque)
    {
        Path = path;
        Loop = loop;
        CutoutTime = cutoutTime;
        OverrideTransition = overrideTransition;
        TransitionType = transitionType;
        TransitionDur = transitionDur;
        EasingType = easingType;
        TransparencyType = transparency;
    }
}

public class NNNVideoComponent : MonoBehaviour
{
    #region 回调
    public delegate void OnVideoPlayOver();
    protected OnVideoPlayOver _videoPlayCompleteEvent;
    protected OnVideoPlayOver _videoPlayFinishedOnceEvent;

    /// <summary>
    /// 设置播放结束回调（非Loop）
    /// </summary>
    /// <param name="callback"></param>
    public void SetPlayOverCallback(OnVideoPlayOver callback)
    {
        _videoPlayCompleteEvent = callback;
    }

    /// <summary>
    /// 设置播放结束回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetPlayOverInLoopingCallback(OnVideoPlayOver callback)
    {
        _videoPlayFinishedOnceEvent = callback;
    }

    public delegate void OnVideoPlay();
    protected OnVideoPlay _videoPlayStartEvent;

    /// <summary>
    /// 设置开始播放回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetPlayStartCallback(OnVideoPlay callback)
    {
        _videoPlayStartEvent = callback;
    }
    #endregion

    #region flag
    private bool _triggerPlayVideoCompleteOnce = false;
    #endregion

    [SerializeField]
    [Header("主解码器")]
    protected MediaPlayer _mainMediaPlayer;

    [SerializeField]
    [Header("主渲染器")]
    protected DisplayUGUI _mainOutput;

    protected NullReferenceException _nullReferenceError;

    protected ArgumentException _argumentException;

    protected virtual void Start()
    {
        if (_nullReferenceError == null)
        {
            _nullReferenceError = new NullReferenceException($"[视频组件]：主播放器未引用，请检查物体[{this.gameObject.name}]主播放器配置");
        }

        if (_argumentException == null)
        {
            _argumentException = new ArgumentException($"[视频组件]：参数异常，请检查物体[{this.gameObject.name}]主播放器调用参数");
        }

        Debug.Assert(_mainMediaPlayer != null, $"[视频组件]：主播放器未引用，请检查物体[{this.gameObject.name}]主播放器配置");

        if (_mainMediaPlayer != null)
        {
            _mainMediaPlayer.Events.AddListener(OnVideoEvent);
        }
    }

    protected virtual void OnDestroy()
    {
        _mainMediaPlayer.Events.RemoveListener(OnVideoEvent);
        Stop();
    }

    public float GetProgress()
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        double totalTime = 0 , curTime = 0;
        if (_mainMediaPlayer.Info != null)
        {
            totalTime = _mainMediaPlayer.Info.GetDuration();
        }
        if (_mainMediaPlayer.Control != null)
        {
            curTime = _mainMediaPlayer.Control.GetCurrentTime();
        }
        float percent = (float)(curTime / totalTime);
        return Mathf.Clamp(percent, 0.0f, 1.0f);
    }

    public float GetVolume()
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        if (_mainMediaPlayer.Control == null)
        {
            return 0;
        }
        return _mainMediaPlayer.Control.GetVolume();
    }

    public bool IsPlaying()
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        if (_mainMediaPlayer.Control == null)
        {
            return false;
        }
        return _mainMediaPlayer.Control.IsPlaying();
    }

    public void Pause()
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        _mainMediaPlayer.Pause();
    }

    public virtual void Replay()
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }

        _mainMediaPlayer.Rewind(false);
        _mainMediaPlayer.Play();
    }

    public void SetMute(bool mute)
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        if (_mainMediaPlayer.Control == null)
        {
            return;
        }
        _mainMediaPlayer.Control.MuteAudio(mute);
    }

    public void SetVolume(float volume)
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        if (_mainMediaPlayer.Control == null)
        {
            return;
        }
        _mainMediaPlayer.Control.SetVolume(volume);
    }

    public virtual void Stop()
    {
        if (_mainMediaPlayer == null)
        {
            return;
        }
        _mainMediaPlayer.Stop();

        _mainMediaPlayer.CloseMedia();
    }

    protected void Play()
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        _mainMediaPlayer.Play();
    }

    public void Resuem()
    {
        Play();
    }

    public bool CanPlay()
    {
        if (_mainMediaPlayer == null)
        {
            ThrowNullError();
        }
        if (_mainMediaPlayer.Control == null)
        {
            return false;
        }
        return _mainMediaPlayer.Control.CanPlay();
    }

    public void SetAspectRatio(ScaleMode scaleMode)
    {
        _mainOutput.ScaleMode = scaleMode;
    }

    protected virtual void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {}

    protected void ThrowNullError()
    {
        throw _nullReferenceError;
    }

    private void Update()
    {
        if (_mainMediaPlayer.IsPlaying())
        {
            if (_mainMediaPlayer.GetCurrentPlayedTime() == 0)
            {
                _triggerPlayVideoCompleteOnce = false;
            }
        }

        if (!_triggerPlayVideoCompleteOnce && _mainMediaPlayer.IsCurrentVideoPlayOver())
        {
            _triggerPlayVideoCompleteOnce = true;
            if (_videoPlayFinishedOnceEvent != null)
            {
                _videoPlayFinishedOnceEvent.Invoke();
            }
        }
    }
}
