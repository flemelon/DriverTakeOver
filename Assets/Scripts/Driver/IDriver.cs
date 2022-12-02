using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDriver
{
    public void SetPathGenerator (PathGenerator pathGenerator);
    public void SetTrack (Track track);
    public void SetCarController (CarController carController);
}
