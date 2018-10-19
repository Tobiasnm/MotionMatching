using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Globalization;
using UnityEngine.AI;
using RedScarf.EasyCSV;


/* This class contains the implementation of the first iteration of our Motion Matching System.
 * Animations were recorded with a MoCap suit in .fbx format. 
 * In each situation it selects the correct frame using KNN algorithm. 
 * The two main methods of this class are 'ChooseNextFrame(...)' and 'ChooseAnimation(...)',
 * so please take a look at them. 
 * Less important methods are at the bottom of the class.  */
public class AnimationController2 : MonoBehaviour
{
    private Animator animator;  // Character animator component

    private int frameCount = 0; // How many frames have been played after last frame search. Used for loop protection
    private bool playingAnim = false;   // True if the system is automatically playing frames

    private GameObject player;
    private GameObject cursor;

    private float currentFrame;

    private float totalFrames = 8926; // TODO: Delete this and read it from file
    private string animationName = "walking";

    private float nextSpeed;
    private float nextRot;
    private float playerRotation, playerVel;
    private Vector3 targetRot;
    private float rotSum;
    private float velAvg;

    public TextAsset text;
    CsvTable table;

    int[] neigh;
    bool inMemory;

    //Debug stuff
    private float currFrameVel;
    private float cost;
    private float[] costs;
    int index = 0;
    private int[] memory;

    void Start()
    {
        // Character animator setup
        animator = GetComponent<Animator>();
        animator.speed = 0f; // This allows the system to play animations frame by frame

        player = GameObject.FindGameObjectWithTag("Player");
        cursor = GameObject.FindGameObjectWithTag("Cursor");

        CsvHelper.Init();
        table = CsvHelper.Create(text.name, text.text);

        neigh = new int[50];
        memory = new int[100];

        costs = new float[50];
    }

    void FixedUpdate()
    {
        currFrameVel = float.Parse(table.Read((int)currentFrame, 2));

        if (!playingAnim)
        {
            currentFrame = GetBestFrame();   // Here the best possible frame is picked and it's ready to be played
            animator.Play(animationName, 0, currentFrame / totalFrames); // The selected frame is played
            animator.Play("walking1", 0, currentFrame / totalFrames);
            animator.SetBool("Go", true);
            //animator.SetBool("Go", false);
            playingAnim = true; // Sets up the flag for indicating that some frames need to be played before choosing again
        }

        else
        {
            currentFrame = (currentFrame + 1) % totalFrames; // The next frame is selected to be played
            if (currentFrame == 0) { currentFrame = 2; }    // Avoid T-Pose

            animator.Play(animationName, 0, currentFrame / totalFrames); // Frame is played
            animator.Play("walking1", 0, currentFrame / totalFrames);


            frameCount++;   // Increment count to know when to stop playing frames
            
            if (frameCount >= 5)   // When 10 frames are played, it's time to choose a frame again
            {
                playingAnim = false;
                frameCount = 0;
            }
        }
    }

    // Chooses the best frame based on the current frame
    private int GetBestFrame()
    {
        for (int i = 0; i < 50; i++)
        {
            neigh[i] = int.Parse(table.Read((int)currentFrame, i+3));
        }
        float bestCost = float.MaxValue;
        float currCost = -1;
        int bestFrame = -1;

        foreach (int n in neigh)
        {
            currCost = Cost(n);
            //costs[index] = currCost;
            //index++;
            for (int i = 0; i < memory.Length; i++)
            {
                if (n == memory[i])
                    inMemory = true;
            }

            if (currCost < bestCost && !inMemory)
            {
                bestCost = currCost;
                bestFrame = n;
            }
            inMemory = false;
        }

        for (int i = 0; i < memory.Length - 1; i++)
        {
            memory[i] = memory[i + 1];
        }
        memory[memory.Length - 1] = bestFrame;

        index = 0;
        playerVel = player.GetComponent<NavMeshAgent>().velocity.magnitude;
        targetRot = cursor.transform.position - player.transform.position;
        playerRotation = Vector3.SignedAngle(player.transform.forward, targetRot, Vector3.up);
        UpdateParams(playerRotation, playerVel);
        return int.Parse(table.Read(bestFrame, 0));
    }

    // TODO: Define a cost function of jumping from current frame to frame 'nextFrame'
    private float Cost(int nextFrame)
    {
        rotSum = 0;
        velAvg = 0;
        //Frame next = framedb.GetFrame(nextFrame);
        for (int i = 0; i < 30; i++)
        {
            rotSum += float.Parse(table.Read(nextFrame + i, 1));
            velAvg += float.Parse(table.Read(nextFrame + i, 2));
        }
        velAvg /= 30;

        //float deltaDirection = (nextRot - rotSum) / 180;
        float deltaDirection = 0;
        float deltaSpeed = nextSpeed - velAvg / 5;

        cost = deltaDirection + Mathf.Abs(deltaSpeed);
        return cost;
    }

    public void UpdateParams(float rot, float speed)
    {
        nextSpeed = speed;
        nextRot = rot;
    }
}