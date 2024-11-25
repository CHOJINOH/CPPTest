using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCarController : MonoBehaviour
{
    // 자동차 이동 속도
    private float speed = 10f;
    // 자동차 회전 속도
    private float turnSpeed = 50f;
    // 연결된 바퀴들
    private Wheel[] wheels;

    // Rigidbody 변수
    private Rigidbody rb;

    void Start()
    {
        // Rigidbody 가져오기
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name}에 Rigidbody가 없습니다! Rigidbody를 추가하세요.");
            return;
        }

        // 자식 객체에서 Wheel 컴포넌트 가져오기
        wheels = GetComponentsInChildren<Wheel>();
        if (wheels == null || wheels.Length == 0)
        {
            Debug.LogError("자동차에 연결된 바퀴가 없습니다. 바퀴들을 자식 객체로 추가하세요.");
            return;
        }
    }

    void FixedUpdate()
    {
        if (wheels == null || wheels.Length == 0) return;

        // 사용자 입력
        float forwardInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // 자동차 이동 처리
        Vector3 forwardMovement = transform.forward * forwardInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMovement);

        // 자동차 회전 처리
        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // 바퀴 동작 처리
        foreach (Wheel wheel in wheels)
        {
            if (wheel == null) continue; // 만약 wheel이 null이면 무시합니다.

            if (wheel.IsSteerable) // 앞바퀴만 조향
            {
                wheel.Steer(turnInput);
            }

            if (Mathf.Abs(forwardInput) > 0.01f) // 이동 시 바퀴 회전
            {
                wheel.Rotate(forwardInput);
            }

            wheel.ApplyTraction(); // 접지력 적용
        }
    }
}
