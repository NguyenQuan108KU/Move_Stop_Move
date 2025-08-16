using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HairManager : MonoBehaviour
{
    public GameObject[] hairList;
    public GameObject[] buttons;
    public int layerButton;

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                layerButton = buttons[index].layer;
                SaveHat();
                SetHairs(layerButton);
            });
        }
    }
    public void SetHairs(int x)
    {
        for(int i = 0; i < hairList.Count(); i++)
        {
            if(x == i)
            {
                hairList[i].SetActive(true);
            }
            else
            {
                hairList[i].SetActive(false);
            }
        }
    }
    public void SaveHat()
    {
        PlayerPrefs.SetInt("IndexHat", layerButton);
    }
    public int LoadHats()
    {
        int x = PlayerPrefs.GetInt("IndexHat");
        return x;
    }
}
