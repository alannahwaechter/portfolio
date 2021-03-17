using System;
using System.Linq;
using Core;
using Menu;
using RLAgent;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Game
{
    public class GeneralGameManager : MonoBehaviour
    {
        public static GeneralGameManager Instance;
        public static CharacterSettings[] Characters;

        [FormerlySerializedAs("_fighterModel")] [Header("RL Agent")][SerializeField]private NNModel _leftFighterModel;
        [SerializeField]private NNModel _rightFighterModel;
        [SerializeField] private NNModel _selfTrainedModel;

        private CanvasGroup _endGamePanel;
        private CharacterSettings _winner;
        private Button[] _endGamePanelButtons;

        private TrainingEnvironmentInstanceSettings _rlTraining;

        public NNModel LeftFighterModel => _leftFighterModel;
        public NNModel RightFighterModel => _rightFighterModel;
        public NNModel SelfTrainedModel => _selfTrainedModel;

        public enum CharacterType
        {
            NotSelected,
            Player,
            ClassicAI,
            RLAgent
        }

        private void OnEnable()
        {
            _rlTraining = FindObjectOfType<TrainingEnvironmentInstanceSettings>();
            if (_rlTraining != null && _rlTraining.IsRlTraining)
            {
                Instance = _rlTraining.GameManagerInstance;
            }
            else
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
            }    
            
            if (_rlTraining != null && _rlTraining.IsRlTraining)
                return;
            
            SceneManager.sceneLoaded += OnSceneChangedEvent;
        }
        
        private void Update()
        {
            if (_rlTraining != null && _rlTraining.IsRlTraining)
                return;
            
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
            {
                foreach (CharacterSettings character in Characters)
                {
                    if (character.CurrentHealth <= 0)
                    {
                        _endGamePanel.alpha = 1;
                        _endGamePanel.interactable = true;
                        _endGamePanel.blocksRaycasts = true;
                        Time.timeScale = 0;

                        _winner = character.Opponent;
                        Text text = _endGamePanel.GetComponentInChildren<Text>();
                        text.text = _winner.name + " " + _winner.FighterName + " WINS!";
                    }
                }
            }
        }


        private void OnDisable()
        {
            if (_rlTraining != null && _rlTraining.IsRlTraining)
                return;
            
            SceneManager.sceneLoaded -= OnSceneChangedEvent;
        }

        private void OnSceneChangedEvent(Scene scene, LoadSceneMode mode)
        {
            if (scene != SceneManager.GetSceneByBuildIndex(2))
            {
                return;
            }

            Characters = FindObjectsOfType<CharacterSettings>();
            Characters = Characters.OrderBy(settings => settings.gameObject.name).ToArray();
            Characters[0].CharType = MainMenuManager.CharOneType;
            Characters[1].CharType = MainMenuManager.CharTwoType;
            if (Characters[0].CharType == CharacterType.ClassicAI)
            {
                Characters[0].gameObject.AddComponent<ClassicAIControl>();
                Characters[0].gameObject.AddComponent<UtilityScoreManager>();
            }
            if (Characters[1].CharType == CharacterType.ClassicAI)
            {
                Characters[1].gameObject.AddComponent<ClassicAIControl>();
                Characters[1].gameObject.AddComponent<UtilityScoreManager>();
            }
            
            if (Characters[0].CharType == CharacterType.RLAgent)
            {
                Characters[0].gameObject.AddComponent<FighterAgent>();
            }
            if (Characters[1].CharType == CharacterType.RLAgent)
            {
                Characters[1].gameObject.AddComponent<FighterAgent>();
            }

            foreach (CharacterSettings character in Characters)
            {
                character.InitializeCharacter();
            }
            
            Actions.GetCharactersOutsideTraining();

            _endGamePanel = FindObjectOfType<Canvas>().GetComponentsInChildren<CanvasGroup>().OrderBy(cg => cg.name).ToArray()[2];
            _endGamePanel.alpha = 0;
            _endGamePanel.interactable = false;
            _endGamePanel.blocksRaycasts = false;

            gameObject.GetComponent<InGameMenuManager>().StartInGameMenu();
            
            Time.timeScale = 1;

            _endGamePanelButtons = _endGamePanel.GetComponentsInChildren<Button>().OrderBy(button => button.gameObject.name).ToArray();
            
            _endGamePanelButtons[0].onClick.RemoveAllListeners();
            _endGamePanelButtons[1].onClick.RemoveAllListeners();
            
            _endGamePanelButtons[0].onClick.AddListener(gameObject.GetComponent<InGameMenuManager>().BackToMenu);
            _endGamePanelButtons[1].onClick.AddListener(gameObject.GetComponent<InGameMenuManager>().Restart);
        }
    }
}
