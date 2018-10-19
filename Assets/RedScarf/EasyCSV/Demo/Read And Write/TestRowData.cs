using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedScarf.EasyCSV.Demo
{
    public class TestRowData
    {
        public string id;
        public string columnName1;
        public string columnName2;
        public string columnName3;
        public string columnName4;
        public int columnName5;

        public override string ToString()
        {
            var fields = GetType().GetFields();
            var str = "";
            foreach (var field in fields)
            {
                str += "    "+field.Name + ":" + field.GetValue(this)+"\r\n";
            }
            str = "【TestRowData】\r\n"
                +"{\r\n"
                + str 
                +"}";

            return str;
        }
    }
}