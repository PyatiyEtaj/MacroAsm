using IronPython.Hosting;
using System.Text.RegularExpressions;
using MASM;

namespace Directives
{
    public abstract class Directive
    {
        protected static Microsoft.Scripting.Hosting.ScriptEngine _engine;

        static Directive()
        {
            _engine = Python.CreateEngine();
        }

        public static void EraseComments(ref string text)
        {
            Regex regex = new Regex(@";.*");
            text = regex.Replace(text, "");
        }
        
        protected virtual string Parse(ref string text, ref int pos)
        {
            string res;
            int tmp = text.IndexOf("\n", pos);
            res = text.Substring(pos, tmp - pos);
            text = text.Remove(pos, tmp - pos);
            return res;
        }

        public abstract void Run(MacroAsm masm, ref string text, ref int pos);
    }
}
