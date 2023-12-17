using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdCamControl : MonoBehaviour
{
    public Transform target;  // ī�޶� ����ٴ� ��� ������Ʈ
    public float rotationSpeed = 2.0f;  // ���콺 ȸ�� �ӵ�
    public float distance = 5.0f;  // ī�޶�� ��� ������Ʈ ���� �Ÿ�

    private void Update()
    {
        // ���콺 �Է��� �޾� ī�޶� ȸ��
        float horizontalInput = Input.GetAxis("Mouse X") * Input.GetJoystickNames().Length == 0 ? Input.GetAxis("Mouse X") : Input.GetAxis("Horizontal");

        // ���� ī�޶��� ���� ����
        float currentHeight = transform.position.y;

        // ��� ������Ʈ�� �������� ���� �Ǵ� ���������� ȸ��
        transform.RotateAround(target.position, Vector3.up, horizontalInput);

        // ī�޶��� ��ġ�� ��� ������Ʈ �ֺ����� �����ϸ鼭 ���� ����
        Vector3 desiredPosition = target.position - transform.forward * distance;
        desiredPosition.y = currentHeight;  // ���� ����
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5.0f);
    }
}
