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

    public void ChangeFloorMat(Material mat)
    {
        _floorRenderer.material = mat;
    }

    public void DisableFloor()
    {
        _floorRenderer.enabled = false;
        _floorCollider.enabled = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
