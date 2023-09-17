using System;
using System.Collections;
using System.Collections.Generic;
using SSSTools.FunText.Attributes;
using SSSTools.FunText.Effects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Random = UnityEngine.Random;

namespace SSSTools.FunText.Core
{
    [RequireComponent(typeof(FunTextAnimator))]
    [DefaultExecutionOrder(2)]
    public class FunTextTyper : MonoBehaviour
    {
        enum OnEnd
        {
            Nothing,
            Restart,
            Reverse,
            PingPong
        }

        enum State
        {
            Waiting, Typing, Erasing
        }

        [SerializeField] float charsPerSecond;
        [SerializeField] float punctuationWait;
        [SerializeField] AudioClip[] typingSounds;
        [SerializeField] bool startOnPlay = true;
        [SerializeField] Pitch pitch;
        [SerializeField] List<ShowCharacterEffectDataObject> globalDefaultEffects;
        [SerializeField] List<ShowCharacterEffect> localDefaultEffects;
        [SerializeField] List<ShowCharacterEffectDataObject> globalTagEffects;
        [SerializeField] List<ShowCharEffectTag> localTagEffects;
        [SerializeField] List<WaitAction> waitActions;
        [SerializeField] OnEnd onEnd;
        [SerializeField] bool canSkipTyping;
        [SerializeField] bool canSkipErasing;
        [SerializeReference, ChooseReference] CustomYieldInstruction _skipAction;

        FunTextAnimator _animator;
        AudioSource _audioSource;
        State _state;

        IEnumerator _activeRoutine;
        IEnumerator _skipRoutine;

        public IEnumerable<ShowCharacterEffectDataObject> GlobalDefaultEffects => globalDefaultEffects;

        public IEnumerable<ShowCharacterEffect> LocalDefaultEffects => localDefaultEffects;

        public IEnumerable<ShowCharacterEffectDataObject> GlobalTagEffects => globalTagEffects;
        
        public IEnumerable<ShowCharEffectTag> LocalTagEffects => localTagEffects;

        public IEnumerable<WaitAction> WaitActions => waitActions;

        public IEnumerable<CustomTyperEvent> CustomTyperEvents => customTyperEvents;

        [SerializeField] ShowCharacterEvent onNewCharacterShowed;
        [SerializeField] HideCharacterEvent onNewCharacterHidden;
        [SerializeField] OnTyperFinishedEvent onTyperFinished;
        [SerializeField] List<CustomTyperEvent> customTyperEvents;

        public event UnityAction<char> OnNewCharacterShowed
        {
            add => onNewCharacterShowed.AddListener(value);
            remove => onNewCharacterShowed.RemoveListener(value);
        }
        
        public event UnityAction<char> OnNewCharacterHidden
        {
            add => onNewCharacterHidden.AddListener(value);
            remove => onNewCharacterHidden.RemoveListener(value);
        }
        
        /// <summary>
        /// On new character showed and hidden
        /// </summary>
        public event UnityAction<char> OnNewCharacter
        {
            add
            {
                onNewCharacterShowed.AddListener(value);
                onNewCharacterHidden.AddListener(value);
            }
            remove
            {
                onNewCharacterShowed.RemoveListener(value);
                onNewCharacterHidden.RemoveListener(value);
            }
        }

        public event UnityAction OnTyperFinished
        {
            add => onTyperFinished.AddListener(value);
            remove => onTyperFinished.RemoveListener(value);
        }

        void Awake()
        {
            _animator = GetComponent<FunTextAnimator>();
            _audioSource = GetComponent<AudioSource>();
        }
        
        void Start()
        {
            if (startOnPlay)
                StartTyping();
        }

        public void StartTyping()
        {
            if (_state != State.Waiting)
                return;
            
            _state = State.Typing;

            _activeRoutine = StartTypingCoRoutine();
            
            if (canSkipTyping)
                ListenForSkip();
            
            StartCoroutine(_activeRoutine);
            
            IEnumerator StartTypingCoRoutine()
            {
                // TODO: Time should scale
                CustomWaitForSeconds timer = new CustomWaitForSeconds(1 / charsPerSecond);
                CustomWaitForSeconds punctuationTimer = new CustomWaitForSeconds(punctuationWait);

                if (_animator.TryGetWaiter(out IEnumerator waiter))
                    yield return waiter;
                
                while (_animator.SetNextCharVisible(out char nextChar))
                {
                    onNewCharacterShowed.Invoke(nextChar);
                    
                    if (_animator.TryGetEvent(out CustomTyperEvent customEvent))
                        customEvent.Invoke();
                    
                    if (_animator.TryGetWaiter(out waiter))
                        yield return waiter;
                    
                    // if (nextChar == ' ') 
                    //     continue;

                    if (_audioSource != null && typingSounds.Length > 0)
                    {
                        _audioSource.pitch = Random.Range(pitch.min, pitch.max);
                        _audioSource.PlayOneShot(typingSounds[Random.Range(0, typingSounds.Length)]);
                    }

                    if (char.IsPunctuation(nextChar))
                        yield return punctuationTimer;
                    else
                        yield return timer;
                }

                // TODO: Calculate max duration or give user the choice
                yield return new CustomWaitForSeconds(1);
            
                OnTypingEnd();
            }
        }

        public void StartTyping(string text)
        {
            _state = State.Waiting;
            _animator.SetText(text);
            
            StartTyping();
        }

        public void StartErasing()
        {
            if (_state != State.Waiting)
                return;
            
            _state = State.Erasing;
            
            _activeRoutine = StartErasingCoRoutine();
            
            if (canSkipErasing)
                ListenForSkip();
            
            StartCoroutine(_activeRoutine);
            
            IEnumerator StartErasingCoRoutine()
            {
                CustomWaitForSeconds timer = new CustomWaitForSeconds(1 / charsPerSecond);

                while (_animator.HideNextCharacter(out char hidden))
                {
                    onNewCharacterHidden.Invoke(hidden);
                    // TODO: Play Sounds?
                    
                    yield return timer;
                }

                yield return new CustomWaitForSeconds(1);
                
                OnTypingEnd();
            }
        }

        public void Skip()
        {
            StopCoroutine(_activeRoutine);
            _animator.SetAllVisible();
        }

        void OnTypingEnd()
        {
            onTyperFinished.Invoke();
            
            switch (onEnd)
            {
                case OnEnd.Nothing:
                    _state = State.Waiting;
                    break;
                case OnEnd.Restart:
                    _animator.HideAllCharacters();
                    StartTyping();
                    break;
                case OnEnd.Reverse:
                    // TODO: Disappearing effects
                    StartErasing();
                    break;
                case OnEnd.PingPong:
                    switch (_state)
                    {
                        case State.Waiting:
                            break;
                        case State.Typing:
                            _state = State.Waiting;
                            StartErasing();
                            break;
                        case State.Erasing:
                            _state = State.Waiting;
                            StartTyping();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ListenForSkip()
        {
            _skipRoutine = ListenForSkipRoutine();
            StartCoroutine(_skipRoutine);
            
            IEnumerator ListenForSkipRoutine()
            {
                yield return _skipAction;
                Skip();
            }
        }
    }

    [Serializable]
    internal struct Pitch
    {
        [Range(-3, 3)] public float min, max;
    }

    [Serializable]
    public class WaitAction
    {
        [SerializeField] string tag;
        [SerializeReference] IEnumerator _waiter;

        public IEnumerator Waiter => _waiter;

        public string Tag => tag;
    }

    [Serializable]
    public class ShowCharacterEffect
    {
        public float duration;
        [SerializeReference, ChooseReference] public List<ICharacterEffect> Effects;

        public void PlayShowEffect(ref CharacterData data)
        {
            foreach (ICharacterEffect eff in Effects)
            {
                eff.PlayAnimation(ref data, data.TimeVisible / duration);
            }
        }

        public void PlayHideEffect(ref CharacterData data)
        {
            foreach (ICharacterEffect eff in Effects)
            {
                eff.PlayAnimation(ref data, 1 - data.TimeHidden / duration);
            }
        }

        public bool HasCharacterFinishedShowing(CharacterData character)
        {
            return character.TimeVisible > duration;
        }
        
        public bool HasCharacterFinishedHiding(CharacterData character)
        {
            return character.TimeHidden > duration;
        }
    }

    [Serializable]
    public class ShowCharEffectTag
    {
        [SerializeField] ShowCharacterEffect effect;
        [SerializeField] string tag;

        public ShowCharacterEffect Effect => effect;

        public string Tag => tag;
    }

    [Serializable]
    public class CustomWaitForSeconds : CustomYieldInstruction
    {
        [SerializeField, Min(0)] float seconds = 1;

        public new object Current => TimePassed;
        
        public float TimePassed { get; private set; }

        public float Seconds
        {
            get => seconds;
            set => seconds = value;
        }
        
        CustomWaitForSeconds() { }
        
        public CustomWaitForSeconds(float seconds)
        {
            this.seconds = seconds;
        }

        public bool KeepWaiting()
        {
            TimePassed += Time.deltaTime;
            
            if (TimePassed < Seconds)
                return true;

            Reset();
            return false;
        }

        public CustomWaitForSeconds Wait(float seconds)
        {
            this.seconds = seconds;
            return this;
        }

        public override void Reset()
        {
            TimePassed = 0;
        }

        public override bool keepWaiting => KeepWaiting();
    }
    
    internal class WaitAnyInput : CustomYieldInstruction
    {
#if !ENABLE_INPUT_SYSTEM
            public override bool keepWaiting => !Input.anyKeyDown;
#else
        bool _isKeyPressed;
        bool _isWaiting;

        public override bool keepWaiting
        {
            get
            {
                if (!_isWaiting)
                {
                    InputSystem.onAnyButtonPress.CallOnce(ctrl => _isKeyPressed = true);
                    _isWaiting = true;
                    return true;
                }

                if (!_isKeyPressed) 
                    return true;
                
                Reset();
                return false;
            }
        }

        public override void Reset()
        {
            _isKeyPressed = false;
            _isWaiting = false;
        }
        
#endif
    }

    public enum ButtonPressType
    {
        Hold, // Returns true if pressed
        Down, // Only in the first frame that was pressed
        Up, // First frame that got releases
#if ENABLE_INPUT_SYSTEM
        Performed // First frame that was performed (i.e. after a hold operation was successful) Allows more intricate operations 
#endif
    }
    
    [Serializable]
    public class WaitInput : CustomYieldInstruction
    {
        [SerializeField] ButtonPressType buttonPressType;
        
#if !ENABLE_INPUT_SYSTEM
        
        [SerializeField] KeyCode key = KeyCode.Space;

        public override bool keepWaiting => !(type switch
        {
            Type.Hold => Input.GetKey(key),
            Type.Down => Input.GetKeyDown(key),
            Type.Up => Input.GetKeyUp(key),
            _ => throw new ArgumentOutOfRangeException()
        });
#else
        [SerializeField] InputActionReference inputActionReference;

        public ButtonPressType ButtonPressType
        {
            get => buttonPressType;
            set => buttonPressType = value;
        }

        public InputActionReference InputActionReference
        {
            get => inputActionReference;
            set => inputActionReference = value;
        }

        WaitInput()
        {
        }

        public WaitInput(InputActionReference inputActionReference)
        {
            this.inputActionReference = inputActionReference;
        }

        public override bool keepWaiting => !(buttonPressType switch
        {
            ButtonPressType.Hold => inputActionReference.action.IsPressed(),
            ButtonPressType.Down => inputActionReference.action.WasPressedThisFrame(),
            ButtonPressType.Up => inputActionReference.action.WasReleasedThisFrame(),
            ButtonPressType.Performed => inputActionReference.action.WasPerformedThisFrame(),
            _ => throw new ArgumentOutOfRangeException()
        });

        public void Enable() => inputActionReference.action.Enable();
        public void Disable() => inputActionReference.action.Disable();
#endif
    }
    
    [Serializable]
    public class ShowCharacterEvent : UnityEvent<char> { }

    [Serializable]
    public class HideCharacterEvent : UnityEvent<char> { }
        
    [Serializable]
    public class OnTyperFinishedEvent : UnityEvent { }

    [Serializable]
    public class CustomTyperEvent
    {
        [SerializeField] string tag;
        [SerializeField] UnityEvent @event;

        public string Tag => tag;

        public void AddListener(UnityAction a) => @event.AddListener(a);
        public void RemoveListener(UnityAction a) => @event.RemoveListener(a);
        public void Invoke() => @event.Invoke();
    }
}