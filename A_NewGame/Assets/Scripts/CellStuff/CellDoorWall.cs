using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Collider), typeof(Renderer))]
public class CellDoorWall : MonoBehaviour
{
    [SerializeField]
    private GameObject cellDoorWallPrefab;

    [SerializeField]
    private GameObject _doorPrefab;
    private CellDoor _door;

    private void Start()
    {
        if (_doorPrefab!= null)
            AddDoor();
    }
    private void AddDoor()
    {
        Instantiate(cellDoorWallPrefab, transform.parent);
        _door = GetComponent<CellDoor>();

    }
    //private Renderer _doorRenderer;
    //private Collider _doorCollider;

}
