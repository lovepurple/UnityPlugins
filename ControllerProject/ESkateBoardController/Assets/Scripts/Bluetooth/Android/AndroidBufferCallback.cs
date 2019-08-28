using System;
using UnityEngine;

public class AndroidBufferCallback : AndroidJavaProxy, IAndroidByteBufferCallback
{
    private Action<byte[]> OnGotBufferFromAndroidCallback;

    public AndroidBufferCallback(Action<byte[]> onGotBufferFromAndroidCallback, string javaInterface) : base(javaInterface)
    {
        this.OnGotBufferFromAndroidCallback = onGotBufferFromAndroidCallback;
    }
    public void sendMessageBuffer(byte[] buffer)
    {
        OnGotBufferFromAndroidCallback?.Invoke(buffer);
    }
}
