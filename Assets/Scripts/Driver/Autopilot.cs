using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Autopilot : MonoBehaviour, IDriver
{

    public PathGenerator pathGenerator;
    public Track track;
    public PathCreator pathCreator;
    private BezierPath bezierPath;
    private VertexPath path;

    [SerializeField] private GameObject speedArrow;
    private GameObject speedCheckPoint;
    private GameObject navCheckPoint;

    private CarController carController;

    public int currentNavCheckPointIndex = 0;
    public int currentSpeedCheckPointIndex = 10;
    public float maxSpeedCoefficiant = 0.6f;
    public float maxSteeringAngle = 30f;
    public float steeringAngle;

    public float speed;
    public float maxSpeed = 12f;
    public float minSpeed = 6f;
    public float foresightDistanceStep = 2;
    public List<Tuple<float, float, Vector3>> foresight;
    public float throttle = 0.5f;
    public float minThrottle = -0.5f;
    public float maxThrottle = 0.5f;


    void Start()
    {
        InitObjects();
    }

    private void FixedUpdate()
    {
        HandleNavigation();
        HandleSpeed();
        HandleSteering();
    }

    void InitObjects ()
    {

        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        track = GameObject.Find("GenRoad").GetComponent<Track>();
        carController = GameObject.Find("Car").GetComponent<CarController>();
        foresight = new List<Tuple<float, float, Vector3>>();

        Vector3 prevPoint = carController.GetPosition();
        float dst = track.path.GetClosestDistanceAlongPath(prevPoint);

        for(int i = 0; dst < (track.path.length + foresightDistanceStep) && i <= 15; i++)
        {
            float frstDst = dst+(i*foresightDistanceStep);
            Vector3 currentPoint = track.path.GetDirectionAtDistance(frstDst);

            float speedLimit = CalculateSpeed(currentPoint, currentPoint - prevPoint); 
            foresight.Add(Tuple.Create(frstDst, speedLimit, currentPoint));
            prevPoint = currentPoint;
            //Debug.Log(frstDst + ", " + speedLimit + ", " + currentPoint );
        }

        Quaternion speedCheckPointRot = Quaternion.LookRotation(track.waypoints[currentNavCheckPointIndex], Vector3.up);
        speedCheckPoint = (GameObject) Instantiate(speedArrow, track.waypoints[currentNavCheckPointIndex], speedCheckPointRot);
    }

    public void SetPathGenerator(PathGenerator pathGenerator)
    {
        this.pathGenerator = pathGenerator;
    }

    public void SetTrack(Track track)
    {
        this.track = track;
    }

    public void SetCarController(CarController carController)
    {
        this.carController = carController;
    }

    private void HandleNavigation()
    {
        if(currentNavCheckPointIndex < track.waypoints.Length && 
            Mathf.Abs(Vector3.Distance(track.waypoints[currentNavCheckPointIndex], carController.GetPosition())) <= 3f){
            currentNavCheckPointIndex = 
                currentNavCheckPointIndex < track.waypoints.Length - 1? currentNavCheckPointIndex + 1 : track.waypoints.Length - 1;
            if(speedCheckPoint != null)
            {
                speedCheckPoint.transform.position = track.waypoints[currentNavCheckPointIndex];
                speedCheckPoint.transform.rotation = Quaternion.LookRotation(track.waypoints[currentNavCheckPointIndex], Vector3.up);
            }
        }
    }

    private void HandleSpeed()
    {
        string output = "";
        foreach(Tuple<float, float, Vector3> f in foresight)
        {
            output += (int) (f.Item2 * 3.6f) + "km/h, ";
        }
        Debug.Log(output);
        speed = carController.GetSpeed();
        // TODO: needs revision
        float currentDistance = track.GetCurrentDistance(carController.GetPosition());
        
        if ((speed >= (foresight[0].Item2 * 1.1f)) ) //|| ((speed >= minSpeed) && (carController.steeringAngle > 15f)))
        {
            throttle = throttle <= minThrottle ? minThrottle : throttle - 0.2f;
        }
        else if(speed <= (foresight[0].Item2 * 0.9f))
        {
            throttle = throttle >= maxThrottle ? maxThrottle : throttle + 0.1f;
        } 
        else {
            throttle = 0;
        }

        carController.SetThrottle(throttle);

        
        Tuple<float, float, Vector3> last;
        if (foresight.Count > 0) 
        {
            last = foresight[foresight.Count-1];
            float nextDst = last.Item1 + foresightDistanceStep;

            if(currentDistance >= foresight[0].Item1 && nextDst <= track.path.length)
            {
                foresight.RemoveAt(0);

                Vector3 nextPoint = track.path.GetDirectionAtDistance(nextDst);
                float speedLimit = CalculateSpeed(nextPoint, nextPoint - last.Item3);
                foresight.Add(Tuple.Create(nextDst, speedLimit, nextPoint));
                if(speedLimit > foresight[foresight.Count - 1].Item2)
                {
                    for(int i = (foresight.Count - 2); i >= (foresight.Count - 5); i--)
                    {
                        float newSpeedLimit = foresight[i].Item2 > minSpeed + 2f? foresight[i].Item2 - 2f : minSpeed;  
                    }
                }
            }
        }
    }

    private void HandleSteering()
    {
        var relativePos = track.waypoints[currentNavCheckPointIndex] - carController.GetPosition();
        var targetRotation = Quaternion.LookRotation(relativePos);
        float y = carController.GetTransform().eulerAngles.y;
        var DeltaAngle = Mathf.DeltaAngle(y, targetRotation.eulerAngles.y);
        float delta = Mathf.Clamp(DeltaAngle, -1, 1);
        steeringAngle = maxSteeringAngle * delta;

        carController.SetSteeringAngle(steeringAngle);
    }

    private float CalculateSpeed (Vector3 p1, Vector3 p2)
    {
        var relativePos = p2 - p1;
        var targetRotation = Quaternion.LookRotation(relativePos);
        float y = carController.GetTransform().eulerAngles.y;
        var delta = Mathf.DeltaAngle(y, targetRotation.eulerAngles.y);
        float normalized = Mathf.Abs(Mathf.Cos(delta%90));
        //Debug.Log("NORMALIZED: " + normalized);
        float speed =  Mathf.Lerp(minSpeed, maxSpeed, normalized);
        return speed;
    }
}