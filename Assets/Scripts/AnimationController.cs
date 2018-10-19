using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Globalization;
using UnityEngine.AI;


/* This class contains the implementation of the first iteration of our Motion Matching System.
 * Animations were recorded with a MoCap suit in .fbx format. 
 * In each situation it selects the correct frame using KNN algorithm. 
 * The two main methods of this class are 'ChooseNextFrame(...)' and 'ChooseAnimation(...)',
 * so please take a look at them. 
 * Less important methods are at the bottom of the class.  */
public class AnimationController : MonoBehaviour
{

   /* #region Attributes
    private Animator animator;  // Character animator component

    private int frames = 8926; //Amount of frames in the fbx
    private float frameNumber = 3.0f; //Which frame you want to play
    private int frameCount = 0; // How many frames have been played after last frame search. Used for loop protection

    private int animOffset = 1;

    private bool playingAnim = false;   // True if the system is automatically playing frames

    private GameObject player;
    private GameObject cursor;

    #region Attributes for new DB approach
    private float currentFrame;

    private float longTakeFrameCount = 8926; // TODO: Delete this and read it from file
    private string longTakeStateName = "walking";

    public FrameDB framedb;
    private float currSpeed, nextSpeed;
    private float currRot, nextRot;
    private float playerRotation, playerVel;
    private Vector3 targetRot;
    private float rotSum;
    private float velAvg;

    //private FrameDB framedb;
    #endregion

    #endregion


    void Start()
    {
        // Character animator setup
        animator = GetComponent<Animator>();
        animator.speed = 0f; // This allows the system to play animations frame by frame

        player = GameObject.FindGameObjectWithTag("Player");
        cursor = GameObject.FindGameObjectWithTag("Cursor");

    }

    void FixedUpdate()
    {

        if (!playingAnim)
        {
            Frame f = GetBestFrame();   // Here the best possible frame is picked and it's ready to be played
            currentFrame = f.GetIndex();    // We get the index of such frame within the long animation

            animator.Play(longTakeStateName, 0, currentFrame / longTakeFrameCount); // The selected frame is played

            playingAnim = true; // Sets up the flag for indicating that some frames need to be played before choosing again

        }
        else
        {
            currentFrame = (currentFrame + 1) % longTakeFrameCount; // The next frame is selected to be played
            if (currentFrame == 0) { currentFrame = 2; }    // Avoid T-Pose

            animator.Play(longTakeStateName, 0, currentFrame / longTakeFrameCount); // Frame is played


            frameCount++;   // Increment count to know when to stop playing frames
            if (frameCount >= 10)   // When 10 frames are played, it's time to choose a frame again
            {
                playingAnim = false;
                frameCount = 0;
            }
        }
    }

    #region Functions for new approach
    // Chooses the best frame based on the current frame
    private Frame GetBestFrame()
    {
        List<int> neigh = framedb.GetFrame((int)currentFrame).GetNeighbours();

        float bestCost = float.MaxValue;
        float currCost = -1;
        int bestFrame = -1;

        foreach (int n in neigh)
        {
            currCost = Cost(n);
            if (currCost < bestCost && n > currentFrame || n < currentFrame - 15)
            {
                bestCost = currCost;
                bestFrame = n;

            }
        }
        playerVel = player.GetComponent<NavMeshAgent>().velocity.magnitude;
        targetRot = cursor.transform.position - player.transform.position;
        playerRotation = Vector3.SignedAngle(player.transform.forward, targetRot, Vector3.up);
        print("vel: " + playerVel + "rot: " + playerRotation);
        UpdateParams(playerRotation, playerVel);
        return framedb.GetFrame(bestFrame);
    }

    // TODO: Define a cost function of jumping from current frame to frame 'nextFrame'
    private float Cost(int nextFrame)
    {
        Frame next = framedb.GetFrame(nextFrame);
        for (int i = 0; i < 10; i++)
        {
            rotSum += framedb.GetFrame(nextFrame + i).GetDirection();
            velAvg += framedb.GetFrame(nextFrame + i).GetSpeed();
        }
        velAvg /= 10;

        float deltaDirection = (nextRot - rotSum) / 180;
        float deltaSpeed = nextSpeed - velAvg / 5;

        float cost = deltaDirection + Mathf.Abs(deltaSpeed);
        rotSum = 0;
        velAvg = 0;
        return cost;
    }

    public void UpdateParams(float rot, float speed)
    {
        currSpeed = nextSpeed;
        nextSpeed = speed;

        currRot = nextRot;
        nextRot = rot;
    }
    
    #endregion*/
}