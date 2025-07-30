namespace NoahsArk.Entities
{
    public class Gold : Item
    {
        #region Fields
        private int _amount;
        private string _name = "Gold";
        #endregion

        #region Properties
        public int Amount { get { return _amount; } set { _amount = value; } }
        public override string Name => _name;
        #endregion
    }
}
