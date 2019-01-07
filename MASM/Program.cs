using System;
using System.ComponentModel;

namespace MASM
{
    class Program
    {
        
        static void Main(string[] args)
        {
            MacroAsm masm = new MacroAsm();
            masm.Run("input", "output");
            Console.ReadKey();
        }
    }
}
