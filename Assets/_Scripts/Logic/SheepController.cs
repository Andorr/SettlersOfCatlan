using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SheepController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0.4f;
    public float walkTime = 4f;
    public GameObject[] waypoints;
    public int waypointRadius = 1;
    private int randSpot;
    private Vector3 newPos;
    void Start()
    {
        randSpot = Random.Range(0,waypoints.Length);
        Debug.Log(randSpot);
        newPos = waypoints[randSpot].transform.position;
        Debug.Log(newPos);
    }

    // Update is called once per frame
    void Update()
    {

        float dist = Vector3.Distance(transform.position, newPos);

        if(dist < waypointRadius){
            randSpot = Random.Range(0,waypoints.Length);
            newPos = waypoints[randSpot].transform.position;
        }

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, newPos, step);

    }


}
