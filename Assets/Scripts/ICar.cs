using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICar
{
    // continous value in degrees from -30 to 30
    void SetSteeringAngle (float steeringAngle);

    // continous value from -1 to 1 
    // (-1 is breaking, 0 is rolling, 1 is accelerating)
    void SetThrottle (float throttle);

    Vector3 GetPosition();

    float GetSpeed();
}
