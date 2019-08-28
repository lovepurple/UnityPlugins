using System;
using UnityEngine;

public class AndroidStringCallback : AndroidJavaProxy, IAndroidStringCallback
{
    private Action<string> m_onReceivedMessageCallback;

    public AndroidStringCallback(Action<string> OnReceiveMessageFromAndroid,string javaInterface) : base(javaInterface)
    {
        this.m_onReceivedMessageCallback = OnReceiveMessageFromAndroid;
    }

    public void sendMessage(string msg)
    {
        m_onReceivedMessageCallback?.Invoke(msg);
    }
}
