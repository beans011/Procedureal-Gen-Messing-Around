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

[CreateAssetMenu(menuName = "Biome Lookup Table")]
public class BiomeTable : ScriptableObject
{
    public List<Biome> biomes;
}