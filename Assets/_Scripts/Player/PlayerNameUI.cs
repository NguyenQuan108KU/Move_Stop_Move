using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerNameUI : MonoBehaviour, IPointerDownHandler
{
    [Header("UI Components")]
    public TMP_Text playerNameText;         // Text hiển thị tên nhân vật
    public TMP_InputField nameInputField;   // Ô nhập tên (ẩn sẵn)
    public bool isEditing = false;         // Trạng thái đang nhập tên hay không
    private const string PLAYER_NAME_KEY = "NamePlayer";
    public AdsController banner;
    void Start()
    {
        // Load tên đã lưu
        string savedName = PlayerPrefs.GetString(PLAYER_NAME_KEY, "Player");
        playerNameText.text = savedName;

        // Ẩn input ban đầu
        nameInputField.gameObject.SetActive(false);

        // Khi nhấn Enter cũng ẩn input
        nameInputField.onEndEdit.AddListener(OnNameInputEnd);
        nameInputField.onValueChanged.AddListener(OnNameValueChanged);

    }

    // Bắt sự kiện nhấn chuột xuống trên vùng đối tượng
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isEditing)
            CloseInputField();
        else
            OpenInputField();
    }

    private void OpenInputField()
    {
        banner.HideBanner();
        isEditing = true;
        nameInputField.gameObject.SetActive(true);
        //playerNameText.gameObject.SetActive(false);

        nameInputField.text = playerNameText.text;
        nameInputField.Select();
        nameInputField.ActivateInputField();
        EventSystem.current.SetSelectedGameObject(nameInputField.gameObject);
    }

    private void CloseInputField()
    {
        banner.ShowBanner();
        isEditing = false;

        string newName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(newName))
            newName = "Player";

        playerNameText.text = newName;
        PlayerPrefs.SetString(PLAYER_NAME_KEY, newName);
        PlayerPrefs.Save();

        nameInputField.gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(true);
    }

    private void OnNameInputEnd(string newName)
    {
        CloseInputField();
    }
    private void OnNameValueChanged(string newValue)
    {
        playerNameText.text = newValue;  // Cập nhật theo từng ký tự
        playerNameText.ForceMeshUpdate();
    }

}
