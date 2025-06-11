using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Transform), typeof(Collider))]
public class CellWall : MonoBehaviour
{
    //[SerializeField]
    //private WallID _wallID;

    private Collider _wallCollider;
    //private Renderer _wallRenderer;

    [SerializeField]
    private List<GameObject> _wallProps;
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

        if (_wallPlane != null && _planeRenderer != null)
            _planeRenderer.enabled = false;
        if (_wallProps != null)
            DisableWallProps();
    }

    public void EnableWall()
    {
        _wallCollider.enabled = true;

        if (_wallPlane != null && _planeRenderer != null)
            _planeRenderer.enabled = true;
        if (_wallProps != null)
            EnableWallProps();
    }

    public void DestroyWallProps()
    {
        if ( _wallProps == null)
        {
            return;
        }

        for (int i = _wallProps.Count - 1; i >= 0; i--)
        {
            if (_wallProps[i] != null)
                Destroy(_wallProps[i]);
        }
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
            DisableWallProps();
    }
    public void EnableRenderer()
    {
        if (_planeRenderer != null)
            _planeRenderer.enabled = true;
        if (_wallProps != null)
            EnableWallProps();
    }
    public void SwitchRendererStatus()
    {
        if (_planeRenderer != null)
            _planeRenderer.enabled = !_planeRenderer.enabled;
        if (_wallProps != null)
            SwitchWallPropsStatus();
    }

    private void DisableWallProps()
    {
        foreach (GameObject prop in _wallProps)
        {
            if (prop != null)
                prop.SetActive(false);
        }
    }

    private void EnableWallProps()
    {
        foreach (GameObject prop in _wallProps)
        {
            if (prop != null)
                prop.SetActive(true);
        }
    }

    private void SwitchWallPropsStatus()
    {
        foreach (GameObject prop in _wallProps)
        {
            if (prop != null)
                prop.SetActive(!prop.activeSelf);
        }
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
