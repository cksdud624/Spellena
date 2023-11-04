using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.UI;

public class GlobalUI : MonoBehaviourPunCallbacks
{
    public GameObject inGameUI;
    public GameObject etcUI;

    Dictionary<string, GameObject> UIObjects = new Dictionary<string, GameObject>();

    public struct OccupyingTeam
    {
        public string name;
        public float rate;
    }

    public struct Occupation
    {
        public float rate;
    }

    // �Ͻ����� ���� ���� string ������
    public string gameStateString;
    // ��ü Ÿ�̸�
    public float globalTimerUI;
    // �߰��ð� Ÿ�̸�
    public float roundEndTimerUI;
    //�߰� �ð�
    public float roundEndTimeUI = 5f;
    // A���� ���ɵ�
    public Occupation occupyingAUI;
    // B���� ���ɵ�
    public Occupation occupyingBUI;
    // ���� ������ ��
    public OccupyingTeam occupyingTeamUI;

    public Photon.Realtime.Player[] allPlayers;
    public List<GameObject> playersA = new List<GameObject>(); // Red
    public List<GameObject> playersB = new List<GameObject>(); // Blue
    
    void Start()
    {
        ConnectUI();
        InitUI();
    }

    void ConnectUI()
    {
        UIObjects["inGameUI"] = inGameUI;
        UIObjects["etcUI"] = etcUI;

        UIObjects["unContested"] = FindObject(inGameUI, "UnContested");
        UIObjects["captured_Red"] = FindObject(inGameUI, "RedCapture");
        UIObjects["captured_Blue"] = FindObject(inGameUI, "BlueCapture");
        UIObjects["redFillCircle"] = FindObject(inGameUI, "RedOutline");
        UIObjects["blueFillCircle"] = FindObject(inGameUI, "BlueOutline");
        UIObjects["redPayload"] = FindObject(inGameUI, "RedPayload_Filled");
        UIObjects["bluePayload"] = FindObject(inGameUI, "BluePayload_Filled");
        UIObjects["redExtraUI"] = FindObject(inGameUI, "RedCTF");
        UIObjects["blueExtraUI"] = FindObject(inGameUI, "BlueCTF");
        UIObjects["redPercentage"] = FindObject(inGameUI, "RedOccupyingPercent");
        UIObjects["bluePercentage"] = FindObject(inGameUI, "BlueOccupyingPercent");
        UIObjects["extraObj"] = FindObject(inGameUI, "Extra");
        UIObjects["extraTimer"] = FindObject(inGameUI, "ExtaTimer");
        UIObjects["redExtraObj"] = FindObject(inGameUI, "Red");
        UIObjects["blueExtraObj"] = FindObject(inGameUI, "Blue");
        UIObjects["redCTF"] = FindObject(inGameUI, "RedCTF_Filled");
        UIObjects["blueCTF"] = FindObject(inGameUI, "BlueCTF_Filled");
        UIObjects["redFirstPoint"] = FindObject(inGameUI, "RedFirstPoint");
        UIObjects["redSecondPoint"] = FindObject(inGameUI, "RedSecondPoint");
        UIObjects["blueFirstPoint"] = FindObject(inGameUI, "BlueFirstPoint");
        UIObjects["blueSecondPoint"] = FindObject(inGameUI, "BlueSecondPoint");

        UIObjects["gameStateUI"] = FindObject(etcUI, "GameState");
        UIObjects["timer"] = FindObject(etcUI, "Timer");
    }

    void InitUI()
    {
        UIObjects["inGameUI"].SetActive(false);
        UIObjects["etcUI"].SetActive(true);
    }

    void Update()
    {
        UIObjects["timer"].GetComponent<Text>().text = ((int)globalTimerUI + 1).ToString();
        UIObjects["gameStateUI"].GetComponent<Text>().text = gameStateString;
        UIObjects["redPayload"].GetComponent<Image>().fillAmount = occupyingAUI.rate * 0.01f;
        UIObjects["bluePayload"].GetComponent<Image>().fillAmount = occupyingBUI.rate * 0.01f;
        UIObjects["redPercentage"].GetComponent<Text>().text = string.Format((int)occupyingAUI.rate + "%");
        UIObjects["bluePercentage"].GetComponent<Text>().text = string.Format((int)occupyingBUI.rate + "%");
        UIObjects["extraTimer"].GetComponent<Text>().text = string.Format("{0:F2}", roundEndTimerUI);
        UIObjects["redFillCircle"].GetComponent<Image>().fillAmount = occupyingTeamUI.rate * 0.01f;
        UIObjects["blueFillCircle"].GetComponent<Image>().fillAmount = occupyingTeamUI.rate * 0.01f;
        UIObjects["redCTF"].GetComponent<Image>().fillAmount = roundEndTimerUI / roundEndTimeUI;
        UIObjects["blueCTF"].GetComponent<Image>().fillAmount = roundEndTimerUI / roundEndTimeUI;
    }

    [PunRPC]
    public void ActiveUI(string uiName, bool isActive)
    {
        UIObjects[uiName].SetActive(isActive);
    }

    GameObject FindObject(GameObject parrent, string name)
    {
        GameObject foundObject = null;
        Transform[] array = parrent.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in array)
        {
            if (transform.name == name)
            {
                foundObject = transform.gameObject;
                break; // ã������ ������ ����.
            }
        }

        if (foundObject == null)
        {
            Debug.LogError("�ش� �̸��� ���� ������Ʈ�� ã�� ���߽��ϴ� : " + name);
        }

        return foundObject;
    }
}
