package com.rufat.lazhansky.soundlevelchecker;

import android.os.Bundle;
import com.unity3d.player.UnityPlayerActivity;

public class SoundLevelCheckerActivity extends UnityPlayerActivity
{
    public SoundChecker soundChecker = new SoundChecker(this);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sound_level_checker);
    }

    public void debugLogTest()
    {

    }
}
