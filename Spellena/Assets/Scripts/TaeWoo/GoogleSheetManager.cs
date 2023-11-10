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
    private object[,] dividData;

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

        dividData = new object[rowSize, columSize];

        // ���� ���̷� ���η� ������.
        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            // ���� ���̷� ���η� ������.
            for (int j = 0; j < columSize; j++)
            {
                dividData[i, j] = column[j];
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

    }
}