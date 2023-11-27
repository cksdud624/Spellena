using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class GoogleSheetManager : EditorWindow
{
    private GoogleSheetData googleSheetData;

    private AeternaData aeternaData;
    private ElementalOrderData elementalOrderData;

    // URL
    private string URL;

    // ���ڿ��� ���� 2���� �迭
    private string[,] dividData;

    [MenuItem("Custom/GoogleSpreadSheetManager")]
    public static void ShowWindow()
    {
        EditorWindow wnd = GetWindow<GoogleSheetManager>();
        wnd.titleContent = new GUIContent("GoogleSheetManager");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Sheet Data", EditorStyles.boldLabel);
        googleSheetData = (GoogleSheetData)EditorGUILayout.ObjectField("SheetData", googleSheetData, typeof(GoogleSheetData), true);
        EditorGUILayout.LabelField("");
        aeternaData = (AeternaData)EditorGUILayout.ObjectField("AeternaData", aeternaData, typeof(AeternaData), true);
        elementalOrderData = (ElementalOrderData)EditorGUILayout.ObjectField("ElementalOrderData", elementalOrderData, typeof(ElementalOrderData), true);

        if (GUILayout.Button("������ �ҷ����� �����ϱ�"))
        {
            InitData();
            Debug.Log("������ �ҷ���");
        }

    }

    void InitData()
    {

        for (int i = 0; i < googleSheetData.gooleSheets.Length; i++)
        {
            URL = googleSheetData.gooleSheets[i].address + googleSheetData.exportFormattsv + googleSheetData.andRange
                + googleSheetData.gooleSheets[i].range_1 + ":" + googleSheetData.gooleSheets[i].range_2;

            UnityWebRequest www = UnityWebRequest.Get(URL);
            www.SendWebRequest();
            while (!www.isDone)
            { 
                //Debug.Log("������ �������� ��...");                                
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("������ �������� ����: " + www.error);
                return;
            }

            string Data = www.downloadHandler.text;
            Debug.Log("������ �������� ����");

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
        switch (index)
        {
            case 0:
                GiveAeternaData();
                break;
            case 1:
                GiveElementalOrderData();
                break;
            default:
                break;
        }
    }

    void GiveElementalOrderData()
    {
        elementalOrderData.hp = int.Parse(dividData[28, 0]);
        elementalOrderData.moveSpeed = float.Parse(dividData[29, 0]);
        elementalOrderData.backSpeed = float.Parse(dividData[30, 0]);
        elementalOrderData.sideSpeed = float.Parse(dividData[31, 0]);
        elementalOrderData.runSpeedRatio = float.Parse(dividData[32, 0]);
        elementalOrderData.sitSpeed = float.Parse(dividData[33, 0]);
        elementalOrderData.sitBackSpeed = float.Parse(dividData[34, 0]);
        elementalOrderData.sitSideSpeed = float.Parse(dividData[35, 0]);
        elementalOrderData.jumpHeight = float.Parse(dividData[36, 0]);
        elementalOrderData.headShotRatio = float.Parse(dividData[38, 0]);
        //��ų 1
        elementalOrderData.ragnaEdgeFloorDamage = float.Parse(dividData[8, 1]);
        elementalOrderData.ragnaEdgeCylinderDamage = float.Parse(dividData[8, 2]);
        elementalOrderData.rangaEdgeCoolDownTime = float.Parse(dividData[9, 2]);
        elementalOrderData.ragnaEdgeCastingTime = float.Parse(dividData[10, 1]);
        elementalOrderData.ragnaEdgeFloorLifeTime = float.Parse(dividData[16, 1]);
        elementalOrderData.ragnaEdgeCylinderLifeTime = float.Parse(dividData[16, 2]);
        elementalOrderData.rangaEdgeCylinderDebuff = dividData[19, 2];
        //��ų 2
        elementalOrderData.burstFlareBullet = int.Parse(dividData[4, 9]);
        elementalOrderData.burstFlareDamage = float.Parse(dividData[8, 9]);
        elementalOrderData.burstFlareCoolDownTime = float.Parse(dividData[9, 9]);
        elementalOrderData.burstFlareCastingTime = float.Parse(dividData[10, 9]);
        elementalOrderData.burstFlareLifeTime = float.Parse(dividData[16, 9]);
        //��ų 3
        elementalOrderData.gaiaTiedDamage = float.Parse(dividData[8, 3]);
        elementalOrderData.gaiaTiedCoolDownTime = float.Parse(dividData[9, 8]);
        elementalOrderData.gaiaTiedCastingTime = float.Parse(dividData[10, 3]);
        elementalOrderData.gaiaTiedMaxDistace = float.Parse(dividData[12, 3]);
        elementalOrderData.gaiaTiedLifeTime = new float[6];
        for (int i = 0; i < 6; i++)
        {
            elementalOrderData.gaiaTiedLifeTime[i] = float.Parse(dividData[16, 3 + i]);
        }
        //��ų 4
        elementalOrderData.meteorStrikeDamage = float.Parse(dividData[8, 10]);
        elementalOrderData.meteorStrikeCoolDownTime = float.Parse(dividData[9, 10]);
        elementalOrderData.meteorStrikeCastingTime = float.Parse(dividData[10, 10]);
        elementalOrderData.meteorStrikeMaxDistance = float.Parse(dividData[12, 10]);
        elementalOrderData.meteorStrikeLifeTime = float.Parse(dividData[16, 10]);
        //��ų 5
        elementalOrderData.terraBreakDamageFirst = float.Parse(dividData[8, 11]);
        elementalOrderData.terraBreakDamage = float.Parse(dividData[8, 12]);
        elementalOrderData.terraBreakCoolDownTime = float.Parse(dividData[9, 12]);
        elementalOrderData.terraBreakCastingTime = float.Parse(dividData[10, 12]);
        elementalOrderData.terraBreakMaxDistance = float.Parse(dividData[12, 11]);
        elementalOrderData.terraBreakLifeTimeFirst = float.Parse(dividData[16, 11]);
        elementalOrderData.terraBreakLifeTime = float.Parse(dividData[16, 12]);
        //��ų 6
        elementalOrderData.eterialStormDamage = float.Parse(dividData[8, 13]);
        elementalOrderData.eterialStormCoolDownTime = float.Parse(dividData[9, 13]);
        elementalOrderData.eterialStormCastingTime = float.Parse(dividData[10, 13]);
        elementalOrderData.eterialStormLifeTime = float.Parse(dividData[16, 13]);
    }

    void GiveAeternaData()
    {
        aeternaData.hp = int.Parse(dividData[28, 0]);
        aeternaData.moveSpeed = float.Parse(dividData[29, 0]);
        aeternaData.backSpeed = float.Parse(dividData[30, 0]);
        aeternaData.sideSpeed = float.Parse(dividData[31, 0]);
        aeternaData.runSpeedRatio = float.Parse(dividData[32, 0]);
        aeternaData.sitSpeed = float.Parse(dividData[33, 0]);
        aeternaData.sitBackSpeed = float.Parse(dividData[34, 0]);
        aeternaData.sitSideSpeed = float.Parse(dividData[35, 0]);
        aeternaData.jumpHeight = float.Parse(dividData[36, 0]);
        aeternaData.headShotRatio = float.Parse(dividData[39, 0]);

        //[Tooltip("�⺻ ���� �� Ÿ��")]
        aeternaData.basicAttackTime = float.Parse(dividData[9, 1]);
        //[Tooltip("��ų1 ��Ż �� Ÿ��")]
        aeternaData.skill1DoorCoolTime = float.Parse(dividData[9, 2]);
        //[Tooltip("��ų2 �� Ÿ��")]
        aeternaData.skill2CoolTime = float.Parse(dividData[9, 3]);
        //[Tooltip("��ų3 �� Ÿ��")]
        aeternaData.skill3CoolTime = float.Parse(dividData[9, 5]);

        //[Tooltip("�⺻ ���� ����")]
        aeternaData.DimenstionSlash_0_lifeTime = float.Parse(dividData[47, 12]) / float.Parse(dividData[46, 12]);
        //[Tooltip("�⺻ ���� ������")]
        aeternaData.DimenstionSlash_0_Damage = int.Parse(dividData[8, 6]);
        //[Tooltip("�⺻ ���� ���ǵ�")]
        aeternaData.DimenstionSlash_0_Speed = (int)(float.Parse(dividData[46, 12]));
        //[Tooltip("�⺻ ���� ����")]
        aeternaData.DimenstionSlash_0_Healing = -int.Parse(dividData[8, 9]);

        //[Header("���׸��� ��ų1 ������")]
        //[Tooltip("��ų1 ��Ż ����")]
        aeternaData.skill1Time = 5;
        //[Tooltip("��ų1 ��Ż ��ȯ �ִ� ��Ÿ�")]
        aeternaData.skill1DoorSpawnMaxRange = 8;
        //[Tooltip("��ų1 ��Ż ���� ����")]
        aeternaData.skill1DoorRange = 3;
        //[Tooltip("��ų1 ��Ż �߽� ��")]
        aeternaData.skill1InnerForce = float.Parse(dividData[43, 13]);

        //[Header("���׸��� ��ų2 ������")]
        //[Tooltip("��ų2 ���� �ð�")]
        aeternaData.skill2DurationTime = 10;
        //[Tooltip("��ų2 ����ü ������ �ִ� �ð�")]
        aeternaData.skill2HoldTime = 10;

        //[Header("���׸��� ��ų3 ������")]
        //[Tooltip("��ų3 ���� �ð�")]
        aeternaData.skill3DurationTime  = 10;

        //[Header("���׸��� ��ų4 ������")]
        //[Tooltip("��ų4 �ñر� �ڽ�Ʈ")]
        aeternaData.skill4Cost = int.Parse(dividData[12, 6]);
        //[Tooltip("��ų4 ���� �ð�")]
        aeternaData.skill4DurationTime = 12;

        //[Header("��ų4 1�ܰ� ������")]
        //[Tooltip("��ų4 1�ܰ� ���� �ð�")]
        aeternaData.skill4Phase1Time = float.Parse(dividData[11, 6]);
        //[Tooltip("1�ܰ� ���� ����")]
        aeternaData.DimenstionSlash_1_lifeTime = float.Parse(dividData[47, 14]) / int.Parse(dividData[46, 14]);
        //[Tooltip("1�ܰ� ���� ������")]
        aeternaData.DimenstionSlash_1_Damage = int.Parse(dividData[8, 6]);
        //[Tooltip("1�ܰ� ���� ���ǵ�")]
        aeternaData.DimenstionSlash_1_Speed = (int)(float.Parse(dividData[46, 14]));
        //[Tooltip("1�ܰ� ���� ����")]
        aeternaData.DimenstionSlash_1_Healing = -int.Parse(dividData[8, 9]);

        //[Header("��ų4 2�ܰ� ������")]
        //[Tooltip("��ų4 2�ܰ� ���� �ð�")]
        aeternaData.skill4Phase2Time = float.Parse(dividData[11, 7]);
        //[Tooltip("2�ܰ� ���� ����")]
        aeternaData.DimenstionSlash_2_lifeTime = float.Parse(dividData[47, 15]) / int.Parse(dividData[46, 15]);
        //[Tooltip("2�ܰ� ���� ������")]
        aeternaData.DimenstionSlash_2_Damage = int.Parse(dividData[8, 7]);
        //[Tooltip("2�ܰ� ���� ���ǵ�")]
        aeternaData.DimenstionSlash_2_Speed = (int)(float.Parse(dividData[46, 15]));
        //[Tooltip("2�ܰ� ���� ����")]
        aeternaData.DimenstionSlash_2_Healing = -int.Parse(dividData[8, 10]);

        //[Header("��ų4 3�ܰ� ������")]
        //[Tooltip("��ų4 3�ܰ� ���� �ð�")]
        aeternaData.skill4Phase3Time = float.Parse(dividData[11, 8]);
        //[Tooltip("3�ܰ� ���� ����")]
        aeternaData.DimenstionSlash_3_lifeTime = float.Parse(dividData[47, 16]) / int.Parse(dividData[46, 16]);
        //[Tooltip("3�ܰ� ���� ������")]
        aeternaData.DimenstionSlash_3_Damage = int.Parse(dividData[8, 8]);
        //[Tooltip("3�ܰ� ���� ���ǵ�")]
        aeternaData.DimenstionSlash_3_Speed = (int)(float.Parse(dividData[46, 16]));
        //[Tooltip("3�ܰ� ���� ����")]
        aeternaData.DimenstionSlash_3_Healing = -int.Parse(dividData[8, 11]);
    }
}

#endif