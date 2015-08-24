using System;
using System.Collections.Generic;
using System.Text;

namespace HesapMakinesi.Calculator
{
    class Operator : IExpression
    {
        private char symbol;
        private int precedenceValue;

        private delegate double OperatorFunction(IExpression eq1, IExpression eq2);
        private OperatorFunction function;

        /// <summary>
        /// Kurucu Metod - Constructor
        /// </summary>
        private Operator(char symbol, OperatorFunction function, int p)
        {
            this.symbol = symbol;
            this.function = function;
            this.precedenceValue = p;
        }

        /// <summary>
        /// İşlem önceliğini kıyaslar
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        public static bool isHigherPrecedence(Operator op1, Operator op2)
        {
            return (op1.precedenceValue < op2.precedenceValue);
        }

        public static bool isAnOperator(char c)
        {
            if (c == Operators.PLUS.symbol ||
                c == Operators.MINUS.symbol ||
                c == Operators.MULTIPLY.symbol ||
                c == Operators.DIVIDE.symbol ||
                c == Operators.MOD.symbol ||
                c == Operators.POWER.symbol)
                    return true;
            else
                return false;
        }

        public static Operator GetOperator(char c)
        {
            if (c == Operators.PLUS.symbol)
                return Operators.PLUS;
            else if (c == Operators.MINUS.symbol)
                return Operators.MINUS;
            else if (c == Operators.MULTIPLY.symbol)
                return Operators.MULTIPLY;
            else if (c == Operators.DIVIDE.symbol)
                return Operators.DIVIDE;
            else if (c == Operators.MOD.symbol)
                return Operators.MOD;
            else if (c == Operators.POWER.symbol)
                return Operators.POWER;
            else
                return null;
        }

        public double Exe(IExpression eq1, IExpression eq2)
        {
            return this.function(eq1, eq2);
        }

        public double Exe()
        {
            throw new NotImplementedException("Bu operator arguman aldığı için parametresiz hesap fonksiyonu implement edilmez.");
        }

        public override string ToString()
        {
            return this.symbol.ToString();
        }

        public abstract class Operators
        {
            public static readonly Operator PLUS = new Operator('+', (eq1, eq2) => { return eq1.Exe() + eq2.Exe(); }, 6);
            public static readonly Operator MINUS = new Operator('-', (eq1, eq2) => { return eq1.Exe() - eq2.Exe(); }, 5);
            public static readonly Operator MULTIPLY = new Operator('*', (eq1, eq2) => { return eq1.Exe() * eq2.Exe(); }, 4);
            public static readonly Operator DIVIDE = new Operator('/', (eq1, eq2) => { return eq1.Exe() / eq2.Exe(); }, 3);
            public static readonly Operator MOD = new Operator('%', (eq1, eq2) => { return eq1.Exe() % eq2.Exe(); }, 2);
            public static readonly Operator POWER = new Operator('^', (eq1, eq2) => { return Math.Pow(eq1.Exe(), eq2.Exe()); }, 1);
        }
    }

    public class RankedChangingEvent
    {

    }
}
