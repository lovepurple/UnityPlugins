using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using BLEFramework.Unity;
using EngineCore;
using static BluetoothProxy;

public class BluetoothPanel : UIPanelLogicBase
{
    private const string Device_A = "98:D3:31:F5:8B:1A";

    private const string BLE_Device_COMPANY = "00:15:84:32:17:5A";      //BT101
    private const string BLE_Device_HOME = "11:15:85:00:4F:D3";         //BT20

    private MaskableGraphic m_btnConnectA;
    private MaskableGraphic m_btnConnectB;
    private MaskableGraphic m_btnDisconnect;

    private MaskableGraphic m_btnBLE_1;
    private MaskableGraphic m_btnBLE_2;

    public BluetoothPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
        PanelName = "连接设置";
    }

    public override void OnCreate()
    {
        base.OnCreate();

        m_btnConnectA = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button_BLC_1");
        m_btnDisconnect = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (2)");

        m_btnBLE_1 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button_BLE_1");
        m_btnBLE_2 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button_BLE_2");
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        base.OnEnter(onEnterParams);

        GlobalEvents.OnBluetoothDeviceChanged += OnBluetoothDeviceChangedHandler;

        m_btnConnectA.AddClickCallback(OnBtnConnectAClick);
        m_btnDisconnect.AddClickCallback(OnBtnDisconnectClick);
        m_btnBLE_1.AddClickCallback(OnBtnBLE1ConnectClick);
        m_btnBLE_2.AddClickCallback(OnBtnBLE2ConnectClick);
    }

    private void OnBtnConnectAClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(Device_A);
    }

    private void OnBtnBLE1ConnectClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(BLE_Device_COMPANY);
    }

    private void OnBtnBLE2ConnectClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(BLE_Device_HOME);
    }

    private void OnBluetoothDeviceChangedHandler(EBluetoothDeviceType bluetoothDeviceType)
    {
        m_btnBLE_1.SetActive(bluetoothDeviceType == EBluetoothDeviceType.BLUETOOTH_CLASSIC);
        m_btnBLE_1.SetActive(bluetoothDeviceType == EBluetoothDeviceType.BLUETOOTH_LOW_ENERGY);
        m_btnBLE_2.SetActive(bluetoothDeviceType == EBluetoothDeviceType.BLUETOOTH_LOW_ENERGY);
    }


    private void OnBtnDisconnectClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.Disconnect();
    }

    public override void OnExit()
    {
        m_btnConnectA.RemoveClickCallback(OnBtnConnectAClick);
        m_btnDisconnect.RemoveClickCallback(OnBtnDisconnectClick);
        m_btnBLE_1.RemoveClickCallback(OnBtnBLE1ConnectClick);
        m_btnBLE_2.RemoveClickCallback(OnBtnBLE2ConnectClick);

        GlobalEvents.OnBluetoothDeviceChanged -= OnBluetoothDeviceChangedHandler;
    }
}
