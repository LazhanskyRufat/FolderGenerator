package com.rufat.lazhansky.soundlevelchecker;

import android.content.Context;
import android.media.AudioManager;

import com.unity3d.player.UnityPlayerActivity;

/**
 * Created by lazhansky.rufat on 21.06.2016.
 */
public class SoundChecker
{
    public UnityPlayerActivity activity;
    private AudioManager _audioManager;

    public SoundChecker(UnityPlayerActivity activity)
    {
        this.activity = activity;
        _audioManager = (AudioManager) activity.getSystemService(Context.AUDIO_SERVICE);
    }

    public int GetCurrentSoundLevel()
    {
//        int streamVolume = 0;
//
//        switch (_audioManager.getRingerMode())
//        {
//            case AudioManager.STREAM_MUSIC:
//                break;
//        }
        return _audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
    }

    public int GetMaxSoundLevel()
    {
        return _audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
    }
}
