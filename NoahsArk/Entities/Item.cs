using System;

namespace NoahsArk.Entities
{
    public abstract class Item
    {
        #region Fields
        private string _name;
        private readonly Random _random = new Random();
        #endregion

        #region Properties
        public virtual string Name { get { return _name; } set { _name = value; } }    
        public Random Random { get { return _random; } }
        #endregion
    }
}
