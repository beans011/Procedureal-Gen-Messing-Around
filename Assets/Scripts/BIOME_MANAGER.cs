using UnityEngine;
using UnityEngine.Tilemaps;

public class BIOME_MANAGER : MonoBehaviour
{
    #region SINGLETON
    public static BIOME_MANAGER instance;

    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    /*
     
    --RULES OF ENGAGMENT FOR THIS SYSTEM-- 

    DEPENDING WHAT TILE ORDER IN BIOME LIST DEPENDS ON HOW THEY WILL BE GENERATED

    THIS IS VERY IMPORTANT YOU MUST REMEMBER THIS

     */

    //MAKE SURE ALL THE NOISE MAPS ARE HERE
    private float[,] mangroveNoiseMap;

    //Generates all the biome noisemaps
    public void GenerateBiomeNoiseMaps(BiomeTable biomeTable)
    {
        float seed = WORLD_GENERATOR.instance.GetElevationSeed();
        Debug.Log("GENERATING GOD BIOME SEEDS");

        foreach (Biome biome in biomeTable.biomes) 
        { 
            if (biome.name == "mangrove")
            {
                GenerateMangroveNoiseMap(biome, seed);
            }
        }  
    }

    //Generates the mangrove noise map
    private void GenerateMangroveNoiseMap(Biome biome, float seed)
    {
        mangroveNoiseMap = new float[WORLD_GENERATOR.instance.GetWorldX(), WORLD_GENERATOR.instance.GetWorldY()];
        
        for (int x = 0; x < WORLD_GENERATOR.instance.GetWorldX(); x++)
        {
            for (int y = 0; y < WORLD_GENERATOR.instance.GetWorldY(); y++)
            {
                float elevation = GenerateBiomeNoise(x, y, seed, biome.noiseMapData);
                mangroveNoiseMap[x, y] = elevation;
            }
        }
    }

    //Takes in the biome data to return a value for the noise map
    private float GenerateBiomeNoise(int x, int y, float seed, BiomeNoiseMapData data)
    {
        float noiseHeight = 0f;
        float amplitude = data.amplitude;
        float frequency = data.frequency;

        for (int i = 0; i < data.octaves; i++)
        {
            float sampleX = (x + seed) / data.noiseScale * data.frequency;
            float sampleY = (y + seed) / data.noiseScale * data.frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

            noiseHeight += perlinValue * amplitude;

            amplitude *= data.persistence;
            frequency *= data.lacurnity;
        }

        return Mathf.Clamp01((noiseHeight + 1) / 2f);
    }

    //The function to call for getting a tile will eventually add more biome generation maths in here 
    public TileBase GetTileForBiome(Biome biome, Vector3Int pos, GameObject chunk)
    {        
        Vector3Int posWorld = new Vector3Int(
            pos.x + (int)chunk.transform.position.x,
            pos.y + (int)chunk.transform.position.y,
            0
        );

        if (biome.name == "shallowOcean")
        {
            return GetFirstTile(biome);
        }
        if (biome.name == "beach")
        {
            return GetFirstTile(biome);
        }
        if (biome.name == "meadow")
        {
            return GetFirstTile(biome);
        }
        if (biome.name == "mangrove")
        {
            return GetMangroveTile(posWorld, biome);
        }

        Debug.LogError("NO TILE RETURNED");
        return null;
    }

    //GENERATION FOR GETTING BIOMES WITH ONLY TILES
    private TileBase GetFirstTile(Biome biome)
    {
        return biome.tiles[0];
    }

    //Generation for mangrove biome
    private TileBase GetMangroveTile(Vector3Int posWorld, Biome biome)
    {
        if (biome.tiles == null)
        {
            Debug.LogError("NO TILES IN MANGROVE BIOME");
            return null;
        }

        float noiseValue = mangroveNoiseMap[posWorld.x, posWorld.y];

        //CAN TWEAK NOISE VALUES HERE AS WHAT I WANT
        if (noiseValue < 0.3f)
        {
            return biome.tiles[0];
        }
        else
        {
            return biome.tiles[1];
        }
    }
}
