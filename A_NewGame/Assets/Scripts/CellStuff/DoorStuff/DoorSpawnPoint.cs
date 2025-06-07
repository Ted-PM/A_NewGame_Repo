using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DoorSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private DoorTypes _doorType;
    //[SerializeField]
    //private CellDoorWall _doorWay;

    private bool _hasDoor = false;
    private float _doorStatus = 0f;

    private GameObject _newDoor = null;
    //private GameObject _newDoorWay = null;

    //[SerializeField]
    //private Collider _doorSpawnCollider;
    //[SerializeField]
    //private Collider _doorSpawnBottomCollider;

    private void FixedUpdate()
    {
        //if (_newDoor != null && DoorDisabled())
        //{
        //    //_newDoorWay.GetComponent<CellDoorWall>().doorWayDisabled = false;
        //    DisableDoor();
        //    //StartCoroutine(WaitThenEnableCollider());
        //}
    }

    //private bool DoorDisabled()
    //{
    //    if (_newDoor == null)
    //    {
    //        return false;
    //    }
    //    //if (_newDoorWay == null)
    //    //    return false;
    //    return _doorWay.DoorIsDisabled();
    //}

    public void DisableDoor()
    {
        if (_hasDoor &&_newDoor != null && _newDoor.activeInHierarchy)
        {
            //Debug.Log("Disabling Doorway");
                //Debug.Log("Disabling Door");
            //_doorWay.DisableDoor();
            _doorStatus = _newDoor.GetComponent<CellDoor>().GetDoorCurrentPosition();
            _newDoor.GetComponent<CellDoor>().DisableDoor();
            _newDoor.SetActive(false);
            _newDoor = null;
            
            //StartCoroutine(WaitThenEnableCollider());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Door spawn point collison is null!!");
            return;
        }
        else if (_hasDoor && (_newDoor == null || !_newDoor.activeInHierarchy))
        {
            //Debug.Log("Collision w enemy at: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
            //Debug.Log("Distance between player & enemy: " + Vector3.Distance(transform.position, other.gameObject.transform.position));
            //_doorSpawnCollider.enabled = false;
            _newDoor = null;
            SpawnDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Door Wall other is null!!");
        }
        DisableDoor();
        //if (other.gameObject.transform.position == _playerTransform.transform.position)
        //else
        //Debug.Log("Door Wall other is not player");
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other == null || other.gameObject == null)
    //    {
    //        Debug.LogError("Door spawn point collison is null!!");
    //        return;
    //    }
    //    else if (_newDoorWay != null && _newDoorWay.activeInHierarchy)
    //    {
    //        Debug.Log("Disabling Doorway");
    //        _newDoorWay.SetActive(false);
    //        _newDoorWay = null;
    //        if (_hasDoor && _newDoor != null)
    //        {
    //            Debug.Log("Disabling Door");
    //            _doorStatus = _newDoor.GetComponent<CellDoor>().GetDoorCurrentPosition();
    //            _newDoor.GetComponent<CellDoor>().DisableDoor();
    //            _newDoor.SetActive(false);
    //            _newDoor = null;
    //        }
    //    }
    //}
    //private void SpawnDoorWay()
    //{
    //    _newDoorWay = DoorSpawner.Instance.GetDoorWayFromPool((int)_doorWayType);

    //    if (_newDoorWay == null)
    //    {
    //        Debug.Log("DoorWay pool empty!!");
    //        return;
    //    }
    //    else
    //    {
    //        _newDoorWay.transform.position = transform.position;
    //        _newDoorWay.transform.rotation = transform.rotation;
    //        _newDoorWay.SetActive(true);
    //        if (_hasDoor)
    //            SpawnDoor();
    //    }
    //}

    private void SpawnDoor()
    {
        _newDoor = DoorSpawner.Instance.GetDoorFromPool((int)_doorType);

        if ( _newDoor == null)
        {
            Debug.Log("Door pool empty!!");
            return;
        }
        else
        {
            _newDoor.transform.position = transform.position + new Vector3(0, 3.19f, 0f);
            _newDoor.transform.rotation = transform.rotation;
            _newDoor.SetActive(true);
            _newDoor.GetComponent<CellDoor>().EnableDoor(_doorStatus);
            //_newDoorWay.GetComponent<CellDoorWall>().AddDoorObject(_newDoor);
        }
    }


    public void DoorWayHasDoor(bool hasDoor)
    {
        _hasDoor = hasDoor;
    }

    //private IEnumerator WaitThenEnableCollider()
    //{
    //    yield return new WaitForSeconds(1f);
    //    _doorSpawnCollider.enabled = true;
    //}
}
