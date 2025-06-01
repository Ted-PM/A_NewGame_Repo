using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CellDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject _doorPivot;

    [SerializeField]
    private GameObject _doorObject;
    private Collider _doorCollider;

    [SerializeField]
    private Transform _start;
    [SerializeField]
    private Transform _endPositive;
    [SerializeField]
    private Transform _endNegative;
    [SerializeField]
    private float _timeToOpenDoor;

    private bool _doorOpen = false;
    private bool _canInteractWithDoor = true;

    //private Quaternion _open = new Quaternion(0f, 27.5f, 0f, 0f);

    private void Awake()
    {
        if (!_doorObject.TryGetComponent<Collider>(out _doorCollider))
            Debug.LogError("No Door Collider!");
        //_doorCollider = _doorObject.GetComponent<Collider>();
        //_doorRenderer = GetComponent<Renderer>();
        //_closedPos = this.transform;
    }

    public void InteractWithDoor(bool openPosDir = false)
    {
        if (!_canInteractWithDoor)
            return;
        StartCoroutine(WaitBeforeCanInteractAgain());
        Transform currentPos = _doorPivot.transform;
        //_doorCollider.enabled = false;

        if (!_doorOpen)
        {
            _doorOpen = true;
            if (openPosDir)
                StartCoroutine(OpenDoor(currentPos, _endPositive));
            else
                StartCoroutine(OpenDoor(currentPos, _endNegative));
        }
        else
        {
            _doorOpen = false;
            StartCoroutine(CloseDoor(currentPos));
        }
    }

    private IEnumerator WaitBeforeCanInteractAgain()
    {
        _canInteractWithDoor = false;
        yield return new WaitForSeconds(0.01f);
        _canInteractWithDoor = true;
    }

    private IEnumerator OpenDoor(Transform currentPos, Transform newPos)
    {
        Debug.Log("Opening Door");
        float t = 0f;
        float time = 0f;
        yield return null;

        while (t < 1 && _doorOpen)
        {
            t = time / _timeToOpenDoor;
            time += Time.deltaTime;

            _doorPivot.transform.localRotation = Quaternion.Lerp(currentPos.localRotation, newPos.localRotation, t);
            yield return null;
        }
        //_doorCollider.enabled = true;
    }

    private IEnumerator CloseDoor(Transform currentPos)
    {
        Debug.Log("Closing Door");

        float t = 0f;
        float time = 0f;
        yield return null;

        while (t < 1 && !_doorOpen)
        {
            t = time / _timeToOpenDoor;
            time += Time.deltaTime;

            _doorPivot.transform.localRotation = Quaternion.Lerp(currentPos.localRotation, _start.localRotation, t);
            yield return null;
        }

        //_doorCollider.enabled = true;
    }


    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.G))
    //        InteractWithDoor();
    //}
}
