using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Collider), typeof(Renderer))]
public class CellWall : MonoBehaviour
{
    //[SerializeField]
    //private WallID _wallID;

    private Collider _wallCollider;
    private Renderer _wallRenderer;

    private void Awake()
    {
        _wallCollider = GetComponent<Collider>();
        _wallRenderer = GetComponent<Renderer>();
        //SetWallID();
    }

    //public WallID GetWallID()
    //{ 
    //    return _wallID; 
    //}

    public void SetWallMaterial(Material wallMaterial)
    {
        _wallRenderer.material = wallMaterial;
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
    //// dead
    //private void SetWallID()
    //{
    //    if (transform.position.z > 0)
    //    {
    //        if (transform.position.x == 0)
    //            _wallID = WallID.PosZ;
    //        else if (transform.position.x < 15)
    //            _wallID = WallID.PosZ_PosX;
    //    }
    //    if (transform.position.x > 0)
    //    {
    //        if (transform.position.z == 0)
    //            _wallID = WallID.PosX_PosZ;
    //        else if (transform.position.z < 15)
    //            _wallID = WallID.PosX;
    //    }
    //    if (transform.position.z < 0)
    //    {
    //        if (transform.position.x == 0)
    //            _wallID = WallID.NegZ;
    //        else if (transform.position.x < 15)
    //            _wallID = WallID.NegZ_PosX;
    //    }
    //    if (transform.position.x < 0)
    //    {
    //        if (transform.position.z == 0)
    //            _wallID = WallID.NegX;
    //        else if (transform.position.z < 15)
    //            _wallID = WallID.NegX_PosZ;
    //    }
    //}

    public float GetWallX()
    {
        return transform.localPosition.x;
    }

    public float GetWallZ()
    {
        return transform.localPosition.z;
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
