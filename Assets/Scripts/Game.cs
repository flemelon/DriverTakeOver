using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public PathGenerator pathGenerator;
    public Track track;
    public IDriver currentDriver;
    public IDriver simulatedDriver;
    public CarController carController;
    CameraFollow cameraFollow;
    public GameObject carPrefab;
    public GameObject car;

    public float torTime;
    

    // Start is called before the first frame update
    void Awake()
    {
        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        track = GameObject.Find("GenRoad").GetComponent<Track>();

        Quaternion rot = pathGenerator.path.GetRotationAtDistance(0);
        Quaternion targetRot = Quaternion.identity * new Quaternion(0, rot.y, 0, rot.w);
        Vector3 position = pathGenerator.path.GetPointAtDistance(0);
        car = Instantiate(carPrefab, position, targetRot);
        cameraFollow = GameObject.Find("Camera").GetComponent<CameraFollow>();
        cameraFollow.target = car.GetComponent<Transform>();

        carController = car.GetComponent<CarController>();

        //currentDriver = GameObject.Find("Autopilot").GetComponent<Autopilot>();
        simulatedDriver = GameObject.Find("Simulated").GetComponent<SimulatedDriver>();
        currentDriver = simulatedDriver;
        currentDriver.StartStopTimer(true);

        torTime = pathGenerator.exclamationMarkTime - 0.05f;

        if(torTime < 0f)
        {
            torTime = 0.05f;
        }
        else
        {
            if(torTime > 1f){
                torTime = 0.95f;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(currentDriver.GetCurrentTime() >= torTime && currentDriver.GetDriverType() == DriverType.Autopilot)
        { 
            currentDriver.SetCarController(null);
            currentDriver = simulatedDriver;
            currentDriver.StartStopTimer(true);
        }
    }
}
