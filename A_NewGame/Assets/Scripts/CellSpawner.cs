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
using Unity.VisualScripting;

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

    [SerializeField, Tooltip("Prefabs to be used when moving to second floor.")]
    private GameObject[] _yTransitionPrefabs;
    private List<int[]> _secondFloorCellsToDisable;

    //public CellSpawner(GameObject parentSpawner)
    //{
    //    _parentSpawner = parentSpawner;
    //}

    [SerializeField]
    private GameObject _secondFloorSpawner;
    private CellSpawner _secondFloor;
    public int _secondFloorX = -1;
    public int _secondFloorZ = -1;
    public int _secondFloorY = -1;

    [SerializeField]
    private bool _hasSecondFloor;
    [SerializeField]
    private bool _isSecondFloor;

    //public Cell transitionCell;
    [HideInInspector]
    public int[] transitionCellDimensions;
    [HideInInspector]
    public int[] transitionalCellPos;

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
        if (oddsOfChoosePrefab == null || oddsOfChoosePrefab.Length < _prefabs.Length
            || oddsOfChoosePrefab.Length > _prefabs.Length || oddsOfChoosePrefab.Sum() == 0)
        {
            oddsOfChoosePrefab = new int[_prefabs.Length];
            Array.Fill(oddsOfChoosePrefab, 1);
        }
        else
        {
            oddsOfChoosePrefab[0] = 1;
        }

        if (_hasSecondFloor)
        {
            _secondFloorCellsToDisable = new List<int[]>();
            if (_secondFloorX == -1 && _secondFloorZ == -1 && _secondFloorY == -1)
            {
                _secondFloorX = X_Width / 2;
                _secondFloorZ = Z_Height / 2;
                _secondFloorY = 1;
            }
            Debug.Log("Transitional Cell Pos: " + _secondFloorX + ", " + _secondFloorZ);
            if (_secondFloorX == Start_X && _secondFloorZ == Start_Z)
            {
                Debug.LogError("Can't transition to second floor at start cell!!");
                _hasSecondFloor = false;
            }
        }
        else
        {
            _secondFloorX = -1;
            _secondFloorZ = -1;
            _secondFloorY = -1;
        }

        transitionCellDimensions = new int[2];
        transitionalCellPos = new int[2];

        //transitionCellDimensions = new int[2];

        //if (_isSecondFloor)
        //{
        //    transitionCellDimensions = FindFirstObjectByType<CellSpawner>().GetTransitionalCellDimensions();
        //}
        //if (_isSecondFloor)
        //{
        //    try
        //    {
        //        //transitionCell = new Cell();
        //        //_cellMatrixBool = this.GetComponentInParent<CellSpawner>()._cellMatrixBool;
        //        this.transitionCell = this.GetComponentInParent<CellSpawner>().transitionCell;
        //    }
        //    catch { Debug.Log("NoParent"); }
        //}


    }
    void Start()
    {
        if (!_isSecondFloor)
        {
            SpawnCells();
            // starts generating maze before walls created like a dumbass, now use coroutine.
            StartCoroutine(WaitForABit());
        }
    }

    public void Start2()
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
        
        if (_hasSecondFloor)
        {
            Debug.Log("Now spawning, trans dimension: " + transitionCellDimensions[0] + ", " + transitionCellDimensions[1]);
            StartCoroutine(WaitTheSpawnSecondFloor());
        }
   }

   private IEnumerator WaitTheSpawnSecondFloor()
    {
        yield return new WaitForSeconds(1f);
        Vector3 secondFloorSpawnPoint = new Vector3(this.transform.position.x + 10* _secondFloorX, 
            this.transform.position.x + 10 * _secondFloorY,
            this.transform.position.z + 10 * _secondFloorZ);

        GameObject SecondFloor = Instantiate(_secondFloorSpawner, secondFloorSpawnPoint, Quaternion.identity);
        _secondFloor = SecondFloor.GetComponent<CellSpawner>();
        _secondFloor.SetTransitionalCellDImenstions(transitionCellDimensions, _cellMatrix[transitionalCellPos[0], transitionalCellPos[1]].GetComponent<Cell>());
        _secondFloor.Start2();
        //_secondFloor.cellsp
        //_secondFloor.Init()
        yield return new WaitForSeconds(1f);
        _secondFloor.DisableCellsOnTopFloor(_secondFloorCellsToDisable, _secondFloorX, _secondFloorZ);
    }

    public void DisableCellsOnTopFloor(List<int[]> objectivePos, int secondFloorX, int secondFloorZ)
    {
        Debug.Log("Disabling " + objectivePos.Count + " Cells in top floor;");
        List <int[]> relativePos = new List <int[]>();
        int[] tempIndex;

        if (objectivePos.Count > 0)
        {
            for (int i = 0; i < objectivePos.Count; i++)
            {
                tempIndex = new int[2];
                tempIndex[0] = objectivePos[i][0] - secondFloorX;
                tempIndex[1] = objectivePos[i][1] - secondFloorZ;
                relativePos.Add(tempIndex);
                Debug.Log("Including Cell: " + relativePos[i][0] + ", " + relativePos[i][1]);
            }

            for (int i = 0;i < relativePos.Count; i++)
            {
                //current._zPosHorizontalWalls[i].DisableWall();
                //try
                //{
                //    _generator.DisableZHorizontalWall(_cellMatrix[relativePos[i][0] + 1, relativePos[i][1]].GetComponent<Cell>(), 
                //        _cellMatrix[relativePos[i][0], relativePos[i][1]].GetComponent<Cell>(), relativePos[i], false);
                //    //DisableZHorizontalWall(Cell current, Cell next, int[] wallToBeDisabled, bool found)
                //}
                //catch { Debug.Log("No wall to right."); }
                //try
                //{
                //    _generator.DisableXVerticalWall(_cellMatrix[relativePos[i][0], relativePos[i][1]+1].GetComponent<Cell>(), 
                //        _cellMatrix[relativePos[i][0], relativePos[i][1]].GetComponent<Cell>(), relativePos[i], false);

                //}
                //catch { Debug.Log("No wall above."); }
                
                _cellMatrix[relativePos[i][0], relativePos[i][1]].GetComponent<Cell>().DisableCell();
            }

        }
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

    private void SetSecondFloorPos()
    {
        if (!_hasSecondFloor)
            return;

        _secondFloorX = GetRandomNumber(1, X_Width);
        _secondFloorZ = GetRandomNumber(1, Z_Height);
        _secondFloorY = 1;
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

        //SetSecondFloorPos();

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

        bool isTransitionalPrefab = false;

        if (x == _secondFloorX && z == _secondFloorZ)
        {
            isTransitionalPrefab = true;
            //Debug.Log("Is Transitional: " + x.ToString() + ", " + z.ToString());
        }

        potentialPrefabs = PopulatePrefabList(potentialPrefabs, isTransitionalPrefab);

        potentialPrefabs = RandomizeList(potentialPrefabs);

        Cell potentialPrefab;

        // first iteration only
        if (x == Start_X && z == Start_Z)
        {
            potentialPrefab = StartCell.GetComponent<Cell>();

            if (CheckIfCellInBounds(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
            {
                if (CheckIfCellCanSpawn(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
                {
                    // pass -1 as prefab choice so it chooses the assigned "Start" prefab but
                    // if that one doesn't fit on matrix, it'll default to the usual ones
                    SpawnIndividualCell(x, z, -1, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight(), false);
                    return;
                }
            }
        }



        // try random prefab, if doesn't work (to big / cells occupied) remove from list & try again
        for (int i = (potentialPrefabs.Count - 1); i >= 0; i--)
        {
            if (!isTransitionalPrefab)
                potentialPrefab = _prefabs[potentialPrefabs[i]].GetComponent<Cell>();
            else
                potentialPrefab = _yTransitionPrefabs[potentialPrefabs[i]].GetComponent<Cell>();
                

            if (!CheckIfCellInBounds(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
                continue;
            
            if (CheckIfCellCanSpawn(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
            {
                SpawnIndividualCell(x, z, potentialPrefabs[i], potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight(), isTransitionalPrefab);
                break;
            }
        }
    }

    private List<int> PopulatePrefabList(List<int> list, bool isTransitional)
    {
        // allow you to make 1 prefab more likely that others by adding it's 
        // index in prefab list multiple times
        if (isTransitional)
        { 
            for (int i = 0; i < _yTransitionPrefabs.Length; i++)
            {
                //Debug.Log("Transitional Prefab added: " + i);
                list.Add(i);
            }
            return list;
        }

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

    private void SpawnIndividualCell(int x, int z, int prefabChoice, int cellXWidth, int cellzHeight, bool isTransitionalPrefab)
    {
        if (_XContainers[x] != null && !isTransitionalPrefab)
            _cellMatrix[x, z] = Instantiate(_prefabs[prefabChoice], _XContainers[x].transform);
        else if (_XContainers[x] != null && isTransitionalPrefab)
        {
            transitionCellDimensions = new int[2];
            Debug.Log("TransitionalCell spawned at: " + x + ", " + z);
            _cellMatrix[x, z] = Instantiate(_yTransitionPrefabs[prefabChoice], _XContainers[x].transform);
            transitionCellDimensions[0] = _cellMatrix[x, z].GetComponent<Cell>().GetCellXWidth();
            transitionCellDimensions[1] = _cellMatrix[x, z].GetComponent<Cell>().GetCellZHeight();
            transitionalCellPos[0] = x;
            transitionalCellPos[1] = z;
            Debug.Log("TransitionalCell Initial Dimensions: " + transitionCellDimensions[0] + ", " + transitionCellDimensions[1]);
            //transitionCell = _cellMatrix[x, z].GetComponent<Cell>();
        }
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


        //_cellMatrix[x, z].transform.position = new Vector3(x * 10f, this.transform.position.y, z * 10f);
        _cellMatrix[x, z].transform.position = new Vector3(x * 10f + this.transform.position.x, this.transform.position.y, z * 10f + this.transform.position.z);
        _cellMatrix[x, z].name = "Cell " + x.ToString() + ", " + z.ToString();
        _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z);
        int[] secondFloorCells;
        for (int i = 0; i < cellXWidth; i++)
        {
            for (int j = 0; j < cellzHeight; j++)
            {
                _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x + i, z + j);
                _cellMatrix[(x + i), (z + j)] = _cellMatrix[x, z];
                if (isTransitionalPrefab)
                {
                    secondFloorCells = new int[2];
                    secondFloorCells[0] = x + i;
                    secondFloorCells[1] = z + j;
                    _secondFloorCellsToDisable.Add(secondFloorCells);
                }
            }
        }

        MarkDeadCells(_cellMatrix[x, z].GetComponent<Cell>(), x, z);
    }

    private void MarkDeadCells(Cell currentCell, int x, int z)
    {
        if (!currentCell.HasDeadCell()) 
            return;

        for (int i = 0; i < currentCell.GetNumDeadCells(); i++)
        {
            _cellMatrixBool[currentCell.GetDeadCellX(i),currentCell.GetDeadCellZ(i)] = true;
        }
        
    }

    private bool CheckIfCellInBounds(int x, int z, int cellXWidth, int cellzHeight)
    {
        return ((x + cellXWidth - 1) < X_Width && x >= 0 && (z + cellzHeight - 1) < Z_Height && z >= 0);
    }

    private bool CheckIfCellCanSpawn(int x, int z, int cellXWidth, int cellzHeight)
    {
        bool isTransitionCell = false;
        //bool isTopFloor = false;
        //if (transitionCell != null && _isSecondFloor)
        //    isTopFloor = true;


        if (_hasSecondFloor && x == _secondFloorX && z == _secondFloorZ)
        {
            //Debug.Log("isTransitionCell at " + x + ", " + z);
            isTransitionCell = true;
        }

        bool canSpawn = true;

        for (int i = 0; i < cellXWidth; i++)
        {
            for (int j = 0; j < cellzHeight; j++)
            {
                if (_cellMatrix[(x + i), (z + j)] != null)
                {
                    canSpawn = false;
                    break;
                }
                if (!isTransitionCell && (x+i) == _secondFloorX && (z+j) == _secondFloorZ)
                {
                    canSpawn = false;
                    break;
                }
                if (_isSecondFloor)
                {
                    //if (transitionCellDimensions == null)
                    //{
                    //    transitionCellDimensions = this.GetComponentInParent<CellSpawner>().transitionCellDimensions;
                    //}
                    //transitionCellDimensions = this.GetComponentInParent<CellSpawner>().GetTransitionalCellDimensions();
                    Debug.Log("Transitional cell Dismension: " + transitionCellDimensions[0] + ", " + transitionCellDimensions[1]);
                        
                    //try
                        //{
                        //    //transitionCell = new Cell();
                        //    //_cellMatrixBool = this.GetComponentInParent<CellSpawner>()._cellMatrixBool;
                        //    this.transitionCell = this.GetComponentInParent<CellSpawner>().transitionCell;
                        //}
                        //catch { Debug.Log("NoParent Later"); }


                    Debug.Log("isTopFloor");
                    for (int a = 0; a < transitionCellDimensions[0]; a++)
                    {
                        for (int b = 0; b < transitionCellDimensions[1]; b++)
                        {
                            Debug.Log("Looking at cell: " + (x+i) + ", " + (z+j) + " // " + a + ", " + b);
                            if (x+i == a && z+j == b && (cellXWidth > 1 || cellzHeight > 1))
                            {
                                canSpawn = false;
                                Debug.Log("Couldn't spawn big at " + (x + i) + ", " + (z + j));
                                break;
                            }
                        }
                    }
                }
            }
        }

        return canSpawn;    
    }

    public int[] GetTransitionalCellDimensions()
    {
        Debug.Log("Passing Back Trans Cell: " + transitionCellDimensions[0] + ", " + transitionCellDimensions[1]);
        return transitionCellDimensions; 
    }

    public void SetTransitionalCellDImenstions(int[] parentDimensions, Cell transCell)
    {
        transitionCellDimensions = new int[parentDimensions.Length];
        transitionCellDimensions[0] = parentDimensions[0];
        transitionCellDimensions[1] = parentDimensions[1];
        Start_X = transCell.transCellExitX;
        Start_Z = transCell.transCellExitZ;

        Debug.Log("Trans Cell Start: " + Start_X + ", " + Start_Z);

        for (int x = 0; x < transitionCellDimensions[0]; x++)
        {
            for (int z = 0; z < transitionCellDimensions[1]; z++)
            {
                if (x != Start_X || z != Start_Z)
                {
                    Debug.Log("Checking off Cell: " + x + ", " + z);
                    _cellMatrixBool[x, z] = true;
                }
                else
                    Debug.Log("Kept Cell: " + x + ", " + z);

            }
        }


    }

    public int GetStartXWidth()
    { 
        return X_Width; 
    }

    public int GetStartZHeight()
    {
        return Z_Height;
    }

    public int GetSecondFloorX()
    {
        return _secondFloorX;
    }
    public int GetSecondFloorZ()
    {
        return _secondFloorZ;
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
