using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(QuantumCell),typeof(AudioSource))]
public class CellAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public Dictionary<AudioType, AudioSample> Audio = new();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        QuantumCell quantumCell = GetComponent<QuantumCell>();

        quantumCell.OnCollapseState.AddListener(_ => PlayAudio(AudioType.Spawn));
        quantumCell.OnResetState.AddListener(_ => PlayAudio(AudioType.Erase));

        if (TryGetComponent<CellPainter>(out CellPainter cellPainter))
        {
            cellPainter.OnHover?.AddListener(_ => PlayAudio(AudioType.Hover));
            cellPainter.OnIndexChange?.AddListener(_ => PlayAudio(AudioType.Scroll));
        }
    }

    private void PlaySoundFX(AudioClip sfx, float volume)
    {
        audioSource.pitch = Random.Range(1f, 1.1f);
        audioSource.clip = sfx;
        audioSource.volume = volume;
        audioSource.Play();
    }


    private void PlayAudio(AudioType audioType)
    {
        if (Audio.TryGetValue(audioType, out var audioKVP))
            PlaySoundFX(audioKVP.AudioClip, audioKVP.Volume);
    }
    public void SetProperties(Dictionary<AudioType, AudioSample> audio)
    {
        Audio = audio;
    }
}
    