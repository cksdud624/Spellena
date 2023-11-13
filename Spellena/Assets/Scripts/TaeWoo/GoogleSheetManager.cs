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
            default:
                break;
        }
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
        aeternaData.jumpHeight = float.Parse(dividData[36, 0]);
        aeternaData.headShotRatio = float.Parse(dividData[38, 0]);

        //[Tooltip("�⺻ ���� �� Ÿ��")]
        aeternaData.basicAttackTime = float.Parse(dividData[8, 1]);
        //[Tooltip("��ų1 ��Ż �� Ÿ��")]
        aeternaData.skill1DoorCoolTime = float.Parse(dividData[8, 2]);
        //[Tooltip("��ų2 �� Ÿ��")]
        aeternaData.skill2CoolTime = float.Parse(dividData[8, 3]);
        //[Tooltip("��ų3 �� Ÿ��")]
        aeternaData.skill3CoolTime = float.Parse(dividData[8, 5]);

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

#endif