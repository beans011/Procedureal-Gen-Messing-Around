using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WORLD_GENERATOR : MonoBehaviour
{
    #region SINGLETON
    public static WORLD_GENERATOR instance;

    private void Awake()
    {
        CreateSingleton();

        //------START OF WORLD GEN------  REST STARTS IN START() FUNCTION
        //generate world size x and y
        worldSizeX = worldWidth * chunkWidth;
        worldSizeY = worldHeight * chunkHeight;
        GenerateTileMapObejcts();
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

    #region EVENTS
    public static event Action FinishedGeneration;
    public static void OnFinishedGeneration()
    {
        FinishedGeneration?.Invoke();
    }
    #endregion

    private GameObject world;
    private List<GameObject> chunks = new List<GameObject> { };

    [Header("Biome Look Table")]
    [Tooltip("Place the look up table SO here")]
    [SerializeField] private BiomeTable biomeTable;

    [Header("World Dimensions")]
    [Tooltip("World dimensions for how many chunks across and tall")]
    [SerializeField] private int worldWidth = 5;
    [SerializeField] private int worldHeight = 5;
    private int worldSizeX;
    private int worldSizeY;

    [Header("Chunk Dimensions")]
    [SerializeField] private int chunkWidth  = 16 ;
    [SerializeField] private int chunkHeight = 16;

    [Header("Perlin Noise Variables")]
    private float elevationSeed;
    [Tooltip("Controls the zoom level of the noise")]
    [Range(0.001f, 1f)] [SerializeField] private float elevationScale;
    private float[,] elevationMap;

    //falloff variables
    private float falloffPower;
    private float[,] falloffMap; //for pre calculating falloff values maybe has better optimisation then
    private int falloffEdgeMargin; //to ensure smooth transistion to water at all edges

    //island edge values
    private float islandEdgeSeed;
    private float islandEdgeFrequency;
    private float islandEdgeAmplitude;
    private float islandEdgeA; //values to control the falloff curve a - controls steepness of curve
    private float islandEdgeB; // b - controls the bias of curve (how much area is considered land)

    private void Start()
    {


        //checking biome elevation gaps
        for (int i = 1; i < biomeTable.biomes.Count; i++)
        {
            if (biomeTable.biomes[i].elevationMin > biomeTable.biomes[i - 1].elevationMax)
            {
                Debug.LogWarning($"Gap detected between {biomeTable.biomes[i - 1].name} and {biomeTable.biomes[i].name}");
            }
        }

        

        //order is important must have generation after determining the values
        GenerateSeed();
        GenerateFallOff();
        GenerateElevationNoiseMap(); //need to generate the falloff map first        
        StartCoroutine(GenerateChunks());
    }

    //Generates Seeds ----GENERATING GOD SEED---- ALL VARIABLES THAT GO INTO DETERMINING RANDOMNESS IN TERRAIN GO HERE
    private void GenerateSeed()
    {
        //CLAMP VALUES HERE FOR GENERATION

        //GENERATING GOD SEED
        elevationSeed = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        Debug.Log("GEMERATING GOD SEED...\n" + "SEED: " + elevationSeed.ToString());

        //GENERATING GOD FALLOFF
        falloffPower = UnityEngine.Random.Range(1.0f, 5.0f);
        falloffEdgeMargin = UnityEngine.Random.Range(5, 12);
        Debug.Log("GENERATING GOD FALLOFF...\n" + "FALLOFF: " + falloffPower.ToString());
        Debug.Log("GENERATING GOD EDGE MARGIN: " + falloffEdgeMargin.ToString());

        //GENERATING ISLAND EDGE SEED
        islandEdgeSeed = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        islandEdgeFrequency = UnityEngine.Random.Range(0.1f, 0.6f);
        islandEdgeAmplitude = UnityEngine.Random.Range(0.5f, 3.5f);
        islandEdgeA = UnityEngine.Random.Range(2.0f, 6.0f);
        islandEdgeB = UnityEngine.Random.Range(1.5f, 3.5f);
        Debug.Log("Generating GOD ISLAND EDGE SEED...");
        Debug.Log("ISLAND EDGE SEED: " + islandEdgeSeed.ToString());
        Debug.Log("ISLAND EDGE FREQUENCY: " + islandEdgeFrequency.ToString());
        Debug.Log("ISLAND EDGE AMPLITUDE: " + islandEdgeAmplitude.ToString());
        Debug.Log("ISLAND EDGE A: " + islandEdgeA.ToString());
        Debug.Log("ISLAND EDGE B: " + islandEdgeB.ToString());
    }

    //Generates the tilemap objects 
    private void GenerateTileMapObejcts()
    {
        world = new GameObject("WORLD"); //create gameobject that all chunks will be under        
    }

    //Fills in the chunks 
    private IEnumerator GenerateChunks()
    {
        for (int cx = 0; cx < worldWidth; cx++) //Start of iterating through chunks
        {
            for (int cy = 0; cy < worldHeight; cy++)
            {
                GameObject chunk = CreateNewChunk(cx, cy);                

                for (int x = 0; x < chunkWidth; x++) 
                {
                    for (int y = 0; y < chunkHeight; y++) 
                    {                       
                        int worldX = cx * chunkWidth + x;
                        int worldY = cy * chunkHeight + y;

                        Vector3Int tilePos = new Vector3Int(x, y, 0);

                        //Get the elevation value                        
                        float elevation = elevationMap[worldX, worldY];                                

                        Biome chosenBiome = GetTileData(elevation);
                        TILE_MANAGER.instance.PlaceTile(chosenBiome, tilePos, chunk);
                    }
                }

                yield return null;
            }
        }

        FinishedGenerationHere();
    }

    //clamp the elevation so all values are 0 - 1
    private float ClampElevation(float givenElevation)
    {
        if (givenElevation < 0)
        {
            return 0.0f;
        }
        if (givenElevation > 1.0f)
        { 
            return 1.0f;
        }
        
        return givenElevation;
    }

    //Gets the correct tile determined by the generated noise
    Biome GetTileData(float elevation)
    {
        foreach (Biome biome in biomeTable.biomes)
        {
            Biome biomeCheck = biome;

            if (elevation >= biomeCheck.elevationMin && elevation < biomeCheck.elevationMax + 0.01)
            {
                return biomeCheck;
            }
        }

        Debug.Log(elevation);
        Debug.LogError("NO VALID BIOME FOUND - CHECK THE BIOME TABLE");
        return null;
    }

    //Generates a noise map for elevation
    private void GenerateElevationNoiseMap()
    {
        elevationMap = new float[worldSizeX, worldSizeY];

        for (int x = 0; x < worldSizeX; x++) 
        {
            for (int y = 0; y < worldSizeY; y++) 
            {
                float elevation = Mathf.PerlinNoise((x + elevationSeed) * elevationScale, (y + elevationSeed) * elevationScale); //change this line when want to change elevation stuff
                elevation = elevation - falloffMap[x, y];
                elevation = ClampElevation(elevation);

                elevationMap[x, y] = elevation;
            }
        }
    }

    //Generates a falloff map to make water at the edges
    private void GenerateFallOff()
    {
        falloffMap = new float[worldSizeX, worldSizeY];

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                float nx = (float)x / (float)worldSizeX * 2f - 1f;
                float ny = (float)y / (float)worldSizeY * 2f - 1f;

                float distance = Mathf.Sqrt(nx * nx + ny * ny);
                float baseFalloff = FalloffCurve(distance) * falloffPower;

                float edgeNoise = Mathf.PerlinNoise((x + islandEdgeSeed) * islandEdgeFrequency, (y + islandEdgeSeed) * islandEdgeFrequency) * islandEdgeAmplitude;

                float islandFalloff = Mathf.Clamp01(baseFalloff - edgeNoise);

                int distToEdge = Mathf.Min(x, y, worldSizeX - 1 - x, worldSizeY - 1 - y);

                if (distToEdge < falloffEdgeMargin) //blend into water like gozilla return to ocean
                {
                    float t = distToEdge / (float)falloffEdgeMargin;
                    falloffMap[x, y] = Mathf.Lerp(1f, islandFalloff, t); //1 = full falloff (water)
                }
                else
                {
                    falloffMap[x, y] = islandFalloff;
                }
            }
        }
    }
    
    //Controls how strong the fall off acts
    private float FalloffCurve(float value)
    {
        value = Mathf.Clamp01(value); //was causing NaN to be returned need to keep it under control
        return Mathf.Pow(value, islandEdgeA) / (Mathf.Pow(value, islandEdgeA) + Mathf.Pow(islandEdgeB - islandEdgeB * value, islandEdgeA));
    }

    //Generates a new chunk
    private GameObject CreateNewChunk(int chunkX, int chunkY)
    {
        //Generate parent chunk obj
        GameObject chunk = new GameObject("Chunk_" +  chunkX + "_" + chunkY);
        chunk.AddComponent<Grid>(); //FORGOT THIS AND ITS VERY IMPORTANT
        chunk.transform.parent = world.transform;

        //Generate walkable chunk
        GameObject walkableChunk = new GameObject("Walkable_Chunk_" + chunkX + "_" + chunkY);
        walkableChunk.transform.parent = chunk.transform;
        Tilemap walkableTilemap = walkableChunk.AddComponent<Tilemap>();
        walkableChunk.AddComponent<TilemapRenderer>();

        //Generate non walkable chunk
        GameObject nonWalkableChunk = new GameObject("Non_Walkable_Chunk_" + chunkX + "_" + chunkY);
        nonWalkableChunk.transform.parent = chunk.transform;
        Tilemap nonWalkableTilemap = nonWalkableChunk.AddComponent<Tilemap>();
        nonWalkableChunk.AddComponent<TilemapRenderer>();
        nonWalkableChunk.AddComponent<TilemapCollider2D>();

        //Turn off renderer and turn on later may help optimise
        walkableChunk.GetComponent<TilemapRenderer>().enabled = false; 
        nonWalkableChunk.GetComponent<TilemapRenderer>().enabled = false;
        nonWalkableChunk.GetComponent<TilemapCollider2D>().enabled = false;

        //Set position of new chunks
        chunk.transform.position = new Vector3(chunkX * chunkWidth, chunkY * chunkHeight, 0);

        //Add chunk to list for easy access
        chunks.Add(chunk);
        return chunk;
    }

    //Gets the list of chunks
    public List<GameObject> GetChunks()
    {
        return chunks;
    }

    //Stuff to run on generation finish
    private void FinishedGenerationHere()
    {
        if (CHUNK_MANAGER.instance.turnOnOcclusionCulling == false) //for options i guess
        {
            foreach (GameObject chunk in chunks) //Re-enable the kack
            {
                TilemapRenderer walkableTilemap = chunk.transform.GetChild(0).GetComponent<TilemapRenderer>();
                TilemapRenderer nonWalkableTilemap = chunk.transform.GetChild(1).GetComponent<TilemapRenderer>();
                TilemapCollider2D collider2d = chunk.transform.GetChild(1).GetComponent<TilemapCollider2D>();

                walkableTilemap.enabled = true;
                nonWalkableTilemap.enabled = true;
                collider2d.enabled = true;
            }
        }      

        FinishedGeneration(); //This be last ALWAYS
    }

    //Gets the center coords of the map
    public Vector3 GetCenterPos()
    {
        Vector3 centerPos = new Vector3(worldSizeX / 2, worldSizeY / 2, 0);
        return centerPos;
    }

    //Gets the chunk width and height
    public int GetChunkWidth() { return chunkWidth; }
    public int GetChunkHeight() { return chunkHeight; }
}
