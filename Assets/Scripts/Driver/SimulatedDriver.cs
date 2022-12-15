using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedDriver : Autopilot
{
    public float timeRemaining = 10;
    float error = 0.4f;

    public override void Start()
    {
        InitObjects();
    }

    public override void StartStopTimer(bool startTime)
    {
        this.time = Time.time;
        isTimerRunning = startTime;

        Debug.Log(timeRemaining + ", " + time + ", " + isTimerRunning);
    }

    public override void FixedUpdate()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                time += Time.fixedDeltaTime; 
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                isTimerRunning = false;
            }
        }

        HandleNavigation();
        HandleSpeed();
        HandleSteering();
    }

    public override void HandleSpeed()
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

        float actualThrottle = (throttle == 0) ? 0 : throttle - error * (Mathf.Sin((int)time) - 1);

        if( timeRemaining <= 0 || (currentNavCheckPointIndex >= track.waypoints.Length - 1) )
        {
            throttle = -1;
        }

        carController.SetThrottle(throttle);
    }

    public override void HandleSteering()
    {
        var relativePos = track.waypoints[currentNavCheckPointIndex] - carController.GetPosition();
        var targetRotation = Quaternion.LookRotation(relativePos);
        float y = carController.GetTransform().eulerAngles.y;
        var DeltaAngle = Mathf.DeltaAngle(y, targetRotation.eulerAngles.y);
        float delta = Mathf.Clamp(DeltaAngle, -1, 1);
        steeringAngle = maxSteeringAngle * delta;



        if(steeringAngle < 5)
        {
            error = 0.2f;
        } 
        else
        {
            if(steeringAngle < 10)
            {
                error = 0.8f;
            }
            else
            {
                error = 0.4f;
            }
        }

        float actualSteering = steeringAngle * (1 - error * Mathf.Abs(Mathf.Sin((int)time)));

        carController.SetSteeringAngle(steeringAngle);
    }

    public override DriverType GetDriverType()
    {
        return DriverType.Simulated_Driver;
    }
}
