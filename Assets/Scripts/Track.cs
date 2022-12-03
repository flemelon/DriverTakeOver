using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class Track : MonoBehaviour
{
    public PathGenerator pathGenerator;
    public PathCreator pathCreator;
    public BezierPath bezierPath;
    public VertexPath path;
    public float distance;
    private float minDistance = 1.0f;

    public Vector3[] waypoints;
    public float[] speedLimit;

    public float maxSpeed = 13;
    public float minSpeed = 8;

    private float minSdlpDistance = 0.0f;

    public float sdlpSum = 0;
    public int n = 0;

    [SerializeField] private GameObject checkpoint;
    private GameObject[] checkpointHolder;
    private bool debug = true;

    void Awake()
    {
        Init();
        CreateCheckPoints();
    }

    void Init()
    {
        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        pathCreator = pathGenerator.pathCreator;
        bezierPath = pathGenerator.bezierPath;
        path = pathGenerator.path;
        distance = path.length;
    }

    void CreateCheckPoints()
    {
        int size = (int) (distance / minDistance) + 1;
        waypoints = new Vector3[size];
        speedLimit = new float[size];

        waypoints[0] = path.localPoints[0];


        checkpointHolder = new GameObject[size];
        Quaternion checkpointRot0 =  path.GetRotationAtDistance(0);
        checkpointHolder[0] = (GameObject) Instantiate(checkpoint, waypoints[0], checkpointRot0);
        checkpointHolder[0].GetComponent<CheckpointSingle>().SetTrack(this);
        
        speedLimit[0] = minSpeed;
        for(int i = 1; i<size; i++)
        {
            float d = i*minDistance;
            waypoints[i] = path.GetPointAtDistance(d);

            Quaternion checkpointRot = path.GetRotationAtDistance(d);
            checkpointHolder[i] = (GameObject) Instantiate(checkpoint, waypoints[i], checkpointRot);
            checkpointHolder[i].GetComponent<CheckpointSingle>().SetTrack(this);

            Vector3 vec1 = path.GetNormalAtDistance(d-minDistance);
            Vector3 vec2 = path.GetNormalAtDistance(d);
            float radius = Utils.RadiusLength(waypoints[i-1], vec1, waypoints[i], vec2);
            float factor = 0.8f;

            if (radius > maxSpeed)
            {
                speedLimit[i] = maxSpeed;
            } 
            else 
            {
                if(radius < minSpeed)
                {
                    speedLimit[i] = minSpeed;
                }
                else
                {
                    speedLimit[i] = radius * factor;
                }
            }
        }

        float[] shiftedSpeedLimit = new float[size];
        int shiftingSize = 15;
        Array.Copy(speedLimit, shiftingSize, shiftedSpeedLimit, 0, speedLimit.Length - shiftingSize);
        for(int i = speedLimit.Length - 1; i >=shiftingSize; i--)
        {
            shiftedSpeedLimit[i] = minSpeed;
        }
    }

    public float GetCurrentDistance(Vector3 position)
    {
        return path.GetClosestDistanceAlongPath(position);
    }

    public void AddSdlpDistance(float sdlpDistance)
    {
        if(sdlpDistance >= minSdlpDistance)
        {
            sdlpSum+=(sdlpDistance);
        }
        n++;
    }

    public float GetSdlp(){
        return Mathf.Sqrt((sdlpSum * sdlpSum)/n);
    }
}
