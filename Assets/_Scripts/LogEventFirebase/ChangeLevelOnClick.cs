using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ChangeLevelOnClick : MonoBehaviour, IPointerClickHandler
{
    public string _levelToLoad;
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(_levelToLoad);
    }
}
