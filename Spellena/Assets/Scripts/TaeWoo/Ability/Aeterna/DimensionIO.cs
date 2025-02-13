using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Player
{
    public class DimensionIO : Ability
    {
        private Aeterna Player;
        private Animator animator;
        private GameObject sword;
        private GameObject swordTrigger;

        [HideInInspector]
        public SpawnObjectType enemyProjectileType;

        [HideInInspector]
        public object[] enemyObjectData;

        public override void AddPlayer(Character player)
        {
            Player = (Aeterna)player;
            animator = Player.GetComponent<Animator>();
            sword = Player.DimensionSword;
            swordTrigger = Player.swordTrigger;
        }

        public override void IsDisActive()
        { 
            if(Player.playerActionDatas[(int)PlayerActionState.Skill2].isExecuting && Player.skill2Phase == 1)
            {
                sword.GetComponent<PhotonView>().RPC("ActivateParticle", RpcTarget.AllBuffered, 2, false);
                Player.dimensionSwordForMe[2].SetActive(false);
                Player.skillTimer[2] = 0.1f;
            }
        }

        public override void Execution(ref int phase)
        {
            switch (phase)
            {
                case 1:
                    OnDuration();
                    break;
                case 2:
                    OnHoldShoot();
                    break;
            }
        }

        private void OnDuration()
        {
            Player.BasicAttackTrigger();
            Player.GetComponent<PhotonView>().RPC("BasicAttackTrigger", RpcTarget.AllBufferedViaServer);
            StartCoroutine(ActiveTirgger());
            StartCoroutine(EndAttack());
        }

        IEnumerator ActiveTirgger()
        {
            yield return new WaitForSeconds(0.2f);
            swordTrigger.GetComponent<BoxCollider>().enabled = true;
        }

        IEnumerator EndAttack()
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(1).Length);
            swordTrigger.GetComponent<BoxCollider>().enabled = false;
        }

        public void CheckHold()
        {
            swordTrigger.GetComponent<BoxCollider>().enabled = false;

            sword.GetComponent<PhotonView>().RPC("ActivateParticle", RpcTarget.AllBuffered, 2, false);
            Player.dimensionSwordForMe[2].SetActive(false);
            Player.dimensionSwordForMe[6].SetActive(true);

            enemyObjectData = sword.GetComponent<AeternaSword>().contactObjectData;
            Debug.Log("<color=magenta>" + (string)enemyObjectData[2] + "</color>");

            Player.aeternaUI.GetComponent<AeternaUI>().UIObjects["skill_2_Image_Hashold"].SetActive(true);
            Player.aeternaUI.GetComponent<AeternaUI>().UIObjects["skill_2_Image_Nohold"].SetActive(false);
            
            Player.skill2Phase = 2;
            Player.playerActionDatas[(int)PlayerActionState.Skill2].isExecuting = false;

            Player.skillTimer[2] = Player.aeternaData.skill2HoldTime;
            Player.skillButton = 0;

            Player.soundManager.PlayAudio("SweepSound", 1.0f, false, false, "EffectSound");
        }

        private void OnHoldShoot()
        {
            Player.aeternaUI.GetComponent<AeternaUI>().UIObjects["skill_2_Image_Hashold"].SetActive(false);
            Player.aeternaUI.GetComponent<AeternaUI>().UIObjects["skill_2_Image_Nohold"].SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                ShootProjectile(enemyObjectData,Player.camera.transform.localRotation, Player.camera.transform.rotation);
            }

            else
            {
                photonView.RPC("ShootProjectile", RpcTarget.MasterClient, enemyObjectData, Player.camera.transform.localRotation, Player.camera.transform.rotation);
            }

            Player.skill2Phase = 3;
            Player.skillTimer[2] = Player.aeternaData.skill2CoolTime;
        }

        [PunRPC]
        public void ShootProjectile(object[] enemyData, Quaternion rot, Quaternion rot1)
        {
            object[] data = enemyData;
            data[0] = Player.playerName;
            data[1] = gameObject.tag;

            if((string)data[2] == "BurstFlare")
            {
                data[3] = Player.camera.transform.position;
                data[4] = rot1 * Vector3.forward;
                PhotonNetwork.Instantiate("Projectiles/" + (string)data[2], Vector3.zero, Quaternion.identity, 0, data);
            }

            else if((string)data[2] == "Dagger")
            {
                PhotonNetwork.Instantiate("Projectiles/" + (string)data[2], Player.camera.transform.position, rot1, 0, data);
            }

            else if((string)data[2] == "DragonicFlame")
            {
                PhotonNetwork.Instantiate("Projectiles/Dragonic Flame Projectile 3", Player.camera.transform.position, rot1, 0, data);
            }

            else if ((string)data[2] == "DragonPunch")
            {
                PhotonNetwork.Instantiate("Projectiles/Dragon punch", Player.camera.transform.position, rot1, 0, data);
            }

            else
            {
                data[3] = rot;
                PhotonNetwork.Instantiate("Projectiles/" + (string)data[2], Player.camera.transform.position, Player.transform.localRotation, 0, data);
            }

        }


        //[PunRPC]
        //public void ShootProjectile(object[] enemyData, Quaternion rot)
        //{
        //    object[] data = enemyData;
        //    data[0] = Player.playerName;
        //    data[1] = gameObject.tag;
        //    data[3] = rot;

        //    Debug.Log((string)data[2]);

        //    PhotonNetwork.Instantiate("Projectiles/" + (string)data[2],
        //        Player.camera.transform.position, Player.transform.localRotation, 0, data);
        //}
    }
}