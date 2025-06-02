using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Collider))]
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
        //if (!TryGetComponent<Renderer>(out _wallRenderer))
            //con;
            //Debug.Log("No Wall Renderer");
    }

    public void SetWallMaterial(Material wallMaterial)
    {
        if (_wallRenderer != null) 
            _wallRenderer.material = wallMaterial;

        Renderer wallRenderer;
        if (_wallPlane != null && _wallPlane.TryGetComponent<Renderer>(out wallRenderer))
            wallRenderer.material = wallMaterial;
        //try
        //{
            
        //}
        //catch
        //{
        //    Debug.LogError("Wall Plane Not Found!!");
        //}
    }
    public void SetWallColor(Color wallColor)
    {
        if (_wallRenderer != null)
            _wallRenderer.material.color = wallColor;

        Renderer wallRenderer;
        if (_wallPlane != null && _wallPlane.TryGetComponent<Renderer>(out wallRenderer))
            wallRenderer.material.color = wallColor;

        //try
        //{
        //    _wallPlane.GetComponent<Renderer>().material.color = wallColor;
        //}
        //catch
        //{
        //    Debug.LogError("Wall Plane Not Found!!");
        //}
    }

    public void DisableWall()
    {
        _wallCollider.enabled = false;
        if (_wallRenderer != null)
            _wallRenderer.enabled = false;
        if (_wallPlane != null)
            _wallPlane.SetActive(false);// = false;
    }

    //public void EnableW

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
        if (_wallRenderer != null && _wallRenderer.enabled == true)
            _wallRenderer.enabled = false;
    }
    public void EnableRenderer()
    {
        if (_wallRenderer != null && _wallRenderer.enabled == false)
            _wallRenderer.enabled = true;
    }
    public void SwitchRendererStatus()
    {
        if (_wallRenderer != null)
            _wallRenderer.enabled = !_wallRenderer.enabled;
    }
    
    public Vector3 GetWallPosition()
    {
        return transform.position;
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
