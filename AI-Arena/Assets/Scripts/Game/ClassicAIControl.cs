using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Core;
using RLAgent;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class ClassicAIControl : MonoBehaviour
    {
        // [SerializeField] private float _lowHealthThreshold = 0.4f;
        private float _decisionTime;
        private float _currentDecisionTime;

        private Actions _actions;
        private UtilityScoreManager _scoreManager;
        private TrainingEnvironmentInstanceSettings _rlTraining;

        // private Node _rootNode;

        private void Start()
        {
            _rlTraining = GetComponentInParent<TrainingEnvironmentInstanceSettings>();
            if (_rlTraining != null && _rlTraining.IsRlTraining)
            {
                _actions = _rlTraining.ActionsInstance;
            }
            else
            {
                _actions = Actions.Instance;
            }
            _scoreManager = GetComponent<UtilityScoreManager>();
            _currentDecisionTime = 0;
            _decisionTime = 2;
        }
   
        public void FixedUpdate()
        {
            _currentDecisionTime += Time.fixedDeltaTime;

            if (_currentDecisionTime >= _decisionTime)
            {
                StandardEvaluation();
            }

            _scoreManager.ExecuteByScore(_scoreManager.CurrentActionScore);
            
            if (_scoreManager.CurrentActionScore == _scoreManager.FrontPunchScore)
            {
                AttackEvaluation(_actions.FrontPunch.ExecutionTime);
            }
            else if (_scoreManager.CurrentActionScore == _scoreManager.BackPunchScore)
            {
                AttackEvaluation(_actions.BackPunch.ExecutionTime);
            }
            else if (_scoreManager.CurrentActionScore == _scoreManager.FrontKickScore)
            {
                AttackEvaluation(_actions.FrontKick.ExecutionTime);
            }
            else if (_scoreManager.CurrentActionScore == _scoreManager.BackKickScore)
            {
                AttackEvaluation(_actions.BackKick.ExecutionTime);
            }
        }

        private void StandardEvaluation()
        {
            _scoreManager.EvaluateScore();
            _decisionTime = Random.Range(1f, 4f);
            _currentDecisionTime = 0;
        }

        private void AttackEvaluation(float minDecisionTime)
        {
            _scoreManager.EvaluateScore();
            _decisionTime = Random.Range(minDecisionTime, 2f);
            _currentDecisionTime = 0;
        }
    }
}
