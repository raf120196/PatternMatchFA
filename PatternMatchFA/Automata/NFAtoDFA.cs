using System.Collections;

namespace PatternMatchFA
{
    internal class NFAtoDFA
    {
        private Hashtable hashTableStates = new Hashtable();

        private class DFAState
        {
            public Set setEpsClosure = null;
            public bool marked = false;
        }

        public void AddDFAState(StateOfFA state, Set setEpsClosure)
        {
            DFAState dfaState = new DFAState();
            dfaState.setEpsClosure = setEpsClosure;
            hashTableStates[state] = dfaState;
        }

        public StateOfFA FindEpsClosureStates(Set setEpsClosure)
        {       
            foreach (DictionaryEntry obj in hashTableStates)
            {
                DFAState dfaState = (DFAState)obj.Value;

                if (dfaState.setEpsClosure.IsEqual(setEpsClosure))
                {
                    return (StateOfFA)obj.Key;
                }
            }

            return null;
        }

        public Set GetEpsClosureDFAState(StateOfFA state)
        {
            DFAState dfaState = (DFAState)hashTableStates[state];

            if (dfaState != null)
            {
                return dfaState.setEpsClosure;
            }

            return null;
        }

        public StateOfFA GetNextUnmarkedDFAState()
        {
            foreach (DictionaryEntry obj in hashTableStates)
            {
                DFAState dfaState = (DFAState)obj.Value;

                if (!dfaState.marked)
                {
                    return (StateOfFA)obj.Key;
                }
            }

            return null;
        }

        public void Mark(StateOfFA state)
        {
            DFAState stateRecord = (DFAState)hashTableStates[state];
            stateRecord.marked = true;
        }

        public Set GetAllDfaState()
        {
            Set setState = new Set();

            foreach (object obj in hashTableStates.Keys)
            {
                setState.AddElement(obj);
            }

            return setState;
        }
    }
}
