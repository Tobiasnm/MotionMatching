using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class bvhreader : MonoBehaviour {

    StreamReader reader;
    string path = "Assets/BVH-files/walking.bvh"; //File to read
    public string[] text;
    public string[][] text2;

    public int numberOfFrames;
    string fileName = "walking";

    StreamWriter writer; 


    void Start () {
        text = new string[1];
        
        reader = new StreamReader(path);
        writer = new StreamWriter("Assets/Output/" + fileName + ".txt"); //File to write to

        for (int i = 0; i < 500; i++) 
        {
            text[0] = reader.ReadLine();
            if (text[0].Contains("Frames"))
            {
                numberOfFrames = int.Parse(text[0].Split(':')[1]);
            }
            if(text[0].Contains("Time:"))
            {
                text = new string[numberOfFrames];
                text2 = new string[numberOfFrames][];
                break;
            }
        }
        for (int i = 0; i < numberOfFrames; i++)
        {
            text[i] = reader.ReadLine();
            text2[i] = text[i].Split(null);
        }
        reader.Close();
        for (int j = 0; j < text2.Length - 1; j++)
        {
            for (int i = 0; i < text2[0].Length - 1; i++)
            {
                if (i > text2[j].Length - 3)
                {
                    writer.Write(text2[j][i]);
                }
                else
                    writer.Write(text2[j][i] + ",");
            }
            writer.WriteLine("");
        }
        writer.Close();
    }

	void Update () {
		
	}
}
