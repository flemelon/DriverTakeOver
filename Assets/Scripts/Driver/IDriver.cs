using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDriver
{
    float time { get; set; }
    float throttle { get; set; }
    float speed { get; set; }
    bool timeStart { get; set; }
    
    void StartStopTimer(bool startStop);
}
