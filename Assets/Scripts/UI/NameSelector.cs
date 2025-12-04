using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button setNameButton;

    [SerializeField] private int minLength=1,maxLength=12;

    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            ChangeScene();
            return;
        }

        nameField.text = PlayerPrefs.GetString("Name", string.Empty);
        NameChanged();
    }
    public void NameChanged()
    {
        setNameButton.interactable=nameField.text.Length>minLength&&nameField.text.Length<maxLength;
    }
    public void Connect()
    {
        PlayerPrefs.SetString("Name", nameField.text);
        ChangeScene();
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
