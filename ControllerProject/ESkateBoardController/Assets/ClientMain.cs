using EngineCore;
using UnityEngine;

public class ClientMain : MonoSingleton<ClientMain>
{
    private SkateMessageHandler m_skateMessageHandler = null;

    private void Awake()
    {
        BluetoothProxy.Intance.InitializeBluetoothProxy();
        m_skateMessageHandler = new SkateMessageHandler(BluetoothProxy.Intance.BluetoothDevice);
    }

    private void Start()
    {
        MainPanel mainPanel = new MainPanel(transform as RectTransform);

        mainPanel.OnCreate();
        mainPanel.OnEnter();
    }

    private void Update()
    {
        BluetoothProxy.Intance.Tick();
        TimeModule.Instance.Update();
    }

    public SkateMessageHandler SkateMessageHandler => m_skateMessageHandler;
}
