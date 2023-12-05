using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginSystem : MonoBehaviour
{
    // �α���
    public InputField email;
    public InputField passward;
    public GameObject faultPanel;

    // ȸ�� ����
    private bool readyToRegister = false;
    public InputField registerEmail;
    public InputField registerPassward;
    public InputField registerPasswardCheck;
    public Text passwardOkText;
    public InputField userName;
    public InputField birthDate;
    public InputField phoneNumber;
    public InputField nickNameField;
    public Text nickNameCheckText;

    // ���̵� ã��
    public InputField userNameInput;
    public InputField phoneNumberInput;
    public Text resultEmailText;

    // ��й�ȣ ã��
    public InputField userEmailInputField;
    public InputField userNameInputField;
    public InputField userBirthDateInputField;
    public InputField userPhoneNumberInputField;
    public Text resultPasswardText;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseLoginManager.Instance.Init();
        faultPanel.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (string.IsNullOrEmpty(registerPasswardCheck.text))
        {
            passwardOkText.text = "";
        }
        else if (registerPassward.text == registerPasswardCheck.text)
        {
            passwardOkText.text = "Ȯ�εǾ����ϴ�.";
        }
        else
        {
            passwardOkText.text = "��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
        }

        if(nickNameField.text != null && userName.text != null && birthDate.text != null
            && birthDate.text != null && phoneNumber.text != null &&
            registerPassward.text == registerPasswardCheck.text)
        {
            readyToRegister = true;
        }
    }

    public void Register()
    {
        FirebaseLoginManager.Instance.Register(registerEmail.text, registerPassward.text);
    }

    public void SignIn()
    {
        FirebaseLoginManager.Instance.SignIn(email.text, passward.text, faultPanel);
    }

    public void SignOut()
    {
        FirebaseLoginManager.Instance.SignOut();
    }

    public void OkBtnClick()
    {
        if (readyToRegister)
        {
            FirebaseLoginManager.Instance.SetNickname(nickNameField.text);
            FirebaseLoginManager.Instance.SetUserName(userName.text);
            FirebaseLoginManager.Instance.SetBirthDate(birthDate.text);
            FirebaseLoginManager.Instance.SetPhoneNumber(phoneNumber.text);
            Register();
        }
    }

    public void OnClickCheckNickName()
    {
        nickNameCheckText.text = nickNameField.text;
    }

    public void OnClickFindID()
    {
        FindIDToName();
    }

    public void OnClickFindPassward()
    {
        FindPasswardToEmail();
    }

    async void FindIDToName()
    {
        string _result = await FirebaseLoginManager.Instance.FindUserEmail(userNameInput.text,
            phoneNumberInput.text);

        resultEmailText.text = _result;
    }

    async void FindPasswardToEmail()
    {
        string _result = await FirebaseLoginManager.Instance.FindUserPassward(userEmailInputField.text,
            userNameInputField.text, userBirthDateInputField.text, userPhoneNumberInputField.text);

        resultPasswardText.text = _result;
    }

    public void OnClickGameExit()
    {
        Application.Quit();
    }
}
