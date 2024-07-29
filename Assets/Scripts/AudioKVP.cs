using UnityEngine;

[System.Serializable]
public record AudioKVP
{
    [field: SerializeField] public AudioType AudioType { get; set; }
    [field: SerializeField] public AudioSample AudioSample { get; set; }
}

[System.Serializable]
public struct AudioSample
{
    public AudioClip AudioClip;
    [Range(0f, 1f)] public float Volume;
}