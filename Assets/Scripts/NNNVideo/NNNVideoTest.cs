using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NNNVideoTest : MonoBehaviour
{
    public NNNPlaylistVideo PlaylistVideo;
    public NNNSingleVideo SingleVideo;

    Button _btn;

    // Start is called before the first frame update
    void Start()
    {
        _btn = this.GetComponent<Button>();
        _btn.onClick.AddListener(OnBtnClick);
    }

    void OnBtnClick()
    {
        if (PlaylistVideo != null)
        {
            PlaylistVideo.Play(true, new NNNVideoConfig(@"Video/output-lr.mp4", overrideTransition: true, transitionType: RenderHeads.Media.AVProVideo.PlaylistMediaPlayer.Transition.Fade
            , transitionDur: 1, transparency: RenderHeads.Media.AVProVideo.TransparencyMode.Transparent)
              , new NNNVideoConfig(@"Video/Video_NNNtraining_Enter.mp4"));
        }

        if (SingleVideo != null)
        {
            SingleVideo.Play(@"Video/output-lr.mp4", true, RenderHeads.Media.AVProVideo.TransparencyMode.Transparent);
        }
    }
}
