package com.lovepurple.blecontroller;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothManager;
import android.bluetooth.BluetoothProfile;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.util.Log;

import com.google.gson.Gson;
import com.lovepurple.bluetoothcommom.BluetoothDeviceInfo;
import com.lovepurple.bluetoothcommom.IBluetoothManager;
import com.lovepurple.bluetoothcommom.IUnityBluetoothAdapter;
import com.lovepurple.bluetoothcommom.UnityBridgeUtility;
import com.lovepurple.bluetoothcommom.UnityMessageAdapter;
import com.lovepurple.bluetoothcommom.UnityMessageDefine;
import com.lovepurple.bluetoothcommom.UnityStringCallback;

import java.util.ArrayList;
import java.util.UUID;

/**
 * BLE 控制器
 */

public class BLEManager implements IBluetoothManager, IUnityBluetoothAdapter {
    private static BLEManager _instance;

    //接、收的 UUID
    public static UUID BLE_SHIELD_TX_UUID = UUID.fromString("713d0003-503e-4c75-ba94-3148f18d941e");
    public static UUID BLE_SHIELD_RX_UUID = UUID.fromString("713d0002-503e-4c75-ba94-3148f18d941e");

    public final static UUID UUID_SERVICE =
            UUID.fromString("0000ffe0-0000-1000-8000-00805f9b34fb");

    // 自定义的Intent
    public final static String ACTION_RECEIVED_DATA = "ACTION_RECEIVED_DATA";
    public final static String RECEIVED_DATA_INTENT_KEY = "RECEIVED_DATA_INTENT_KEY";

    private Context _applicationContext;
    private BluetoothManager mBluetoothManager = null;
    private BluetoothAdapter mBluetoothAdapter = null;
    private BluetoothGatt mBluetoothGatt;       //远程设备的gatt

    //发送到Unity的代理
    private UnityStringCallback mSendToUnityHandler = null;

    private BLEManager(Context context) {
        this._applicationContext = context;
    }


    public static BLEManager getInstance(Context context) {
        if (_instance == null) {
            synchronized (BLEManager.class) {
                _instance = new BLEManager(context);
                _instance.initializeBluetoothManager();
            }
        }
        return _instance;
    }

    @Override
    public void initializeBluetoothManager() {
        this.mBluetoothManager = (BluetoothManager) _applicationContext.getSystemService(Context.BLUETOOTH_SERVICE);
        this.mBluetoothAdapter = mBluetoothManager.getAdapter();

        registerBLEIntentReceiver();
    }

    @Override
    public boolean isEnabled() {
        return mBluetoothAdapter != null && mBluetoothAdapter.isEnabled();
    }

    @Override
    public boolean isSupported() {
        //判断是否支持BLE
        return _applicationContext.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE);
    }

    @Override
    public String getPariedDevices() {
        Gson gson = new Gson();

        ArrayList<BluetoothDeviceInfo> deviceInfos = new ArrayList<>();

        for (BluetoothDevice bluetoothDevice : mBluetoothAdapter.getBondedDevices()) {
            String address = bluetoothDevice.getAddress();
            String bluetoothDeviceName = bluetoothDevice.getName();

            BluetoothDeviceInfo deviceInfo = new BluetoothDeviceInfo();
            deviceInfo.deviceAddress = address;
            deviceInfo.deviceName = bluetoothDeviceName;
            deviceInfo.deviceBondState = 1;

            deviceInfos.add(deviceInfo);
        }

        return gson.toJson(deviceInfos);
    }

    @Override
    public boolean connectDevice(String dstAddress) {
        if (mBluetoothAdapter == null || dstAddress == null) {
            log(1, "BluetoothAdapter 未初始化 或 目标地址为空");
            return false;
        }

        BluetoothDevice remoteDevice = mBluetoothAdapter.getRemoteDevice(dstAddress);
        if (remoteDevice == null) {
            log(1, dstAddress + "远程设备不存在");
            return false;
        }

        //关闭之前的gatt服务
        if (mBluetoothGatt != null) {
            mBluetoothGatt.close();
        }

        mBluetoothGatt = remoteDevice.connectGatt(_applicationContext, false, mGattCallback);
        return true;
    }

    /**
     * 注册要监听的Intent事件
     */
    private void registerBLEIntentReceiver() {
        IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction(ACTION_RECEIVED_DATA);


        _applicationContext.registerReceiver(_mGattUpdateReceiver, intentFilter);

    }

    //gatt服务回调
    private final BluetoothGattCallback mGattCallback = new BluetoothGattCallback() {
        @Override
        public void onServicesDiscovered(BluetoothGatt gatt, int status) {

        }

        //连接状态改变，status ：操作是否成功   newState：具体的状态
        @Override
        public void onConnectionStateChange(BluetoothGatt gatt, int status, int newState) {
            if (status == BluetoothGatt.GATT_SUCCESS) {

                if (newState == BluetoothProfile.STATE_CONNECTED) {
                    mBluetoothGatt.discoverServices();

                } else if (newState == BluetoothProfile.STATE_DISCONNECTED) {
                    mBluetoothGatt.close();
                    mBluetoothGatt = null;

                }
            }
        }

        //接收到数据,status 都是指的是否成功
        @Override
        public void onCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
            if (status == BluetoothGatt.GATT_SUCCESS && characteristic.getUuid() == BLE_SHIELD_RX_UUID) {
                byte[] recvData = characteristic.getValue();

                //Intent 实现事件通知
                Intent recvIntent = new Intent(ACTION_RECEIVED_DATA);
                recvIntent.putExtra(RECEIVED_DATA_INTENT_KEY, recvData);

                _applicationContext.sendBroadcast(recvIntent);      //发送相关Intent 的事件
            }
        }

        //同上，没有status
        @Override
        public void onCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic) {
            if (characteristic.getUuid() == BLE_SHIELD_RX_UUID) {
                byte[] recvData = characteristic.getValue();

                //Intent 实现事件通知
                Intent recvIntent = new Intent(ACTION_RECEIVED_DATA);
                recvIntent.putExtra(RECEIVED_DATA_INTENT_KEY, recvData);

                _applicationContext.sendBroadcast(recvIntent);      //发送相关Intent 的事件
            }
        }


    };

    /**
     * 自定义的Receiver
     */
    private final BroadcastReceiver _mGattUpdateReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String intentAction = intent.getAction();

            //接收到消息
            if (intentAction.equals(ACTION_RECEIVED_DATA)) {
                byte[] recvBuffer = intent.getByteArrayExtra(RECEIVED_DATA_INTENT_KEY);
                UnityMessageAdapter messageToUnity = new UnityMessageAdapter();
                messageToUnity.mMessageID = UnityMessageDefine.SEND_MESSAGE_BUFFER;
                messageToUnity.mMessageBody = new String(recvBuffer);

                sendMessageToUnity(new Gson().toJson(messageToUnity));
            }
        }
    };

    @Override
    public void disconnect() {
        if (mBluetoothGatt == null || mBluetoothAdapter == null) {
            log(1, "bluetooth 未初始化");
        }

        mBluetoothGatt.disconnect();
        mBluetoothGatt.close();
        mBluetoothGatt = null;
    }

    @Override
    public void searchDevices() {

    }

    @Override
    public void sendData(byte[] bufferData) {

    }

    @Override
    public void log(int logType, String logMessage) {
        String logFormatToUnity = UnityBridgeUtility.convertToUnityLog(logType, logMessage);
        sendMessageToUnity(logFormatToUnity);

        Log.w(this.getClass().getSimpleName(), logMessage);
    }

    @Override
    public void initializeBluetoothForUnity(UnityStringCallback sendStringToUnityCallback) {
        this.mSendToUnityHandler = sendStringToUnityCallback;
    }

    @Override
    public void sendMessageToUnity(String strMessage) {
        if (this.mSendToUnityHandler != null) {
            if (strMessage != "")
                this.mSendToUnityHandler.sendMessage(strMessage);
        }
    }


}
