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
        if (GUILayout.Button("add "))
        {
            List<byte> list1 = new List<byte>() { 6, 48, 120 };

            List<byte> list2 = DigitUtility.GetFixedLengthBufferList(list1, 10, 48);

            Debug.Log(list2);
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
