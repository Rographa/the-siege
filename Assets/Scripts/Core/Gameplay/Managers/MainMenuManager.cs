using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.Gameplay.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private string gameScene = "Game";
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private void OnEnable()
        {
            HandleSubscription(true);
        }

        private void OnDisable()
        {
            HandleSubscription(false);
        }

        private void HandleSubscription(bool subscribe)
        {
            switch (subscribe)
            {
                case true:
                    playButton.onClick.AddListener(PlayButtonClicked);
                    quitButton.onClick.AddListener(QuitButtonClicked);
                    break;
                case false:
                    playButton.onClick.RemoveListener(PlayButtonClicked);
                    quitButton.onClick.RemoveListener(QuitButtonClicked);
                    break;
            }
        }

        private void PlayButtonClicked()
        {
            SceneManager.LoadScene(gameScene);
        }

        private void QuitButtonClicked()
        {
            Application.Quit();
        }
    }
}