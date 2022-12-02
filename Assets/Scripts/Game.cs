using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    PathGenerator pathGenerator;
    Track track;
    IDriver driver;
    CarController carController;

    // Start is called before the first frame update
    void Awake()
    {
        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        track = GameObject.Find("GenRoad").GetComponent<Track>();
        carController = GameObject.Find("Car").GetComponent<CarController>();
        // set driver always to Auto; change later
        driver = GameObject.Find("Autopilot").GetComponent<Autopilot>();
        driver.SetPathGenerator(pathGenerator);
        driver.SetTrack(track);
        driver.SetCarController(carController);
        // init car prefab
        // set autopilot as driver

    }

    // Update is called once per frame
    void Update()
    {
        // check if exclamation mark is passed --> change driver
    }
}
