using UnityEngine;
using UnityEngine.Tilemaps;

public class OcclusionCulling : MonoBehaviour
{
    private Plane[] cameraVieport;
    private Camera camera;
    private Vector3 playerPos;

    [SerializeField] private float chunkRenderDistance;

    private void Start()
    {
        camera = Camera.main;
        playerPos = gameObject.transform.position;
    }

    private void Update()
    {
        if (CHUNK_MANAGER.instance.turnOnOcclusionCulling == false) { return; } //for having options i guess

        playerPos = gameObject.transform.position;
        cameraVieport = GeometryUtility.CalculateFrustumPlanes(camera);

        for (int i = 0; i < CHUNK_MANAGER.instance.GetListChunks().Count; i++) 
        {
            float distance = Vector3.Distance(playerPos, CHUNK_MANAGER.instance.GetIndexChunkCenters(i));
            bool shouldRender = distance > chunkRenderDistance;

            if (distance > chunkRenderDistance) 
            {
                SetChunkActive(CHUNK_MANAGER.instance.GetIndexChunk(i), false);
                continue;
            }

            Bounds chunkBounds = new Bounds(
                CHUNK_MANAGER.instance.GetIndexChunkCenters(i), 
                new Vector2(WORLD_GENERATOR.instance.GetChunkWidth(), WORLD_GENERATOR.instance.GetChunkHeight())
                );

            bool inView = GeometryUtility.TestPlanesAABB(cameraVieport, chunkBounds);
            SetChunkActive(CHUNK_MANAGER.instance.GetIndexChunk(i), inView);
        }
    }

    private void SetChunkActive(GameObject chunk, bool active)
    {
        //turn off the renderer for occlusion culling effect
        foreach (var renderer in chunk.GetComponentsInChildren<TilemapRenderer>())
        {
            renderer.enabled = active;
        }

        //turn off the collider may be cool as well
        foreach (var renderer in chunk.GetComponentsInChildren<TilemapCollider2D>())
        {
            renderer.enabled = active;
        }
    }
}
