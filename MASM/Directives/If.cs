using LexicalAnalysis;
using MASM;
using System;
using System.Text.RegularExpressions;

namespace Directives
{
    public class If : Сonditions
    {
        public override void Run(MacroAsm masm, ref string text, ref int pos)
        {
            MatchCollection res = Lexer.Run(Parse(ref text, ref pos));
            if (res.Count < 4)
            {
                throw new Exception("Недостаточное количество параметров");
            }
            string expression = "";
            for (int i = 1; i < res.Count; i++)
            {
                expression += res[i].Value;
            }
            Console.WriteLine("Выражение - " + expression);
            bool check = _engine.Execute(expression);
            if (check)
            {
                var item = GetBlock(text, pos);
                pos = item.Index;
                text = text.Remove(item.Index, item.Length);
                if (item.Value == DirNames.Else.Value)
                {
                    Skip(ref text, pos);
                }
            }
            else
            {
                Skip(ref text, pos);
            }
        }
    }
}
