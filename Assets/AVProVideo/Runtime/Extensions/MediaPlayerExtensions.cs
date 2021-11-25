using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
    public partial class MediaPlayer
    {
        public virtual bool IsPlaying()
        {
            if (_controlInterface != null)
            {
                return _controlInterface.IsPlaying();
            }
            return false;
        }

        public virtual double GetCurrentPlayedTime()
        {
            if (_controlInterface == null)
            {
                return -1;
            }
            return _controlInterface.GetCurrentTime();
        }

        /// <summary>
        /// 当前视频是否播放结束
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCurrentVideoPlayOver()
        {
            if (_controlInterface == null || _baseMediaPlayer == null)
            {
                return false;
            }
            return _controlInterface.GetCurrentTime() > 0 && _controlInterface.GetCurrentTime() >= _baseMediaPlayer.GetDuration();
        }

        public bool OpenMedia(MediaPathType pathType, string path, bool autoPlay = true, TransparencyMode transparency = TransparencyMode.Opaque)
        {
            _mediaSource = MediaSource.Path;
            _mediaPath.Path = path;
            _mediaPath.PathType = pathType;

            _fallbackMediaHints.transparency = transparency;
            _fallbackMediaHints.alphaPacking = AlphaPacking.LeftRight;  //默认的AplhaPacking方式是从左到右
            _autoPlayOnStart = autoPlay;

            if (_controlInterface == null)
            {
                //_autoOpen = false;		 // If OpenVideoFromFile() is called before Start() then set _autoOpen to false so that it doesn't load the video a second time during Start()
                Initialise();
            }

            return OpenMedia();
        }

        public bool OpenMedia(MediaPath path, bool autoPlay = true, TransparencyMode transparency = TransparencyMode.Opaque)
        {
            return OpenMedia(path.PathType, path.Path, autoPlay, transparency);
        }


    }
}
