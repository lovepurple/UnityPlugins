/// <summary>
/// Android 到Unity的回调
/// </summary>
public interface IAndroidStringCallback 
{
    /// <summary>
    /// Android 发送消息到Unity 签名要与Android定义一致
    /// </summary>
    /// <param name="msg"></param>
    void sendMessage(string msg);
}

/// <summary>
/// Android 到 Unity的Byte[] 回调
/// </summary>
public interface IAndroidByteBufferCallback
{
    void sendMessageBuffer(byte[] buffer);
}