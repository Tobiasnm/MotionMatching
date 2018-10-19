using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedScarf.EasyCSV
{
    /// <summary>
    /// csv表格坐标（行，列）
    /// </summary>
    public struct CsvTableCoords
    {
        public int row;
        public int column;

        public CsvTableCoords(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public void Set(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }
}