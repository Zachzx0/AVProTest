using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class NNNSingleVideo : NNNVideoComponent
{
    public const string PlaySingleVideoPrefabAddr = "NNNVideo_NNNSigleVideo";

    public void Play(string path, bool isLoop, TransparencyMode transparency = TransparencyMode.Opaque, bool autoPlay = true)
    {
        _mainMediaPlayer.OpenMedia(MediaPathType.RelativeToStreamingAssetsFolder, path, autoPlay, transparency);

        _mainMediaPlayer.Control.SetLooping(isLoop);
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
                if (_videoPlayCompleteEvent != null)
                {
                    _videoPlayCompleteEvent.Invoke();
                }
                break;
            default:
                break;
        }
    }
}
