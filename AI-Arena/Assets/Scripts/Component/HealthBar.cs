using Core;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Component
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;

        private float _currentHealth;

        private float _damageAmount;

        public float CurrentHealth => _currentHealth;

        private void Awake()
        {
            _currentHealth = 1;
        }

        public void ResetHealth()
        {
            _currentHealth = 1;
        }
    
        // public void AddOnDamageEvent()
        // {
        //     Events.OnDamage += OnDamageEvent;
        // }
        //
        // public void RemoveOnDamageEvent()
        // {
        //     Events.OnDamage -= OnDamageEvent;
        // }

        private void Update()
        {
            _healthBar.fillAmount = _currentHealth;
        }

        public void OnDamageEvent(float amount)
        {
            _currentHealth -= amount;
        }
    }
}
