using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using NUnit.Framework;
public class CellSpawnerSpawner : MonoBehaviour
{
    [SerializeField]
    private CellSpawner cellSpawner;

    private List<bool[,]> _listOfFloorMatrixBools;

    [SerializeField]
    private int _numberOfFloors;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
