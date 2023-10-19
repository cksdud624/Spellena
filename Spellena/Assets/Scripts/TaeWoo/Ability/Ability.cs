using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace Player
{
    public class Ability : MonoBehaviourPunCallbacks,IPunObservable
    {
        public int ID;          // player ID
        public virtual void IsActive() { }
        public virtual void IsDisActive() { }
        public virtual void AddPlayer(Character player) { }
        public virtual void Execution() { }
        public virtual void Execution(ref int time) { }

        public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // �����͸� ������ �κ�
                stream.SendNext(ID);
            }

            else
            {
                // �����͸� �޴� �κ�
                ID = (int)stream.ReceiveNext();
            }
        }
    }
}