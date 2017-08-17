using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calcoolator
{
public class MEMORY
    {
        public string zmienna { get; set; }
        public string wartosc { get; set; }
    }

    public static class QCALC
    {
        public static double Qeq(string wejscie)
        {
            EQEXE eqexe = new EQEXE(wejscie);
            double otp = 0;
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string l1 = eqexe.st.Replace(".", separator);
            double.TryParse(l1, out otp);
            return otp;
        }

    }

    public class MCALC
    {
        public MCALC()
        {

        }

        public string wartosc(string zmienna)
        {
            string otp = "";

            for(int i = 0; i< MEMORYY.Count; i++)
            {
                if (zmienna == MEMORYY[i].zmienna)
                {
                    otp = MEMORYY[i].wartosc;
                }
            }

            return otp;
        }

        public string podstaw(string rownanie, List<MEMORY> MEMORYY)
        {
            string rex = "";

            if (MEMORYY.Count > 0)
                rex += "|";
            for (int i = 0; i < MEMORYY.Count; i++)
            {
                if (i < MEMORYY.Count - 1)
                {
                    rex += @"^" + MEMORYY[i].zmienna + "$|";
                }
                else
                {
                    rex += @"^" + MEMORYY[i].zmienna + "$";
                }
            }

            string regex = NUMERIC.REGEX + "|(" + OPERATORS.REGEX() + "|" + FUNCTIONS.REGEX() + rex + ")";

            string[] result1 = Regex.Split(rownanie, regex, RegexOptions.IgnoreCase);

            string result = "";
            for (int i = 0; i < result1.Length; i++)
            {
                
                if (result1[i].Length > 0)
                {
                    string str = "";
                    for (int j = 0; j < MEMORYY.Count; j++)
                    {
                        if (MEMORYY[j].zmienna == result1[i])
                        {
                            str = MEMORYY[j].wartosc;
                        }
                    }

                    if(str != "")
                    {
                        result += str;
                    }
                    else
                    {
                        result += result1[i];
                    }
                }
            }
            
            return result;
        }

        public List<MEMORY> MEMORYY = new List<MEMORY> { };

        public void AddMemory(string ZMIENNA, string WARTOSC)
        {
            MEMORYY.Add(new MEMORY { zmienna = ZMIENNA, wartosc= WARTOSC });
        }
        public void RemoveMemory(string ZMIENNA)
        {
            for (int i = MEMORYY.Count - 1; i > -1; i--)
            {
                if (ZMIENNA == MEMORYY[i].zmienna)
                {
                    MEMORYY.RemoveAt(i);
                }
            }
        }
        public string ThisMemory(string ZMIENNA)
        {
            OBL();
            string otp = "";

            for (int i = 0; i < MEMORYY.Count; i++)
            {
                if (ZMIENNA == MEMORYY[i].zmienna)
                {
                    otp = MEMORYY[i].wartosc;
                }
            }

            return otp;
        }
        public void ThisMemory(string ZMIENNA, string WARTOSC)
        {
            bool jest = false;
            for (int i = 0; i < MEMORYY.Count; i++)
            {
                if (ZMIENNA == MEMORYY[i].zmienna)
                {
                    MEMORYY[i].wartosc = WARTOSC;
                    jest = true;
                }
            }

            if(jest == false)
                MEMORYY.Add(new MEMORY { zmienna = ZMIENNA, wartosc = WARTOSC });
        }

        public string OBLICZ()
        {
            OBL();
            return WYJSCIE;
        }

        public void EQ(string wejscie)
        {
            if (wejscie.Contains(";")) 
                WEJSCIE += wejscie;
            else
                WEJSCIE += wejscie +";";
        }

        public string[,] MemoryArray()
        {
            string[,] arr = new string[MEMORYY.Count, 2];
            for (int i = 0; i < MEMORYY.Count; i++)
            {
                arr[i, 0] = MEMORYY[i].zmienna;
                arr[i, 1] = MEMORYY[i].wartosc;
            }
            return arr;
        }

        public string WEJSCIE { get; set; }
        public string WYJSCIE = "";

        public void OBL()
        {
            WYJSCIE = "";
            List<string> OPERACJA = WEJSCIE.Replace(" ","").Replace("\n", "").Replace("\r", "").Split(';').ToList();

            for (int i = 0; i < OPERACJA.Count; i++)
            {
                if (OPERACJA[i].Contains("="))
                {
                    List<string> linia = OPERACJA[i].Split('=').ToList();

                    if (linia.Count > 1)
                    {
                        EQEXE eqexe = new EQEXE();
                        string L = eqexe.tryExe(linia[0]);
                        string P = eqexe.tryExe(linia[1]);


                        if (L == "")
                        {
                            if (wartosc(linia[0]).Length > 0)
                            {
                               OPERACJA[i] = linia[0] + "=" + wartosc(linia[0]) + ";";
                            }
                            else
                            {
                                if (linia[1].Length > 0)
                                {
                                    string roboczy = podstaw(linia[1], MEMORYY);
                                    eqexe = new EQEXE(roboczy);

                                    if (linia.Count > 2 && linia[2].Length == 0)
                                    {
                                        OPERACJA[i] = linia[0] + "=" + linia[1] + "=" + eqexe.st + ";";
                                    }
                                    else
                                        OPERACJA[i] = linia[0] + "=" + linia[1] + ";";

                                    if (!MEMORYY.Contains(new MEMORY { zmienna = linia[0], wartosc = eqexe.st }))
                                        MEMORYY.Add(new MEMORY { zmienna = linia[0], wartosc = eqexe.st });
                                }
                                else
                                {
                                    string roboczy = podstaw(linia[0], MEMORYY);
                                    eqexe = new EQEXE(roboczy);

                                    OPERACJA[i] = linia[0] + "=" + eqexe.st + ";";

                                    if (!MEMORYY.Contains(new MEMORY { zmienna = linia[0], wartosc = eqexe.st }))
                                        MEMORYY.Add(new MEMORY { zmienna = linia[0], wartosc = eqexe.st });
                                }
                            }
                        }
                        else
                        {
                            if (L == P)
                            {
                                OPERACJA[i] = linia[0] + "=" + linia[1] + ";";
                            }
                            else
                            {
                                string roboczy = podstaw(linia[0], MEMORYY);
                                eqexe = new EQEXE(roboczy);
                                OPERACJA[i] = linia[0] + "=" + eqexe.st + ";";
                            }
                        }
                    }
                }
                else
                {
                    if (OPERACJA[i].Length > 0)
                    {
                        EQEXE eqexe = new EQEXE();

                        string roboczy = podstaw(OPERACJA[i], MEMORYY);
                        string L = eqexe.tryExe(roboczy);

                        if (L != "")
                        {
                            eqexe = new EQEXE(roboczy);

                            if (eqexe.st.Length > 0)
                                OPERACJA[i] = OPERACJA[i] + "=" + eqexe.st + ";";
                        }
                    }
                }

                if (OPERACJA[i].Length > 0)
                    WYJSCIE += OPERACJA[i] + "\r\n";
            }
        }
    } 

    public class EQEXE
    {
        public double WYJSCIE = 0.0;

        public string st = "";

        public EQEXE(string ROWNANIE)
        {
            ONP onp = new ONP(ROWNANIE.ToUpper());
            List<string> WEJSCIE = onp.WYJSCIE;

            Stack<string> STOS = new Stack<string> { };

            string REGEX = NUMERIC.REGEX;

            List<OPERATOR> LOP = OPERATORS.OPERATORS_;
            List<OPERATOR> LOF = FUNCTIONS.FUNKCJA;

            for (int i = 0; i < WEJSCIE.Count; i++)
            {
                Match rx = Regex.Match(WEJSCIE[i], REGEX, RegexOptions.IgnoreCase);
                if (rx.Success)
                {
                    if (WEJSCIE[i].ToUpper() == "PI")
                        STOS.Push(Math.PI.ToString());
                    else
                    {
                        if (WEJSCIE[i].ToUpper() == "E")
                            STOS.Push(Math.E.ToString());
                        else
                            STOS.Push(WEJSCIE[i]);
                    }
                }
                else
                {
                    foreach (OPERATOR O in LOP)
                    {
                        if (WEJSCIE[i].ToUpper() == O.Body.ToUpper())
                        {
                            switch (O.type)
                            {
                                case 2:
                                    {
                                        // 2 liczby
                                        try
                                        {
                                            string l1 = STOS.Pop();

                                            string l2 = STOS.Pop();

                                            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                                            l1 = l1.Replace(".", separator);
                                            l2 = l2.Replace(".", separator);

                                            double liczba1 = 0;
                                            double liczba2 = 0;

                                            double.TryParse(l1, out liczba1);
                                            double.TryParse(l2, out liczba2);

                                            

                                            EQUALS eq = new EQUALS();
                                            double SUMA = eq.WYJSCIE(WEJSCIE[i].ToUpper(), liczba2, liczba1);

                                            STOS.Push(SUMA.ToString());

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        // 1 liczba
                                        try
                                        {
                                            string l1 = STOS.Pop();
                                            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                                            l1 = l1.Replace(".", separator);

                                            double liczba1 = 0;

                                            double.TryParse(l1, out liczba1);

                                            EQUALS eq = new EQUALS();
                                            double SUMA = eq.WYJSCIE(WEJSCIE[i].ToUpper(), liczba1, 0);

                                            STOS.Push(SUMA.ToString());
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    foreach (OPERATOR O in LOF)
                    {
                        if (WEJSCIE[i].ToUpper() == O.Body.ToUpper())
                        {
                            List<double> ParametersZ = new List<double> { };

                            try
                            {
                                for (int k = 0; k < O.type; k++)
                                {
                                    string l1 = STOS.Pop();

                                    string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                                    l1 = l1.Replace(".", separator);

                                    double liczba1 = 0;
                                    double.TryParse(l1, out liczba1);
                                    ParametersZ.Add(liczba1);
                                }

                                List<double> Parameters = new List<double> { };

                                for (int k = ParametersZ.Count - 1; k > -1; k--)
                                {
                                    Parameters.Add(ParametersZ[k]);
                                }

                                EQUALS eq = new EQUALS();

                                double SUMA = eq.WYJSCIE(WEJSCIE[i].ToUpper(), Parameters.ToArray());

                                STOS.Push(SUMA.ToString());
                            }
                            catch
                            {
                                MessageBox.Show("Za mało parametrów funkcji, bądź parametry są niewłaściwe", "Bład zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }

            }

            try
            {

                string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                string l1 = STOS.Pop();
                l1 = l1.Replace(separator , ".");

                st = l1;
                //MessageBox.Show(STOS.Pop());
            }
            catch
            {
                MessageBox.Show("Nie udało się policzyć wyrażenia: " + ROWNANIE, "Bład zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public EQEXE()
        {
        }

        public string tryExe(string ROWNANIE)
        {
            ONP onp = new ONP(ROWNANIE.ToUpper());
            List<string> WEJSCIE = onp.WYJSCIE;

            Stack<string> STOS = new Stack<string> { };

            string REGEX = NUMERIC.REGEX;

            List<OPERATOR> LOP = OPERATORS.OPERATORS_;
            List<OPERATOR> LOF = FUNCTIONS.FUNKCJA;

            for (int i = 0; i < WEJSCIE.Count; i++)
            {
                Match rx = Regex.Match(WEJSCIE[i], REGEX, RegexOptions.IgnoreCase);
                if (rx.Success)
                {
                    if (WEJSCIE[i].ToUpper() == "PI")
                        STOS.Push(Math.PI.ToString());
                    else
                    {
                        if (WEJSCIE[i].ToUpper() == "E")
                            STOS.Push(Math.E.ToString());
                        else
                            STOS.Push(WEJSCIE[i]);
                    }
                }
                else
                {
                    foreach (OPERATOR O in LOP)
                    {
                        if (WEJSCIE[i].ToUpper() == O.Body.ToUpper())
                        {
                            switch (O.type)
                            {
                                case 2:
                                    {
                                        // 2 liczby
                                        try
                                        {
                                            string l1 = STOS.Pop();

                                            string l2 = STOS.Pop();

                                            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                                            l1 = l1.Replace(".", separator);
                                            l2 = l2.Replace(".", separator);

                                            double liczba1 = 0;
                                            double liczba2 = 0;

                                            double.TryParse(l1, out liczba1);
                                            double.TryParse(l2, out liczba2);


                                            EQUALS eq = new EQUALS();
                                            double SUMA = eq.WYJSCIE(WEJSCIE[i].ToUpper(), liczba2, liczba1);

                                            STOS.Push(SUMA.ToString());

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        // 1 liczba
                                        try
                                        {
                                            string l1 = STOS.Pop();
                                            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                                            l1 = l1.Replace(".", separator);

                                            double liczba1 = 0;

                                            double.TryParse(l1, out liczba1);

                                            EQUALS eq = new EQUALS();
                                            double SUMA = eq.WYJSCIE(WEJSCIE[i].ToUpper(), liczba1, 0);

                                            STOS.Push(SUMA.ToString());
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    foreach (OPERATOR O in LOF)
                    {
                        if (WEJSCIE[i].ToUpper() == O.Body.ToUpper())
                        {
                            List<double> ParametersZ = new List<double> { };

                            try
                            {
                                for (int k = 0; k < O.type; k++)
                                {
                                    string l1 = STOS.Pop();
                                    double liczba1 = 0;
                                    double.TryParse(l1, out liczba1);
                                    ParametersZ.Add(liczba1);
                                }

                                List<double> Parameters = new List<double> { };

                                for (int k = ParametersZ.Count - 1; k > -1; k--)
                                {
                                    Parameters.Add(ParametersZ[k]);
                                }

                                EQUALS eq = new EQUALS();

                                double SUMA = eq.WYJSCIE(WEJSCIE[i].ToUpper(), Parameters.ToArray());

                                STOS.Push(SUMA.ToString());
                            }
                            catch
                            {
                            }
                        }
                    }
                }

            }

            try
            {
                string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                string l1 = STOS.Pop();
                l1 = l1.Replace(separator, ".");

                return l1;
            }
            catch
            {
                return "";
            }
            
        }
    }

    public class ONP
    {
        public List<string> WYJSCIE = new List<string> { };
        public ONP(string WEJSCIE)
        {
            List<OPERATOR> LOP = OPERATORS.OPERATORS_;
            List<OPERATOR> LFU = FUNCTIONS.FUNKCJA;

            DEKOMPOSITION DEKOMPONUJ = new DEKOMPOSITION(WEJSCIE);
            List<string> OPE = DEKOMPONUJ.WYJSCIE;

            Stack stack = new Stack { };
            List<string> WY = new List<string> { };

            string REGEX = NUMERIC.REGEX;
            string REGEXF = FUNCTIONS.REGEX();
            string REGEXO = OPERATORS.REGEX();

            try
            {
                for (int i = 0; i < OPE.Count; i++)
                {
                    Match rx = Regex.Match(OPE[i], REGEX, RegexOptions.IgnoreCase);
                    if (rx.Success)
                    {
                        WY.Add(OPE[i]);
                    }
                    else
                    {
                        Match rxx = Regex.Match(OPE[i], REGEXF, RegexOptions.IgnoreCase);
                        if (rxx.Success)
                        {
                            stack.Push(OPE[i]);
                        }
                        else
                        {
                            if (OPE[i] == "(")
                            {
                                stack.Push("(");
                            }
                            else
                            {
                                if (OPE[i] == ",")
                                {
                                    while (stack.Count != 0 && stack.Peek().ToString() != "(")
                                    {
                                        WY.Add(stack.Pop().ToString());
                                    }
                                }
                                else
                                {
                                    if (OPE[i] == ")")
                                    {
                                        while (stack.Count != 0 && stack.Peek().ToString() != "(")
                                        {
                                            WY.Add(stack.Pop().ToString());
                                        }

                                        //if (stack.Count != 0)
                                        //    MessageBox.Show(stack.Peek().ToString());

                                        if (stack.Count != 0 && stack.Peek().ToString() == "(")
                                        {
                                            stack.Pop();

                                            if (stack.Count != 0)
                                            {
                                                Match rx2 = Regex.Match(stack.Peek().ToString(), REGEXF, RegexOptions.IgnoreCase);
                                                if (rx2.Success)
                                                {
                                                    WY.Add(stack.Pop().ToString());
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int Validyty = 0;
                                        foreach (OPERATOR O in LOP)
                                        {
                                            if (O.Body == OPE[i])
                                            {
                                                Validyty = O.Validyty;
                                            }
                                        }

                                        while (stack.Count != 0
                                            && OPERATORS.GetValidyty(stack.Peek().ToString()) >= Validyty
                                            )
                                        {
                                            WY.Add(stack.Pop().ToString());
                                        }

                                        stack.Push(OPE[i]);
                                    }
                                }
                            }
                        }
                    }
                }
                while (stack.Count != 0)
                {
                    WY.Add(stack.Pop().ToString());
                }
            }
            catch
            {
                MessageBox.Show("Nie udało się przekształcić wyrażenia :" + WEJSCIE, "Bład zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            WYJSCIE = WY;
        }

        public string LINEAR()
        {
            string output = "";
            for (int i = 0; i < WYJSCIE.Count; i++)
            {
                if (i < WYJSCIE.Count - 1)
                    output += WYJSCIE[i] + " ";
                else
                    output += WYJSCIE[i];
            }
            return output;
        }
    }

    public class DEKOMPOSITION
    {
        public List<string> WYJSCIE = new List<string> { };
        public DEKOMPOSITION(string WEJSCIE)
        {
            string regex = NUMERIC.REGEX + "|(" + OPERATORS.REGEX() + "|" + FUNCTIONS.REGEX() + ")";
            string REGEXF = FUNCTIONS.REGEX();
            string regexnf = NUMERIC.REGEX + "|" + FUNCTIONS.REGEX() ;

            string[] result1 = Regex.Split(WEJSCIE, regex, RegexOptions.IgnoreCase);

            List<string> WYJSCIE_TP = new List<string> { };
            Stack<string> stack = new Stack<string> { };


            List<string> resultl = new List<string> { };
            for (int i = 0; i < result1.Length; i++)
            {
                if (result1[i].Replace(" ","").Length > 0)
                {
                    resultl.Add(result1[i]);
                }
            }
            string[] result = resultl.ToArray(); 


            for (int i = 0; i < result.Length; i++)
            {
                if (i > 0)
                {
                    if (result[i].Length > 0)
                    {
                        if (result[i] == "-")
                        {
                            Match rx = Regex.Match(result[i - 1], NUMERIC.REGEX, RegexOptions.IgnoreCase);

                            if (result[i - 1] != ")" && !rx.Success)
                            {
                                WYJSCIE_TP.Add("[");
                                WYJSCIE_TP.Add("0");
                                WYJSCIE_TP.Add(result[i]);
                            }
                            else
                            {
                                WYJSCIE_TP.Add(result[i]);
                            }
                        }
                        else
                        {
                            WYJSCIE_TP.Add(result[i]);
                        }
                    }
                }
                else
                {
                    if (result[i].Length > 0)
                    {
                        if (result[i] == "-")
                        {
                            WYJSCIE_TP.Add("[");
                            WYJSCIE_TP.Add("0");
                            WYJSCIE_TP.Add(result[i]);
                        }
                        else
                        {
                            WYJSCIE_TP.Add(result[i]);
                        }
                    }
                }
            }

            List<string> WYJSCIE2 = new List<string> { };
            Stack<string> stak = new Stack<string> { };

            for (int i = 0; i < WYJSCIE_TP.Count; i++)
            {
                switch (WYJSCIE_TP[i])
                {
                    case "(":
                        {
                            WYJSCIE2.Add("(");
                            stak.Push("(");
                        }
                        break;
                    case "[":
                        {
                            WYJSCIE2.Add("(");
                            stak.Push("[");
                        }
                        break;
                    case ")":
                        {
                            if (stak.Count != 0 && stak.Peek().ToString() == "(")
                            {
                                WYJSCIE2.Add(")");
                                stak.Pop();
                            }
                            if (stak.Count != 0 && stak.Peek().ToString() == "[")
                            {
                                WYJSCIE2.Add(")");
                                stak.Pop();
                            }
                            if (stak.Count != 0 && stak.Peek().ToString() == "(")
                            {
                                WYJSCIE2.Add(")");
                                stak.Pop();
                            }
                        }
                        break;
                    default:
                        {
                            if (i > 2)
                            {
                                Match ry = Regex.Match(WYJSCIE_TP[i], NUMERIC.REGEX, RegexOptions.IgnoreCase);
                                if (WYJSCIE_TP[i - 3] == "[" && WYJSCIE_TP[i - 2] == "0" && WYJSCIE_TP[i - 1] == "-" && ry.Success)
                                {
                                    if (stak.Count != 0 && stak.Peek().ToString() == "[")
                                    {
                                        WYJSCIE2.Add(WYJSCIE_TP[i]);
                                        WYJSCIE2.Add(")");
                                        stak.Pop();
                                    }
                                }
                                else
                                {
                                    WYJSCIE2.Add(WYJSCIE_TP[i]);
                                }
                            }
                            else
                                WYJSCIE2.Add(WYJSCIE_TP[i]);
                        }
                        break;
                }

            }

            WYJSCIE = WYJSCIE2;
        }

        void msg(string[] arr, string[] arr0, string z)
        {
            string s = "";

            for (int i = arr.Length - 1; i > -1; i--)
            {
                s += arr[i];
            }

            string s2 = "";

            for (int i = 0; i < arr0.Length; i++)
            {
                s2 += arr0[i]; 
            }
            MessageBox.Show(z + "\r\n" + s+"\r\n"+s2);
        }

        void msg2(string[] arr)
        {
            string s = "";

            for (int i = 0; i < arr.Length ; i++)
            {
                s += arr[i];
            }

            MessageBox.Show(s);
        }

        public string LINEAR()
        {
            string output = "";
            for (int i = 0; i < WYJSCIE.Count; i++)
            {
                if (i < WYJSCIE.Count - 1)
                    output += WYJSCIE[i] + " ";
                else
                    output += WYJSCIE[i];
            }
            return output;
        }
    }

    static class NUMERIC
    {
        public static string REGEX = "";
        static NUMERIC()
        {
            REGEX = @"(^\d*\.\d+$)|(^\d+$)|(^Pi$|^e$)";
        }
    }

    static class OPERATORS
    {
        static public List<OPERATOR> OPERATORS_ = new List<OPERATOR> { };

        static public void AddOperator(string Body, int Validaty, int type)
        {
            OPERATOR op = new OPERATOR();
            op.Body = Body;
            op.Validyty = Validaty;
            op.type = type;
            OPERATORS_.Add(op);
            OPERATORS_.Sort((a, b) => b.Body.Length.CompareTo(a.Body.Length));
        }

        static public string REGEX()
        {
            string rex = "";


            List<OPERATOR> Oper = OPERATORS.OPERATORS_;

            for (int i = 0; i < Oper.Count; i++)
            {
                if (i < Oper.Count - 1)
                {
                    if (Oper[i].Body.Length == 1)
                        rex += @"\" + Oper[i].Body + "|";
                    else
                        rex += @"^" + Oper[i].Body + "$|";
                }
                else
                {
                    if (Oper[i].Body.Length == 1)
                        rex += @"\" + Oper[i].Body;
                    else
                        rex += @"^" + Oper[i].Body + "$";
                }
            }

            return rex;
        }

        static OPERATORS()
        {
            AddOperator("(", 0, 0);
            AddOperator(",", 1, 0);
            AddOperator(")", 1, 0);
            AddOperator("+", 1, 2);
            AddOperator("-", 1, 2);
            AddOperator("*", 2, 2);
            AddOperator(@"/", 2, 2);
            AddOperator("%", 2, 1);
            AddOperator("^", 3, 2);
            AddOperator(@"\", 3, 2);
        }

        static public int GetValidyty(string body)
        {
            int i = 0;
            foreach (OPERATOR O in OPERATORS_)
            {
                if (O.Body == body)
                    i = O.Validyty;
            }
            return i;
        }

    }

    static class FUNCTIONS
    {
        static public List<OPERATOR> FUNKCJA = new List<OPERATOR> { };

        static public void AddFunkcje(string Body, int Validaty, int type)
        {
            OPERATOR op = new OPERATOR();
            op.Body = Body;
            op.Validyty = Validaty;
            op.type = type;
            FUNKCJA.Add(op);
            FUNKCJA.Sort((a, b) => b.Body.Length.CompareTo(a.Body.Length));
        }

        static public string REGEX()
        {
            string rex = "";


            List<OPERATOR> Oper = FUNCTIONS.FUNKCJA;

            for (int i = 0; i < Oper.Count; i++)
            {
                if (i < Oper.Count - 1)
                {
                    if (Oper[i].Body.Length == 1)
                        rex += @"\" + Oper[i].Body + "|";
                    else
                        rex += @"^" + Oper[i].Body + "$|";
                }
                else
                {
                    if (Oper[i].Body.Length == 1)
                        rex += @"\" + Oper[i].Body;
                    else
                        rex += @"^" + Oper[i].Body + "$";
                }
            }

            return rex;
        }

        static FUNCTIONS()
        {
            AddFunkcje("SQRT", 4, 1);
            AddFunkcje("SIN", 4, 1);
            AddFunkcje("COS", 4, 1);
            AddFunkcje("ASIN", 4, 1);
            AddFunkcje("ACOS", 4, 1);
            AddFunkcje("TAN", 4, 1);
            AddFunkcje("ATAN", 4, 1);
            AddFunkcje("SINH", 4, 1);
            AddFunkcje("COSH", 4, 1);
            AddFunkcje("TANH", 4, 1);
            AddFunkcje("WGORE", 4, 1);
            AddFunkcje("WDOL", 4, 1);
            AddFunkcje("LN", 4, 1);
            AddFunkcje("MOD", 4, 1);
            AddFunkcje("LOG10", 4, 1);
            AddFunkcje("RAD", 4, 1);
            AddFunkcje("DEG", 4, 1);

            AddFunkcje("LOG", 4, 2);
            AddFunkcje("MAX", 4, 2);
            AddFunkcje("MIN", 4, 2);
            AddFunkcje("ZAOKR", 4, 2);
            AddFunkcje("REST", 4, 2);
            AddFunkcje("RT", 4, 2);
            AddFunkcje("DL", 4, 2);
        }
    }

    public class EQUALS
    {
        public double WYJSCIE(string body, params double[] a)
        {
            double OUTPUT = 0;

            switch (body.ToUpper())
            {
                case "+":
                    {
                        OUTPUT = a[0] + a[1];
                    }
                    break;
                case "-":
                    {
                        OUTPUT = a[0] - a[1];
                    }
                    break;
                case "/":
                    {
                        OUTPUT = a[0] / a[1];
                    }
                    break;
                case "*":
                    {
                        OUTPUT = a[0] * a[1];
                    }
                    break;
                case "^":
                    {
                        OUTPUT = Math.Pow(a[0], a[1]);
                    }
                    break;
                case "SQRT":
                    {
                        OUTPUT = Math.Sqrt(a[0]);
                    }
                    break;
                case "%":
                    {
                        OUTPUT = a[0] / 100;
                    }
                    break;
                case "LOG10":
                    {
                        OUTPUT = Math.Log10(a[0]);
                    }
                    break;
                case "LN":
                    {
                        OUTPUT = Math.Log(a[0]);
                    }
                    break;
                case "SIN":
                    {
                        OUTPUT = Math.Sin(a[0]);
                    }
                    break;
                case "COS":
                    {
                        OUTPUT = Math.Cos(a[0]);
                    }
                    break;
                case "TAN":
                    {
                        OUTPUT = Math.Tan(a[0]);
                    }
                    break;
                case "ASIN":
                    {
                        OUTPUT = Math.Asin(a[0]);
                    }
                    break;
                case "ACOS":
                    {
                        OUTPUT = Math.Acos(a[0]);
                    }
                    break;
                case "ATAN":
                    {
                        OUTPUT = Math.Atan(a[0]);
                    }
                    break;
                case "SINH":
                    {
                        OUTPUT = Math.Sinh(a[0]);
                    }
                    break;
                case "COSH":
                    {
                        OUTPUT = Math.Cosh(a[0]);
                    }
                    break;
                case "tanh":
                    {
                        OUTPUT = Math.Tanh(a[0]);
                    }
                    break;
                case "WGORE":
                    {
                        OUTPUT = Math.Floor(a[0]);
                    }
                    break;
                case "WDOL":
                    {
                        OUTPUT = Math.Ceiling(a[0]);
                    }
                    break;
                case "REST":
                    {
                        OUTPUT = Math.IEEERemainder(a[0], a[1]);
                    }
                    break;
                case "MAX":
                    {
                        OUTPUT = Math.Max(a[0], a[1]);
                    }
                    break;
                case "MIN":
                    {
                        OUTPUT = Math.Min(a[0], a[1]);
                    }
                    break;
                case "ZAOKR":
                    {
                        int c = (int)Math.Ceiling(a[1]);
                        OUTPUT = Math.Round(a[0], c);
                    }
                    break;
                case "MOD":
                    {
                        OUTPUT = Math.Abs(a[0]);
                    }
                    break;
                case @"\":
                    {
                        OUTPUT = Math.Pow(a[1],  1 / a[0]);
                    }
                    break;
                case "RT":
                    {
                        OUTPUT = Math.Pow(a[1], 1 / a[0]);
                    }
                    break;
                case "DL":
                    {
                        OUTPUT = Math.Sqrt(Math.Pow(a[0], 2) + Math.Pow(a[1], 2));
                    }
                    break;
                case "LOG":
                    {
                        OUTPUT = Math.Log(a[0], a[1]);
                    }
                    break;
                case "DEG":
                    {
                        OUTPUT = a[0] * 180 / Math.PI;
                    }
                    break;
                case "RAD":
                    {
                        OUTPUT = a[0] * Math.PI / 180;
                    }
                    break;
            }

            return OUTPUT;
        }

    }

    public struct OPERATOR
    {
        public string Body;
        public int Validyty;
        public int type;
    }
}
