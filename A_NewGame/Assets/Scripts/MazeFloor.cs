using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Buffers.Text;
using System.Collections;
using NUnit.Framework;
using System.Linq.Expressions;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine.AI;

public class MazeFloor : MonoBehaviour
{
    public GameObject startCell;
    public GameObject emptyCell;
    public GameObject emptyFloorCell;

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
    public int yDepth = 1;

    public int startX = 0;
    public int startZ = 0;

    public int floorLevel = 0;

    private List<int[]> _startCells;

    [Tooltip("Next Floor X & Z relative to this ones matrix.")]
    public bool hasNextFloor;
    public int nextFloorX = -1;
    public int nextFloorZ = -1;
    private int nextFloorXWidth = -1;
    private int nextFloorZHeight = -1;

    [Tooltip("Previous Floor X & Z relative to previous ones matrix.")]
    private bool _hasPrevFloor;
    private int _prevFloorX = 0;
    private int _prevFloorZ = 0;
    private int[] _prevFloorExit;
    private List<int[]> _prevFloorTransitionCells;

    public bool isEmptyFloor;

    public LayerMask navMeshSurfaceLayers;

    [SerializeField]
    private GameObject _floorNavMeshSurface;

    private void Awake()
    {
        _hasPrevFloor = false;
        _prevFloorExit = new int[2];
        _prevFloorTransitionCells = new List<int[]>();
        _startCells = new List<int[]>();


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
        if (hasNextFloor && _verticleTransitionPrefabs.Length <= 0)
            Debug.LogError("No Transitional Prefabs in List!");
        if (_oddsOfChoosePrefab.Length < _prefabs.Length)
        {
            Debug.Log("Prefab odds list < prefabs list.");
            PopulatePrefabOdds();
        }
    }

    public void SetStartCells()
    {
        Cell start = startCell.GetComponent<Cell>();

        int[] cellPos;
        for (int x = startX; x < (startX+ start.GetCellXWidth()); x++ )
        {
            for (int z = startZ; z < (startZ + start.GetCellZHeight()); z++ )
            {
                cellPos = new int[2];
                cellPos[0] = x;
                cellPos[1] = z;
                _startCells.Add(cellPos);
            }
        }

        Debug.Log("StartCells: ");
        for(int i = 0; i < _startCells.Count; i++) 
        {
            Debug.Log(_startCells[i][0] + ", " + _startCells[i][1]);
        }
    }

    private void PopulatePrefabOdds()
    {
        _oddsOfChoosePrefab = null;
        _oddsOfChoosePrefab = new int[_prefabs.Length];

        for (int i = 0; i < _prefabs.Length; i++)
            _oddsOfChoosePrefab[i] = 1;
    }

    public void SetPrevFloorData(MazeFloor prevFloor, Cell transitionCell, int transitionX, int transitionZ, int prevFloorLevel)
    {
        //Debug.Log("Trans Cell Passed: " + transitionCell.name);

        _hasPrevFloor = true;

        _prevFloorX = transitionX;
        _prevFloorZ = transitionZ;

        // Get Width / Height of previous floor
        int prevFloorXWidth = prevFloor.GetFloorXWidth();       //
        int prevFloorZHeight = prevFloor.GetFloorZHeight();     //

        // get actual x & z position of previous floor
        int prevFloorXPos = (prevFloor.GetFloorXPos()/10);
        int prevFloorZPos = (prevFloor.GetFloorZPos()/10);

        // find difference between width / height of current & previous floor
        int xWidthDelta = (prevFloorXWidth - xWidth)/2;     //
        int zHeightDelta = (prevFloorZHeight - zHeight)/2;  //

        // if previous floor not starting at (0,0), move new floor so it's centered above others
        int newFloorStartXPos = xWidthDelta + prevFloorXPos;
        int newFloorStartZPos = zHeightDelta + prevFloorZPos;
        int newFloorStartingY = (prevFloor.GetFloorYPos() / 10) + prevFloor.GetFloorYDepth();

        int transCellXWidth = transitionCell.GetCellXWidth();
        int transCellZHeight = transitionCell.GetCellZHeight();
        floorLevel = prevFloorLevel + 1;
        int[] tempTransCell;

        int newFloorXPos = transitionX - xWidthDelta;    //
        int newFloorZPos = transitionZ - zHeightDelta;    //

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

        //this.transform.position = new Vector3(floorLevel * 10, floorYDepth * 10, floorLevel * 10);
        this.transform.position = new Vector3(10* newFloorStartXPos, newFloorStartingY * 10, 10 * newFloorStartZPos);
    }

    public void SetNextFloorDimensions(int x, int z)
    {
        nextFloorXWidth = x;
        nextFloorZHeight = z;
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
        if (hasNextFloor)
        {
            SpawnTransitionalCell(nextFloorX, nextFloorZ);
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

        //if (isEmptyFloor)
            //DisableEmptyFloorCellWalls();

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
                    else
                    {
                        if (!isEmptyFloor)
                            SpawnNormalCell(i, j);
                        else
                            SpawnEmptyFloorCells(i, j);
                    }
                    //else if (CellIsTransitional(i, j))
                    //{
                    //    //Debug.Log("Next floor trans pos: " + i + ", " + j);
                    //    SpawnTransitionalCell(i, j);
                    //}
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

    private void SpawnEmptyFloorCells(int x, int z)
    {
        if (emptyFloorCell == null)
            Debug.LogError("EmptyFloorCell no set!!");
        SpawnCell(emptyFloorCell, x, z, _XContainers[x], (x.ToString() + ", " + z.ToString()));

        //if (x > 0 && x < xWidth - 1 && z > 0 && z < zHeight - 1)
        //{
        //    _cellMatrix[x, z].GetComponent<Cell>().DisableAllWalls();
        //    return;
        //}

        //if (x > 0)
        //    _cellMatrix[x, z].GetComponent<Cell>().DisableNegXWalls();
        //if (z > 0)
        //    _cellMatrix[x, z].GetComponent<Cell>().DisableNegZWalls();
        //if (x < xWidth - 1)
        //    _cellMatrix[x, z].GetComponent<Cell>().DisablePosXWalls();
        //if (z < zHeight - 1)
        //    _cellMatrix[x, z].GetComponent<Cell>().DisablePosZWalls();
    }

    public void DisableEmptyFloorCellWalls()
    {
        for (int i = 0; i < xWidth; i++)
        {
            for (int j = 0; j < zHeight; j++)
            {
                if (CheckIfCellAbovePrevFloorTransitionals(i, j))
                {
                    _cellMatrix[i, j].GetComponent<Cell>().DisableAllWalls(true);
                    _cellMatrix[i, j].GetComponent<Cell>().DisableFloors();
                }
                else if (!CellIsTransitional(i, j))
                {
                    if (i > 0 && i < xWidth - 1 && j > 0 && j < zHeight - 1)
                    {
                        _cellMatrix[i, j].GetComponent<Cell>().DisableAllWalls(true);
                        continue;
                    }

                    if (i > 0)
                        _cellMatrix[i, j].GetComponent<Cell>().DisableNegXWalls(true);
                    if (j > 0)
                        _cellMatrix[i, j].GetComponent<Cell>().DisableNegZWalls(true);
                    if (i < xWidth - 1)
                        _cellMatrix[i, j].GetComponent<Cell>().DisablePosXWalls(true);
                    if (j < zHeight - 1)
                        _cellMatrix[i, j].GetComponent<Cell>().DisablePosZWalls(true);
                }
                
            }
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
        else if (!CanSpawnDeadCell(potentialPrefab, x, z, potentialPrefab.GetCellXWidth(), potentialPrefab.GetCellZHeight(), isTransitional))
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

    private bool CanSpawnDeadCell(Cell potentialCell, int x, int z, int _xWidth, int _zHeight, bool isTransitional)
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
                if (hasNextFloor && !isTransitional && (IntWithin3_OfOtherInt(i, nextFloorX) && IntWithin3_OfOtherInt(j, nextFloorZ)))
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
        string suffix = _cellMatrix[x, z].GetComponent<Cell>().GetCellXWidth().ToString();
        suffix += ", " + _cellMatrix[x, z].GetComponent<Cell>().GetCellZHeight().ToString();
        _cellMatrix[x, z].name = _name +" -- " +  suffix;
    }

    public void LocateNextFloor()
    {
        //(condition) ? expressionTrue : expressionFalse;


        if (hasNextFloor)
        {
            int nextFloorMinX = (nextFloorXWidth < xWidth) ? (xWidth - nextFloorXWidth) / 2 : 0;
            int nextFloorMaxX = (nextFloorXWidth < xWidth) ? nextFloorXWidth : xWidth;
            nextFloorMaxX += nextFloorMinX;
            nextFloorMinX += 2;
            nextFloorMaxX -= 3;

            int nextFloorMinZ = (nextFloorZHeight < zHeight) ? (zHeight - nextFloorZHeight) / 2 : 0;
            int nextFloorMaxZ = (nextFloorZHeight < zHeight) ? nextFloorZHeight : zHeight;
            nextFloorMaxZ += nextFloorMinZ;
            nextFloorMinZ += 2;
            nextFloorMaxZ -= 3;

            Debug.Log("Next Floor min: " + nextFloorMinX + ", " + nextFloorMinZ);
            Debug.Log("Next Floor Max: " + nextFloorMaxX + ", " + nextFloorMaxZ);

            int counter = 0;
            //if (xWidth < 8 || zHeight < 8)
            //{
            //    nextFloorX = -1;
            //    nextFloorZ = -1;
            //    return;
            //}
            if ((nextFloorMaxX - nextFloorMinX) <= 2 || (nextFloorMaxZ - nextFloorMinZ) <= 2)
            {
                nextFloorX = -1;
                nextFloorZ = -1;
                hasNextFloor = false;
                Debug.LogError("Couldn't Add Next Floor");
                return;
            }

            Debug.Log("Trying Find next Floor: ");

            while (nextFloorX == -1 || nextFloorZ == -1 ||
                IntPairWithin3OfList(nextFloorX, nextFloorZ, _prevFloorTransitionCells) ||
                IntPairWithin3OfList(nextFloorX, nextFloorZ, _startCells))
            {
                if (counter >= 15)
                {
                    nextFloorX = -1;
                    nextFloorZ = -1;
                    hasNextFloor = false;
                    Debug.LogError("Couldn't Find Acceptable next floor pos");
                    return;
                }

                nextFloorX = GetRandomNumber(nextFloorMinX, nextFloorMaxX);   //
                nextFloorZ = GetRandomNumber(nextFloorMinZ, nextFloorMaxZ);  //
                Debug.Log("Trying (" + nextFloorX + ", " + nextFloorZ + ")");
                counter++;
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

    public void AddNavmeshSurfaceData()
    {
        if (_floorNavMeshSurface == null)
            Debug.LogError("NoNavmesh surface!!!!");
        else
        {
            _floorNavMeshSurface.AddComponent<NavMeshSurface>();
            //_floorNavMeshSurface.AddComponent<NavMeshModifierVolume>();
            //_floorNavMeshSurface.transform.localScale = new Vector3(xWidth, 1f, zHeight);
            _floorNavMeshSurface.transform.localPosition = new Vector3((10 * (xWidth - 1)) / 2, 0, (10 * (zHeight - 1)) / 2);
            _floorNavMeshSurface.name = floorLevel.ToString() + ": NavMeshSurface";
            _floorNavMeshSurface.GetComponent<NavMeshSurface>().layerMask = navMeshSurfaceLayers;
            _floorNavMeshSurface.GetComponent<NavMeshSurface>().collectObjects = CollectObjects.Volume;       //
            _floorNavMeshSurface.GetComponent<NavMeshSurface>().center = Vector3.zero;
            //_floorNavMeshSurface.GetComponent<NavMeshModifierVolume>().size = new Vector3(xWidth*10, 1f, zHeight*10);
            _floorNavMeshSurface.GetComponent<NavMeshSurface>().size = new Vector3(xWidth*10, 1f, zHeight*10);
            //_floorNavMeshSurface.GetComponent<NavMeshSurface>().minRegionArea = 8;    //
            //_floorNavMeshSurface.GetComponent<NavMeshSurface>().GetComponent<NavMeshModifierVolume>().size = new Vector3(xWidth*10, 1f, zHeight*10);
            _floorNavMeshSurface.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }

    public void DisableFloorRenderers(int playerFloorLevel)
    {
        for (int x = 0; x < xWidth; x++)
        {
            for (int z = 0; z < zHeight; z++)
            {
                if (_hasPrevFloor && playerFloorLevel == (floorLevel - 1) && IntPairWithin3OfList(x,z,_prevFloorTransitionCells))
                {
                    if (_cellMatrix[x, z] != null && !_cellMatrix[x, z].GetComponent<Cell>().CellRenderersEnabled())
                    {
                        _cellMatrix[x, z].GetComponent<Cell>().EnableCellRenderers();
                    }
                    continue;
                }
                //if (IntPlusMinusEqualToOther(playerFloorLevel, floorLevel) && floorLevel == 0 && IntPairWithin3OfList(x, z, _startCells))
                    //continue;
                if (hasNextFloor && playerFloorLevel == (floorLevel + 1) && IntWithin3_OfOtherInt(x, nextFloorX) && IntWithin3_OfOtherInt(z, nextFloorZ))
                {
                    if (_cellMatrix[x, z] != null && !_cellMatrix[x, z].GetComponent<Cell>().CellRenderersEnabled())
                    {
                        _cellMatrix[x, z].GetComponent<Cell>().EnableCellRenderers();
                    }
                    continue;
                }
                if (playerFloorLevel != floorLevel && _cellMatrix[x, z] != null && _cellMatrix[x, z].GetComponent<Cell>().CellRenderersEnabled())
                {
                    _cellMatrix[x, z].GetComponent<Cell>().DisableCellRenderers();
                }
            }
        }
    }

    public void EnableFloorRenderers()
    {
        for (int x = 0; x < xWidth; x++)
        {
            for (int z = 0; z < zHeight; z++)
            {
                if (_cellMatrix[x, z] != null && !_cellMatrix[x, z].GetComponent<Cell>().CellRenderersEnabled())
                {
                    _cellMatrix[x, z].GetComponent<Cell>().EnableCellRenderers();
                }
            }
        }
    }

    public bool IntPlusMinusEqualToOther(int a, int b)
    {
        return (a==b || a == b-1 || a == b+1);
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

    private bool IntPairWithin3OfList(int x, int z, List<int[]> list)
    {
        bool result = false;

        for (int i = 0; i < list.Count; i++)
        {
            //IntWithin3_OfOtherInt(x, list[i][0])
            if (IntWithin3_OfOtherInt(x, list[i][0]) && IntWithin3_OfOtherInt(z, list[i][1]))
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

    public int GetFloorYDepth()
    {
        return yDepth;
    }

    public int GetFloorXPos()
    {
        return (int) this.transform.position.x;
    }
    public int GetFloorZPos()
    {
        return (int)this.transform.position.z;
    }

    public int GetFloorYPos()
    {
        return (int)this.transform.position.y;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}

