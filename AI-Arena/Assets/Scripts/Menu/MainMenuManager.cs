using System.Collections;
using Game;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu")]
        [SerializeField] private Button _startButton;
        [SerializeField] private CanvasGroup _controls;        
        [SerializeField] private CanvasGroup _credits;
        [SerializeField] private CanvasGroup _fighterSelect;

        [Header("Fighter Select")]
        [SerializeField] private Button _charOnePlayerButton;
        [SerializeField] private Button _charTwoPlayerButton;
        [SerializeField] private Text _charOneChosenText;
        [SerializeField] private Text _charTwoChosenText;
        [SerializeField] private string _baseText;

        public static GeneralGameManager.CharacterType CharOneType;
        public static GeneralGameManager.CharacterType CharTwoType;

        private bool _stopFading;

        private void Start()
        {
            _controls.alpha = 0;
            _controls.blocksRaycasts = false;
            
            _credits.alpha = 0;
            _credits.blocksRaycasts = false;
            
            _fighterSelect.alpha = 0;
            _fighterSelect.interactable = false;
            _fighterSelect.blocksRaycasts = false;
            
            _charOneChosenText.text = "No fighter type selected.";
            _charTwoChosenText.text = "No fighter type selected.";
        }

        public void ChooseCharOneAsPlayer()
        {
            CharOneType = GeneralGameManager.CharacterType.Player;
            _charTwoPlayerButton.interactable = false;
            _charOneChosenText.text = _baseText + " Player";
        }
        public void ChooseCharOneAsClassicAI()
        {
            CharOneType = GeneralGameManager.CharacterType.ClassicAI;
            _charTwoPlayerButton.interactable = true;
            _charOneChosenText.text = _baseText + " Classic AI";
        }
        public void ChooseCharOneAsRLAgent()
        {
            CharOneType = GeneralGameManager.CharacterType.RLAgent;
            _charTwoPlayerButton.interactable = true;
            _charOneChosenText.text = _baseText + " RL Agent";
        }
        public void ChooseCharTwoAsPlayer()
        {
            CharTwoType = GeneralGameManager.CharacterType.Player;
            _charOnePlayerButton.interactable = false;
            _charTwoChosenText.text = _baseText + " Player";
        }
        public void ChooseCharTwoAsClassicAI()
        {
            CharTwoType = GeneralGameManager.CharacterType.ClassicAI;
            _charOnePlayerButton.interactable = true;
            _charTwoChosenText.text = _baseText + " Classic AI";
        }
        public void ChooseCharTwoAsRLAgent()
        {
            CharTwoType = GeneralGameManager.CharacterType.RLAgent;
            _charOnePlayerButton.interactable = true;
            _charTwoChosenText.text = _baseText + " RL Agent";
        }
    
        public void LoadGame()
        {
            if (CharOneType == GeneralGameManager.CharacterType.NotSelected ||
                CharTwoType == GeneralGameManager.CharacterType.NotSelected)
            {
                Debug.Log("Please select a fighter Type for each fighter.");
                return;
            }
            SceneManager.LoadScene(2);
        }

        public void ShowFighterSelect()
        {
            _credits.alpha = 0;

            _controls.alpha = 0;

            _fighterSelect.alpha = 1;
            _fighterSelect.interactable = true;
            _fighterSelect.blocksRaycasts = true;

            _startButton.enabled = false;
        }

        public void ShowControls()
        {
            if (!_startButton.enabled)
            {
                _startButton.enabled = true;
            }
            
            _credits.alpha = 0;

            _controls.alpha = 1;

            _fighterSelect.alpha = 0;
            _fighterSelect.interactable = false;
            _fighterSelect.blocksRaycasts = false;
        }

        public void ShowCredits()
        {
            if (!_startButton.enabled)
            {
                _startButton.enabled = true;
            }

            _credits.alpha = 1;

            _controls.alpha = 0;

            _fighterSelect.alpha = 0;
            _fighterSelect.interactable = false;
            _fighterSelect.blocksRaycasts = false;
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}
