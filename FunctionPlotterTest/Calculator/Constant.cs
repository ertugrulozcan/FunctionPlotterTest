using System;
using System.Collections.Generic;
using System.Text;

namespace HesapMakinesi.Calculator
{
    class Constant : IExpression, IEquable
    {
        private string alias;

        public delegate double VariableValueHandler();
        public event VariableValueHandler GetValue;

        /// <summary>
        /// Kurucu Metod
        /// </summary>
        private Constant(string alias, VariableValueHandler getvalue)
        {
            this.alias = alias;
            this.GetValue = getvalue;
        }

        public double Exe()
        {
            return this.GetValue();
        }

        public override string ToString()
        {
            return this.alias;
        }

        public abstract class Constants
        {
            public static Constant PI = new Constant("π", () => { return Math.PI; });
            public static Constant E = new Constant("е", () => { return Math.E; });
            public static Constant ANS = new Constant("ANS", () => { return Calculator.ANS; });
        }
    }

    class VariableValueEvent
    {

    }
}
