using UnityEngine;

[CreateAssetMenu(fileName = "BGM", menuName = "Audio/Bgm Configuration")]
public class AudioBgmConfigurationSO : ScriptableObject
{
    public EAudioBgmTracking AudioTracking;
    public AudioClip AudioClip;
    [Range(0f, 1f)] public float Volumn = .2f;
}