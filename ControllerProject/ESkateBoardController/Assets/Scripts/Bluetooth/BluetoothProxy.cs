using UnityEngine;

public class BluetoothProxy
{
    private static BluetoothProxy m_instance;

    private IBluetoothDevice m_device;

    private BluetoothProxy() { }

    public void InitializeBluetoothProxy()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                m_device = new AndroidBluetoothClassicDevice();
                break;
            default:
                throw new System.Exception($"Platform{Application.platform.ToString()} BluetoothDevice Not Implememt");

        }

        if (m_device != null)
            m_device.InitializeBluetoothDevice();
    }

    public void Tick()
    {
        if (m_device != null)
            m_device.Tick();
    }



    public IBluetoothDevice BluetoothDevice => this.m_device;

    public static BluetoothProxy Intance => m_instance ?? (m_instance = new BluetoothProxy());
}
