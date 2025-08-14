using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Biome
{
    public string name;

    [Range(0f, 1f)] public float elevationMin;
    [Range(0f, 1f)] public float elevationMax;

    [Range(0f, 1f)] public float moistureMin;
    [Range(0f, 1f)] public float moistureMax;

    public float blendMargin = 0.05f;

    public bool isTileNotWalkable;

    public TileBase[] tiles; //Assign from tile palette in inspector
    public BiomeNoiseMapData noiseMapData;
}

[CreateAssetMenu(menuName = "Biome Noise Map Data")]
public class BiomeNoiseMapData : ScriptableObject
{
    public float noiseScale { get; private set; }
    public int octaves {  get; private set; }
    public float persistence {  get; private set; }
    public float lacurnity { get; private set; }
    public float amplitude { get; private set; }
    public float frequency { get; private set; }
}

[CreateAssetMenu(menuName = "Biome Lookup Table")]
public class BiomeTable : ScriptableObject
{
    public List<Biome> biomes;
}