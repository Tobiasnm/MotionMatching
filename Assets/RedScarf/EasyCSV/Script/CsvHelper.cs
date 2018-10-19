using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RedScarf.EasyCSV
{
    /// <summary>
    /// csv帮助类
    /// </summary>
    public static class CsvHelper
    {
        static Dictionary<string, CsvTable> tableDict;

        static CsvHelper()
        {
            tableDict = new Dictionary<string, CsvTable>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="separator"></param>
        public static void Init(char separator = ',')
        {
            CsvTable.Init(separator);
        }

        /// <summary>
        /// 创建一个csv表格
        /// </summary>
        /// <param name="csvName"></param>
        /// <param name="data"></param>
        public static CsvTable Create(string csvName, string data = "", bool resolveColumnName = true, bool firstColumnIsID = true)
        {
            var table = new CsvTable(csvName, data, resolveColumnName, firstColumnIsID);
            tableDict.Remove(csvName);
            tableDict.Add(csvName, table);

            return table;
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="csvName"></param>
        /// <returns></returns>
        public static CsvTable Get(string csvName)
        {
            if (tableDict.ContainsKey(csvName))
            {
                return tableDict[csvName];
            }

            return null;
        }

        /// <summary>
        /// 使用id对应的行填充行数据
        /// 
        /// 【注】
        /// 1.csv表应第一列为id,且有列名
        /// 2.rowData中的字段名称应一一映射csv表中的列名
        /// 3.rowData中的字段类型实现 IConvertible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T PaddingData<T>(string csvName, string id) where T : new()
        {
            var rowData = new T();
            object temp = rowData;

            var table = Get(csvName);
            if (table == null) return rowData;

            var row = table.GetRowByID(id);

            return PaddingData<T>(csvName, row);
        }

        /// <summary>
        /// 使用id对应的行填充行数据
        /// 
        /// 【注】
        /// 1.csv表应第一列为id,且有列名
        /// 2.rowData中的字段名称应一一映射csv表中的列名
        /// 3.rowData中的字段类型实现 IConvertible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvName"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T PaddingData<T>(string csvName, int row) where T : new()
        {
            var rowData = new T();
            object temp = rowData;

            var table = Get(csvName);
            if (table == null) return rowData;

            if (row < 0 || row > table.RowCount) return rowData;

            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                var valueStr = table.Read(row, field.Name);
                if (string.IsNullOrEmpty(valueStr)) continue;

                try
                {
                    var value = Convert.ChangeType(valueStr, field.FieldType);
                    field.SetValue(temp, value);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Csv padding data error! {0}", e);
                }
            }
            rowData = (T)temp;

            return rowData;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public static void Clear()
        {
            tableDict.Clear();
        }
    }
}
