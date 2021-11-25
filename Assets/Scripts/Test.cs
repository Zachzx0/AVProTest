using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public NNNPlaylistVideo PlaylistVideo;
    public Button BtnChangeVideo;

    List<NNNVideoConfig> _configList = new List<NNNVideoConfig>()
    {
        new NNNVideoConfig(@"AVProVideoSamples/BigBuckBunny-360p30-H264.mp4", overrideTransition: true, transitionType: PlaylistMediaPlayer.Transition.Fade
            , transitionDur: 0),
                new NNNVideoConfig(@"AVProVideoSamples/Cones-2D-1080p60-H264.mp4", overrideTransition: true, transitionType: PlaylistMediaPlayer.Transition.Fade
            , transitionDur: 0),
                        new NNNVideoConfig(@"AVProVideoSamples/Cones-360Mono-4K30-H264.mp4", overrideTransition: true, transitionType: PlaylistMediaPlayer.Transition.Fade
            , transitionDur: 0),
                                new NNNVideoConfig(@"AVProVideoSamples/Cones-360Stereo-2K30-H264.mp4", overrideTransition: true, transitionType: PlaylistMediaPlayer.Transition.Fade
            , transitionDur: 0),
                                        new NNNVideoConfig(@"AVProVideoSamples/Cones-Transparent-2K60-H264.mp4", overrideTransition: true, transitionType: PlaylistMediaPlayer.Transition.Fade
            , transitionDur: 0),
                                                new NNNVideoConfig(@"AVProVideoSamples/RenderHeads-1080p30-H264.mp4", overrideTransition: true, transitionType: PlaylistMediaPlayer.Transition.Fade
            , transitionDur: 0)
    };

    int _index = 0;

    // Start is called before the first frame update
    void Start()
    {
        BtnChangeVideo.onClick.AddListener(ChangeVideo);
    }

    public void ChangeVideo()
    {
        Debug.Log("AVProTest切换视频！！！！！！！！！！！！！！！！！");
        if (PlaylistVideo.IsPlaying())
            PlaylistVideo.ResetAndPlay(false, _configList[_index]);
        else
            PlaylistVideo.Play(false, _configList[_index]);

        if (++_index >= _configList.Count)
        {
            _index = 0;
        }
    }
}
