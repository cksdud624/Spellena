using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AeternaData", menuName = "ScriptableObject/AeternaData")]
public class AeternaData : ScriptableObject
{
    [Header("���׸��� ĳ���� ������")]

    public int hp;
    public float sitSpeed;
    public float sitSideSpeed;
    public float sitBackSpeed;
    public float moveSpeed;
    public float backSpeed;
    public float sideSpeed;
    public float runSpeedRatio;
    public float jumpHeight;
    public float headShotRatio;

    [Header("���׸��� �⺻ ���� ������")]

    [Tooltip("�⺻ ���� �� Ÿ��")]
    public float basicAttackTime;
    [Tooltip("�⺻ ���� ����")]
    public float DimenstionSlash_0_lifeTime;
    [Tooltip("�⺻ ���� ������")]
    public int DimenstionSlash_0_Damage;
    [Tooltip("�⺻ ���� ���ǵ�")]
    public int DimenstionSlash_0_Speed;
    [Tooltip("�⺻ ���� ����")]
    public int DimenstionSlash_0_Healing;

    [Header("���׸��� ��ų1 ������")]
    [Tooltip("��ų1 ��Ż ����")]
    public float skill1Time;
    [Tooltip("��ų1 ��Ż �� Ÿ��")]
    public float skill1DoorCoolTime;
    [Tooltip("��ų1 ��Ż ��ȯ �ִ� ��")]
    public float skill1DoorSpawnMaxRange;
    [Tooltip("��ų1 ��Ż ���� ����")]
    public float skill1DoorRange;
    [Tooltip("��ų1 �߾� ���� ��")]
    public float skill1InnerForce;

    [Header("���׸��� ��ų2 ������")]
    [Tooltip("��ų2 ���� �ð�")]
    public float skill2DurationTime;
    [Tooltip("��ų2 ����ü ������ �ִ� �ð�")]
    public float skill2HoldTime;
    [Tooltip("��ų2 �� Ÿ��")]
    public float skill2CoolTime;

    [Header("���׸��� ��ų3 ������")]
    [Tooltip("��ų3 ���� �ð�")]
    public float skill3DurationTime;
    [Tooltip("��ų3 �� Ÿ��")]
    public float skill3CoolTime;

    [Header("���׸��� ��ų4 ������")]
    [Tooltip("��ų4 �ñر� �ڽ�Ʈ")]
    public int skill4Cost;
    [Tooltip("��ų4 ���� �ð�")]
    public float skill4DurationTime;

    [Header("��ų4 1�ܰ� ������")]
    [Tooltip("��ų4 1�ܰ� ���� �ð�")]
    public float skill4Phase1Time;
    [Tooltip("1�ܰ� ���� ����")]
    public float DimenstionSlash_1_lifeTime;
    [Tooltip("1�ܰ� ���� ������")]
    public int DimenstionSlash_1_Damage;
    [Tooltip("1�ܰ� ���� ���ǵ�")]
    public int DimenstionSlash_1_Speed;
    [Tooltip("1�ܰ� ���� ����")]
    public int DimenstionSlash_1_Healing;

    [Header("��ų4 2�ܰ� ������")]
    [Tooltip("��ų4 2�ܰ� ���� �ð�")]
    public float skill4Phase2Time;
    [Tooltip("2�ܰ� ���� ����")]
    public float DimenstionSlash_2_lifeTime;
    [Tooltip("2�ܰ� ���� ������")]
    public int DimenstionSlash_2_Damage;
    [Tooltip("2�ܰ� ���� ���ǵ�")]
    public int DimenstionSlash_2_Speed;
    [Tooltip("2�ܰ� ���� ����")]
    public int DimenstionSlash_2_Healing;

    [Header("��ų4 3�ܰ� ������")]
    [Tooltip("��ų4 3�ܰ� ���� �ð�")]
    public float skill4Phase3Time;
    [Tooltip("3�ܰ� ���� ����")]
    public float DimenstionSlash_3_lifeTime;
    [Tooltip("3�ܰ� ���� ������")]
    public int DimenstionSlash_3_Damage;
    [Tooltip("3�ܰ� ���� ���ǵ�")]
    public int DimenstionSlash_3_Speed;
    [Tooltip("3�ܰ� ���� ����")]
    public int DimenstionSlash_3_Healing;
}

