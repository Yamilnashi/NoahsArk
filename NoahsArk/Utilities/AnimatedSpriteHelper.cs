using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities.Sprites;
using System.Collections.Generic;
using System.Linq;

namespace NoahsArk.Utilities
{
    public class AnimatedSpriteHelper
    {
        public static Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> GetAnimationData(ContentManager content, Dictionary<EAnimationKey, Dictionary<EDirection, AnimationData>> data)
        {
            Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>> animations = new Dictionary<EAnimationKey, Dictionary<EDirection, AnimatedSprite>>();
            for (int i = 0; i < data.Count; i++)
            {
                EAnimationKey key = data.Keys.ElementAt(i);
                animations.Add(key, new Dictionary<EDirection, AnimatedSprite>());
                for (int j = 0; j < data[key].Count; j++)
                {
                    EDirection direction = data[key].Keys.ElementAt(j);
                    AnimationData animationData = data[key][direction];
                    string formattedFilePath = DirectoryPathHelper.GetFormattedFilePath(animationData.TextureFilePath);
                    Texture2D texture = content.Load<Texture2D>(formattedFilePath);
                    AnimatedSprite sprite = new AnimatedSprite(texture, animationData.FrameCount,
                        animationData.FrameWidth, animationData.FrameHeight, animationData.FrameDuration);
                    animations[key].Add(direction, sprite);
                }
            }
            return animations;
        }
    }
}
