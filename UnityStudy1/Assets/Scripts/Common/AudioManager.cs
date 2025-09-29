using System;
using System.Collections.Generic;
using UnityEngine;

public enum BGM
{
    lobby,
    COUNT
}

public enum SFX
{
    chapter_clear,
    stage_clear,
    ui_button_click,
    COUNT
}

public class AudioManager : SingletonBehaviour<AudioManager>
{
    public Transform BGMTransform;
    public Transform SFXTransform;

    private const string AUDIO_PATH = "Audio";

    private Dictionary<BGM, AudioSource> m_BGMPlayer = new Dictionary<BGM, AudioSource>();
    private AudioSource m_currentBGMSource;
    
    private Dictionary<SFX, AudioSource> m_SFXPlayer = new Dictionary<SFX, AudioSource>();

    protected override void Init()
    {
        base.Init();

        LoadBGMPlayer();
        LoadSFXPlayer();
    }

    private void LoadBGMPlayer()
    {
        for (int i = 0; i < (int)BGM.COUNT; i++)
        {
            string audioName = ((BGM)i).ToString();
            string pathString = $"{AUDIO_PATH}/{audioName}";
            AudioClip audioClip = Resources.Load<AudioClip>(pathString);
            if(audioClip == false)
            {
                Logger.LogError($"{audioName} clip does not exits.");
                continue;
            }

            GameObject newGO = new GameObject(audioName);
            AudioSource newAudioSource = newGO.AddComponent<AudioSource>();
            newAudioSource.clip = audioClip;
            newAudioSource.loop = true;
            newAudioSource.playOnAwake = false;
            newGO.transform.SetParent(BGMTransform);

            m_BGMPlayer[(BGM)i] = newAudioSource;
        }
    }

    private void LoadSFXPlayer()
    {
        for (int i = 0; i < (int)SFX.COUNT; i++)
        {
            string audioName = ((SFX)i).ToString();
            string pathString = $"{AUDIO_PATH}/{audioName}";
            AudioClip audioClip = Resources.Load<AudioClip>(pathString);
            if (audioClip == false)
            {
                Logger.LogError($"{audioName} clip does not exits.");
                continue;
            }

            GameObject newGO = new GameObject(audioName);
            AudioSource newAudioSource = newGO.AddComponent<AudioSource>();
            newAudioSource.clip = audioClip;
            newAudioSource.loop = false;
            newAudioSource.playOnAwake = false;
            newGO.transform.SetParent(SFXTransform);

            m_SFXPlayer[(SFX)i] = newAudioSource;
        }
    }

    public void PlayBGM(BGM bgm)
    {
        if (m_currentBGMSource != null)
        {
            m_currentBGMSource.Stop();
            m_currentBGMSource = null;
        }

        if (m_BGMPlayer.ContainsKey(bgm) == false)
        {
            Logger.LogError($"Invalid clip name. {bgm}");
            return;
        }

        m_currentBGMSource = m_BGMPlayer[bgm];
        m_currentBGMSource.Play();
    }

    public void PauseBGM()
    {
        if (m_currentBGMSource != null)
            m_currentBGMSource.Pause();
    }

    public void ResumeBGM()
    {
        if (m_currentBGMSource != null)
            m_currentBGMSource.UnPause();
    }

    public void StopBGM()
    {
        if (m_currentBGMSource != null)
            m_currentBGMSource.Stop();
    }

    public void PlaySFX(SFX sfx)
    {
        if (m_SFXPlayer.ContainsKey(sfx) == false)
        {
            Logger.LogError($"Invalid clip name. {sfx}");
            return;
        }

        m_SFXPlayer[sfx].Play();
    }

    public void Mute()
    {
        foreach (var audioSourceItem in m_BGMPlayer)
        {
            audioSourceItem.Value.volume = 0f;
        }

        foreach (var audioSourceItem in m_SFXPlayer)
        {
            audioSourceItem.Value.volume = 0f;
        }
    }

    public void UnMute()
    {
        foreach (var audioSourceItem in m_BGMPlayer)
        {
            audioSourceItem.Value.volume = 1f;
        }

        foreach (var audioSourceItem in m_SFXPlayer)
        {
            audioSourceItem.Value.volume = 1f;
        }
    }
}
