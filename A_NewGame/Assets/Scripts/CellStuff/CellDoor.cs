using System.Collections;
using UnityEngine;

public class CellDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject _doorPivot;

    [SerializeField]
    private GameObject _doorObject;
    private Collider _doorCollider;
    private Renderer _doorRenderer;

    [SerializeField]
    private Material _doorMaterial;

    [SerializeField]
    private Transform _start;
    [SerializeField]
    private Transform _endPositive;
    [SerializeField]
    private Transform _endNegative;
    [SerializeField]
    private float _timeToOpenDoor = 1f;
    [SerializeField]
    private float _timeToCloseDoor = 1f;
    [SerializeField]
    private AudioSource _doorAudioSource;
    [SerializeField]
    private AudioClip _doorOpenClip;
    [SerializeField]
    private AudioClip _doorCloseClip;

    private bool _doorOpen = false;
    private bool _canInteractWithDoor = true;

    private void Awake()
    {
        if (!_doorObject.TryGetComponent<Collider>(out _doorCollider))
            Debug.LogError("No Door Collider!");
        if (!_doorObject.TryGetComponent<Renderer>(out _doorRenderer))
            Debug.LogError("No Door Renderer!");

        SetDoorMaterial();
    }

    public void InteractWithDoor(Vector3 playerPos)
    {
        if (!_canInteractWithDoor)
            return;

        StartCoroutine(WaitBeforeCanInteractAgain());
        Transform currentPos = _doorPivot.transform;

        if (!_doorOpen)
        {
            _doorOpen = true;
            StartCoroutine(OpenDoor(currentPos, FindDirectionToOpenDoor(playerPos)));
        }
        else
        {
            _doorOpen = false;
            StartCoroutine(CloseDoor(currentPos));
        }
    }

    private Transform FindDirectionToOpenDoor(Vector3 playerPos)
    {
        float distanceToPosEnd = Vector3.Distance(playerPos, _endPositive.transform.position);
        float distanceToNegEnd = Vector3.Distance(playerPos, _endNegative.transform.position);

        if (distanceToPosEnd > distanceToNegEnd)
            return _endPositive;
        else
            return _endNegative;
    }

    private IEnumerator WaitBeforeCanInteractAgain()
    {
        _canInteractWithDoor = false;
        yield return new WaitForSeconds(0.01f);
        _canInteractWithDoor = true;
    }

    private IEnumerator OpenDoor(Transform currentPos, Transform newPos)
    {
        //Debug.Log("Opening Door");
        PlayAudioClip(_doorOpenClip);
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
    }

    private IEnumerator CloseDoor(Transform currentPos)
    {
        //Debug.Log("Closing Door");
        PlayAudioClip(_doorCloseClip);
        float t = 0f;
        float time = 0f;
        yield return null;

        while (t < 1 && !_doorOpen)
        {
            t = time / _timeToCloseDoor;
            time += Time.deltaTime;

            _doorPivot.transform.localRotation = Quaternion.Lerp(currentPos.localRotation, _start.localRotation, t);
            yield return null;
        }

    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Door audio Clip not set!!");
            return;
        }
        _doorAudioSource.Stop();
        _doorAudioSource.clip = clip;
        _doorAudioSource.Play();
    }

    private void SetDoorMaterial()
    {
        if (_doorMaterial != null && _doorRenderer != null)
        {
            _doorRenderer.material = _doorMaterial;
        }
    }

    public bool DoorIsOpen()
    {
        return _doorOpen;
    }
}
