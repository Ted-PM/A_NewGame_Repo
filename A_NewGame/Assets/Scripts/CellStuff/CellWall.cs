using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Collider), typeof(Renderer))]
public class CellWall : MonoBehaviour
{
    //[SerializeField]
    //private WallID _wallID;

    private Collider _wallCollider;
    private Renderer _wallRenderer;
    
    [SerializeField]
    private GameObject _wallPlane;

    private void Awake()
    {
        _wallCollider = GetComponent<Collider>();
        _wallRenderer = GetComponent<Renderer>();
    }

    public void SetWallMaterial(Material wallMaterial)
    {
        _wallRenderer.material = wallMaterial;
        
        try
        {
            _wallPlane.GetComponent<Renderer>().material = wallMaterial;
        }
        catch
        {
            Debug.LogError("Wall Plane Not Found!!");
        }
    }
    public void SetWallColor(Color wallColor)
    {
        _wallRenderer.material.color = wallColor;

        try
        {
            _wallPlane.GetComponent<Renderer>().material.color = wallColor;
        }
        catch
        {
            Debug.LogError("Wall Plane Not Found!!");
        }
    }

    public void DisableCollider()
    {
        _wallCollider.enabled = false;
    }
    public void EnableCollider()
    {
        _wallCollider.enabled = true;
    }
    public void SwitchColliderStatus()
    {
        _wallCollider.enabled = !_wallCollider.enabled;
    }

    public void DisableRenderer()
    {
        _wallRenderer.enabled = false;
    }
    public void EnableRenderer()
    {
        _wallRenderer.enabled = true;
    }
    public void SwitchRendererStatus()
    {
        _wallRenderer.enabled = !_wallRenderer.enabled;
    }
   
    public float GetWallX()
    {
        return transform.localPosition.x;
    }

    public float GetWallZ()
    {
        return transform.localPosition.z;
    }

    public float GetWallY()
    {
        return transform.localPosition.y;
    }



}

// ID given starting from front (PosZ) then clockwise
//public enum WallID
//{
//    PosZ,       // 0
//    PosZ_PosX,  // 1
//    PosX_PosZ,  // 2
//    PosX,       // 3
//    NegZ_PosX,  // 4
//    NegZ,       // 5
//    NegX,       // 6
//    NegX_PosZ,   // 7
//    None        // 8
//}
