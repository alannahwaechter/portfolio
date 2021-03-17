using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class InGameMenuManager : MonoBehaviour
    {
        private Button[] _menuButtons;
        private CanvasGroup[] _menuCollection;
        private CanvasGroup _charOneMenu;
        private CanvasGroup _charTwoMenu;
        
        private bool _isPlayerInGame;
        private CanvasGroup _playerMenu;

        public void StartInGameMenu()
        {
            _menuCollection = FindObjectsOfType<CanvasGroup>().OrderBy(menu => menu.gameObject.name).ToArray();
            _charOneMenu = _menuCollection[0];
            _charTwoMenu = _menuCollection[1];

            _charOneMenu.alpha = 0;
            _charOneMenu.interactable = false;
            _charOneMenu.blocksRaycasts = false;
        
            _charTwoMenu.alpha = 0;
            _charTwoMenu.interactable = false;
            _charTwoMenu.blocksRaycasts = false;

            Time.timeScale = 1;
            
            if (GeneralGameManager.Characters[0].CharType == GeneralGameManager.CharacterType.Player)
            {
                _playerMenu = _charOneMenu;
            }
            else if (GeneralGameManager.Characters[1].CharType == GeneralGameManager.CharacterType.Player)
            {
                _playerMenu = _charTwoMenu;
            }
            else
            {
                _playerMenu = _charOneMenu;
            }

            _menuButtons = _playerMenu.GetComponentsInChildren<Button>().OrderBy(button => button.gameObject.name).ToArray();
            
            _menuButtons[3].onClick.RemoveAllListeners();
            _menuButtons[2].onClick.RemoveAllListeners();
            _menuButtons[0].onClick.RemoveAllListeners();
            _menuButtons[1].onClick.RemoveAllListeners();
            
            _menuButtons[3].onClick.AddListener(Resume);
            _menuButtons[2].onClick.AddListener(Restart);
            _menuButtons[0].onClick.AddListener(BackToMenu);
            _menuButtons[1].onClick.AddListener(QuitGame);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            ManageMenu(_playerMenu.interactable);
        }

        private void ManageMenu(bool isCurrentlyVisible)
        {
            if (!isCurrentlyVisible)
            {
                _playerMenu.alpha = 1;
                _playerMenu.interactable = true;
                _playerMenu.blocksRaycasts = true;

                Time.timeScale = 0;
            }
            else
            {
                _playerMenu.alpha = 0;
                _playerMenu.interactable = false;
                _playerMenu.blocksRaycasts = false;

                Time.timeScale = 1;
            }
        }

        public void Resume()
        {
            ManageMenu(true);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
