using System;
using System.Text.RegularExpressions;

namespace Directives
{
    public abstract class Cycle : Directive
    {   // создам один раз regex, чтобы не создавать данный объект кучу раз для каждого наследника
        protected static Regex regex = new Regex($"({DirNames.Repeat.Value})|({DirNames.ForEach.Value})|({DirNames.EndCycle.Value})");

        protected override string Parse(ref string text, ref int pos)
        {
            string res;
            var end = GetBlock(text, pos);
            res = text.Substring(pos, end.Index - pos);
            text = text.Remove(pos, end.Index - pos + end.Length);
            return res;
        }

        // нахождение блока c repeat, foreach и endcycle, начиная с позиции pos в тексте
        protected Match GetBlock(string text, int pos)
        {
            MatchCollection res = regex.Matches(text, pos);
            int count = 0, i = 0;
            do
            {
                var cmdType = res[i].Value;
                i++;
                if (cmdType == DirNames.Repeat.Value || cmdType == DirNames.ForEach.Value)
                {
                    count++;
                    continue;
                }
                count--;
            } while (i < res.Count && count > 0);
            if (count > 0)
            {
                throw new Exception($"В цикле недостаточное количество {DirNames.EndCycle.Value}");
            }
            return res[--i];
        }
    }
}
