using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoogleSheetData", menuName = "ScriptableObject/GoogleSheetData")]
public class GoogleSheetData : ScriptableObject
{
    public SheetData[] gooleSheets;

    [System.Serializable]
    public struct SheetData
    {
        [Header("�̸�")]
        public string name;
        [Header("���� ��Ʈ �ּ�(edit? ���� ������ ���ڿ� ����)")]
        public string address;// = "https://docs.google.com/spreadsheets/d/1nRFZ43p-eW-c1i4hLp7vaqQt6ms10xjNhDdVvyk1HIQ/";
        [Header("������ ��Ʈ ���� ��")]
        public string range_1;// = "A2";
        [Header("������ ��Ʈ �� ��")]
        public string range_2;// = "C";
    }

    // export?format=tsv => tsv ���� �������� ����
    [HideInInspector]
    public string exportFormattsv = "export?format=tsv";
    // & range = A2:C3 => ������ ���� �� ����
    [HideInInspector]
    public string andRange = "&range=";

}

