using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/ElementalOrderData")]
public class ElementalOrderData : ScriptableObject
{
    //ĳ���� ���� ������
    public int hp;
    public float moveSpeed;
    public float backSpeed;
    public float sideSpeed;
    public float runSpeedRatio;
    public float sitSpeed;
    public float sitSideSpeed;
    public float sitBackSpeed;
    public float jumpHeight;
    public float headShotRatio;

    //��ų ������

    //��ų1
    public float ragnaEdgeFloorDamage;
    public float ragnaEdgeCylinderDamage;
    public float rangaEdgeCoolDownTime;
    public float ragnaEdgeCastingTime;
    public float ragnaEdgeFloorLifeTime;
    public float ragnaEdgeCylinderLifeTime;
    public string rangaEdgeCylinderDebuff;

    //��ų2
    public int burstFlareBullet;
    public float burstFlareDamage;
    public float burstFlareCoolDownTime;
    public float burstFlareCastingTime;
    public float burstFlareLifeTime;

    //��ų3
    public float[] gaiaTiedDamage;
    public float gaiaTiedCoolDownTime;
    public float gaiaTiedCastingTime;
    public float gaiaTiedMaxDistace;
    public float[] gaiaTiedLifeTime;

    //��ų4
    public float meteorStrikeCastingTime;
    public float meteorStrikeLifeTime;

    //��ų5
    public float terraBreakCastingTime;
    public float terraBreakLifeTime;

    //��ų6
    public float eterialStormCastingTime;
    public float eterialStormLifeTime;

}
