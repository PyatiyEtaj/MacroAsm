using System;
using System.Text.RegularExpressions;

namespace Directives
{
    public abstract class Сonditions : Directive
    {   // создам один раз regex, чтобы не создавать данный объект кучу раз для каждого наследника
        protected static Regex regex = new Regex($"({DirNames.IfNDef.Value})|({DirNames.If.Value})|({DirNames.IfDef.Value})|({DirNames.Else.Value})|({DirNames.EndIF.Value})");
        // нахождение блока c ifdef, else и endif, начиная с позиции pos в тексте
        protected Match GetBlock(string text, int pos)
        {
            MatchCollection res = regex.Matches(text, pos);
            int count = 1, i;
            for (i = 0; i < res.Count && count > 0; i++)
            {
                var cmdType = res[i].Value;
                if (cmdType == DirNames.If.Value || cmdType == DirNames.IfDef.Value || cmdType == DirNames.IfNDef.Value)
                {
                    count++;
                    continue;
                }
                if (i > 0)
                {
                    if (cmdType == DirNames.EndIF.Value)
                    {
                        if (res[i - 1].Value != DirNames.Else.Value)
                        {
                            count++;
                        }
                        continue;
                    }
                }                    
                count--;
            }
            if (count > 0)
            {
                throw new Exception($"Недостаточно {DirNames.EndIF.Value} необходимо еще {count}");
            }
            return res[--i];
        }

        protected void Skip(ref string text, int pos)
        {
            Match res = GetBlock(text, pos);
            text = text.Remove(pos, res.Index - pos + res.Length);
            if (res.Value == DirNames.Else.Value)
            {
                res = GetBlock(text, pos);
                pos = res.Index;
                text = text.Remove(res.Index, res.Length);
            }
        }
    }
}
