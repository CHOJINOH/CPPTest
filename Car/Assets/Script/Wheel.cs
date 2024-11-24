using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float rotationSpeed = 10f; // 바퀴 회전 속도
    public float grip = 1.2f; // 바닥 접지력
    public bool isSteerable = false; // 앞바퀴 여부

    private Rigidbody rb; // Rigidbody 변수

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}에 Rigidbody가 없습니다! Rigidbody를 추가하세요.");
            return;
        }

        // Configurable Joint 초기화
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        if (joint == null)
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
            Debug.Log($"{gameObject.name}에 ConfigurableJoint가 추가되었습니다.");
        }

        // 부모의 Rigidbody 대신 자동차 본체의 Rigidbody 가져오기
        Transform carBody = transform.root.Find("Body"); // 자동차 본체를 찾습니다.
        if (carBody == null)
        {
            Debug.LogError("자동차 본체(Body)를 찾을 수 없습니다. Body 이름 확인!");
            return;
        }

        Rigidbody connectedBody = carBody.GetComponent<Rigidbody>();
        if (connectedBody == null)
        {
            Debug.LogError("자동차 본체(Body)에 Rigidbody가 없습니다! 추가하세요.");
            return;
        }

        // Configurable Joint 설정
        ConfigureJoint(joint, connectedBody, isSteerable);

        Debug.Log($"{gameObject.name}에 ConfigurableJoint가 본체에 연결되었습니다.");
    }


    private void ConfigureJoint(ConfigurableJoint joint, Rigidbody connectedBody, bool isSteerable)
    {
        if (joint == null || connectedBody == null)
        {
            Debug.LogError($"Joint 또는 ConnectedBody가 null입니다. Joint: {joint}, ConnectedBody: {connectedBody}");
            return;
        }

        joint.connectedBody = connectedBody;
        joint.anchor = Vector3.zero; // Joint 중심 설정
        joint.axis = Vector3.right;  // 바퀴 회전 축
        joint.secondaryAxis = Vector3.up; // 보조 축(Y축)

        // Motion 설정
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Angular Motion 설정
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        if (isSteerable)
        {
            joint.angularYMotion = ConfigurableJointMotion.Free; // 조향 가능한 바퀴
        }
        else
        {
            joint.angularYMotion = ConfigurableJointMotion.Locked; // 고정된 바퀴
        }

        // Spring 및 Damping 설정
        JointDrive drive = new JointDrive
        {
            positionSpring = 300f, // Spring 값을 낮춰서 튕김 방지
            positionDamper = 50f,  // Damping 값으로 흔들림 제어
            maximumForce = Mathf.Infinity
        };
        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
    }

    public void Rotate(float input)
    {
        if (rb == null) return;

        // Torque 값을 제한하여 비정상적인 움직임 방지
        float maxTorque = 100f; // 최대 Torque 값 제한
        rb.AddTorque(transform.right * Mathf.Clamp(input * rotationSpeed, -maxTorque, maxTorque));
    }


    public void ApplyTraction()
    {
        if (rb == null) return;

        // 접지력 계산 및 적용
        Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.velocity, transform.forward);

        rb.velocity = forwardVelocity + lateralVelocity * Mathf.Clamp(grip, 0.5f, 1.5f); // Grip 제한
    }

    public void Steer(float turnInput)
    {
        if (!isSteerable || rb == null) return;

        // 조향각 계산
        float steerAngle = Mathf.Clamp(turnInput * 30f, -30f, 30f); // 조향 각도 제한
        Quaternion targetRotation = Quaternion.Euler(0f, steerAngle, 0f);
        rb.MoveRotation(transform.parent.rotation * targetRotation);
    }
}
