using EngineCore;
using UnityEngine;
using UnityEngine.UI;

public class SignalPanel : UIPanelLogicBase
{
    private Image m_imgOnline;
    private Image m_imgOffline;

    private Image[] m_imgBatteryList = new Image[6];
    private Text m_txtBattery;

    public SignalPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        this.m_imgOffline = m_panelRootObject.GetComponent<Image>("img_offline");
        this.m_imgOnline = m_panelRootObject.GetComponent<Image>("img_online");

        for (int i = 0; i <= 5; ++i)
            m_imgBatteryList[i] = m_panelRootObject.GetComponent<Image>($"battery_panel/battery_{0}");

        this.m_txtBattery = m_panelRootObject.GetComponent<Text>("battery_panel/txt_battery");
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


    private void SetBatteryLevel(int batteryLevel)
    {
        for (int i = 0; i <= 5; ++i)
            m_imgBatteryList[i].gameObject.SetActive(i == batteryLevel);

        this.m_txtBattery.text = $"{10}%";
    }

}
