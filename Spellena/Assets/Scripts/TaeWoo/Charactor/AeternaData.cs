using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AeternaData", menuName = "ScriptableObject/AeternaData")]
public class AeternaData : ScriptableObject
{
    [Tooltip("���׸��� ü��")]
    public int Hp;

    [Tooltip("�ɾ� ������ �ȴ� �ӵ�")]
    public float sitSpeed;
    [Tooltip("�ȴ� �ӵ�")]
    public float walkSpeed;
    [Tooltip("�޸��� �ӵ�")]
    public float runSpeed;
    [Tooltip("���� ����")]
    public float jumpHeight;
    
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

    [Tooltip("��ų1 ��Ż ����")]
    public float skill1Time;
    [Tooltip("��ų1 ��Ż �� Ÿ��")]
    public float skill1DoorCoolTime;
    [Tooltip("��ų1 ��Ż ��ȯ �ִ� ��")]
    public float skill1DoorSpawnMaxRange;
    [Tooltip("��ų1 ��Ż ���� ����")]
    public float skill1DoorRange;
    [Tooltip("��ų1 ��Ż ����� ����")]
    public float skill1DeBuffRatio;

    [Tooltip("��ų2 ���� �ð�")]
    public float skill2DurationTime;
    [Tooltip("��ų2 ����ü ������ �ִ� �ð�")]
    public float skill2HoldTime;
    [Tooltip("��ų2 �� Ÿ��")]
    public float skill2CoolTime;

    [Tooltip("��ų3 ���� �ð�")]
    public float skill3DurationTime;
    [Tooltip("��ų3 �� Ÿ��")]
    public float skill3CoolTime;

    [Tooltip("��ų4 ���� �ð�")]
    public float skill4DurationTime;

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

