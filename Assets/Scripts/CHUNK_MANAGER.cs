using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CHUNK_MANAGER : MonoBehaviour
{
    #region SINGLETON
    public static CHUNK_MANAGER instance;

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

    private List<GameObject> chunks = new();
    private List<Vector2> chunkCenters = new();

    [Header("Optimisation Setting")]
    [Tooltip("true = use it, false = not use it")] public bool turnOnOcclusionCulling; //true = use it, false = not use it

    private void Start()
    {
        WORLD_GENERATOR.FinishedGeneration += GetListOfChunks;
    }

    //Gets list of chunks and iterates through to render them after generation is finished
    private void GetListOfChunks()
    {
        chunks = WORLD_GENERATOR.instance.GetChunks();
        SetupChunkCenters();
    }

    //Get chunk info for other scripts
    public List<GameObject> GetListChunks()
    {
        return chunks;
    }

    //Get chosen index chunk obj
    public GameObject GetIndexChunk(int index)
    {
        return chunks[index];
    }

    //Make list of chunk centers in coords for calculating shit
    private void SetupChunkCenters()
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            Vector2 tempChunkCenter = new Vector2(chunks[i].transform.position.x + (WORLD_GENERATOR.instance.GetChunkWidth() / 2f), chunks[i].transform.position.y + (WORLD_GENERATOR.instance.GetChunkHeight() / 2f));
            chunkCenters.Add(tempChunkCenter);
        }
    }

    //Get list of chunk centers
    public List<Vector2> GetListChunkCenters()
    {
        return chunkCenters;
    }

    //Get chosen index of chunk centers
    public Vector2 GetIndexChunkCenters(int index)
    {
        return chunkCenters[index];
    }
}
