using UnityEngine;

[CreateAssetMenu(menuName = "Biome Noise Map Data")]
public class BiomeNoiseMapData : ScriptableObject
{
    public float noiseScale;
    public int octaves;
    public float persistence;
    public float lacurnity;
    public float amplitude;
    public float frequency;
}
