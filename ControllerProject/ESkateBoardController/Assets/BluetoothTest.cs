using EngineCore.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothTest : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Get PairDe ", GUILayout.Width(100), GUILayout.Height(80)))
        {
            var va = BluetoothProxy.Intance.BluetoothDevice.GetPariedDevices();
            Debug.Log(va);
        }

        if (GUILayout.Button("Search", GUILayout.Width(100), GUILayout.Height(80)))
        {
            BluetoothProxy.Intance.BluetoothDevice.SearchDevices();
        }


    }

    private Queue<byte[]> m_data = new Queue<byte[]>();

    private void Update()
    {
        while (m_data.Count > 0)
        {
            m_data.Dequeue();
        }

    }
}
