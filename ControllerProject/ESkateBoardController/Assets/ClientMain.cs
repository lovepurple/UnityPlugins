using EngineCore;
using UnityEngine;

public class ClientMain : MonoSingleton<ClientMain>
{
    private SkateMessageHandler m_skateMessageHandler = null;

    private void Awake()
    {
        SpeedController.Instance.InitSpeedController();
    }

    private void Start()
    {
        MainPanel mainPanel = new MainPanel(transform as RectTransform);

        mainPanel.OnCreate();
        mainPanel.OnEnter();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SpeedController.Instance.BrakeSoftly();
    }


    private void Update()
    {
        BluetoothProxy.Intance.Tick();
        TimeModule.Instance.Update();
    }

    public SkateMessageHandler SkateMessageHandler => m_skateMessageHandler;
}
