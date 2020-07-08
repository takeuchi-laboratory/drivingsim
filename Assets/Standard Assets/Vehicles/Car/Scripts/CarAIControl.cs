
using System;
using UnityEngine;
using Random = UnityEngine.Random;

#pragma warning disable 649
namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarAIControl : MonoBehaviour
    {
        public enum BrakeCondition
        {
            NeverBrake,                 // the car simply accelerates at full throttle all the time.    �����P�Ƀt���X���b�g���ŉ������邾���̃��[�h
            TargetDirectionDifference,  // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.  �ڕW�̕����]���ɍ��킹�ău���[�L�������Ă����B���[�g�x�[�X��AI�A�R�[�i�[�ł̌����ɕ֗�
            TargetDistance,             // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to  �Ԃ̓^�[�Q�b�g�̕����Ɋ֌W�Ȃ��A�^�[�Q�b�g�ɋ߂Â��ƃu���[�L�������܂��B
                                        // head for a stationary target and come to rest when it arrives there. �Ԃ��Œ肳�ꂽ�ڕW�Ɍ������Ă��炢�A�����ɓ���������x��ł��炢�����ꍇ�ɕ֗��ł��B

        }

        // This script provides input to the car controller in the same way that the user control script does.
        // As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.

        // "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
        // in speed and direction while driving towards their target.

        [SerializeField] [Range(0, 1)] private float m_CautiousSpeedFactor = 0.05f;               // percentage of max speed to use when being maximally cautious   �ő�̒��ӂ𕥂��Ă���Ƃ��̍ő呬�x�̊���?
        [SerializeField] [Range(0, 180)] private float m_CautiousMaxAngle = 50f;                  // angle of approaching corner to treat as warranting maximum caution     �ő���̒��ӂ��K�v�ȃR�[�i�[�ւ̃A�v���[�`�p�x
        [SerializeField] private float m_CautiousMaxDistance = 100f;                              // distance at which distance-based cautiousness begins       �����x�[�X�̐T�d�����n�܂鋗��
        [SerializeField] private float m_CautiousAngularVelocityFactor = 30f;                     // how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!) AI�����g�̌��݂̊p���x���l����Ƃ��A�ǂꂾ���T�d�ł���ׂ����B(��]���Ă���ꍇ�͉������ɂ߂�)
        [SerializeField] private float m_SteerSensitivity = 0.05f;                                // how sensitively the AI uses steering input to turn to the desired direction    AI���X�e�A�����O���͂��g���āA�ǂꂾ�����o�I�Ɋ�]�̕����ɋȂ��邩�B
        [SerializeField] private float m_AccelSensitivity = 0.04f;                                // How sensitively the AI uses the accelerator to reach the current desired speed�@���݂̊�]�̑��x�ɓ��B���邽�߂ɁAAI���ǂꂾ���q���ɃA�N�Z�����g�p���Ă��邩
        [SerializeField] private float m_BrakeSensitivity = 1f;                                   // How sensitively the AI uses the brake to reach the current desired speed   AI���ǂꂾ���q���Ƀu���[�L���g���āA���݂̊�]�̑��x�ɓ��B���邩
        [SerializeField] private float m_LateralWanderDistance = 3f;                              // how far the car will wander laterally towards its target   �Ԃ��ڕW�Ɍ������ĉ������ɂǂ̂��炢�ӂ����
        [SerializeField] private float m_LateralWanderSpeed = 0.1f;                               // how fast the lateral wandering will fluctuate  ���h�ꂪ�ǂ̂��炢�̑����ŕϓ����邩
        [SerializeField] [Range(0, 1)] private float m_AccelWanderAmount = 0.1f;                  // how much the cars acceleration will wander �Ԃ̉������ǂꂾ���ӂ����
        [SerializeField] private float m_AccelWanderSpeed = 0.1f;                                 // how fast the cars acceleration wandering will fluctuate    �Ԃ̉����̂ӂ�����ǂ̂��炢�̑����ŕϓ����邩
        [SerializeField] private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance; // what should the AI consider when accelerating/braking? ����Ȋ����Ń��[�h�ǉ��ł���
        [SerializeField] private bool m_Driving;                                                  // whether the AI is currently actively driving or stopped.   �~�܂��Ă��邩�ǂ���
        [SerializeField] private Transform m_Target;                                              // 'target' the target object to aim for. �ڕW�̃^�[�Q�b�g�����߂�
        [SerializeField] private bool m_StopWhenTargetReached;                                    // should we stop driving when we reach the target?   �ڕW�ɓ��B�����Ƃ��ɉ^�]����߂邩
        [SerializeField] private float m_ReachTargetThreshold = 2;                                // proximity to target to consider we 'reached' it, and stop driving. ���B�����ƍl���邽�߂̖ڕW�ւ̋߂��A�����ĉ^�]���~�߂邽�߂̖ڕW�ւ̋߂��B

        private float m_RandomPerlin;             // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
        private CarController m_CarController;    // Reference to actual car controller we are controlling
        private float m_AvoidOtherCarTime;        // time until which to avoid the car we recently collided with        //�֌W�Ȃ�
        private float m_AvoidOtherCarSlowdown;    // how much to slow down due to colliding with another car, whilst avoiding   //�֌W�Ȃ�
        private float m_AvoidPathOffset;          // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding    //�֌W�Ȃ�
        private Rigidbody m_Rigidbody;

        /*public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);    //Mathf.Clamp(�͈͓��Ɏw�肵�����l,�ŏ��l,�ő�l)
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake * m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }


            CalculateRevs();
            GearChanging();

            AddDownForce();
            CheckForWheelSpin();
            TractionControl();
        }*/

        private void Awake()//�N�����Ɉ�񂾂�����
        {
            // get the car controller reference
            m_CarController = GetComponent<CarController>();

            // give the random perlin a random value
            m_RandomPerlin = Random.value*100;

            m_Rigidbody = GetComponent<Rigidbody>();
        }


        private void FixedUpdate()  //���b�����ƂɌĂ΂��
        {
            

            if (m_Target == null || !m_Driving) //�^�[�Q�b�g���������A�h���C�u���[�h�łȂ��Ƃ�
            {
                // Car should not be moving,
                // use handbrake to stop
                m_CarController.Move(0, 0, -1f, 1f);    //�����Ȃ�
            }
            else
            {
             
                Vector3 fwd = transform.forward;        //�Ԃ̐��ʂ̕���
                if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed*0.1f)  //velocity�͑��x�x�N�g���Amagnitude�̓x�N�g���̒����A�Ȃ̂ŃX�s�[�h
                {
                    fwd = m_Rigidbody.velocity;     //���i��ł���ق��ɐi��
                }

                float desiredSpeed = m_CarController.MaxSpeed;


                switch (m_BrakeCondition)
                {
                    case BrakeCondition.TargetDirectionDifference:
                        {
                            // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.

                            // check out the angle of our target compared to the current direction of the car
                            float approachingCornerAngle = Vector3.Angle(m_Target.forward, fwd);    //2�_�Ԃ̊p�x��Ԃ�

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude*m_CautiousAngularVelocityFactor; //rigidbody�̉�]�p�x*AI�����g�̌��݂̊p���x���l����Ƃ��A�ǂꂾ���T�d�ł���ׂ����B

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.InverseLerp(0, m_CautiousMaxAngle,   //Mathf.InverseLerp(a,b,value)�ca��0,b��1�Ƃ����Ƃ���value�̒l(���K��)
                                                                           Mathf.Max(spinningAngle, //Mathf.Max(a,b)�ca,b�̑傫���ق���Ԃ�
                                                                                     approachingCornerAngle));
                            desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed*m_CautiousSpeedFactor, //Mathf.Lerp(a,b,t)�ct�̒l�ɂ����from����to�܂ł̒��Ԓn���擾
                                                      cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.TargetDistance:
                        {
                            // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
                            // head for a stationary target and come to rest when it arrives there.

                            // check out the distance to target
                            Vector3 delta = m_Target.position - transform.position; //�^�[�Q�b�g�Ǝ����Ƃ̈ʒu�̍�
                            float distanceCautiousFactor = Mathf.InverseLerp(m_CautiousMaxDistance, 0, delta.magnitude);    //���K�����ꂽdelta�̒���

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude*m_CautiousAngularVelocityFactor;    //rigidbody�̉�]�p�x*AI�����g�̌��݂̊p���x���l����Ƃ��A�ǂꂾ���T�d�ł���ׂ����B

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.Max(
                                Mathf.InverseLerp(0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
                            desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed*m_CautiousSpeedFactor,
                                                      cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.NeverBrake:
                        break;
                }

                // Evasive action due to collision with other cars:

                // our target position starts off as the 'real' target position
                Vector3 offsetTargetPos = m_Target.position;

                // if are we currently taking evasive action to prevent being stuck against another car:
                if (Time.time < m_AvoidOtherCarTime)    //���̏������Ǝ��s����Ȃ�
                {
                    // slow down if necessary (if we were behind the other car when collision occured)
                    desiredSpeed *= m_AvoidOtherCarSlowdown;        //�ǂ̊����ő��x�����炷��?

                    // and veer towards the side of our path-to-target that is away from the other car
                    offsetTargetPos += m_Target.right*m_AvoidPathOffset;
                }
                else
                {
                    // no need for evasive action, we can just wander across the path-to-target in a random way,
                    // which can help prevent AI from seeming too uniform and robotic in their driving      //�^�[�Q�b�g�܂Ń����_���ɐi�ދ@�\(�߂��Ⴍ����ז�)
                    offsetTargetPos += m_Target.right;
                                        /*
                                       (Mathf.PerlinNoise(Time.time*m_LateralWanderSpeed, m_RandomPerlin)*2 - 1)*
                                       m_LateralWanderDistance;*/
                }

                // use different sensitivity depending on whether accelerating or braking:
                float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
                                                  ? m_BrakeSensitivity
                                                  : m_AccelSensitivity;

                // decide the actual amount of accel/brake input to achieve desired speed.
                float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed)*accelBrakeSensitivity, -1, 1);

                // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
                // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
                accel *= (1 - m_AccelWanderAmount) 
                            /*+
                         (Mathf.PerlinNoise(Time.time*m_AccelWanderSpeed, m_RandomPerlin)*m_AccelWanderAmount)*/;

                // calculate the local-relative position of the target, to steer towards
                Vector3 localTarget = transform.InverseTransformPoint(offsetTargetPos);

                // work out the local angle towards the target
                float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z)*Mathf.Rad2Deg;

                // get the amount of steering needed to aim the car towards the target
                float steer = Mathf.Clamp(targetAngle*m_SteerSensitivity, -1, 1)*Mathf.Sign(m_CarController.CurrentSpeed);

                // feed input to the car controller.
                m_CarController.Move(steer, accel, accel, 0f);

                // if appropriate, stop driving when we're close enough to the target.
                if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold)
                {
                    //m_Driving = false;
                    m_CarController.Move(0, 0, -1f, 0f);
                } 
                
                    
                
            }
        }


        private void OnCollisionStay(Collision col) //�֌W�Ȃ�?
        {
            // detect collision against other cars, so that we can take evasive action
            if (col.rigidbody != null)
            {
                var otherAI = col.rigidbody.GetComponent<CarAIControl>();
                if (otherAI != null)
                {
                    // we'll take evasive action for 1 second
                    m_AvoidOtherCarTime = Time.time + 1;

                    // but who's in front?...
                    if (Vector3.Angle(transform.forward, otherAI.transform.position - transform.position) < 90)
                    {
                        // the other ai is in front, so it is only good manners that we ought to brake...
                        m_AvoidOtherCarSlowdown = 0.5f;
                    }
                    else
                    {
                        // we're in front! ain't slowing down for anybody...
                        m_AvoidOtherCarSlowdown = 1;
                    }

                    // both cars should take evasive action by driving along an offset from the path centre,
                    // away from the other car
                    var otherCarLocalDelta = transform.InverseTransformPoint(otherAI.transform.position);
                    float otherCarAngle = Mathf.Atan2(otherCarLocalDelta.x, otherCarLocalDelta.z);
                    m_AvoidPathOffset = m_LateralWanderDistance*-Mathf.Sign(otherCarAngle);
                }
            }
        }       //�֌W�Ȃ�?


        public void SetTarget(Transform target)
        {
            m_Target = target;
            m_Driving = true;
        }
    }
}
