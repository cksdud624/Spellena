using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SelectingCharacter : MonoBehaviour
{
    public List<GameObject> avatar;
    public GameObject confirm;
    public GameObject reSelect;

    private string selectName;

   public void SelectCharacter(string name)
   {
        switch (name)
        {
            case "Aeterna":
                selectName = name;
                avatar[0].SetActive(true);
                avatar[1].SetActive(false);
                break;
            case "ElementalOrder":
                selectName = name;
                avatar[0].SetActive(false);
                avatar[1].SetActive(true);
                break;
            default:
                break;
        }
   }

    public void ConfirmCharacter()
    {
        if (!avatar[0].activeSelf && !avatar[1].activeSelf) return;

        confirm.SetActive(false);
        reSelect.SetActive(true);
        GameCenterTest.ChangePlayerCustomProperties(PhotonNetwork.LocalPlayer, "Character", selectName);
        // �ð� �ʰ��� �������� ������ ĳ���Ͱ� �ȴ�
        // �ƹ� ���� ���ϸ� �������� ��ȯ
    }

    public void ReSelectCharacter()
    {
        confirm.SetActive(true);
        reSelect.SetActive(false);
    }



}
