using EngineCore;
using UnityEngine;
using UnityEngine.UI;

public class SignalPanel : UIPanelLogicBase
{
    private Image m_imgOnline;
    private Image m_imgOffline;

    public SignalPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        this.m_imgOffline = m_panelRootObject.GetComponent<Image>("img_offline");
        this.m_imgOnline = m_panelRootObject.GetComponent<Image>("img_online");
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        BluetoothEvents.OnBluetoothDeviceStateChangedEvent += OnBluetoothDeviceStateChanged;
        OnBluetoothDeviceStateChanged((int)BluetoothProxy.Intance.BluetoothState);
    }

    private void OnBluetoothDeviceStateChanged(int status)
    {
        BluetoothStatus bluetoothStatus = (BluetoothStatus)status;

        this.m_imgOffline.gameObject.SetActive(bluetoothStatus != BluetoothStatus.CONNECTED);
        this.m_imgOnline.gameObject.SetActive(bluetoothStatus == BluetoothStatus.CONNECTED);
    }

    public override void OnExit()
    {
        BluetoothEvents.OnBluetoothDeviceStateChangedEvent -= OnBluetoothDeviceStateChanged;
    }

}
