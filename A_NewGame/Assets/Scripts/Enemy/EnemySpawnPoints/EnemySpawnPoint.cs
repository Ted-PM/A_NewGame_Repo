using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

[RequireComponent(typeof(Collider))]
public class EnemySpawnPoint : MonoBehaviour
{
    public EnemyType _enemyType;
    public Collider _enemySpawnCollider;

    protected GameObject _newEnemy = null;

    private void FixedUpdate()
    {
        if ((_newEnemy != null && !_newEnemy.activeSelf) || (_newEnemy == null && !_enemySpawnCollider.enabled))
        {
            _newEnemy = null;
            StartCoroutine(WaitThenEnableCollider());
        }
    }

    private IEnumerator WaitThenEnableCollider()
    {
        yield return new WaitForSeconds(1f);
        _enemySpawnCollider.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogError("Enemy spawn point collison is null!!");
            return;
        }
        else if (_newEnemy == null || !_newEnemy.activeInHierarchy)
        {
            //Debug.Log("Collision w enemy at: " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z);
            //Debug.Log("Distance between player & enemy: " + Vector3.Distance(transform.position, other.gameObject.transform.position));
            _enemySpawnCollider.enabled = false;
            _newEnemy = null;
            SpawnEnemy();
        }     
    }

    protected virtual void SpawnEnemy()
    {
        _newEnemy = EnemySpawner.Instance.GetObjectFromPool((int)_enemyType);

        if (_newEnemy == null)
        {
            Debug.Log("Enemy Pool empty!");
        }
        else
        {
            _newEnemy.transform.position = transform.position;
            _newEnemy.transform.rotation = transform.rotation;
            _newEnemy.SetActive(true);
            _newEnemy.GetComponent<EnemyBaseClass>().EnableEnemy();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Debug.Log("Base spawn point trigger exit");
    }



}
