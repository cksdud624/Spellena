using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/ElementalOrderData")]
public class ElementalOrderData : ScriptableObject
{
    //ĳ���� ���� ������
    public int hp;

    public float sitSpeed;
    public float sitSideSpeed;
    public float sitBackSpeed;

    public float moveSpeed;
    public float backSpeed;
    public float sideSpeed;
    public float runSpeedRatio;

    public float jumpHeight;

    //��ų ������

    //��ų1
    public float ragnaEdgeCastingTime;
    public float ragnaEdgeFloorLifeTime;
    public float ragnaEdgeCylinderLifeTime;

    //��ų2
    public float burstFlareLifeTime;

    //��ų3
    public float gaiaTiedCastingTime;
    public float gaiaTiedLifeTime;

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
