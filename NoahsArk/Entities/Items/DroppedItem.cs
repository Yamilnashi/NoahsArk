using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoahsArk.Entities.Items
{
    public class DroppedItem
    {
        #region Fields
        private Item _item;
        private int _quantity;
        private Vector2 _position;
        private Texture2D _texture;
        #endregion

        #region Properties
        public Item Item { get { return _item; } }
        public Texture2D Texture { get { return _texture; } }
        public Vector2 Position { get { return _position; } }
        public int Quantity { get { return _quantity; } }   
        #endregion

        #region Constructor
        public DroppedItem(Item item, int quantity, Vector2 position, Texture2D texture)
        {
            _item = item;
            _quantity = quantity;
            _position = position;
            _texture = texture;
        }
        #endregion
    }
}
