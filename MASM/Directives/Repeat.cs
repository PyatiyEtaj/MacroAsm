using LexicalAnalysis;
using MASM;
using System;

namespace Directives
{
    public class Repeat : Cycle
    {
        private int _countOfRepeat = 0;

        public override void Run(MacroAsm masm, ref string text, ref int pos)
        {
            var parsePart = Parse(ref text, ref pos);
            var endFirstLine = parsePart.IndexOf('\n');
            var headLexems = Lexer.Run(parsePart.Substring(0, endFirstLine));
            if (headLexems.Count < 2)
            {
                throw new Exception("Не указано количество повторений");
            }
            if (headLexems[1].CurGroup() != (int)Lexer.Lexems.Number)
            {
                throw new Exception("Количество повторений должно быть целым числом");
            }
            _countOfRepeat = Int32.Parse(headLexems[1].Value);
            var body = parsePart.Remove(0, endFirstLine + 1);
            string result = "";
            for (int i = 0; i < _countOfRepeat; i++)
            {
                result += body;
            }
            text = text.Insert(pos, result);
        }
    }
}
