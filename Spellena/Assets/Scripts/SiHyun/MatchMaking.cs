using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class MatchMaking : MonoBehaviourPunCallbacks
{
    public GameObject loadingPanel;
    public Text currentPlayerCount;
    public Text playerNameText;
    private LoadBalancingClient loadBalancingClient;
    EnterRoomParams enterRoomParams = new EnterRoomParams();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        loadingPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("���� ���� �õ�");
        PhotonNetwork.ConnectUsingSettings();
        GetUserName();
    }

    async void GetUserName()
    {
        var _user = FirebaseLoginManager.Instance.GetUser();
        playerNameText.text = await FirebaseLoginManager.Instance.ReadUserInfo(_user.UserId);
    }



    public void JoinRandomOrCreateRoom()
    {
        Debug.Log("���� ��Ī ����.");
        PhotonNetwork.LocalPlayer.NickName = FirebaseLoginManager.Instance.GetUser().DisplayName;

        byte _maxPlayers = 10;

        RoomOptions _roomOptions = new RoomOptions();
        _roomOptions.MaxPlayers = _maxPlayers;

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable(),
            expectedMaxPlayers: _maxPlayers, // ������ ���� ����.
            roomOptions: _roomOptions);      // ������ ���� ����.
    }

    public void CancelMatching()
    {
        print("��Ī ���.");
        loadingPanel.SetActive(false);

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

    public override void OnConnectedToMaster()
    {
        print("���� ���� �Ϸ�.");
    }

    public override void OnJoinedRoom()
    {
        print("�� ���� �Ϸ�.");
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}�� �ο��� {PhotonNetwork.CurrentRoom.MaxPlayers} ��Ī ��ٸ��� ��.");
        UpdatePlayerCounts();

        loadingPanel.SetActive(true);
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
