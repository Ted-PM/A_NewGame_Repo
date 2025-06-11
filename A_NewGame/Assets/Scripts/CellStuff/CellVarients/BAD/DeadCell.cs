//using System.Collections.Generic;
//using System.Collections;
//using UnityEngine;
//public class DeadCell : CellBaseClass
//{
//    [Tooltip("Add the dead cells as XXZZ, no spaces, must have 4 characters")]
//    public List<string> _deadCellListStr;
//    private List<int[]> _deadCellListInt;

//    // OVERRIDE
//    protected override void InitializeLists()
//    {
//        _deadCellListInt = new List<int[]>();
//        base.InitializeLists();
//    }
//    public override void SetCellBaseIndex(int x, int z)
//    {
//        base.SetCellBaseIndex(x, z);
//        SetDeadCells();
//    }

//    // SET DEAD CELL DATA
//    private void SetDeadCells()
//    {
//        if (_deadCellListStr == null || _deadCellListStr.Count <= 0)
//            return;

//        int[] tempIndex = null;

//        for (int i = 0; i < _deadCellListStr.Count; i++)
//        {
//            if (_deadCellListStr[i].Length == 4)
//            {
//                tempIndex = new int[2];
//                tempIndex[0] = _cellBaseIndex[0] + GetIntFrom2Char(_deadCellListStr[i][0], _deadCellListStr[i][1]);
//                tempIndex[1] = _cellBaseIndex[1] + GetIntFrom2Char(_deadCellListStr[i][2], _deadCellListStr[i][3]);
//                _deadCellListInt.Add(tempIndex);
//            }
//            else
//                Debug.LogError("Dead Cell Name Wrong!! Name: " + _deadCellListStr[i]);
//        }
//    }

//    // GET DEAD CELL DATA
//    public int GetNumDeadCells()
//    {
//        return _deadCellListInt.Count;
//    }

//    public int GetDeadCellX(int num)
//    {
//        if (num >= _deadCellListInt.Count || num < 0)
//            return -1;
//        return _deadCellListInt[num][0];
//    }

//    public int GetDeadCellZ(int num)
//    {
//        if (num >= _deadCellListInt.Count || num < 0)
//            return -1;
//        return _deadCellListInt[num][1];
//    }

//    public List<int[]> GetDeadCellList()
//    {
//        if (_deadCellListInt == null || _deadCellListInt.Count == 0)
//            return new List<int[]>();
//        return _deadCellListInt;
//    }

//    //      UTILITY
//    private int GetIntFrom2Char(char a, char b)
//    {
//        int result = 0;
//        result += GetIntFromChar(a) * 10;
//        result += GetIntFromChar(b);
//        return result;

//    }

//    private int GetIntFromChar(char ch)
//    {
//        return ((int)ch - 48);
//    }
//}
