using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class WayPointCreator : MonoBehaviour
{

    public PathGenerator pathGenerator;
    public PathCreator pathCreator;
    private BezierPath bezierPath;
    private VertexPath path;
    public float distance;
    private float minDistance = 1.0f;

    public Vector3[] waypoints;

    //=== FOR DEBUGGING WAYPOINTS ===
    //[SerializeField] private GameObject marker;
    //private GameObject[] markerHolder;

    void Start()
    {
        Init();
        CreateWayPoints();
    }

    void Init()
    {
        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        pathCreator = pathGenerator.pathCreator;
        bezierPath = pathGenerator.bezierPath;
        path = pathGenerator.path;
        distance = path.length;
    }

    void CreateWayPoints()
    {
        int size = (int) (distance / minDistance) + 1;
        waypoints = new Vector3[size];
        waypoints[0] = path.localPoints[0];

        //=== FOR DEBUGGING WAYPOINTS ===
        //markerHolder = new GameObject[size];
        //waypoints[0].y = waypoints[0].y + 2;
        //Quaternion markerRot0 = Quaternion.LookRotation(waypoints[0], Vector3.up);
        //markerHolder[0] = (GameObject) Instantiate(marker, waypoints[0], markerRot0);

        for(int i = 1; i<size; i++)
        {
            float d = i*minDistance;
            waypoints[i] = path.GetPointAtDistance(d);

            //=== FOR DEBUGGING WAYPOINTS ===
            //Debug.Log(i + ": " + waypoints[i]);
            //waypoints[i].y = waypoints[i].y + 2;
            //Quaternion markerRot = Quaternion.LookRotation(waypoints[i], Vector3.up);
            //markerHolder[i] = (GameObject) Instantiate(marker, waypoints[i], markerRot);
        }
    }
}
