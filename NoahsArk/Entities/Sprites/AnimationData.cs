using System.Collections.Generic;
using Newtonsoft.Json;
using NoahsArk.Controls;

namespace NoahsArk.Entities.Sprites
{
    public class AnimationData
    {
        #region Fields
        private int _frameCount;
        private int _frameWidth;
        private int _frameHeight;
        private float _frameDuration;
        private string _textureFilePathBase;
        private Dictionary<EDirection, Dictionary<EEquipmentSlot, string>> _equipmentSlotTextureFilePaths;
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
        [JsonProperty("textureFilePathBase")]
        public string TextureFilePathBase { get { return _textureFilePathBase; } set { _textureFilePathBase = value; } }
        [JsonProperty("equipmentSlotTextureFilePaths")]
        public Dictionary<EDirection, Dictionary<EEquipmentSlot, string>> EquipmentSlotTextureFilePaths {  get { return _equipmentSlotTextureFilePaths; } set { _equipmentSlotTextureFilePaths = value; } }
        #endregion

        #region Constructor
        public AnimationData() {}
        #endregion
    }
}
