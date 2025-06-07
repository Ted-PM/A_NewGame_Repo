using UnityEngine;
using System.Collections;
//[RequireComponent(typeof(Transform), typeof(Collider), typeof(Renderer))]
public class CellDoorWall : MonoBehaviour
{
    //[SerializeField]
    //private GameObject cellDoorWallPrefab;

    //[SerializeField]
    //private GameObject _doorPrefab;
    //private GameObject _door;

    //private GameObject _playerTransform;
    //private bool _playerFound = false;

    //public float maxDistanceBeforeDeSpawn = 50f;

    //private bool _doorDisabled = false;

    //private void Start()
    //{
    //    StartCoroutine(TryFindPlayer());
    //    //if (_doorPrefab!= null)
    //    //    AddDoor();
    //}

    //private void OnEnable()
    //{
    //    if (!_playerFound)
    //        StartCoroutine(TryFindPlayer());
    //    //_doorDisabled = false;
    //}

    //public void DisableDoor()
    //{
    //    _doorDisabled = false;
    //}

    //private void FixedUpdate()
    //{
    //    if (_playerFound && PlayerOutOfRange())
    //    {
    //        _doorDisabled = true;
    //    }
    //}

    //public bool DoorIsDisabled()
    //{
    //    return _doorDisabled;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other == null ||  other.gameObject == null)
    //    {
    //        Debug.LogError("Door Wall other is null!!");
    //    }

    //    //if (other.gameObject.transform.position == _playerTransform.transform.position)
    //    _doorDisabled = true;
    //    //else
    //        //Debug.Log("Door Wall other is not player");
    //}

    //public void DisableDoorWay()
    //{
    //    doorWayDisabled = false;
    //    //if (_door != null)
    //    //    _door.GetComponent<CellDoor>().DisableDoor();
    //}

    //private bool PlayerOutOfRange()
    //{
    //    //bool result = (Vector3.Distance(transform.position, _playerTransform.gameObject.transform.position) >= maxDistanceBeforeDeSpawn) ||
    //        //(transform.position.y -_playerTransform.gameObject.transform.position.y >= 7);
    //    return (Vector3.Distance(transform.position, _playerTransform.gameObject.transform.position) >= maxDistanceBeforeDeSpawn);
    //    //return result;
    //}

    //public void AddDoorObject(GameObject door)
    //{
    //    _door = door;
    //}

    //private IEnumerator TryFindPlayer()
    //{
    //    yield return null;

    //    while (Camera.allCamerasCount <= 0)
    //    {
    //        //yield return new WaitForSeconds(1f);
    //        yield return new WaitForFixedUpdate();
    //    }

    //    if (Camera.allCamerasCount == 1)
    //    {
    //        _playerTransform = FindAnyObjectByType<PlayerController>().gameObject;
    //        _playerFound = true;
    //        //_playerPos = _playerTransform.transform.position;
    //    }
    //}
    //private void AddDoor()
    //{
    //    Instantiate(cellDoorWallPrefab, transform.parent);
    //    _door = GetComponent<CellDoor>();

    //}
    //private Renderer _doorRenderer;
    //private Collider _doorCollider;

}
