using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DoorSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private DoorWayTypes _doorWayType;

    private bool _hasDoor = false;
    private float _doorStatus = 0f;

    private GameObject _newDoor = null;
    private GameObject _newDoorWay = null;

    [SerializeField]
    private Collider _doorSpawnCollider;


    private void FixedUpdate()
    {
        if (_newDoorWay != null && DoorWayDisabled())
        {
            //_newDoorWay.GetComponent<CellDoorWall>().doorWayDisabled = false;
            DisableDoorWay();
            //StartCoroutine(WaitThenEnableCollider());
        }
    }

    private bool DoorWayDisabled()
    {
        if (_newDoorWay == null)
            return false;
        return _newDoorWay.GetComponent<CellDoorWall>().DoorWayIsDisabled();
    }

    private void DisableDoorWay()
    {
        if (_newDoorWay != null && _newDoorWay.activeInHierarchy)
        {
            //Debug.Log("Disabling Doorway");
            _newDoorWay.SetActive(false);
            _newDoorWay = null;
            if (_hasDoor && _newDoor != null)
            {
                //Debug.Log("Disabling Door");
                _doorStatus = _newDoor.GetComponent<CellDoor>().GetDoorCurrentPosition();
                _newDoor.GetComponent<CellDoor>().DisableDoor();
                _newDoor.SetActive(false);
                _newDoor = null;
            }

            StartCoroutine(WaitThenEnableCollider());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Door spawn point collison is null!!");
            return;
        }
        else if (_newDoorWay == null || !_newDoorWay.activeInHierarchy)
        {
            //Debug.Log("Collision w enemy at: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
            //Debug.Log("Distance between player & enemy: " + Vector3.Distance(transform.position, other.gameObject.transform.position));
            _doorSpawnCollider.enabled = false;
            _newDoorWay = null;
            _newDoor = null;
            SpawnDoorWay();
        }
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
    private void SpawnDoorWay()
    {
        _newDoorWay = DoorSpawner.Instance.GetDoorWayFromPool((int)_doorWayType);

        if (_newDoorWay == null)
        {
            Debug.Log("DoorWay pool empty!!");
            return;
        }
        else
        {
            _newDoorWay.transform.position = transform.position;
            _newDoorWay.transform.rotation = transform.rotation;
            _newDoorWay.SetActive(true);
            if (_hasDoor)
                SpawnDoor();
        }
    }

    private void SpawnDoor()
    {
        _newDoor = DoorSpawner.Instance.GetDoorFromPool((int)_doorWayType);

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

    private IEnumerator WaitThenEnableCollider()
    {
        yield return new WaitForSeconds(1f);
        _doorSpawnCollider.enabled = true;
    }
}
