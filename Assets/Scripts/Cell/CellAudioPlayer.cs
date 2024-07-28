using UnityEngine;


[RequireComponent(typeof(QuantumCell),typeof(AudioSource))]
public class CellAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    AudioClip spawn;
    AudioClip erase;
    AudioClip scroll;
    AudioClip hover;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        QuantumCell quantumCell = GetComponent<QuantumCell>();
        quantumCell.OnCollapseState.AddListener(state => PlaySoundFX(spawn));
        quantumCell.OnResetState.AddListener(state => PlaySoundFX(erase));
        TryGetComponent<CellPainter>( out CellPainter cellPainter);
        cellPainter.OnHover?.AddListener(state => PlaySoundFX(hover));
        cellPainter.OnIndexChange?.AddListener(state => PlaySoundFX(scroll));

    }
    private void PlaySoundFX(AudioClip sfx)
    {
        audioSource.clip = sfx;
        audioSource.Play();
    }

    public void SetProperties(AudioClip[] audioClip)
    {
        this.spawn = audioClip[0];
        this.erase = audioClip[1];
        this.scroll = audioClip[2];
        this.hover = audioClip[3];
    }
}
    