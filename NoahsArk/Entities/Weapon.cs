namespace NoahsArk.Entities
{
    public abstract class Weapon : Item
    {
        public abstract float CalculateDamage(out bool isCrit);
    }
}
