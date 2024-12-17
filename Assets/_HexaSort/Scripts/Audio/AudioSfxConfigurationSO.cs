using UnityEngine;

[CreateAssetMenu(fileName = "Sfx Configuration", menuName = "Audio/Sfx Configuration")]
public class AudioSfxConfigurationSO : ScriptableObject
{
    public EAudioSfxTracking AudioTracking;
    public AudioClip AudioClip;
    [Range(0f, 1f)] public float Volumn = .2f;
}