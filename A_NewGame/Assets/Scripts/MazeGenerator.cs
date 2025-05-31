using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using NUnit.Framework;

public class MazeGenerator : MonoBehaviour
{
    // parrallel matrix to the one with acc cells, only used to check if cell been visited
    private bool[,] _cellMatrixBool; // [x,z]

    // width of the maze / matrixes (x axis - Horizontal)
    private int X_Width = 0;
    // height of the maze / matrix (z axis - verticle from birds eye view)
    private int Z_Height = 0;

    // where want starting pos to be in maze (as index, not acc vector3)
    private int Start_X = 0;
    private int Start_Z = 0;

    // calls the recursive function which acc generates the maze, also assigns variabls & cretaes bool matrix
    public void GenerateMaze(GameObject[,] cellMatrix, bool[,] cellMatrixBool, int width_X, int height_Z, int spawnX, int spawnZ)
    {
        Debug.Log("Generating New Maze ----------------");
        X_Width = width_X;
        Z_Height = height_Z;

        _cellMatrixBool = cellMatrixBool;
        //_cellMatrixBool = new bool[width_X, height_Z];
        //_cellMatrixBool = (bool[,])cellMatrixBool.Clone();

        Start_X = spawnX;
        Start_Z = spawnZ;

        Debug.Log("Maze Starting at: " + Start_X + ", " + Start_Z);
        Debug.Log("Maze Dimensions: " + width_X + ", " + height_Z);

        if (CheckIfStartPointValid())
            GenerateMazeRecursive(cellMatrix, null, Start_X, Start_Z, null);
        else
            GenerateMazeRecursive(cellMatrix, null, 0, 0, null);

    }

    private bool CheckIfStartPointValid()
    {
        return (Start_X < X_Width && Start_X  >=0 && Start_Z < Z_Height && Start_Z >= 0);
    }

    /*     void GenerateMazeRecursive(GameObject[,] cellMatrix, Cell previousCell, int x, int z)
           
      GameObject[,] cellMatrix = matrix of cell game objects (used to point to cells)
      Cell previousCell = the previous cell which was visited (null on first iteration)
      int x & int z = the x & z index of the current cell in the cell matrix [x,z]
        (the x and z index correspond to the x and z position of the center of the cell in world space
        multiplied by 10, e.g. cell [1,3] is at (10, 0, 30) in world space (x, y, z))
    
            Brief: recursively creates maze by creating list of possible next cells & veryfying it
    
    - 0. a) Check if the new cell hasn't been visited
         b) Find the "base cell" of the new cell            
    - 1. Marks current cell as visited           
    - 2. Disables the walls between the previous cell and this one (if it's not the first iteration) 
    - 2.5 Generate's the starting cell & disables walls on each side of it (more initial paths for player)
    - 3. Creates List of int pairs to hold "adjacent" cells 
            (even ones outside array bounds, e.g. if current is (1,0) -> [1,-1] [1,1] [0,0] etc.)
            (adjusts for own size, if its 2x2 then it will get 8 cells, if 1x1 only 4 etc)
    - 4. Removes cells from the list that are out of bounds, i.e. [-1,0]
    - 5. Removes cells from the list that have been visited
            (even if it's part of a 2x2 cell)
    - 5.5 Removes "dead" cells from potential cells
            (dead cells are "1x1" cells within a larger one which cannot be pathed too or from)
    - 6. Randomizes the List of candidates
    - 7. a) Checks if any candidates exist, if yes assigns a new cell object to the first candidate in the list
         b) creates a new list without the selected candidate
         c) calls itself recursivley
    - 8. Check if any cells that were previously candidates have now been visited
    - 9. Repeat steps 7 - 8 until no candidates exist       
     */

    private void GenerateMazeRecursive(GameObject[,] cellMatrix, Cell previousCell, int x, int z, int[] wallToBeDisabled)
    {
        // 0. a)
        if (x < X_Width && z < Z_Height && !_cellMatrixBool[x, z])
        {        
            Cell _currentCell = cellMatrix[x, z].GetComponent<Cell>();
            // 0. b)
            _currentCell = GetBaseCell(cellMatrix, _currentCell, ref x, ref z);

            // 1. 
            VisitCell(_currentCell, x, z);

            // 2.
            if (previousCell != null)
            {
                DisableWalls(previousCell, _currentCell, wallToBeDisabled);
            }
            // 2.5
            else
            {
                GenerateStartCell(cellMatrix, _currentCell, x, z, _currentCell.xWidth, _currentCell.zHeight);                
            }

            // 3.
            List<int[]> potentialNextCells = GetNextCells(_currentCell, x, z);
            // 4.
            potentialNextCells = RemoveOutOfBoundCells(potentialNextCells);
            // 5.
            potentialNextCells = RemoveVisitedCells(potentialNextCells);
            // 5.5
            potentialNextCells = RemoveDeadCells(potentialNextCells, _currentCell);

            // 6.
            potentialNextCells = RandomizePotentialCells(potentialNextCells);

            if (potentialNextCells == null || potentialNextCells.Count <= 0)
                return; 

            // 9. 
            while (potentialNextCells != null && potentialNextCells.Count > 0)
            {
                // 7. a)
                //Debug.Log("Next Cell Index: " + potentialNextCells[0][0] + ", " + potentialNextCells[0][1]);
                Cell _nextCell = cellMatrix[potentialNextCells[0][0], potentialNextCells[0][1]].GetComponent<Cell>();
                // 7. b) 
                List<int[]> remainingCells = potentialNextCells.Skip(1).ToList();
                // 7. c) 
                GenerateMazeRecursive(cellMatrix, _currentCell, potentialNextCells[0][0], potentialNextCells[0][1], potentialNextCells[0]);
                // 8.
                potentialNextCells = RemoveVisitedCells(remainingCells);
            }
        }
    }


    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
    }

    /*      0. b)   GetBaseCell  
     *   ref int x & ref int z used  to change them to the base cell indexes
     *   
     *      This is because a 2x2 cell might take up [0,0] [0,1] [1,0] [1,1]
     *      but all 4 cells point to [0,0] (bottom left cell) so that when finding adjacent cells
     *      you can use the cell's height / width as an offset, but the offset is only functional
     *      when used relative to the cells "base")
     */
    private Cell GetBaseCell(GameObject[,] cellMatrix, Cell currentCell, ref int x, ref int z)
    {
        int[] baseCellIndex = currentCell.GetIndexInArray();
        x = baseCellIndex[0];
        z = baseCellIndex[1];
        Cell cell = cellMatrix[x, z].GetComponent<Cell>();

        return cell;
    }

    /*      1.    VisitCell       
     * 
     *  Uses current cells width & height to find all "1x1" cells that are part of it 
     *  Iterates through bool matrix and marks each of its cells as visited
     *  
     *      if cell is 2x2, it marks all 4 "1x1" cells as visited in martix
     *      but in real matrix of game objects, all 4 cells point to the base one (min x and z value)
     */
    private void VisitCell(Cell currentCell, int x, int z)
    {
        for (int i = 0; i < currentCell.xWidth; i++)
        {
            for (int j = 0; j < currentCell.zHeight; j++)
            {
                _cellMatrixBool[x + i, z + j] = true;
            }           
        }
    }

    /*      2.5     GenerateStartCell
     * 
     * Creates list containing 1 adjacent cell from each side of the starting one 
     * Removes out of bound cells
     * passes list to disable walls function
    */
    private void GenerateStartCell(GameObject[,] cellMatrix, Cell _currentCell, int x, int z, int cellXWidth, int cellZHeight)
    {
        List<int[]> startCells = new List<int[]>();

        // to left start
        int[] tempCell = new int[2];
        tempCell[0] = x - 1;
        tempCell[1] = z;
        startCells.Add(tempCell);
        //  below  start
        tempCell = new int[2];
        tempCell[0] = x;
        tempCell[1] = z - 1;
        startCells.Add(tempCell);
        // to right of start
        tempCell = new int[2];
        tempCell[0] = x + cellXWidth;
        tempCell[1] = z;
        startCells.Add(tempCell);
        // above start
        tempCell = new int[2];
        tempCell[0] = x;
        tempCell[1] = z + cellZHeight;
        startCells.Add(tempCell);

        startCells = RemoveOutOfBoundCells(startCells);

        for (int i = 0; i < startCells.Count; i++)
        {
            DisableWalls(_currentCell, cellMatrix[startCells[i][0], startCells[i][1]].GetComponent<Cell>(), startCells[i]);
        }
    }

    /*          3.  GetNextCell
     *          
     * finds all adjacent cells to the current one
     * 
     *      if cell is 1x2 at (0,0) - including the "1x1" cells [0,0] and [0,1]
     *      then it adds [1,0] [1,1] [0,2] [-1,0] [-1,1] [ 0,-1]
     *      to the list
    */
    private List<int[]> GetNextCells(Cell currentCell, int x, int z)
    {
        List<int[]> potentialCells = new List<int[]>();

        int[] nextCell;

        for (int i = 0; i < currentCell.xWidth; i++)
        {
            nextCell = new int[2];
            nextCell[0] = i + x;
            nextCell[1] = currentCell.zHeight + z;
            potentialCells.Add(nextCell);
            nextCell = new int[2];
            nextCell[0] = i + x;
            nextCell[1] = z - 1;
            potentialCells.Add(nextCell);
        }
        
        for (int i = 0; i < currentCell.zHeight; ++i)
        {
            nextCell = new int[2];
            nextCell[0] = x + currentCell.xWidth;
            nextCell[1] = z + i;
            potentialCells.Add(nextCell);
            nextCell = new int[2];
            nextCell[0] = x - 1;
            nextCell[1] = z + i;
            potentialCells.Add(nextCell);
        }

        return potentialCells;
    }

    /*          4.      RemoveOutOfBoundCells
     * returns a pruned list, after removing cells outside of the array 
     * basically if the x or z index is negative 
     * or if they are greater than the matrix width / height it removes em
    */
    private List<int[]> RemoveOutOfBoundCells(List<int[]> potentialCells)
    {
        for (int i = (potentialCells.Count - 1); i >= 0; i--)
        {
            if (potentialCells[i][0] < 0 || potentialCells[i][0] >= X_Width)
                potentialCells.RemoveAt(i);
            else if (potentialCells[i][1] < 0 || potentialCells[i][1] >= Z_Height)
                potentialCells.RemoveAt(i);
        }

        return potentialCells;
    }

    /*      5.      RemoveVisitedCells
     * checks new list of potential cells and makes sure they havent been visited
     * if they have been then remove from list
     * also applies if the potential cell is part of a larger one 
     *      (i.e. the 4x4 cell based at (0,0) has marked [0,0] [0,1] [1,0] [1,1]
     *      as visited, even if the cell [0,1] hasn't been explicitly visited)
     * then returnes pruned list, if no potential cells exist returns empty list
    */
    private List<int[]> RemoveVisitedCells(List<int[]> potentialCells)
    {
        List<int[]> remainingCells = new List<int[]>(potentialCells);

        if (remainingCells.Count <= 0)
            return new List<int[]>(); 

        for (int i = (remainingCells.Count - 1); i >= 0; i--)
        {
            if (_cellMatrixBool[remainingCells[i][0], remainingCells[i][1]])
                remainingCells.RemoveAt(i);
        }

        if (remainingCells.Count <= 0)
            { return new List<int[]>(); }

        return remainingCells;
    }

    /*      5.5     RemoveDeadCells
     * 
     * Checks if current cell has dead cells
     *      (dead cells are 1x1 cells in a larger one which can't be used as a
     *      destination or starting point when moving to next cell)
     *      (e.g. a 2x2 cell at (0,0) has a dead cell at [1,0] then the only 
     *      cells you can path from are [0,0] [0,1] [1,1])
     * 
     * Each deadcell has its own index in the array stored
     *      (i.e. in a 2x2 cell at (5,5) with a dead cell at [0,1] then the 
     *      dead cells index is stored as [5, 6])
     * compare each deadcell's index to the list of potential cells,
     * if the deadcell has the same x and z+-1 or the same z and x+-1 then the
     * potential cell cant be accessed because the adjacent cell is dead.
     * remove it from the list
    */
    private List<int[]> RemoveDeadCells(List<int[]> potentialCells, Cell currentCell)
    {
        if (!currentCell.HasDeadCell())
            return potentialCells;

        int[] currIndexList = currentCell.GetIndexInArray();

        List < (int x, int z) > deadCells = currentCell.GetDeadCellList();
        int[] currentDeadCell = new int[2];

        for (int j = 0; j < deadCells.Count; j++)
        {
            currentDeadCell[0] = deadCells[j].x;
            currentDeadCell[1] = deadCells[j].z;

            for (int i = (potentialCells.Count - 1); i >= 0; i--)
            {
                // potential cell to right / left (z +- 1)
                if (potentialCells[i][0] == currentDeadCell[0] && ((potentialCells[i][1] == (currentDeadCell[1] + 1)
                    || (potentialCells[i][1] == (currentDeadCell[1] - 1)))))
                {
                    potentialCells.RemoveAt(i);
                }
                // potential cell above / below (x +- 1)
                else if (potentialCells[i][1] == currentDeadCell[1] && ((potentialCells[i][0] == (currentDeadCell[0] + 1)
                    || (potentialCells[i][0] == (currentDeadCell[0] - 1)))))
                {
                    potentialCells.RemoveAt(i);
                }
            }
        }

        return potentialCells;
    }

    /*          6.      RandomizePotentialCells
     * Randomizes list, if you need a further explination then idk, good luck in life
     */
    private List<int[]> RandomizePotentialCells(List<int[]> potentialCells)
    {
        if (potentialCells.Count <= 0)
            return new List<int[]>(); 

        return potentialCells.OrderBy(x => Random.value).ToList();
    }

    /*     2.   DisableWalls(Cell currentCell, Cell nextCell)     
     * 
     * Get's both current & next cell's BaseCell index in matrix
     * Also gets lists of each "1x1" cell they occupy 
     *      (e.g. if its 1x2 size at (0,0), the list includes [0,0] and [0,1]
     * Then passes the current & next cell to functions which remove their walls
     * 
     *      NOTE: this is very inefficient, and the function currently does basically nothing
     *      in futur, only pass (current, next) & (next, current) to one function, respectively,
     *      by using their relative indexes to identify the most likely wall which they share
     *      (i.e. current's positiv X (right wall) and next's negative X (left wall))
     *      instead of just running that shit 4 times and hoping it works
    */
    public void DisableWalls(Cell currentCell, Cell nextCell, int[] wallToBeDisabled)
    {
        int[] currentCellIndex = currentCell.GetIndexInArray();
        List<int[]> currentCellIndexList = currentCell.GetIndexList();

        int[] nextCellIndex = nextCell.GetIndexInArray();
        List<int[]> nextCellIndexList = nextCell.GetIndexList();

        // make sure that current & next cell are within matrix bounds
        if (currentCellIndex[0] < 0 || nextCellIndex[0] < 0 || currentCellIndex[1] < 0 || nextCellIndex[1] < 0
            || currentCellIndex[0] >= X_Width || nextCellIndex[0] >= X_Width || 
            currentCellIndex[1] >= Z_Height || nextCellIndex[1] >= Z_Height)
            return;
        /*
        Debug.Log("Current Cell Index: " + currentCellIndex[0] + ", " + currentCellIndex[1]);
        
        for (int i = 0; i < currentCell._indexInArrayList.Count; i++)
        {
            Debug.Log("\t" + currentCell._indexInArrayList[i][0] + currentCell._indexInArrayList[i][1]);
        }

        Debug.Log("Next Cell Index: " + nextCellIndex[0] + ", " + nextCellIndex[1]);
        for (int i = 0; i < nextCell._indexInArrayList.Count; i++)
        {
            Debug.Log("\t" + nextCell._indexInArrayList[i][0] + nextCell._indexInArrayList[i][1]);
        }
        */
        //Debug.Log("(main)Walls to be disabled: " + wallToBeDisabled[0] + ", " + wallToBeDisabled[1]);
        DisableZHorizontalWall(currentCell, nextCell, wallToBeDisabled, false);
        DisableXVerticalWall(currentCell, nextCell, wallToBeDisabled, false);
    }

    /*      DisableZHorizontalWall(Cell current, Cell next, int[] wallToBeDisabled, bool found)
     * 
     * wallToBeDisabled = index of "1x1" cell in next( which is the same idex as the
     *      wall to be disabled in current)
     * found is used because the function calls itself to remove the same wall on opposite cell
     * current._zPosHorizontalWalls = a list of all the current cells "front" walls 
     *      (have a positive Z value relative to the center of the cell)
     *      (they basically store the cell on the other side of the wall)
     *      (e.g. if current cell is 1x2 at (0,0), the "front" walls are [1,0] and [1,1])
     * next._indexInArrayList = a list of all the "1x1" cells which next occupies 
     *      (e.g. if it is a 2x2 at (0,0), the list contains [0,0] [0,1] [1,0] [1,1])
     * 
     *      the function compares each element of the current cell's "front" walls to each element
     *      of the next cell's "1x1" cells, if they are equal then that wall must be taken down
     *      because the wall is seperating the current cell and the next cell.
     * 
     *      same goes for the second if statement but comparing the current cell's "bottom" walls and 
     *      the same list of the next cell's "sub cells" ("1x1" cells)
     * 
     * DisableXVerticalWall(Cell current, Cell next) does same shit but on x axis (left / right)
    */
    public void DisableZHorizontalWall(Cell current, Cell next, int[] wallToBeDisabled, bool found)
    {
        for (int j = 0; j < next._indexInArrayList.Count; j++)
        {
            if (CompareTwoArrays(wallToBeDisabled, next._indexInArrayList[j]))
            {
                for (int i = 0; i < current._zPosHorizontalWalls.Count; i++)
                {
                    if (CompareTwoArrays(current._zPosHorizontalWalls[i].GetMatrixID(), next._indexInArrayList[j]))
                    {
                        //Debug.Log("Disabling Walls from " + wallToBeDisabled[0] + ", " + wallToBeDisabled[1]);
                        
                        //current._zPosHorizontalWalls[i].DisableWall();
                        current.DisableSpecificPosZWall(i, found);

                        // removes opposite cell wall if first iteration
                        if (!found)
                        {
                            int[] newWallToBeDisabled = new int[2];
                            newWallToBeDisabled[0] = wallToBeDisabled[0];
                            newWallToBeDisabled[1] = wallToBeDisabled[1] - 1;

                            DisableZHorizontalWall(next, current, newWallToBeDisabled, true);
                        }
                        return;
                    }
                    if (CompareTwoArrays(current._zNegHorizontalWalls[i].GetMatrixID(), next._indexInArrayList[j]))
                    {
                        //Debug.Log("Disabling Walls from " + wallToBeDisabled[0] + ", " + wallToBeDisabled[1]);
                        
                        //current._zNegHorizontalWalls[i].DisableWall();
                        current.DisableSpecificNegZWall(i, found);

                        if (!found)
                        {
                            int[] newWallToBeDisabled = new int[2];
                            newWallToBeDisabled[0] = wallToBeDisabled[0];
                            newWallToBeDisabled[1] = wallToBeDisabled[1] + 1;

                            DisableZHorizontalWall(next, current, newWallToBeDisabled, true);
                        }
                        return;
                    }
                }
            }
        }
    }

    public void DisableXVerticalWall(Cell current, Cell next, int[] wallToBeDisabled, bool found)
    {
        for (int j = 0; j < next._indexInArrayList.Count; j++)
        {
            if (CompareTwoArrays(wallToBeDisabled, next._indexInArrayList[j]))
            {
                for (int i = 0; i < current._xPosVerticleWalls.Count; i++)
                {
                    if (CompareTwoArrays(current._xPosVerticleWalls[i].GetMatrixID(), next._indexInArrayList[j]))
                    {
                        //Debug.Log("Disabling Walls from " + wallToBeDisabled[0] + ", " + wallToBeDisabled[1]);
                        
                        //current._xPosVerticleWalls[i].DisableWall();
                        current.DisableSpecificPosXWall(i, found);

                        if (!found)
                        {
                            int[] newWallToBeDisabled = new int[2];
                            newWallToBeDisabled[0] = wallToBeDisabled[0] - 1;
                            newWallToBeDisabled[1] = wallToBeDisabled[1];

                            DisableXVerticalWall(next, current, newWallToBeDisabled, true);
                        }
                        return;
                    }
                    if (CompareTwoArrays(current._xNegVerticleWalls[i].GetMatrixID(), next._indexInArrayList[j]))
                    {
                        //Debug.Log("Disabling Walls from " + wallToBeDisabled[0] + ", " + wallToBeDisabled[1]);
                        
                        //current._xNegVerticleWalls[i].DisableWall();
                        current.DisableSpecificNegXWall(i, found);

                        if (!found)
                        {
                            int[] newWallToBeDisabled = new int[2];
                            newWallToBeDisabled[0] = wallToBeDisabled[0] + 1;
                            newWallToBeDisabled[1] = wallToBeDisabled[1];

                            DisableXVerticalWall(next, current, newWallToBeDisabled, true);
                        }
                        return;
                    }
                }
            }
        }

    }

    // See comment "6. RandomizePotentialCells"
    private bool CompareTwoArrays(int[] first, int[] second)
    {
        bool result = true;
        if (first[0] != second[0])
            { result = false; }
        if (first[1] != second[1]) 
            { result = false; }

        return result;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
