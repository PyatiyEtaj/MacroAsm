using LexicalAnalysis;
using MASM;
using System.Text.RegularExpressions;

namespace Directives
{
    public class IfDef : Сonditions
    {
        public override void Run(MacroAsm masm, ref string text, ref int pos)
        {
            MatchCollection res = Lexer.Run(Parse(ref text, ref pos));
            bool check = masm.Table.ContainsKey(res[1].Value);
            if (($"#{res[0].Value}" == DirNames.IfDef.Value && check) ||
                 ($"#{res[0].Value}" == DirNames.IfNDef.Value && !check))
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
