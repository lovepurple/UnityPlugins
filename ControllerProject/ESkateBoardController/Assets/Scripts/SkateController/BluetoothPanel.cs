using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using BLEFramework.Unity;
using EngineCore;

public class BluetoothPanel : UIPanelLogicBase
{
    private const string Device_A = "98:D3:31:F5:8B:1A";
    private const string Device_B = "20:16:06:12:28:69";

    private MaskableGraphic m_btnConnectA;
    private MaskableGraphic m_btnConnectB;
    private MaskableGraphic m_btnDisconnect;

    public BluetoothPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
        PanelName = "连接设置";
    }

    public override void OnCreate()
    {
        base.OnCreate();

        m_btnConnectA = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button");
        m_btnConnectB = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (1)");
        m_btnDisconnect = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (2)");
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        base.OnEnter(onEnterParams);

        m_btnConnectA.AddClickCallback(OnBtnConnectAClick);
        m_btnConnectB.AddClickCallback(OnBtnConnectBClick);
        m_btnDisconnect.AddClickCallback(OnBtnDisconnectClick);
    }

    private void OnBtnConnectAClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(Device_A);
    }

    private void OnBtnConnectBClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.ConnectToDevice(Device_B);
    }

    private void OnBtnDisconnectClick(GameObject btn)
    {
        BluetoothProxy.Intance.BluetoothDevice.Disconnect();
    }

    public override void OnExit()
    {
        m_btnConnectA.RemoveClickCallback(OnBtnConnectAClick);
        m_btnConnectB.RemoveClickCallback(OnBtnConnectBClick);
        m_btnDisconnect.RemoveClickCallback(OnBtnDisconnectClick);
    }
}
