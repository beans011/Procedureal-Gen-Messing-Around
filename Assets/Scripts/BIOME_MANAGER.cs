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
    private float[,] deepOceanNoiseMap;
    private float[,] shallowONoiseMap;
    private float[,] icebergNoiseMap;
    private float[,] mangroveNoiseMap;
    private float[,] warmBeachNoiseMap;
    private float[,] beachNoiseMap;
    private float[,] tundraNoiseMap;
    private float[,] jungleNoiseMap;
    private float[,] grasslandNoiseMap;
    private float[,] forestNoiseMap;
    private float[,] taigaNoiseMap;
    private float[,] cliffsNoiseMap;
    private float[,] mountainsNoiseMap;
    private float[,] steppeNoiseMap;
    private float[,] snowCapsNoiseMap;

    #region Generate Biome Noisemaps
    //Generates all the biome noisemaps

    public void GenerateBiomeNoiseMaps(BiomeTable biomeTable)
    {
        float seed = WORLD_GENERATOR.instance.GetElevationSeed();
        Debug.Log("GENERATING GOD BIOME SEEDS");

        foreach (Biome biome in biomeTable.biomes) 
        { 
            if (biome.name == "iceberg") { GenerateIcebergNoiseMap(biome, seed); }
            if (biome.name == "mangrove") { GenerateMangroveNoiseMap(biome, seed); }
            if (biome.name == "tundra") { GenerateTundraNoiseMap(biome, seed); }
            if (biome.name == "cliffs") { GenerateCliffsNoiseMap(biome, seed); }
            if (biome.name == "steppe") { GenerateSteppeNoiseMap(biome, seed); }
        }  
    }

    //Generates the icberg noise map
    private void GenerateIcebergNoiseMap(Biome biome, float seed)
    {
        //Cheeky validation
        if (biome.noiseMapData == null)
        {
            Debug.LogError("NO BIOME NOISE MAP DATA");
            return;
        }

        icebergNoiseMap = new float[WORLD_GENERATOR.instance.GetWorldX(), WORLD_GENERATOR.instance.GetWorldY()];
        
        for (int x = 0; x < WORLD_GENERATOR.instance.GetWorldX(); x++)
        {
            for (int y = 0; y < WORLD_GENERATOR.instance.GetWorldY(); y++)
            {
                float elevation = GenerateBiomeNoise(x, y, seed, biome.noiseMapData);
                icebergNoiseMap[x, y] = elevation;
            }
        }
    }

    //Generates the mangrove noise map
    private void GenerateMangroveNoiseMap(Biome biome, float seed)
    {
        //Cheeky validation
        if (biome.noiseMapData == null)
        {
            Debug.LogError("NO BIOME NOISE MAP DATA");
            return;
        }

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

    //Generates the tundra noise map
    private void GenerateTundraNoiseMap(Biome biome, float seed)
    {
        //Cheeky validation
        if (biome.noiseMapData == null)
        {
            Debug.LogError("NO BIOME NOISE MAP DATA");
            return;
        }

        tundraNoiseMap = new float[WORLD_GENERATOR.instance.GetWorldX(), WORLD_GENERATOR.instance.GetWorldY()];

        for (int x = 0; x < WORLD_GENERATOR.instance.GetWorldX(); x++)
        {
            for (int y = 0; y < WORLD_GENERATOR.instance.GetWorldY(); y++)
            {
                float elevation = GenerateBiomeNoise(x, y, seed, biome.noiseMapData);
                tundraNoiseMap[x, y] = elevation;
            }
        }
    }

    //Generate the cliffs noise map
    private void GenerateCliffsNoiseMap(Biome biome, float seed)
    {
        //Cheeky validation
        if (biome.noiseMapData == null)
        {
            Debug.LogError("NO BIOME NOISE MAP DATA");
            return;
        }

        cliffsNoiseMap = new float[WORLD_GENERATOR.instance.GetWorldX(), WORLD_GENERATOR.instance.GetWorldY()];

        for (int x = 0; x < WORLD_GENERATOR.instance.GetWorldX(); x++)
        {
            for (int y = 0; y < WORLD_GENERATOR.instance.GetWorldY(); y++)
            {
                float elevation = GenerateBiomeNoise(x, y, seed, biome.noiseMapData);
                cliffsNoiseMap[x, y] = elevation;
            }
        }
    }

    //Generate the steppe noise map
    private void GenerateSteppeNoiseMap(Biome biome, float seed)
    {
        //Cheeky validation
        if (biome.noiseMapData == null)
        {
            Debug.LogError("NO BIOME NOISE MAP DATA");
            return;
        }

        steppeNoiseMap = new float[WORLD_GENERATOR.instance.GetWorldX(), WORLD_GENERATOR.instance.GetWorldY()];

        for (int x = 0; x < WORLD_GENERATOR.instance.GetWorldX(); x++)
        {
            for (int y = 0; y < WORLD_GENERATOR.instance.GetWorldY(); y++)
            {
                float elevation = GenerateBiomeNoise(x, y, seed, biome.noiseMapData);
                steppeNoiseMap[x, y] = elevation;
            }
        }
    }

    #endregion

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

        if (biome.name == "deep_ocean") { return GetFirstTile(biome); }
        if (biome.name == "shallow_ocean") { return GetFirstTile(biome); }
        if (biome.name == "iceberg") { return GetIcebergTile(posWorld, biome); }       
        if (biome.name == "mangrove") { return GetMangroveTile(posWorld, biome); }
        if (biome.name == "warm_beach") { return GetFirstTile(biome); }
        if (biome.name == "beach") { return GetFirstTile(biome); }
        if (biome.name == "tundra") { return GetTundraTile(posWorld,biome); }
        if (biome.name == "jungle") { return GetFirstTile(biome); }
        if (biome.name == "grassland") { return GetFirstTile(biome); }
        if (biome.name == "forest") { return GetFirstTile(biome); }
        if (biome.name == "taiga") { return GetFirstTile(biome); }
        if (biome.name == "cliffs") { return GetCliffsTile(posWorld, biome); }
        if (biome.name == "mountains") { return GetFirstTile(biome); }
        if (biome.name == "steppe") { return GetSteppeTile(posWorld, biome); }
        if (biome.name == "snow_caps") { return GetFirstTile(biome); }

        Debug.LogError("NO TILE RETURNED");
        return null;
    }

    #region Get the tiles for biome

    //GENERATION FOR GETTING BIOMES WITH ONLY TILES
    private TileBase GetFirstTile(Biome biome)
    {
        return biome.tiles[0];
    }

    //Generation for iceberg biome
    private TileBase GetIcebergTile(Vector3Int posWorld, Biome biome)
    {
        if (biome.tiles == null)
        {
            Debug.LogError("NO TILES IN ICEBERG BIOME");
            return null;
        }

        float noiseValue = icebergNoiseMap[posWorld.x, posWorld.y];

        //CAN TWEAK NOISE VALUES HERE AS WHAT I WANT
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

    //Generation for tundra biom
    private TileBase GetTundraTile(Vector3Int posWorld, Biome biome)
    {
        if (biome.tiles == null)
        {
            Debug.LogError("NO TILES IN TUNDRA BIOME");
            return null;
        }

        float noiseValue = tundraNoiseMap[posWorld.x, posWorld.y];

        //CAN TWEAK NOISE VALUES HERE AS WHAT I WANT
        return biome.tiles[0];
    }

    //Generation for cliffs biome
    private TileBase GetCliffsTile(Vector3Int posWorld, Biome biome)
    {
        if (biome.tiles == null)
        {
            Debug.LogError("NO TILES IN CLIIFS BIOME");
            return null;
        }

        float noiseValue = cliffsNoiseMap[posWorld.x, posWorld.y];

        //CAN TWEAK NOISE VALUES HERE AS WHAT I WANT
        return biome.tiles[0];
    }

    //Generation for iceberg biom
    private TileBase GetSteppeTile(Vector3Int posWorld, Biome biome)
    {
        if (biome.tiles == null)
        {
            Debug.LogError("NO TILES IN STEPPE BIOME");
            return null;
        }

        float noiseValue = steppeNoiseMap[posWorld.x, posWorld.y];

        //CAN TWEAK NOISE VALUES HERE AS WHAT I WANT
        return biome.tiles[0];
    }

    #endregion
}
