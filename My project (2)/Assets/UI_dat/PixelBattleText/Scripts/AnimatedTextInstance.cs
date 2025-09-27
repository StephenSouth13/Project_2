using UnityEngine;
using System;
using TMPro;

namespace PixelBattleText
{
	[Serializable]
    public class AnimatedTextInstace
    {
        public Transform[] letterTransforms;
        public TMP_Text[][] letters;
        public TextAnimation props;
        public float startTime;
        public Vector2 pos;
        public bool animationFinished;
        public bool active;
    }
}