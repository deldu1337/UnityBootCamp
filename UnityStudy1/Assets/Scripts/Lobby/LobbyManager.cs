using UnityEngine;

public class LobbyManager : SingletonBehaviour<LobbyManager>
{
    public LobbyUIController LobbyUIController { get; private set; }

    protected override void Init()
    {
        m_IsDestroyOnLoad = true;

        base.Init();
    }

    private void Start()
    {
        LobbyUIController = FindFirstObjectByType<LobbyUIController>();
        if (LobbyUIController == null)
        {
            Logger.LogError("LobbyUIController does not exist.");
            return;
        }

        LobbyUIController.Init();
        AudioManager.Instance.PlayBGM(BGM.lobby);
    }
}
