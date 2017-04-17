/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Niobium
{
    /**
    * The Game Interface manager handles the interface actions and transitions for the Game Scenes
    **/
    public class GameInterfaceManager : InterfaceManager
    {
        [Header("Top Hud")]
        public GameObject panelScore;
        public Text textP1Score;
        public Text textP2Score;
        public Text textP1Name;
        public Text textP2Name;
        public Image imageP1Timer;
        public Image imageP1Hightlight;
        public Image imageP2Timer;
        public Image imageP2Hightlight;
        public Color selectedPlayerColor = Color.green;

        [Header("Message Windows")]
        public GameObject panelMessage;
        public Text textMessage;

        [Header("End Game")]
        public GameObject panelEndGame;
        public Image p1WinBadge;
        public Image p2WinBadge;
        public GameObject textDraw;
        public Button buttonEndGameBack;
        public Button buttonEndGameRestart;

        [Header("Panel Quit")]
        public GameObject panelQuit;
        public Button buttonQuit;
        public Button buttonReturn;
        public Button buttonSettings;

        public GameObject panelTutorial;

        // How many seconds the message is shown before vanishing
        private float messageDelay = 2f;

        public static GameInterfaceManager instance;

        void Awake()
        {
            instance = this;

            buttonEndGameBack.onClick.AddListener(() =>
            {
                Preloader.instance.PreloadScene("Main");
            });

            buttonEndGameRestart.onClick.AddListener(() =>
            {
                GameManager.instance.Restart();
            });

            buttonQuit.onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                Preloader.instance.PreloadScene("Main");
            });

            buttonReturn.onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                CloseMenu(panelQuit);
            });

            buttonSettings.onClick.AddListener(() =>
            {
                Time.timeScale = 0;
                OpenMenu(panelQuit);
            });
        }

        // Configures the interface based on the configuration and updates the profiles
        public void Initialize()
        {
            SetPlayer(GameManager.Turn.PLAYER_1);
        }

        public void SetPlayer(GameManager.Turn playerTurn)
        {
            switch (playerTurn)
            {
                case GameManager.Turn.PLAYER_1:
                    textP1Name.color = selectedPlayerColor;
                    textP2Name.color = Color.white;
                    imageP1Hightlight.gameObject.SetActive(true);
                    imageP2Hightlight.gameObject.SetActive(false);
                    ShowMessage("Player 1 Turn");
                    break;
                case GameManager.Turn.PLAYER_2:
                    textP2Name.color = selectedPlayerColor;
                    textP1Name.color = Color.white;
                    imageP1Hightlight.gameObject.SetActive(false);
                    imageP2Hightlight.gameObject.SetActive(true);
                    ShowMessage("Player 2 Turn");
                    break;
            }

        }

        // Updates the players score
        public void UpdateScore(int p1Score, int p2Score)
        {
            textP1Score.text = p1Score.ToString();
            textP2Score.text = p2Score.ToString();
        }

        // Shows a message with a delay
        public void ShowMessage(string message)
        {
            StopCoroutine("ShowMessageCR");

            textMessage.text = message;
            OpenMenu(panelMessage, messageDelay);
        }

        // Updates the circular timer
        public void UpdateTimer(GameManager.Turn playerTurn, float time)
        {
            float relativeTime = time;
            switch (playerTurn)
            {
                case GameManager.Turn.PLAYER_1:
                    imageP1Timer.fillAmount = relativeTime;
                    break;
                case GameManager.Turn.PLAYER_2:
                    imageP2Timer.fillAmount = relativeTime;
                    break;
            }
        }

        // Resets the timer and blurs the inactive player hud
        public void ResetTimer(GameManager.Turn playerTurn)
        {
            switch (playerTurn)
            {
                case GameManager.Turn.PLAYER_1:
                    imageP1Timer.fillAmount = 1f;
                    break;
                case GameManager.Turn.PLAYER_2:
                    imageP2Timer.fillAmount = 1f;
                    break;
            }
        }

        // Show the win window when the game is over
        public void ShowEndGamePanel(GameManager.WinState winState)
        {
            OpenMenu(panelEndGame);

            switch (winState)
            {
                case GameManager.WinState.PLAYER_1_WIN:
                    p1WinBadge.gameObject.SetActive(true);
                    break;
                case GameManager.WinState.PLAYER_2_WIN:
                    p2WinBadge.gameObject.SetActive(true);
                    break;
                case GameManager.WinState.DRAW:
                    textDraw.SetActive(true);
                    break;
            }
        }
    }
}