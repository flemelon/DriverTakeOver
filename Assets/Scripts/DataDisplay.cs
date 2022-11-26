using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DataText;
    [SerializeField] GameObject car;
    private CarController carController;

    private float pollingTime = 0.1f;
    private float time;
    private int frameCount;

    private string displayText;

    void Awake()
    {
        carController = GameObject.Find("Car").GetComponent<CarController>();
    }

    void Update () {
        time += Time.deltaTime;

        frameCount++;

        if(time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount/1f);
            displayText += frameRate.ToString() + " fps\n";
            displayText += "autopilot: " + carController.autoPilot + "\n";
            displayText += "horizontalInput: " + carController.horizontalInput + "\n";
            displayText += "verticalInput: " + carController.verticalInput + "\n";
            displayText += "currentSteeringAngle: " + carController.currentSteeringAngle + "\n";
            displayText += "currentBrakeForce: " + carController.currentBrakeForce + "\n";
            displayText += "current nav checkpoint index: " + carController.currentNavCheckPointIndex + "\n";
            displayText += "current speed checkpoint index: " + carController.currentSpeedCheckPointIndex + "\n";
            displayText += "car position: " + car.transform.position + "\n";
            displayText += "sdlp sum: " + carController.track.sdlpSum + "\n";
            displayText += "number of measurements: " + carController.track.n + "\n";
            displayText += "current sdlp: " + carController.track.GetSdlp() + "\n";
            displayText += "current maxSpeed: " + (int) (carController.track.maxSpeed[carController.currentNavCheckPointIndex]*3.6) + "km/h\n";
            displayText += "speed: " + (int)(carController.speed*3.6) + "km/h";

            DataText.text = displayText;

            displayText = "";
            time -= pollingTime;
            frameCount = 0;
        }
    }
}
