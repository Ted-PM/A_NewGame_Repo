using System.Collections;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform _playerTransform;
    private GameObject _gameObject;
    private bool foundPlayer = false;
    private bool isLooking = false;
    //private void Awake()
    //{
    //    StartCoroutine(TryFindPlayer());
    //}

    private void OnEnable()
    {
        if (!foundPlayer)
            StartCoroutine(TryFindPlayer());
    }

    private IEnumerator TryFindPlayer()
    {
        yield return null;

        while (Camera.allCamerasCount <= 0)
        {
            yield return new WaitForSeconds(1f);
        }

        if (Camera.allCamerasCount == 1)
        {
            _gameObject = FindAnyObjectByType<Camera>().gameObject;
            foundPlayer = true;
        }
        
        //while (!foundPlayer)
        //{
        //    //_playerTransform = FindAnyObjectByType<Camera>().transform;
        //    //_gameObject = FindAnyObjectByType<Camera>().gameObject;
        //    //_gameObject = GameObject.fi;

        //    if (_playerTransform != null)
        //    {
        //        foundPlayer = true;
        //    }
        //    else
        //        yield return new WaitForSeconds(1f);
        //}
    }

    private IEnumerator LookAtPlayer_1()
    {
        isLooking = true;
        //yield return new WaitForSeconds(0.25f);
        yield return new WaitForFixedUpdate();
        
        transform.LookAt(new Vector3(_gameObject.transform.position.x, transform.position.y, _gameObject.transform.position.z));

        isLooking = false;
    }

    private void FixedUpdate()
    {
        if (!isLooking && foundPlayer)
            StartCoroutine(LookAtPlayer_1());
    }
}
