package com.lovepurple.btccontroller;


import android.app.Notification;
import android.content.Intent;
import android.os.Bundle;
import android.view.KeyEvent;

import com.unity3d.player.UnityPlayerActivity;


public class MainActivity extends UnityPlayerActivity {     //导入classes.jar（il2cpp mono不同）

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        BTCManager.getInstance(getApplicationContext());
    }

    @Override
    public boolean onKeyDown(int i, KeyEvent keyEvent) {
        int keyCode = keyEvent.getKeyCode();
        if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
            Intent volumeDownIntent = new Intent(BTCManager.VOLUME_KEY_EVENT);
            volumeDownIntent.putExtra("VOLUMEKEY", -1);
            getApplicationContext().sendBroadcast(volumeDownIntent);

            return false;
        } else if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            Intent volumeUpIntent = new Intent(BTCManager.VOLUME_KEY_EVENT);
            volumeUpIntent.putExtra("VOLUMEKEY", 1);
            getApplicationContext().sendBroadcast(volumeUpIntent);

            return false;
        } else
            return super.onKeyDown(i, keyEvent);
    }
}
