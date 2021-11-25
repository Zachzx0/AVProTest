using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NNNVideoComponent))]
public class NNNVideoComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NNNVideoComponent video = target as NNNVideoComponent;

        if (video == null)
        {
            return;
        }

        if (GUILayout.Button("播放"))
        {
            video?.Resuem();
        }
        if (GUILayout.Button("停止"))
        {
            video?.Stop();
        }
        if (GUILayout.Button("暂停"))
        {
            video?.Pause();
        }
        if (GUILayout.Button("回看"))
        {
            video?.Replay();
        }
    }

    protected void OutputPlayVideoOver()
    {
        Debug.Log("播放完毕");
    }

    protected void OutputPlayLoopVideoOnce()
    {
        Debug.LogError("Loop视频播放完毕一次");
        Debug.Log("Loop视频播放完毕一次");
    }

    protected void OutputPlayListItemComplete(int index)
    {
        Debug.Log($"索引为{index}的视频播放完毕");
    }
}

[CustomEditor(typeof(NNNPlaylistVideo))]
public class NNNPlayListVideoPlayerEditor : NNNVideoComponentEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        NNNPlaylistVideo video = target as NNNPlaylistVideo;

        if (video == null)
        {
            return;
        }

        if (GUILayout.Button("测试播放接口"))
        {
            video.Play(false,
          new NNNVideoConfig[]
          {
             new NNNVideoConfig(@"Video/Video_Loading_transition.mp4")
             , new NNNVideoConfig(@"Video/Video_MatchConfirm.mp4")
          });
            video.SetPlayOverCallback(OutputPlayVideoOver);
            video.SetPlayOverInLoopingCallback(OutputPlayLoopVideoOnce);
            video.SetListItemPlayOverCallback(OutputPlayListItemComplete);
        }

        if (GUILayout.Button("测试播放接口2"))
        {
            video.ResetAndPlay(false,
          new NNNVideoConfig[]
          {
             new NNNVideoConfig(@"Video/Video_Mode_cup_Begain.webm", overrideTransition:true, transitionType: RenderHeads.Media.AVProVideo.PlaylistMediaPlayer.Transition.Fade
             , transitionDur:1)
             , new NNNVideoConfig(@"Video/Video_Mode_cup_Idle.webm")
          });
            video.SetPlayOverCallback(OutputPlayVideoOver);
            video.SetPlayOverInLoopingCallback(OutputPlayLoopVideoOnce);
            video.SetListItemPlayOverCallback(OutputPlayListItemComplete);
        }
    }
}

[CustomEditor(typeof(NNNSingleVideo))]
public class NBASingleVideoEditor : NNNVideoComponentEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        NNNSingleVideo video = target as NNNSingleVideo;

        if (video == null)
        {
            return;
        }


    }
}