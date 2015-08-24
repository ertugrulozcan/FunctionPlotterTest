using System;
using System.Collections.Generic;
using System.Text;

/*
 * Ahmet Ertuğrul Özcan
 * 2015
 */
namespace HesapMakinesi.Calculator
{
    /// <summary>
    /// Calculator.cs
    /// Ana sınıf
    /// </summary>
    public abstract class Calculator
    {
        public static double ANS = 0;

        /// <summary>
        /// Hesaplama fonksiyonu
        /// </summary>
        /// <param name="equation"></param>
        /// <returns></returns>
        public static double Execute(string equation)
        {
            ANS = Execute(Equation.Parse(equation));
            return ANS;
        }

        /// <summary>
        /// Hesaplama fonksiyonu
        /// </summary>
        /// <param name="equation"></param>
        /// <returns></returns>
        public static double Execute(Equation equation)
        {
            ANS = equation.Exe();
            return ANS;
        }

        private static DegreeTypes degreeType = DegreeTypes.Degree;
        /// <summary>
        /// Derece cinsi
        /// </summary>
        public static DegreeTypes DegreeType
        {
            get { return degreeType; }
            set { degreeType = value; }
        }

        public static char DOT = '.';
        public static char FUNC_PARAM_SEPERATOR = ',';
    }

    public enum DegreeTypes
    {
        Degree, Radian, Grad
    };
}
