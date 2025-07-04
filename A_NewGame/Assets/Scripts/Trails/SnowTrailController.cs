using UnityEngine;

public class SnowTrailController : MonoBehaviour
{
    public Camera trailCamera;
    public Material snowMaterial; // or use MaterialPropertyBlock
    public Vector2 renderSize = new Vector2(20, 20); // area covered by camera
    private Vector2 offset;

    void LateUpdate()
    {
        Vector3 playerPos = transform.position;

        // Snap the camera to the center of the player (or grid if needed)
        Vector3 camPos = new Vector3(playerPos.x, trailCamera.transform.position.y, playerPos.z);
        trailCamera.transform.position = camPos;

        offset = new Vector2(camPos.x - renderSize.x / 2, camPos.z - renderSize.y / 2);

        // Send offset and size to the shader
        snowMaterial.SetVector("_WorldOffset", offset);
        snowMaterial.SetVector("_WorldSize", renderSize);
    }
}
