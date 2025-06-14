using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemySpawnPoint : MonoBehaviour
{
    public EnemyType _enemyType;
    public Collider _enemySpawnCollider;
    [Tooltip("(Only for TALL GUY enemy)")]
    public Transform _enemyDestination;

    private GameObject _newEnemy;

    private void FixedUpdate()
    {
        if (_newEnemy != null && !_newEnemy.activeSelf)
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

    private void SpawnEnemy()
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
            if (_enemyType == EnemyType.TallGuy)
                _newEnemy.GetComponent<TallGuyEnemy>().SetDestinationPos(_enemyDestination.position, transform.position);

            //Debug.Log("New " + _enemyType + " spawned.");

        }
    }

}
