using LexicalAnalysis;
using MASM;
using System;
using System.Text.RegularExpressions;

namespace Directives
{
    public class Define : Directive
    {
        public override void Run(MacroAsm masm, ref string text, ref int pos)
        {
            MatchCollection res = Lexer.Run(Parse(ref text, ref pos));
            string value = null;
            switch (res.Count)
            {
                case 1:
                    throw new Exception("Ожидалось имя и значение закрепленное за ним");
                case 2:
                    masm.Table.Add(res[1].Value, value);
                    break;
                default:
                    for (int i = 2; i < res.Count; i++)
                    {
                        value += res[i].Value;
                    }
                    value = _engine.Execute(value).ToString();
                    masm.Table.Add(res[1].Value, value);
                    break;
            }
            Regex r = new Regex($"\\b{res[1].Value}\\b");
            if (value != null)
            {
                text = r.Replace(text, value);
            }
        }

    }
}
