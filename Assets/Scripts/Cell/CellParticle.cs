using UnityEngine;

[RequireComponent(typeof(QuantumCell))]
public class CellParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfxPrefab;
    private ParticleSystem vfx;
    private void Awake()
    {
        vfxPrefab.gameObject.SetActive(false);
        vfx = Instantiate(vfxPrefab, transform);
        GetComponent<QuantumCell>().OnCollapseState.AddListener(() => Play());
    }
    private void Play()
    {
        vfx.gameObject.SetActive(true);
        vfx.Play();
    }
}