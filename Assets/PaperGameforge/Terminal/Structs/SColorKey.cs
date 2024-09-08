using System;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [Serializable]
    public struct SColorKey
    {
        [SerializeField] private string key;
        [SerializeField] private Color color;

        public string Key { get => key; }
        public Color Color { get => color; }

        public SColorKey(string key, Color color)
        {
            this.key = key;
            this.color = color;
        }
    }
}
