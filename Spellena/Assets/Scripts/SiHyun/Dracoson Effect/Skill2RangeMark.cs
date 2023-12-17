using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill2RangeMark : MonoBehaviour
{
    public Camera camera;
    // Start is called before the first frame update

    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        // ī�޶��� ���� ���͸� �̿��Ͽ� ���ο� ��ġ ���
        Vector3 targetPosition = camera.transform.position + camera.transform.forward * 1.5f;

        // ������Ʈ ��ġ ����
        targetPosition.y = Mathf.Max(targetPosition.y, 1.1f);
        transform.position = targetPosition;
    }
}
