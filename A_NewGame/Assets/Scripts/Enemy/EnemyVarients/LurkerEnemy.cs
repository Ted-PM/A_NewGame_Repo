using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class LurkerEnemy : EnemyBaseClass
{
    [SerializeField]
    private AudioClip _enemyPassiveAudio;
    [SerializeField]
    private AudioClip _enemyAgroAudio;

    private bool _playerStaring = false;
    private bool _checkingIfStaring = false;
    private bool _goToPlayer = false;
    public override void GoToPlayer()
    {
        if (!_enemyMoved && _playerStaring && _enemyState == EnemyStates.Agro)
        {
            _enemyMoved = true;
            PlayOneShotAudio(_enemyAgroAudio);
            //if (!_enemyAnimator.enabled) 
            StartCoroutine(YellThenRun());
            //_enemyAnimator.SetBool("Static", false);
        }
        else if (!_checkingIfStaring && !_playerStaring)
            StartCoroutine(PlayerIsStaring());
        if (_enemyState == EnemyStates.Agro && _goToPlayer)
            base.SetEnemyPath();
    }
    /// ------------
    protected override IEnumerator CheckIfEnemyAgroed()
    {
        //_enemyAnimator.SetBool("Static", true);
        yield return new WaitForFixedUpdate();
        //_enemyAnimator.enabled = false;
        _playerStaring = false;
        _checkingIfStaring = false;
        _goToPlayer = false;

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
    //    _enemyAnimator.SetBool("Yelling", false);
    //    _enemyAnimator.enabled = false;
    //    _playerStaring = false;
    //    _checkingIfStaring = false;
    //    _goToPlayer = false;
    //    yield return null;

    //    StartCoroutine(base.EnemySeenForFirstTime());
    //    //while (_enemyState != EnemyStates.Agro && _enemyState != EnemyStates.Disabled)
    //    //{
    //    //    yield return new WaitForFixedUpdate();
    //    //    if (EnemyVisibleToPlayer())
    //    //        _enemyState = EnemyStates.Agro;
    //    //    //_enemySeenForFirstTime = EnemyVisibleToPlayer();
    //    //}
    //    //StartCoroutine(base.CanNoLongerSeePlayer());
    //}

    protected override void EnableAgent()
    {
        base.EnableAgent();
    }

    protected override IEnumerator WaitThenEnableAgent()
    {
        if (_enemyState != EnemyStates.Disabled)
            StartCoroutine(base.WaitThenEnableAgent());
        yield break;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    private IEnumerator YellThenRun()
    {
        //_enemyAnimator.SetBool("Yelling", true);
        _enemyAnimator.enabled = true;
        //yield return new WaitForFixedUpdate();
        //_enemyAnimator.SetBool("Static", false);
        _enemyAnimator.SetBool("move", true);

        yield return new WaitForFixedUpdate();
        _enemyAnimator.SetTrigger("StartYelling");
        transform.LookAt(_playerPos);
        yield return new WaitForSeconds(1.1f);
        //_enemyAnimator.SetBool("Yelling", false);
        _goToPlayer = true;
        base.StopAudio();
    }

    private IEnumerator PlayerIsStaring()
    {
        _checkingIfStaring = true;
        bool isStaring = true;
        float time = 0f;

        while (time < 2.5f)
        {
            time += Time.fixedDeltaTime;
            isStaring = PlayerLookingAtEnemyDirectly(20f);
            if (!isStaring)
                break;
            yield return new WaitForFixedUpdate();
        }
        _playerStaring = isStaring;
        _checkingIfStaring = false;
    }
}
