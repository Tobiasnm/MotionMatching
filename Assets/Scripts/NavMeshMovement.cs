using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour {

    GameObject cursor;
    NavMeshAgent navmesh;

	// Use this for initialization
	void Start () {
        cursor = GameObject.FindGameObjectWithTag("Cursor");
        navmesh = this.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        navmesh.destination = cursor.transform.position;
	}
}
