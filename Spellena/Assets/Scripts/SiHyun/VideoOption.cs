using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    int previousWidth;   // ���� ȭ�� �ʺ�
    int previousHeight;  // ���� ȭ�� ����
    int previousNum;     // ���� ��ӹڽ� �ε��� ��ȣ
    int resolutionNum;
    public GameObject resolutionCheckPanel;
    public GameObject settingPanel;
    public GameObject friendsPanel;
    public GameObject playPanel;
    public GameObject gameOffPanel;
    public Button mainEscButton;
    public Button friendEscButton;
    public Button playEscButton;
    public Button settingEscButton;
    public Dropdown resolutionDropdown;
    public Dropdown fullScreenDropdown;
    public Slider soundSlider;
    public Slider bgmSlider;
    public Slider effectSlider;
    public Slider voiceSlider;
    public Slider sensitivitySlider;
    public InputField soundInput;
    public InputField bgmInput;
    public InputField effectInput;
    public InputField voiceInput;
    public InputField sensitivityInput;
    private bool isEditText = false;
    FullScreenMode screenMode;
    List<Resolution> resolutions = new List<Resolution>();

    Button escButton;
    

    // Start is called before the first frame update
    void Start()
    {
        InitUI();
        previousWidth = Screen.width;
        previousHeight = Screen.height;
        previousNum = resolutionDropdown.value;
        resolutionCheckPanel.SetActive(false);
        settingPanel.SetActive(false);
        friendsPanel.SetActive(false);
        Screen.SetResolution(780, 500, FullScreenMode.Windowed);

        //soundValueText.GetComponentInChildren<Button>().onClick.AddListener(() => EditText());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel.activeSelf)
            {
                OnClickEscButton(settingEscButton);
            }
            else if(friendsPanel.activeSelf)
            {
                OnClickEscButton(friendEscButton);
            }
            else if(playPanel.activeSelf)
            {
                OnClickEscButton(playEscButton);
            }
            else if(gameOffPanel.activeSelf)
            {
                gameOffPanel.SetActive(false);
            }
            else 
            {
                OnClickEscButton(mainEscButton);
            }
        }
        if(!isEditText)
        {
            UpdateSliderValueToText();
        }

    }

    public void OnClickEscButton(Button _btn)
    {
        escButton = _btn;
        escButton.onClick.Invoke();
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

    public void FullScreenBtn()
    {
        int _x = fullScreenDropdown.value;
        if (_x == 0)
        {
            screenMode = FullScreenMode.FullScreenWindow;
        }
        else if(_x == 1)
        {
            screenMode = FullScreenMode.Windowed;
        }
        else if(_x == 2)
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

    public void UpdateSliderValueToText()
    {
        float _soundVal = soundSlider.value;
        float _bgmVal = bgmSlider.value;
        float _effectVal = effectSlider.value;
        float _voiceVal = voiceSlider.value;
        float _sensitivityVal = sensitivitySlider.value;

        soundInput.text = string.Format("{0}", _soundVal * 100);
        bgmInput.text = string.Format("{0}", _bgmVal * 100);
        effectInput.text = string.Format("{0}", _effectVal * 100);
        voiceInput.text = string.Format("{0}", _voiceVal * 100);
        sensitivityInput.text = string.Format("{0}", _sensitivityVal * 5);
    }

    /*public void OnInputFieldChanged(string value)
    {
        float _valToSlider = int.Parse(value) / 100;

        soundSlider.value = _valToSlider;
    }*/

}

