using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public Dictionary<string, Transform> audioObjs = new Dictionary<string, Transform>();
    public Dictionary<string, AudioSource> audios = new Dictionary<string, AudioSource>();

    void Awake()
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
    public void PlayAudio(string name, float vol, bool isLoop)
    {
        if(audios.ContainsKey(name))
        {
            if (audios[name].isPlaying && audios[name].loop == isLoop) return;

            audios[name].volume = vol;
            audios[name].loop = isLoop;

            audios[name].Play();
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void PlayAudio(string name, float vol, bool isLoop, bool isOnly) // only�� ��� �ٸ� ��� �Ҹ��� OFF
    {
        if (audios.ContainsKey(name))
        {
            //Debug.Log("audios[name].isPlaying : " + audios[name].isPlaying + " / " + "audios[name].loop == isLoop" + (audios[name].loop==isLoop));
            if (audios[name].isPlaying && audios[name].loop == isLoop) return;

            if (isOnly)
            {
                foreach(var source in audios.Values)
                {
                    source.Stop();
                }
            }

            audios[name].volume = vol;
            audios[name].loop = isLoop;
            audios[name].Play();
            //Debug.Log(name);
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

    [PunRPC]
    public void PlayAudio(string name, float vol, bool isLoop, string parrentName) // �ش� �θ� ������ ������� ��� OFF
    {
        if (audios.ContainsKey(name))
        {
            if (audios[name].isPlaying && audios[name].loop == isLoop) return;
            if (audioObjs[parrentName] == null)
            {
                Debug.LogError("�ش� �θ��� �̸��� �����ϴ� : " + parrentName);
                return;
            }

            
            for(int i = 0; i < audioObjs[parrentName].childCount; i++)
            {
                audios[audioObjs[parrentName].GetChild(i).name].Stop();
            }

            audios[name].volume = vol;
            audios[name].loop = isLoop;
            audios[name].Play();
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
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
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
    public void VolumControl(string name, float vol)
    {
        if (audios.ContainsKey(name))
        {
            audios[name].volume = vol;
        }

        else
        {
            Debug.LogError("�ش� �̸��� ������� �����ϴ� : " + name);
        }
    }

}
