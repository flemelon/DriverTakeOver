using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DataText;
    [SerializeField] GameObject car;
    [SerializeField] GameObject leader;
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
            displayText += "distance: " + carController.distance + "\n";
            displayText += "steeringCoefficient: " + carController.steeringCoefficient + "\n";
            displayText += "distanceCoefficient: " + carController.distanceCoefficient + "\n";
            displayText += "isBraking: " + carController.isBraking + "\n";
            displayText += "leader position: " + leader.transform.position + "\n";
            displayText += "car position: " + car.transform.position;

            DataText.text = displayText;

            displayText = "";
            time -= pollingTime;
            frameCount = 0;
        }
    }
}