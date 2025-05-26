using System.Collections;
using UnityEngine;

public class CellSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _1X1Prefab;

    [SerializeField]
    private GameObject[] _prefabs; // 1x1, 2x1, 1x2, 2x2

    private GameObject[,] _cellMatrix; // [x,z]

    private GameObject[] _XContainers;
    public int X_Width = 10;
    public int Z_Height = 10;

    [SerializeField]
    private MazeGenerator _generator;

    private void Awake()
    {
        if (_cellMatrix != null)
        {
            _cellMatrix = null;
        }

        _cellMatrix = new GameObject[X_Width, Z_Height];

        if (_XContainers == null)
        {
            _XContainers = new GameObject[X_Width];
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
        _generator.GenerateMaze(_cellMatrix, X_Width, Z_Height);
    }

    private void SetMatrixID()
    {
        for (int x = 0; x < X_Width; x++)
        {
            for (int z = 0; z < Z_Height; z++)
            {
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

    // primitive version of choose random cell size for cells.
    private void SpawnCell(int x, int z)
    {
        int choice = GetRandomNumber(0, 10);

        // occupies 2 spaces in x axis 2x1
        if (choice == 0 && x + 1 < X_Width && _cellMatrix[(x + 1), z] == null)
        {
            _cellMatrix[x, z] = Instantiate(_prefabs[1], _XContainers[x].transform);
            _cellMatrix[x, z].transform.position = new Vector3(x * 10f, 0, z * 10f);
            _cellMatrix[x, z].name = "Cell " + x.ToString() + ", " + z.ToString();

            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z); //
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x, z); //

            _cellMatrix[(x + 1), z] = _cellMatrix[x, z];
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x+1, z);
        }
        // 2 spaces in Y axis 1x2
        else if (choice == 1 && z + 1 < Z_Height && _cellMatrix[x, (z + 1)] == null)
        {
            _cellMatrix[x, z] = Instantiate(_prefabs[2], _XContainers[x].transform);
            _cellMatrix[x, z].transform.position = new Vector3(x * 10f, 0, z * 10f);
            _cellMatrix[x, z].name = "Cell " + x.ToString() + ", " + z.ToString();

            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z); //
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x, z);

            _cellMatrix[x, (z+1)] = _cellMatrix[x, z];
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x, z+1);
        }
        // 4 spaces in x and y axis 2x2
        else if ((choice == 2 && x + 1 < X_Width && z + 1 < Z_Height) && _cellMatrix[(x + 1), z] == null && _cellMatrix[x, (z + 1)] == null && _cellMatrix[(x+1), (z + 1)] == null)
        {
            _cellMatrix[x, z] = Instantiate(_prefabs[3], _XContainers[x].transform);
            _cellMatrix[x, z].transform.position = new Vector3(x * 10f, 0, z * 10f);
            _cellMatrix[x, z].name = "Cell " + x.ToString() + ", " + z.ToString();

            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z); //
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x, z);

            _cellMatrix[(x + 1), z] = _cellMatrix[x, z];
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x+1, z);

            _cellMatrix[x, (z + 1)] = _cellMatrix[x, z];
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x, z+1);

            _cellMatrix[(x+1), (z + 1)] = _cellMatrix[x, z];
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x+1, z+1);
        }
        else
        {
            _cellMatrix[x, z] = Instantiate(_prefabs[0], _XContainers[x].transform);
            _cellMatrix[x, z].transform.position = new Vector3(x * 10f, 0, z * 10f);
            _cellMatrix[x, z].name = "Cell " + x.ToString() + ", " + z.ToString();

            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArrayList(x, z);
            _cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z);   
        }

        //_cellMatrix[x, z].GetComponent<Cell>().SetMatrixID(x,z);

        //_cellMatrix[x, z].GetComponent<Cell>().SetIndexInArray(x, z);
    }

    private int GetRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
