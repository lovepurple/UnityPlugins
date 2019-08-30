using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothTest : MonoBehaviour
{
    public Button BtnGetBondDevices;
    public Button BtnSearch;
    public Button BtnGetStatus;
    public Button BtnGetConnectedName;
    public Button BtnIsEnable;
    public Button SendData;

    public Text DisplayTxt;

    private void Awake()
    {
        BluetoothProxy.Intance.InitializeBluetoothProxy();

        BluetoothProxy.Intance.BluetoothDevice.OnBluetoothDeviceStateChangedEvent += BluetoothDevice_OnBluetoothDeviceStateChangedEvent;
        BluetoothProxy.Intance.BluetoothDevice.OnConnectedEvent += BluetoothDevice_OnConnectedEvent;
        BluetoothProxy.Intance.BluetoothDevice.OnErrorEvent += BluetoothDevice_OnErrorEvent;
        BluetoothProxy.Intance.BluetoothDevice.OnReceiveDataEvent += BluetoothDevice_OnReceiveDataEvent;
        BluetoothProxy.Intance.BluetoothDevice.OnSearchedDeviceEvent += BluetoothDevice_OnSearchedDeviceEvent;
        BluetoothProxy.Intance.BluetoothDevice.OnSearchFinishEvent += BluetoothDevice_OnSearchFinishEvent;

    }

    private void BluetoothDevice_OnSearchFinishEvent(List<BluetoothDeviceInfo> obj)
    {
        Debug.Log(obj);
    }

    private void BluetoothDevice_OnSearchedDeviceEvent(BluetoothDeviceInfo obj)
    {
        Debug.Log(obj);
    }

    private void BluetoothDevice_OnReceiveDataEvent(byte[] obj)
    {
        DisplayTxt.text = obj.ToString();
    }

    private void BluetoothDevice_OnErrorEvent(string obj)
    {
        DisplayTxt.text = obj.ToString();
    }

    private void BluetoothDevice_OnConnectedEvent()
    {
        throw new System.NotImplementedException();
    }

    private void BluetoothDevice_OnBluetoothDeviceStateChangedEvent(int obj)
    {
        DisplayTxt.text = obj.ToString();
    }

    void Start()
    {
        BtnGetBondDevices.AddClickCallback(go =>
        {
            BluetoothProxy.Intance.BluetoothDevice.GetPariedDevices();
        });

        BtnSearch.AddClickCallback(go =>
        {
            BluetoothProxy.Intance.BluetoothDevice.SearchDevices();
        });

        BtnGetStatus.AddClickCallback(go =>
        {
            BluetoothProxy.Intance.BluetoothDevice.GetBluetoothDeviceStatus();
        });

        BtnGetConnectedName.AddClickCallback(go =>
        {
            BluetoothProxy.Intance.BluetoothDevice.GetConnectedDeviceName();
        });

        BtnIsEnable.AddClickCallback(go =>
        {
            BluetoothProxy.Intance.BluetoothDevice.IsBluetoothEnabled();
        });

        SendData.AddClickCallback(go =>
        {
            byte[] sendData = Encoding.ASCII.GetBytes("hello nidaye");
            BluetoothProxy.Intance.BluetoothDevice.SendData(sendData);
        });

    }

}
