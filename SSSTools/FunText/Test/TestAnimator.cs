/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSSTools.FunText.Effects;
using TMPro;
using UnityEditor;
using UnityEngine;
using AnimationInfo = SSSTools.FunText.Core.AnimationInfo;

namespace SSSTools.FunText.Test
{
    [RequireComponent(typeof(TMP_Text))]
    public class TestAnimator : MonoBehaviour
    {
        
        TMP_Text textMeshPro;
        Mesh textMesh;
        Vector3[] vertices;
        Color[] colors;
        
        float colorIndex = 0;

        [SerializeField] Material material;
        // User animations
        [SerializeField] List<FunText.Core.TextAnimation> animations;
        // List that stores all the info for all animations
        [SerializeField] List<AnimationInfo> infos = new List<AnimationInfo>(); 

        IBaseEffect effect;
        CharacterData[] data;

        void Awake()
        {
            textMeshPro = GetComponent<TMP_Text>();
            textMeshPro.ForceMeshUpdate(true);
            
            TMP_CharacterInfo info = textMeshPro.textInfo.characterInfo[0];
            info.character = 'H';
            info.material = material;
            textMeshPro.textInfo.characterInfo[0].material = material;
            textMeshPro.textInfo.characterInfo[0] = info;
            textMeshPro.textInfo.meshInfo[0].material = material;

            textMeshPro.textInfo.characterInfo[0].character = 'H';
            
            textMeshPro.ForceMeshUpdate();
            textMeshPro.SetMaterialDirty();

            effect = new WaveEffect();
            
            ReadTags();
            
            textMeshPro.ForceMeshUpdate(true);

            data = new CharacterData[textMeshPro.textInfo.characterCount];
            textMesh = textMeshPro.mesh;
            for (int i = 0; i < textMeshPro.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo tmpCharacterInfo = textMeshPro.textInfo.characterInfo[i];
                
                data[i] = new CharacterData(
                    new[]{tmpCharacterInfo.bottomLeft, tmpCharacterInfo.topLeft, tmpCharacterInfo.topRight, tmpCharacterInfo.bottomRight}, 
                    new []{tmpCharacterInfo.vertex_BL.color, tmpCharacterInfo.vertex_TL.color, tmpCharacterInfo.vertex_TR.color, tmpCharacterInfo.vertex_BR.color},
                    tmpCharacterInfo,
                    i);
            }
        }

        IEnumerator TypeWriter()
        {
            textMeshPro.maxVisibleCharacters = 0;
            
            while (textMeshPro.maxVisibleCharacters < textMeshPro.textInfo.characterCount)
            {
                textMeshPro.maxVisibleCharacters++;
                yield return new WaitForSeconds(0.5f);
            }
        }

        void ReadTags()
        {
            print("Reading tags");
            
            StringBuilder finalText = new StringBuilder(textMeshPro.text.Length);
            Dictionary<string, List<AnimationInfo>> animationsStack = new Dictionary<string, List<AnimationInfo>>();
            
            textMeshPro.ForceMeshUpdate();
            
            /*for (int i = 0, charIndex = 0; i < textMeshPro.text.Length; i++)
            {
                char stringChar = textMeshPro.text[i]; // The char in the full string
                //char realChar = textMeshPro.textInfo.characterInfo[charIndex].character; // Ignores rich text from tmp
                
                // Reading a rich text tag just add and ignore
                // if (stringChar != realChar)
                // {
                //     finalText.Append(stringChar);
                //     continue;
                // }

                switch (stringChar)
                {
                    case '<':
                        int next = ReadTag(i + 1, out string effectTag);
                        int end = FindCloseTag(next);
                        /*if (next > 0 && IsValidTag(effectTag) && end != -1)
                        {
                            // Every time there is a '<' check if it is a valid tag
                            // If yes then skip it. If not add to text
                            
                            int count = 0;
                            charIndex += effectTag.Length + 2;
                            for (int j = next + 1; j < end - 2; j++)
                            {
                                finalText.Append(textMeshPro.text[j]);

                                if (!textMeshPro.text[j].Equals(textMeshPro.textInfo.characterInfo[charIndex].character)) 
                                    continue;
                                
                                charIndex++;
                                count++;
                            }
                            
                            animations.Add(new Animation(new TestEffect(), i, i + count)); 
                            i = end;
                        }
                        else
                        {
                            finalText.Append(stringChar);
                        }#2#

                        if (next > 0 && IsValidTag(effectTag, out ValidTag validTag, out TextAnimation textAnim))
                        {
                            // TODO: Interface with what to do
                            switch (validTag.options)
                            {
                                case TagOptions.Create:
                                    if (animationsStack.TryGetValue(effectTag, out List<AnimationInfo> infos))
                                        infos.Add(new AnimationInfo(charIndex, charIndex + 1, textAnim));
                                    else
                                        animationsStack.Add(effectTag,
                                            new List<AnimationInfo> {new AnimationInfo(charIndex, charIndex + 1, textAnim)});
                                    break;
                                case TagOptions.Last:
                                    if (animationsStack.TryGetValue(effectTag, out infos))
                                    {
                                        int lastIndex = infos.Count - 1;
                                        this.infos.Add(infos[lastIndex]);
                                        infos.RemoveAt(lastIndex);
                                    }
                                    else
                                        Debug.LogWarning(
                                            $"No effect opened with tag: <{effectTag}>"); // TODO: Error messages in const

                                    break;
                                case TagOptions.All:
                                    AddAll();
                                    break;
                                case TagOptions.AllSameTag:
                                    AddEffects(effectTag);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            
                            
                            i = next;
                            charIndex += effectTag.Length + 2;
                        }
                        else
                        {
                            finalText.Append(stringChar);
                        }
                        break;
                    default:
                        AddCharacter(stringChar);
                        break;
                }

                charIndex++;
            }#1#

            // Final text without tmp rich text tags. Allows proper counting of characters
            for (int realCharIndex = 0; realCharIndex < textMeshPro.textInfo.characterCount; realCharIndex++)
            {
                char letter = textMeshPro.textInfo.characterInfo[realCharIndex].character;
                
                switch (letter)
                {
                    case '<':
                        int next = ReadTag(realCharIndex + 1, out string effectTag);
                        if (next > 0 && IsValidTag(effectTag, out ValidTag validTag, out FunText.Core.TextAnimation textAnim))
                        {
                            // TODO: Interface with what to do
                            switch (validTag.options)
                            {
                                case TagOptions.Create:
                                    if (animationsStack.TryGetValue(effectTag, out List<AnimationInfo> infos))
                                        infos.Add(new AnimationInfo(realCharIndex,0,0, textAnim));
                                    else
                                        animationsStack.Add(effectTag,
                                            new List<AnimationInfo> {new AnimationInfo(realCharIndex,0,0, textAnim)});
                                    break;
                                case TagOptions.Last:
                                    if (animationsStack.TryGetValue(effectTag[1..], out infos))
                                    {
                                        int lastIndex = infos.Count - 1;
                                        this.infos.Add(infos[lastIndex]);
                                        infos.RemoveAt(lastIndex);
                                    }
                                    else
                                        Debug.LogWarning($"No effect opened with tag: <{effectTag}>"); // TODO: Error messages in const
                                    break;
                                case TagOptions.All:
                                    AddAll();
                                    break;
                                case TagOptions.AllSameTag:
                                    AddEffects(effectTag);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            
                            
                            realCharIndex = next;
                        }
                        // else
                        // {
                        //     finalText.Append(letter);
                        // }
                        break;
                    default:
                        AddToEnd(letter);
                        break;
                }
            }
            
            // Final Text with rich text tags. Allows rich text tags to be present together with animations
            for (int i = 0; i < textMeshPro.text.Length; i++)
            {
                char letter = textMeshPro.text[i];

                switch (letter)
                {
                    case '<':
                        int next = ReadTagFromString(i + 1, out string t);
                        if (next > 0 && IsValidTag(t, out ValidTag validTag, out FunText.Core.TextAnimation _))
                        {
                            i = next;
                        }
                        else
                        {
                            finalText.Append(letter);
                        }
                        break;
                    default:
                        finalText.Append(letter);
                        break;
                }
            }
            
            // Adds remaining effects. Allows the user to not need to close the tag if effect is to run until the end
            AddAll();

            textMeshPro.text = finalText.ToString();
            //textMeshPro.text = "Modified Text";
            textMeshPro.ForceMeshUpdate();
            //textMeshPro.textInfo.characterInfo[0].isVisible = false;

            textMeshPro.renderMode = TextRenderFlags.Render;

            // Char index is the index in the character info
            int ReadTag(int charIndex, out string tag)
            {
                int stringIndex = textMeshPro.textInfo.characterInfo[charIndex].index; // Index in the text string
                int closer = textMeshPro.text.IndexOf('>', stringIndex);
                
                tag = closer > 0 ? textMeshPro.text.Substring(stringIndex, closer - stringIndex) : string.Empty;
                
                return charIndex + closer - stringIndex;
            }

            int ReadTagFromString(int index, out string tag)
            {
                int closer = textMeshPro.text.IndexOf('>', index);
                
                tag = closer > 0 ? textMeshPro.text.Substring(index, closer - index) : string.Empty;
                
                return closer;
            }

            void AddEffects(string tag)
            {
                if (!animationsStack.TryGetValue(tag, out List<AnimationInfo> anims))
                {
                    Debug.LogWarning($"No effect opened with tag: <{tag}>");
                    return;
                }
                
                foreach (AnimationInfo animationInfo in anims)
                {
                    infos.Add(animationInfo);
                }
                    
                anims.Clear();
            }

            void AddAll()
            {
                foreach (AnimationInfo animationInfo in animationsStack.Values.SelectMany(animationInfos => animationInfos))
                {
                    infos.Add(animationInfo);
                }

                animationsStack.Clear();
            }
            
            bool IsValidTag(string tag, out ValidTag options, out FunText.Core.TextAnimation animation)
            {
                string lowerTag = tag.ToLower();
                
                if (lowerTag.Equals("//"))
                {
                    options = new ValidTag()
                    {
                        Tag = tag,
                        options = TagOptions.All
                    };
                    animation = default;
                    return true;
                }
                    
                if (lowerTag.Equals($"//{lowerTag}"))
                {
                    options = new ValidTag()
                    {
                        Tag = tag,
                        options = TagOptions.AllSameTag
                    };
                    animation = default;
                    return true;
                }
                
                foreach (FunText.Core.TextAnimation a in animations)
                {
                    string animTagLower = a.tag.ToLower();
                    
                    if (animTagLower.Equals(lowerTag))
                    {
                        options = new ValidTag()
                        {
                            Tag = tag,
                            options = TagOptions.Create
                        };
                        animation = a;
                        return true;
                    }
                    
                    if ($"/{animTagLower}".Equals(lowerTag))
                    {
                        options = new ValidTag()
                        {
                            Tag = tag,
                            options = TagOptions.Last
                        };
                        animation = a;
                        return true;
                    }
                    
                }

                print($"Tag not valid: {lowerTag}");
                
                options = default;
                animation = default;
                return false;
            }

            int FindCloseTag(int start)
            {
                while (true)
                {
                    if (start >= textMeshPro.text.Length - 1 || start < 0)
                        return -1;

                    int open = textMeshPro.text.IndexOf('<', start);
                    int close = textMeshPro.text.IndexOf('>', start + 1);

                    if (open < 0 || close < 0) 
                        return -1;

                    string tag = textMeshPro.text.Substring(open, close - open + 1);

                    if (tag.Equals("</>")) 
                        return close;

                    start = close;
                }
            }

            void AddToEnd(char letter)
            {
                //finalText.Append(letter);

                foreach (AnimationInfo animationInfo in animationsStack.Values.SelectMany(animationInfos => animationInfos))
                {
                    //animationInfo.End++;
                }
            }
        }
        
        struct ValidTag
        {
            public string Tag;
            public TagOptions options;
        }

        enum TagOptions
        {
            Create,
            Last,
            All,
            AllSameTag
        }
        
        void Update()
        {
            
#if UNITY_EDITOR
            
            if (!EditorApplication.isPlaying)
            {
                ReadTags();
                return;
            }
#endif

            
            // textMeshPro.ForceMeshUpdate();
            foreach (CharacterData characterData in data)
            {
                characterData.Reset();
            }

            foreach (AnimationInfo info in infos)
            {
                info.TextAnimation.Play(data, info.Start, info.End);
            }

            // for (int i = 0; i < textMeshPro.textInfo.characterCount; i++)
            // {
            //     if (!textMeshPro.textInfo.characterInfo[i].isVisible)
            //         continue;
            //     
            //     effect.PlayAnimation(ref data[i], i);
            // }

            // int index = 0;
            // for (int i = 0; i < textMesh.vertices.Length - 3; i+=4)
            // {
            //     for (int j = 0; j < 4; j++)
            //     {
            //         textMesh.vertices[i + j] = data[index].Vertices[j];
            //         textMesh.colors[i + j] = data[index].Colors[j];
            //     }
            //
            //     index++;
            // }
            
            // TODO: What happens when user changes the text
            
            List<Vector3> v = textMesh.vertices.ToList();
            List<Color> c = textMesh.colors.ToList();
            TMP_TextInfo textInfo = textMeshPro.textInfo;
            for (int i = 0; i < textMeshPro.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                
                if (!charInfo.isVisible)
                    continue;

                for (int j = 0; j < 4; j++)
                {
                    // v[i*4 + j] = data[index].Vertices[j];
                    // c[i*4 + j] = data[index].Colors[j];

                    int materialIndex = charInfo.materialReferenceIndex;
                    
                    textInfo.meshInfo[materialIndex].vertices[charInfo.vertexIndex + j] = data[i].Vertices[j];
                    textInfo.meshInfo[materialIndex].colors32[charInfo.vertexIndex + j] = data[i].Colors[j];
                }
            }
            
            //textMeshPro.UpdateVertexData();
            
            // textMesh.vertices = v.ToArray();
            // textMesh.colors = c.ToArray();
            // textMeshPro.canvasRenderer.SetMesh(textMesh);

            // if (textMeshPro.havePropertiesChanged)
            // {
            //     print("Changed");
            //     textMeshPro.ForceMeshUpdate();
            // }
            
            
        }

        void UpdateS()
        {   
            textMeshPro.ForceMeshUpdate(true);
            textMesh = textMeshPro.mesh;
            vertices = textMesh.vertices;
            colors = textMesh.colors;
            
            // Per vertex
            // for (int i = 0; i < vertices.Length; i++)
            // {
            //     vertices[i] += Wobble(i) * 5;
            // }
            
            // Per character
            // for (int i = 0; i < textMeshPro.textInfo.characterCount; i++)
            // {
            //     int index = textMeshPro.textInfo.characterInfo[i].vertexIndex;
            //
            //     Vector3 offset = Wobble(i);
            //
            //     for (int j = 0; j < 4; j++)
            //     {
            //         vertices[index++] += offset;
            //     }
            // }

            Color[] staticColors = {Color.black, Color.blue, Color.magenta, Color.green,};
            
            // Color
            
            for (int i = 0; i < vertices.Length; i++)
            {
                Color color = staticColors[(int)colorIndex % staticColors.Length];
                colors[i] = color;
                colors[i++] = color;

                colorIndex += 0.001f;
            }

            textMesh.vertices = vertices;
            textMesh.colors = colors;
            textMeshPro.canvasRenderer.SetMesh(textMesh);
        }

        
        
        
        
    }
}
*/
