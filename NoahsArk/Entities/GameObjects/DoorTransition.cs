using Microsoft.Xna.Framework;
using NoahsArk.Controls;
using NoahsArk.Levels;

namespace NoahsArk.Entities.GameObjects
{
    public class DoorTransition
    {
        #region Fields
        private Rectangle _triggerArea;
        private EMapCode _targetMap;
        private Vector2 _spawnPosition;
        private EDirection _direction;
        #endregion

        #region Properties
        public Rectangle TriggerArea { get { return _triggerArea; } }
        public EMapCode TargetMap { get { return _targetMap; } }
        public Vector2 SpawnPosition { get { return _spawnPosition; } }
        public EDirection Direction { get { return _direction; } }
        #endregion

        #region Constructor
        public DoorTransition(Rectangle triggerArea, EMapCode targetMap, Vector2 spawnPosition, EDirection direction)
        {
            _triggerArea = triggerArea;
            _targetMap = targetMap;
            _spawnPosition = spawnPosition;
            _direction = direction;
        }
        #endregion

        #region Methods
        #endregion
    }
}
