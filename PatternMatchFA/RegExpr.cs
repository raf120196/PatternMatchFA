using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace PatternMatchFA
{
    class RegExpr
    {
        private static CheckerRegex checker = new CheckerRegex();
        private int lastErrPos = -1;
        private int lastErrLen = -1;
        private TypeError lastErr = TypeError.ERROR_NO;
        private bool matchStart = false;
        private bool matchEnd = false;
        private bool isGreedy = false;
        private StateOfFA initialState = null;
        private Set setOfInputSymbols = new Set();

        private string PostfixNotation(string pattern)
        {
            Stack operators = new Stack();
            Queue postfix = new Queue();

            bool needEscape = false;

            for (int i = 0; i < pattern.Length; i++)
            {
                char c = pattern[i];

                if (!needEscape && c == MetaSymbols.ESCAPE)
                {
                    postfix.Enqueue(c);
                    needEscape = true;
                    continue;
                }

                if (needEscape)
                {
                    postfix.Enqueue(c);
                    needEscape = false;
                    continue;
                }

                switch (c)
                {
                    case MetaSymbols.OPEN_PREN:
                        operators.Push(c);
                        break;
                    case MetaSymbols.CLOSE_PREN:
                        while ((char)operators.Peek() != MetaSymbols.OPEN_PREN)
                        {
                            postfix.Enqueue(operators.Pop());
                        }
                        operators.Pop();
                        break;
                    default:
                        while (operators.Count > 0)
                        {
                            char ch = (char)operators.Peek();

                            int peekPrior = GetPriority(ch);
                            int currPrior = GetPriority(c);

                            if (peekPrior >= currPrior)
                            {
                                postfix.Enqueue(operators.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                        operators.Push(c);
                        break;
                }
            }

            while (operators.Count > 0)
            {
                postfix.Enqueue((char)operators.Pop());
            }
            StringBuilder s = new StringBuilder(1024);
            while (postfix.Count > 0)
            {
                s.Append((char)postfix.Dequeue());
            }
                
            return s.ToString();
        }

        private int GetPriority(char c)
        {
            switch (c)
            {
                case MetaSymbols.OPEN_PREN:
                    return 0;
                case MetaSymbols.ALTERNATE:
                    return 1;
                case MetaSymbols.CONCANATE:
                    return 2;
                case MetaSymbols.ZERO_OR_ONE:
                case MetaSymbols.ZERO_OR_MORE:
                case MetaSymbols.ONE_OR_MORE:
                    return 3;
                case MetaSymbols.COMPLEMENT:
                    return 4;
                default:
                    return 5;
            }
        }

        public TypeError CompilationWithLog(string pattern, StringBuilder log)
        {
            StateOfFA.ResetCounter();
            int lineLen = 0;

            InfoCheck inf = checker.Check(pattern);
            UpdateCheckInfo(inf);

            if (inf.TypeError != TypeError.ERROR_NO)
            {
                return inf.TypeError;
            }

            string postfixNotation = PostfixNotation(inf.FormattedString);

            log.AppendLine("Регулярное выражение:\t\t\t" + pattern);
            log.AppendLine("Отформатированное регулярное выражение:\t" + inf.FormattedString);
            log.AppendLine("Постфиксная запись:\t\t\t" + postfixNotation);
            log.AppendLine();

            StateOfFA stateStartNfa = CreateNFA(postfixNotation);
            log.AppendLine();
            log.AppendLine("Таблица переходов НКА:");
            lineLen = GetSerializedFsa(stateStartNfa, log);
            log.AppendLine();

            StateOfFA.ResetCounter();
            StateOfFA stateStartDfa = NFAtoDFA(stateStartNfa);
            log.AppendLine();
            log.AppendLine("Таблица переходов ДКА:");
            lineLen = GetSerializedFsa(stateStartDfa, log);
            log.AppendLine();

            StateOfFA stateStartDfaM = Minimization(stateStartDfa);
            initialState = stateStartDfaM;
            log.AppendLine();
            log.AppendLine("Таблица переходов минимального ДКА:");
            lineLen = GetSerializedFsa(stateStartDfaM, log);
            log.AppendLine();

            return TypeError.ERROR_NO;
        }

        private void UpdateCheckInfo(InfoCheck inf)
        {
            if (inf.TypeError == TypeError.ERROR_NO)
            {
                matchStart = inf.MatchAtStart;
                matchEnd = inf.MatchAtEnd;
            }

            lastErr = inf.TypeError;
            lastErrPos = inf.ErrorStartAt;
            lastErrLen = inf.ErrorLength;
        }

        private Set EpsClosure(StateOfFA initialState) // находит множество состояний, достижимых из текущей по epsilon
        {
            Set procc = new Set();
            Set unprocc = new Set();

            unprocc.AddElement(initialState);

            while (unprocc.Count > 0)
            {
                StateOfFA state = (StateOfFA)unprocc[0];
                StateOfFA[] arr = state.GetTransitions(MetaSymbols.EPSILON);
                procc.AddElement(state);
                unprocc.RemoveElement(state);

                if (arr != null)
                {
                    foreach (StateOfFA state_eps in arr)
                    {
                        if (!procc.ElementExist(state_eps))
                        {
                            unprocc.AddElement(state_eps);
                        }
                    }
                }
            }

            return procc;
        }

        private Set EpsClosure(Set set) // находит множество состояний, достижимых из set по epsilon
        {
            Set allStates = new Set();

            foreach (object obj in set)
            {
                StateOfFA state = (StateOfFA)obj;

                Set setEpsClosure = EpsClosure(state);
                allStates.Union(setEpsClosure);
            }

            return allStates;
        }

        private Set Transition(StateOfFA state, string s) // находит множество состояний, достижимых из state по s
        {
            Set set = new Set();
            StateOfFA[] arr = state.GetTransitions(s);

            if (arr != null)
            {
                set.AddElementRange(arr);
            }

            return set;
        }

        private Set Transition(Set setState, string s) // находит множество состояний, достижимых из множества setState по s
        {
            Set set = new Set();

            foreach (object obj in setState)
            {
                StateOfFA state = (StateOfFA)obj;
                Set setMove = Transition(state, s);
                set.Union(setMove);
            }

            return set;
        }

        public bool IsGreedy
        {
            get
            {
                return isGreedy;
            }
            set
            {
                isGreedy = value;
            }
        }

        public int GetLastErrorLength()
        {
            return lastErrLen;
        }

        public TypeError GetLastError()
        {
            return lastErr;
        }

        public int GetLastErrorPosition()
        {
            return lastErrPos;
        }

        public bool IsReady()
        {
            return (initialState != null);
        }

        private StateOfFA CreateNFA(string postfixNotation) // построение конечного недетерминированного автомата
        {
            Stack stackNFA = new Stack();
            Transition t = null;
            Transition t1 = null;
            Transition t2 = null;
            Transition t3 = null;
            bool needEscape = false;

            foreach (char c in postfixNotation)
            {
                if (needEscape == false && c == MetaSymbols.ESCAPE)
                {
                    needEscape = true;
                    continue;
                }

                if (needEscape == true)
                {
                    t3 = new Transition();
                    t3.GetStartState().AddTransition(c.ToString(), t3.GetFinalState());

                    stackNFA.Push(t3);

                    needEscape = false;
                    continue;
                }

                switch (c)
                {
                    case MetaSymbols.ZERO_OR_MORE:  // Замыкание Клини *
                        t1 = (Transition)stackNFA.Pop();
                        t3 = new Transition();

                        t1.GetFinalState().AddTransition(MetaSymbols.EPSILON, t1.GetStartState());
                        t1.GetFinalState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());

                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t1.GetStartState());
                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());

                        stackNFA.Push(t3);
                        break;

                    case MetaSymbols.ALTERNATE:  // Альтернативность |
                        t2 = (Transition)stackNFA.Pop();
                        t1 = (Transition)stackNFA.Pop();

                        t3 = new Transition();

                        t1.GetFinalState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());
                        t2.GetFinalState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());

                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t1.GetStartState());
                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t2.GetStartState());

                        stackNFA.Push(t3);
                        break;

                    case MetaSymbols.CONCANATE:  // Конкатенация .
                        t2 = (Transition)stackNFA.Pop();
                        t1 = (Transition)stackNFA.Pop();

                        t1.GetFinalState().AddTransition(MetaSymbols.EPSILON, t2.GetStartState());

                        t3 = new Transition(t1.GetStartState(), t2.GetFinalState());
                        stackNFA.Push(t3);
                        break;

                    case MetaSymbols.ONE_OR_MORE:  // Плюсик + (X.X*)

                        t1 = (Transition)stackNFA.Pop();
                        t3 = new Transition();

                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t1.GetStartState());
                        t3.GetFinalState().AddTransition(MetaSymbols.EPSILON, t1.GetStartState());
                        t1.GetFinalState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());

                        stackNFA.Push(t3);
                        break;

                    case MetaSymbols.ZERO_OR_ONE:  // Вопросик ? (X | e)
                        t1 = (Transition)stackNFA.Pop();
                        t3 = new Transition();

                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t1.GetStartState());
                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());
                        t1.GetFinalState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());

                        stackNFA.Push(t3);
                        break;

                    case MetaSymbols.ANY_ONE_CHAR: // один произвольный символ _
                        t3 = new Transition();
                        t3.GetStartState().AddTransition(MetaSymbols.ANY_ONE_CHAR_TRANS, t3.GetFinalState());
                        stackNFA.Push(t3);
                        break;

                    case MetaSymbols.COMPLEMENT:  // Дополнение ^ 
                        t1 = (Transition)stackNFA.Pop();

                        Transition d = new Transition();
                        d.GetStartState().AddTransition(MetaSymbols.DUMMY, d.GetFinalState());
                        t1.GetFinalState().AddTransition(MetaSymbols.EPSILON, d.GetStartState());

                        Transition exprAny = new Transition();
                        exprAny.GetStartState().AddTransition(MetaSymbols.ANY_ONE_CHAR_TRANS, exprAny.GetFinalState());

                        t3 = new Transition();
                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, t1.GetStartState());
                        t3.GetStartState().AddTransition(MetaSymbols.EPSILON, exprAny.GetStartState());

                        exprAny.GetFinalState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());
                        d.GetFinalState().AddTransition(MetaSymbols.EPSILON, t3.GetFinalState());

                        stackNFA.Push(t3);
                        break;

                    default:
                        t3 = new Transition();
                        t3.GetStartState().AddTransition(c.ToString(), t3.GetFinalState());

                        stackNFA.Push(t3);
                        break;
                }
            }

            Debug.Assert(stackNFA.Count == 1);
            t = (Transition)stackNFA.Pop();
            t.GetFinalState().TerminalState = true;

            return t.GetStartState();
        }

        private StateOfFA NFAtoDFA(StateOfFA startState) // детерминизация конечного автомата
        {
            Set allInput = new Set();
            Set allStates = new Set();

            GetAllStateAndInput(startState, allStates, allInput);
            allInput.RemoveElement(MetaSymbols.EPSILON);

            NFAtoDFA convert = new NFAtoDFA();
            Set setMove = null;
            Set setEpsClosure = null;

            setEpsClosure = EpsClosure(startState);
            StateOfFA stateStartDFA = new StateOfFA();

            if (IsTerminalGroup(setEpsClosure) == true)
            {
                stateStartDFA.TerminalState = true;
            }

            convert.AddDFAState(stateStartDFA, setEpsClosure);

            string s = String.Empty;

            StateOfFA state1 = null;
            Set setT = null;
            StateOfFA stateDFA = null;

            while ((state1 = convert.GetNextUnmarkedDFAState()) != null)
            {
                convert.Mark(state1);
                setT = convert.GetEpsClosureDFAState(state1);

                foreach (object obj in allInput)
                {
                    s = obj.ToString();
                    setMove = Transition(setT, s);

                    if (!setMove.IsEmpty())
                    {
                        setEpsClosure = EpsClosure(setMove);
                        stateDFA = convert.FindEpsClosureStates(setEpsClosure);

                        if (stateDFA == null)
                        {
                            stateDFA = new StateOfFA();
                            if (IsTerminalGroup(setEpsClosure))
                            {
                                stateDFA.TerminalState = true;
                            }

                            convert.AddDFAState(stateDFA, setEpsClosure);
                        }

                        state1.AddTransition(s, stateDFA);
                    }
                }
            }

            return stateStartDFA;
        }

        private bool IsTerminalGroup(Set set) // проверяет, есть ли среди состояний множества заключительное
        {
            foreach (object objState in set)
            {
                StateOfFA state = (StateOfFA)objState;
                if (state.TerminalState == true)
                {
                    return true;
                }
            }
            return false;
        }

        static internal void GetAllStateAndInput(StateOfFA stateStart, Set allStates, Set allInput)
        {
            Set unprocc = new Set();
            unprocc.AddElement(stateStart);

            while (unprocc.Count > 0)
            {
                StateOfFA state = (StateOfFA)unprocc[0];

                allStates.AddElement(state);
                unprocc.RemoveElement(state);

                foreach (object obj in state.GetAllKeys())
                {
                    string s = (string)obj;
                    allInput.AddElement(s);

                    StateOfFA[] arr = state.GetTransitions(s);
                    if (arr != null)
                    {
                        foreach (StateOfFA st in arr)
                        {
                            if (!allStates.ElementExist(st))
                            {
                                unprocc.AddElement(st);
                            }
                        }
                    }
                }
            }
        }

        private StateOfFA Minimization(StateOfFA initialState) // Минимизация автомата
        {
            Set allInput = new Set();
            Set allStates = new Set();

            GetAllStateAndInput(initialState, allStates, allInput);

            StateOfFA newInitialState = null;
            ArrayList arrGroup = null;

            arrGroup = PartitionDFAGroups(allStates, allInput);

            foreach (object obj in arrGroup)
            {
                Set setGroup = (Set)obj;

                bool isAcceptingGroup = IsTerminalGroup(setGroup);
                bool isStartingGroup = setGroup.ElementExist(initialState);

                StateOfFA stateRepresentative = (StateOfFA)setGroup[0];
                if (isStartingGroup)
                {
                    newInitialState = stateRepresentative;
                }
                if (isAcceptingGroup)
                {
                    stateRepresentative.TerminalState = true;
                }
                if (setGroup.GetCardinality() == 1)
                {
                    continue;
                }

                setGroup.RemoveElement(stateRepresentative);

                StateOfFA stateToBeReplaced = null;
                int nReplecementCount = 0;
                foreach (object objStateToReplaced in setGroup)
                {
                    stateToBeReplaced = (StateOfFA)objStateToReplaced;
                    allStates.RemoveElement(stateToBeReplaced);

                    foreach (object objState in allStates)
                    {
                        StateOfFA state = (StateOfFA)objState;
                        nReplecementCount += state.ChangeTransition(stateToBeReplaced, stateRepresentative);
                    }
                }
            }
            
            int idx = 0;
            while (idx < allStates.Count)
            {
                StateOfFA state = (StateOfFA)allStates[idx];
                if (state.IsItDeadState())
                {
                    allStates.RemoveAt(idx);
                    continue;
                }
                idx++;
            }

            return newInitialState;
        }

        private ArrayList PartitionDFAGroups(Set setMasterDfa, Set setInputSymbol)
        {
            ArrayList arrGroup = new ArrayList();
            Map map = new Map();
            Set setEmpty = new Set();
            Set setAccepting = new Set();
            Set setNonAccepting = new Set();

            foreach (object objState in setMasterDfa)
            {
                StateOfFA state = (StateOfFA)objState;

                if (state.TerminalState)
                {
                    setAccepting.AddElement(state);
                }
                else
                {
                    setNonAccepting.AddElement(state);
                }
            }

            if (setNonAccepting.GetCardinality() > 0)
            {
                arrGroup.Add(setNonAccepting);
            }

            arrGroup.Add(setAccepting);
          
            IEnumerator iterInput = setInputSymbol.GetEnumerator();
            iterInput.Reset();

            while (iterInput.MoveNext())
            {
                string sInputSymbol = iterInput.Current.ToString();

                int nPartionIndex = 0;
                while (nPartionIndex < arrGroup.Count)
                {
                    Set setToBePartitioned = (Set)arrGroup[nPartionIndex];
                    nPartionIndex++;

                    if (setToBePartitioned.IsEmpty() || setToBePartitioned.GetCardinality() == 1)
                    {
                        continue;
                    }

                    foreach (object objState in setToBePartitioned)
                    {
                        StateOfFA state = (StateOfFA)objState;
                        StateOfFA[] arrState = state.GetTransitions(sInputSymbol.ToString());

                        if (arrState != null)
                        {
                            Debug.Assert(arrState.Length == 1);

                            StateOfFA stateTransionTo = arrState[0];

                            Set setFound = FindGroup(arrGroup, stateTransionTo);
                            map.Add(setFound, state);
                        }
                        else
                        {
                            map.Add(setEmpty, state);

                        }
                    }

                    if (map.Count > 1) 
                    {
                        arrGroup.Remove(setToBePartitioned);
                        foreach (DictionaryEntry de in map)
                        {
                            Set setValue = (Set)de.Value;
                            arrGroup.Add(setValue);
                        }
                        nPartionIndex = 0;
                        iterInput.Reset();
                    }
                    map.Clear();
                }
            }

            return arrGroup;
        }

        private Set FindGroup(ArrayList arrGroup, StateOfFA state)
        {
            foreach (object objSet in arrGroup)
            {
                Set set = (Set)objSet;
                if (set.ElementExist(state) == true)
                {
                    return set;
                }
            }

            return null;
        }

        private string SetToString(Set set)
        {
            string s = "";
            foreach (object objState in set)
            {
                StateOfFA state = (StateOfFA)objState;
                s += state.GetID.ToString() + ", ";
            }

            s = s.TrimEnd(new char[] { ' ', ',' });
            if (s.Length == 0)
            {
                s = "Empty";
            }
            s = "{" + s + "}";
            return s;
        }

        static internal int GetSerializedFsa(StateOfFA stateStart, StringBuilder sb)
        {
            Set setAllState = new Set();
            Set setAllInput = new Set();
            GetAllStateAndInput(stateStart, setAllState, setAllInput);
            return GetSerializedFsa(stateStart, setAllState, setAllInput, sb);
        }
        static internal int GetSerializedFsa(StateOfFA stateStart, Set setAllState, Set setAllSymbols, StringBuilder sb)
        {
            int nLineLength = 0;
            int nMinWidth = 6;
            string sLine = String.Empty;
            string sFormat = String.Empty;
            setAllSymbols.RemoveElement(MetaSymbols.EPSILON);
            setAllSymbols.AddElement(MetaSymbols.EPSILON);

            object[] arrObj = new object[setAllSymbols.Count + 1];
            arrObj[0] = "Состояние";
            sFormat = "{0,-12}";
            for (int i = 0; i < setAllSymbols.Count; i++)
            {
                string sSymbol = setAllSymbols[i].ToString();
                arrObj[i + 1] = sSymbol;

                sFormat += " | ";
                sFormat += "{" + (i + 1).ToString() + ",-" + Math.Max(Math.Max(sSymbol.Length, nMinWidth), sSymbol.ToString().Length) + "}";
            }
            sLine = String.Format(sFormat, arrObj);
            nLineLength = Math.Max(nLineLength, sLine.Length);
            sb.AppendLine(("").PadRight(nLineLength, '-'));
            sb.AppendLine(sLine);
            sb.AppendLine(("").PadRight(nLineLength, '-'));


            int nTransCount = 0;
            foreach (object objState in setAllState)
            {
                StateOfFA state = (StateOfFA)objState;
                arrObj[0] = (state.Equals(stateStart) ? "->" + state.ToString() : state.ToString());

                for (int i = 0; i < setAllSymbols.Count; i++)
                {
                    string sSymbol = setAllSymbols[i].ToString();

                    StateOfFA[] arrStateTo = state.GetTransitions(sSymbol);
                    string sTo = String.Empty;
                    if (arrStateTo != null)
                    {
                        nTransCount += arrStateTo.Length;
                        sTo = arrStateTo[0].ToString();

                        for (int j = 1; j < arrStateTo.Length; j++)
                        {
                            sTo += ", " + arrStateTo[j].ToString();
                        }
                    }
                    else
                    {
                        sTo = "--";
                    }
                    arrObj[i + 1] = sTo;
                }

                sLine = String.Format(sFormat, arrObj);
                sb.AppendLine(sLine);
                nLineLength = Math.Max(nLineLength, sLine.Length);
            }

            sFormat = "Количество состояний: {0}, \nКоличество входных символов: {1}, \nКоличество переходов: {2}";
            sLine = String.Format(sFormat, setAllState.Count, setAllSymbols.Count, nTransCount);
            nLineLength = Math.Max(nLineLength, sLine.Length);
            sb.AppendLine(("").PadRight(nLineLength, '-'));
            sb.AppendLine(sLine);
            nLineLength = Math.Max(nLineLength, sLine.Length);
            setAllSymbols.RemoveElement(MetaSymbols.EPSILON);

            return nLineLength;
        }

        public bool FindMatch(string text, int startPos, int endPos, ref int foundStartPos, ref int foundEndPos)
        {
            if (initialState == null || startPos < 0)
            {
                return false;
            }

            StateOfFA stateStart = initialState;

            foundStartPos = -1;
            foundEndPos = -1;

            bool isTerminal = false;
            StateOfFA toState = null;
            StateOfFA currentState = stateStart;
            int idx = startPos;
            int lastPos = endPos;

            while (idx <= lastPos)
            {
                if (isGreedy && IsAnyOneChar(currentState) == true) // _*
                {
                    if (foundStartPos == -1)
                    {
                        foundStartPos = idx;
                    }
                    AnyOneChar(currentState, text, ref idx, lastPos);
                }

                char c = text[idx];
                toState = currentState.GetTransition(c.ToString()); // перешли по символу
                if (toState == null)
                {
                    toState = currentState.GetTransition(MetaSymbols.ANY_ONE_CHAR_TRANS);
                }

                if (toState != null) // если есть такое состояние
                {
                    if (foundStartPos == -1)
                    {
                        foundStartPos = idx; // отсюда начинаем смотреть
                    }

                    if (toState.TerminalState) // если перешли в заключительное
                    {
                        if (!matchEnd || idx == lastPos)
                        {                           
                            isTerminal = true;
                            foundEndPos = idx;
                            if (!isGreedy)
                            {
                                break;
                            }
                        }
                    }

                    currentState = toState;
                    idx++;
                }
                else
                {
                    if (!matchStart && !isTerminal) // сбросим показатели
                    {
                        idx = ((foundStartPos != -1) ? (foundStartPos + 1) : (idx + 1));
                        foundStartPos = -1;
                        foundEndPos = -1;
                        currentState = stateStart;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!isTerminal)
            {
                if (!stateStart.TerminalState)
                {
                    return false;
                }
                else
                {
                    foundStartPos = startPos;
                    foundEndPos = foundStartPos - 1;
                    return true;
                }
            }

            return true;
        }

        private bool IsAnyOneChar(StateOfFA s) // x_*
        {
            return (s == s.GetTransition(MetaSymbols.ANY_ONE_CHAR_TRANS));
        }

        private void AnyOneChar(StateOfFA s, string text, ref int currentIdx, int lastPos)
        {
            int k = currentIdx;
            while (k <= lastPos)
            {
                char c = text[k];
                StateOfFA toState = s.GetTransition(c.ToString());
                if (toState != null)
                {
                    currentIdx = k;
                }

                k++;
            }
        }
    }
}