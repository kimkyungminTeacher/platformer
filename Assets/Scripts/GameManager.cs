using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health = 3;

    public PlayerMove player;
    public GameObject[] stages;

    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;

    private void Awake() {
        UIRestartBtn.SetActive(false);
    }

    private void Update() {
        UIPoint.text = (totalPoint + stagePoint).ToString();
        UIStage.text = "STAGE" + (stageIndex + 1);
    }

    public void NextStage()
    {
        //점수 계산
        totalPoint += stagePoint;
        stagePoint = 0;

        if (stageIndex == stages.Length - 1){
            //game clear
            ActiveButton("Clear!");
        } else {
            //stage 변경
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            
            //respwon
            player.respown();
        }
    }

    private void ActiveButton(string buttonName)
    {
        Time.timeScale = 0;
            
        Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
        btnText.text = buttonName;
        UIRestartBtn.SetActive(true);
    }

    public void AddStagePoint(string name)
    {
        if (name.StartsWith("Bronze"))
            this.stagePoint += 50;
        else if (name.StartsWith("Silver"))
            this.stagePoint += 100;                
        else if (name.StartsWith("Gold"))
            this.stagePoint += 150;   
        else if (name.StartsWith("Enemy"))
            this.stagePoint += 300;   
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
        {
            HealthDown();
            player.respown();
        }
    }

    public void HealthDown()
    {
        health--;
        UIHealth[health].color = new Color(1, 1, 1, 0.4f);

        if (health == 0)
        {
            player.die();
            ActiveButton("Restart");
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
