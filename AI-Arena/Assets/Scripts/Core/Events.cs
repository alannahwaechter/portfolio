namespace Core
{
    public class Events
    {
        public delegate void DamageEvent(float amount);
        public static event DamageEvent OnDamage;
    
        public delegate void SceneChangeEvent();
        public static event SceneChangeEvent OnSceneChanged;

        public delegate void AttackEndedEvent();
        public static event AttackEndedEvent OnAttackEnded;

        public static void InvokeDamageEvent(float amount)
        {
            OnDamage?.Invoke(amount/100);
        }

        public static void InvokeSceneChangeEvent()
        {
            OnSceneChanged?.Invoke();
        }

        public static void InvokeAttackEndedEvent()
        {
            OnAttackEnded?.Invoke();
        }
    }
}
