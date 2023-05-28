using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDBattler.Runtime
{
    public static class ResourceManager
    {
        public static Sprite GetSprite(string filePath, string subFileName = "")
        {
            if (string.IsNullOrEmpty(subFileName)) return Resources.Load<Sprite>(filePath);

            Sprite[] sprites = Resources.LoadAll<Sprite>(filePath);
            foreach (var sprite in sprites)
            {
                if (sprite.name == subFileName) return sprite;
            }

            return null;
        }
    }
}
