using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RedScarf.EasyCSV
{
    /// <summary>
    /// 单张csv表
    /// </summary>
    public sealed class CsvTable
    {
        const char ESCAPE_CHAR = '"';
        const char CARRIAGE_RETURN = '\r';
        const char LINE_BREAK = '\n';
        const string END_OF_LINE = "\r\n";
        const int DEFAULT_COLUMN_COUNT = 50;
        const int DEFAULT_ROW_COUNT = 999;
        static char s_Separator = ',';

        string m_Name;
        List<List<string>> m_RawDataList;
        StringBuilder stringBuilder;
        Dictionary<string, int> columnNameDict;
        Dictionary<string, int> rowIdDict;
        bool m_FirstColumnIsID;
        bool m_ResolveColumnName;

        internal static void Init(char separator)
        {
            s_Separator = separator;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param m_Name="name">表名</param>
        /// <param m_Name="data">数据</param>
        /// <param m_Name="resolveColumnName">是否解析列名</param>
        /// <param m_Name="firstColumnIsID">第一列是否为id列</param>
        public CsvTable(string name, string data,bool resolveColumnName,bool firstColumnIsID)
        {
            m_Name = name;
            m_FirstColumnIsID = firstColumnIsID;
            m_ResolveColumnName = resolveColumnName;
            m_RawDataList = new List<List<string>>(DEFAULT_ROW_COUNT);
            stringBuilder = new StringBuilder(DEFAULT_COLUMN_COUNT* DEFAULT_ROW_COUNT);
            columnNameDict = new Dictionary<string, int>(DEFAULT_COLUMN_COUNT);
            rowIdDict = new Dictionary<string, int>(DEFAULT_ROW_COUNT);

            Append(data);

            ResolveColumnName();
        }

        /// <summary>
        /// 解析行id
        /// </summary>
        /// <param name="row"></param>
        void ResolveRowID(int row)
        {
            if (row < 0||row>=m_RawDataList.Count) return;

            var idValue = Read(row, 0);
            if (!string.IsNullOrEmpty(idValue))
            {
                rowIdDict.Remove(idValue);
                rowIdDict.Add(idValue,row);
            }
        }

        /// <summary>
        /// 解析列名
        /// </summary>
        void ResolveColumnName()
        {
            if (!m_ResolveColumnName) return;

            var columnName = "";
            if (m_RawDataList.Count > 0)
            {
                var columnNameList = m_RawDataList[0];
                for (var i = 0; i < columnNameList.Count; i++)
                {
                    columnName = columnNameList[i];
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        string.Intern(columnName);
                        if (columnNameDict.ContainsKey(columnName))
                        {
                            columnNameDict.Remove(columnName);
                        }
                        columnNameDict.Add(columnName, i);
                    }
                }
            }
        }

        /// <summary>
        /// 由id获取行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetRowByID(string id)
        {
            if (m_FirstColumnIsID)
            {
                if (rowIdDict.ContainsKey(id))
                {
                    return rowIdDict[id];
                }
            }

            return -1;
        }

        /// <summary>
        /// 由列名获取列
        /// </summary>
        /// <param m_Name="columnName"></param>
        /// <returns></returns>
        int GetColumnByColumnName(string columnName)
        {
            if (!string.IsNullOrEmpty(columnName))
            {
                if (columnNameDict.ContainsKey(columnName))
                {
                    return columnNameDict[columnName];
                }
            }

            return -1;
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param m_Name="row"></param>
        /// <param m_Name="column"></param>
        /// <returns></returns>
        public string Read(int row, int column)
        {
            if (row < 0 || row >= m_RawDataList.Count || column < 0) return string.Empty;

            var rowData = m_RawDataList[row];
            if (column >= rowData.Count) return string.Empty;

            return rowData[column];
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param m_Name="row"></param>
        /// <param m_Name="columnName">列名</param>
        /// <returns></returns>
        public string Read(int row,string columnName)
        {
            if (!string.IsNullOrEmpty(columnName))
            {
                if (row >= 0 && row < m_RawDataList.Count)
                {
                    var column=GetColumnByColumnName(columnName);
                    if (column>=0)
                    {
                        if (m_RawDataList[row].Count > column)
                        {
                            return m_RawDataList[row][column];
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string Read(string id, string columnName)
        {
            return Read(GetRowByID(id),columnName);
        }

        public string Read(string id, int column)
        {
            return Read(GetRowByID(id), column);
        }

        /// <summary>
        /// 写入值
        /// </summary>
        /// <param m_Name="row"></param>
        /// <param m_Name="column"></param>
        /// <param m_Name="value"></param>
        public void Write(int row, int column, string value)
        {
            if (row < 0 || column < 0) return;

            if (!string.IsNullOrEmpty(value))
            {
                var newRowCount = row - m_RawDataList.Count + 1;
                if (newRowCount > 0)
                {
                    //插入新行
                    for (var i = 0; i < newRowCount; i++)
                    {
                        var newRowData = new List<string>(DEFAULT_COLUMN_COUNT);
                        m_RawDataList.Add(newRowData);
                    }
                }

                var currentRow = m_RawDataList[row];
                var newColumnCount = column - currentRow.Count + 1;
                if (newColumnCount > 0)
                {
                    //插入新列
                    for (var i = 0; i < newColumnCount; i++)
                    {
                        currentRow.Add(string.Empty);
                    }
                }
                currentRow[column] = value;
            }
            else if (row < m_RawDataList.Count)
            {
                if (column < m_RawDataList[row].Count)
                {
                    //覆盖值
                    m_RawDataList[row][column] = value;
                }
            }
            ResolveRowID(row);
        }

        /// <summary>
        /// 写入值
        /// </summary>
        /// <param m_Name="row"></param>
        /// <param m_Name="columnName"></param>
        /// <param m_Name="value"></param>
        public void Write(int row,string columnName,string value)
        {
            var column = GetColumnByColumnName(columnName);
            if (column >= 0)
            {
                Write(row,column,value);
            }
        }

        public void Write(string id, string columnName, string value)
        {
            Write(GetRowByID(id),columnName,value);
        }

        public void Write(string id, int column, string value)
        {
            Write(GetRowByID(id), column, value);
        }

        /// <summary>
        /// 追加数据到行尾
        /// </summary>
        /// <param m_Name="data"></param>
        public void Append(string data)
        {
            InsertData(m_RawDataList.Count,data);
        }

        /// <summary>
        /// 插入数据到指定行
        /// </summary>
        /// <param m_Name="row"></param>
        /// <param m_Name="data"></param>
        public void InsertData(int row,string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            row = Mathf.Clamp(row,0, m_RawDataList.Count);
            var list = GetDataList(data);
            if (list != null)
            {
                m_RawDataList.InsertRange(row, list);

                //记录id
                var end = row + list.Count;
                for (var i=row;i< end; i++)
                {
                    ResolveRowID(i);
                }
            }
        }

        /// <summary>
        /// 由csv转换为数组
        /// </summary>
        /// <param m_Name="data"></param>
        /// <returns></returns>
        List<List<string>> GetDataList(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            stringBuilder.Length = 0;

            var list = new List<List<string>>();
            var len = data.Length;
            var c = char.MinValue;
            var escapeCharCount = 0;
            var singleRowlist = new List<string>();
            for (var i = 0; i < len; i++)
            {
                c = data[i];
                if (c == ESCAPE_CHAR)
                {
                    escapeCharCount++;
                    if (escapeCharCount == 1 || escapeCharCount % 2 == 0)
                    {
                        //转义符
                        continue;
                    }
                }

                if (escapeCharCount % 2 == 0)
                {
                    if (c == s_Separator)
                    {
                        //列分隔符
                        singleRowlist.Add(stringBuilder.ToString());
                        stringBuilder.Length = 0;
                        escapeCharCount = 0;

                        continue;
                    }
                    else if (c == CARRIAGE_RETURN)
                    {
                        if (i + 1 < len)
                        {
                            if (data[i + 1] == LINE_BREAK)
                            {
                                //换行符
                                singleRowlist.Add(stringBuilder.ToString());
                                list.Add(singleRowlist);
                                singleRowlist = new List<string>();
                                stringBuilder.Length = 0;
                                escapeCharCount = 0;

                                i++;

                                continue;
                            }
                        }
                    }
                }

                stringBuilder.Append(c);
            }

            //最后一行如果没有换行符那么添加最后一行数据
            var lastRowValue = stringBuilder.ToString();
            if (!string.IsNullOrEmpty(lastRowValue))
            {
                singleRowlist.Add(lastRowValue);
                list.Add(singleRowlist);
            }
            stringBuilder.Length = 0;

            return list;
        }

        /// <summary>
        /// 移除值
        /// </summary>
        /// <param m_Name="row"></param>
        /// <param m_Name="column"></param>
        public void RemoveValue(int row, int column)
        {
            if (row < 0 || column < 0) return;
            if (row >= m_RawDataList.Count) return;

            var rowData = m_RawDataList[row];
            if (column >= rowData.Count) return;

            rowData.RemoveAt(column);
        }

        /// <summary>
        /// 移除值
        /// </summary>
        /// <param m_Name="row"></param>
        /// <param m_Name="columnName"></param>
        public void RemoveValue(int row, string columnName)
        {
            var column = GetColumnByColumnName(columnName);
            RemoveValue(row,column);
        }

        /// <summary>
        /// 移除指定行
        /// </summary>
        /// <param m_Name="row"></param>
        public void RemoveRow(int row)
        {
            if (row < 0 || row >= m_RawDataList.Count) return;

            m_RawDataList.RemoveAt(row);
        }

        /// <summary>
        /// 移除指定列
        /// </summary>
        /// <param m_Name="column"></param>
        public void RemoveColumn(int column)
        {
            if (column < 0) return;

            foreach (var row in m_RawDataList)
            {
                if (column < row.Count)
                {
                    row.RemoveAt(column);
                }
            }
        }

        /// <summary>
        /// 移除列
        /// </summary>
        /// <param m_Name="columnName">列名</param>
        public void RemoveColumn(string columnName)
        {
            var column = GetColumnByColumnName(columnName);
            if (column>=0)
            {
                RemoveColumn(column);
                columnNameDict.Remove(columnName);
            }
        }

        /// <summary>
        /// 查找值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public CsvTableCoords FindValue(string value, CsvTableCoords start)
        {
            var coords = new CsvTableCoords(-1,-1);
            var startRow = Mathf.Clamp(start.row,0,m_RawDataList.Count);

            var firstRow=m_RawDataList[startRow];
            for (var i=start.column;i<firstRow.Count;i++)
            {
                if (firstRow[i] == value)
                {
                    coords.Set(startRow, i);
                    return coords;
                }
            }

            startRow++;
            for (var i = startRow; i < m_RawDataList.Count; i++)
            {
                var rowData = m_RawDataList[i];
                for (var j = 0; j < rowData.Count; j++)
                {
                    if (rowData[j] == value)
                    {
                        coords.Set(i, j);
                        return coords;
                    }
                }
            }

            return coords;
        }

        public CsvTableCoords FindValue(string value)
        {
            return FindValue(value,new CsvTableCoords(0,0));
        }

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount
        {
            get
            {
                return m_RawDataList.Count;
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 原始数组数据
        /// </summary>
        public List<List<string>> RawDataList
        {
            get
            {
                return m_RawDataList;
            }
        }

        /// <summary>
        /// 获取csv数据
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            stringBuilder.Length = 0;

            var columnMax = 0;
            foreach (var row in m_RawDataList)
            {
                columnMax = Mathf.Max(columnMax, row.Count);
            }
            var rowLen = 0;
            var stringLen = 0;
            var hasSpecialCharacters = false;
            var stringValue = "";
            var charValue =char.MinValue;
            var columnStart = 0;
            foreach (var row in m_RawDataList)
            {
                rowLen = row.Count;
                for (var i = 0; i < columnMax; i++)
                {
                    hasSpecialCharacters = false;
                    columnStart = stringBuilder.Length;
                    if (i < rowLen)
                    {
                        stringValue = row[i];
                        stringLen = stringValue.Length;
                        for (var j=0;j<stringLen;j++)
                        {
                            charValue = stringValue[j];
                            if (charValue == s_Separator)
                            {
                                //分隔符
                                hasSpecialCharacters = true;
                            }
                            else if(charValue ==ESCAPE_CHAR)
                            {
                                //引号,需转义
                                stringBuilder.Append(ESCAPE_CHAR);
                                hasSpecialCharacters = true;
                            }
                            stringBuilder.Append(charValue);
                        }
                    }
                    if (hasSpecialCharacters)
                    {
                        //有特殊字符,需要用引号括起来
                        stringBuilder.Insert(columnStart, ESCAPE_CHAR);
                        stringBuilder.Append(ESCAPE_CHAR);
                    }
                    stringBuilder.Append(s_Separator);
                }
                //移除最后的分隔符及添加换行
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append(LINE_BREAK);
            }

            return stringBuilder.ToString();
        }
    }
}