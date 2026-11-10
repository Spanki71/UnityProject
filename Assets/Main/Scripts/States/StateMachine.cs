using System;
using System.Collections.Generic;
using Main.Interfaces;

namespace Main.Scripts.States
{
    public class StateMachine
    {
        private IGameState m_currentState => m_stateStack?.Count > 0 ? m_stateStack.Peek() : null;
        private Stack<IGameState> m_stateStack = new();
        private readonly Dictionary<Type, IGameState> m_states = new Dictionary<Type, IGameState>();

        public void Register(IGameState state)
        {
            m_states.Add(state.GetType(), state);
        }

        public void ChangeState<T>() where T : IGameState
        {
            while (m_stateStack.Count > 0)
            {
                m_stateStack.Pop().OnExit();
            }

            PushState<T>();
        }

        public void PushState<T>() where T : IGameState
        {
            IGameState nextState = (m_states[typeof(T)]);
            m_stateStack.Push(nextState);
            nextState.OnEnter();
        }

        public void PopState()
        {
            m_stateStack?.Pop();
        }

        public void Update()
        {
            if (m_currentState is IUpdateable updateable)
            {
                updateable.Update();
            }
        }
    }
}