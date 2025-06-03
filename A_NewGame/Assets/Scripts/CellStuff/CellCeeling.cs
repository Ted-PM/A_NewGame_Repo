using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Collider), typeof(Renderer))]
public class CellCeeling : MonoBehaviour
{
    private Renderer _ceelingRenderer;
    private Collider _ceelingCollider;

    private void Awake()
    {
        _ceelingCollider = GetComponent<Collider>();
        _ceelingRenderer = GetComponent<Renderer>();
    }

    public void ChangeCeelingMat(Material mat)
    {
        _ceelingRenderer.material = mat;
    }

    public void DisableCeelingRenderer()
    {
        _ceelingRenderer.enabled = false;
    }

    public void EnableCeelingRenderer()
    {
        _ceelingRenderer.enabled = true;
    }

    public void DisableCeeling()
    {
        _ceelingRenderer.enabled = false;
        _ceelingCollider.enabled = false;
    }
}
