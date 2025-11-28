namespace Core.Interfaces
{
    public interface IDamager
    {
        public IDamageable Target { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        public void Attack();
    }
}