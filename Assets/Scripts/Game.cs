using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    PathGenerator pathGenerator;
    Track track;
    IDriver driver;
    CarController carController;
    CameraFollow cameraFollow;
    public GameObject carPrefab;
    public GameObject car;
    

    // Start is called before the first frame update
    void Awake()
    {
        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        track = GameObject.Find("GenRoad").GetComponent<Track>();

        Quaternion rot = pathGenerator.path.GetRotationAtDistance(0);
        Quaternion targetRot = rot * Quaternion.identity;
        car = Instantiate(carPrefab, new Vector3(0, 0, 0), targetRot);
        cameraFollow = GameObject.Find("Camera").GetComponent<CameraFollow>();
        cameraFollow.target = car.GetComponent<Transform>();

        carController = car.GetComponent<CarController>();
        // set driver always to Auto; change later
        driver = GameObject.Find("Autopilot").GetComponent<Autopilot>();
        driver.StartStopTimer(true);
    }

    // Update is called once per frame
    void Update()
    {
        // check if exclamation mark is passed --> change driver
    }
}
