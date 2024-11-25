using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCarController : MonoBehaviour
{
    // �ڵ��� �̵� �ӵ�
    private float speed = 10f;
    // �ڵ��� ȸ�� �ӵ�
    private float turnSpeed = 50f;
    // ����� ������
    private Wheel[] wheels;

    // Rigidbody ����
    private Rigidbody rb;

    void Start()
    {
        // Rigidbody ��������
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}�� Rigidbody�� �����ϴ�! Rigidbody�� �߰��ϼ���.");
            return;
        }

        // �ڽ� ��ü���� Wheel ������Ʈ ��������
        wheels = GetComponentsInChildren<Wheel>();
        if (wheels == null || wheels.Length == 0)
        {
            Debug.LogError("�ڵ����� ����� ������ �����ϴ�. �������� �ڽ� ��ü�� �߰��ϼ���.");
            return;
        }
    }

    void FixedUpdate()
    {
        if (wheels == null || wheels.Length == 0) return;

        // ����� �Է�
        float forwardInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // �ڵ��� �̵� ó��
        Vector3 forwardMovement = transform.forward * forwardInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMovement);

        // �ڵ��� ȸ�� ó��
        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // ���� ���� ó��
        foreach (Wheel wheel in wheels)
        {
            if (wheel == null) continue; // ���� wheel�� null�̸� �����մϴ�.

            if (wheel.IsSteerable) // �չ����� ����
            {
                wheel.Steer(turnInput);
            }

            if (Mathf.Abs(forwardInput) > 0.01f) // �̵� �� ���� ȸ��
            {
                wheel.Rotate(forwardInput);
            }

            wheel.ApplyTraction(); // ������ ����
        }
    }
}
