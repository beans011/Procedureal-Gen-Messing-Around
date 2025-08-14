using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class TileData
{
    public Biome biome;
    public bool isNotWalkable;
    public TileBase tile;
    public Vector3 tileWorldPos;
}

public class TILE_MANAGER : MonoBehaviour
{
    #region SINGLETON
    public static TILE_MANAGER instance;

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

    private Dictionary<Vector2Int, TileData> tileDataMap = new Dictionary<Vector2Int, TileData>(); //for very fast look ups at runtime

    //offset stuff for getting surrounding tiles
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1),
        new Vector2Int( 0, -1),                        new Vector2Int( 0,  1),
        new Vector2Int( 1, -1), new Vector2Int( 1, 0), new Vector2Int( 1,  1)
    };

    //Determines which grid to place tile in and places it
    public void PlaceTile(Biome passedBiome, Vector3Int pos, GameObject chunk)
    {
        if (passedBiome == null)
        {
            Debug.LogError("NO BIOME FOUND - - CHECK IF ONE HAS BEEN PASSED THROUGH");
            return;
        }

        //Get tilemaps and kack
        Tilemap walkableTilemap = chunk.transform.GetChild(0).GetComponent<Tilemap>();
        Tilemap nonWalkableTilemap = chunk.transform.GetChild(1).GetComponent<Tilemap>();

        //Cheeky validation
        if (walkableTilemap == null)
        {
            Debug.LogError("NO WALKABLE TILEMAP FOUND - - CHECK IF ONE HAS BEEN PASSED THROUGH");
            return;
        }
        if (nonWalkableTilemap == null)
        {
            Debug.LogError("NO NON-WALKABLE TILEMAP FOUND - - CHECK IF ONE HAS BEEN PASSED THROUGH");
            return;
        }

        TileBase tile = BIOME_MANAGER.instance.GetTileForBiome(passedBiome, pos, chunk);

        if (passedBiome.isTileNotWalkable)
        {
            nonWalkableTilemap.SetTile(pos, tile);
        }
        else
        {
            walkableTilemap.SetTile(pos, tile);
        }

        //Adding the tile to all powerful data set
        AddTileToDataMap(passedBiome, pos, chunk, tile);
    }

    //Adds the tile to the tileDataMap
    private void AddTileToDataMap(Biome passedBiome, Vector3 pos, GameObject chunk, TileBase tilePassed)
    {
        //Work out world pos
        Vector3Int posWorld = new Vector3Int(
                (int)(pos.x + (int)chunk.transform.position.x),
                (int)(pos.y + (int)chunk.transform.position.y),
                0
            );

        //Create the data set 
        TileData newTileData = new TileData
        {
            biome = passedBiome,
            isNotWalkable = passedBiome.isTileNotWalkable,
            tile = tilePassed,
            tileWorldPos = posWorld
        };

        //cheeky validation
        if (newTileData == null)
        {
            Debug.LogError("TILE DATA IS NULL");
            return;
        }

        tileDataMap[new Vector2Int(posWorld.x, posWorld.y)] = newTileData; //add to dictionary
    }

    //Return tile data for a specific tile - using world co-ord
    public TileData GetSpecificTileData(Vector2Int pos)
    {
        if (tileDataMap.TryGetValue(pos, out TileData tile))
        {
            return tile;
        }

        Debug.LogError("NO TILE FOUND - - CHECK DICTIONARY WORKS");
        return null;
    }

    //Returns a list of tiles that surround a chosen tiles
    public List<TileData> GetSurroundingTiles(Vector2Int pos) 
    { 
        List<TileData> surroundingTiles = new List<TileData>();

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborPos = pos + dir;

            if (tileDataMap.TryGetValue(neighborPos, out TileData tile))
            {
                surroundingTiles.Add(tile);
            }
        }

        return surroundingTiles;
    }
}
