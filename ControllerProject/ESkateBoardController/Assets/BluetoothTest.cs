using EngineCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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

        if (GUILayout.Button("Send", GUILayout.Width(100), GUILayout.Height(80)))
        {
            string content = "abndsfjioaslkrjqou4308921u4lkjdsfkasnvkadht9823u4029134i21034u0892315u7";
            BluetoothProxy.Intance.BluetoothDevice.SendData(Encoding.UTF8.GetBytes(content).ToList());
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
