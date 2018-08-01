using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LF_Leg : MonoBehaviour {

    GameObject LF_Thigh;
    GameObject LF_Calf;

    private float velocity_ = 250;
    private float force_ = 300;

    void objectInit() {

        LF_Thigh = GameObject.Find("LF_Thigh");
        LF_Calf = GameObject.Find("LF_Calf");

        HingeJoint hinge_LFT = LF_Thigh.GetComponent<HingeJoint>();

        JointLimits limits = hinge_LFT.limits;
        JointMotor motor = hinge_LFT.motor;

        //DOG2.0 - 1

        //limits.min = 5;
        //limits.max = 85;
        //hinge_LFT.useLimits = true;
        //hinge_LFT.limits = limits;

        //motor.targetVelocity = velocity_;
        //motor.force = force_;
        //motor.freeSpin = false;
        //hinge_LFT.useMotor = true;
        //hinge_LFT.motor = motor;


        //HingeJoint hinge_LFC = LF_Calf.GetComponent<HingeJoint>();
        //limits = hinge_LFC.limits;
        //motor = hinge_LFC.motor;

        //limits.min = -120;
        //limits.max = -20;
        //hinge_LFC.useLimits = true;
        //hinge_LFC.limits = limits;

        //motor.targetVelocity = -velocity_;
        //motor.force = force_;
        //motor.freeSpin = false;
        //hinge_LFC.useMotor = true;
        //hinge_LFC.motor = motor;



        limits.min = -50;
        limits.max = -5;
        hinge_LFT.useLimits = true;
        hinge_LFT.limits = limits;

        motor.targetVelocity = -velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_LFT.useMotor = true;
        hinge_LFT.motor = motor;


        HingeJoint hinge_LFC = LF_Calf.GetComponent<HingeJoint>();
        limits = hinge_LFC.limits;
        motor = hinge_LFC.motor;

        limits.min = 20;
        limits.max = 100;
        hinge_LFC.useLimits = true;
        hinge_LFC.limits = limits;

        motor.targetVelocity = velocity_;
        motor.force = force_;
        motor.freeSpin = false;
        hinge_LFC.useMotor = true;
        hinge_LFC.motor = motor;






    }

    void Start () {

        objectInit();
    }
	

    public void Thigh_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_LFT = LF_Thigh.GetComponent<HingeJoint>();

        float angle_ = hinge_LFT.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr*15+100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_LFT.motor;       

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;
            
        }

        hinge_LFT.motor = motor;

    }

    public void Calf_RunAngle(float angle)
    {

        float angle_deg = angle * Mathf.Rad2Deg; //弧度转角度

        HingeJoint hinge_LFC = LF_Calf.GetComponent<HingeJoint>();

        float angle_ = hinge_LFC.angle;

        float angle_dirr = Mathf.Abs(angle_deg - angle_);

        float velocity = angle_dirr * 10 + 100;  //有一定减速

        if (velocity > velocity_) velocity = velocity_;

        JointMotor motor = hinge_LFC.motor;

        if (angle_deg > angle_)
        {

            motor.targetVelocity = velocity;

        }
        else if (angle_deg < angle_)
        {
            motor.targetVelocity = -velocity;

        }

        hinge_LFC.motor = motor;

    }

    public float Thigh_GetAngle()
    {

        HingeJoint hinge_LFT = LF_Thigh.GetComponent<HingeJoint>();

        float angle_rad = hinge_LFT.angle * Mathf.Deg2Rad;

        return angle_rad;

    }

    public float Calf_GetAngle()
    {

        HingeJoint hinge_LFC = LF_Calf.GetComponent<HingeJoint>();

        float angle_rad = hinge_LFC.angle * Mathf.Deg2Rad;

        return angle_rad;

    }
}
