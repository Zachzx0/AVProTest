using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNNPlaylistVideo : NNNVideoComponent
{
    public const string PlayListVideoPrefabAddr = "NNNVideo_NNNPlaylistVideo";
    const int PlayListMaxNum = 8;   //播放列表最大长度，PlaylistMediaPlayer.Playlist.item最大容量为8

    #region 回调
    public delegate void OnVideoItemPlayComplete(int index);
    OnVideoItemPlayComplete _playlistItemPlayCompleteEvent;

    /// <summary>
    /// 设置列表项播放结束回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetListItemPlayOverCallback(OnVideoItemPlayComplete callback)
    {
        _playlistItemPlayCompleteEvent = callback;
    }
    #endregion

    PlaylistMediaPlayer _mPlayListMediaPlayer = null;

    protected override void Start()
    {
        base.Start();
        _mPlayListMediaPlayer = _mainMediaPlayer as PlaylistMediaPlayer;
    }

    /// <summary>
    ///播放列表
    ///
    ///AVPro有个bug待解决。如果前置视频是透明视频，承接下一个视频的时候如果存在过渡配置，就会出现异常
    ///详见https://github.com/RenderHeads/UnityPlugin-AVProVideo/issues/912
    /// </summary>
    /// <param name="loopPlay">循环播放</param>
    /// <param name="videoCfg">播放配置</param>
    public void Play(bool loopPlay, params NNNVideoConfig[] videoCfg)
    {
        if (videoCfg != null && videoCfg.Length <= 0)
        {
            throw _argumentException;
        }

        PlaylistMediaPlayer playlist = _mainMediaPlayer as PlaylistMediaPlayer;

        playlist.Playlist.Items.Clear();
        playlist.LoopMode = loopPlay ? PlaylistMediaPlayer.PlaylistLoopMode.Loop : PlaylistMediaPlayer.PlaylistLoopMode.None;

        foreach(var videoItem in videoCfg)
        {
            MediaPlaylist.MediaItem mediaItem = new MediaPlaylist.MediaItem();
            mediaItem.loop = videoItem.Loop;
            mediaItem.mediaPath = new MediaPath(videoItem.Path, MediaPathType.RelativeToStreamingAssetsFolder);
            mediaItem.progressMode = videoItem.CutoutTime == 0 ? PlaylistMediaPlayer.ProgressMode.OnFinish : PlaylistMediaPlayer.ProgressMode.BeforeFinish;
            mediaItem.progressTimeSeconds = videoItem.CutoutTime;

            mediaItem.isOverrideTransition = videoItem.OverrideTransition;
            mediaItem.overrideTransition = videoItem.TransitionType;
            mediaItem.overrideTransitionDuration = videoItem.TransitionDur;
            mediaItem.overrideTransitionEasing = videoItem.EasingType;

            mediaItem.TransParencyMode = videoItem.TransparencyType;

            playlist.Playlist.Items.Add(mediaItem);
        }

        playlist.JumpToItem(0);

#if UNITY_IOS
        StartCoroutine(DelayPlay());

#else
        Play();
#endif
    }

    IEnumerator DelayPlay()
    {
        yield return new WaitForSeconds(1);
        Play();
    }

    public void ResetAndPlay(bool loopPlay, params NNNVideoConfig[] videoCfg)
    {
        Pause();
        Play(loopPlay, videoCfg);
    }

    public override void Stop()
    {
        if (_mPlayListMediaPlayer == null)
        {
            return;
        }

        _mPlayListMediaPlayer.CurrentPlayer.Stop();
        _mPlayListMediaPlayer.CurrentPlayer.CloseMedia();

        _mPlayListMediaPlayer.NextPlayer.Stop();
        _mPlayListMediaPlayer.NextPlayer.CloseMedia();
    }

    public override void Replay()
    {
        if (_mPlayListMediaPlayer == null)
        {
            ThrowNullError();
        }

        _mPlayListMediaPlayer.JumpToItem(0);
    }

    protected override void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.Started:
                if (_videoPlayStartEvent != null)
                {
                    _videoPlayStartEvent.Invoke();
                }
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                if (_playlistItemPlayCompleteEvent != null)
                {
                    int index = _mPlayListMediaPlayer != null ? _mPlayListMediaPlayer.PlaylistIndex : -1;

                    _playlistItemPlayCompleteEvent.Invoke(index);
                }
                break;
            case MediaPlayerEvent.EventType.PlaylistFinished:
                if (_videoPlayCompleteEvent != null)
                {
                    _videoPlayCompleteEvent.Invoke();
                }
                break;
            default:break;
        }
    }
}
