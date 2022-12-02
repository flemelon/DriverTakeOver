using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DataText;
    [SerializeField] GameObject car;
    private CarController carController;
    private Autopilot autopilot;
    private Track track;

    private float pollingTime = 0.1f;
    private float time;
    private int frameCount;

    private string displayText;

    void Awake()
    {
        carController = GameObject.Find("Car").GetComponent<CarController>();
        autopilot = GameObject.Find("Autopilot").GetComponent<Autopilot>();
        track = GameObject.Find("GenRoad").GetComponent<Track>();
    }

    void Update () {
        time += Time.deltaTime;

        frameCount++;

        if(time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount/1f);
            displayText += frameRate.ToString() + " fps\n";
            //displayText += "autopilot: " + carController.autoPilot + "\n";
            //displayText += "horizontalInput: " + carController.horizontalInput + "\n";
            //displayText += "verticalInput: " + carController.verticalInput + "\n";
            displayText += "currentSteeringAngle: " + carController.steeringAngle + "\n";
            displayText += "currentBrakeForce: " + carController.currentBrakeForce + "\n";
            displayText += "current nav checkpoint index: " + autopilot.currentNavCheckPointIndex + "\n";
            displayText += "current speed checkpoint index: " + autopilot.currentSpeedCheckPointIndex + "\n";
            displayText += "car position: " + car.transform.position + "\n";
            displayText += "sdlp sum: " + track.sdlpSum + "\n";
            displayText += "number of measurements: " + track.n + "\n";
            displayText += "current sdlp: " + track.GetSdlp() + "\n";
            //displayText += "current maxSpeed: " + (int) (autopilot.maxSpeed[carController.currentNavCheckPointIndex]*3.6) + "km/h\n";
            displayText += "speed: " + (int)(autopilot.speed*3.6) + "km/h";
            

            DataText.text = displayText;

            displayText = "";
            time -= pollingTime;
            frameCount = 0;
        }
    }
}
