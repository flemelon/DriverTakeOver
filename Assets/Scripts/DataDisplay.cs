using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DataText;
    private GameObject car;
    private CarController carController;
    private IDriver driver;
    private Track track;
    private Game game;

    private float deltaTime = 0.0f;

    private string displayText;

    void Awake()
    {
        game = GameObject.Find("Parent").GetComponent<Game>();
        car = game.car;
        carController = game.car.GetComponent<CarController>();
        driver = GameObject.Find("Autopilot").GetComponent<Autopilot>();
        track = GameObject.Find("GenRoad").GetComponent<Track>();
    }

    void Update () {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        displayText += string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps) + "\n";
        displayText += "driving time: " + (int) driver.time + "s\n";
        displayText += "steering angle: " + carController.steeringAngle + "\n";
        displayText += "throttle: " + driver.throttle + "\n";
        displayText += "car position: " + car.transform.position + "\n";
        displayText += "sdlp sum: " + track.sdlpSum + "\n";
        displayText += "number of measurements: " + track.n + "\n";
        displayText += "current sdlp: " + track.GetSdlp() + "\n";
        displayText += "speed: " + (int)(driver.speed*3.6) + "km/h";
        

        DataText.text = displayText;

        displayText = "";
        
    }
}
