using Microsoft.Xna.Framework;

namespace NoahsArk.Entities
{
    public class GoldTier
    {
        #region Fields
        private int _minAmount;
        private int _maxAmount;
        private Rectangle _spriteRectangle;
        #endregion

        #region Properties
        public Rectangle SpriteRectangle { get { return _spriteRectangle; } }   
        #endregion

        #region Constructor
        public GoldTier(int minAmount, int maxAmount, Rectangle spriteRectangle)
        {
            _minAmount = minAmount;
            _maxAmount = maxAmount;
            _spriteRectangle = spriteRectangle;
        }
        #endregion

        #region Methods
        public bool IsInRange(int amount)
        {
            return amount >= _minAmount && amount <= _maxAmount;
        }
        #endregion
    }
}
