using System.Text;

namespace TASI.Token
{
    public class Tokeniser
    {
        private static StringBuilder handleLineCharSB = new();
        private static readonly char[] HexChars = "0123456789ABCDE".ToCharArray();
        public static int HandleLineChar(string input, out int endChar, int startChar, Global global)
        {
            endChar = startChar;
            if (input[endChar] == 'Ⅼ')
                endChar++;
            StringBuilder nextLine = handleLineCharSB;
            nextLine.Clear();
            while (input[endChar] != 'Ⅼ')
            {
                nextLine.Append(input[endChar]);
                endChar++;
            }
            int parsedLine = int.Parse(nextLine.ToString());
            global.CurrentLine = parsedLine;
            return int.Parse(nextLine.ToString());
        }

        private static List<Command> TokeniseInputRecursive(string input, out int endChar, out int line, Global global, int currentLine = 0, int startChar = -1)
        {
            bool canEndAtDataEnd;
            if (startChar == -1)
            {
                canEndAtDataEnd = true;
                startChar = 0;
            }
            else
            {
                canEndAtDataEnd = false;
            }
            List<Command> result = new();
            line = currentLine;
            StringBuilder sb = new();
            int startLine = startChar;
            global.CurrentLine = line;
            for (endChar = startChar; endChar < input.Length; endChar++)
            {
                char c = input[endChar];

                switch (c)
                {
                    case ')':
                    case ']':
                        throw new CodeSyntaxException($"Unexpected '{c}'");

                    case '$':
                        sb.Clear();
                        //sb.Append("0x");
                        startLine = line;
                        endChar++;
                        bool negativeNumber = false;
                        if (input[endChar] == '-')
                        {
                            endChar++;
                            negativeNumber = true;
                        }

                        while (HexChars.Contains(char.ToUpper(input[endChar])))
                        {


                            if (input[endChar] == 'Ⅼ')
                            {
                                line = HandleLineChar(input, out endChar, endChar, global);
                                endChar++;
                                continue;
                            }
                            sb.Append(input[endChar]);
                            endChar++;

                            if (endChar == input.Length)
                                break;
                        }
                        if (sb.Length == 0)
                            throw new CodeSyntaxException("Invalid hex num. (You forgot the number)");
                        endChar--;
                        result.Add(new((negativeNumber ? -1 : 1) * long.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber), startLine, line));
                        break;
                    case '\"':
                        result.Add(HandleString(input, endChar, out endChar, out line, global, line));
                        break;
                    case ';':
                        result.Add(new(Command.CommandTypes.EndCommand, ";", global, line, line));
                        break;
                    case '{':
                        int lineStart = line;
                        var inCodeContainer = TokeniseInputRecursive(input, out endChar, out line, global, line, endChar + 1);
                        result.Add(new(inCodeContainer, global, lineStart));
                        break;
                    case '}':
                        return result;
                    case 'Ⅼ':
                        line = HandleLineChar(input, out endChar, endChar, global);
                        break;
                    default:
                        if (ignoreChars.Contains(c))
                        {
                            break;
                        }
                        sb.Clear();
                        startLine = line;
                        if (specialCommandChars.Contains(input[endChar]))
                        {
                            throw new InternalInterpreterException($"Internal: parsed as statement, but was special command char '{input[endChar]}'");
                        }
                        //Do that cleaner bruh
                        while (!specialCommandChars.Contains(input[endChar]))
                        {


                            if (input[endChar] == 'Ⅼ')
                            {
                                line = HandleLineChar(input, out endChar, endChar, global);
                                endChar++;
                                continue;
                            }
                            sb.Append(input[endChar]);
                            endChar++;

                            if (endChar == input.Length)
                                break;
                        }
                        endChar--;
                        result.Add(new(Command.CommandTypes.Statement, sb.ToString(), global, startLine, line));
                        break;
                }
            }
            if (!canEndAtDataEnd)
                throw new CodeSyntaxException("Invalid code container formating (You probably forgot a '}')");
            return result;
        }
        private static readonly HashSet<char> ignoreChars = new()
        {
            ' ', '\t'
        };

        private static readonly Dictionary<char, char> backslashReplace = new Dictionary<char, char>()
        {
            { 'n', '\n' },
            { '\"', '\"' },
            { 't', '\t' },
            { 'l', 'Ⅼ' },
            {'h', '#' },
            {'\\', '\\' }


        };

        private static StringBuilder handleStringSB = new();
        public static Command HandleString(string input, int start, out int endCharIDX, out int endLine, Global global, int startLine = -1, bool replaceEscape = true)
        {


            StringBuilder resultString = handleStringSB;
            resultString.Clear();


            if (input[start] == '\"')
                start++;
            endLine = startLine;
            Command result = new(Command.CommandTypes.String, "", global, startLine, endLine);
            bool lastCharBackslash = false;
            for (endCharIDX = start; endCharIDX < input.Length; endCharIDX++)
            {
                if (lastCharBackslash)
                {
                    lastCharBackslash = false;

                    if (!backslashReplace.TryGetValue(input[endCharIDX], out char replace))
                        throw new CodeSyntaxException($"Invalid string escape char: '{input[endCharIDX]}'");
                    if (replaceEscape)
                    {
                        resultString.Append(replace);
                    }
                    else
                    {
                        resultString.Append('\\');

                        resultString.Append(replace);
                    }
                    continue;
                }
                switch (input[endCharIDX])
                {
                    case 'Ⅼ':
                        endLine = HandleLineChar(input, out endCharIDX, endCharIDX, global);
                        continue;
                    case '\\':
                        lastCharBackslash = true;
                        continue;
                    case '\"':
                        result.CommandEnd = endLine;
                        result.commandText = resultString.ToString();
                        return result;
                }

                resultString.Append(input[endCharIDX]);

            }
            throw new CodeSyntaxException("Didn't end string.");

        }


        internal static readonly HashSet<char> specialCommandChars = new() { '\"', '[', ']', '(', ')', ';', '{', '}', ' ', '$' }; //A sb or syntax will end if it contains any of these chars and the correct type will follow

        public static List<Command> CallTokeniseInput(string line, Global global, int currentLine = 0)
        {
            return TokeniseInputRecursive(line, out int _, out int _, global, currentLine);

        }

    }
}
