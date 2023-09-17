using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSSTools.Extensions;
using SSSTools.FunText.Effects;
using TMPro;
using UnityEngine;

namespace SSSTools.FunText.Core
{
    /// <summary>
    /// Component to animate the text from the text mesh pro component attached to this game object
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    [AddComponentMenu("SSSTools/FunText/Animator")]
    [ExecuteAlways]
    [DefaultExecutionOrder(1)] // TODO: Should keep the execution order??
    public class FunTextAnimator : MonoBehaviour
    {
        #region Classes, structs and enums

        enum TagOptions
        {
            Create,
            CloseLastSameTag,
            CloseLastAdded,
            CloseAll,
            CLoseAllSameTag,
            EnableNoParse,
            DisableNoParse
        }

        struct AnimationDetails
        {
            public TextAnimation Animation;
            public int Start;
            public List<int> InvisibleChars;

            public AnimationDetails(int start, TextAnimation animation)
            {
                Start = start;
                InvisibleChars = new List<int>();
                Animation = animation;
            }
        }

        [Serializable]
        class CursorDetails
        {
            [SerializeField] internal float blinkRate = 3f;
            [SerializeField] internal bool show = true;
            [SerializeField] internal char cursorChar = '|';

            [SerializeField, Tooltip("How long to wait for the cursor to be considered inactive and start animating")]
            internal float timeToInactive = 1f;

            public float OffsetFromDescender { get; }

            public float TimeInactive { get; set; }

            public bool IsInactive => TimeInactive >= timeToInactive;

            public CursorDetails(float offsetFromDescender)
            {
                OffsetFromDescender = offsetFromDescender;
            }
        }

        struct CharacterEffectDetails
        {
            public ShowCharacterEffect Eff;
            public List<int> CharIndex;

            public CharacterEffectDetails(ShowCharacterEffect eff, int charIndex)
            {
                Eff = eff;
                CharIndex = new List<int>() {charIndex};
            }
        }

        #endregion

        #region Variables

        TMP_Text _tmpro;
        TMP_TextInfo _tmproInfo;

        string _textTags;
        string _text;
        int _visibleChars;

        CharacterData[] _characterData;
        Dictionary<int, IEnumerator> _waiters;
        Dictionary<int, CustomTyperEvent> _events;

        List<AnimationInfo> _animationInfos;

        [SerializeField] List<TextAnimationDataObject> globalAnimations;
        [SerializeField] List<TextAnimation> localAnimations;
        CharacterData[] _validData;

        [SerializeField] CursorDetails cursorDetails;

        #endregion

        #region Properties

        public CharacterData Cursor => _validData[CharacterCount];

        public int CharacterCount => _validData.Length - 1;

        #endregion

        #region Methods

        #region Public

        public void SetAllVisible()
        {
            SetVisibleCharacters(CharacterCount);
        }

        public void SetVisibleCharacters(int number)
        {
            if (number <= _validData.Length)
                _visibleChars = number;

            UpdateCursorPosition();
            // TODO: Finish get all characters visible
        }

        public bool SetNextCharVisible(out char c)
        {
            if (_visibleChars >= _validData.Length - 1)
            {
                c = ' ';
                //Cursor.Reset();
                //ShowCursor();
                return false;
            }

            _validData[_visibleChars].TimeHidden = 0;
            CharacterData charData = _validData[_visibleChars++];
            TMP_CharacterInfo charInfo = charData.Info;

            UpdateCursorPosition();

            if (!charInfo.isVisible)
            {
                c = ' ';
                return true;
            }

            c = charInfo.character;

            return true;
        }

        public void HideAllCharacters()
        {
            while (HideNextCharacter(out _))
            {
            }
        }

        public bool HideNextCharacter(out char hidden)
        {
            if (_visibleChars == 0)
            {
                hidden = '\0';
                return false;
            }

            _visibleChars--;

            hidden = _validData[_visibleChars].Info.character;

            _validData[_visibleChars].TimeVisible = 0;

            // No effects. Hide immediately
            if (_validData[_visibleChars].OnBecomeVisibleEffects.Count == 0)
            {
                for (int v = 0; v < _validData[_visibleChars].Colors.Length; v++)
                {
                    _validData[_visibleChars].Vertices[v] = Vector3.zero;
                    _validData[_visibleChars].Colors[v] = Color.clear;
                }
            }

            UpdateCursorPosition();
            return true;
        }

        public bool TryGetWaiter(out IEnumerator waiter)
        {
            return _waiters.TryGetValue(_visibleChars, out waiter);
        }

        public bool TryGetEvent(out CustomTyperEvent @event)
        {
            return _events.TryGetValue(_visibleChars, out @event);
        }

        public void SetText(string text)
        {
            _tmpro.text = text;
            UpdateText();
        }

        public void ShowCursor()
        {
            Cursor.Reset();
            cursorDetails.show = true;
            UpdateCursorPosition();
        }

        public void HideCursor()
        {
            cursorDetails.show = false;
            for (int i = 0; i < Cursor.Colors.Length; i++)
            {
                Cursor.Colors[i] = Color.clear;
            }
        }

        #endregion

        #region Unity Messages

        void Awake()
        {
            _tmpro = GetComponent<TMP_Text>();
        }

        void Start()
        {
            // TODO: Remove unnecessary mesh updates

            //UpdateText();

            //StartCoroutine(ShowText());
        }

        void OnEnable()
        {
            UpdateText();
        }

        void Update()
        {
            if (!_text.Equals(_tmpro.text))
            {
                UpdateText();
            }

            // for (int i = 0; i < _tmproInfo.characterCount; i++)
            // {
            //     TMP_CharacterInfo character = _tmproInfo.characterInfo[i];
            //     
            //     if (!character.isVisible)
            //         continue;
            //     
            //     _characterData[i].Reset();
            // }

#if UNITY_EDITOR
            // If we recompile we will miss this references so we are checking and adding
            if (_tmpro == null || _tmproInfo == null)
                Awake();

            if (_characterData == null)
                UpdateText();
#endif

            for (int index = 0; index < _visibleChars; index++)
            {
                _validData[index].Reset();
                _validData[index].TimeVisible += Time.deltaTime; // TODO: Delta time
            }

            for (int index = _visibleChars; index < _validData.Length - 1; index++)
            {
                _validData[index].TimeHidden += Time.deltaTime; // TODO: Delta time
            }

            // TODO: Delta
            cursorDetails.TimeInactive += Time.deltaTime;

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                PlayAnimations();
            }
#else
            PlayAnimations();
#endif

            UpdateMesh();

            if (_tmpro.havePropertiesChanged)
                _tmpro.ForceMeshUpdate();
        }

        #endregion

        #region Private

        void UpdateMesh()
        {
            for (int i = 0; i < _characterData.Length; i++)
            {
                // TEST!!
                TMP_CharacterInfo charInfo = _tmproInfo.characterInfo[i];
                //TMP_CharacterInfo charInfo = _tmproInfo.characterInfo[_characterData[i].Index];

                if (!charInfo.isVisible)
                    continue;

                for (int j = 0; j < 4; j++)
                {
                    int materialIndex = charInfo.materialReferenceIndex;

                    _tmproInfo.meshInfo[materialIndex].vertices[charInfo.vertexIndex + j] =
                        _characterData[i].Vertices[j];
                    _tmproInfo.meshInfo[materialIndex].colors32[charInfo.vertexIndex + j] = _characterData[i].Colors[j];
                    _tmproInfo.meshInfo[materialIndex].uvs0[charInfo.vertexIndex + j] = _characterData[i].Uvs[j];
                    _tmproInfo.meshInfo[materialIndex].uvs2[charInfo.vertexIndex + j] = _characterData[i].Uvs2[j];
                }
            }

            // CURSOR TEST
            // int cursorIndex = _tmproInfo.characterCount;
            // TMP_CharacterInfo cursorInfo = _tmproInfo.characterInfo[cursorIndex];
            //
            // for (int j = 0; j < 4; j++)
            // {
            //     int cursorMaterialIndex = cursorInfo.materialReferenceIndex;
            //
            //     _tmproInfo.meshInfo[cursorMaterialIndex].vertices[cursorInfo.vertexIndex + j] = _characterData[cursorIndex - 1].Vertices[j];
            //     _tmproInfo.meshInfo[cursorMaterialIndex].colors32[cursorInfo.vertexIndex + j] = _characterData[cursorIndex - 1].Colors[j];
            // }


            _tmpro.UpdateVertexData();
        }

        void UpdateText()
        {
            _tmpro.ForceMeshUpdate(true);

            _animationInfos = new List<AnimationInfo>();
            _tmproInfo = _tmpro.textInfo;

            // TODO: Initialize on awake then only clearing to prevent memory waste
            bool hasTyper = TryGetComponent(out FunTextTyper typer);
            HashSet<ShowCharacterEffect> openDetails = new HashSet<ShowCharacterEffect>();
            Dictionary<ShowCharacterEffect, List<int>> charIndicesByEffect =
                new Dictionary<ShowCharacterEffect, List<int>>();
            _waiters = new Dictionary<int, IEnumerator>();
            _events = new Dictionary<int, CustomTyperEvent>();

            // TODO: MAKE IT INDICES ONLY
            List<CharacterData> tempData = new List<CharacterData>();
            ReadTags();

            Mesh mesh = _tmpro.mesh;

            StorePositions();
            AddCharEffects(); // TODO: Idea: Instead of showeff, list of indices store the effects for each index then on store positions add the effects

            ComputeAnimationBounds();

            Array.Resize(ref _tmproInfo.characterInfo, 512);

            _tmpro.text = _text + cursorDetails.cursorChar;
            _tmpro.ForceMeshUpdate();
            _tmpro.UpdateGeometry(mesh, 0);
            _tmpro.text = _text;

            // Increase count to leave space for cursor
            // _tmproInfo.characterCount++;
            // _tmproInfo.characterInfo[_tmproInfo.characterCount - 1] = Cursor.Info;
            //
            // int index = _tmproInfo.characterCount - 2;
            // while (index > 0 && !_tmproInfo.characterInfo[index].isVisible)
            // {
            //     index--;
            // }
            // if (index >= 0)
            //     _tmproInfo.characterInfo[_tmproInfo.characterCount - 1].vertexIndex = _tmproInfo.characterInfo[index].vertexIndex + 4;

            // if (_tmproInfo.characterCount > _tmproInfo.characterInfo.Length)
            // {
            //     Array.Resize(ref _tmproInfo.characterInfo, _tmproInfo.characterCount);
            //     _tmproInfo.characterInfo[^1] = Cursor.Info;
            // }

            _characterData = new CharacterData[_tmproInfo.characterCount];
            for (int i = 0, j = 0; i < _characterData.Length; i++)
            {
                if (j < tempData.Count && tempData[j].Index == i)
                {
                    _characterData[i] = _validData[j];
                    j++;
                    continue;
                }

                _characterData[i] = new CharacterData(new Vector3[4], new Color32[4], new Vector2[4], new Vector2[4],
                    _tmproInfo.characterInfo[i], i);
            }

            _characterData[CharacterCount] = _validData[CharacterCount];
            if (cursorDetails.show)
                Cursor.Reset();
            cursorDetails = new CursorDetails(Cursor.Info.topLeft.y - Cursor.Info.descender);

            InitializeAnimations();

            AddCharEffects();

            //visibleChars = _validData.Length;
            // TODO: Logic for whether we should show the text immediately or not
            SetVisibleCharacters(!Application.isPlaying || !hasTyper || !typer.enabled ? _validData.Length - 1 : 0);

            _tmpro.ForceMeshUpdate();

            #region Aux Methods

            void ReadTags()
            {
                StringBuilder finalText = new StringBuilder(_tmpro.text.Length);
                Dictionary<string, Stack<AnimationDetails>> animationsStack =
                    new Dictionary<string, Stack<AnimationDetails>>();

                int charIndex = 0;

                string lastAnimationTag = string.Empty;
                ShowCharacterEffect lastCharEffect = null;
                bool noParse = false;

                // Final text without tmp rich text tags. Allows proper counting of characters
                for (int tmpCharIndex = 0; tmpCharIndex < _tmproInfo.characterCount; tmpCharIndex++)
                {
                    TMP_CharacterInfo tmpCharacterInfo = _tmproInfo.characterInfo[tmpCharIndex];
                    char letter = _tmproInfo.characterInfo[tmpCharIndex].character;

                    // TODO: Refactor
                    switch (letter)
                    {
                        // Animation 
                        case '<':
                        {
                            int next = ReadTag(tmpCharIndex + 1, '>', out string effectTag);
                            if (effectTag.ToLower() == "np")
                            {
                                noParse = true;
                                tmpCharIndex = next;
                                break;
                            }

                            if (effectTag.ToLower() == "/np")
                            {
                                noParse = false;
                                tmpCharIndex = next;
                                break;
                            }

                            if (next > 0 &&
                                IsAnimationTagValid(effectTag, out TagOptions validTag, out TextAnimation textAnim) &&
                                !noParse)
                            {
                                // TODO: Interface with what to do or refactor
                                switch (validTag)
                                {
                                    case TagOptions.Create:
                                        if (animationsStack.TryGetValue(effectTag, out Stack<AnimationDetails> infos))
                                            infos.Push(new AnimationDetails(charIndex, textAnim));
                                        else
                                            animationsStack.Add(effectTag,
                                                new Stack<AnimationDetails>(new[]
                                                    {new AnimationDetails(charIndex, textAnim)}));
                                        lastAnimationTag = effectTag;
                                        break;
                                    case TagOptions.CloseLastSameTag:
                                        CloseLastAnimationWithTag(effectTag.Substring(1), charIndex - 1,
                                            effectTag); // TODO: Error messages in const
                                        break;
                                    case TagOptions.CloseLastAdded:
                                        CloseLastAnimationWithTag(lastAnimationTag, charIndex - 1, effectTag);
                                        break;
                                    case TagOptions.CloseAll:
                                        AddAll(charIndex - 1);
                                        break;
                                    case TagOptions.CLoseAllSameTag:
                                        AddEffects(charIndex - 1, effectTag.Substring(2));
                                        break;
                                    case TagOptions.EnableNoParse:
                                        noParse = true;
                                        break;
                                    case TagOptions.DisableNoParse:
                                        noParse = false;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                tmpCharIndex = next;

                                //_characterData[tmpCharIndex] = new CharacterData(new Vector3[4], new Color32[4], tmpCharacterInfo, tmpCharIndex);
                            }
                            else
                            {
                                AddToTempData(tmpCharacterInfo, tmpCharIndex);
                            }

                            break;
                        }
                        // Show character effect
                        case '[':
                        {
                            // TODO: Possible bug when there is a '[' without typer
                            if (!hasTyper)
                                break;

                            int next = ReadTag(tmpCharIndex + 1, ']', out string effectTag);
                            if (next > 0 && IsTyperEffectTagValid(effectTag, typer, out TagOptions validTag,
                                    out ShowCharacterEffect characterEffect) && !noParse)
                            {
                                // TODO: Interface with what to do or refactor
                                switch (validTag)
                                {
                                    case TagOptions.Create:
                                        openDetails.Add(characterEffect);

                                        if (charIndicesByEffect.TryGetValue(characterEffect, out List<int> indices))
                                        {
                                            indices.Add(charIndex);
                                        }
                                        else
                                        {
                                            charIndicesByEffect.Add(characterEffect, new List<int>());
                                        }

                                        lastCharEffect = characterEffect;
                                        break;
                                    case TagOptions.CloseLastSameTag:
                                        CloseLastCharEffectWithTag(characterEffect);
                                        break;
                                    case TagOptions.CloseLastAdded:
                                        CloseLastCharEffectWithTag(lastCharEffect);
                                        break;
                                    case TagOptions.CloseAll:
                                        CloseAllShowCharEffects();
                                        break;
                                    case TagOptions.CLoseAllSameTag:
                                        CloseLastCharEffectWithTag(characterEffect);
                                        break;
                                    case TagOptions.EnableNoParse:
                                    case TagOptions.DisableNoParse:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                tmpCharIndex = next;

                                //_characterData[tmpCharIndex] = new CharacterData(new Vector3[4], new Color32[4], tmpCharacterInfo, tmpCharIndex);
                            }
                            else if (next > 0 && IsWaitActionTag(effectTag, typer, out IEnumerator waiter) && !noParse)
                            {
                                _waiters.Add(charIndex, waiter);
                                tmpCharIndex = next;
                            }
                            else if (next > 0 && IsCustomTyperEvent(effectTag, typer, out CustomTyperEvent @event))
                            {
                                _events.Add(charIndex, @event);
                                tmpCharIndex = next;
                            }
                            else
                            {
                                AddToTempData(tmpCharacterInfo, tmpCharIndex);
                            }

                            break;
                        }
                        default:
                            // Increase invisible characters for the current open animations
                            if (!_tmproInfo.characterInfo[tmpCharIndex].isVisible)
                            {
                                foreach (Stack<AnimationDetails> detailsList in animationsStack.Values)
                                {
                                    // for (int index = 0; index < detailsList.Count; index++)
                                    // {
                                    //     AnimationDetails details = detailsList.ToArray()[index];
                                    //     details.InvisibleChars.Add(charIndex);
                                    //     detailsList[index] = details;
                                    // }

                                    foreach (AnimationDetails details in detailsList.ToArray())
                                    {
                                        details.InvisibleChars.Add(charIndex);
                                    }
                                }
                            }
                            else
                            {
                                foreach (ShowCharacterEffect fx in openDetails)
                                {
                                    charIndicesByEffect[fx].Add(charIndex);
                                }
                            }

                            /*_characterData[tmpCharIndex] = new CharacterData(
                                new[]
                                {
                                    tmpCharacterInfo.bottomLeft, tmpCharacterInfo.topLeft, tmpCharacterInfo.topRight,
                                    tmpCharacterInfo.bottomRight
                                },
                                new[]
                                {
                                    tmpCharacterInfo.vertex_BL.color, tmpCharacterInfo.vertex_TL.color,
                                    tmpCharacterInfo.vertex_TR.color, tmpCharacterInfo.vertex_BR.color
                                },
                                tmpCharacterInfo,
                                tmpCharIndex);*/

                            tempData.Add(new CharacterData(new Vector3[4], new Color32[4], new Vector2[4],
                                new Vector2[4], tmpCharacterInfo,
                                tmpCharIndex));

                            charIndex++;
                            break;
                    }
                }

                // Reset no parse
                noParse = false;

                // Final Text with rich text tags. Allows rich text tags to be present together with animations
                for (int i = 0; i < _tmpro.text.Length; i++)
                {
                    char letter = _tmpro.text[i];

                    switch (letter)
                    {
                        case '<':
                        {
                            int next = ReadTagFromString(i + 1, '>', out string t);
                            if (t.ToLower() == "np")
                            {
                                noParse = true;
                                i = next;
                                break;
                            }

                            if (t.ToLower() == "/np")
                            {
                                noParse = false;
                                i = next;
                                break;
                            }

                            if (next > 0 && IsAnimationTagValid(t, out TagOptions _, out TextAnimation _) && !noParse)
                            {
                                i = next;
                            }
                            else
                            {
                                finalText.Append(letter);
                            }

                            break;
                        }
                        case '[':
                        {
                            int next = ReadTagFromString(i + 1, ']', out string t);
                            if (next > 0 &&
                                (IsTyperEffectTagValid(t, typer, out TagOptions _, out ShowCharacterEffect _) ||
                                 IsWaitActionTag(t, typer, out _) || IsCustomTyperEvent(t, typer, out _)) && !noParse)
                            {
                                i = next;
                            }
                            else
                            {
                                finalText.Append(letter);
                            }

                            break;
                        }
                        default:
                            finalText.Append(letter);
                            break;
                    }
                }

                // Adds remaining effects. Allows the user to not need to close the tag if effect is to run until the end
                AddAll(charIndex - 1);

                _text = _tmpro.text;
                // Set cursor
                finalText.Append(cursorDetails.cursorChar);
                _tmpro.text = finalText.ToString();
                _tmpro.ForceMeshUpdate();

                //_tmpro.renderMode = TextRenderFlags.Render;

                // Index is the index in the character info
                int ReadTag(int index, char closer, out string tag)
                {
                    int stringIndex = _tmproInfo.characterInfo[index].index; // Index in the text string
                    int closerIndex = _tmpro.text.IndexOf(closer, stringIndex);

                    tag = closerIndex > 0
                        ? _tmpro.text.Substring(stringIndex, closerIndex - stringIndex)
                        : string.Empty;

                    return index + closerIndex - stringIndex;
                }

                int ReadTagFromString(int index, char closer, out string tag)
                {
                    int closerIndex = _tmpro.text.IndexOf(closer, index);

                    tag = closerIndex > 0 ? _tmpro.text.Substring(index, closerIndex - index) : string.Empty;

                    return closerIndex;
                }

                void AddEffects(int index, string tag)
                {
                    if (!animationsStack.TryGetValue(tag, out Stack<AnimationDetails> anims))
                    {
                        Debug.LogWarning($"No effect opened with tag: <{tag}>");
                        return;
                    }

                    foreach (AnimationDetails animationInfo in anims)
                    {
                        _animationInfos.Add(new AnimationInfo(animationInfo.Start, index, animationInfo.InvisibleChars,
                            animationInfo.Animation));
                    }

                    anims.Clear();
                }

                void AddAll(int index)
                {
                    foreach (AnimationDetails details in
                             animationsStack.Values.SelectMany(animationInfos => animationInfos))
                    {
                        _animationInfos.Add(new AnimationInfo(details.Start, index, details.InvisibleChars,
                            details.Animation));
                    }

                    animationsStack.Clear();
                }

                void CloseAllShowCharEffects()
                {
                    openDetails.Clear();
                }

                void CloseLastAnimationWithTag(string tag, int end, string effectTag)
                {
                    if (animationsStack.TryGetValue(tag, out Stack<AnimationDetails> animationDetailsList) &&
                        animationDetailsList.Count > 0)
                    {
                        AnimationDetails details = animationDetailsList.Pop();
                        AnimationInfo info = new AnimationInfo(details.Start, end,
                            details.InvisibleChars, details.Animation);
                        _animationInfos.Add(info);
                    }
                    else
                        Debug.LogWarning(
                            $"No effect opened with tag: <{effectTag}>");
                }

                void CloseLastCharEffectWithTag(ShowCharacterEffect effect)
                {
                    openDetails.Remove(effect);
                }

                void AddToTempData(TMP_CharacterInfo tmpCharacterInfo, int tmpCharIndex)
                {
                    tempData.Add(new CharacterData(new Vector3[4], new Color32[4], new Vector2[4], new Vector2[4],
                        tmpCharacterInfo,
                        tmpCharIndex));

                    charIndex++;
                }
            }

            void StorePositions()
            {
                _validData = new CharacterData[_tmproInfo.characterCount];
                for (int i = 0; i < _tmproInfo.characterCount; i++)
                {
                    TMP_CharacterInfo tmpCharacterInfo = _tmproInfo.characterInfo[i];

                    _validData[i] = new CharacterData(
                        new[]
                        {
                            tmpCharacterInfo.bottomLeft, tmpCharacterInfo.topLeft,
                            tmpCharacterInfo.topRight, tmpCharacterInfo.bottomRight
                        },
                        new[]
                        {
                            tmpCharacterInfo.vertex_BL.color, tmpCharacterInfo.vertex_TL.color,
                            tmpCharacterInfo.vertex_TR.color, tmpCharacterInfo.vertex_BR.color
                        },
                        new[]
                        {
                            tmpCharacterInfo.vertex_BL.uv, tmpCharacterInfo.vertex_TL.uv,
                            tmpCharacterInfo.vertex_TR.uv, tmpCharacterInfo.vertex_BR.uv
                        },
                        new[]
                        {
                            tmpCharacterInfo.vertex_BL.uv2, tmpCharacterInfo.vertex_TL.uv2,
                            tmpCharacterInfo.vertex_TR.uv2, tmpCharacterInfo.vertex_BR.uv2
                        },
                        tmpCharacterInfo,
                        i);

                    if (!hasTyper)
                        continue;

                    foreach (ShowCharacterEffect effect in typer.GlobalDefaultEffects.And(typer.LocalDefaultEffects))
                    {
                        _validData[i].OnBecomeVisibleEffects.Add(effect);
                    }
                }
            }

            void ComputeAnimationBounds()
            {
                foreach (AnimationInfo animationInfo in _animationInfos)
                {
                    animationInfo.ComputeBounds(_validData);
                }
            }

            void AddCharEffects()
            {
                if (!hasTyper)
                    return;
                
                foreach (KeyValuePair<ShowCharacterEffect, List<int>> kv in charIndicesByEffect)
                {
                    foreach (int i in kv.Value)
                    {
                        _validData[i].OnBecomeVisibleEffects.Add(kv.Key);
                    }
                }
            }

            #endregion
        }

        bool IsAnimationTagValid(string tag, out TagOptions options, out TextAnimation animation)
        {
            string lowerTag = tag.ToLower();

            if (ValidateGlobalTags(lowerTag, out options))
            {
                animation = default;
                return true;
            }

            foreach (TextAnimation a in globalAnimations.Select(o => o.Animation))
            {
                if (!IsTagValid(a.tag, lowerTag, out options))
                    continue;

                animation = a;
                return true;
            }

            foreach (TextAnimation a in localAnimations)
            {
                if (!IsTagValid(a.tag, lowerTag, out options))
                    continue;

                animation = a;
                return true;
            }

            options = default;
            animation = default;
            return false;
        }

        static bool IsTagValid(string effectTag, string tag, out TagOptions tagOptions)
        {
            string animTagLower = effectTag.ToLower();

            if (tag.Equals(animTagLower))
            {
                tagOptions = TagOptions.Create;
                return true;
            }

            if (tag.Equals($"/{animTagLower}"))
            {
                tagOptions = TagOptions.CloseLastSameTag;
                return true;
            }

            if (tag.Equals($"//{animTagLower}"))
            {
                tagOptions = TagOptions.CLoseAllSameTag;
                return true;
            }

            tagOptions = default;
            return false;
        }

        static bool IsTyperEffectTagValid(string tag, FunTextTyper typer, out TagOptions options,
            out ShowCharacterEffect effect)
        {
            if (ValidateGlobalTags(tag, out options))
            {
                effect = default;
                return true;
            }

            foreach ((string t, ShowCharacterEffect eff) in typer.GlobalTagEffects.And(typer.LocalTagEffects))
            {
                if (!IsTagValid(t, tag, out options))
                    continue;

                effect = eff;

                if (eff.Effects.Count != 0)
                    return true;

                Debug.LogWarning($"Effect with tag: {t} does not have any effects! Ignoring it");
            }

            effect = default;
            return false;
        }

        static bool IsWaitActionTag(string tag, FunTextTyper typer, out IEnumerator waiter)
        {
            foreach (WaitAction waitAction in typer.WaitActions)
            {
                if (!waitAction.Tag.Equals(tag))
                    continue;

                waiter = waitAction.Waiter;
                return true;
            }

            waiter = default;
            return false;
        }

        bool IsCustomTyperEvent(string tag, FunTextTyper typer, out CustomTyperEvent @event)
        {
            if (tag.Equals(string.Empty) || tag[0] != '@')
            {
                @event = default;
                return false;
            }

            foreach (CustomTyperEvent e in typer.CustomTyperEvents)
            {
                if (!tag.Equals('@' + e.Tag))
                    continue;

                @event = e;
                return true;
            }

            @event = default;
            return false;
        }

        static bool ValidateGlobalTags(string lowerTag, out TagOptions options)
        {
            switch (lowerTag)
            {
                case "//":
                    options = TagOptions.CloseAll;
                    return true;
                case "/":
                    options = TagOptions.CloseLastAdded;
                    return true;
                case "np":
                    options = TagOptions.EnableNoParse;
                    return true;
                case "/np":
                    options = TagOptions.DisableNoParse;
                    return true;
            }

            options = default;
            return false;
        }

        void InitializeAnimations()
        {
            // foreach (TextAnimation localAnimation in localAnimations)
            // {
            //     localAnimation.Initialize();
            // }
            //
            // foreach (TextAnimation globalAnimation in globalAnimations)
            // {
            //     globalAnimation.Initialize();
            // }
        }

        void UpdateCursorPosition()
        {
            if (!cursorDetails.show)
                return;
            
            TMP_CharacterInfo nextInfo = _validData[_visibleChars].Info;

            // Calculate distance from cursor TL to char BR
            Vector3 dist = new Vector3(nextInfo.bottomLeft.x, nextInfo.descender + cursorDetails.OffsetFromDescender) -
                           Cursor.Vertices[1];

            for (int i = 0; i < 4; i++)
            {
                Cursor.Vertices[i] += dist;
                Cursor.Colors[i] = Cursor.SourceColors[i];
            }

            cursorDetails.TimeInactive = 0;
        }

        void PlayAnimations()
        {
            foreach (AnimationInfo anim in _animationInfos)
            {
                anim.Play(ref _validData);
            }

            for (int i = 0; i < _visibleChars; i++)
            {
                _validData[i].PlayShowAnimations();
            }

            for (int i = _visibleChars; i < _validData.Length - 1; i++)
            {
                _validData[i].PlayHideEffects();
            }

            if (!cursorDetails.show || !cursorDetails.IsInactive)
                return;

            for (int i = 0; i < Cursor.Colors.Length; i++)
            {
                Cursor.Colors[i] = Color32.Lerp(Cursor.SourceColors[i], Color.clear,
                    Mathf.RoundToInt(Mathf.PingPong(Time.time * cursorDetails.blinkRate, 1)));
            }
        }

        #endregion

        #endregion
    }
}