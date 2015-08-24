using System;
using System.Collections.Generic;
using System.Text;

namespace HesapMakinesi.Calculator
{
    class Operand : IExpression, IEquable
    {
        private double value;

        /// <summary>
        /// Kurucu Metod
        /// </summary>
        public Operand(double value)
        {
            this.value = value;
        }

        /// <summary>
        /// Atama (eşittir) operatörünün overload edilmesi
        /// Bu sayede bir Operand nesnesine doğrudan eşittir operatörü ile double bir değer atanabilecektir.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Operand(double value)
        {
            return new Operand(value);
        }

        public double Exe()
        {
            return value;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}
