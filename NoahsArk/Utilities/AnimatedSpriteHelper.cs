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
                    for (int k = 0; k < animationData.EquipmentSlotTextureFilePaths.Keys.Count; k++)
                    {
                        EDirection direction = animationData.EquipmentSlotTextureFilePaths.Keys.ElementAt(k);
                        animations[animationKey][direction] = new Dictionary<EEquipmentSlot, AnimatedSprite>();
                        for (int l = 0; l < animationData.EquipmentSlotTextureFilePaths[direction].Keys.Count; l++)
                        {
                            EEquipmentSlot slot = animationData.EquipmentSlotTextureFilePaths[direction].Keys.ElementAt(l);
                            string slotFileName = animationData.EquipmentSlotTextureFilePaths[direction][slot];
                            animations[animationKey][direction][slot] = CreateAnimatedSprite(content, animationData, slotFileName);
                        }
                    }
                }
            }
            return animations;
        }
        #region Private
        private static AnimatedSprite CreateAnimatedSprite(ContentManager content, AnimationData animationData, string slotFileName)
        {
            string completeFilePath = Path.Combine(animationData.TextureFilePathBase, slotFileName);
            string formattedFilePath = DirectoryPathHelper.GetFormattedFilePath(completeFilePath);
            Texture2D texture = content.Load<Texture2D>(DirectoryPathHelper.GetFormattedFilePath(formattedFilePath));
            AnimatedSprite sprite = new AnimatedSprite(texture, animationData.FrameCount, animationData.FrameWidth, 
                animationData.FrameHeight, animationData.FrameDuration);
            return sprite;
        }
        #endregion
    }
}
