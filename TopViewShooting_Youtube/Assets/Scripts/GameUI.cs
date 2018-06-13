using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    UIButton playAgain;

	void Start ()
    {
        playAgain = GameObject.Find("Game Over").GetComponent<UIButton>();
        FindObjectOfType<Player>().OnDeath += OnGameOver;
	}
	
	void Update ()
    {
		
	}

    void OnGameOver()
    {

    }
}
