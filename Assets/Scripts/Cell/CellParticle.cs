using UnityEngine;

[RequireComponent(typeof(QuantumCell))]
public class CellParticle : MonoBehaviour
{
    private ParticleSystem Vfx;
    private void Awake()
    {
        GetComponent<QuantumCell>().OnCollapseState.AddListener(state => Play());
    }
    public void Initialize(ParticleSystem vfx)
    {
        Vfx = Instantiate(vfx, transform);
        Vfx.gameObject.SetActive(false);
    }
    private void Play()
    {
        Vfx.gameObject.SetActive(true);
        Vfx.Play();
    }
}