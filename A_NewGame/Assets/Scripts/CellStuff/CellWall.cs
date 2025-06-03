using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Collider))]
public class CellWall : MonoBehaviour
{
    //[SerializeField]
    //private WallID _wallID;

    private Collider _wallCollider;
    //private Renderer _wallRenderer;

    [SerializeField]
    private GameObject _wallProps;
    [SerializeField]
    private GameObject _wallPlane;
    private Renderer _planeRenderer;

    private void Awake()
    {
        _wallCollider = GetComponent<Collider>();
        if (!_wallPlane.TryGetComponent<Renderer>(out _planeRenderer))
            Debug.Log("No Plane Renderer");
    }

    public void SetWallMaterial(Material wallMaterial)
    {
        if (_planeRenderer != null )
            _planeRenderer.material = wallMaterial;
    }
    public void SetWallColor(Color wallColor)
    {
        if (_planeRenderer != null)
            _planeRenderer.material.color = wallColor;
    }

    public void DisableWall()
    {
        _wallCollider.enabled = false;

        if (_wallPlane != null)
            _wallPlane.SetActive(false);// = false;
        if (_wallProps != null)
            _wallProps.SetActive(false);
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
        if (_planeRenderer != null)
            _planeRenderer.enabled = false;
        if (_wallProps != null)
            _wallProps.SetActive(false);
    }
    public void EnableRenderer()
    {
        if (_planeRenderer != null)
            _planeRenderer.enabled = true;
        if (_wallProps != null)
            _wallProps.SetActive(true);
    }
    public void SwitchRendererStatus()
    {
        if (_planeRenderer != null)
            _planeRenderer.enabled = !_planeRenderer.enabled;
        if (_wallProps != null)
            _wallProps.SetActive(!_wallProps.activeSelf);
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
