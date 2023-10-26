using Arcade_Racer.Scripts.Cars;

using NaughtyAttributes;

using System;
using System.Collections.Generic;

using UnityEditor.ShaderGraph.Drawing;

using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectsObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    [Header("Controls")] 
    public KeyCode handBrake = KeyCode.Space;
    
    [Header("Drivetrain")]
    [DisableIf(EConditionOperator.Or, "frontWheelDrive", "allWheelDrive")] public bool rearWheelDrive;
    [DisableIf(EConditionOperator.Or, "rearWheelDrive", "allWheelDrive")] public bool frontWheelDrive;
    [DisableIf(EConditionOperator.Or, "rearWheelDrive", "frontWheelDrive")] public bool allWheelDrive;

    [Header("Speed")] 
    public float topSpeed = 300f;
    public float acceleration = 30f;
    [ReadOnly] public float currentSpeed;
    [ReadOnly] public float currentRpm;

    [Header("Brake")] 
    public float brakeForce = 50.0f;
    public float frontBrakeForce = 300f;
    public float rearBrakeForce = 200f;
    
    [Header("Steering")]
    public float turningSensitivity = 1.0f;
    [DisableIf(EConditionOperator.Or, "rearWheelSteering", "allWheelSteering")] public bool frontWheelSteering;
    [DisableIf(EConditionOperator.Or, "frontWheelSteering", "allWheelSteering")] public bool rearWheelSteering;
    [DisableIf(EConditionOperator.Or, "rearWheelSteering", "frontWheelSteering")] public bool allWheelSteering;
    [EnableIf(EConditionOperator.Or, "frontWheelSteering", "allWheelSteering"), Range(-10f, 150f)] public float frontSteerAngle = 40f;
    [EnableIf(EConditionOperator.Or, "rearWheelSteering", "allWheelSteering"), Range(-10f, 150f)] public float rearSteerAngle = 30f;
    
    [Header("Center of Mass")]
    public Vector3 centerOfMass;
    
    public List<Wheel> wheels;

    private float moveInput;
    private float steerInput;

    private Rigidbody carRigidBody;

    private CarLights carLights;

    private void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = centerOfMass;

        carLights = GetComponent<CarLights>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        GetInputs();
        AnimateWheels();
        //WheelEffects();
    }

    private void LateUpdate()
    {
        Move();
        Steer();
        Brake();

        currentSpeed = carRigidBody.velocity.magnitude * 3.6f;
    }
    
    private void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    private void Move()
    {
        if(rearWheelDrive)
        {
            foreach(Wheel wheel in wheels)
            {
                if(wheel.axel == Axel.Rear)
                    wheel.wheelCollider.motorTorque = moveInput * acceleration * topSpeed * Time.deltaTime;

                if(wheel.axel == Axel.Front)
                    wheel.wheelCollider.motorTorque = 0;
            }
        }

        if(frontWheelDrive)
        {
            foreach(Wheel wheel in wheels)
            {
                if(wheel.axel == Axel.Front)
                    wheel.wheelCollider.motorTorque = moveInput * acceleration * topSpeed * Time.deltaTime;

                if(wheel.axel == Axel.Rear)
                    wheel.wheelCollider.motorTorque = 0;
            }
        }

        if(allWheelDrive)
        {
            foreach(Wheel wheel in wheels)
                wheel.wheelCollider.motorTorque = moveInput * acceleration * topSpeed * Time.deltaTime;
        }
    }

    private void Steer()
    {
        if(frontWheelSteering)
        {
            foreach(Wheel wheel in wheels)
            {
                if(wheel.axel == Axel.Front)
                {
                    float steerAngle = steerInput * turningSensitivity * frontSteerAngle;
                    wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
                }
            }
        }

        if(rearWheelSteering)
        {
            foreach(Wheel wheel in wheels)
            {
                if(wheel.axel == Axel.Rear)
                {
                    float steerAngle = steerInput * turningSensitivity * rearSteerAngle;
                    wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, -steerAngle, 0.6f);
                }
            }
        }

        if(allWheelSteering)
        {
            foreach(Wheel wheel in wheels)
            {
                if(wheel.axel == Axel.Front)
                {
                    float steerAngle = steerInput * turningSensitivity * frontSteerAngle;
                    wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
                }
                
                if(wheel.axel == Axel.Rear)
                {
                    float steerAngle = steerInput * turningSensitivity * rearSteerAngle;
                    wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, -steerAngle, 0.6f);
                }
            }
        }
        
    }

    private void Brake()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            foreach(Wheel wheel in wheels)
                wheel.wheelCollider.brakeTorque = 600 * brakeForce * Time.deltaTime;

            carLights.isBackLightOn = true;
            carLights.OperateBackLights();
        }
        else
        {
            foreach(Wheel wheel in wheels)
                wheel.wheelCollider.brakeTorque = 0;
            
            carLights.isBackLightOn = false;
            carLights.OperateBackLights();
        }
    }

    private void AnimateWheels()
    {
        foreach(Wheel wheel in wheels)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            wheel.wheelModel.transform.position = position;
            wheel.wheelModel.transform.rotation = rotation;
        }
    }

    private void WheelEffects()
    {
        foreach(Wheel wheel in wheels)
        {
            if(Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded && carRigidBody.velocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectsObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.wheelEffectsObj.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }
}
