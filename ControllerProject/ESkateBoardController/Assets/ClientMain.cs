using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientMain : MonoBehaviour
{
    public string MAC = "98:D3:31:F5:8B:1A";


    private Text m_receiveMessage = null;

    private Button m_btnConnect = null;
    private Button m_btnDisconnect = null;
    private Button m_btnCurrentStatus = null;
    private Button m_btnMotorInit = null;
    private Button m_btnMotorMin = null;
    private Button m_btnMotorNormalStart = null;
    private Button m_btnSpeedup = null;
    private Button m_btnSpeedDown = null;
    private Button m_btnStop = null;
    private Button m_btnMaxSpeed = null;
    private Button m_btnPowerOff = null;

    private void Start()
    {
        m_receiveMessage = transform.Find("Text").GetComponent<Text>();

        m_btnConnect = transform.Find("GameObject/Button").GetComponent<Button>();
        m_btnConnect.AddClickCallback(OnBtnConnectClick);

        m_btnDisconnect = transform.Find("GameObject/Button (1)").GetComponent<Button>();
        m_btnDisconnect.AddClickCallback(OnBtnDisconnectClick);

        m_btnCurrentStatus = transform.Find("GameObject/Button (2)").GetComponent<Button>();
        m_btnCurrentStatus.AddClickCallback(OnBtnBluetoothStatusClick);

        m_btnMotorInit = transform.Find("GameObject/Button (3)").GetComponent<Button>();
        m_btnMotorInit.AddClickCallback(OnBtnMotorInitClick);

        m_btnMotorMin = transform.Find("GameObject/Button (4)").GetComponent<Button>();
        m_btnMotorMin.AddClickCallback(OnBtnMotorMinSpeedClick);

        m_btnMotorNormalStart = transform.Find("GameObject/Button (5)").GetComponent<Button>();
        m_btnMotorNormalStart.AddClickCallback(OnBtnMotorNormalStartClick);

        m_btnSpeedup = transform.Find("GameObject/Button (6)").GetComponent<Button>();
        m_btnSpeedup.AddClickCallback(OnBtnSpeedupClick);

        m_btnSpeedDown = transform.Find("GameObject/Button (7)").GetComponent<Button>();
        m_btnSpeedDown.AddClickCallback(OnBtnSpeedDownClick);

        m_btnStop = transform.Find("GameObject/Button (8)").GetComponent<Button>();
        m_btnStop.AddClickCallback(OnBtnStopClick);

        m_btnMaxSpeed = transform.Find("GameObject/Button (9)").GetComponent<Button>();
        m_btnMaxSpeed.AddClickCallback(OnBtnMaxSpeedClick);

        m_btnPowerOff = transform.Find("GameObject/Button (10)").GetComponent<Button>();
        m_btnPowerOff.AddClickCallback(OnBtnPowerOff);


        BluetoothProxy.Intance.InitializeBluetoothProxy();
    }

    private void OnBtnConnectClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(MAC);
    }

    private void OnBtnDisconnectClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.Disconnect();
    }

    private void OnBtnMotorInitClick(GameObject btn)
    {
        byte[] buffer = new byte[2];
        
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_INITIALIZE;
        buffer[1] = (byte)'\n';

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }

    private void OnBtnBluetoothStatusClick(GameObject btn)
    {
        this.m_receiveMessage.text = BluetoothProxy.Intance.BluetoothDevice.GetBluetoothDeviceStatus().ToString();
    }

    private void OnBtnMotorMinSpeedClick(GameObject btn)
    {
        byte[] buffer = new byte[1];
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER;

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }

    private void OnBtnMotorNormalStartClick(GameObject btn)
    {
        byte[] buffer = new byte[1];
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_NORMAL_START;

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }
    

    private void OnBtnSpeedupClick(GameObject btn)
    {

    }

    private void OnBtnSpeedDownClick(GameObject btn)
    {

    }

    private void OnBtnStopClick(GameObject btn)
    {

    }

    private void OnBtnMaxSpeedClick(GameObject btn)
    {

    }

    private void OnBtnPowerOff(GameObject btn)
    {
        byte[] buffer = new byte[1];
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_POWEROFF;

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }
}
