using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;

public class BetweenSceneBGM : MonoBehaviour
{
    private static BetweenSceneBGM instance;

    public static BetweenSceneBGM Instance
    {
        get
        {
            return instance;
        }
    }

    private SettingManager settingManager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        LinkSettingManager();
        SetVol();
    }

    private void Update()
    {
        //SetVol();
    }

    void LinkSettingManager()
    {
        GameObject temp = GameObject.Find("SettingManager");

        if (temp == null)
        {
            Debug.LogError("SettingManager�� ã�� �� �����ϴ�.");
            return;
        }

        settingManager = temp.GetComponent<SettingManager>();

        if (settingManager == null)
        {
            Debug.LogError("SettingManager�� Component�� ã�� �� �����ϴ�.");
            return;
        }
    }

    void SetVol()
    {
        GetComponent<AudioSource>().volume = 1.0f * settingManager.soundVal * settingManager.bgmVal;
    }

    //private void OnApplicationFocus(bool focus)
    //{
    //    if(!focus)
    //    {
    //        FirebaseLoginManager.Instance.SignOut();
    //    }

    //}

    //private void OnApplicationQuit()
    //{
    //    // �α� �ƿ�
    //    FirebaseLoginManager.Instance.SignOut();
    //}

}
