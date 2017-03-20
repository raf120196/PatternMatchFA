using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchFA
{
    class Relations
    {
        private SortedSet<string> alphabet = new SortedSet<string>();
        private List<StateOfFA> states = new List<StateOfFA>();
        private SortedDictionary<string, List<Pair<StateOfFA, StateOfFA>>> allRelations 
            = new SortedDictionary<string, List<Pair<StateOfFA, StateOfFA>>>();
        public List<Pair<StateOfFA, StateOfFA>> delta_w = new List<Pair<StateOfFA, StateOfFA>>();

        public void AddToAlphabet(string s)
        {
            if (!alphabet.Contains(s))
            {
                alphabet.Add(s);
            }
        }

        public void AddToStates(StateOfFA s)
        {
            if (!states.Contains(s))
            {
                states.Add(s);
            }
        }

        public void AllTransitions()
        {
            foreach (StateOfFA st in states)
            {
                foreach (string s in alphabet)
                {
                    StateOfFA[] to = st.GetTransitions(s);

                    for (int i = 0; to != null && i < to.Length; i++)
                    {
                        if (!allRelations.ContainsKey(s))
                        {
                            List<Pair<StateOfFA, StateOfFA>> list = new List<Pair<StateOfFA, StateOfFA>>();
                            list.Add(new Pair<StateOfFA, StateOfFA>(st, to[i]));
                            allRelations.Add(s, list);
                        }
                        else
                        {
                            allRelations[s].Add(new Pair<StateOfFA, StateOfFA>(st, to[i]));                           
                        }
                    }
                }
            }
        }

        public StateOfFA[] GetSecondProjection(List<Pair<StateOfFA, StateOfFA>> list)
        {
            List<StateOfFA> x = new List<StateOfFA>();

            foreach (Pair<StateOfFA, StateOfFA> pair in list)
            {
                if (!x.Contains(pair.Second))
                {
                    x.Add(pair.Second);
                }
            }

            if (x.Count != 0)
            {
                return x.ToArray();
            }

            return null;
        }

        public StateOfFA[] GetSecondProjection()
        {
            List<StateOfFA> x = new List<StateOfFA>();

            foreach (Pair<StateOfFA, StateOfFA> pair in delta_w)
            {
                if (!x.Contains(pair.Second))
                {
                    x.Add(pair.Second);
                }
            }

            if (x.Count != 0)
            {
                return x.ToArray();
            }

            return null;
        }

        public bool HaveTerminalState(StateOfFA[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].TerminalState)
                {
                    return true;
                }
            }

            return false;
        }

        public void init(StateOfFA state)
        {
            foreach (string s in alphabet)
            {
                foreach (Pair<StateOfFA, StateOfFA> pair in allRelations[s])
                {
                    if (pair.First == state)
                    {
                        delta_w.Add(pair);
                    }
                }
            }

            GoByEps(ref delta_w);
        }

        private void GoByEps(ref List<Pair<StateOfFA, StateOfFA>> list)
        {
            while (true)
            {
                List<Pair<StateOfFA, StateOfFA>> tmp = Multiply(list, MetaSymbols.EPSILON);
                if (tmp != null)
                {
                    list.AddRange(tmp);
                }
                else
                {
                    break;
                }
            }
        }

        public List<Pair<StateOfFA, StateOfFA>> Multiply(List<Pair<StateOfFA, StateOfFA>> list, string s)
        {
            List<Pair<StateOfFA, StateOfFA>> ans = new List<Pair<StateOfFA, StateOfFA>>();

            foreach (Pair<StateOfFA, StateOfFA> pair1 in list)
            {
                foreach (Pair<StateOfFA, StateOfFA> pair2 in allRelations[s])
                {
                    if (pair1.Second == pair2.First)
                    {
                        ans.Add(new Pair<StateOfFA, StateOfFA>(pair1.First, pair2.Second));
                    }
                }
            }

            GoByEps(ref ans);

            if (ans.Count != 0)
            {
                return ans;
            }

            return null;
        }
    }
}
