using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TallGuySpawnPoint : EnemySpawnPoint
{
    public List<Transform> enemyDestinations;
    private List<Vector3> _tallGuyDestinations;

    protected override void SpawnEnemy()
    {
        base.SpawnEnemy();

        if (_tallGuyDestinations == null || _tallGuyDestinations.Count <= 0)
            AddTallEnemyDestinations();

        _newEnemy.GetComponent<TallGuyEnemy>().SetDestinationPos(_tallGuyDestinations);
    }


    private void AddTallEnemyDestinations()
    {
        _tallGuyDestinations = new List<Vector3>();

        for (int i = 0; i < enemyDestinations.Count; i++)
        {
            _tallGuyDestinations.Add(enemyDestinations[i].position);
        }
    }

    //protected override void OnTriggerExit(Collider other)
    //{
    //    if (other == null ||  other.gameObject == null)
    //    {
    //        Debug.Log("Spawn point other == null");
    //        return;
    //    }
    //    else if (_newEnemy != null && _newEnemy.activeInHierarchy)
    //    {
    //        Debug.Log("Disabling Tall Guy");

    //        _newEnemy.GetComponent<EnemyBaseClass>().DisableEnemy();
    //        _newEnemy.SetActive(false);
    //        _newEnemy = null;
    //    }
    //}
}
