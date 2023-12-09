using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameCenterTestData", menuName = "ScriptableObject/GameCenterTestData")]
public class GameCenterTestData : ScriptableObject
{
    [Header("���Ӽ��� ������")]

    [Tooltip("��, ĳ���� �ε� Ÿ��")]
    public float loadingTime;
    [Tooltip("ĳ���� ���� Ÿ��")]
    public float characterSelectTime;
    [Tooltip("���� �غ� �ð�")]
    public float readyTime;
    [Tooltip("�÷��̾� ������ Ÿ��")]
    public float playerRespawnTime;
    [Tooltip("��ý�Ʈ Ÿ��")]
    public float assistTime;

    [Tooltip("�κ��� ��Ÿ��")]
    public float angelStatueCoolTime;
    [Tooltip("�κ��� �ʴ� ü�� ������")]
    public int angelStatueHpPerTime;
    [Tooltip("�κ��� ȿ�� ���� �ð�")]
    public int angelStatueContinueTime;

    [Tooltip("���� ��ȯ �� �Դ� ����")]
    public float occupyingGaugeRate;
    [Tooltip("���� ��ȯ�ϴ� �ð�")]
    public float occupyingReturnTime;
    [Tooltip("���� % �Դ� ����")]
    public float occupyingRate;
    [Tooltip("�߰��ð��� �߻��ϴ� ���� ������")]
    public float occupyingComplete;
    [Tooltip("�߰� �ð�")]
    public float roundEndTime;
    [Tooltip("���� ��� Ȯ�� �ð�")]
    public float roundEndResultTime;
}
