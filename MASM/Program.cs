using System;
using System.ComponentModel;

namespace MASM
{
    class Program
    {        
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                string input = args[0];
                string output = args[1];
                MacroAsm masm = new MacroAsm();
                masm.Run(input, output);
            }
            else
            {
                Console.WriteLine("Необходимо 2 параметра имя_входного_файла и имя_выхрдного_файла");
            }
            //MacroAsm masm = new MacroAsm();
            //masm.Run("inp", "output");     
            Console.ReadKey();
        }
    }
}
