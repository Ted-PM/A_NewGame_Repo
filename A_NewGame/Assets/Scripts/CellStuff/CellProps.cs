using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(Collider))]
public class CellProps : MonoBehaviour
{
    //private Collider _propCollider;

    [SerializeField]
    private List<Renderer> _propRenderers;

    private void Awake()
    {
        //_propCollider = GetComponent<Collider>();
        //_propCollider.excludeLayers = ~0 - LayerMask.GetMask("PlayerLayer");
    }

    private void Start()
    {
        //DisableRenderers();
    }
    public void DisableProps()
    {
        //_propCollider.enabled = false;
        DisableRenderers();
    }
    public void DisableRenderers()
    {
        if (_propRenderers == null || _propRenderers.Count <= 0)
            return;

        foreach (Renderer renderer in _propRenderers)
        {
            if (renderer != null && renderer.enabled)
                renderer.enabled = false;
        }
    }

    public void EnableProps()
    {
        //_propCollider.enabled = true;
        EnableRenderers();
    }

    public void EnableRenderers()
    {
        if (_propRenderers == null || _propRenderers.Count <= 0)
            return;

        foreach (Renderer renderer in _propRenderers)
        {
            if (renderer != null && !renderer.enabled)
                renderer.enabled = true;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other == null || other.gameObject ==null)
    //    {
    //        Debug.LogWarning("Prop collision is null!!");
    //        return;
    //    }

    //    //EnableRenderers();
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other == null || other.gameObject == null)
    //    {
    //        Debug.LogWarning("Prop collision is null!!");
    //        return;
    //    }

    //    //DisableRenderers();
    //}
}
