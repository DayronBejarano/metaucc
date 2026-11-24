using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField profileNameInputField;
    public Button loginButton;

    void Start()
    {
        if (loginButton != null)
        {
            loginButton.interactable = false;
        }

        if (profileNameInputField != null)
        {
            profileNameInputField.onValueChanged.AddListener(CheckInput);
        }
    }

    void OnDestroy()
    {
        if (profileNameInputField != null)
        {
            profileNameInputField.onValueChanged.RemoveListener(CheckInput);
        }
    }

    private void CheckInput(string inputText)
    {
        if (loginButton != null)
        {
            loginButton.interactable = !string.IsNullOrWhiteSpace(inputText);
        }
    }

    public void OnLoginClicked()
    {
        if (profileNameInputField == null || string.IsNullOrWhiteSpace(profileNameInputField.text))
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("MainMenu: GameManager.Instance no está en la escena.");
            return;
        }

        string profileName = profileNameInputField.text.Trim();
        GameManager.Instance.LoginUser(profileName);

        if (SaveSystem.ProfileExists(profileName))
        {
            SceneManager.LoadScene("Proyecto_General");
        }
        else
        {
            SceneManager.LoadScene("Avatars_EditorScene");
        }
    }

    public void OnEditAvatarClicked()
    {
        if (profileNameInputField == null || string.IsNullOrWhiteSpace(profileNameInputField.text))
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("MainMenu: GameManager.Instance no está en la escena.");
            return;
        }

        string profileName = profileNameInputField.text.Trim();
        GameManager.Instance.LoginUser(profileName);
        SceneManager.LoadScene("Avatars_EditorScene");
    }
}
