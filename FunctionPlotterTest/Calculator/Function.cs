using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DefinedFunction = System.Func<HesapMakinesi.Calculator.IExpression[], double>;

namespace HesapMakinesi.Calculator
{
    /// <summary>
    /// Parametreli fonksiyon
    /// </summary>
    class Function : IExpression, IEquable
    {
        /// <summary>
        /// Fonksiyonun parametreleri
        /// </summary>
        private IExpression[] parameters;

        /// <summary>
        /// Operatörün yapacağı işlem fonksiyonu
        /// </summary>
        /// <returns></returns>
        public DefinedFunction operation;

        /// <summary>
        /// Kurucu Metod
        /// </summary>
        /// <param name="alias">abbreviation</param>
        /// <param name="GetValueFunc"></param>
        public Function(DefinedFunction operation, IExpression[] parameters)
        {
            this.operation = operation;
            this.parameters = parameters;
        }

        public Function(DefinedFunction operation, IExpression parameter)
        {
            this.operation = operation;
            this.parameters = new IExpression[] { parameter };
        }

        public Function(DefinedFunction operation, double dparameter)
        {
            this.operation = operation;
            this.parameters = new IExpression[] { new Operand(dparameter) };
        }

        public double Exe()
        {
            return this.operation.Invoke(this.parameters);
        }

        public override string ToString()
        {
            String parameters = string.Empty;
            foreach (IExpression equ in this.parameters)
                parameters += equ.ToString() + ", ";
            parameters = parameters.Substring(0, parameters.Length - 2);

            var keys = from entry in f where entry.Value == this.operation select entry.Key;
            return f.FirstOrDefault(x => x.Value == this.operation).Key + '(' + parameters.ToString() + ')';
        }

        public static Dictionary<string, DefinedFunction> f = new Dictionary<string, DefinedFunction>
        {
            { "sin", Functions.sin },
            { "arcsin", Functions.arcsin },
            { "sinh", Functions.sinh },
            { "cos", Functions.cos },
            { "arccos", Functions.arccos },
            { "cosh", Functions.cosh },
            { "tan", Functions.tan },
            { "arctan", Functions.arctan },
            { "tanh", Functions.tanh },
            { "cot", Functions.cot },
            { "log", Functions.log },
            { "log10", Functions.log10 },
            { "ln", Functions.ln },
            { "pow", Functions.pow },
            { "sqrt", Functions.sqrt },
            { "fact", Functions.fact }
        };

        public static bool isFunction(string fName)
        {
            DefinedFunction func = f[fName];
            return (func != null);
        }

        private abstract class Functions
        {
            private static double ToRadian(double rdg)
            {
                switch (Calculator.DegreeType)
                {
                    case DegreeTypes.Radian: return rdg;
                    case DegreeTypes.Degree: return DegreeToRadian(rdg);
                    case DegreeTypes.Grad: return GradToRadian(rdg);
                    default: return DegreeToRadian(rdg);
                }
            }

            private static double FromRadian(double radian)
            {
                switch (Calculator.DegreeType)
                {
                    case DegreeTypes.Radian: return radian;
                    case DegreeTypes.Degree: return RadianToDegree(radian);
                    case DegreeTypes.Grad: return RadianToGrad(radian);
                    default: return RadianToDegree(radian);
                }
            }

            public static readonly DefinedFunction sin = (IExpression[] operands) => { return Math.Sin(ToRadian(operands[0].Exe())); };
            public static readonly DefinedFunction arcsin = (IExpression[] operands) => { return FromRadian(Math.Asin(operands[0].Exe())); };
            public static readonly DefinedFunction sinh = (IExpression[] operands) => { return Math.Sinh(ToRadian(operands[0].Exe())); };
            public static readonly DefinedFunction cos = (IExpression[] operands) => { return Math.Cos(ToRadian(operands[0].Exe())); };
            public static readonly DefinedFunction cosh = (IExpression[] operands) => { return Math.Cosh(ToRadian(operands[0].Exe())); };
            public static readonly DefinedFunction arccos = (IExpression[] operands) => { return FromRadian(Math.Acos(operands[0].Exe())); };
            public static readonly DefinedFunction tan = (IExpression[] operands) => { return Math.Tan(ToRadian(operands[0].Exe())); };
            public static readonly DefinedFunction tanh = (IExpression[] operands) => { return Math.Tanh(ToRadian(operands[0].Exe())); };
            public static readonly DefinedFunction arctan = (IExpression[] operands) => { return FromRadian(Math.Atan(operands[0].Exe())); };
            public static readonly DefinedFunction cot = (IExpression[] operands) => { return 1 / Math.Tan(ToRadian(operands[0].Exe())); };
            public static readonly DefinedFunction log = (IExpression[] operands) => { return Math.Log(operands[0].Exe(), operands[1].Exe()); };
            public static readonly DefinedFunction log10 = (IExpression[] operands) => { return Math.Log10(operands[0].Exe()); };
            public static readonly DefinedFunction ln = (IExpression[] operands) => { return Math.Log(operands[0].Exe(), Math.E); };
            public static readonly DefinedFunction pow = (IExpression[] operands) => { return Math.Pow(operands[0].Exe(), operands[1].Exe()); };
            public static readonly DefinedFunction sqrt = (IExpression[] operands) => { return (operands.Length == 1) ? Math.Sqrt(operands[0].Exe()) : Math.Pow(operands[0].Exe(), 1 / operands[1].Exe()); };
            public static readonly DefinedFunction fact = (IExpression[] operands) => { return factorial(operands[0].Exe()); };
        }

        #region Matematiksel fonksiyonlar
        
        private static double factorial(double n)
        {
            if (n < 0)
                throw new HesapMakinesi.Calculator.Equation.MathException("Negatif sayıların faktöriyeli alınamaz.");
            else if (n == 0)
                return 1;
            else
            {
                double fact = 1;
                while (n > 0)
                    fact *= n--;

                return fact;
            }
        }

        private static double RadianToDegree(double radian)
        {
            return radian * 180 / Math.PI;
        }

        private static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }

        private static double RadianToGrad(double radian)
        {
            return radian * 200 / Math.PI;
        }

        private static double GradToRadian(double grad)
        {
            return grad * Math.PI / 200;
        }

        #endregion

    }
}
