using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace RedScarf.EasyCSV.Demo
{
    public class CsvTest : MonoBehaviour
    {
        public TextAsset text;
        CsvTable table;

        private void Start()
        {
            CsvHelper.Init();
            table = CsvHelper.Create(text.name, text.text);
            Debug.Log("Test: " + table.Read(2000, 5));
        }
    }
}