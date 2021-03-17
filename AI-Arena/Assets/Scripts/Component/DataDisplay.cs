using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Component
{
    public class DataDisplay : MonoBehaviour
    {
        private float _timeToWait = 0.3f;
        private float _currentTime;

        private bool _isShowingData;

        private void Start()
        {
            _isShowingData = false;
        }
        public void SetData(Text text, string data)
        {
            ShowAttackData(text, data + "!");
        }
        
        // private IEnumerator ShowAttackData(Text text, string data, float waitSeconds)
        // {
        //     yield return new WaitForSeconds(waitSeconds);
        //     text.text = data;
        //
        //     text.CrossFadeAlpha(1, 0, false);
        //     yield return new WaitForSeconds(0.3f);
        //     text.CrossFadeAlpha(0, 1, false);
        // }
        
        private void ShowAttackData(Text text, string data)
        {
            if (_isShowingData)
            {
                _isShowingData = false;
            }
            
            text.text = data;

            text.CrossFadeAlpha(1, 0, false);
            _isShowingData = true;
            text.CrossFadeAlpha(0, 1, false);
        }

        private void Update()
        {
            if (_isShowingData)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime >= _timeToWait)
                {
                    _isShowingData = false;
                }
            }
            else
            {
                _currentTime = 0;
            }
        }
    }
}
