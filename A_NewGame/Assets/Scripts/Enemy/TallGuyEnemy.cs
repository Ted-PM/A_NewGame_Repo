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

    private List<Vector3> _destinations;
    private int _numDestinations = -1;

    //private Vector3 _posToRoamTo;

    private Vector3 _currentDestination;
    private int _currentDestinationIndex = 0;

    //private Vector3 _destinationPos;
    //private Vector3 _startPos;

    //private bool _goingToSpawn = false;

    private bool _destinationSet = false;

    //public void SetDestinationPos(Vector3 destPos, Vector3 startPos)
    public void SetDestinationPos(List<Vector3> destinations)
    {
        if (destinations == null || destinations.Count == 0)
        {
            Debug.LogError("No Tall Guy Positions!!");
            return;
        }

        _destinations = destinations;
        _currentDestination = _destinations[0];
        _currentDestinationIndex = 0;
        _numDestinations = _destinations.Count;
        Debug.Log("Num destinations: " +  _numDestinations);
        //_destinationPos = destPos;
        //_goingToSpawn = true;
        //_posToRoamTo = destPos;
        _enemyAnimator.enabled = true;
        //_startPos = startPos;
        _destinationSet = false;
        StartCoroutine(WaitUntilEnemyReady());
        //UpdateDestination();
    }

    protected override void Update()
    {
        if (_enemyState != EnemyStates.Disabled && _destinationSet)
            SetEnemyPath();
    }
    protected override void FixedUpdate()
    {
        if (_enemyState != EnemyStates.Disabled && _destinationSet)
        {
             agent.CalculatePath(_currentDestination, _enemyPath);         
        }
    }

    private IEnumerator WaitUntilEnemyReady()
    {
        while (!_playerFound || _destinations == null || _destinations.Count <= 0 || _numDestinations <= 0)
        {
            yield return new WaitForFixedUpdate();
        }

        UpdateDestination();
    }

    private void UpdateDestination()
    {
        //_goingToSpawn = !_goingToSpawn;
        _enemyAnimator.SetTrigger("Spin");
        if (FiftyFifty())
            _currentDestinationIndex++;
        else
            _currentDestinationIndex--;

        Debug.Log("Curr dest index: " + _currentDestinationIndex);

        int counter = 0;
        while (counter < 5 && (_currentDestinationIndex < 0 || _currentDestinationIndex >= _numDestinations))
        {
            _currentDestinationIndex = _currentDestinationIndex % _numDestinations;
            counter++;
            //yield return new WaitForFixedUpdate();
        }
        

        if (_currentDestinationIndex < 0 || _currentDestinationIndex >= _numDestinations)
        {
            Debug.LogWarning("Destination index being weird!!");
            _currentDestinationIndex = 0;
        }
        //    _currentDestinationIndex = _currentDestinationIndex % _numDestinations;

        Debug.Log("New dest index: " + _currentDestinationIndex);
        _currentDestination = _destinations[_currentDestinationIndex];
        //if (!_goingToSpawn)        
        //    _posToRoamTo = _destinationPos;       
        //else        
        //    _posToRoamTo = _startPos;

        _destinationSet = true;
        StartCoroutine(WaitTillNearNewPos());
    }


    private IEnumerator WaitTillNearNewPos()
    {
        yield return new WaitForFixedUpdate();

        while (!EnemyNearPosition(_currentDestination))// && _enemyState != EnemyStates.Disabled)
        {
            yield return new WaitForSeconds(0.5f);
        }


        _destinationSet = false;
        _playerPos = _playerTransform.transform.position;

        if (_playerFound && (GetDistanceToPlayer() >= _distanceBeforeDeAgro || !_enemyRenderer.isVisible))
        {
            DisableEnemy();
        }
        else
            UpdateDestination();

    }
    //public override void EnableEnemy()
    //{
    //    StopAllCoroutines();
    //    base.EnableEnemy();
    //    _enemyAnimator.enabled = true;
    //}

    protected override void DisableEnemy()
    {
        _destinations = null;
        _currentDestinationIndex = 0;
        _destinationSet = false;
        _numDestinations = -1;
        _currentDestination = Vector3.zero;
        base.DisableEnemy();
    }


    private bool FiftyFifty()
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

    protected override IEnumerator CheckIfEnemyAgroed()
    {
        yield return null;
    }

    protected override IEnumerator CheckIfEnemyDeAgroed()
    {
        yield return null;
    }

    protected override IEnumerator CanPathToPlayer()
    {
        yield return null;
    }

}
