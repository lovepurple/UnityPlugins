using System;
using UnityEngine;

public class AndroidIntCallback : AndroidJavaProxy, IAndroidIntCallback
{
    private Action<int> OnGotIntMessageCallback;

    public AndroidIntCallback(Action<int> onGotIntMessageCallback, string javaInterface) : base(javaInterface)
    {
        this.OnGotIntMessageCallback = onGotIntMessageCallback;
    }

    public void sendMessageInt(int val)
    {
        OnGotIntMessageCallback?.Invoke(val);
    }
}
