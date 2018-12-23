using LexicalAnalysis;
using MASM;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Directives
{
    public class Macros : Directive
    {
        private string _name = null;
        private List<string> _args;

        private List<string> GetArgs(MatchCollection mc, int pos = 1)
        {
            if (mc[pos].Value != "(")
            {
                throw new Exception(@"Ожидалася символ '('");
            }
            List<string> ls = new List<string>();
            string localValue = null;
            pos++;
            bool isEnd = false;
            for (; pos < mc.Count && !isEnd; pos++)
            {
                var tmp = mc[pos].CurGroup();
                switch (tmp)
                {
                    case (int)Lexer.Lexems.Ident:
                    case (int)Lexer.Lexems.Number:
                        localValue += mc[pos].Value;
                        break;
                    case (int)Lexer.Lexems.ErrorName:
                        
                        throw new Exception("Аргумент содержит недопустимое имя: " + mc[pos].Value);
                    case (int)Lexer.Lexems.Sign:
                        if (mc[pos].Value == ",")
                        {
                            if (localValue != null)
                                ls.Add(localValue);
                            localValue = null;
                        }
                        else if (mc[pos].Value == ")")
                        {
                            if (localValue != null)
                                ls.Add(localValue);
                            isEnd = true;
                        }
                        else
                        {
                            localValue += mc[pos].Value;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (!isEnd)
                throw new Exception(@"Ожидался символ ')'");

            return ls;
        }

        private void FindAndChangeMacros(ref string text, string body)
        {
            Regex r = new Regex($"{_name}\\s.*");
            MatchCollection res = r.Matches(text);
            for (int count = 0; count < res.Count; count++)
            {
                var tmp = Lexer.Run(res[count].Value);
                var localArgs = GetArgs(tmp);
                if (localArgs.Count != _args.Count)
                {
                    throw new Exception("Неверное количество параметров");
                }
                var localBody = body;
                for (int i = 0; i < _args.Count; i++)
                {
                    r = new Regex($"\\${_args[i]}");
                    localBody = r.Replace(localBody, localArgs[i]);
                }
                text = text.Replace(res[count].Value, localBody, StringComparison.Ordinal);
            }
        }

        protected override string Parse(ref string text, ref int pos)
        {
            string res = "";
            int tmp = text.IndexOf(DirNames.EndMacro.Value, pos);
            if (tmp == -1)
                throw new Exception("Ожидалось оканчание макроопределения");
            res = text.Substring(pos, tmp - pos);
            text = text.Remove(pos, tmp + DirNames.EndMacro.Value.Length - pos);
            return res;
        }

        public override void Run(MacroAsm masm, ref string text, ref int pos)
        {
            // получаем все макроопределение
            // разделяем все макроопределение на голову и тело
            // голова содержит имя и аргументы
            // тело содержит команды ассемблера
            var body = Parse(ref text, ref pos);
            var head = body.Substring(0, body.IndexOf("\n", 0));
            body = body.Substring(head.Length, body.Length - head.Length);
            MatchCollection res = Lexer.Run(head);
            if (res.Count < 2 || res[1].CurGroup() != (int) Lexer.Lexems.Ident)
            {
                throw new Exception("Ожидалось имя макроопределения");   
            }
            // устанавливаем название макроопределения
            // а также аргументы
            _name = res[1].Value;
            _args = GetArgs(res, 2);
            masm.Table.Add(_name, body);
            FindAndChangeMacros(ref text, body);
        }
    }
}
