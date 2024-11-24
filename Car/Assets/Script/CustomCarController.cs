using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCarController : MonoBehaviour
{
    public float speed = 10f; // �ڵ��� �̵� �ӵ�
    public float turnSpeed = 50f; // �ڵ��� ȸ�� �ӵ�
    public Wheel[] wheels; // ����� ������

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}�� Rigidbody�� �����ϴ�! Rigidbody�� �߰��ϼ���.");
            return;
        }
    }

    void FixedUpdate()
    {
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
            if (wheel.isSteerable) // �չ����� ����
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
