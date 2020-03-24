using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Text txtScore;

    private int totScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        //처음 실행된후 저장된 스코어정보 로드입니다.
        totScore = PlayerPrefs.GetInt("TOT_SCORE", 0);
        DispScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //점수누적 및 화면표기
    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";

        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }
}
