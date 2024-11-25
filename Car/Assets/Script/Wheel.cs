using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    // ���� ȸ�� �ӵ�
    private float rotationSpeed = 500f; // �ӵ��� �� ũ�� �����ؼ� �ð������� ��Ȯ�ϰ� ǥ��
    // �ٴ� ������
    private float grip = 1.2f;
    // �չ��� ����
    [SerializeField]
    private bool isSteerable = false;

    // Rigidbody ����
    private Rigidbody rb;

    // �ð��� ȸ���� ���� (�ڽ� ������Ʈ)
    [SerializeField]
    private Transform visualWheel;

    public bool IsSteerable // IsSteerable ������Ƽ
    {
        get { return isSteerable; }
        set { isSteerable = value; }
    }

    void Start()
    {
        // Rigidbody ��������
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}�� Rigidbody�� �����ϴ�! Rigidbody�� �߰��ϼ���.");
            return;
        }

        // �θ��� Rigidbody ��� �ڵ��� ��ü�� Rigidbody ��������
        Transform carBody = transform.root.Find("Body"); // �ڵ��� ��ü�� ã���ϴ�.
        if (carBody == null)
        {
            Debug.LogError("�ڵ��� ��ü(Body)�� ã�� �� �����ϴ�. Body �̸� Ȯ��!");
            return;
        }

        Rigidbody connectedBody = carBody.GetComponent<Rigidbody>();
        if (connectedBody == null)
        {
            Debug.LogError("�ڵ��� ��ü(Body)�� Rigidbody�� �����ϴ�! �߰��ϼ���.");
            return;
        }

        // Configurable Joint �ʱ�ȭ
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        if (joint == null)
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
            Debug.Log($"{gameObject.name}�� ConfigurableJoint�� �߰��Ǿ����ϴ�.");
        }

        // Configurable Joint ����
        ConfigureJoint(joint, connectedBody, isSteerable);
        Debug.Log($"{gameObject.name}�� ConfigurableJoint�� ��ü�� ����Ǿ����ϴ�.");

        // �ð��� ������ �����Ǿ� �ִ��� Ȯ��
        if (visualWheel == null)
        {
            Debug.LogError($"{gameObject.name}�� �ð��� ����(visualWheel)�� �������� �ʾҽ��ϴ�. �ڽ� ������Ʈ�� �����ϼ���.");
        }
    }

    // ConfigurableJoint�� �����ϴ� �޼���
    private void ConfigureJoint(ConfigurableJoint joint, Rigidbody connectedBody, bool isSteerable)
    {
        if (joint == null || connectedBody == null)
        {
            Debug.LogError($"Joint �Ǵ� ConnectedBody�� null�Դϴ�. Joint: {joint}, ConnectedBody: {connectedBody}");
            return;
        }

        joint.connectedBody = connectedBody;
        joint.anchor = Vector3.zero; // Joint �߽� ����
        joint.axis = Vector3.right;  // ���� ȸ�� ��
        joint.secondaryAxis = Vector3.up; // ���� ��(Y��)

        // Motion ����
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Angular Motion ����
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        if (isSteerable)
        {
            joint.angularYMotion = ConfigurableJointMotion.Free; // ���� ������ ����
        }
        else
        {
            joint.angularYMotion = ConfigurableJointMotion.Locked; // ������ ����
        }

        // Spring �� Damping ����
        JointDrive drive = new JointDrive
        {
            positionSpring = 300f, // Spring ���� ���缭 ƨ�� ����
            positionDamper = 50f,  // Damping ������ ��鸲 ����
            maximumForce = Mathf.Infinity
        };
        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
    }

    // ������ ȸ����Ű�� �޼��� (�ð��� ȸ�� �߰�)
    public void Rotate(float input)
    {
        if (rb == null) return;

        // Torque ���� �����Ͽ� ���������� ������ ����
        float maxTorque = 100f; // �ִ� Torque �� ����
        rb.AddTorque(transform.right * Mathf.Clamp(input * rotationSpeed, -maxTorque, maxTorque));

        // �ð��� ���� ȸ��
        if (visualWheel != null)
        {
            float rotationAngle = input * rotationSpeed * Time.deltaTime;
            visualWheel.Rotate(Vector3.right, rotationAngle);
        }
    }

    // �������� �����ϴ� �޼���
    public void ApplyTraction()
    {
        if (rb == null) return;

        // ������ ��� �� ����
        Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.velocity, transform.forward);

        rb.velocity = forwardVelocity + lateralVelocity * Mathf.Clamp(grip, 0.5f, 1.5f); // Grip ����
    }

    // ������ ������ ó���ϴ� �޼��� (�ð��� ���� ����)
    public void Steer(float turnInput)
    {
        if (!isSteerable || rb == null) return;

        // ���Ⱒ ���
        float steerAngle = Mathf.Clamp(turnInput * 60f, -60f, 60f); // ���� ���� ����
        Quaternion targetRotation = Quaternion.Euler(0f, steerAngle, 0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * 5f);
    }
}
