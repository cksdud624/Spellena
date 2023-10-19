using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    int previousWidth;   // ���� ȭ�� �ʺ�
    int previousHeight;  // ���� ȭ�� ����
    int previousNum;     // ���� ��ӹڽ� �ε��� ��ȣ
    public GameObject resolutionCheckPanel;
    public GameObject settingPanel;
    FullScreenMode screenMode;
    public Dropdown resolutionDropdown;
    public Dropdown fullScreenDropdown;
    List<Resolution> resolutions = new List<Resolution>();
    int resolutionNum;
    // Start is called before the first frame update
    void Start()
    {
        InitUI();
        previousWidth = Screen.width;
        previousHeight = Screen.height;
        previousNum = resolutionDropdown.value;
        resolutionCheckPanel.SetActive(false);
        settingPanel.SetActive(false);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel.activeSelf)
            {
                settingPanel.SetActive(false);
            }
            else
            {
                settingPanel.SetActive(true);
            }
        }
    }
    void InitUI()
    {
        // 60������ ���� �ػ󵵸� ����Ʈ�� �߰� 
        /*for(int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }*/
        // ���� ������ ��� �ػ� �߰�
        resolutions.AddRange(Screen.resolutions);
        resolutionDropdown.options.Clear();

        int optionNum = 0;
        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + " x " + item.height + " (" + item.refreshRate + "hz)";
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();
    }
    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;

        Screen.SetResolution(resolutions[resolutionNum].width,
                             resolutions[resolutionNum].height,
                             screenMode);

        resolutionCheckPanel.SetActive(true);
    }

    public void FullScreenBtn(int x)
    {
        if(x == 0)
        {
            screenMode = FullScreenMode.FullScreenWindow;
        }
        else if(x == 1)
        {
            screenMode = FullScreenMode.Windowed;
        }
        else if(x == 2)
        {
            screenMode = FullScreenMode.ExclusiveFullScreen;
        }
        Screen.SetResolution(resolutions[resolutionNum].width,
                             resolutions[resolutionNum].height,
                             screenMode);
    }

    public void onClickOkBtn()
    {
        previousWidth = resolutions[resolutionNum].width;
        previousHeight = resolutions[resolutionNum].height;
        previousNum = resolutionNum;

        resolutionCheckPanel.SetActive(false);
    }

    public void onClickCancelBtn()
    {
        Screen.SetResolution(previousWidth,
                             previousHeight,
                             screenMode);

        resolutionDropdown.value = previousNum;

        resolutionCheckPanel.SetActive(false);
    }
}
