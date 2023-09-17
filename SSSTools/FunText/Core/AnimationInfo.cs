using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SSSTools.FunText.Effects;
using UnityEngine;

namespace SSSTools.FunText.Core
{
    public class AnimationInfo
    {
        readonly ReadOnlyCollection<int> _invisibleCharacterIndices;
        
        public TextBounds Bounds { get; private set; }
        
        public TextBounds[] WordBounds { get; private set; }

        public float T { get; private set; }
        
        public float TimePassed { get; private set; }

        // TODO: Refactor 
        // Idea: Have the character info here
        public TextAnimation TextAnimation { get; }
        
        public int Start { get; }
        
        public int End { get; }

        /// <summary>
        /// The amount of invisible characters between end and start
        /// </summary>
        public int InvisibleCharacters => _invisibleCharacterIndices.Count;

        /// <summary>
        /// The length in characters of this animation
        /// </summary>
        public int CharacterCount => End - Start + 1;

        /// <summary>
        /// The length but ignoring invisible characters
        /// </summary>
        public int CountNoInvisibleCharacters => CharacterCount - InvisibleCharacters;

        public AnimationInfo(int start, int end, List<int> invisibleChars, TextAnimation textAnimation)
        {
            Start = start;
            End = end;
            _invisibleCharacterIndices = invisibleChars.AsReadOnly();
            TextAnimation = textAnimation;
        }

        public int GetIndexWithoutInvisibleChars(int index, out int wordIndex)
        {
            return GetIndexWithoutInvisibleCharsFromReal(index + Start, out wordIndex) - Start;
        }

        public int GetIndexWithoutInvisibleCharsFromReal(int index, out int wordIndex)
        {
            wordIndex = 0;

            for (int invIndex = 0; invIndex < _invisibleCharacterIndices.Count; invIndex++)
            {
                if (index < _invisibleCharacterIndices[invIndex])
                    return index;

                index--;
                wordIndex++;
            }

            return index;
        }

        /// <summary>
        /// The normalized value of index. How far from end is it
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetNormalized(int index) => index / ((float) CharacterCount - 1);

        public float GetNormalizedFromReal(int index) => GetNormalized(index - Start);

        /// <summary>
        /// The normalized value of index but ignoring invisible characters
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetNormalizedNoInvisible(int index) =>
            GetIndexWithoutInvisibleChars(index, out int _) / ((float) CountNoInvisibleCharacters - 1);

        public float GetNormalizedNoInvisibleFromReal(int index) => GetNormalizedNoInvisible(index - Start);

        public void Play(ref CharacterData[] data)
        {
            UpdateT();
            TextAnimation.Play(data, this);
        }
        
        void UpdateT()
        {
            TimePassed += TextAnimation.DeltaTime.GetDeltaTime();
            float newT = TextAnimation.AnimationType.ComputeT(TimePassed * TextAnimation.Speed);

            TextAnimation.Duration.Update(TimePassed, TextAnimation.Speed);

            if (TextAnimation.Duration.IsComplete())
            {
                T = 0;
                return;
            }

            T = newT;
        }

        internal void ComputeBounds(CharacterData[] data)
        {
            int start = Start;

            if (End >= data.Length)
            {
                Debug.LogError("End of animation bigger than data available");
                return;
            }

            // TODO: Proper bounds by checking min and max X and Y
            Bounds = GetBounds(Start, End);

            List<TextBounds> wordsBounds = new List<TextBounds>(_invisibleCharacterIndices.Count + 1);
            foreach (int index in _invisibleCharacterIndices)
            {
                wordsBounds.Add(GetBounds(start, index - 1));
                start = index + 1;
            }
            
            wordsBounds.Add(GetBounds(start, End));

            WordBounds = wordsBounds.ToArray();

            Vector3 GetBotLeft(int index) => data[index].SourceVertices[0];
            Vector3 GetTopRight(int index) => data[index].SourceVertices[2];
            TextBounds GetBounds(int startIndex, int endIndex) => new TextBounds(GetBotLeft(startIndex), GetTopRight(endIndex));
        }
    }

    public readonly struct TextBounds
    {
        public readonly Vector3 BotLeft;
        public readonly Vector3 TopRight;

        public TextBounds(Vector3 botLeft, Vector3 topRight)
        {
            BotLeft = botLeft;
            TopRight = topRight;
        }
    }
}