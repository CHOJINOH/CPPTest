using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    // 바퀴 회전 속도
    private float rotationSpeed = 500f; // 속도를 더 크게 설정해서 시각적으로 명확하게 표현
    // 바닥 접지력
    private float grip = 1.2f;
    // 앞바퀴 여부
    [SerializeField]
    private bool isSteerable = false;

    // Rigidbody 변수
    private Rigidbody rb;

    // 시각적 회전용 바퀴 (자식 오브젝트)
    [SerializeField]
    private Transform visualWheel;

    public bool IsSteerable // IsSteerable 프로퍼티
    {
        get { return isSteerable; }
        set { isSteerable = value; }
    }

    void Start()
    {
        // Rigidbody 가져오기
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}에 Rigidbody가 없습니다! Rigidbody를 추가하세요.");
            return;
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

        // Configurable Joint 초기화
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        if (joint == null)
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
            Debug.Log($"{gameObject.name}에 ConfigurableJoint가 추가되었습니다.");
        }

        // Configurable Joint 설정
        ConfigureJoint(joint, connectedBody, isSteerable);
        Debug.Log($"{gameObject.name}에 ConfigurableJoint가 본체에 연결되었습니다.");

        // 시각적 바퀴가 설정되어 있는지 확인
        if (visualWheel == null)
        {
            Debug.LogError($"{gameObject.name}에 시각적 바퀴(visualWheel)가 설정되지 않았습니다. 자식 오브젝트로 설정하세요.");
        }
    }

    // ConfigurableJoint를 설정하는 메서드
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

    // 바퀴를 회전시키는 메서드 (시각적 회전 추가)
    public void Rotate(float input)
    {
        if (rb == null) return;

        // Torque 값을 제한하여 비정상적인 움직임 방지
        float maxTorque = 100f; // 최대 Torque 값 제한
        rb.AddTorque(transform.right * Mathf.Clamp(input * rotationSpeed, -maxTorque, maxTorque));

        // 시각적 바퀴 회전
        if (visualWheel != null)
        {
            float rotationAngle = input * rotationSpeed * Time.deltaTime;
            visualWheel.Rotate(Vector3.right, rotationAngle);
        }
    }

    // 접지력을 적용하는 메서드
    public void ApplyTraction()
    {
        if (rb == null) return;

        // 접지력 계산 및 적용
        Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.velocity, transform.forward);

        rb.velocity = forwardVelocity + lateralVelocity * Mathf.Clamp(grip, 0.5f, 1.5f); // Grip 제한
    }

    // 바퀴의 조향을 처리하는 메서드 (시각적 조향 포함)
    public void Steer(float turnInput)
    {
        if (!isSteerable || rb == null) return;

        // 조향각 계산
        float steerAngle = Mathf.Clamp(turnInput * 60f, -60f, 60f); // 조향 각도 제한
        Quaternion targetRotation = Quaternion.Euler(0f, steerAngle, 0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * 5f);
    }
}
