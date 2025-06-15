using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CrawlerEnemy : EnemyBaseClass
{
    [SerializeField]
    private AudioClip _enemyPassiveAudio;
    [SerializeField]
    private AudioClip _enemyAgroAudio;

    //private bool  _chasePlayer = false;

    public override void GoToPlayer()
    {
        if (!_enemyMoved && _enemyState == EnemyStates.Agro && _playerTransform.GetComponent<PlayerController>().GetPlayerState() != MovementStates.Crouch)
        {
            //_enemyMoved = true;
            //StartCoroutine(WaitThenAgro());
            //transform.rotation = Quaternion.identity;
            //_enemyRB.useGravity = true;
            //_enemyAnimator.SetBool("Static", false);
            _enemyAnimator.SetBool("move", true);

            PlayOneShotAudio(_enemyAgroAudio);
            _enemyMoved = true;
        }
        if (_enemyState == EnemyStates.Agro && _enemyMoved)// && _chasePlayer)
            base.SetEnemyPath();
    }

    //private IEnumerator WaitThenAgro()
    //{
    //    //yield return new WaitForSeconds(0.2f);
    //    //_enemyAnimator.transform.localRotation = Quaternion.identity;
    //    //_enemyAnimator.transform.localPosition = Vector3.zero;
    //    //transform.rotation = Quaternion.identity;
    //    //_enemyRB.useGravity = true;
    //    StartCoroutine(MakeEnemyFall());
    //    yield return new WaitForSeconds(1.2f);
    //    _enemyAnimator.SetBool("Static", false);
    //    PlayOneShotAudio(_enemyAgroAudio);
    //    _chasePlayer = true;
    //}

    //private IEnumerator MakeEnemyFall()
    //{
    //    float t = 0;
    //    float time = 0;
    //    Vector3 currentPos = _enemyAnimator.transform.localPosition;
    //    Vector3 newPos = Vector3.zero;

    //    while (t < 1)
    //    {
    //        t = time / 1;
    //        time += Time.fixedDeltaTime;

    //        _enemyAnimator.transform.localPosition = Vector3.Lerp(currentPos, newPos, t);
    //        yield return new WaitForFixedUpdate();
    //    }
    //    _enemyAnimator.transform.localRotation = Quaternion.identity;
    //}

    public override void EnableEnemy()
    {
        base.EnableEnemy();

        //agent.enabled = false;
        //_enemyRB.useGravity = false;
        //_chasePlayer = false;



        //transform.position += new Vector3(0, 10, 0);
        //transform.rotation = new Quaternion(0, 0, 180, 0);
        //_enemyState = EnemyStates.Static;
        //StopAllCoroutines();

        //if (_enemyAudioSource != null)
        //    _enemyAudioSource.enabled = true;
        //EnableAgent();
        //_enemySpawn = transform.position;
        //if (!_playerFound)
        //    StartCoroutine(TryFindPlayer());
        //else
        //{
        //    _playerPos = _playerTransform.transform.position;

        //    StartCoroutine(CheckIfEnemyAgroed());
        //    StartCoroutine(CanPathToPlayer());
        //}
    }

    /// ------------
    protected override IEnumerator CheckIfEnemyAgroed()
    {
        //_chasePlayer = false;
        //_enemyAnimator.transform.localPosition = new Vector3(0, 10, 0);
        //_enemyAnimator.transform.localRotation = new Quaternion(0, 0, 180, 0);
        StartCoroutine(base.CheckIfEnemyAgroed());
        yield return null;
    }

    protected override IEnumerator CheckIfEnemyDeAgroed()
    {
        StartCoroutine(base.CheckIfEnemyDeAgroed());
        yield return null;
    }
    /// ------------
    //protected override IEnumerator EnemySeenForFirstTime()
    //{
    //    if (_enemyState != EnemyStates.Disabled)
    //        StartCoroutine(base.EnemySeenForFirstTime());
    //    yield break;
    //}

    protected override void EnableAgent()
    {
        base.EnableAgent();
        _enemyAnimator.enabled = true;
    }

    protected override IEnumerator WaitThenEnableAgent()
    {
        if (_enemyState != EnemyStates.Disabled)
        {
            _enemyAnimator.enabled = true;
            StartCoroutine(base.WaitThenEnableAgent());
        }
        yield break ;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
