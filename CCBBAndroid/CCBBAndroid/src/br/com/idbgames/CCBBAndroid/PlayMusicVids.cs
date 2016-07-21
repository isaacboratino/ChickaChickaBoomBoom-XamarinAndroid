using Android.App;
using Android.Content.Res;
using Android.Media;
using Android.Util;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace br.com.idbgames.CCBBAndroid
{
    public class PlayMusicVids
    {
        protected SoundPool sound = null;
        protected MediaPlayer player = null;
        protected VideoView mVideoView = null;
        protected Dictionary<string, int> aMap = new Dictionary<string, int>();
        protected Activity activityContext;

        public PlayMusicVids(Activity aContext)
        {
            activityContext = aContext;
        }

        public PlayMusicVids(Activity aContext, string[] nameSoundsArray, string directory, string type)
        {
            activityContext = aContext;

            LoadSounds(nameSoundsArray, directory, type);
        }

        public void LoadSounds(string[] nameSoundsArray, string directory, string type)
        {

            try
            {

                aMap.Clear();

                sound = null;
                sound = new SoundPool(1, Stream.Music, 0);

                // Load all letters
                for (int i = 0; i <= nameSoundsArray.Length; i++)
                {
                    string soundName = nameSoundsArray[i];
                    //string file = "sounds/gp/" + soundName + ".3gp";
                    string file = directory + "/" + soundName + "." + type;
                    AssetFileDescriptor soundFile = activityContext.Assets.OpenFd(file);
                    aMap.Add(soundName, sound.Load(soundFile, 1));
                    soundFile.Close();
                }

            }
            catch (Exception e)
            {
                Log.Info("ERROR", e.Message);
            }
        }

        public void playMusic(string directory, string soundName, string type)
        {
            playMusic(directory + "/" + soundName + "." + type);
        }

        public void playMusic(string soundNameWithDirectoryAndType)
        {

            try
            {

                this.stopAll();

                //string file = "sounds/gp/" + soundName + ".3gp";
                string file = soundNameWithDirectoryAndType;

                AssetFileDescriptor afd = activityContext.Assets.OpenFd(file);
                player = new MediaPlayer();
                player.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                player.Prepare();
                player.Start();

            }
            catch (Exception e)
            {
                Log.Info("ERROR", e.Message);
            }
        }

        public void playBeep(string soundName)
        {

            try
            {

                this.stopAll();

                if (player != null)
                    if (player.IsPlaying)
                        player.Stop();

                if (sound == null)
                    sound = new SoundPool(1, Stream.Music, 0);

                sound.Play(aMap[soundName], 1f, 1f, 0, 0, 1f);

            }
            catch (Exception e)
            {
                Log.Info("ERROR", e.Message);
            }
        }

        public void playVideo(string fullResourceValidName, int videoName, int videoView)
        {

            try
            {

                stopAll();

                //	Displays a video file.        
                //string uriPath = "android.resource://com.idbgm.abchebraico/"+soundName;
                string uriPath = fullResourceValidName + "/" + videoName;
                Android.Net.Uri uri = Android.Net.Uri.Parse(uriPath);

                if (mVideoView == null)
                    mVideoView = (VideoView)activityContext.FindViewById(videoView);

                mVideoView.SetVideoURI(uri);
                mVideoView.BringToFront();

                MediaController mc = new MediaController(activityContext);
                mc.SetAnchorView(mVideoView);
                mc.SetMediaPlayer(mVideoView);

                mVideoView.SetMediaController(mc);
                mVideoView.Visibility = Android.Views.ViewStates.Visible;
                mVideoView.Start();

            }
            catch (Exception e) { }
        }

        public void playVideo(string fullResourceValidName, int videoName)
        {
            playVideo(fullResourceValidName, videoName, mVideoView.Id);
        }

        public void stopAll()
        {
            // stop audio
            if (player == null)
                player = new MediaPlayer();

            if (player.IsPlaying)
                player.Stop();

            // stop video
            if (mVideoView != null)
            {
                mVideoView.StopPlayback();
                mVideoView.Visibility = Android.Views.ViewStates.Gone;
            }

        }
    }
}