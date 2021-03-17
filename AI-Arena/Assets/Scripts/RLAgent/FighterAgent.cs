using Game;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace RLAgent
{
    public class FighterAgent : Agent
    {
        private CharacterSettings _character;
        private CharacterSettings _opponent;

        private TrainingEnvironmentInstanceSettings _rlTraining;
        private GeneralGameManager _gameManager;
        private Actions _actions;
        private DecisionRequester _requester;

        private Vector3 _characterStartingPosition;
        private Vector3 _opponentStartingPosition;

        private float _healthBefore;
        private float _opponentHealthBefore;

        public override void Initialize()
        {
            _gameManager = GeneralGameManager.Instance;

            // _episodeTime = 0;
            _character = GetComponent<CharacterSettings>();
            _opponent = _character.Opponent;

            _characterStartingPosition = gameObject.transform.position;
            _opponentStartingPosition = _opponent.gameObject.transform.position;

            _rlTraining = GetComponentInParent<TrainingEnvironmentInstanceSettings>();
        
            if (_rlTraining != null && _rlTraining.IsRlTraining)
            {
                _actions = _rlTraining.ActionsInstance;
            }
            else
            {
                if (_characterStartingPosition.x < 0)
                {
                    // GetComponent<BehaviorParameters>().TeamId = 0;
                    SetModel("Fighter", _gameManager.LeftFighterModel);
                }
                else if (_characterStartingPosition.x > 0)
                {
                    // GetComponent<BehaviorParameters>().TeamId = 1;
                    SetModel("Fighter", _gameManager.RightFighterModel);
                }
                // SetModel("Fighter", _gameManager.SelfTrainedModel);
                _actions = Actions.Instance;
                GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 20;
                GetComponent<BehaviorParameters>().BrainParameters.VectorActionSpaceType = SpaceType.Discrete;
                GetComponent<BehaviorParameters>().BrainParameters.VectorActionSize[0] = 8;
                GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
            
                gameObject.AddComponent<DecisionRequester>();
            }

            _healthBefore = 1;
            _opponentHealthBefore = 1;
            _requester = GetComponent<DecisionRequester>();
        }
    
        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(Vector3.Distance(gameObject.transform.position, _opponent.gameObject.transform.position));

            sensor.AddObservation(_opponent.IsBlocking);
        
            sensor.AddObservation(_character.CurrentHealth);
            sensor.AddObservation(_opponent.CurrentHealth);
        
            sensor.AddObservation(_actions.FrontPunch.BaseDamage);
            sensor.AddObservation(_actions.FrontPunch.AttackDistance);
            sensor.AddObservation(_actions.FrontPunch.ExecutionTime);
            sensor.AddObservation(_actions.FrontPunch.IsSuccess);
        
            sensor.AddObservation(_actions.BackPunch.BaseDamage);
            sensor.AddObservation(_actions.BackPunch.AttackDistance);
            sensor.AddObservation(_actions.BackPunch.ExecutionTime);
            sensor.AddObservation(_actions.BackPunch.IsSuccess);
        
            sensor.AddObservation(_actions.FrontKick.BaseDamage);
            sensor.AddObservation(_actions.FrontKick.AttackDistance);
            sensor.AddObservation(_actions.FrontKick.ExecutionTime);
            sensor.AddObservation(_actions.FrontKick.IsSuccess);
        
            sensor.AddObservation(_actions.BackKick.BaseDamage);
            sensor.AddObservation(_actions.BackKick.AttackDistance);
            sensor.AddObservation(_actions.BackKick.ExecutionTime);
            sensor.AddObservation(_actions.BackKick.IsSuccess);
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            if (_characterStartingPosition.x < 0 && _character.transform.position.x > _opponent.transform.position.x)
            {
                Reset();
            }
        
            if (_characterStartingPosition.x > 0 && _character.transform.position.x < _opponent.transform.position.x)
            {
                Reset();
            }

            if (Mathf.FloorToInt(vectorAction[0]) == 1)
            {
                MoveRight();
                _requester.TakeActionsBetweenDecisions = true;
            }
        
            else if (Mathf.FloorToInt(vectorAction[0]) == 2)
            {
                MoveLeft();
                _requester.TakeActionsBetweenDecisions = true;
            }
        
            else if (Mathf.FloorToInt(vectorAction[0]) == 3)
            {
                FrontPunch();
                if (Vector3.Distance(_character.transform.position, _opponent.transform.position) >
                    _actions.FrontPunch.AttackDistance)
                {
                    AddReward(-0.1f);
                }
                _requester.TakeActionsBetweenDecisions = false;
            }
        
            else if (Mathf.FloorToInt(vectorAction[0]) == 4)
            {
                BackPunch();
                if (Vector3.Distance(_character.transform.position, _opponent.transform.position) >
                    _actions.BackPunch.AttackDistance)
                {
                    AddReward(-0.1f);
                }
                _requester.TakeActionsBetweenDecisions = false;
            }
        
            else if (Mathf.FloorToInt(vectorAction[0]) == 5)
            {
                FrontKick();
                if (Vector3.Distance(_character.transform.position, _opponent.transform.position) >
                    _actions.FrontKick.AttackDistance)
                {
                    AddReward(-0.1f);
                }
                _requester.TakeActionsBetweenDecisions = false;
            }
        
            else if (Mathf.FloorToInt(vectorAction[0]) == 6)
            {
                BackKick();
                if (Vector3.Distance(_character.transform.position, _opponent.transform.position) >
                    _actions.BackKick.AttackDistance)
                {
                    AddReward(-0.1f);
                }
                _requester.TakeActionsBetweenDecisions = false;
            }
        
            else if (Mathf.FloorToInt(vectorAction[0]) == 7)
            {
                Block();
                _requester.TakeActionsBetweenDecisions = false;
            }

            if (_character.CurrentHealth < _healthBefore)
            {
                AddReward(-((_healthBefore - _character.CurrentHealth)/10));
                _healthBefore = _character.CurrentHealth;
            }
            if (_opponent.CurrentHealth < _opponentHealthBefore)
            {
                AddReward((_opponentHealthBefore - _opponent.CurrentHealth)/10);
                _opponentHealthBefore = _opponent.CurrentHealth;
            }

            if (_character.CurrentHealth <= 0)
            {
                AddReward(-1);
                EndEpisode();
            }
            else if (_opponent.CurrentHealth <= 0)
            {
                AddReward(1);
                EndEpisode();
            }
        }

        private void MoveRight()
        {
            _character.IsBlocking = false;
            _actions.Move(1, _character.Controller, _character.MovementSpeed);
        }
        private void MoveLeft()
        {
            _character.IsBlocking = false;
            _actions.Move(-1, _character.Controller, _character.MovementSpeed);
        }
        private void FrontPunch()
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.FrontPunch);
        }
        private void BackPunch()
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.BackPunch);
        }
        private void FrontKick()
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.FrontKick);
        }
        private void BackKick()
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.BackKick);
        }
        private void Block()
        {
            _character.IsBlocking = true;
        }

        private void Reset()
        {
            _character.transform.position = _characterStartingPosition;
            _opponent.transform.position = _opponentStartingPosition;
            _actions.ResetIsCompleted();        
        }
    
        public override void OnEpisodeBegin()
        {
            if (_rlTraining != null && _rlTraining.IsRlTraining)
            {
                _actions = _rlTraining.ActionsInstance;
            }
            else
            {
                _actions = Actions.Instance;
                return;
            }
            _character.HealthBar.ResetHealth();
            _opponent.HealthBar.ResetHealth();
            Reset();
        }

        public override void Heuristic(float[] actionsOut)
        {
            actionsOut[0] = 0;

            if (Input.GetKey(KeyCode.D))
            {
                actionsOut[0] = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                actionsOut[0] = 2;
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                actionsOut[0] = 3;
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                actionsOut[0] = 4;
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                actionsOut[0] = 5;
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                actionsOut[0] = 6;
            }
            else if (Input.GetKey(KeyCode.O))
            {
                actionsOut[0] = 7;
            }
        }
    }
}
