using System;
using UnityEngine;

public class AndroidStringCallback : AndroidJavaProxy, IUnityStringCallback
{
    private Action<string> m_onReceivedMessageCallback;

    public AndroidStringCallback(Action<string> OnReceiveMessageFromAndroid) : base("com.lovepurple.btccontroller.UnityCallback")
    {
        this.m_onReceivedMessageCallback = OnReceiveMessageFromAndroid;
    }

    public void sendMessage(string msg)
    {
        m_onReceivedMessageCallback?.Invoke(msg);
    }
}
