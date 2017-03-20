using System;
using System.Text;

namespace PatternMatchFA
{
    internal class InfoCheck
    {
        public TypeError TypeError = TypeError.ERROR_NO;
        public int ErrorStartAt = -1;
        public int ErrorLength = -1;
        public string FormattedString = String.Empty;
        public bool MatchAtStart = false;
        public bool MatchAtEnd = false;
    }       

    internal class CheckerRegex
    {
        private InfoCheck checkInfo = null;
        private int currentPos = -1;
        private int patternLength = -1;
        private const char CHR_NULL = '\0';
        private char symb = CHR_NULL;
        private string pattern = String.Empty;
        private StringBuilder str = null;
        private bool mayConcanate = false;
        private bool mayAlternate = false;

        public InfoCheck Check(string pattern)
        {
            checkInfo = new InfoCheck();
            currentPos = -1;
            patternLength = pattern.Length;
            symb = CHR_NULL;
            this.pattern = pattern;
            str = new StringBuilder(1024);
            mayConcanate = false;
            mayAlternate = false;

            if (pattern.Length == 0)
            {
                checkInfo.TypeError = TypeError.ERROR_EMPTY_STRING;
                return checkInfo;
            }

            GetNextSymbol();

            if (!(pattern.CompareTo(MetaSymbols.MATCH_START.ToString()) == 0 || pattern.CompareTo(MetaSymbols.MATCH_END.ToString()) == 0 ||
                pattern.CompareTo(MetaSymbols.MATCH_START.ToString() + MetaSymbols.MATCH_END.ToString()) == 0))
            {
                if (pattern[0] == MetaSymbols.MATCH_START)
                {
                    checkInfo.MatchAtStart = true;
                    Accept(MetaSymbols.MATCH_START);
                }

                if (pattern[patternLength - 1] == MetaSymbols.MATCH_END)
                {
                    checkInfo.MatchAtEnd = true;
                    patternLength--;
                }
            }

            try
            {
                while (currentPos < patternLength)
                {
                    switch (symb)
                    {
                        case MetaSymbols.ALTERNATE:
                        case MetaSymbols.ZERO_OR_MORE:
                        case MetaSymbols.ONE_OR_MORE:                        
                        case MetaSymbols.ZERO_OR_ONE:
                            Abort(TypeError.ERROR_OPERAND_MISSING, currentPos, 1);
                            break;
                        case MetaSymbols.CLOSE_PREN:
                            Abort(TypeError.ERROR_PREN_MISMATCH, currentPos, 1);
                            break;
                        default:
                            Expression();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            checkInfo.FormattedString = str.ToString();
            return checkInfo;
        }

        private void GetNextSymbol()
        {
            currentPos++;
            if (currentPos < patternLength)
            {
                symb = pattern[currentPos];
            }
            else
            {
                symb = CHR_NULL;
            }
        }

        private bool Accept(char c)
        {
            if (symb == c)
            {
                GetNextSymbol();
                return true;
            }

            return false;
        }

        private void Abort(TypeError err, int errPos, int errLen)
        {
            checkInfo.TypeError = err;
            checkInfo.ErrorStartAt = errPos;
            checkInfo.ErrorLength = errLen;

            throw new Exception("Syntex error.");
        }

        private void AppendConcanate()
        {
            if (mayConcanate)
            {
                str.Append(MetaSymbols.CONCANATE);
                mayConcanate = false;
            }
        }

        private void AppendAlternate()
        {
            if (mayAlternate)
            {
                str.Append(MetaSymbols.ALTERNATE);
                mayAlternate = false;
            }
        }

        private bool AcceptPostfixOperator()
        {
            switch (symb)
            {
                case MetaSymbols.ONE_OR_MORE:
                case MetaSymbols.ZERO_OR_MORE:
                case MetaSymbols.ZERO_OR_ONE:
                    str.Append(symb);
                    return Accept(symb);
                default:
                    return false;
            }
        }

        private bool AcceptNotEscapeChar()
        {
            switch (symb)
            {
                case MetaSymbols.ALTERNATE:
                case MetaSymbols.CHARSET_START:
                case MetaSymbols.CLOSE_PREN:
                case MetaSymbols.ESCAPE:
                case MetaSymbols.ONE_OR_MORE:
                case MetaSymbols.OPEN_PREN:
                case MetaSymbols.ZERO_OR_MORE:
                case MetaSymbols.ZERO_OR_ONE:
                case MetaSymbols.CONCANATE:
                case CHR_NULL:
                    return false;
                default:
                    AppendConcanate();
                    str.Append(symb);
                    Accept(symb);
                    break;
            }
            return true;
        }

        private bool Expect(char c)
        {
            if (Accept(c))
            {
                return true;
            }
            return false;
        }

        private bool ExpectEscapeChar()
        {
            switch (symb)
            {
                case MetaSymbols.ALTERNATE:
                case MetaSymbols.ANY_ONE_CHAR:
                case MetaSymbols.CHARSET_START:
                case MetaSymbols.CLOSE_PREN:
                case MetaSymbols.COMPLEMENT:
                case MetaSymbols.ESCAPE:
                case MetaSymbols.ONE_OR_MORE:
                case MetaSymbols.OPEN_PREN:
                case MetaSymbols.ZERO_OR_MORE:
                case MetaSymbols.ZERO_OR_ONE:
                    str.Append(MetaSymbols.ESCAPE);
                    str.Append(symb);
                    Accept(symb);
                    break;
                case MetaSymbols.NEW_LINE:
                    str.Append('\n');
                    Accept(symb);
                    break;
                case MetaSymbols.TAB:
                    str.Append('\t');
                    Accept(symb);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void Expression()
        {
            while (Accept(MetaSymbols.ESCAPE))
            {
                AppendConcanate();
                if (!ExpectEscapeChar())
                {
                    Abort(TypeError.ERROR_INVALID_ESCAPE, currentPos - 1, 1);
                }
                AcceptPostfixOperator();
                mayConcanate = true;
            }

            while (Accept(MetaSymbols.CONCANATE))
            {
                AppendConcanate();
                str.Append(MetaSymbols.ESCAPE);
                str.Append(MetaSymbols.CONCANATE);
                AcceptPostfixOperator();
                mayConcanate = true;
            }

            while (Accept(MetaSymbols.COMPLEMENT))
            {
                AppendConcanate();
                str.Append(MetaSymbols.ESCAPE);
                str.Append(MetaSymbols.CONCANATE);
                AcceptPostfixOperator();
                mayConcanate = true;
            }

            while (AcceptNotEscapeChar())
            {
                AcceptPostfixOperator();
                mayConcanate = true;
                Expression();
            }

            if (Accept(MetaSymbols.OPEN_PREN))
            {
                int entryPoint = currentPos - 1;
                AppendConcanate();
                str.Append(MetaSymbols.OPEN_PREN);
                Expression();

                if (!Expect(MetaSymbols.CLOSE_PREN))
                {
                    Abort(TypeError.ERROR_PREN_MISMATCH, entryPoint, currentPos - entryPoint);
                }

                str.Append(MetaSymbols.CLOSE_PREN);

                if (currentPos - entryPoint == 2)
                {
                    Abort(TypeError.ERROR_EMPTY_PREN, entryPoint, currentPos - entryPoint);
                }

                AcceptPostfixOperator();
                mayConcanate = true;
                Expression();
            }

            if (Accept(MetaSymbols.CHARSET_START))
            {
                int entryPoint = currentPos - 1;

                AppendConcanate();

                bool complem = false;
                if (Accept(MetaSymbols.COMPLEMENT))
                {
                    complem = true;
                }

                string temp = str.ToString();

                str = new StringBuilder(1024);
                mayAlternate = false;
                SetOfChar();

                if (!Expect(MetaSymbols.CHARSET_END))
                {
                    Abort(TypeError.ERROR_BRACKET_MISMATCH, entryPoint, currentPos - entryPoint);
                }

                int lenBracket = currentPos - entryPoint;
                if (lenBracket == 2)
                {
                    Abort(TypeError.ERROR_EMPTY_BRACKET, entryPoint, currentPos - entryPoint);
                }
                else
                {
                    if (lenBracket == 3 && complem)
                    {
                        str = new StringBuilder(1024);
                        str.Append(temp);
                        str.Append(MetaSymbols.OPEN_PREN);
                        str.Append(MetaSymbols.ESCAPE);
                        str.Append(MetaSymbols.COMPLEMENT);
                        str.Append(MetaSymbols.CLOSE_PREN);
                    }
                    else
                    {
                        string setOfChar = str.ToString();
                        str = new StringBuilder(1024);
                        str.Append(temp);
                        if (complem)
                        {
                            str.Append(MetaSymbols.COMPLEMENT);
                        }
                        str.Append(MetaSymbols.OPEN_PREN);
                        str.Append(setOfChar);
                        str.Append(MetaSymbols.CLOSE_PREN);
                    }
                }

                AcceptPostfixOperator();
                mayConcanate = true;
                Expression();
            }

            if (Accept(MetaSymbols.ALTERNATE))
            {
                int entryPoint = currentPos - 1;
                mayConcanate = false;
                str.Append(MetaSymbols.ALTERNATE);
                Expression();

                if (currentPos - entryPoint == 1)
                {
                    Abort(TypeError.ERROR_OPERAND_MISSING, entryPoint, currentPos - entryPoint);
                }

                Expression();
            }
        }

        private string ExpectEscapeCharInBracket()
        {
            char c = symb;

            switch (symb)
            {
                case MetaSymbols.CHARSET_END:
                case MetaSymbols.ESCAPE:
                    Accept(symb);
                    return MetaSymbols.ESCAPE.ToString() + c.ToString();
                case MetaSymbols.NEW_LINE:                    
                    Accept(symb);
                    return ('\n').ToString();
                case MetaSymbols.TAB:
                    Accept(symb);
                    return ('\t').ToString();
                default:
                    return String.Empty;
            }
        }

        private string AcceptNotEscapeCharInBracket()
        {
            char c = symb;

            switch (c)
            {
                case MetaSymbols.CHARSET_END:
                case MetaSymbols.ESCAPE:
                case CHR_NULL:
                    return String.Empty;
                case MetaSymbols.ALTERNATE:
                case MetaSymbols.ANY_ONE_CHAR:
                case MetaSymbols.CLOSE_PREN:
                case MetaSymbols.COMPLEMENT:
                case MetaSymbols.ONE_OR_MORE:
                case MetaSymbols.OPEN_PREN:
                case MetaSymbols.ZERO_OR_MORE:
                case MetaSymbols.ZERO_OR_ONE:
                case MetaSymbols.CONCANATE:
                    Accept(symb);
                    return MetaSymbols.ESCAPE.ToString() + c.ToString();
                default:
                    Accept(symb);
                    return c.ToString();
            }
        }

        private void SetOfChar()
        {
            string left = String.Empty;
            string rang = String.Empty;
            string right = String.Empty;

            string temp = String.Empty;
            int start = -1;
            int beginRang = -1;
            int len = -1;
            while (true)
            {
                temp = String.Empty;

                start = currentPos;
                if (Accept(MetaSymbols.ESCAPE))
                {
                    if ((temp = AcceptNotEscapeCharInBracket()) == String.Empty)
                    {
                        Abort(TypeError.ERROR_INVALID_ESCAPE, currentPos - 1, 1);
                    }
                    len = 2;
                }
                
                if (temp == String.Empty)
                {
                    temp = AcceptNotEscapeCharInBracket();
                    len = 1;
                }

                if (temp == String.Empty)
                {
                    break;
                }

                if (left == String.Empty)
                {
                    beginRang = start;
                    left = temp;
                    AppendAlternate();
                    str.Append(temp);
                    mayAlternate = true;
                    continue;
                }

                if (rang == String.Empty)
                {
                    if (temp != MetaSymbols.RANGE.ToString())
                    {
                        beginRang = start;
                        left = temp;
                        AppendAlternate();
                        str.Append(temp);
                        mayAlternate = true;
                        continue;
                    }
                    else
                    {
                        rang = temp;
                    }
                    continue;
                }

                right = temp;

                bool expandRange = ExpandRange(left, right);

                if (!expandRange)
                {
                    int leng = (start + len) - beginRang;
                    Abort(TypeError.ERROR_INVALID_RANGE, beginRang, leng);
                }

                left = String.Empty;
                right = String.Empty;
                rang = String.Empty;

                if (rang != String.Empty)
                {
                    AppendAlternate();
                    str.Append(rang);
                    mayAlternate = true;
                }
            }
        }

        private bool ExpandRange(string left, string right)
        {
            char cLeft = (left.Length > 1 ? left[1] : left[0]);
            char cRight = (right.Length > 1 ? right[1] : right[0]);

            if (cLeft > cRight)
            {
                return false;
            }

            cLeft++;
            while (cLeft <= cRight)
            {
                AppendAlternate();

                switch (cLeft)
                {
                    case MetaSymbols.ALTERNATE:
                    case MetaSymbols.ANY_ONE_CHAR:
                    case MetaSymbols.CLOSE_PREN:
                    case MetaSymbols.COMPLEMENT:
                    case MetaSymbols.CONCANATE:
                    case MetaSymbols.ESCAPE:
                    case MetaSymbols.ONE_OR_MORE:
                    case MetaSymbols.ZERO_OR_MORE:
                    case MetaSymbols.ZERO_OR_ONE:
                    case MetaSymbols.OPEN_PREN:
                        str.Append(MetaSymbols.ESCAPE);
                        break;
                    default:
                        break;
                }

                str.Append(cLeft);
                mayAlternate = true;
                cLeft++;
            }

            return true;
        }
    }
}
