using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scoreboard : MonoBehaviour {
    
	void Awake () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}

    GameManager gm;

    public Text titleText;
    string defaultTitleText = "SCORES";
    string gameOverText = "GAME OVER";
    string winningText = "YOU WON!!";
    public Text[] PlayerScores;
    public GameObject scoreboard;
    public Button NextRoundButton;
    public Button PlayAgainButton;
    public Button ExitButton;

    public float waitBetweenRounds = 2f;
    public float waitBetweenGames = 2f;

    int[] playerScores = new int[4];

    public void ShowScoreboard()
    {
        Player[] players = gm.Players;
        int minScore = 999;
        int maxScore = 0;
        int winningPlayer = 0;
        bool hasHumanPlayer = false;
        playerScores = PlayerPrefsX.GetIntArray("PlayerScores", 0, 4);

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Score > maxScore)
            {
                maxScore = players[i].Score;
            }
            PlayerScores[i].text = "(" + playerScores[i].ToString() + ") " + players[i].PlayerName + ": " + players[i].Score;
            if(gm.PlayerAIs[i] == null)
            {
                hasHumanPlayer = true;
            }
        }

        if(maxScore >= 100)
        {
            if (gm.PlayerAIs[winningPlayer] == null)
            {
                titleText.text = winningText;
            }
            else
            {
                titleText.text = gameOverText;
            }
            NextRoundButton.gameObject.SetActive(false);
            PlayAgainButton.gameObject.SetActive(true);
            ExitButton.gameObject.SetActive(true);

            // Get the player with the fewest points
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Score < minScore)
                {
                    minScore = players[i].Score;
                    winningPlayer = i;
                }
            }
            // Get the saved total wins
            playerScores[winningPlayer]++;
            PlayerPrefsX.SetIntArray("PlayerScores", playerScores);
        }
        else
        {
            titleText.text = defaultTitleText;
            NextRoundButton.gameObject.SetActive(true);
            PlayAgainButton.gameObject.SetActive(false);
            ExitButton.gameObject.SetActive(false);
        }

        scoreboard.SetActive(true);

        if (!hasHumanPlayer)
        {
            if(maxScore >= 100)
                StartCoroutine("NextGameCoroutine");
            else
                StartCoroutine("NextRoundCoroutine");
        }
    }

    public void HideScoreboard()
    {
        scoreboard.SetActive(false);
    }

    public void NextRound()
    {
        HideScoreboard();
        gm.SetupRound();
    }

    IEnumerator NextRoundCoroutine()
    {
        yield return new WaitForSeconds(waitBetweenRounds);

        NextRound();
    }

    public void NextGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator NextGameCoroutine()
    {
        yield return new WaitForSeconds(waitBetweenGames);

        NextGame();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
