using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCenterDataType
{
    public struct PlayerStat
    {
        public int index;
        public string name;
        public Photon.Realtime.Player player;
        public string character;
        public int characterViewID;
        public string team;

        public int killCount;
        public int deadCount;
        public int assistCount;
        public int ultimateCount;

        public int totalDamage;
        public int totalHeal;
        public bool isAlive;

        public float angleStatueCoolTime;
        public string killerName;

        public float respawnTime;
        public Vector3 spawnPoint;

        public string damagePart;
        public Vector3 damageDirection;
        public float damageForce;

        public int playerAssistViewID;
    }

    public struct OccupyingTeam
    {
        public string name;
        public float rate;
    }

    public struct Occupation
    {
        public float rate;
    }

    public struct GlobalTimer
    {
        // ��ü Ÿ�̸�
        public float globalTime;
        // ��ǥ ��ü Ÿ�̸� ��
        public float globalDesiredTime;
    }

    public struct LoadingTimer
    {
        //[Tooltip("��, ĳ���� �ε� Ÿ��")]
        public float loadingTime;
    }

    public struct CharacterSelectTimer
    {
        //[Tooltip("ĳ���� ���� Ÿ��")]
        public float characterSelectTime;
    }

    public struct PlayerList
    {
        public List<PlayerStat> playersA; // Red
        public List<PlayerStat> playersB; // Blue 
    }

    public struct RoundData
    {
        // ���� ����
        public int roundCount_A;
        public int roundCount_B;
    }

    public struct DuringRoundData
    {
        // ���� ���� �����ϴ� ����
        public int teamAOccupying;
        public int teamBOccupying;

        // ���� ��ȯ Ÿ�̸�
        public float occupyingReturnTimer;
        // �߰��ð� Ÿ�̸�
        public float roundEndTimer;
        // ���� �������� ��
        public string currentOccupationTeam;

        // A���� ���ɵ�
        public Occupation occupyingA;
        // B���� ���ɵ�
        public Occupation occupyingB;
        // ���� ������ ��
        public OccupyingTeam occupyingTeam;

        // �� �̸�
        public string teamA;
        public string teamB;

        // �÷��̾� ��Ȱ ť
        public Queue<PlayerStat> playerRespawnQue;

        public BitFlag flag;
    }
    public struct GameReadyStandardData
    {
        //[Tooltip("���� �غ� �ð�")]
        public float readyTime;
    }


    public struct DuringRoundStandardData
    {
        //[Tooltip("�÷��̾� ������ Ÿ��")]
        public float playerRespawnTime;
        //[Tooltip("��ý�Ʈ Ÿ��")]
        public float assistTime;

        //[Tooltip("�κ��� ��Ÿ��")]
        public float angelStatueCoolTime;
        //[Tooltip("�κ��� �ʴ� ü�� ������")]
        public int angelStatueHpPerTime;
        //[Tooltip("�κ��� ȿ�� ���� �ð�")]
        public int angelStatueContinueTime;

        //[Tooltip("���� ��ȯ �� �Դ� ����")]
        public float occupyingGaugeRate;
        //[Tooltip("���� ��ȯ�ϴ� �ð�")]
        public float occupyingReturnTime;
        //[Tooltip("���� % �Դ� ����")]
        public float occupyingRate;
        //[Tooltip("�߰��ð��� �߻��ϴ� ���� ������")]
        public float occupyingComplete;
        //[Tooltip("�߰� �ð�")]
        public float roundEndTime;
    }

    public struct RoundEndStandardData
    {
        //[Tooltip("���� ��� Ȯ�� �ð�")]
        public float roundEndResultTime;
        //[Tooltip("���� ���� ����")]
        public int roundEndNumber;
    }

    public enum GameState
    {
        InTheLobby,
        InTheRoom,
        LoadingScene,
        CharacterSelect,
        GameReady,
        DuringRound,
        RoundEnd,
        GameResult
    }

    public enum StateCheck
    {
        OccupyBarCountOnce = 0x0001,
        isFighting = 0x0002
    }

    public struct BitFlag
    {
        public uint flag;
        public BitFlag(uint _flag)
        {
            this.flag = _flag;
        }

        public void BitAdd(uint _num)
        {
            this.flag |= _num;
        }

        public void BitSub(uint _num)
        {
            uint temp = this.flag & _num;
            this.flag ^= temp;
        }

        public bool BitCompare(uint _num)
        {
            return ((this.flag ^ _num) == 0);
        }

    }
}
