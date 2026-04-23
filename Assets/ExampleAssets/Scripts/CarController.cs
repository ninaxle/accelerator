using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    [SerializeField] private float boostMultiplier = 4f;  // 4x faster when boosting
    private bool isBoosting;

    private bool isBreaking; //brake when space is pressed --will probs remove...

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float maxSpeed = 30f;

    public float MaxSpeed => maxSpeed;
    public float CurrentThrottle => verticalInput;
    public float CurrentSpeedRatio => Mathf.Clamp01(verticalInput);
    public bool IsBoosting => isBoosting;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering(); //rotate the wheels
        UpdateWheels();
    }


    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Mathf.Max(0, Input.GetAxis(VERTICAL));
        isBoosting = Input.GetKey(KeyCode.LeftShift); // or RightShift, or Space

        // isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        float currentMotorForce = isBoosting ? motorForce * boostMultiplier : motorForce;
        frontLeftWheelCollider.motorTorque = Mathf.Max(0, verticalInput) * currentMotorForce;
        frontRightWheelCollider.motorTorque = Mathf.Max(0, verticalInput) * currentMotorForce;

        // currentbreakForce = isBreaking ? breakForce : 0f; //if space is pressed, apply break force, else no break force
        // ApplyBreaking();       
    }

    // private void ApplyBreaking() //breaking applied to all wheels
    // {
    //     frontRightWheelCollider.brakeTorque = currentbreakForce;
    //     frontLeftWheelCollider.brakeTorque = currentbreakForce;
    //     rearLeftWheelCollider.brakeTorque = currentbreakForce;
    //     rearRightWheelCollider.brakeTorque = currentbreakForce;
    // }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot
; wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
