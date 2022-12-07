using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedDriver : Autopilot
{
    float endTime = 0;

    void Start()
    {
        InitObjects();
    }

    private void FixedUpdate()
    {
        if(timeStart && time <= endTime)
        {
            time += Time.fixedDeltaTime;  
        }

        HandleNavigation();
        HandleSpeed();
        HandleSteering();
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

        if((currentNavCheckPointIndex >= track.waypoints.Length - 1) || time >= (endTime + 10))
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

        float error = 0.4f;

        if(steeringAngle < 5)
        {
            error = 0.2f;
        } 
        else
        {
            if(steeringAngle < 15)
            {
                error = 0.8f;
            }
        }

        float actualSteering = steeringAngle * (1 - error * Mathf.Abs(Mathf.Sin(time)));

        carController.SetSteeringAngle(steeringAngle);
    }
}
