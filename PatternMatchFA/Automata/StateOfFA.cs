using System.Collections;

namespace PatternMatchFA
{
    internal class StateOfFA
    {
        private static int stateID = 0;
        public Map transitions = new Map();
        private int curID = 0;

        public StateOfFA()
        {
            this.curID = stateID++;
        }

        public int GetID
        {
            get
            {
                return curID;
            }
        }

        public void AddTransition(string label, StateOfFA to)
        {
            transitions.Add(label, to);
        }

        public StateOfFA[] GetTransitions(string s)
        {
            if (transitions.Contains(s))
            {
                Set set = (Set)transitions[s];
                return (StateOfFA[])set.ToArray(typeof(StateOfFA));
            }

            return null;
        }

        public StateOfFA GetTransition(string s)
        {
            if (transitions.Contains(s))
            {
                Set set = (Set)transitions[s];
                return (StateOfFA)set[0];
            }

            return null;
        }

        public void DeleteTransition(string state)
        {
            if (transitions.Contains(state))
            {
                transitions.Remove(state);
            }
        }

        public int ChangeTransition(StateOfFA old, StateOfFA @new)
        {
            int count = 0;

            foreach(DictionaryEntry obj in transitions)
            {
                Set set = (Set)obj.Value;
                if (set.ElementExist(old))
                {
                    set.RemoveElement(old);
                    set.AddElement(@new);
                    count++;
                }
            }

            return count;
        }

        private bool terminalState = false;

        public bool TerminalState
        {
            get
            {
                return terminalState;
            }
            set
            {
                terminalState = value;
            }
        }

        public bool IsItDeadState()
        {
            if (terminalState)
            {
                return false;
            }

            if (transitions.Count == 0)
            {
                return false;
            }

            foreach (DictionaryEntry obj in transitions)
            {
                Set set = (Set)obj.Value;

                foreach (object o in set)
                {
                    StateOfFA state = (StateOfFA)o;
                    if (!state.Equals(this))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void ResetCounter()
        {
            stateID = 0;
        }

        public ICollection GetAllKeys()
        {
            return transitions.Keys;
        }

        public override string ToString()
        {
            string s = "s" + this.GetID.ToString();

            if (this.TerminalState)
            {
                s = "/" + s + "/";
            }

            return s;
        }
    }
}
