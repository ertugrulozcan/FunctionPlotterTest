using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HesapMakinesi.Calculator
{
    /// <summary>
    /// Equation.cs
    /// Denklem sınıfı
    /// </summary>
    public class Equation : List<IExpression>, ICollection<IExpression>, IExpression, IEquable
    {
        /// <summary>
        /// Kurucu Metod - Constructor
        /// </summary>
        public Equation()
        {

        }

        /// <summary>
        /// Denklem string olarak verilir. Bu stringten bir Equation nesnesi üretilir.
        /// </summary>
        public static Equation Parse(string infix)
        {
            // Denklemin boşluklardan arındırılması;
            infix = infix.Trim(' ');

            // Denklem negatif mi?
            if (infix[0] == '-')
                infix = "0" + infix;

            // Denklemin oluşturulması;
            Equation equation = new Equation();

            try
            {
                for (int i = 0; i < infix.Length; i++)
                {
                    char c = infix[i];

                    // Eğer bir rakam veya virgül ise;
                    if (Char.IsDigit(c) || c == Calculator.DOT)
                    {
                        string number = string.Empty;
                        while (Char.IsDigit(c) || c == Calculator.DOT)
                        {
                            number += c;

                            if (++i == infix.Length)
                            {
                                break;
                            }
                            
                            c = infix[i];
                        }

                        equation.Add(new Operand(Double.Parse(number.Replace(Calculator.DOT, ','))));
                        i--;
                        continue;
                    }

                    // Eğer bir sabit ise;
                    // ANS değeri;
                    if(c == 'A')
                    {
                        c = infix[++i];
                        if (c == 'N')
                        {
                            c = infix[++i];
                            if (c == 'S')
                            {
                                equation.Add(Constant.Constants.ANS);
                                continue;
                            }
                        }
                    }

                    // PI sayısı
                    if (c == 'π')
                    {
                        equation.Add(Constant.Constants.PI);
                        continue;
                    }

                    // Doğal logaritmik e sabiti;
                    if (c == 'е')
                    {
                        equation.Add(Constant.Constants.E);
                        continue;
                    }

                    // Eğer bir operatör ise;
                    if (Operator.isAnOperator(c))
                    {
                        equation.Add(Operator.GetOperator(c));
                        continue;
                    }

                    // Eğer bir fonksiyon ise;
                    if (Char.IsLetter(c))
                    {
                        string functionName = string.Empty;
                        while (Char.IsLetter(c))
                        {
                            functionName += c;
                            c = infix[++i];

                            if (i == infix.Length - 1)
                            {
                                break;
                            }
                        }

                        // Okunan fonksiyon tanınan bir fonksiyon mu?
                        if (Function.isFunction(functionName))
                        {
                            // Eğer fonksiyon isminden sonra parantez başlamıyorsa;
                            if (c != '(')
                                throw new SyntaxException("Fonksiyon parantezi açılmamış!");

                            string innerP = "";
                            c = infix[++i];
                            int paranthesesCount = 1;
                            while (!(paranthesesCount == 0 && c == ')'))
                            {
                                innerP += c;
                                c = infix[++i];

                                if (c == '(')
                                    paranthesesCount++;
                                else if (c == ')')
                                    paranthesesCount--;

                                if (i == infix.Length - 1)
                                {
                                    break;
                                }
                            }

                            equation.Add(new Function(Function.f[functionName], GetParametersForFunction(innerP)));
                        }
                        else
                            throw new SyntaxException("Bilinmeyen fonksiyon veya yazım hatası!");

                        continue;
                    }

                    // Eğer bir parantez açılmışsa;
                    if (infix[i] == '(')
                    {
                        i++;

                        // Parantez içi;
                        string innerParanteses = "";

                        // Parantez içindeki açılan parantezlerin sayısı;
                        int paranthesesCount = 0;

                        // paranthesesCount = 0 iken pivot karakter de ')' ise parantez içinin sonuna gelinmiş demektir.
                        while (!(paranthesesCount == 0 && infix[i] == ')'))
                        {
                            innerParanteses += infix[i];

                            if (infix[i] == '(')
                                paranthesesCount++;
                            else if (infix[i] == ')')
                                paranthesesCount--;

                            i++;

                            if (i == infix.Length)
                                break;
                        }

                        equation.Add(Equation.Parse(innerParanteses));
                        continue;
                    }

                    /*
                    if (c == '(')
                    {
                        string innerParanteses = "";
                        c = infix[++i];
                        int paranthesesCount = 1;
                        while (!(paranthesesCount == 0 && c == ')'))
                        {
                            innerParanteses += c;
                            c = infix[++i];

                            if (c == '(')
                                paranthesesCount++;
                            else if (c == ')')
                                paranthesesCount--;

                            if (i == infix.Length - 1)
                            {
                                break;
                            }
                        }

                        equation.Add(Equation.Parse(innerParanteses));
                        continue;
                    }
                    */
                }
            }
            catch (SyntaxException ex)
            {
                throw new SyntaxException("Sözdizimi hatası : " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new SyntaxException("Tespit edilemeyen hata : " + ex.Message);
            }

            return equation;
        }

        private static IExpression[] GetParametersForFunction(string innerParanteses)
        {
            string[] parameters = innerParanteses.Split(Calculator.FUNC_PARAM_SEPERATOR);
            IExpression[] result = new IExpression[parameters.Length];

            for(int i = 0; i < parameters.Length; i++)
            {
                result[i] = Equation.Parse(parameters[i]);
            }

            return result;
        }
     
        public override string ToString()
        {
            string result = string.Empty;
            foreach (IExpression e in this)
                result += e.ToString();
            return result;
        }

        public double Exe()
        {
            Stack<IExpression> equationStack = new Stack<IExpression>();
            Equation postfix = InfixToPostfix(this);

            foreach (IExpression e in postfix)
            {
                if (e is Operator)
                {
                    IExpression eq2 = equationStack.Pop();
                    IExpression eq1 = equationStack.Pop();

                    double opResult = (e as Operator).Exe(eq1, eq2);
                    equationStack.Push(new Operand(opResult));
                }
                else
                {
                    equationStack.Push(e);
                }
            }

            if (equationStack.Count != 1)
                throw new Exception("Operand yığınıda sadece sonuç kalmış olmalıydı :(");

            return equationStack.Pop().Exe();
        }

        private static Equation InfixToPostfix(Equation infix)
        {
            Equation postfix = new Equation();
            Stack<Operator> opStack = new Stack<Operator>();

            foreach (IExpression e in infix)
            {
                if (e is IEquable)
                {
                    // Eğer eleman bir operand ise direkt eklenir
                    postfix.Add(e);
                }
                else if (e is Operator)
                {
                    // Eğer eleman bir operator ise;                    
                    // Gelen operatorün işlem önceliği eğer yığının en üstündeki operatörün işlem önceliğinden yüksekse;
                    if(opStack.Count > 0)
                    {
                        Operator topOperator = opStack.Pop();
                        if (!Operator.isHigherPrecedence(e as Operator, topOperator))
                        {
                            // Yığındaki tüm operatörleri postfixe ekle
                            postfix.Add(topOperator);
                            while (opStack.Count != 0)
                                postfix.Add(opStack.Pop());

                            opStack.Push(e as Operator);
                        }
                        else
                        {
                            // Aksi halde operatorleri yığına geri koy ve devam et
                            opStack.Push(topOperator);
                            opStack.Push(e as Operator);
                        }
                    }
                    else
                    {
                        opStack.Push(e as Operator);
                    }
                }
            }

            if (opStack.Count > 0)
                while (opStack.Count != 0)
                    postfix.Add(opStack.Pop());

            return postfix;
        }

        /// <summary>
        /// Yazım (Syntax) hataları için hata denetim sınıfı
        /// </summary>
        public class SyntaxException : Exception
        {
            String description;

            public SyntaxException(string message)
            {
                this.description = message;
            }

            public override string ToString()
            {
                return "Yazım (Syntax) hatası. " + description;
            }

            public override string Message
            {
                get
                {
                    return ToString();
                }
            }
        }

        /// <summary>
        /// Matematiksel hatalar için hata denetim sınıfı
        /// </summary>
        public class MathException : Exception
        {
            String description;

            public MathException(string message)
            {
                this.description = message;
            }

            public override string ToString()
            {
                return "Matematiksel hata. " + description;
            }

            public override string Message
            {
                get
                {
                    return ToString();
                }
            }
        }
    }
}
