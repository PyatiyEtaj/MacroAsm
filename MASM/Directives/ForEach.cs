using LexicalAnalysis;
using MASM;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Directives
{
    public class ForEach : Cycle
    {
        private string _var = "$$$";
        private Regex _localRegex;
        private List<string> _args = new List<string>();

        public override void Run(MacroAsm masm, ref string text, ref int pos)
        {
            _args.Clear();
            var parsePart = Parse(ref text, ref pos);
            var endFirstLine = parsePart.IndexOf('\n');
            MatchCollection headLexems = Lexer.Run(parsePart.Substring(0, endFirstLine));
            var body = parsePart.Remove(0, endFirstLine + 1);
            if (headLexems.Count < 3)
            {
                throw new Exception("[foreach] - Не указаны значения для подстановки в след. блок");
            }
            _var = headLexems[1].Value;
            for (int i = 2; i < headLexems.Count; i++)
            {
                var tmp = headLexems[i].CurGroup();
                if (tmp == (int)Lexer.Lexems.Ident || tmp == (int)Lexer.Lexems.Number)
                    _args.Add(headLexems[i].Value);
            }
            _localRegex = new Regex($"\\${_var}");
            string result = "";
            foreach (var item in _args)
            {
                result += _localRegex.Replace(body, item);
            }
            text = text.Insert(pos, result);
        }
    }
}
