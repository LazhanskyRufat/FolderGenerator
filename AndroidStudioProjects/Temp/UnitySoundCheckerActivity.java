package com.rufat.lazhansky.soundlevelchecker;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;

public class UnitySoundCheckerActivity extends UnityPlayerActivity {

    public SoundChecker soundChecker = new SoundChecker(this);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_unity_sound_checker);
    }
}
