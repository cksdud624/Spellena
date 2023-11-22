using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public GameObject mapName;
    public GameObject helpBackImage;
    public GameObject helpText;
    public GameObject helpWords;
    public GameObject loadingSign;

    private Text mapNameText;
    private Image helpBackImageComponent;
    private Text helpWordsText;
    private RectTransform loadingSignRectTransform;

    private float timer = 0.0f;
    private float helpBackImageOpenSpeed = 1.0f;
    private float loadingSignRotateSpeed = 3.5f;
    private float loadingSignRotateFrequency = 0.5f;

    void Start()
    {
        mapNameText = mapName.GetComponent<Text>();
        helpBackImageComponent = helpBackImage.GetComponent<Image>();
        helpWordsText = helpWords.GetComponent<Text>();
        loadingSignRectTransform = loadingSign.GetComponent<RectTransform>();

        mapNameText.text = "������ ����";
        helpWordsText.text = "������ õ�� ������ Ȱ���ϸ� ü���� ȸ����ų �� �ֽ��ϴ�!";

        StartCoroutine(SlideHelpBackImage());
    }

    IEnumerator SlideHelpBackImage()
    {
        while(helpBackImageComponent.fillAmount < 1.0f)
        {
            helpBackImageComponent.fillAmount += helpBackImageOpenSpeed * Time.deltaTime;
            yield return null;
        }

        helpText.SetActive(true);
        helpWords.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        LoadingSignRotate();
    }

    void LoadingSignRotate()
    {
        loadingSignRectTransform.Rotate(0, 0, loadingSignRotateSpeed * Mathf.Abs(Mathf.Sin(loadingSignRotateFrequency * timer)));
    }
}
