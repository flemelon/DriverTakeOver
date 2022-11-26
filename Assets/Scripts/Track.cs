using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Track : MonoBehaviour
{
    public PathGenerator pathGenerator;
    public PathCreator pathCreator;
    private BezierPath bezierPath;
    private VertexPath path;
    public float distance;
    private float minDistance = 1.0f;

    public Vector3[] waypoints;
    public float[] maxSpeed;

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
        maxSpeed = new float[size];

        waypoints[0] = path.localPoints[0];


        checkpointHolder = new GameObject[size];
        Quaternion checkpointRot0 =  path.GetRotationAtDistance(0);
        checkpointHolder[0] = (GameObject) Instantiate(checkpoint, waypoints[0], checkpointRot0);
        checkpointHolder[0].GetComponent<CheckpointSingle>().SetTrack(this);
        
        maxSpeed[0] = 6;
        for(int i = 1; i<size; i++)
        {
            float d = i*minDistance;
            waypoints[i] = path.GetPointAtDistance(d);

            Quaternion checkpointRot = path.GetRotationAtDistance(d);
            checkpointHolder[i] = (GameObject) Instantiate(checkpoint, waypoints[i], checkpointRot);
            checkpointHolder[i].GetComponent<CheckpointSingle>().SetTrack(this);

            if(i>30)
            {
                float angle = Mathf.Abs(Vector3.SignedAngle(waypoints[i], waypoints[i] - waypoints[i-7], Vector3.up));
                //Debug.Log("Angle: "+ angle);
                if(angle > 30){
                    maxSpeed[i] = 6f;
                }else if(angle > 15){
                    maxSpeed[i] = 9f;
                }else {
                    maxSpeed[i] = 12f;
                }
                Debug.Log("MaxSpeed: " + maxSpeed[i]);
            }
            else 
            {
                maxSpeed[i] = 6f;
            }
        }
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
