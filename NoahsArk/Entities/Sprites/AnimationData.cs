using System.Collections.Generic;
using Newtonsoft.Json;
using NoahsArk.Controls;

namespace NoahsArk.Entities.Sprites
{
    public class AnimationData
    {
        #region Fields        
        private string _textureFilePathBase;
        private Dictionary<EDirection, AnimationFrameData> _directionAnimationData;
        #endregion

        #region Properties        
        [JsonProperty("textureFilePathBase")]
        public string TextureFilePathBase { get { return _textureFilePathBase; } set { _textureFilePathBase = value; } }
        [JsonProperty("directionAnimationData")]
        public Dictionary<EDirection, AnimationFrameData> DirectionAnimationData {  get { return _directionAnimationData; } set { _directionAnimationData = value; } }
        #endregion

        #region Constructor
        public AnimationData() {}
        #endregion
    }
}
