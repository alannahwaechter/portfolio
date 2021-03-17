using Game;
using UnityEngine;

namespace RLAgent
{
    public class TrainingEnvironmentInstanceSettings : MonoBehaviour
    {
        [SerializeField] private bool _isRLTraining;
    
        private GeneralGameManager _gameManagerInstance;
        private Actions _actionsInstance;
        private CharacterSettings[] _characters;

        public GeneralGameManager GameManagerInstance => _gameManagerInstance;
        public Actions ActionsInstance => _actionsInstance;

        public bool IsRlTraining => _isRLTraining;

        private void InitializeEnvironment()
        {
            _gameManagerInstance = GetComponentInChildren<GeneralGameManager>();
            _actionsInstance = GetComponentInChildren<Actions>();
            _characters = GetComponentsInChildren<CharacterSettings>();

            foreach (CharacterSettings character in _characters)
            {
                character.InitializeCharacter();
            }
        }

        private void Awake()
        {
            InitializeEnvironment();
        }
    }
}
