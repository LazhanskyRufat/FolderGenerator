package com.lazhansky.rufat.volumelevelchecker;

import android.app.Activity;
import android.content.Context;
import android.media.AudioManager;

/**
 * Created by lazhansky.rufat on 22.06.2016.
 */
public class VolumeLevelChecker
{
    public static int GetCurrentVolumeLevel(Activity activity)
    {
        AudioManager audioManager = (AudioManager) activity.getSystemService(Context.AUDIO_SERVICE);
        return audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
    }

    public static int GetMaxVolumeLevel(Activity activity)
    {
        AudioManager audioManager = (AudioManager) activity.getSystemService(Context.AUDIO_SERVICE);
        return audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
    }
}
