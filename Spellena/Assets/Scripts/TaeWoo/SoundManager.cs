using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public SettingManager settingManager;

    public Dictionary<string, Transform> audioObjs = new Dictionary<string, Transform>();
    public Dictionary<string, AudioSource> audios = new Dictionary<string, AudioSource>();

    void Awake()
    {
        LinkSettingManager();
        InitAudioSettings();
    }

    public void LinkSettingManager()
    {
        GameObject temp = GameObject.Find("SettingManager");

        if(temp==null)
        {
            Debug.LogError("SettingManager�� ã�� �� �����ϴ�.");
            return;
        }

        settingManager = temp.GetComponent<SettingManager>();

        if(settingManager == null)
        {
            Debug.LogError("SettingManager�� Component�� ã�� �� �����ϴ�.");
            return;
        }
    }

    float SetVolData(string type)
    {
        float vol = 0.0f;

        switch(type)
        {
            case "BGM":
                vol = settingManager.bgmVal;
                break;
            case "EffectSound":
                vol = settingManager.effectVal;
                break;
            case "VoiceSound":
                vol = settingManager.voiceVal;
                break;
            default:
                Debug.LogError("SetVolData�� �߸��� �Ű�����");
                break;

        }

        return vol;
    }

    [PunRPC]
    public void InitAudioSettings()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>(true);

        foreach (Transform childTransform in childTransforms)
        {
            audioObjs[childTransform.name] = childTransform;

            if (audioObjs[childTransform.name].GetComponent<AudioSource>())
            {
                audios[childTransform.name] = childTransform.gameObject.GetComponent<AudioSource>();
            }
        }
    }

    [PunRPC]
    public void PlayAudio(string name, float vol, bool isLoop, bool isOnly, string type) // only�� ��� �ٸ� ��� �Ҹ��� OFF
    {
        if (audios.ContainsKey(name))
        {           
            if (audios[name].isPlaying) return;

            if (isOnly)
            {
                foreach(var source in audios.Values)
                {
                    source.Stop();
                }
            }

            audios[name].volume = vol * SetVolData(type) * settingManager.soundVal;
            audios[name].loop = isLoop;
            audios[name].Play();
            Debug.Log("PlayAudio : " + (name));
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void PlayAudio(string name, float vol, bool isLoop, bool isOnly, float delayTime, string type) // only�� ��� �ٸ� ��� �Ҹ��� OFF
    {
        if (audios.ContainsKey(name))
        {
            if (audios[name].isPlaying) return;

            if (isOnly)
            {
                foreach (var source in audios.Values)
                {
                    source.Stop();
                }
            }

            audios[name].volume = vol * SetVolData(type) * settingManager.soundVal;
            audios[name].loop = isLoop;
            audios[name].PlayDelayed(delayTime);
            Debug.Log("PlayAudio : " + (name));
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void PlayAudio(string name, float vol, bool isLoop, string parrentName,string type) // �ش� �θ� ������ ������� ��� OFF
    {
        if (audios.ContainsKey(name))
        {
            if (audios[name].isPlaying) return;
            if (audioObjs[parrentName] == null)
            {
                Debug.LogError("�ش� �θ��� �̸��� �����ϴ� : " + parrentName);
                return;
            }


            for (int i = 0; i < audioObjs[parrentName].childCount; i++)
            {
                audios[audioObjs[parrentName].GetChild(i).name].Stop();
            }

            audios[name].volume = vol * SetVolData(type) * settingManager.soundVal;
            audios[name].loop = isLoop;
            audios[name].Play();
            Debug.Log("PlayAudio : " + (name));
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void PlayRandomAudio(string name, float vol, bool isLoop, bool isOnly, int start, int end,string type) // ���� �ε��� ��ȣ�� ���� ���� ���
    {
        int randomIndex = Random.Range(start, end + 1);

        if (audios.ContainsKey(name + randomIndex))
        {
            if (audios[name + randomIndex].isPlaying) return;

            if (isOnly)
            {
                foreach (var source in audios.Values)
                {
                    source.Stop();
                }
            }

            audios[name + randomIndex].volume = vol * SetVolData(type) * settingManager.soundVal;
            audios[name + randomIndex].loop = isLoop;
            audios[name + randomIndex].Play();
            Debug.Log("PlayAudio : " + (name + randomIndex));
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void PlayRandomAudioOverlap(string name, float vol, bool isLoop, bool isOnly, int start, int end,string type) // ���� �ε��� ��ȣ�� ���� ���� ���
    {
        int randomIndex = Random.Range(start, end + 1);

        if (audios.ContainsKey(name + randomIndex))
        {
            if (audios[name + randomIndex].isPlaying)
            {
                audios[name + randomIndex].Stop();
            }

            if (isOnly)
            {
                foreach (var source in audios.Values)
                {
                    source.Stop();
                }
            }

            audios[name + randomIndex].volume = vol * SetVolData(type) * settingManager.soundVal;
            audios[name + randomIndex].loop = isLoop;
            audios[name + randomIndex].Play();
            Debug.Log("PlayAudio : " + (name + randomIndex));
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void PlayAudioOverlap(string name, float vol, bool isLoop, bool isOnly, string type) // ���������� ��� ����
    {
        if (audios.ContainsKey(name))
        {
            if (audios[name].isPlaying)
            {
                audios[name].Stop();
            }

            if (isOnly)
            {
                foreach (var source in audios.Values)
                {
                    source.Stop();
                }
            }

            audios[name].volume = vol * SetVolData(type) * settingManager.soundVal;
            audios[name].loop = isLoop;
            audios[name].Play();
            Debug.Log("PlayAudio : " + (name));
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void StopAudio(string name)
    {
        if (audios.ContainsKey(name))
        {
            if (!audios[name].isPlaying) return;

            audios[name].Stop();
            Debug.Log("StopAudio : " + (name));
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void StopLocalAudio(string parrentName)
    {
        if (audioObjs[parrentName] == null)
        {
            Debug.LogError("�ش� �θ��� �̸��� �����ϴ� : " + parrentName);
            return;
        }

        for (int i = 0; i < audioObjs[parrentName].childCount; i++)
        {
            audios[audioObjs[parrentName].GetChild(i).name].Stop();
        }

        Debug.Log("StopLocalAudio");
    }


    [PunRPC]
    public void StopAllAudio()
    {
        foreach (var source in audios.Values)
        {
            source.Stop();
        }
    }

    [PunRPC]
    public void VolumControl(string name, float vol,string type)
    {
        if (audios.ContainsKey(name))
        {
             audios[name].volume = vol * SetVolData(type) * settingManager.soundVal;
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

}
