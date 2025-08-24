using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Next_Func : MonoBehaviour
{
    //---Joystick---
    [SerializeField] private GameObject joystick;
    //---Spawn Zombie---
    [SerializeField] private GameObject spawnZombie;
    //---Menu---
    [SerializeField] private GameObject menu1;
    [SerializeField] private GameObject menu2;

    public List<Sprite> listImages;
    public int selectOption = 0;
    public Image imageOfFunction;
    public void NextOption()
    {
        selectOption++;
        if (selectOption >= listImages.Count)
        {
            selectOption = 0;
        }
        UpdateOption(selectOption);
    }
    public void UpdateOption(int index)
    {
        imageOfFunction.sprite = listImages[index];
    }
    public void OnJoystick()
    {
        joystick.SetActive(true);
        Time.timeScale = 1;
        spawnZombie.SetActive(true);
        menu1.SetActive(false);
        menu2.SetActive(false);
    }
    public void SetDefaultFunction()
    {
        PlayerPrefs.SetInt("Function", -1);
    }
    public void SaveOption()
    {
        PlayerPrefs.SetInt("Function", selectOption);
    }
}
