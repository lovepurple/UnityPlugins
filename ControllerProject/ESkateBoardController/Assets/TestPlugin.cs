using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlugin : MonoBehaviour
{
    private static float DEVICE_RESOLUTION_SCALAR = Mathf.Max((float)Screen.width / 1920, (float)Screen.height / 1080);

    public static float DEFAULT_GUIELEMENT_HEIGHT = 30.0f * DEVICE_RESOLUTION_SCALAR;
    public static float DEFAULT_GUI_BUTTON_WIDTH = 100 * DEVICE_RESOLUTION_SCALAR;

    // Start is called before the first frame update
    void Start()
    {
        BluetoothProxy.Intance.InitializeBluetoothProxy();
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnGUI()
    {
        if (GUILayout.Button("IsEnable bluetooth", GUILayout.Width(DEFAULT_GUI_BUTTON_WIDTH), GUILayout.Height(DEFAULT_GUIELEMENT_HEIGHT)))
        {
            bool isEnable = BluetoothProxy.Intance.BluetoothDevice.IsBluetoothEnabled();
            Debug.Log(isEnable);
        }

        if (GUILayout.Button("get other", GUILayout.Width(DEFAULT_GUI_BUTTON_WIDTH), GUILayout.Height(DEFAULT_GUIELEMENT_HEIGHT)))
        {
            string name = BluetoothProxy.Intance.BluetoothDevice.GetConnectedDeviceName();
            Debug.Log(name);
        }
    }
}
