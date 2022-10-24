using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController : MonoBehaviour
{

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public bool autoPilot; //{get {return autoPilot;} set{ autoPilot = true; }}

    public float horizontalInput; //{get;}
    public float verticalInput; //{get;}
    public float currentSteeringAngle; //{get;}
    public float steeringAngle; //{get;}
    public float currentBrakeForce; //{get;}
    public float distance; //{get;}
    public float steeringCoefficient; //{get;}
    public float distanceCoefficient; //{get;}
    public bool isBraking; //{get;}
    
    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteeringAngle;

    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private Transform leader;

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
    }

    private void FixedUpdate()
    {
       // transform.position = instance.transform.position;
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
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
        distance = Vector3.Distance(leader.transform.position, car.transform.position);

        steeringCoefficient = (currentSteeringAngle + maxSteeringAngle) / ((maxSteeringAngle + 10)*2);
        distanceCoefficient = ((distance - minDistance) /(maxDistance - minDistance));

        float adjSteeringCoefficient = Mathf.Max(Mathf.Abs((currentSteeringAngle + maxSteeringAngle) / ((maxSteeringAngle + 10)*2)), 0.3f);
        float adjDistanceCoefficient = Mathf.Min(Mathf.Abs(((distance - minDistance) /(maxDistance - minDistance))),0.5f);

        if( distance <= minDistance )
        {
            currentBrakeForce = 3000;
            adjDistanceCoefficient = 0;
            adjSteeringCoefficient = 0;
        } else if (distance >= maxDistance )
        {
            currentBrakeForce = 0;
        }

        frontLeftWheelCollider.motorTorque = adjSteeringCoefficient * adjDistanceCoefficient * motorForce;
        frontRightWheelCollider.motorTorque = adjSteeringCoefficient * adjDistanceCoefficient * motorForce;

        string brakeLog = (distance <= minDistance) ? "\nbreak!" : "";

        /*
        Debug.Log(
            "steeringAngle: " + currentSteeringAngle + 
            "\nsteerCoefficient: " + steeringCoefficient + 
            "\nadjSteerCoefficient: " + adjSteeringCoefficient + 
            "\ndisCoefficient: "+ distanceCoefficient +
            "\nadjDisCoefficient: "+ adjDistanceCoefficient + 
            "\ndistance: "+ distance + 
            "\ncurrentBrakeForce: "+ currentBrakeForce +
            brakeLog
        );
        */
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
            var relativePos = leader.position - transform.position;
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
        wheelTransform.rotation = rot; //Quaternion.Inverse(rot);
        wheelTransform.position = pos;
    }
}
