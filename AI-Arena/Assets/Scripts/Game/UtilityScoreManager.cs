using System.Collections.Generic;
using Game;
using RLAgent;
using UnityEngine;
using Random = UnityEngine.Random;

public class UtilityScoreManager : MonoBehaviour
{
    private CharacterSettings _character;
    private CharacterSettings _opponent;
    private Actions _actions;
    private TrainingEnvironmentInstanceSettings _rlTraining;
    private CharacterController _controller;
    
    private List<float> _scores;
    private List<float> _scoresOverZero;

    private float _frontPunchScore;
    private float _backPunchScore;
    private float _frontKickScore;
    private float _backKickScore;

    private float _blockScore;

    private float _moveLeftScore;
    private float _moveRightScore;
    private float _moveNotScore;

    private float _currentActionScore;

    private int _scoreIndex;

    // private float _rangeScore;
    
    public float FrontPunchScore => _frontPunchScore;
    public float BackPunchScore => _backPunchScore;
    public float FrontKickScore => _frontKickScore;
    public float BackKickScore => _backKickScore;

    public float CurrentActionScore => _currentActionScore;
    
    private void Start()
    {
        _rlTraining = GetComponentInParent<TrainingEnvironmentInstanceSettings>();
        _character = gameObject.GetComponent<CharacterSettings>();
        _controller = _character.GetComponent<CharacterController>();
        _opponent = _character.Opponent;
        
        if (_rlTraining != null && _rlTraining.IsRlTraining)
        {
            _actions = _rlTraining.ActionsInstance;
        }
        else
        {
            _actions = Actions.Instance;
        }
        
        _scores = new List<float>();
        _scoresOverZero = new List<float>();
        
        EvaluateScore();
    }

    private void CalculateScore()
    {
        if (Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition) >
            _actions.FrontPunch.AttackDistance)
        {
            _frontPunchScore = 0;
        }
        else
        {
            if (!_opponent.IsCountered)
            {
                _frontPunchScore = Remap(_actions.FrontPunch.TotalDamageAmount + _actions.FrontPunch.ExecutionTime, 0,
                    3, 0, 1);
            }
            else
            {
                _frontPunchScore = Remap((_actions.FrontPunch.TotalDamageAmount + _actions.FrontPunch.ExecutionTime) * 1.5f, 0,
                    3, 0, 1);
            }
        }

        if (Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition) >
            _actions.BackPunch.AttackDistance)
        {
            _backPunchScore = 0;
        }
        else
        {
            if (!_opponent.IsCountered)
            {
                _backPunchScore = Remap(_actions.BackPunch.TotalDamageAmount + _actions.BackPunch.ExecutionTime, 0, 3,
                    0, 1);
            }
            else
            {
                _backPunchScore = Remap((_actions.BackPunch.TotalDamageAmount + _actions.BackPunch.ExecutionTime) * 1.5f, 0, 3,
                    0, 1);
            }
        }

        if (Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition) >
            _actions.FrontKick.AttackDistance)
        {
            _frontKickScore = 0;
        }
        else
        {
            if (!_opponent.IsCountered)
            {
                _frontKickScore = Remap(_actions.FrontKick.TotalDamageAmount + _actions.FrontKick.ExecutionTime, 0, 3,
                    0, 1);
            }
            else
            {
                _frontKickScore = Remap((_actions.FrontKick.TotalDamageAmount + _actions.FrontKick.ExecutionTime) * 1.5f, 0, 3,
                    0, 1);
            }
        }

        if (Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition) >
            _actions.BackKick.AttackDistance)
        {
            _backKickScore = 0;
        }
        else
        {
            if (!_opponent.IsCountered)
            {
                _backKickScore = Remap(_actions.BackKick.TotalDamageAmount + _actions.BackKick.ExecutionTime, 0, 4, 0,
                    1);
            }
            else
            {
                _backKickScore = Remap((_actions.BackKick.TotalDamageAmount + _actions.BackKick.ExecutionTime) * 1.5f, 0, 4, 0,
                    1);
            }
        }
        
        _blockScore = (1 - _character.CurrentHealth) - (_character.BlockTime/2);

        if (gameObject.transform.localPosition.x > _opponent.transform.localPosition.x)
        {
            _moveLeftScore = Remap(Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition), 0, 20, 0, 1);
            _moveRightScore = Remap((20 - Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition)) * (1 - _character.CurrentHealth), 0, 20, 0, 1);
        }
        else
        {
            _moveLeftScore = Remap((20 - Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition)) * (1 - _character.CurrentHealth), 0, 20, 0, 1);
            _moveRightScore = Remap(Vector3.Distance(gameObject.transform.localPosition, _opponent.transform.localPosition), 0, 20, 0, 1);
        }
        // Debug.Log("Front Punch Score: " + _frontPunchScore);
        // Debug.Log("Back Punch Score: " + _backPunchScore);
        // Debug.Log("Front Kick Score: " + _frontKickScore);
        // Debug.Log("Back Kick Score: " + _backKickScore);
        // Debug.Log("Block Score: " + _blockScore);
        // Debug.Log("Move Left Score: " + _moveLeftScore);
        // Debug.Log("Move Right Score: " + _moveRightScore);
        // Debug.Log("Move Not Score: " + _moveNotScore);

        _moveNotScore = Random.Range(0f, 0.8f);
    }
    
    //wolfram alpha / plot graph online / desmos

    public void EvaluateScore()
    {
        CalculateScore();

        _scores.Add(_frontPunchScore);
        _scores.Add(_backPunchScore);
        _scores.Add(_frontKickScore);
        _scores.Add(_backKickScore);
        _scores.Add(_blockScore);
        _scores.Add(_moveLeftScore);
        _scores.Add(_moveRightScore);
        _scores.Add(_moveNotScore);
        
        _scores.Sort();
        _scores.Reverse();

        foreach (float score in _scores)
        {
            if (score > 0)
            {
                _scoresOverZero.Add(score);
            }

            if (_scoresOverZero.Count >= 2)
            {
                if (_scoresOverZero[0] - _scoresOverZero[1] > 0.25f)
                {
                    _scoresOverZero.Remove(_scoresOverZero[1]);
                }
            }

            if (_scoresOverZero.Count >= 3)
            {
                if (_scoresOverZero[0] - _scoresOverZero[2] > 0.5f)
                {
                    _scoresOverZero.Remove(_scoresOverZero[2]);
                }
            }
        }

        if (_scoresOverZero.Count == 1)
        {
            _scoreIndex = 0;
        }

        else if (_scoresOverZero.Count == 2)
        {
            _scoreIndex = Random.Range(0, 1);
        }

        else
        {
            _scoreIndex = Random.Range(0, 2);
        }

        _currentActionScore = _scores[_scoreIndex];

        _scores.Clear();
    }

    public void ExecuteByScore(float score)
    {
        if (Mathf.Approximately(score, _frontPunchScore))
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.FrontPunch);
        }
        else if (Mathf.Approximately(score, _backPunchScore))
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.BackPunch);
        }
        else if (Mathf.Approximately(score, _frontKickScore))
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.FrontKick);
        }
        else if (Mathf.Approximately(score, _backKickScore))
        {
            _character.IsBlocking = false;
            _character.DoAttack(_actions.BackKick);
        }
        else if (Mathf.Approximately(score, _blockScore))
        {
            _character.IsBlocking = true;
        }
        else if (Mathf.Approximately(score, _moveLeftScore))
        {
            _character.IsBlocking = false;
            _actions.Move(-1, _controller, _character.MovementSpeed);
        }
        else if (Mathf.Approximately(score, _moveRightScore))
        {
            _character.IsBlocking = false;
            _actions.Move(1, _controller, _character.MovementSpeed);
        }
        else if (Mathf.Approximately(score, _moveNotScore))
        {
            _actions.Move(0, _controller, _character.MovementSpeed);
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
