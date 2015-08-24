using System;
using System.Collections.Generic;
using System.Text;

namespace HesapMakinesi.Calculator
{
    public abstract class Test
    {
        public static void Run()
        {
            try
            {
                string equ = "5+7*11-23 + sin(";
                System.Diagnostics.Debug.WriteLine(equ + " = " + Calculator.Execute(equ));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Hata : " + ex.Message);
            }
        }
    }
}
