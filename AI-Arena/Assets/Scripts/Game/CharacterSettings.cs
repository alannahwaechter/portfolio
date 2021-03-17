using System;
using System.Collections;
using System.Globalization;
using Component;
using Core;
using RLAgent;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Game
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterSettings : MonoBehaviour
    {
        [Tooltip("Pass the health bar for this character.")][SerializeField] private HealthBar _healthBar;
        [Tooltip("Link the respective opponent here.")][SerializeField] private CharacterSettings _opponent;
        [SerializeField] private Text _attackNameText;
        [SerializeField] private Text _damageAmountText;
        [SerializeField] private Text _nameText;
        [SerializeField][Range(1, 10)] private float _movementSpeed;
        [SerializeField] private int _maxBlockHitCount;
        [SerializeField] private float _unableToBlockTime;
        public GeneralGameManager.CharacterType CharType;
        [HideInInspector]public bool IsBlocking;
        
        private CharacterController _controller;
        private Actions _actions;
        private DataDisplay _dataDisplay;
        private Animator _animator;
        private TrainingEnvironmentInstanceSettings _rlTraining;

        private float _delayAfterCounter;
        private float _currentDelayAfterCounter;
        private float _blockTime;

        private bool _isCountered;
        private bool _unableToBlock;

        private int _currentBlockHitCount;

        private string _fighterName;
        
        public HealthBar HealthBar => _healthBar;
        public CharacterSettings Opponent => _opponent;
        public float CurrentHealth => _healthBar.CurrentHealth;
        public float MovementSpeed => _movementSpeed;
        public float BlockTime => _blockTime;
        public bool IsCountered => _isCountered;
        public string FighterName => _fighterName;

        public CharacterController Controller => _controller;

        public void InitializeCharacter()
        {
            _rlTraining = GetComponentInParent<TrainingEnvironmentInstanceSettings>();
            _controller = GetComponent<CharacterController>();

            if (_rlTraining != null && _rlTraining.IsRlTraining)
            {
                _actions = _rlTraining.ActionsInstance;
            }
            else
            {
                _actions = Actions.Instance;
            }

            _dataDisplay = FindObjectOfType<DataDisplay>();
            _animator = GetComponentInChildren<Animator>();
            IsBlocking = false;
            _delayAfterCounter = 3;
            _currentDelayAfterCounter = 0;
            _isCountered = false;
            _unableToBlock = false;
            
            switch (CharType)
            {
                case GeneralGameManager.CharacterType.ClassicAI:
                    _fighterName = "UTILITY KAIN";
                    break;
                case GeneralGameManager.CharacterType.Player:
                    _fighterName = "HUMAN PLAYER";
                    break;
                case GeneralGameManager.CharacterType.RLAgent:
                    _fighterName = "REMAN LUSS";
                    break;
            }

            _nameText.text = _fighterName;
        }
        private void FixedUpdate()
        {
            switch (CharType)
            {
                case GeneralGameManager.CharacterType.ClassicAI:
                    break;
                case GeneralGameManager.CharacterType.Player:
                    if (IsBlocking)
                        return;
                    _actions.Move(Input.GetAxis("Horizontal"), _controller, _movementSpeed);
                    break;
                case GeneralGameManager.CharacterType.RLAgent:
                    break;
            }
        }

        private void Update()
        {
            if (_controller.velocity.x != 0)
            {
                _unableToBlock = true;
            }
            else
            {
                _unableToBlock = false;
            }
            
            if (IsBlocking)
            {
                _blockTime += Time.deltaTime;
            }
            else
            {
                _blockTime = 0;
            }
            if (_isCountered)
            {
                _currentDelayAfterCounter += Time.deltaTime;

                if (_currentDelayAfterCounter >= _delayAfterCounter)
                {
                    _currentDelayAfterCounter = 0;
                    _isCountered = false;
                }
            }
            _animator.SetBool("IsWalking", _controller.velocity.x != 0);
            _animator.SetBool("IsBlocking", IsBlocking);

            if (CharType != GeneralGameManager.CharacterType.Player)
            {
                return;
            }
            
            CheckForPlayerAttack();
            
            if (Input.GetKey(KeyCode.O) && !_unableToBlock)
            {
                IsBlocking = true;
                _dataDisplay.SetData(_attackNameText, "Block");
            }
            else
            {
                IsBlocking = false;
            }
        }
        
        private void CheckForPlayerAttack()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                DoAttack(_actions.FrontPunch);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                DoAttack(_actions.BackPunch);
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                DoAttack(_actions.FrontKick);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                DoAttack(_actions.BackKick);
            }
        }

        public void DoAttack(ScriptableAttack attack)
        {
            if (IsBlocking || _isCountered)
            {
                return;
            }

            attack.CharAnimator = _animator;

            float actualDamage;
            
            if (_opponent.IsBlocking)
            {
                actualDamage = attack.BaseDamage / _actions.BlockDenominator;
            }
            else
            {
                actualDamage = attack.BaseDamage;
                if (_opponent._currentBlockHitCount > 0)
                {
                    _opponent._currentBlockHitCount = 0;
                }
            }
        
            if (attack.LameCount >= 3)
            {
                attack.TotalDamageAmount = actualDamage / (attack.LameCount - 2);
            }
            else
            {
                attack.TotalDamageAmount = actualDamage;
            }

            _actions.Attack(_opponent, attack);
            
            if (attack.IsSuccess)
            {
                if (_opponent.IsBlocking)
                {
                    _opponent._currentBlockHitCount++;
                }
                else
                {
                    _opponent._currentBlockHitCount = 0;
                }

                if (_dataDisplay != null)
                {
                    _dataDisplay.SetData(_attackNameText, attack.name);
                    _dataDisplay.SetData(_damageAmountText, Math.Round(attack.TotalDamageAmount, 2).ToString());
                }
            }

            if (_opponent.IsBlocking && _opponent._blockTime <= 1)
            {
                _isCountered = true;
                if (_dataDisplay != null)
                {
                    _dataDisplay.SetData(_attackNameText, attack.name);
                    _dataDisplay.SetData(_damageAmountText, "Countered");
                }
            }

            if (_opponent._currentBlockHitCount == _opponent._maxBlockHitCount && _opponent.IsBlocking)
            {
                _opponent.IsBlocking = false;
                _unableToBlock = true;
                StartCoroutine(UnableToBlockTimer());
            }
        }

        public IEnumerator UnableToBlockTimer()
        {
            float duration = _unableToBlockTime;
            float normalizedTime = 0;
            while(normalizedTime <= duration)
            {
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
            _unableToBlock = false;
        }
    }
}
