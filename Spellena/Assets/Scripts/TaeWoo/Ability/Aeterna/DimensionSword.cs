using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class DimensionSword : Ability
    {
        private Aeterna Player;
        private Animator animator;
        private GameObject Sword;
        private int NearDamage = 80;
        private int FarDamage = 120;

        public DimensionSword(Aeterna player)
        {
            Player = player;
            animator = player.GetComponent<Animator>();
            Sword = player.DimensionSword;
        }
        public void Execution()
        {
            //���� �ִϸ��̼� ���, Į Ȱ��ȭ
            //���� Ÿ���� ���ٸ� ���Ÿ� ����
            animator.SetTrigger("BasicAttack");
            Sword.GetComponent<BoxCollider>().enabled = true;
            //StartCoroutine(EndAttack());
            Debug.Log("DimensionSword!");
        }

        void Update()
        {
            Debug.Log("Update");
        }

        IEnumerator EndAttack()
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(1).Length);
            Sword.GetComponent<BoxCollider>().enabled = false;
            Debug.Log("DimensionSword finish");
        }
    }
}