using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetManager : MonoBehaviour
{
    public GoogleSheetData googleSheetData;
    [Header("")]
    public AeternaData aeternaData;

    private string URL;

    // ���ڿ��� ���� 2���� �迭
    private string[,] dividData;

    IEnumerator Start()
    {
        for(int i = 0; i < googleSheetData.gooleSheets.Length; i++)
        {
            URL = googleSheetData.gooleSheets[i].address + googleSheetData.exportFormattsv + googleSheetData.andRange
                + googleSheetData.gooleSheets[i].range_1 + ":" + googleSheetData.gooleSheets[i].range_2;

            UnityWebRequest www = UnityWebRequest.Get(URL);
            yield return www.SendWebRequest();   // URL�� ���

            string Data = www.downloadHandler.text; // string �����ͷ� �޾ƿ�
            Debug.Log("End Receving");

            DividText(Data);
            GiveData(i);
        }
        
    }


    void DividText(string tsv)
    {
        //���� �������� ������.
        string[] row = tsv.Split('\n');
        // ���� ��
        int rowSize = row.Length;
        // ���� ��
        int columSize = row[0].Split('\t').Length;

        dividData = new string[columSize, rowSize];

        // ���� ���̷� ���η� ������.
        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            // ���� ���̷� ���η� ������.
            for (int j = 0; j < columSize; j++)
            {
                dividData[j, i] = column[j];
                //Debug.Log(column[j]);
            }
        }
    }

    void GiveData(int index)
    {
        switch(index)
        {
            case 0:
                GiveAeternaData();
                break;
            default:
                break;
        }
    }

    void GiveAeternaData()
    {
        Debug.Log(dividData[28, 0]);
        //aeternaData.Hp = (int)dividData[28,0];
        //aeternaData.sitSpeed;
        //aeternaData.walkSpeed;
        //[Tooltip("�޸��� �ӵ�")]
        //aeternaData.runSpeed;
        //[Tooltip("���� ����")]
        //aeternaData.jumpHeight;

        //[Header("���׸��� �⺻ ���� ������")]
        //[Tooltip("�⺻ ���� �� Ÿ��")]
        //aeternaData.basicAttackTime;
        //[Tooltip("�⺻ ���� ����")]
        //aeternaData.DimenstionSlash_0_lifeTime;
        //[Tooltip("�⺻ ���� ������")]
        //aeternaData.DimenstionSlash_0_Damage;
        //[Tooltip("�⺻ ���� ���ǵ�")]
        //aeternaData.DimenstionSlash_0_Speed;
        //[Tooltip("�⺻ ���� ����")]
        //aeternaData.DimenstionSlash_0_Healing;

        //[Header("���׸��� ��ų1 ������")]
        //[Tooltip("��ų1 ��Ż ����")]
        //aeternaData.skill1Time;
        //[Tooltip("��ų1 ��Ż �� Ÿ��")]
        //aeternaData.skill1DoorCoolTime;
        //[Tooltip("��ų1 ��Ż ��ȯ �ִ� ��")]
        //aeternaData.skill1DoorSpawnMaxRange;
        //[Tooltip("��ų1 ��Ż ���� ����")]
        //aeternaData.skill1DoorRange;
        //[Tooltip("��ų1 ��Ż ����� ����")]
        //aeternaData.skill1DeBuffRatio;

        //[Header("���׸��� ��ų2 ������")]
        //[Tooltip("��ų2 ���� �ð�")]
        //aeternaData.skill2DurationTime;
        //[Tooltip("��ų2 ����ü ������ �ִ� �ð�")]
        //aeternaData.skill2HoldTime;
        //[Tooltip("��ų2 �� Ÿ��")]
        //aeternaData.skill2CoolTime;

        //[Header("���׸��� ��ų3 ������")]
        //[Tooltip("��ų3 ���� �ð�")]
        //aeternaData.skill3DurationTime;
        //[Tooltip("��ų3 �� Ÿ��")]
        //aeternaData.skill3CoolTime;

        //[Header("���׸��� ��ų4 ������")]
        //[Tooltip("��ų4 �ñر� �ڽ�Ʈ")]
        //aeternaData.skill4Cost;
        //[Tooltip("��ų4 ���� �ð�")]
        //aeternaData.skill4DurationTime;

        //[Header("��ų4 1�ܰ� ������")]
        //[Tooltip("��ų4 1�ܰ� ���� �ð�")]
        //aeternaData.skill4Phase1Time;
        //[Tooltip("1�ܰ� ���� ����")]
        //aeternaData.DimenstionSlash_1_lifeTime;
        //[Tooltip("1�ܰ� ���� ������")]
        //aeternaData.DimenstionSlash_1_Damage;
        //[Tooltip("1�ܰ� ���� ���ǵ�")]
        //aeternaData.DimenstionSlash_1_Speed;
        //[Tooltip("1�ܰ� ���� ����")]
        //aeternaData.DimenstionSlash_1_Healing;

        //[Header("��ų4 2�ܰ� ������")]
        //[Tooltip("��ų4 2�ܰ� ���� �ð�")]
        //aeternaData.skill4Phase2Time;
        //[Tooltip("2�ܰ� ���� ����")]
        //aeternaData.DimenstionSlash_2_lifeTime;
        //[Tooltip("2�ܰ� ���� ������")]
        //aeternaData.DimenstionSlash_2_Damage;
        //[Tooltip("2�ܰ� ���� ���ǵ�")]
        //aeternaData.DimenstionSlash_2_Speed;
        //[Tooltip("2�ܰ� ���� ����")]
        //aeternaData.DimenstionSlash_2_Healing;

        //[Header("��ų4 3�ܰ� ������")]
        //[Tooltip("��ų4 3�ܰ� ���� �ð�")]
        //aeternaData.skill4Phase3Time;
        //[Tooltip("3�ܰ� ���� ����")]
        //aeternaData.DimenstionSlash_3_lifeTime;
        //[Tooltip("3�ܰ� ���� ������")]
        //aeternaData.DimenstionSlash_3_Damage;
        //[Tooltip("3�ܰ� ���� ���ǵ�")]
        //aeternaData.DimenstionSlash_3_Speed;
        //[Tooltip("3�ܰ� ���� ����")]
        //aeternaData.DimenstionSlash_3_Healing;
    }
}