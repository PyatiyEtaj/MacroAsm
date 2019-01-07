using LexicalAnalysis;
using MASM;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Directives
{
    public class Import : Directive
    {
        private string _filename = null;

        public override void Run(MacroAsm masm, ref string text, ref int pos)
        {
            _filename = null;
            MatchCollection res = Lexer.Run(Parse(ref text, ref pos));
            if (res.Count < 4)
            {
                throw new Exception("Неверный формат директивы: #import <...>");
            }
            if (res[1].Value != "<")
            {
                throw new Exception(@"Ожидался символ '<'");
            }
            // путь до файла, который необходимо импортировать
            for (int i = 2; i < res.Count; i++)
            {
                switch (res[i].CurGroup())
                {
                    case (int)Lexer.Lexems.Sign:
                        if (res[i].Value == ">")
                        {
                            i = res.Count;
                        }                            
                        else if (res[i].Value == "<")
                        {
                            throw new Exception(@"Ошибочный символ '<'");
                        }
                        else
                        {
                            _filename += res[i].Value;
                        }                            
                        break;
                    default:
                        _filename += res[i].Value;
                        break;
                }
            }
            using (StreamReader sr = new StreamReader(_filename))
            {
                text = text.Insert(pos, sr.ReadToEnd());
                Directive.EraseComments(ref text);
            }
        }
    }
}
