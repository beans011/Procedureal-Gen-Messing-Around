using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.transform.position = WORLD_GENERATOR.instance.GetCenterPos();
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        rb.linearVelocity = moveInput * moveSpeed;

        //for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2Int newOne = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);
            Debug.Log(TILE_MANAGER.instance.GetSpecificTileData(newOne).biome.name);
            
            List<TileData> tileData = TILE_MANAGER.instance.GetSurroundingTiles(newOne);

            foreach (TileData tile in tileData) 
            { 
                Debug.Log(tile.isNotWalkable);
            }
        }
    }
}
