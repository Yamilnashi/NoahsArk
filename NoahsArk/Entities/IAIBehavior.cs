using Microsoft.Xna.Framework;

namespace NoahsArk.Entities
{
    public interface IAIBehavior
    {
        void Update(Enemy enemy, GameTime gameTime);
    }
}
