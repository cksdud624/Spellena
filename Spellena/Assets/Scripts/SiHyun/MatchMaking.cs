using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;

public class MatchMaking : MonoBehaviourPunCallbacks
{
    public Text currentPlayerCount;
    public Text playerNameText;

    private FirebaseUser user;

    public GameObject matchButton;

    private void Awake()
    { 
        PhotonNetwork.AutomaticallySyncScene = true;


        // OnConnectedToMaster �̺�Ʈ�� ���� ������ ���
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        user = FirebaseLoginManager.Instance.GetUser();
        GetUserName();

        DatabaseReference lobbyMasterRef =
            FirebaseLoginManager.Instance.GetReference().Child("users").Child(user.UserId).Child("isLobbyMaster?");
        lobbyMasterRef.ValueChanged += (sender, args) =>
        {
            Debug.Log("�̺�Ʈ �ڵ鷯 ȣ��");
            if (args.DatabaseError != null)
            {
                Debug.Log("�̺�Ʈ �ڵ鷯 ȣ�� ����");
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            try
            {
                Debug.Log("�̺�Ʈ �ڵ鷯 ȣ�� ����");

                if (args.Snapshot != null)
                {
                    Debug.Log("������ ����");

                    if ((bool)args.Snapshot.Value)
                    {
                        Debug.Log("����");
                        matchButton.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("���� �ƴ�");
                        matchButton.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("������ ����");
                }

            }
            catch (Exception ex)
            {
                Debug.LogError("Exception in ValueChanged event handler: " + ex.Message);
            }
        };
        Button _matchButton = matchButton.GetComponent<Button>();
        _matchButton.onClick.AddListener(StartMatchMaking);
    }

    async void GetUserName()
    {
        string _userName = await FirebaseLoginManager.Instance.ReadUserInfo(user.UserId);
        if(!string.IsNullOrEmpty(_userName))
        {
            PhotonNetwork.LocalPlayer.NickName = _userName;
            playerNameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }

    private void StartMatchMaking()
    {
        PartyMatchMaking();
    }

    private async void PartyMatchMaking()
    {
        await JoinRandomOrCreateRoomAsync();

    }

    private async Task JoinRandomOrCreateRoomAsync()
    {
        List<string> _partyMembers = await FirebaseLoginManager.Instance.GetFriendsList(user.UserId);
        _partyMembers.Insert(0, user.UserId);
        string[] _partys = _partyMembers.ToArray();

        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable(),
            expectedMaxPlayers: 10,
            matchingType: MatchmakingMode.FillRoom,
            typedLobby: null,
            sqlLobbyFilter: "",
            expectedUsers: _partys);
    }

    public void CancelMatching()
    {
        print("��Ī ���.");

        print("�� ����.");
        PhotonNetwork.LeaveRoom();
    }

    private void UpdatePlayerCounts()
    {
        currentPlayerCount.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void OnClickLoad()
    {
        SceneManager.LoadScene("SiHyun RoomLobby Test");
    }

    public void OnClickGameOff()
    {
        FirebaseLoginManager.Instance.SignOut();
        Application.Quit();
    }

    //���� �ݹ� �Լ�
    #region 

    // PhotonNetwork ������ �Ϸ�Ǹ� ȣ��Ǵ� �ݹ�
    public override void OnConnectedToMaster()
    {
        print("���� ���� �Ϸ�.");
        FirebaseLoginManager.Instance.SetPhotonId(FirebaseLoginManager.Instance.GetUser().UserId, PhotonNetwork.LocalPlayer.UserId);
        Debug.Log(PhotonNetwork.LocalPlayer.UserId);
    }

    public override void OnJoinedRoom()
    {
        print("�� ���� �Ϸ�.");
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}�� �ο��� {PhotonNetwork.CurrentRoom.MaxPlayers} ��Ī ��ٸ��� ��.");
        UpdatePlayerCounts();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"�÷��̾� {newPlayer.NickName} �� ����.");
        UpdatePlayerCounts();

        if(PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.LoadLevel("Game");
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"�÷��̾� {otherPlayer.NickName} �� ����.");
        UpdatePlayerCounts();
    }

    #endregion
}
