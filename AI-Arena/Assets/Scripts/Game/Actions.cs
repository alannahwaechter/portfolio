using System;
using System.Collections;
using Core;
using RLAgent;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class Actions : MonoBehaviour
    {
        public static Actions Instance;
        
        [Header("Values")]
        [Tooltip("On block the damage received is divided by this value.")][SerializeField] private float _blockDenominator;

        [Header("Attacks")] 
        [Tooltip("Link Front Punch Scriptable Object here.")][SerializeField] private ScriptableAttack _frontPunch;
        [Tooltip("Link Back Punch Scriptable Object here.")][SerializeField] private ScriptableAttack _backPunch;
        [Tooltip("Link Front Kick Scriptable Object here.")][SerializeField] private ScriptableAttack _frontKick;
        [Tooltip("Link Back Kick Scriptable Object here.")][SerializeField] private ScriptableAttack _backKick;

        private bool _isCompleted;

        private ScriptableAttack _currentAttack;
        private static CharacterSettings _charOne;
        private static CharacterSettings _charTwo;
        private TrainingEnvironmentInstanceSettings _rlTraining;
        private float _normalizedTime = 0;
        private float _duration = 0;

        private CharacterSettings _opponent;
        private ScriptableAttack _attack;
        
        public float BlockDenominator => _blockDenominator;
        public ScriptableAttack FrontPunch => _frontPunch;
        public ScriptableAttack BackPunch => _backPunch;
        public ScriptableAttack FrontKick => _frontKick;
        public ScriptableAttack BackKick => _backKick;

        // private void Awake()
        // {
        //     // Debug.Log("Actions.Awake() has been called");
        // }

        private void OnEnable()
        {
            _rlTraining = GetComponentInParent<TrainingEnvironmentInstanceSettings>();
            
            if (_rlTraining != null && _rlTraining.IsRlTraining)
            {
                Instance = _rlTraining.ActionsInstance;
                _charOne = _rlTraining.GetComponentsInChildren<CharacterSettings>()[0];
                _charTwo = _rlTraining.GetComponentsInChildren<CharacterSettings>()[1];
            }
            else
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                    // Debug.Log("Creating Actions singleton with gameObject ID " + gameObject.GetInstanceID());
                }
                else
                {
                    // Debug.LogWarning("Actions.Instance is not null, destroying gameObject ID " +  gameObject.GetInstanceID());
                    Destroy(gameObject);
                    return;
                }
            }

            _isCompleted = true;

            _frontPunch.LameCount = 0;
            _backPunch.LameCount = 0;
            _frontKick.LameCount = 0;
            _backKick.LameCount = 0;            
        }

        public void ResetIsCompleted()
        {
            _isCompleted = true;
        }

        public static void GetCharactersOutsideTraining()
        {
            _charOne = GeneralGameManager.Characters[0];
            _charTwo = GeneralGameManager.Characters[1];
        }

        public void Attack(CharacterSettings opponent, ScriptableAttack attack)
        {
            // Debug.Log("Initialize " + attack.name + ".");

            if (!_isCompleted)
            {
                // Debug.Log(attack.name + " failed due to other action.");
                attack.IsSuccess = false;
                return;
            }

            _isCompleted = false;
            
            attack.CharAnimator.SetTrigger(attack.TriggerName);

            if (Vector3.Distance(_charOne.transform.localPosition,
                        _charTwo.transform.localPosition) > attack.AttackDistance)
            {
                // Debug.Log(attack.name + " failed due to character being out of range.");
                _isCompleted = true;
                attack.IsSuccess = false;
                return;
            }

            if (attack != _currentAttack)
            {
                _currentAttack = attack;
                attack.LameCount = 0;
                attack.TotalDamageAmount = attack.BaseDamage;
            }
            else
            {
                attack.LameCount++;
            }

            _opponent = opponent;
            _attack = attack;
            _duration = attack.ExecutionTime;

            // StartCoroutine(AttackExecutionTimer(opponent, attack));
            // ExecuteAttackTimer(opponent, attack);
            // Invoke("ExecuteAttackTimer", attack.ExecutionTime);
        }

        private void Update()
        {
            if (_duration == 0)
            {
                return;
            }

            _duration -= Time.deltaTime;
            if (_duration <= _normalizedTime)
            {
                _duration = 0;
                ExecuteAttack();
            }
        }

        private IEnumerator AttackExecutionTimer(CharacterSettings opponent, ScriptableAttack attack)
        {
            float duration = attack.ExecutionTime;
            float normalizedTime = 0;
            while (normalizedTime <= duration)
            {
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }

            DoDamage(opponent, attack);
            attack.IsSuccess = true;
            _isCompleted = true;
        }

        private void ExecuteAttack()
        {
            DoDamage(_opponent, _attack);
            _attack.IsSuccess = true;
            _isCompleted = true;
        }

        public void DoDamage(CharacterSettings opponent, ScriptableAttack attack)
        {
            int denominator;
            
            if (_rlTraining != null && _rlTraining.IsRlTraining)
            {
                denominator = 10;
            }

            else
            {
                denominator = 100;
            }
            
            opponent.HealthBar.OnDamageEvent(attack.TotalDamageAmount/denominator);
        }

        /// <summary>
        /// Move the Character
        /// </summary>
        /// <param name="dirValue">left or right expressed through -1 and 1 for ai</param>
        /// <param name="controller">the respective character controller</param>
        /// <param name="speed">well that is self explaining</param>
        public void Move(float dirValue, CharacterController controller, float speed)
        {
            var direction = new Vector3(dirValue * (speed/100), 0, 0);
            controller.Move(direction);
        }
    }
}
