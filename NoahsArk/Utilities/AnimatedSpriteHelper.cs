using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NoahsArk.Controls;
using NoahsArk.Entities;
using NoahsArk.Entities.Sprites;

namespace NoahsArk.Utilities
{
    public class AnimatedSpriteHelper
    {
        public static Dictionary<EAnimationKey, Dictionary<EDirection, Dictionary<EEquipmentSlot, AnimatedSprite>>> GetAnimationData(ContentManager content, Dictionary<EAnimationType, Dictionary<EAnimationKey, AnimationData>> data)
        {
            Dictionary<EAnimationKey, Dictionary<EDirection, Dictionary<EEquipmentSlot, AnimatedSprite>>> animations = new Dictionary<EAnimationKey, Dictionary<EDirection, Dictionary<EEquipmentSlot, AnimatedSprite>>>();
            for (int i = 0; i < data.Keys.Count; i++)
            {
                EAnimationType animationType = data.Keys.ElementAt(i);
                for (int j = 0; j < data[animationType].Keys.Count; j++)
                {
                    EAnimationKey animationKey = data[animationType].Keys.ElementAt(j);
                    animations.Add(animationKey, new Dictionary<EDirection, Dictionary<EEquipmentSlot, AnimatedSprite>>());
                    AnimationData animationData = data[animationType][animationKey];
                    for (int k = 0; k < animationData.DirectionAnimationData.Keys.Count; k++)
                    {
                        EDirection direction = animationData.DirectionAnimationData.Keys.ElementAt(k);
                        animations[animationKey][direction] = new Dictionary<EEquipmentSlot, AnimatedSprite>();
                        AnimationFrameData frameData = animationData.DirectionAnimationData[direction];
                        for (int l = 0; l < frameData.EquipmentSlotTextureFilePaths.Keys.Count; l++)
                        {
                            EEquipmentSlot slot = frameData.EquipmentSlotTextureFilePaths.Keys.ElementAt(l);
                            string slotFileName = frameData.EquipmentSlotTextureFilePaths[slot];
                            animations[animationKey][direction][slot] = CreateAnimatedSprite(content, frameData, animationData.TextureFilePathBase, slotFileName);
                        }
                    }
                }
            }
            return animations;
        }
        #region Private
        private static AnimatedSprite CreateAnimatedSprite(ContentManager content, AnimationFrameData frameData, string textureFilePathBase, string slotFileName)
        {
            string completeFilePath = Path.Combine(textureFilePathBase, slotFileName);
            string formattedFilePath = DirectoryPathHelper.GetFormattedFilePath(completeFilePath);
            Texture2D texture = content.Load<Texture2D>(DirectoryPathHelper.GetFormattedFilePath(formattedFilePath));
            AnimatedSprite sprite = new AnimatedSprite(texture, frameData.FrameCount, frameData.FrameWidth, 
                frameData.FrameHeight, frameData.FrameDuration, frameData.HitboxOffsetX, frameData.HitboxOffsetY,
                frameData.HitFrame);
            return sprite;
        }
        #endregion
    }
}
