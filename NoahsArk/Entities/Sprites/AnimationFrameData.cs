using System.Collections.Generic;
using Newtonsoft.Json;
using NoahsArk.Entities.Items;
using NoahsArk.Entities.Items.Weapons;

namespace NoahsArk.Entities.Sprites
{
    public class AnimationFrameData
    {
        #region Fields
        private int _frameCount;
        private int _frameWidth;
        private int _frameHeight;
        private float _frameDuration;
        private float _hitboxOffsetX;
        private float _hitboxOffsetY;
        private int _hitFrame;
        private Dictionary<EEquipmentSlot, string> _equipmentSlotTextureFilePaths;
        #endregion

        #region Properties
        [JsonProperty("frameCount")]
        public int FrameCount { get { return _frameCount; } set { _frameCount = value; } }
        [JsonProperty("frameWidth")]
        public int FrameWidth { get { return _frameWidth; } set { _frameWidth = value; } }
        [JsonProperty("frameHeight")]
        public int FrameHeight { get { return _frameHeight; } set { _frameHeight = value; } }
        [JsonProperty("frameDuration")]
        public float FrameDuration { get { return _frameDuration; } set { _frameDuration = value; } }
        [JsonProperty("hitboxOffsetX")]
        public float HitboxOffsetX { get { return _hitboxOffsetX; } set { _hitboxOffsetX = value; } }
        [JsonProperty("hitboxOffsetY")]
        public float HitboxOffsetY { get { return _hitboxOffsetY; } set { _hitboxOffsetY = value; } }
        [JsonProperty("hitFrame")]
        public int HitFrame { get { return _hitFrame; } set { _hitFrame = value; } }      
        [JsonProperty("equipmentSlotTextureFilePaths")]
        public Dictionary<EEquipmentSlot, string> EquipmentSlotTextureFilePaths { get { return _equipmentSlotTextureFilePaths; } set { _equipmentSlotTextureFilePaths = value; } }
        #endregion

        #region Constructor
        public AnimationFrameData() { }
        #endregion
    }
}
