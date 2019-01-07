using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Directives;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace MASM
{
    public class MacroAsm
    {
        public Dictionary<string, string> Table { get; set; }
        private Dictionary<string, Directive> _dirs = new Dictionary<string, Directive>();
        /*private ScriptEngine _py = Python.GetEngine();
        private ScriptScope _nameTable;*/
        public MacroAsm()
        {
            Table = new Dictionary<string, string>();

            _dirs.Add(DirNames.Define.Value,  new Define());
            _dirs.Add(DirNames.If.Value,      new If());
            _dirs.Add(DirNames.IfDef.Value,   new IfDef());
            _dirs.Add(DirNames.IfNDef.Value,  new IfDef());
            _dirs.Add(DirNames.Macros.Value,  new Macros());
            _dirs.Add(DirNames.Repeat.Value,  new Repeat());
            _dirs.Add(DirNames.ForEach.Value, new ForEach());
            _dirs.Add(DirNames.Import.Value,  new Import());
        }

        public void Run(string inputFileName, string outputFileName)
        {
            string text = "";
            try
            {
                using (StreamReader sr = new StreamReader(inputFileName, Encoding.UTF8))
                {
                    text = sr.ReadToEnd();
                }
                // удаление комментариев
                Directive.EraseComments(ref text);

                // производим импорт для всех файлов
                bool check = ImportAllFiles(ref text);
                int pos = text.IndexOf("#");
                while (pos != -1)
                {
                    var dirName = GetCmdName(text, pos);
                    // вызов директивы
                    if (_dirs.ContainsKey(dirName))
                    {
                        _dirs[dirName].Run(this, ref text, ref pos);
                        pos = text.IndexOf("#"); // нахожу след. директиву
                    }
                    else
                    {
                        throw new Exception("Неожиданная встреча директивы: " + dirName);
                    }
                }
                using (StreamWriter sw = new StreamWriter(outputFileName, false, Encoding.UTF8))
                {
                    sw.WriteLine(text.Trim('\n', ' ', '\r'));
                }
                Console.WriteLine("Успешное завершение работы");
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter($"{outputFileName}_LOG"))
                {
                    sw.WriteLine(text.Insert(text.Length - 1, $"***[ Ошибка: {e.Message} ]***"));
                }
                Console.WriteLine($"ИСКЛЮЧЕНИЕ: {e.Message}");
                Console.WriteLine("Аварийное завершение программы!");
                return;
            }
        }

        private bool ImportAllFiles(ref string text)
        {
            int pos = -1;
            int countOfImport = 0;
            do
            {
                pos = text.IndexOf(DirNames.Import.Value);
                if (pos != -1)
                {
                    _dirs[DirNames.Import.Value].Run(this, ref text, ref pos);
                    countOfImport++;
                }
                if (countOfImport > 50)
                    return false;
                    //throw new Exception($"Достигнуто ограничение по исп. {DirNames.Import.Value}");
                    
            } while (pos != -1);
            return true;
        }

        private string GetCmdName(string text, int pos)
        {
            int tmp = text.IndexOf(" ", pos);
            string cmdName = text.Substring(pos, tmp-pos);
            return cmdName;
        }
    }
}
