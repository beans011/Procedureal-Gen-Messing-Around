using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileData
{
    public Biome biome;
    public bool isWalkable;
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

    private TileData[] tileDataArray; //where all the tileData is stored

    //Declares the tileDataArray
    public void DeclareTileDataArray(int arrayLength)
    {
        tileDataArray = new TileData[arrayLength];
    }

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

        TileBase tile = passedBiome.tiles[UnityEngine.Random.Range(0, passedBiome.tiles.Length)];

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
        Vector3Int posInt = new Vector3Int(
                (int)(pos.x + (int)chunk.transform.position.x),
                (int)(pos.y + (int)chunk.transform.position.y),
                0
            );

        //Create the data set 
        TileData newTileData = new TileData
        {
            biome = passedBiome,
            isWalkable = passedBiome.isTileNotWalkable,
            tile = tilePassed,
            tileWorldPos = posInt
        };

        tileDataArray.Append(newTileData);
    }
}
