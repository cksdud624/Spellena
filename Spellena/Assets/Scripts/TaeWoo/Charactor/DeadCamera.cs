using System.Collections.Generic;
using UnityEngine;
using Player;
using System;

public class DeadCamera : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 clampInDegrees = new Vector2(360, 120);
    public Vector2 Sensitivity = new Vector2(6, 6);
    public bool isOpposite = false;
    public float distance = 2;

    [HideInInspector]
    public DeathCamUI deathCamUI;

    int index = 0;
    GameObject targetPlayer;
    Vector3 rayDirection = new Vector3(0,0,1);
    Vector3 offset = new Vector3(0.0f, 1.5f, 0.0f);

    private Vector2 mouseAbsolute;
    private Vector2 smoothMouse;

    private Vector2 mouseDelta;
    private SettingManager settingManager;
    private bool showSettings = false;

    List<Character> players = new List<Character>();
    private void Start()
    {
        deathCamUI = GameObject.Find("DeathUI").GetComponent<DeathCamUI>();
        LinkSettingManager();
    }

    void LinkSettingManager()
    {
        GameObject temp = GameObject.Find("SettingManager");

        if (temp == null)
        {
            Debug.LogError("SettingManager�� ã�� �� �����ϴ�.");
            return;
        }

        settingManager = temp.GetComponent<SettingManager>();

        if (settingManager == null)
        {
            Debug.LogError("SettingManager�� Component�� ã�� �� �����ϴ�.");
            return;
        }
    }

    // Ȱ��ȭ �� �� ���� ���� �����ִ� �÷��̾��� ����Ʈ�� �޴´� -> �߰� Ż�� ���
    void OnEnable()
    {
        targetPlayer = gameObject.transform.parent.gameObject;

        Character[] allPlayer = FindObjectsOfType<Character>();

        foreach (var player in allPlayer)
        {
            if (player.name == gameObject.name) continue;

            if (CompareTag(player.gameObject.tag))
            {
                players.Add(player);
            }
        }

    }

    private void Update()
    {
        if(settingManager.SettingPanel.activeSelf)
        {
            SetData();
            ShowCursor();
        }

        else
        {
            EraseCursor();
            if (Input.GetMouseButtonDown(0))
            {
                ChangePlayerCam();
                ChangeCamera(targetPlayer.name);
            }

            TPSView();
        }
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EraseCursor()
    {
        // Ŀ���� ����� ������Ų��.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetData()
    {
        Sensitivity = new Vector2(settingManager.sensitivityVal * 20, settingManager.sensitivityVal * 20);
        isOpposite = settingManager.isOpposite;
    }

    void ChangePlayerCam()
    {
        index++;
        if (index < 0) index = players.Count - 1;
        if (index >= players.Count) index = 0;
        targetPlayer = players[index].gameObject;
    }

    void ChangeCamera(string name)
    {
        deathCamUI.showPlayerCamID.text = name;
        deathCamUI.SwitchCam();
    }

    void TPSView()
    {
        if(isOpposite)
        {
            mouseDelta = new Vector2(-Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }

        else
        {
            mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }

        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(Sensitivity.x, Sensitivity.y));

        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / Sensitivity.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / Sensitivity.y);

        mouseAbsolute += smoothMouse;

        // ���콺 �̵��� ���� ray ȸ�� 
        float rotationY = -mouseAbsolute.x;
        float rotationX = mouseAbsolute.y;

        if (clampInDegrees.x < 360)
            mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        if (clampInDegrees.y < 360)
            mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        // ȸ������ Quaternion���� ��ȯ
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.right);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.up);

        // Ray�� ������ ȸ�������� ����
        rayDirection = yQuaternion * xQuaternion * Vector3.forward;

        Ray ray = new Ray(targetPlayer.transform.position + offset, rayDirection);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit,distance))
        {
            if (hit.collider.tag == "Ground")
            {
                transform.position = hit.point;
            }

            else
            {
                transform.position = ray.GetPoint(distance);
            }
        }

        else
        {
            transform.position = ray.GetPoint(distance);
        }

        transform.LookAt(targetPlayer.transform.position + offset);
    }
}
