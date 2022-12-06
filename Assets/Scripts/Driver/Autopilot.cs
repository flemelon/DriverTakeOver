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
    public float throttle = 1.0f;
    public float minThrottle = -1.0f;
    public float maxThrottle = 1.0f;


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
        speed = carController.GetSpeed();
        float currentMaxSpeed = track.speedLimit[currentNavCheckPointIndex];

        if(currentNavCheckPointIndex + (int)speed < track.speedLimit.Length 
            && currentMaxSpeed > track.speedLimit[currentNavCheckPointIndex + (int)speed])
        {
            currentMaxSpeed = track.speedLimit[currentNavCheckPointIndex + (int)speed];
        }

        if ((speed >= (currentMaxSpeed * 1.1f)) )
        {
            throttle = throttle <= minThrottle ? minThrottle : throttle - 0.2f;
        }
        else if(speed <= (currentMaxSpeed * 0.9f))
        {
            throttle = throttle >= maxThrottle ? maxThrottle : throttle + 0.1f;
        } 
        else 
        {
            throttle = 0;
        }

        if(currentNavCheckPointIndex >= track.waypoints.Length - 1)
        {
            throttle = -1;
        }

        carController.SetThrottle(throttle);
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
        float speed =  Mathf.Lerp(track.minSpeed, track.maxSpeed, normalized);
        return speed;
    }
}