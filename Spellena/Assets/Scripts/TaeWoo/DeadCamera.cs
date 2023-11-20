using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.InputSystem;

public class DeadCamera : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 sensitivity = new Vector2(6, 6);
    public float distance = 5;

    int index = 0;
    bool isButtonDown = false;

    GameObject targetPlayer;
    Vector3 rayDirection = new Vector3(0,0,1);

    private Vector2 mouseAbsolute;
    private Vector2 smoothMouse;

    private Vector2 mouseDelta;

    List<Character> players = new List<Character>();

    // Ȱ��ȭ �� �� ���� ���� �����ִ� �÷��̾��� ����Ʈ�� �޴´� -> �߰� Ż�� ���
    void OnEnable()
    {
        targetPlayer = gameObject.transform.parent.gameObject;

        Character[] allPlayer = FindObjectsOfType<Character>();

        foreach (var player in allPlayer)
        {
            if (CompareTag(player.gameObject.tag))
            {
                players.Add(player);
            }
        }

    }

    void OnMouseButton()
    {
        if (!isButtonDown)
        {
            index++;
            if (index < 0) index = players.Count - 1;
            if (index >= players.Count) index = 0;
            targetPlayer = players[index].gameObject;
            isButtonDown = !isButtonDown;
        }
    }

    private void Update()
    {
        TPSView();
    }

    void TPSView()
    {
        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x, sensitivity.y));

        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / sensitivity.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / sensitivity.y);

        mouseAbsolute += smoothMouse;

        // ȸ������ Quaternion���� ��ȯ
        Quaternion xQuaternion = Quaternion.AngleAxis(-mouseAbsolute.y, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(mouseAbsolute.x, Vector3.up);

        // Ray�� ������ ȸ�������� ����
        rayDirection = xQuaternion * yQuaternion * Vector3.forward;

        Ray ray = new Ray(targetPlayer.transform.position, rayDirection);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit,distance,LayerMask.NameToLayer("Ground") | LayerMask.NameToLayer("Wall")))
        {
            transform.position = hit.point;
        }

        else
        {
            transform.position = ray.GetPoint(distance);
        }

        transform.LookAt(targetPlayer.transform);
    }
}
