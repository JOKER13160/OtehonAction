using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    GameObject scoreText;
    float score = 0;
    
    public void GetKoban()
    {
        this.score += 100;
    }
    void Start()
    {
        this.scoreText = GameObject.Find("Score");
    }

    // Update is called once per frame
    void Update()
    {
        this.scoreText.GetComponent<TextMeshProUGUI>().text =
           "Score : " + this.score.ToString();
    }
}
