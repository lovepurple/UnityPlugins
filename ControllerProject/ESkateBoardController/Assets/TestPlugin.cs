using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestPlugin : MonoBehaviour
{
    private static float DEVICE_RESOLUTION_SCALAR = Mathf.Max((float)Screen.width / 1920, (float)Screen.height / 1080);

    public static float DEFAULT_GUIELEMENT_HEIGHT = 30.0f * DEVICE_RESOLUTION_SCALAR;
    public static float DEFAULT_GUI_BUTTON_WIDTH = 100 * DEVICE_RESOLUTION_SCALAR;

    public string MAC = "98:D3:31:F5:8B:1A";
    // Start is called before the first frame update
    void Start()
    {
        BluetoothProxy.Intance.InitializeBluetoothProxy();
        BluetoothProxy.Intance.BluetoothDevice.OnReceiveDataEvent += BluetoothDevice_OnReceiveDataEvent;
    }

    private void BluetoothDevice_OnReceiveDataEvent(byte[] obj)
    {
        string recvBuffer = Encoding.ASCII.GetString(obj);
        Debug.Log("recv :" + recvBuffer);
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnGUI()
    {
        if (GUILayout.Button("GetPariedDevices", GUILayout.Width(DEFAULT_GUI_BUTTON_WIDTH), GUILayout.Height(DEFAULT_GUIELEMENT_HEIGHT)))
        {
            BluetoothProxy.Intance.BluetoothDevice.GetPariedDevices(pariedDevices =>
            {
                Debug.Log(pariedDevices);
            });
        }

        if (GUILayout.Button("Connect to Device", GUILayout.Width(DEFAULT_GUI_BUTTON_WIDTH), GUILayout.Height(DEFAULT_GUIELEMENT_HEIGHT)))
        {
            BluetoothProxy.Intance.BluetoothDevice.OnConnectedEvent += () =>
            {
                Debug.Log("succeed");
            };
            BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(MAC);
        }

        if (GUILayout.Button("Disconnect", GUILayout.Width(DEFAULT_GUI_BUTTON_WIDTH), GUILayout.Height(DEFAULT_GUIELEMENT_HEIGHT)))
        {
            BluetoothProxy.Intance.BluetoothDevice.Disconnect();
        }

        if (GUILayout.Button("SendData", GUILayout.Width(DEFAULT_GUI_BUTTON_WIDTH), GUILayout.Height(DEFAULT_GUIELEMENT_HEIGHT)))
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes("HelloWorld");
            BluetoothProxy.Intance.BluetoothDevice.SendData(data);
        }


    }
}
