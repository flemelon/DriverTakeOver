using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class CarController : MonoBehaviour, ICar
{
    public float steeringAngle; 
    public float currentBrakeForce; 
    public float maxBreakForce = 3000;

    [SerializeField] private float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteeringAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private Transform car;

    private void FixedUpdate()
    {
        UpdateWheels();
    }

    public void SetSteeringAngle(float steeringAngle)
    {
        this.steeringAngle = steeringAngle;
        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;
    }

    public void SetThrottle (float throttle)
    {
        if(throttle >= -1 && throttle < 0)
        {
            ApplyBrakeTorque(Mathf.Abs(throttle));
            ApplyMotorTorque(0);
        }
        else if(throttle == 0)
        {
            ApplyBrakeTorque(0);
            ApplyMotorTorque(0);
        }
        else if(throttle > 0 && throttle <= 1)
        {
            ApplyBrakeTorque(0);
            ApplyMotorTorque(throttle);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
        //Vector3.LerpUnclamped(rearLeftWheelTransform.position, rearRightWheelTransform.position, 0.5f);
    }

    public float GetSpeed()
    {
        return transform.GetComponent<Rigidbody>().velocity.magnitude;
    }

    public Vector3 GetForward()
    {
        return transform.forward;
    } 

    public Transform GetTransform()
    {
        return transform;
    }

    private void ApplyMotorTorque(float accelerationCoefficient)
    {
        frontLeftWheelCollider.motorTorque = accelerationCoefficient * motorForce;
        frontRightWheelCollider.motorTorque = accelerationCoefficient * motorForce;
    }

    private void ApplyBrakeTorque(float brakeCoefficient)
    {
        frontRightWheelCollider.brakeTorque = brakeCoefficient * maxBreakForce;
        frontLeftWheelCollider.brakeTorque = brakeCoefficient * maxBreakForce;
        rearRightWheelCollider.brakeTorque = brakeCoefficient * maxBreakForce;
        rearLeftWheelCollider.brakeTorque = brakeCoefficient * maxBreakForce;
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
