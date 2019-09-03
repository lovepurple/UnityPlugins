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

    private const int GearCount = 5; //档位数量

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
    private Button m_btnSetLowSpeed = null;
    private Button m_btnSetHighSpeed = null;

    private Button m_btnTestMessage = null;

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

        m_btnSetLowSpeed = transform.Find("GameObject/Button (14)").GetComponent<Button>();
        m_btnSetLowSpeed.AddClickCallback(btn => SetSpeed(0.33f));

        m_btnSetHighSpeed = transform.Find("GameObject/Button (15)").GetComponent<Button>();
        m_btnSetHighSpeed.AddClickCallback(btn => SetSpeed(0.66f));

        m_btnTestMessage = transform.Find("GameObject/Button (16)").GetComponent<Button>();
        m_btnTestMessage.AddClickCallback(OnTestMessageClick);


        ControllerJoyStick.onMove.AddListener(OnJoyStickMove);
        ControllerJoyStick.onMoveSpeed.AddListener(OnJoyStickMoveSpeed);
        ControllerJoyStick.onMoveStart.AddListener(OnJoyStickMoveStart);
        ControllerJoyStick.onMoveEnd.AddListener(OnJoyStickMoveEnd);

        BluetoothProxy.Intance.InitializeBluetoothProxy();
        m_skateMessageHandler = new SkateMessageHandler(BluetoothProxy.Intance.BluetoothDevice);

        BluetoothProxy.Intance.BluetoothDevice.OnErrorEvent += (errorMsg) =>
        {
            //m_receiveMessage.text = errorMsg;
            Debug.Log(errorMsg);
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
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_INITIALIZE);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnBtnBluetoothStatusClick(GameObject btn)
    {
        this.m_receiveMessage.text = BluetoothProxy.Intance.BluetoothDevice.GetBluetoothDeviceStatus().ToString();
    }

    private void OnBtnMotorMinSpeedClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnBtnMotorNormalStartClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_NORMAL_START);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }


    private void OnBtnSpeedupClick(GameObject btn)
    {
    }

    private void OnBtnSpeedDownClick(GameObject btn)
    {

    }

    private void OnBtnStopClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnBtnMaxSpeedClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MAX_POWER);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnBtnPowerOff(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_POWEROFF);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnBtnGetCurrentSpeedClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_GET_SPEED);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);

    }

    private void OnTestMessageClick(GameObject btn)
    {
        SetSpeed(0.1f);
        SetSpeed(0.2f);
        SetSpeed(0.3f);
        SetSpeed(0.4f);
        SetSpeed(0.5f);
    }



    private void OnGetMotorSpeedResponse(object data)
    {
        char[] speed = (char[])data;

        this.m_receiveMessage.text = $"当前速度：{DigitUtility.GetUInt32(speed) / 999.0f}";
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
            //0.1为一档
            //四舍五入
            float gear = Mathf.RoundToInt(delta.y * GearCount) * 1.0f / GearCount;        //Shader中常用方法

            SetSpeed(gear);
        }

    }

    private void SetSpeed(float percentage01)
    {
        int speedThoudsand = (int)MathUtil.Remap(percentage01, 0, 1, 0, 999);

        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_DRIVE);

        messageBuffer.AddRange(Encoding.ASCII.GetBytes(speedThoudsand.ToString()));

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnJoyStickMoveSpeed(Vector2 delta)
    {
    }

    private void OnJoyStickMoveEnd()
    {
    }

    private void OnJoyStickMoveStart()
    {
    }

}
