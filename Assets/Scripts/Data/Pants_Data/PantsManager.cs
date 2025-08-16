using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PantsManager : MonoBehaviour
{
    public PantsDatabases pantsDatabases;
    public GameObject[] buttons;
    public GameObject pantsOfPlayer;
    public int layerButton;
    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; 
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                layerButton = buttons[index].layer;
                SavePants();
                SetPants(layerButton);
            });
        }
    }
    public void SetPants(int x)
    {
        if(pantsOfPlayer != null)
            pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[x].material;
    }
    public void SavePants()
    {
        PlayerPrefs.SetInt("IndexPants",layerButton);

    }
    public int LoadPants()
    {
        int x = PlayerPrefs.GetInt("IndexPants");
        return x;
    }
}
