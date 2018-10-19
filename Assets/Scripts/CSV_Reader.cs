using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/* Class used for reading frame data from .CSV files */
public class CSV_Reader : MonoBehaviour {
/*
    private static string path = "Assets/CSV/"; // path where csv files are located
    //List<FrameInfo> frames = new List<FrameInfo>(); //list of all the frames in FrameInfo structure
    private static StreamReader reader; 
    private static string text;
    private static string[] data;
    private static string[] row;

   


    void Start () {
        
    }
	
	void Update () {
		
	}

    void Awake() {
        ReadLongTakeCSV();
    }

    public static List<Frame> ReadFile(string file) {
        //List<FrameInfo> frames = new List<FrameInfo>();
        List<Frame> frames = new List<Frame>();
        string full_path = path + file; 

        reader = new StreamReader(full_path);

        int index;
        float direction, velocity;
        List<int> neighs = new List<int>();
        int N = 15;

        for (int i = 0; i < TotalLines(full_path) - 1; i++)
        {
            //neighs = new List<int>();

            text = reader.ReadLine(); //1 line at a time 
            row = text.Split(new char[] { ',' }); //split the line at commas

            #region Previous version
            //FrameInfo f = new FrameInfo(); //initialize a FrameInfo


            //f.frame = int.Parse(row[0]);
            //f.walking = int.Parse(row[1]);
            //f.startWalk = int.Parse(row[2]);
            //f.turnLeft = int.Parse(row[3]);
            //f.turnRight = int.Parse(row[4]);
            //f.fastLeft = int.Parse(row[5]);
            //f.fastRight = int.Parse(row[6]);
            //f.stop = int.Parse(row[7]);
            #endregion
            index = int.Parse(row[0]);
            direction = float.Parse(row[1]);
            velocity = float.Parse(row[2]);

            Frame f = new Frame(index, direction, velocity);
            for (int j = 3; j < N + 3; j++) {
                //neighs.Add((int)float.Parse(row[j]));
                f.AddFrameToNeigh(int.Parse(row[j]));
            }

            //Frame f = new Frame(index, direction, velocity, neighs);
            frames.Add(f); //add f to the list frames
        
        }

        return frames;
    }

    private static int TotalLines(string filePath)
    {
        using (StreamReader r = new StreamReader(filePath))
        {
            int i = 0;
            while (r.ReadLine() != null) { i++; }
            return i;
        }
    }


    //[MenuItem("Assets/Create/My Scriptable Object")]
    public void ReadLongTakeCSV()
    {
        Debug.Log("Loading animation data...");
        //ReadFile(longTakeFileName);

        //Debug.Log("Data loaded succesfully, database has " + framedb.GetAllFrames().Count + " frames.");
    }*/
}
