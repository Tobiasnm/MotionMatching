using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using RedScarf.EasyCSV;
using System.Collections.Generic;
using System;

namespace RedScarf.EasyCsvEditor
{
    /// <summary>
    /// csv工具窗口
    /// </summary>
    public class CsvToolsWindow : EditorWindow
    {
        const string RES_PATH_SUFFIX = "EasyCSV/Editor/EditorResources";
        const string CONFIG_FILE = "EditorConfig.txt";

        static string resPath = "";
        static string configFile = "";
        static string classTextHelpImage = "Csv2ClassImage.png";

        Texture2D classTextImage;
        Vector2 classTextScrollViewPos;
        Config m_Config;

        [MenuItem("Tools/EasyCsv/Csv Tools Window")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<CsvToolsWindow>();
            window.Show();
            window.titleContent = new GUIContent("Csv Tools");

            var paths = AssetDatabase.GetAllAssetPaths();
            foreach (var path in paths)
            {
                if (path.EndsWith(RES_PATH_SUFFIX))
                {
                    resPath = path;
                    break;
                }
            }
        }

        private void OnGUI()
        {
            if (m_Config == null)
            {
                m_Config = new Config();
                configFile = resPath + "/" + CONFIG_FILE;
                if (File.Exists(configFile))
                {
                    var contents = File.ReadAllText(configFile);
                    m_Config = JsonUtility.FromJson<Config>(contents);
                }
            }

            DrawCsv2ClassTool(new Rect(0,0,300,300));
        }

        /// <summary>
        /// 绘制Csv转换类工具
        /// </summary>
        /// <param name="rect"></param>
        void DrawCsv2ClassTool(Rect rect)
        {
            if (classTextImage == null)
            {
                classTextImage = AssetDatabase.LoadAssetAtPath<Texture2D>(resPath+"/"+classTextHelpImage);
            }

            using (new GUILayout.AreaScope(rect))
            {
                GUILayout.Label("Csv -> Class tool",EditorStyles.boldLabel);

                using (var scrollViewScope=new GUILayout.ScrollViewScope(classTextScrollViewPos,GUILayout.Height(classTextImage.height)))
                {
                    GUI.DrawTexture(new Rect(0,0, rect.width, classTextImage.height),classTextImage,ScaleMode.ScaleToFit);
                    classTextScrollViewPos = scrollViewScope.scrollPosition;
                }
                GUILayout.Label("The first line is the field name,\r\nThe second line is the field type");

                GUILayout.Space(20);

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Namespace", GUILayout.Width(100));
                    m_Config.namespaceStr = GUILayout.TextField(m_Config.namespaceStr);
                }

                using (new GUILayout.HorizontalScope())
                {
                    m_Config.isClass = GUILayout.Toggle(m_Config.isClass, new GUIContent("isClass?"));

                    if (GUILayout.Button("Csv -> Class"))
                    {
                        var classStrDict = new Dictionary<string, string>();

                        //打开csv文件目录
                        var openPath = Directory.Exists(m_Config.sourcePath) ? m_Config.sourcePath : Application.dataPath;
                        var sourceFolder = EditorUtility.OpenFolderPanel("Select csv folder", openPath, "");
                        if (string.IsNullOrEmpty(sourceFolder)) return;

                        m_Config.sourcePath = sourceFolder;

                        var files = Directory.GetFiles(sourceFolder, "*.csv");
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file);
                            var contents = File.ReadAllText(file);
                            var table = CsvHelper.Create(fileName, contents);

                            var classStr = GetClassStr(table, fileName);

                            classStrDict.Add(fileName, classStr);
                        }

                        if (!EditorUtility.DisplayDialog("Successful", "Read csv file successful,now select output folder!", "Ok", "Cancel")) return;

                        //输出文件夹
                        var savePath = Directory.Exists(m_Config.outputPath) ? m_Config.outputPath : Application.dataPath;
                        var outputFolderPath = EditorUtility.SaveFolderPanel("Select output folder", savePath, "");
                        if (string.IsNullOrEmpty(outputFolderPath)) return;

                        m_Config.outputPath = outputFolderPath;

                        foreach (var item in classStrDict)
                        {
                            var path = outputFolderPath + "/" + item.Key + ".cs";
                            File.WriteAllText(path, item.Value);
                        }

                        File.WriteAllText(configFile, JsonUtility.ToJson(m_Config));

                        AssetDatabase.Refresh();

                        EditorUtility.DisplayDialog("Successful", "Conver to class successful!", "Ok", "Cancel");
                    }
                }
            }
        }

        /// <summary>
        /// 获取类文本
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        string GetClassStr(CsvTable table,string csvName)
        {
            var classText = new ClassText(m_Config.namespaceStr,csvName,m_Config.isClass);
            var len=table.RawDataList[0].Count;
            for (var i=0;i<len;i++)
            {
                var fieldName = table.Read(0,i);
                var fieldType = table.Read(1,i);
                var fieldNode = new FieldNode(fieldName,fieldType);
                classText.Add(fieldNode);
            }

            return classText.GetClassText();
        }

        [Serializable]
        class Config
        {
            public string outputPath="";
            public string sourcePath="";
            public string namespaceStr="";
            public bool isClass;
        }
    }
}