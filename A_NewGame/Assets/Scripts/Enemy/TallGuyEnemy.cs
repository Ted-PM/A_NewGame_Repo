using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class TallGuyEnemy : EnemyBaseClass
{
    [SerializeField]
    private AudioClip _enemyPassiveAudio;
    [SerializeField]
    private AudioClip _enemyAgroAudio;

    //[SerializeField]
    //private Transform _enemyNewPos;

    private Vector3 _posToRoamTo;

    private Vector3 _destinationPos;
    private Vector3 _startPos;

    private bool _goingToSpawn = false;

    private bool _destinationSet = false;

    public void SetDestinationPos(Vector3 destPos, Vector3 startPos)
    {
        _destinationPos = destPos;
        _goingToSpawn = true;
        _posToRoamTo = destPos;
        _enemyAnimator.enabled = true;
        _startPos = startPos;
        _destinationSet = false;
        UpdateDestination();
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
             agent.CalculatePath(_posToRoamTo, _enemyPath);         
        }
    }

    private void UpdateDestination()
    {
        _goingToSpawn = !_goingToSpawn;
        _enemyAnimator.SetTrigger("Spin");
        if (!_goingToSpawn)        
            _posToRoamTo = _destinationPos;       
        else        
            _posToRoamTo = _startPos;

        _destinationSet = true;
        StartCoroutine(WaitTillNearNewPos());
    }


    private IEnumerator WaitTillNearNewPos()
    {
        yield return new WaitForFixedUpdate();

        while (!EnemyNearPosition(_posToRoamTo))// && _enemyState != EnemyStates.Disabled)
        {
            yield return new WaitForSeconds(0.5f);
        }


        _destinationSet = false;
        _playerPos = _playerTransform.transform.position;

        if (_playerFound && (GetDistanceToPlayer() >= _distanceBeforeDeAgro || !_enemyRenderer.isVisible))
        {
            _goingToSpawn = false;
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
