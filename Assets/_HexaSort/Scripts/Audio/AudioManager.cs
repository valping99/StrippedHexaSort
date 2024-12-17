using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private List<AudioBgmConfigurationSO> _audioBgmConfigurations;
    [SerializeField] private List<AudioSfxConfigurationSO> _audioSfxConfigurations;
    [SerializeField] private bool _dontDestroyOnLoad;

    private void Awake()
    {
        if (Instance == null && _dontDestroyOnLoad)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PlaySfxSource(EAudioSfxTracking track)
    {
        var audio = _audioSfxConfigurations.Find(x => x.AudioTracking == track);
        if (audio == null)
        {
            Debug.Log($"No audio configuration found for track {track}");
            return;
        }

        _sfxSource.volume = audio.Volumn;
        _sfxSource.PlayOneShot(audio.AudioClip);
    }

    public void PlayBgmSource(EAudioBgmTracking track)
    {
        var audio = _audioBgmConfigurations.Find(x => x.AudioTracking == track);
        if (audio == null)
        {
            Debug.Log($"No audio configuration found for track {track}");
            return;
        }

        _sfxSource.volume = audio.Volumn;
        _sfxSource.PlayOneShot(audio.AudioClip);
    }
}