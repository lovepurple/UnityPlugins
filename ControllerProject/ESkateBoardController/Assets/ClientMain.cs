using EngineCore;
using EngineCore.Utility;
using GOGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ClientMain : MonoBehaviour
{
    private const string Device_A = "98:D3:31:F5:8B:1A";
    private const string Device_B = "20:16:06:12:28:69";

    private Text m_receiveMessage = null;

    private Button m_btnConnectA = null;
    private Button m_btnConnectB = null;
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
    private Button m_btnGetCurrentSpeed = null;

    private SkateMessageHandler m_skateMessageHandler = null;

    public ETCJoystick ControllerJoyStick;

    private void Start()
    {
        m_receiveMessage = transform.Find("Text").GetComponent<Text>();

        m_btnConnectA = transform.Find("GameObject/Button (12)").GetComponent<Button>();
        m_btnConnectA.AddClickCallback(OnBtnConnectAClick);

        m_btnConnectB = transform.Find("GameObject/Button (13)").GetComponent<Button>();
        m_btnConnectB.AddClickCallback(OnBtnConnectBClick);

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

        m_btnGetCurrentSpeed = transform.Find("GameObject/Button (11)").GetComponent<Button>();
        m_btnGetCurrentSpeed.AddClickCallback(OnBtnGetCurrentSpeedClick);

        ControllerJoyStick.onMove.AddListener(OnJoyStickMove);
        ControllerJoyStick.onMoveSpeed.AddListener(OnJoyStickMoveSpeed);
        ControllerJoyStick.onMoveStart.AddListener(OnJoyStickMoveStart);
        ControllerJoyStick.onMoveEnd.AddListener(OnJoyStickMoveEnd);

        BluetoothProxy.Intance.InitializeBluetoothProxy();
        m_skateMessageHandler = new SkateMessageHandler(BluetoothProxy.Intance.BluetoothDevice);

        BluetoothProxy.Intance.BluetoothDevice.OnErrorEvent += (errorMsg) =>
        {
            m_receiveMessage.text = errorMsg;
        };

        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_SPEED, OnGetMotorSpeedResponse);




    }

    private void OnBtnConnectBClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(Device_B);
    }

    private void OnBtnConnectAClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(Device_A);
    }


    private void OnBtnDisconnectClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.Disconnect();
    }

    private void OnBtnMotorInitClick(GameObject btn)
    {
        byte[] buffer = new byte[1];

        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_INITIALIZE;
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
        byte[] buffer = new byte[1];
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER;

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }

    private void OnBtnMaxSpeedClick(GameObject btn)
    {
        byte[] buffer = new byte[1];
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_CORRECT_MAX_POWER;

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }

    private void OnBtnPowerOff(GameObject btn)
    {
        byte[] buffer = new byte[1];
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_POWEROFF;

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }

    private void OnBtnGetCurrentSpeedClick(GameObject btn)
    {
        byte[] buffer = new byte[1];
        buffer[0] = (byte)MessageDefine.E_C2D_MOTOR_GET_SPEED;

        BluetoothProxy.Intance.BluetoothDevice.SendData(buffer);
    }



    private void OnGetMotorSpeedResponse(object data)
    {
        char[] speed = (char[])data;

        this.m_receiveMessage.text = $"当前速度：{DigitUtility.GetUInt32(speed) / 1000.0f}";
    }

    private void Update()
    {
        BluetoothProxy.Intance.Tick();
    }


    /****************************************/
    private void OnJoyStickMove(Vector2 delta)
    {
        //todo:之后考虑差值
        //char buffer[]
        if (delta.y >= 0)
        {
            int speedThoudsand = (int)MathUtil.Remap(1, 0, 1, 0, 999);


            byte[] speedBuffer = new byte[4];
            speedBuffer[0] = (byte)MessageDefine.E_C2D_MOTOR_DRIVE;

            char[] a = speedThoudsand.ToString().ToCharArray();
            byte[] buffer = Encoding.ASCII.GetBytes(a);

            buffer.CopyTo(speedBuffer, 1);

            BluetoothProxy.Intance.BluetoothDevice.SendData(speedBuffer);
        }

    }

    private void OnJoyStickMoveSpeed(Vector2 delta)
    {
        Debug.Log("onmove speed : " + delta.ToString());
    }

    private void OnJoyStickMoveEnd()
    {
        Debug.Log("onmove end ");
    }

    private void OnJoyStickMoveStart()
    {
        Debug.Log("onmove start ");
    }
}
