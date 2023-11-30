using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterSoundManager : MonoBehaviourPunCallbacks
{
    public List<AudioData> actAudios = new List<AudioData>();
    private Dictionary<string, AudioClip> actAudioDatas = new Dictionary<string, AudioClip>();

    public List<AudioData> speakAudios = new List<AudioData>();
    private Dictionary<string, AudioClip> speakAudioDatas = new Dictionary<string, AudioClip>();

    public List<AudioData> skillAudios = new List<AudioData>();
    private Dictionary<string, AudioClip> skillAudioDatas = new Dictionary<string, AudioClip>();

    [HideInInspector]
    public AudioSource actAudioSource;
    [HideInInspector]
    public AudioSource speakAudioSource;
    [HideInInspector]
    public AudioSource skillAudioSource;

    [System.Serializable]
    public struct AudioData
    {
        public string name;
        public AudioClip clip;
    }

    void Start()
    {
        LinkDatas();
    }

    void LinkDatas()
    {
        actAudioSource = GameCenterTest.FindObject(gameObject, "CharacterActSound").GetComponent<AudioSource>();
        speakAudioSource = GameCenterTest.FindObject(gameObject, "CharacterSpeakSound").GetComponent<AudioSource>();
        skillAudioSource = GameCenterTest.FindObject(gameObject, "CharacterSkillSound").GetComponent<AudioSource>();

        foreach (AudioData data in actAudios)
        {
            actAudioDatas.Add(data.name, data.clip);
        }

        foreach (AudioData data in speakAudios)
        {
            speakAudioDatas.Add(data.name, data.clip);
        }

        foreach (AudioData data in skillAudios)
        {
            skillAudioDatas.Add(data.name, data.clip);
        }
    }

    // �� ����� ���� �÷��� �� �� �ִ�
    // ���� ���ǿ� ������� �� �ٽ� ����ϸ� return
    // stop�ϸ� �ش� ����� �ҽ��� ����


    [PunRPC]
    public void ControlActAudio(string key, float vol, bool isLoop, bool isPlay)
    {
        if (!actAudioDatas.ContainsKey(key)) return;

        if (isPlay)
        {
            if (actAudioSource.clip == actAudioDatas[key] && actAudioSource.isPlaying)
            {
                Debug.Log("���� ���� ���");
                return;
            }

            actAudioSource.clip = actAudioDatas[key];
            actAudioSource.volume = vol;
            actAudioSource.loop = isLoop;

            actAudioSource.Play();
        }

        else
        {
            actAudioSource.Stop();   
        }

    }

    [PunRPC]
    public void ControlSpeakAudio(string key, float vol, bool isLoop, bool isPlay)
    {
        if (!speakAudioDatas.ContainsKey(key)) return;

        if (isPlay)
        {
            if (speakAudioSource.clip == speakAudioDatas[key] && speakAudioSource.isPlaying)
            {
                Debug.Log("���� ���� ���");
                return;
            }

            speakAudioSource.clip = speakAudioDatas[key];
            speakAudioSource.volume = vol;
            speakAudioSource.loop = isLoop;

            speakAudioSource.Play();
        }

        else
        {
            actAudioSource.Stop();     
        }
    }

    [PunRPC]
    public void ControlSkillAudio(string key, float vol, bool isLoop, bool isPlay)
    {
        if (!skillAudioDatas.ContainsKey(key)) return;    

        if (isPlay)
        {
            if (skillAudioSource.clip == skillAudioDatas[key] && skillAudioSource.isPlaying)
            {
                Debug.Log("���� ���� ���");
                return;
            }

            skillAudioSource.clip = skillAudioDatas[key];
            skillAudioSource.volume = vol;
            skillAudioSource.loop = isLoop;

            skillAudioSource.Play();
        }

        else
        {
            actAudioSource.Stop();
        }
    }

}
