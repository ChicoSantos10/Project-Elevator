using System;
using System.Collections.Generic;

namespace State_Machine
{
    public class StateMachine
    {
        class Transition
        {
            public Func<bool> Condition { get; }

            public IState From { get; }

            public IState To { get; }

            public Transition(IState from, IState to, Func<bool> condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }
        }

        Dictionary<IState, List<Transition>> conditions = new Dictionary<IState, List<Transition>>();

        public IState Current { get; private set; }

        public StateMachine(IState current)
        {
            Current = current;
        }

        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            throw new NotImplementedException();
            
            Transition t = new Transition(from, to, condition);

            if (conditions.TryGetValue(from, out List<Transition> transitions))
            {
                transitions.Add(t);
            }
            else
            {
                conditions.Add(from, new List<Transition> {t});
            }
        }

        public void Update() => Current.Update();

        public void ChangeState(IState to)
        {
            if (Current.Equals(to))
                return;

            Current.OnExit();
            Current = to;
            Current.OnEnter();
        }

        public void ForceChangeState(IState to)
        {
            Current = to;
            Current.OnEnter();
        }
    }
}