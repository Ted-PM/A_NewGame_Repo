using UnityEngine;
using System.Collections.Generic;
public class Level_BaseClass : MonoBehaviour
{
    public List<CellPrefabs> cellPrefabs;

    public int lvlXDimensions = 20;
    public int lvlZDimensions = 20;
    public int lvlYDimensions = 1;

    private GameObject[,] _cellMatrix;
    private int[,] _cellMatrixInt;
    private bool[,] _cellMatrixBool;

    private void Awake()
    {
        VerifyDimensions();
        InitializeMatrixes();
        SetIntMatrixDefaultValues();
    }

    private void VerifyDimensions()
    {
        if (!DimensionsValid())
        {
            Debug.LogError("Dimensions invalid!! (" + lvlXDimensions + ", " + lvlYDimensions + ", " + lvlZDimensions + ")");
            lvlXDimensions = 20;
            lvlZDimensions = 20;
            lvlYDimensions = 1;
        }
    }
    private bool DimensionsValid()
    {
        if (lvlXDimensions <= 0 || lvlZDimensions <= 0 || lvlYDimensions <= 0)
            return false;

        return true;
    }
    private void InitializeMatrixes()
    {
        _cellMatrix = new GameObject[lvlXDimensions, lvlZDimensions];
        _cellMatrixInt = new int[lvlXDimensions, lvlZDimensions];
        _cellMatrixBool = new bool[lvlXDimensions, lvlZDimensions];
    }
    private void SetIntMatrixDefaultValues()
    {
        for (int x = 0; x < lvlXDimensions; x++)        
            for (int z = 0; z < lvlZDimensions; z++)
                _cellMatrixInt[x,z] = -1;       
    }
    private bool PrefabsAreValid()
    {
        if (cellPrefabs == null || cellPrefabs.Count == 0)
        {
            Debug.LogError("Prefabs Invalid!!");
            return false;
        }

        return true;
    }


    [System.Serializable]
    public struct CellPrefabs
    {
        public CellBaseClass cellPrefab;

        public readonly ushort xDim { get { return (ushort)cellPrefab.GetCellXWidth(); } }
        public readonly ushort zDim { get { return (ushort)cellPrefab.GetCellZHeight(); } }
        public readonly ushort yDim { get { return (ushort)cellPrefab.GetCellYFloors(); } }
    }
}
