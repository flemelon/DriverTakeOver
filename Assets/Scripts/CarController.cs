using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class CarController : MonoBehaviour
{
    public PathGenerator pathGenerator;
    public PathCreator pathCreator;
    private BezierPath bezierPath;
    private VertexPath path;

    [SerializeField] private GameObject speedArrow;
    private GameObject speedCheckPoint;
    private GameObject navCheckPoint;

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public bool autoPilot;

    public float horizontalInput; 
    public float verticalInput; 
    public float currentSteeringAngle; 
    public float steeringAngle; 
    public float currentBrakeForce; 
    public float distance; 
    public float steeringCoefficient; 
    public float distanceCoefficient; 
    public bool isBraking; 
    public float maxSpeedCoefficiant = 0.7f;
    public float maxBreakForce = 1000;
    public float speed;

    public int currentNavCheckPointIndex = 0;
    public int currentSpeedCheckPointIndex = 10;
    
    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteeringAngle;

    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private Transform car;

    void Start()
    {
        autoPilot = true;
        InitObjects();
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleCheckPoint();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    void InitObjects ()
    {

        pathGenerator = GameObject.Find("GenRoad").GetComponent<PathGenerator>();
        pathCreator = pathGenerator.pathCreator;
        bezierPath = pathGenerator.bezierPath;
        path = pathGenerator.path;
        Quaternion speedCheckPointRot = Quaternion.LookRotation(path.localPoints[currentSpeedCheckPointIndex], Vector3.up);
        speedCheckPoint = (GameObject) Instantiate(speedArrow, path.localPoints[currentSpeedCheckPointIndex], speedCheckPointRot);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleCheckPoint()
    {
        if(currentNavCheckPointIndex < path.localPoints.Length && 
            Mathf.Abs(Vector3.Distance(path.localPoints[currentNavCheckPointIndex], car.transform.position)) <= 3f){
            currentNavCheckPointIndex += 1;
            currentSpeedCheckPointIndex += 1;
            if(speedCheckPoint != null)
            {
                speedCheckPoint.transform.position = path.localPoints[currentSpeedCheckPointIndex];
                speedCheckPoint.transform.rotation = Quaternion.LookRotation(path.localPoints[currentSpeedCheckPointIndex], Vector3.up);
            }
        }
        speed = transform.GetComponent<Rigidbody>().velocity.magnitude;
    }

    private void HandleMotor()
    {
        currentBrakeForce = 0;
        if(autoPilot == true)
        {
            HandleSpeed();
        }
        else
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;

            currentBrakeForce = isBraking ? brakeForce : 0f;
        }
        ApplyBraking();
    }

    private void HandleSpeed()
    {
        float currentSteeringAngle = Vector3.SignedAngle(path.localPoints[currentSpeedCheckPointIndex], transform.forward, Vector3.up);
        float speedCoefficient = Mathf.Max(Mathf.Abs((currentSteeringAngle + maxSteeringAngle) / ((maxSteeringAngle + 10)*2)), maxSpeedCoefficiant);

        if(speed >= 6 && Mathf.Abs(currentSteeringAngle) >=15)
        {
            currentBrakeForce = 3000;
        } else {
            currentBrakeForce = 0;
        }
        frontLeftWheelCollider.motorTorque = speedCoefficient * motorForce;
        frontRightWheelCollider.motorTorque = speedCoefficient * motorForce;
    }

    private void ApplyBraking()
    {
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        if(autoPilot == false)
        {
            currentSteeringAngle = maxSteeringAngle * horizontalInput;
        }
        else
        {
            var relativePos = path.localPoints[currentNavCheckPointIndex] - transform.position;
            var targetRotation = Quaternion.LookRotation(relativePos);
            float y = transform.eulerAngles.y;
            var DeltaAngle = Mathf.DeltaAngle(y, targetRotation.eulerAngles.y);
            float delta = Mathf.Clamp(DeltaAngle, -1, 1);
            currentSteeringAngle = maxSteeringAngle * delta;
        }

        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel (WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
