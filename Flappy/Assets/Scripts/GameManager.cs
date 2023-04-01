using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int score = 0;
    public TMP_Text scoreText;
    public TMP_Text gameOverText;
    public GameObject playButton;

    public Bird bird;

    private enum State {
        Idle, Paused, Over,
    }

    private State state;

    void Awake(){
        Application.targetFrameRate = 60;
        startNewGame();
        pauseGame();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.P)){
            if (state == State.Idle){
                pauseGame();
            }
            else if (state == State.Paused){
                resumeGame();
            }
        }
    }    

    public void startNewGame(){
        score = 0;
        scoreText.SetText(score.ToString());
        gameOverText.gameObject.SetActive(false);
        playButton.SetActive(false);
        Time.timeScale = 1f;
        state = State.Idle;
        bird.enabled = true;
        Pipe[] pipes = FindObjectsOfType<Pipe>();
        for (int i=0; i < pipes.Length; i++){
            Destroy(pipes[i].gameObject);
        }
    }

    public void passedGate(){
        score += 1;
        scoreText.SetText(score.ToString());
        print("Score: " + score.ToString());
    }

    public void endGame(){
        pauseGame();
        gameOverText.gameObject.SetActive(true);
        CancelInvoke("SpawnPipe");
        bird.enabled = false;
        state = State.Over;
    }

    private void pauseGame(){
        Time.timeScale = 0;
        bird.enabled = false;
        playButton.SetActive(true);
        state = State.Paused;
    }

    private void resumeGame (){
        Time.timeScale = 1;
        playButton.SetActive(false);
        bird.enabled = true;
        state = State.Idle;
    }
}
