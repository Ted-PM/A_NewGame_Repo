using System.Collections;
//using Unity.VisualScripting;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using System.Linq;
using System;
//using UnityEngine.Rendering;
//using UnityEditor.Rendering;
using UnityEditor;

//[Serializable] public class PefabAndOdds : SerializedDictionary<GameObject, int> { }

public class CellSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject StartCell;

    [SerializeField,Tooltip("Last Prefab in List is start cell, Keep first one as smallest possible cell.")]
    private GameObject[] _prefabs;
    [SerializeField, Tooltip("The likeleyHood the prefab at that index will be chosen. Also always includes 1st Prefab.")]
    private int[] oddsOfChoosePrefab;

    private GameObject[,] _cellMatrix; // [x,z]
    private bool[,] _cellMatrixBool;

    private GameObject[] _XContainers;
    public int X_Width = 10;
    public int Z_Height = 10;

    [SerializeField]
    private MazeGenerator _generator;

    [SerializeField]
    private int Start_X = 0;
    [SerializeField]
    private int Start_Z = 0;

    private void Awake()
    {
        if (_cellMatrix != null)
        {
            _cellMatrix = null;
        }
        if (_cellMatrixBool != null)
        {
            _cellMatrixBool = null;
        }

        _cellMatrix = new GameObject[X_Width, Z_Height];
        _cellMatrixBool = new bool[X_Width, Z_Height];

        if (_XContainers == null)
        {
            _XContainers = new GameObject[X_Width];
        }
        if (oddsOfChoosePrefab == null || oddsOfChoosePrefab.Length < _prefabs.Length || oddsOfChoosePrefab.Sum() == 0)
        {
            oddsOfChoosePrefab = new int[_prefabs.Length];
            Array.Fill(oddsOfChoosePrefab, 1);
        }
        else
        {
            oddsOfChoosePrefab[0] = 1;
        }
    }
    void Start()
    {
        SpawnCells();
        // starts generating maze before walls created like a dumbass, now use coroutine.
        StartCoroutine(WaitForABit());
    }

   private IEnumerator WaitForABit()
    {
        yield return new WaitForSeconds(1f);
        SetMatrixID();
        yield return new WaitForSeconds(1f);
        //PrintWalls();
        //yield return new WaitForSeconds(1f);
        _generator.GenerateMaze(_cellMatrix, _cellMatrixBool, X_Width, Z_Height, Start_X, Start_Z);
    }

    private void SetMatrixID()
    {
        for (int x = 0; x < X_Width; x++)
        {
            for (int z = 0; z < Z_Height; z++)
            {
                //Debug.Log("Setting Matrix ID (" + x + ", " + z + ")");
                _cellMatrix[x, z].GetComponent<Cell>().SetMatrixID(x, z);
            }
        }
    }

    private void PrintWalls()
    {
        for (int x = 0; x < X_Width; x++)
        {
            for (int z = 0; z < Z_Height; z++)
            {
                _cellMatrix[x, z].GetComponent<Cell>().PrintWallStuff();
            }
        }
    }

    private void SpawnCells()
    {
        if (Start_X >=0 && Start_X < X_Width && Start_Z >=0 && Start_Z < Z_Height) 
            SpawnCell(Start_X, Start_Z);

        for (int x = 0; x < X_Width; x++)
        {
            // create empty object to store/organise the cells in column thing
            _XContainers[x] = new GameObject();
            _XContainers[x].transform.parent = this.transform;
            _XContainers[x].name = "X: " + x.ToString();

            for (int z = 0; z < Z_Height; z++)
            {
                if (_cellMatrix[x, z] == null)
                {
                    SpawnCell(x, z);
                    //MarkDeadCells(_cellMatrix[x, z].GetComponent<Cell>(), x, z);
                }
                else
                {
                    //Debug.Log("Cell Occupied: " + x.ToString() + ", " + z.ToString());
                }
            }
        }
    }

    private void SpawnCell(int x, int z)
    {
        List<int> potentialPrefabs = new List<int>();

        potentialPrefabs = PopulatePrefabList(potentialPrefabs);

        potentialPrefabs = RandomizeList(potentialPrefabs);

        Cell potentialPrefab;

        if (x == Start_X && z == Start_Z)
        {
            potentialPrefab = StartCell.GetComponent<Cell>();
            //Debug.Log("StartCell x/z: " + potentialPrefab.GetCellXWidth() + ", " + potentialPrefab.GetCellZHeight());
            if (CheckIfCellInBounds(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
            {
                //Debug.Log("StartCell inbounds");
                if (CheckIfCellCanSpawn(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
                {
                    //Debug.Log("StartCell can spawn");
                    SpawnIndividualCell(x, z, -1, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight());
                    return;
                }
            }
        }

        //Debug.Log("List Size:" + potentialPrefabs.Count);

        for (int i = (potentialPrefabs.Count - 1); i >= 0; i--)
        {
            //Debug.Log("Potential Prefab num: " + potentialPrefabs[i]);
            potentialPrefab = _prefabs[potentialPrefabs[i]].GetComponent<Cell>();

            if (!CheckIfCellInBounds(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
                continue;
            
            if (CheckIfCellCanSpawn(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
            {
                SpawnIndividualCell(x, z, potentialPrefabs[i], potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight());
                break;
            }
        }
    }

    private List<int> PopulatePrefabList(List<int> list)
    {
        for (int i = 0; i < _prefabs.Length; i++)
        {
            for (int j = 0; j < oddsOfChoosePrefab[i]; j++)
            {
                list.Add(i);
            }

        }

        return list;
    }

    private List<int> RandomizeList(List<int> list)
    {
        if (list.Count <= 0)
            return new List<int>();

        return list.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    private void SpawnIndividualCell(int x, int z, int prefabChoice, int cellXWidth, int cellzHeight)
    {
        if (_XContainers[x] != null)
            _cellMatrix[x, z] = Instantiate(_prefabs[prefabChoice], _XContainers[x].transform);
        else 
        {
            GameObject StartContainer = new GameObject();
            StartContainer.transform.parent = this.transform;
            StartContainer.name = "Start Cell: " + Start_X.ToString() + ", " + Start_Z.ToString();

            if (prefabChoice == -1)
                _cellMatrix[x, z] = Instantiate(StartCell, StartContainer.transform);
            else
                _cellMatrix[x, z] = Instantiate(_prefabs[prefabChoice], StartContainer.transform);
        }


        _cellMatrix[x, z].transform.position = new Vector3(x * 10f, 0, z * 10f);
        _cellMatrix[x, z].name = "Cell " + x.ToString() + ", " + z.ToString();
        _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z);

        for (int i = 0; i < cellXWidth; i++)
        {
            for (int j = 0; j < cellzHeight; j++)
            {
                _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x + i, z + j);
                _cellMatrix[(x + i), (z + j)] = _cellMatrix[x, z];
            }
        }

        //_cellMatrix[x, z].GetComponent<Cell>().SetDeadCells();

        MarkDeadCells(_cellMatrix[x, z].GetComponent<Cell>(), x, z);
        //if (_cellMatrix[x, z].GetComponent<Cell>().GetNumDeadCells() > 0)
        //{
        //}
    }

    private void MarkDeadCells(Cell currentCell, int x, int z)
    {
        //Debug.Log("CurrentCell Base: " + x + ", " + z);
        //Debug.Log("Num Dead Cells: " + currentCell.GetNumDeadCells());

        for (int i = 0; i < currentCell.GetNumDeadCells(); i++)
        {
            //Debug.Log("Dead Cell at: " + (x + currentCell.GetDeadCellX(i)) + ", " + (z + currentCell.GetDeadCellZ(i)));
            //Debug.Log("Dead Cell at: " + (currentCell.GetDeadCellX(i)) + ", " + (currentCell.GetDeadCellZ(i)));
            _cellMatrixBool[currentCell.GetDeadCellX(i),currentCell.GetDeadCellZ(i)] = true;
            //_cellMatrixBool[x + currentCell.GetDeadCellX(i), z + currentCell.GetDeadCellZ(i)] = true;
        }
        
    }

    private bool CheckIfCellInBounds(int x, int z, int cellXWidth, int cellzHeight)
    {
        return ((x + cellXWidth - 1) < X_Width && x >= 0 && (z + cellzHeight - 1) < Z_Height && z >= 0);
    }

    private bool CheckIfCellCanSpawn(int x, int z, int cellXWidth, int cellzHeight)
    {
        bool canSpawn = true;

        for (int i = 0; i < cellXWidth; i++)
        {
            for (int j = 0; j < cellzHeight; j++)
            {
                if (_cellMatrix[(x + i), (z + j)] != null)
                {
                    canSpawn = false;
                }
            }
        }

        return canSpawn;    
    }

    private int GetRandomNumber(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
