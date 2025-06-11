using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Collider), typeof(Renderer))]
public class CellFloor : MonoBehaviour
{
    private Renderer _floorRenderer;
    private Collider _floorCollider;

    private void Awake()
    {
        _floorCollider = GetComponent<Collider>();
        _floorRenderer = GetComponent<Renderer>();
    }

    public void DisableFloorRenderer()
    {
        _floorRenderer.enabled = false;
    }

    public void EnableFloorRenderer()
    {
        _floorRenderer.enabled = true;
    }

    public void ChangeFloorMat(Material mat)
    {
        _floorRenderer.material = mat;
    }

    public void DisableFloor()
    {
        _floorRenderer.enabled = false;
        _floorCollider.enabled = false;
    }

    public void EnableFloor()
    {
        _floorCollider.enabled = true;
        _floorRenderer.enabled = true;
    }

}
