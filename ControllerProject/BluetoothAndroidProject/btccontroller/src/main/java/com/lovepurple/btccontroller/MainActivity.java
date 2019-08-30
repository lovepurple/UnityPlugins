package com.lovepurple.btccontroller;


import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;


public class MainActivity extends UnityPlayerActivity {     //导入classes.jar（il2cpp mono不同）

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        BTCManager.getInstance(getApplicationContext());
    }
}
