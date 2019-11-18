package com.lovepurple.blecontroller;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattService;
import android.bluetooth.BluetoothManager;
import android.bluetooth.BluetoothProfile;
import android.bluetooth.le.BluetoothLeScanner;
import android.bluetooth.le.ScanCallback;
import android.bluetooth.le.ScanResult;
import android.bluetooth.le.ScanSettings;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.util.Log;

import com.google.gson.Gson;
import com.lovepurple.bluetoothcommom.BluetoothDeviceInfo;
import com.lovepurple.bluetoothcommom.BluetoothStatus;
import com.lovepurple.bluetoothcommom.IBluetoothManager;
import com.lovepurple.bluetoothcommom.IUnityBluetoothAdapter;
import com.lovepurple.bluetoothcommom.UnityBridgeUtility;
import com.lovepurple.bluetoothcommom.UnityMessageAdapter;
import com.lovepurple.bluetoothcommom.UnityMessageDefine;
import com.lovepurple.bluetoothcommom.UnityStringCallback;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Queue;
import java.util.UUID;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.LinkedTransferQueue;

/**
 * BLE 控制器
 */

public class BLEManager implements IBluetoothManager, IUnityBluetoothAdapter {
    private static BLEManager _instance;

    private static String TAG = BLEManager.class.getName();

    //接、收的 Characteristic  UUID
    //不同的设备 可能不相同  BT16 BT20 都是这个
    public static UUID BLE_SHIELD_TX_UUID = UUID.fromString("0000ffe1-0000-1000-8000-00805f9b34fb");
    public static UUID BLE_SHIELD_RX_UUID = UUID.fromString("0000ffe2-0000-1000-8000-00805f9b34fb");

    //服务UUID
    public final static UUID UUID_SERVICE =
            UUID.fromString("0000ffe0-0000-1000-8000-00805f9b34fb");

   /*
        另一组常用的uuid
   mServiceUuidtry1 = "0000ffe0-0000-1000-8000-00805f9b34fb";
    mRxUuidtry1 = "0000ffe2-0000-1000-8000-00805f9b34fb";
    mTxUuidtry1 = "0000ffe1-0000-1000-8000-00805f9b34fb";
    mServiceUuidtry2 = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
    mRxUuidtry2 = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
    mTxUuidtry2 = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";

    */

    // 自定义的Intent
    public final static String ACTION_RECEIVED_DATA = "ACTION_RECEIVED_DATA";
    public final static String RECEIVED_DATA_INTENT_KEY = "RECEIVED_DATA_INTENT_KEY";
    public final static String ACTION_SCANNED_FINISH = "ACTION_SCANNED_FINISH";
    public final static String ACTION_BLUETOOTH_STATE_CHANGED = "ACTION_BLUETOOTH_STATE_CHANGED";


    private Context _applicationContext;
    private BluetoothManager mBluetoothManager = null;
    private BluetoothAdapter mBluetoothAdapter = null;
    private BluetoothGatt mBluetoothGatt;       //远程设备的gatt
    private static final long SCAN_PERIOD = 10000;

    // 数据发送队列
    private Queue<byte[]> mSendQueue = new LinkedTransferQueue<>();

    //当前蓝牙状态
    private BluetoothStatus mDeviceCurrentStatus = BluetoothStatus.FREE;

    //扫描到的设备列表
    private HashMap<String, BluetoothDeviceInfo> mSearchedRemoteDeviceMap = new HashMap<>();


    //线程池
    private ExecutorService mExecutorSerivicePool = Executors.newCachedThreadPool();

    //发送到Unity的代理
    private UnityStringCallback mSendToUnityHandler = null;

    //发送线程的线程池句柄
    private Future mSendThreadHandler = null;

    //接收，发送的服务 ，有时不用区分，可以通过一个同时收发
    private BluetoothGattCharacteristic mTxCharacteristic;
    private BluetoothGattCharacteristic mRxCharacteristic;

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
        intentFilter.addAction(ACTION_SCANNED_FINISH);
        intentFilter.addAction(ACTION_BLUETOOTH_STATE_CHANGED);

        _applicationContext.registerReceiver(mBLEUpdateReceiver, intentFilter);

    }

    //gatt服务回调
    private final BluetoothGattCallback mGattCallback = new BluetoothGattCallback() {

        //发现服务
        @Override
        public void onServicesDiscovered(BluetoothGatt gatt, int status) {

            if (status != BluetoothGatt.GATT_SUCCESS) {
                Log.e(TAG, "Serivces discovery failed :" + status);
                return;
            }

            //获取通讯的服务
            //todo : 后期尝试能不能不通过指定的UUID获取（不同类型的设备有可能不同）
            BluetoothGattService service = gatt.getService(UUID_SERVICE);

            mTxCharacteristic = service.getCharacteristic(BLE_SHIELD_TX_UUID);
            if (mTxCharacteristic == null) {
                Log.e(TAG, "send charactristic can't find");
                return;
            }

            mRxCharacteristic = service.getCharacteristic(BLE_SHIELD_RX_UUID);
            if (mTxCharacteristic == null) {
                Log.e(TAG, "receive charactristic can't find");
                return;
            }

            gatt.setCharacteristicNotification(mTxCharacteristic, true);
            gatt.setCharacteristicNotification(mRxCharacteristic, true);
            gatt.readCharacteristic(mRxCharacteristic);


            SendRunnable mSendThread = new SendRunnable();
            mSendThreadHandler = mExecutorSerivicePool.submit(mSendThread);

        }

        //连接状态改变，status ：操作是否成功   newState：具体的状态
        @Override
        public void onConnectionStateChange(BluetoothGatt gatt, int status, int newState) {
            if (status == BluetoothGatt.GATT_SUCCESS) {

                final Intent bluetoothStausChangedIntent = new Intent(ACTION_BLUETOOTH_STATE_CHANGED);

                if (newState == BluetoothProfile.STATE_CONNECTED) {
                    mDeviceCurrentStatus = BluetoothStatus.CONNECTED;
                    mBluetoothGatt.discoverServices();

                } else if (newState == BluetoothProfile.STATE_DISCONNECTED) {
                    mBluetoothGatt.close();
                    mBluetoothGatt = null;
                    mDeviceCurrentStatus = BluetoothStatus.FREE;
                    mSendThreadHandler.cancel(true);            //取消线程
                }

                _applicationContext.sendBroadcast(bluetoothStausChangedIntent);
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
    private final BroadcastReceiver mBLEUpdateReceiver = new BroadcastReceiver() {
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
            // 扫描结束
            else if (intentAction.equals(ACTION_SCANNED_FINISH)) {
                UnityMessageAdapter messageToUnity = new UnityMessageAdapter();
                messageToUnity.mMessageID = UnityMessageDefine.SEARCHED_DEVICE_FINISH;
                messageToUnity.mMessageBody = mSearchedRemoteDeviceMap.values().toArray();

                Gson gson = new Gson();
                String jsonMessageToUnity = gson.toJson(messageToUnity);
                sendMessageToUnity(jsonMessageToUnity);
            }
            // 蓝牙状态改变
            else if (intentAction.equals(ACTION_BLUETOOTH_STATE_CHANGED)) {
                UnityMessageAdapter messageToUnity = new UnityMessageAdapter();
                messageToUnity.mMessageID = UnityMessageDefine.BLUETOOTH_STATE_CHANGED;
                messageToUnity.mMessageBody = mDeviceCurrentStatus.ordinal();

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
        mRxCharacteristic = null;
        mTxCharacteristic = null;
        mSendThreadHandler.cancel(true);
    }

    private boolean mIsScaningDevice = false;

    @Override
    public void searchDevices(boolean isEnable) {
        final BluetoothLeScanner scanner = mBluetoothAdapter.getBluetoothLeScanner();

        if (isEnable) {
            //子线程扫描
            new Thread() {
                @Override
                public void run() {
                    mSearchedRemoteDeviceMap.clear();
                    mIsScaningDevice = true;
                    ScanSettings.Builder builder = new ScanSettings.Builder();
                    builder.setScanMode(ScanSettings.SCAN_MODE_BALANCED);
                    scanner.startScan(mLeScanCallback);

                    //阻塞住主线程
                    try {
                        Thread.sleep(SCAN_PERIOD);
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }

                    //SCAN_PERIOD 时间后关闭扫描
                    mIsScaningDevice = false;
                    scanner.stopScan(mLeScanCallback);
                }
            }.start();
        } else {
            mIsScaningDevice = false;
            scanner.stopScan(mLeScanCallback);
        }
    }

    //Android 中大量使用 callback的写法
    private ScanCallback mLeScanCallback = new ScanCallback() {
        //扫到设备回调
        @Override
        public void onScanResult(int callbackType, ScanResult result) {


        }

        //扫到所有回调
        @Override
        public void onBatchScanResults(List<ScanResult> results) {
            for (ScanResult scanResult : results) {
                BluetoothDevice searchDevice = scanResult.getDevice();
                BluetoothDeviceInfo searchDeviceInfo = new BluetoothDeviceInfo();
                searchDeviceInfo.deviceAddress = searchDevice.getAddress();
                searchDeviceInfo.deviceName = searchDevice.getName();
                searchDeviceInfo.deviceBondState = searchDevice.getBondState();

                mSearchedRemoteDeviceMap.put(searchDeviceInfo.deviceName, searchDeviceInfo);
            }

            Intent intent = new Intent(ACTION_SCANNED_FINISH);
            _applicationContext.sendBroadcast(intent);
        }

        @Override
        public void onScanFailed(int errorCode) {
            log(UnityMessageDefine.SEND_ERROR, "scan error :" + errorCode);
        }
    };

    @Override
    public void sendData(byte[] bufferData) {
        if (mDeviceCurrentStatus == BluetoothStatus.CONNECTED) {
            mSendQueue.add(bufferData);
        }
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

    /**
     * 发送线程
     */
    private class SendRunnable implements Runnable {

        private boolean isRunning = true;

        @Override
        public void run() {
            while (isRunning && mTxCharacteristic != null) {
//                try {
                int messageCount = mSendQueue.size();
                Log.e(TAG, String.valueOf(messageCount));

                //合包发送
                for (int i = 0; i < mSendQueue.size(); ++i) {
                    byte[] messageBuffer = mSendQueue.poll();
                    mTxCharacteristic.setValue(messageBuffer);           //todo:超过20字节需要分包？
                    mBluetoothGatt.writeCharacteristic(mTxCharacteristic);

//
//                        _bluetoothSendStream.write(messageBuffer);
//
//                        if (messageBuffer[messageBuffer.length - 1] != '\n')
//                            _bluetoothSendStream.write('\n');
                }

//                    if (messageCount > 0)
//                        _bluetoothSendStream.flush();

//                } catch (IOException e) {
//                    log(UnityMessageDefine.SEND_ERROR, e.getMessage());
//                }
            }
        }

        public void killSend() {
            this.isRunning = false;
        }
    }

}
