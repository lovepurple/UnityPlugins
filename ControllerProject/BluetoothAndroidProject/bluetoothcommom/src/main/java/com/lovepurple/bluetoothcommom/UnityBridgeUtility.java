package com.lovepurple.bluetoothcommom;

import com.google.gson.Gson;

public final class UnityBridgeUtility {

    /**
     * 获取发送到Unity的Log
     *
     * @param logType
     * @param logContent
     */
    public static String convertToUnityLog(int logType, String logContent) {
        String sendLogContentToUnity = "";
        switch (logType) {
            case UnityMessageDefine.SEND_LOG:
            case UnityMessageDefine.SEND_ERROR:
                UnityMessageAdapter messageAdapter = new UnityMessageAdapter();
                messageAdapter.mMessageID = logType;
                messageAdapter.mMessageBody = logContent;

                Gson gson = new Gson();
                sendLogContentToUnity = gson.toJson(messageAdapter);
                break;
        }

        return sendLogContentToUnity;
    }
}
