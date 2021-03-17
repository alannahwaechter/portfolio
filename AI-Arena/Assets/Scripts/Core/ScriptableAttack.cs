using System.Collections;
using UnityEngine;
using Game;

namespace Core
{
    [CreateAssetMenu(fileName = "New Scriptable Attack", menuName = "Scriptable Attack")]
    public class ScriptableAttack : ScriptableObject
    {
        [Header("Damage Amount")] [Tooltip("Set the respective damage amount for this attack type.")] [SerializeField]
        private float _baseDamage;

        [Header("Action Time")]
        [Tooltip("Set the respective time in seconds for this attack to be completed.")]
        [SerializeField]
        private float _executionTime;

        [Header("Action Distance")]
        [Tooltip("Set the respective distance for this attack within it can be executed.")]
        [SerializeField]
        private float _attackDistance;

        [Header("Animation")]
        [Tooltip("Type in the respective name of the trigger used for the animation for this attack.")]
        [SerializeField]
        private string _triggerName;

        [HideInInspector] public Animator CharAnimator;

        private float _totalDamageAmount;
        private bool _isSuccess;
        private int _lameCount;

        public float BaseDamage => _baseDamage;
        public float ExecutionTime => _executionTime;
        public float AttackDistance => _attackDistance;
        public string TriggerName => _triggerName;

        public int LameCount
        {
            get => _lameCount;
            set { _lameCount = value; }
        }

        public float TotalDamageAmount
        {
            get => _totalDamageAmount;
            set => _totalDamageAmount = value;
        }

        public bool IsSuccess
        {
            get => _isSuccess;
            set => _isSuccess = value;
        }

        private void Awake()
        {
            _isSuccess = false;
            TotalDamageAmount = _baseDamage;
        }
    }
}
