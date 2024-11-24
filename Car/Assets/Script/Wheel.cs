using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float rotationSpeed = 10f; // ���� ȸ�� �ӵ�
    public float grip = 1.2f; // �ٴ� ������
    public bool isSteerable = false; // �չ��� ����

    private Rigidbody rb; // Rigidbody ����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}�� Rigidbody�� �����ϴ�! Rigidbody�� �߰��ϼ���.");
            return;
        }

        // Configurable Joint �ʱ�ȭ
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        if (joint == null)
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
            Debug.Log($"{gameObject.name}�� ConfigurableJoint�� �߰��Ǿ����ϴ�.");
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

        // Configurable Joint ����
        ConfigureJoint(joint, connectedBody, isSteerable);

        Debug.Log($"{gameObject.name}�� ConfigurableJoint�� ��ü�� ����Ǿ����ϴ�.");
    }


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

    public void Rotate(float input)
    {
        if (rb == null) return;

        // Torque ���� �����Ͽ� ���������� ������ ����
        float maxTorque = 100f; // �ִ� Torque �� ����
        rb.AddTorque(transform.right * Mathf.Clamp(input * rotationSpeed, -maxTorque, maxTorque));
    }


    public void ApplyTraction()
    {
        if (rb == null) return;

        // ������ ��� �� ����
        Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.velocity, transform.forward);

        rb.velocity = forwardVelocity + lateralVelocity * Mathf.Clamp(grip, 0.5f, 1.5f); // Grip ����
    }

    public void Steer(float turnInput)
    {
        if (!isSteerable || rb == null) return;

        // ���Ⱒ ���
        float steerAngle = Mathf.Clamp(turnInput * 30f, -30f, 30f); // ���� ���� ����
        Quaternion targetRotation = Quaternion.Euler(0f, steerAngle, 0f);
        rb.MoveRotation(transform.parent.rotation * targetRotation);
    }
}
