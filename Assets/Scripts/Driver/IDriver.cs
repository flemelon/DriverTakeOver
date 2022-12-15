using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDriver
{
    float time { get; set; }
    float throttle { get; set; }
    float speed { get; set; }
    bool isTimerRunning { get; set; }
    int currentNavCheckPointIndex { get; set; }
    int currentSpeedCheckPointIndex { get; set; }
    
    void StartStopTimer(bool startStop);
    float GetCurrentTime();
    DriverType GetDriverType();
    void SetCarController(CarController carController);
    void Enable();
    void Disable();
}
