using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using System.Xml.Linq;
using System.Buffers.Text;
using System.Collections;
using NUnit.Framework;

public class MazeFloor : MonoBehaviour
{
    public GameObject startCell;
    public GameObject emptyCell;

    [SerializeField, Tooltip("Must include atleast one 1x1 cell.")]
    private GameObject[] _prefabs;

    [SerializeField, Tooltip("The likeleyHood the prefab at that index will be chosen. Also always includes 1st Prefab.")]
    private int[] _oddsOfChoosePrefab;

    [SerializeField, Tooltip("Prefabs to be used when moving to second floor.")]
    private GameObject[] _verticleTransitionPrefabs;

    private GameObject[,] _cellMatrix;
    private bool[,] _cellMatrixBool;
    private GameObject[] _XContainers;
    public GameObject specialCells;

    public int xWidth = 10;
    public int zHeight = 10;

    public int startX = 0;
    public int startZ = 0;

    public int floorLevel = 0;

    [Tooltip("Next Floor X & Z relative to this ones matrix.")]
    public bool hasNextFloor;
    public int nextFloorX = -1;
    public int nextFloorZ = -1;

    [Tooltip("Previous Floor X & Z relative to previous ones matrix.")]
    private bool _hasPrevFloor;
    private int _prevFloorX = 0;
    private int _prevFloorZ = 0;
    private int[] _prevFloorExit;
    private List<int[]> _prevFloorTransitionCells;

    private void Awake()
    {
        _hasPrevFloor = false;
        _prevFloorExit = new int[2];
        _prevFloorTransitionCells = new List<int[]>();
        //SetFloorDimensions();
        //CheckPrefabListAndOdds();
        //SpawnXContainers();

        //if (hasNextFloor)
        //    SetNextFloorTransition();
    }

    public void InitializeFloor(int width, int height)
    {
        SetFloorDimensions(width, height);
        CheckPrefabListAndOdds();
        SpawnXContainers();
    }

    private void SetFloorDimensions(int width, int height)
    {
        if (width >=1 || height >= 1)
        {
            xWidth = width;
            zHeight = height;
        }

        _cellMatrix = new GameObject[xWidth, zHeight];
        _cellMatrixBool = new bool[xWidth, zHeight];
        //_XContainers = new GameObject[xWidth];
    }

    private void SpawnXContainers()
    {
        _XContainers = new GameObject[xWidth];
        for (int i = 0; i < xWidth; i++)
        {
            _XContainers[i] = new GameObject(i.ToString() + ": ");
            _XContainers[i].transform.parent = this.transform;
        }
    }

    private void CheckPrefabListAndOdds()
    {
        if (_prefabs.Length <= 0)
            Debug.LogError("No Prefabs in List!");
        if (_verticleTransitionPrefabs.Length <= 0)
            Debug.LogError("No Transitional Prefabs in List!");
        if (_oddsOfChoosePrefab.Length < _prefabs.Length)
        {
            Debug.Log("Prefab odds list < prefabs list.");
            PopulatePrefabOdds();
        }
    }

    private void PopulatePrefabOdds()
    {
        _oddsOfChoosePrefab = null;
        _oddsOfChoosePrefab = new int[_prefabs.Length];

        for (int i = 0; i < _prefabs.Length; i++)
            _oddsOfChoosePrefab[i] = 1;
    }

    public void SetPrevFloorData(Cell transitionCell, int transitionX, int transitionZ, int prevFloorLevel)
    {
        //Debug.Log("Trans Cell Passed: " + transitionCell.name);

        _hasPrevFloor = true;
        _prevFloorX = transitionX;
        _prevFloorZ = transitionZ;
        int transCellXWidth = transitionCell.GetCellXWidth();
        int transCellZHeight = transitionCell.GetCellZHeight();
        floorLevel = prevFloorLevel + 1;
        int[] tempTransCell;

        int newFloorXPos = transitionX - 1;    //
        int newFloorZPos = transitionZ - 1;    //

        int[] transCellExit = new int[2];
        _prevFloorExit[0] = transitionCell.transCellExitX + newFloorXPos; //
        _prevFloorExit[1] = transitionCell.transCellExitZ + newFloorZPos; //

        for (int x = 0; x < transCellXWidth; x++)
        {
            for (int z = 0; z < transCellZHeight; z++) 
            {
                tempTransCell = new int[2];
                tempTransCell[0] = x + newFloorXPos;    //
                tempTransCell[1] = z + newFloorZPos;    //
                _prevFloorTransitionCells.Add(tempTransCell);
                //Debug.Log("Adding Trans Cell: " + tempTransCell[0] + ", " + tempTransCell[1]);
            }
        }
        Debug.Log("New Floor Entrance: " + _prevFloorExit[0] + ", " + _prevFloorExit[1]);

        this.transform.position = new Vector3(floorLevel * 10, floorLevel * 10, floorLevel * 10);
    }

    public void SpawnFloor()
    {
        Debug.Log("Creating Floor: " + floorLevel + " ----------------");
        if (hasNextFloor)
        { 
            LocateNextFloor();
        }

        if (nextFloorX == -1 ||  nextFloorZ == -1)
        {
            hasNextFloor = false;
        }

        if (!_hasPrevFloor)
        {
            SpawnStartCell(startX, startZ);
        }
        //else
        //{
        //    SpawnEmptyCell(startX, startZ);
        //}

        SpawnCells();
        StartCoroutine(WaitForABit());
    }

    public IEnumerator WaitForABit()
    {
        yield return new WaitForSeconds(1f);
        SetCellMatrixIds();

        if (_hasPrevFloor)
            SetVisitTransitionCells();
    }

    private void SetCellMatrixIds()
    {
        for (int x = 0; x < xWidth; x++)
        {
            for (int z = 0; z < zHeight; z++)
            {
                _cellMatrix[x, z].GetComponent<Cell>().SetMatrixID(x, z);
            }
        }
    }

    private void SpawnCells()
    {
        //Debug.Log("Spawning Cells");
        for (int i = 0; i < xWidth; i++)
        {
            for (int j = 0; j < zHeight; j++)
            {
                if (_cellMatrix[i, j] == null)
                {
                    if (CheckIfCellAbovePrevFloorTransitionals(i, j))
                    {
                        //Debug.Log("Spawning Empty Cell at: " + i + ", " + j);
                        SpawnEmptyCell(i, j);
                    }
                    else if (CellIsTransitional(i, j))
                    {
                        //Debug.Log("Next floor trans pos: " + i + ", " + j);
                        SpawnTransitionalCell(i, j);
                    }
                    else
                        SpawnNormalCell(i, j);
                }                
            }
        }
    }

    private void SpawnStartCell(int x, int z)
    {
        SpawnCell(startCell, x, z, specialCells, ("Start Cell:  " + x.ToString() + ", " + z.ToString()));
    }

    private void SpawnEmptyCell(int x, int z)
    {
        //Debug.Log("Spawning Empty Cell at: " + x.ToString() + ", " + z.ToString());
        SpawnCell(emptyCell, x, z, specialCells, ("Above Trans Cell:  " + x.ToString() + ", " + z.ToString()));
    }

    private void SpawnTransitionalCell(int x, int z)
    {
        //Debug.Log("Spawning Trans Cell at: " + x + ", " + z);
        bool cellSpawned = false;
        List<int> possibleCells = new List<int>();
        possibleCells = GetListOfPossiblePrefabs(_verticleTransitionPrefabs, true);
        possibleCells = RandomizeIntList(possibleCells);

        int potentialCellIndex;

        while (!cellSpawned && possibleCells.Count > 0)
        {
            potentialCellIndex = possibleCells[0];
            cellSpawned = CanSpawnCell(x, z, potentialCellIndex, true);

            if (!cellSpawned)
                possibleCells = RemoveDuplicateIntsFromList(possibleCells, potentialCellIndex);
            else
                SpawnCell(_verticleTransitionPrefabs[potentialCellIndex], x, z, specialCells, (x.ToString() + ", " + z.ToString()));

            //if (!CanSpawnCell(x, z, possibleCells, false))
        }

        if (possibleCells.Count <= 0)
        {
            Debug.LogError("Couldn't Find acceptable cell!!");
        }
    }

    private void SpawnNormalCell(int x, int z)
    {
        bool cellSpawned = false;

        List<int> possibleCells = new List<int>();
        possibleCells = GetListOfPossiblePrefabs(_prefabs, false);
        possibleCells = RandomizeIntList(possibleCells);

        int potentialCellIndex;

        while (!cellSpawned && possibleCells.Count > 0) 
        {
            potentialCellIndex = possibleCells[0];
            cellSpawned = CanSpawnCell(x, z, potentialCellIndex, false);

            if (!cellSpawned)
                possibleCells = RemoveDuplicateIntsFromList(possibleCells, potentialCellIndex);
            else
                SpawnCell(_prefabs[potentialCellIndex], x, z, _XContainers[x], (x.ToString() + ", " + z.ToString()));
            //if (!CanSpawnCell(x, z, possibleCells, false))
        }

        if (possibleCells.Count <= 0)
        {
            Debug.LogError("Couldn't Find acceptable cell!!");
        }
        //Cell potentialPrefab = _prefabs[possibleCells[possibleCells.Count - 1]].GetComponent<Cell>();

        //if (CellIsInBounds(x,z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))

    }

    private bool CanSpawnCell(int x, int z, int possibleCellIndex, bool isTransitional)
    {
        bool result = true;

        Cell potentialPrefab;
        if (!isTransitional)
            potentialPrefab = _prefabs[possibleCellIndex].GetComponent<Cell>();
        else
            potentialPrefab = _verticleTransitionPrefabs[possibleCellIndex].GetComponent<Cell>();

        if (!CellIsInBounds(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
            result =  false;
        else if (!CellHasSpaceToSpawn(x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight(), isTransitional))
            result = false;
        else if (!CanSpawnDeadCell(potentialPrefab, x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight()))
            result = false;


        return result;

    }

    private bool CellIsTransitional(int x, int z)
    {
        if (!hasNextFloor)
            return false;
        
        return x == nextFloorX && z == nextFloorZ;
    }

    private bool CheckIfCellAbovePrevFloorTransitionals(int x, int z)
    {
        return IntPairIsInList(x, z, _prevFloorTransitionCells);
    }

    private bool CanSpawnDeadCell(Cell potentialCell, int x, int z, int _xWidth, int _zHeight)
    {
        if (!potentialCell.HasDeadCell())
            return true;

        bool result = true;

        for (int i = x; i < (x + _xWidth); i++)
        {
            for (int j = z; j < (z + _zHeight); j++)
            {
                if (!_hasPrevFloor && IntWithin3_OfOtherInt(i, startX) && IntWithin3_OfOtherInt(j, startZ))
                {
                    result = false;
                    break;            
                }
                if (_hasPrevFloor && (IntWithin3_OfOtherInt(i, _prevFloorExit[0]) && IntWithin3_OfOtherInt(j, _prevFloorExit[1])))
                {
                    result = false;
                    break;
                }
                if (hasNextFloor && (IntWithin3_OfOtherInt(i, nextFloorX) && IntWithin3_OfOtherInt(j, nextFloorZ)))
                {
                    result = false;
                    break;
                }
            }
        }

        return result;  
    }

    private bool IntWithin3_OfOtherInt(int a, int b)
    {
        return (a <= (b + 3) && a >= (b - 3));
    }

    private List<int> GetListOfPossiblePrefabs(GameObject[] prefabList, bool isTransitional)
    {
        if (prefabList.Length <= 0)
            return new List<int>();

        List<int> potentialPrefabs = new List<int>();

        for (int i = 0; i < prefabList.Length; i++)
        {
            if (!isTransitional)
            {
                for (int j = 0; j < _oddsOfChoosePrefab.Length; j++)
                {
                    potentialPrefabs.Add(i);
                }
            }
            else
                potentialPrefabs.Add(i);
        }

        return potentialPrefabs;
    }
    


    private List<int> RemoveDuplicateIntsFromList(List<int> list, int value)
    {
        if (list.Count <= 0)
            return new List<int>();

        for (int i = (list.Count-1); i >= 0; i--)
            if (list[i] == value)
                list.RemoveAt(i);

        return list;
    }

    private bool CellIsInBounds(int x, int z, int _xWidth, int _zHeight)
    {
        return !(x < 0 || z < 0 || (x + _xWidth - 1) >= xWidth || (z + _zHeight - 1) >= zHeight);
    }

    private bool CellHasSpaceToSpawn(int x, int z, int _xWidth, int _zHeight, bool isTransitional)
    {
        bool result = true;

        for (int i = x; i < (x + _xWidth); i++)
        {
            for (int j = z; j < (z + _zHeight); j++)
            {
                if (_cellMatrix[i, j] != null)
                {
                    result = false;
                    break;
                }
                if (!isTransitional && i == nextFloorX && j == nextFloorZ)
                {
                    result = false;
                    break;
                }
                if (_hasPrevFloor && CheckIfCellAbovePrevFloorTransitionals(i, j))
                {
                    result = false;
                    break;
                }
            }
        }
        return result;
    }

    private void SpawnCell(GameObject cellPrefab, int x, int z, GameObject parent, string _name)
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab is NULL.");
            return;
        }

        _cellMatrix[x,z] = Instantiate(cellPrefab, parent.transform);

        MoveCellPosition(x, z);
        SetCellName(x, z, _name);
        SetCellIndexInArray(x, z);
        AddPointersToUsedCells(x, z);
        MarkDeadCells(_cellMatrix[x, z].GetComponent<Cell>(), x, z); 
    }

    private void SetCellIndexInArray(int x, int z)
    {
        _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z);
    }

    private void AddPointersToUsedCells(int x, int z)
    {
        Cell newCell = _cellMatrix[x, z].GetComponent<Cell>();
        int newCellXWidth = newCell.GetComponent<Cell>().GetCellXWidth();
        int newCellZHeight = newCell.GetComponent<Cell>().GetCellZHeight();

        for (int i = 0; i < newCellXWidth; i++)
        {
            for (int j = 0; j < newCellZHeight; j++)
            {
                _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x + i, z + j);
                _cellMatrix[x + i, z + j] = _cellMatrix[x, z];
            }
        }
    }

    private void MoveCellPosition(int x, int z)
    {
        _cellMatrix[x, z].transform.localPosition = new Vector3(x*10, 0, z*10);
    }

    private void SetCellName(int x, int z, string _name)
    {
        _cellMatrix[x, z].name = _name;
    }

    public void LocateNextFloor()
    {
        if (hasNextFloor)
        {
            if (xWidth < 8 || zHeight < 8)
            {
                nextFloorX = -1;
                nextFloorZ = -1;
                return;
            }

            while (nextFloorX == -1 || nextFloorZ == -1 ||
                IntPairIsInList(nextFloorX, nextFloorZ, _prevFloorTransitionCells))
            {
                nextFloorX = GetRandomNumber(4, xWidth - 3);   //
                nextFloorZ = GetRandomNumber(4, zHeight - 3);  //
            }
        }
        Debug.Log("Current Floor Entrance to next at: " + nextFloorX + ", " + nextFloorZ);
    }

    private void MarkDeadCells(Cell currentCell, int x, int z)
    {
        if (!currentCell.HasDeadCell())
            return;

        for (int i = 0; i < currentCell.GetNumDeadCells(); i++)
        {
            _cellMatrixBool[currentCell.GetDeadCellX(i), currentCell.GetDeadCellZ(i)] = true;
        }

    }


    private void SetVisitTransitionCells()
    {
        for (int i = 0; i < _prevFloorTransitionCells.Count; i++)
        {
            //Debug.Log("Prev Floor Exit: " + _prevFloorExit[0] +", " + _prevFloorExit[1]);
            //Debug.Log("Prev Floor Trans Cell: " + _prevFloorTransitionCells[i][0] + ", " + _prevFloorTransitionCells[i][1]);
            if (_prevFloorTransitionCells[i][0] != _prevFloorExit[0] ||
                _prevFloorTransitionCells[i][1] != _prevFloorExit[1])
            {
                //Debug.Log("Visiting Cell:  + " + _prevFloorTransitionCells[i][0] + ", " + _prevFloorTransitionCells[i][1]);
                _cellMatrixBool[_prevFloorTransitionCells[i][0], _prevFloorTransitionCells[i][1]] = true;
            }
            else
            {
                startX = _prevFloorTransitionCells[i][0];
                startZ = _prevFloorTransitionCells[i][1];
                //Debug.Log("New Start At: " +  startX + ", " + startZ);
            }
        }
    }

    public void DisableCellsAboveTransitional()
    {
        for (int i = 0; i < _prevFloorTransitionCells.Count; i++)
        {
            //Debug.Log("Disabling Cell above trans at: " + _prevFloorTransitionCells[i][0] + ", " + _prevFloorTransitionCells[i][1]);
            _cellMatrix[_prevFloorTransitionCells[i][0], _prevFloorTransitionCells[i][1]].GetComponent<Cell>().DisableCell();
        }
    }

    private int GetRandomNumber(int min, int max)
    {
        if (min >= max)
        {
            Debug.LogError("Cant Spawn Transition cell");
            return 1;
        }
        return UnityEngine.Random.Range(min, max);
    }

    private bool IntPairIsInList(int[] pair, List<int[]> list)
    {
        bool result = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (pair[0] == list[i][0] && pair[1] == list[i][1])
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private bool IntPairIsInList(int x, int z, List<int[]> list)
    {
        bool result = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (x == list[i][0] && z == list[i][1])
            {
                //Debug.Log("PairInList");
                result = true;
                break;
            }
        }

        return result;
    }

    private List<int> RandomizeIntList(List<int> list)
    {
        if (list.Count <= 0)
            return new List<int>();

        return list.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    public bool[,] GetCellMatrixBool()
    {
        return _cellMatrixBool;
    }

    public GameObject[,] GetCellMatrix()
    {
        return _cellMatrix;
    }

    public int GetFloorXWidth()
    {
        return xWidth;
    }

    public int GetFloorZHeight()
    {
        return zHeight;
    }

    public int GetStartX()
    {
        return startX;
    }

    public int GetStartZ()
    {
        return startZ;
    }

    public Cell GetTransitionalCell()
    {
        return _cellMatrix[nextFloorX, nextFloorZ].GetComponent<Cell>();
    }

    public int GetTransitionX()
    {
        return nextFloorX;
    }
    public int GetTransitionZ()
    {
        return nextFloorZ;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}

