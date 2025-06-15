using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UIElements;
public class TallGuyEnemy : EnemyBaseClass
{
    [SerializeField]
    private AudioClip _enemyPassiveAudio;
    [SerializeField]
    private AudioClip _enemyAgroAudio;

    //[SerializeField]
    //private Transform _enemyNewPos;

    public List<Vector3> _destinations;
    //private int _numDestinations = -1;

    //private Vector3 _posToRoamTo;

    public Vector3 _currentDestination;
    public int _currentDestinationIndex = 0;

    //private int prevIndex = -1;
    //private bool hasDoubledBack = false;

    //private Vector3 _destinationPos;
    //private Vector3 _startPos;

    //private bool _goingToSpawn = false;

    public bool _destinationSet = false;

    //public void SetDestinationPos(Vector3 destPos, Vector3 startPos)

    //protected override void Awake()
    //{
    //    base.Awake();
    //    agent.updatePosition = false;
    //    agent.updateRotation = true;
    //    //_enemyAnimator.applyRootMotion = true;
    //}

    public void SetDestinationPos(List<Vector3> destinations)
    {
        if (destinations == null || destinations.Count == 0)
        {
            Debug.LogError("No Tall Guy Positions!!");
            return;
        }
        Debug.Log("Destinations Set");

        _destinations = destinations;
        _currentDestinationIndex = GetRandomNum(0, _destinations.Count);
        _currentDestination = _destinations[_currentDestinationIndex];
        agent.enabled = false;

        transform.position = _currentDestination;
        agent.enabled = true;
        //_numDestinations = _destinations.Count;
        //Debug.Log("Num destinations: " + _destinations.Count);
        //_destinationPos = destPos;
        //_goingToSpawn = true;
        //_posToRoamTo = destPos;
        _enemyAnimator.enabled = true;
        //_startPos = startPos;
        _destinationSet = false;

        UpdateDestination();

        //if (_enemyState != EnemyStates.Disabled && this.gameObject.activeInHierarchy)
        //    StartCoroutine(UpdateDestination());
        //UpdateDestination();
    }

    //public override void GoToPlayer()
    //{
    //    if (_enemyState != EnemyStates.Disabled && _destinationSet)
    //}


    protected override void Update()
    {
        if (_enemyState != EnemyStates.Disabled && _destinationSet)
        {
            //Vector3 rootPosition = _enemyAnimator.rootPosition;
            //rootPosition.y = agent.nextPosition.y;
            //transform.position = rootPosition;
            //agent.nextPosition = rootPosition;

            //_enemyAnimator.SetFloat("Speed", agent.velocity.magnitude);
            //transform.position = _enemyAnimator.rootPosition;
            //Vector3 rootPos = new Vector3(_enemyAnimator.rootPosition.x, 0, _enemyAnimator.rootPosition.z);
            //agent.transform.position = rootPosition;
            //agent.transform.position = _enemyBody.transform.position;
            //agent.transform.LookAt(_currentDestination);

            agent.destination = _currentDestination;
            //_enemyBody.transform.position = _enemyAnimator.rootPosition;

            //SetEnemyPath();
        }
    }

    //private void OnAnimatorMove()
    //{
    //    Vector3 rootPosition = _enemyAnimator.rootPosition;
    //    Vector3 delataRootPosition = _enemyAnimator.deltaPosition;
    //    Debug.Log("Root Pos: " + rootPosition.x + ", " + rootPosition.y + ", " + rootPosition.z);
    //    Debug.Log("current Pos: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
    //    rootPosition.y = agent.nextPosition.y;

    //    //agent.nextPosition = rootPosition;
    //    agent.nextPosition += delataRootPosition;
    //    transform.position = rootPosition;
    //    Debug.Log("new Pos: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
    //}


    protected override void FixedUpdate()
    {
        if (_enemyState != EnemyStates.Disabled && _destinationSet)
        {
            //Vector3 rootPosition = _enemyAnimator.rootPosition;
            //rootPosition.y = agent.nextPosition.y;
            //transform.position = rootPosition;
            //agent.nextPosition = rootPosition;

            //if (Vector3.Distance(_currentDestination, _enemyAnimator.rootPosition) <= 1)
            //if (EnemyNearPosition(_currentDestination))
            //{
            //    _destinationSet = false;
            //    _enemyAnimator.applyRootMotion = false;
            //    UpdateDestination();
            //}
             //agent.CalculatePath(_currentDestination, _enemyPath);         
        }
    }

    //private IEnumerator WaitUntilEnemyReady()
    //{
    //    while (!_playerFound || _destinations == null || _destinations.Count <= 0)
    //    {
    //        yield return new WaitForFixedUpdate();
    //    }

    //    UpdateDestination();
    //}

    private void UpdateDestination()
    {
        //_goingToSpawn = !_goingToSpawn;
        _enemyAnimator.SetTrigger("Spin");


        if (FiftyFifty())
            _currentDestinationIndex++;
        else
            _currentDestinationIndex--;

        //Debug.Log("Curr dest index: " + _currentDestinationIndex + ", num dest: " + _destinations.Count + ", new dest should be: "+
        //    (_currentDestinationIndex % _destinations.Count));

        _currentDestinationIndex = _currentDestinationIndex % _destinations.Count;
        if (_currentDestinationIndex == -1)
            _currentDestinationIndex = _destinations.Count - 1;

        //prevIndex = _currentDestinationIndex;
        //int counter = 0;
        //while (counter < 5 && (_currentDestinationIndex < 0 || _currentDestinationIndex >= _destinations.Count))
        //{
        //    _currentDestinationIndex = _currentDestinationIndex % _destinations.Count;
        //    counter++;
        //    yield return new WaitForFixedUpdate();
        //}


            //if (_currentDestinationIndex < 0 || _currentDestinationIndex >= _destinations.Count)
            //{
            //    Debug.LogWarning("Destination index being weird!!");
            //    _currentDestinationIndex = 0;
            //}
            //    _currentDestinationIndex = _currentDestinationIndex % _numDestinations;

        Debug.Log("Path updated");
        _currentDestination = _destinations[_currentDestinationIndex];

        if (_enemyState != EnemyStates.Disabled)
        {
            agent.CalculatePath(_currentDestination, _enemyPath);
            //transform.LookAt(_currentDestination);

            //_enemyAnimator.applyRootMotion = true;
            _destinationSet = true;
            StartCoroutine(WaitTillNearNewPos());
        }

    }


    private IEnumerator WaitTillNearNewPos()
    {
        yield return new WaitForFixedUpdate();

        while (!EnemyNearPosition(_currentDestination) && _enemyState != EnemyStates.Disabled)
        {
            //if (_enemyAnimator.GetCurrentAnimatorStateInfo(0).speed == 0.4f)
            //{
            //    _destinationSet = true;
            //    //_enemyAnimator.applyRootMotion = true;
            //}

            yield return new WaitForFixedUpdate();
            //yield return new WaitForSeconds(0.5f);
        }

        //_enemyAnimator.applyRootMotion = false;


        _destinationSet = false;
        _playerPos = _playerTransform.transform.position;

        //if (_playerFound && (GetDistanceToPlayer() >= _distanceBeforeDeAgro))// || !_enemyRenderer.isVisible))
        //{
        //    DisableEnemy();
        //}
        //else
        UpdateDestination();

    }
    //public override void EnableEnemy()
    //{
    //    StopAllCoroutines();
    //    base.EnableEnemy();
    //    _enemyAnimator.enabled = true;
    //}

    public override void DisableEnemy()
    {
        StopAllCoroutines();
        if (_destinations != null)
        {
            for (int i = _destinations.Count - 1; i >= 0; i--)
                _destinations.RemoveAt(i);
            _destinations = null;
        }


        _currentDestinationIndex = 0;
        _destinationSet = false;
        //_numDestinations = -1;
        _currentDestination = Vector3.zero;
        base.DisableEnemy();
    }


    private bool FiftyFifty()
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

    private bool OneInThree()
    {
        return UnityEngine.Random.Range(0, 3) == 0;
    }

    private int GetRandomNum(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    protected override IEnumerator CheckIfEnemyAgroed()
    {
        yield return null;
    }

    protected override IEnumerator CheckIfEnemyDeAgroed()
    {
        yield return null;
    }

    //protected override void OnTriggerExit(Collider other)
    //{
    //    if (other == null || other.gameObject == null)
    //    {
    //        Debug.Log("Spawn point other == null");
    //        return;
    //    }
    //    else if (this.gameObject.activeInHierarchy)// && !_enemyRenderer.isVisible)
    //    {
    //        Debug.Log("Disabling Tall Guy");

    //        DisableEnemy();

    //        //_newEnemy.SetActive(false);
    //        //_newEnemy = null;
    //    }
    //}

    protected override IEnumerator CanPathToPlayer()
    {
        yield return null;
        while (_enemyState != EnemyStates.Disabled)
        {
            if (_playerFound && GetDistanceToPlayer() >= _distanceBeforeDeAgro && !_enemyRenderer.isVisible)
            {
                Debug.Log("Enemy Disabled");
                DisableEnemy();
            }

            yield return new WaitForFixedUpdate();
        }
    }

}
