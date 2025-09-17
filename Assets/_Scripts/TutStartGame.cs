using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutStartGame : MonoBehaviour
{
    public GameObject enemyActive;
    public GameObject arrow;
    public GameObject textStop;
    public GameObject textMove;
    public GameObject arrowToMove;
    private void Update()
    {
        DisplayText();
    }
    public void DisplayText()
    {
        if(GameController.instance.playerController.pointOfPlayerDefault == 3)
        {
            textMove.SetActive(true);
            textStop.SetActive(false);
            arrowToMove.SetActive(true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyActive.SetActive(true);
            arrow.SetActive(false);
            textStop.SetActive(true);
        }
    }
}
