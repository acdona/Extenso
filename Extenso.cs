using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;

namespace NumeroExtenso
{
    public static class Extenso
    {
        private static readonly string[] CardinaisAte19 = { "zero", "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove", "dez", "onze", "doze", "treze", "quatorze", "quinze", "dezesseis", "dezessete", "dezoito", "dezenove" };
        private static readonly string[] Cardinais20ate90 = { "vinte", "trinta", "quarenta", "cinquenta", "sessenta", "setenta", "oitenta", "noventa" };
        private static readonly string[] Cardinais100ate900 = { "cem", "cento", "duzentos", "trezentos", "quatrocentos", "quinhentos", "seiscentos", "setecentos", "oitocentos", "novecentos" };
        private static readonly string[,] Cardinais1000 = { { "", "" }, { "mil", "mil" }, { "milhão", "milhões" }, { "bilhão", "bilhões" }, { "trilhão", "trilhões" }, { "quadrilhão", "quadrilhões" } };
        private static readonly string[] CardinaisDecimo = { "", "décimo", "centésimo" };
        private static readonly string[] CardinaisFracao = { "", "milésimo", "milionésimo", "bilionésimo", "trilionésimo" };
        private static readonly string[] Moeda = { "real", "reais" };

        public static string ToLongString(this Int64 valor)
        {
            return IntToLongString(valor);
        }
        public static string ToLongString(this Double valor)
        {
            return DoubleToLongString(valor);
        }
        public static string ToLongString(this Decimal valor)
        {
            return DecimalToLongString(valor, Moeda);
        }

        public static string IntToLongString(Int64 valor)
        {
            StringBuilder sb = new StringBuilder();

            if (valor == 0)
            {
                sb.Append(CardinaisAte19[0]);
                return sb.ToString();
            }

            if (valor < 0)
            {
                sb.Append("menos ");
            }

            sb.Append(IntMilhares(Math.Abs(valor)));
            return sb.ToString();
        }

        public static string DoubleToLongString(Double valor)
        {
            StringBuilder sb = new StringBuilder();

            Decimal dec = (Decimal)valor - Decimal.Truncate((Decimal)valor);

            if (!(valor > 0 && valor < 1))
            {
                sb.Append(IntToLongString((Int64)valor));

                if (dec > 0 && dec < 1)
                    sb.Append(" virgula ");
            }

            if (dec > 0)
            {
                string decString = dec.ToString().Substring(2);
                Int64 decAsInt = Int64.Parse(decString);
                if (decAsInt > 0)
                {
                    int qntCasas = decString.Count();

                    sb.Append(IntMilhares(decAsInt));
                    sb.Append(" ");

                    int mod = qntCasas % 3;
                    int div = qntCasas / 3;
                    bool plural = decAsInt > 1;

                    if (div <= 4)
                    {
                        if (mod > 0)
                        {
                            sb.Append(CardinaisDecimo[mod]);
                            sb.Append(plural ? "s" : "");
                        }
                        if (mod > 0 && div > 0)
                            sb.Append(" de "); 
                        if (div > 0)
                        {
                            sb.Append(CardinaisFracao[div]);
                            sb.Append(plural ? "s" : "");
                        }
                    }
                    else
                    {
                        return valor.ToString();
                    }
                }
            }
            return sb.ToString();
        }

        public static string DecimalToLongString(Decimal valor, string[] Moeda)
        {
            string[] moedaNome = { Moeda[0], Moeda[1] };
            string[] centavoNome = { "centavo", "centavos" };

            Decimal rounded = Decimal.Round(valor, 2);
            Int64 intero = Decimal.ToInt64(rounded);
            Int64 centavos = (Int64)(valor * 100) % 100;

            double logMil = (int)Math.Log(intero, 1000);
            bool incluiDe = (logMil >= 2 && (int)logMil == logMil);

            decimal n = Convert.ToString(intero).Length;

            if (n >= 6)
            {
                string str = Convert.ToString(intero);
                str = str.Substring((int)(n - 6));
                n = decimal.Parse(str);

                if (n > 0)
                {
                    incluiDe = false;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(intero.ToLongString());
            sb.Append(" ");
            if (incluiDe)
            {
                sb.Append("de ");
            }
            sb.Append(moedaNome[intero == 1 ? 0 : 1]);
            if (centavos > 0)
            {
                sb.Append(" e ");
                sb.Append(centavos.ToLongString());
                sb.Append(" ");
                sb.Append(centavoNome[centavos == 1 ? 0 : 1]);
            }

            return sb.ToString();
        }

        private static string Int3Casas(Int64 valor)
        {
            StringBuilder sb = new StringBuilder();

            if (valor > 0 && valor < 20) sb.Append(CardinaisAte19[valor]);
            else if (valor >= 20 && valor < 100)
            {
                Int64 div = valor / 10;
                Int64 mod = valor % 10;

                sb.Append(Cardinais20ate90[div - 2]);

                if (mod > 0)
                {
                    sb.Append(" e ");
                    sb.Append(CardinaisAte19[mod]);
                }
            }
            else if (valor == 100) sb.Append(Cardinais100ate900[0]);
            else if (valor > 100)
            {
                sb.Append(Cardinais100ate900[valor / 100]);

                Int64 mod = valor % 100;
                if (mod > 0)
                {
                    sb.Append(" e ");
                    sb.Append(Int3Casas(mod));
                }
            }

            return sb.ToString();
        }

        private static string IntMilhares(Int64 valor)
        {
            int logDez = (int)Math.Log10(valor);
            int logMil = (int)Math.Log(valor, 1000);

            if (logMil == 0)
            {
                return Int3Casas(valor);
            }
            else if (logMil > 6) return "Impossivel Converter";
            else
            {
                StringBuilder sb = new StringBuilder();
                Int64 powMil = (Int64)Math.Pow(10, logMil * 3);

                Int64 centena = valor / powMil;
                Int64 resto = valor % powMil;

                bool umMil = centena == 1 && logMil == 1;

                if (!umMil)
                {
                    sb.Append(Int3Casas(centena));
                    sb.Append(" ");
                }

                sb.Append(Cardinais1000[logMil, centena > 1 ? 1 : 0]);

                if (resto > 0)
                {
                    string separador = ", ";
                    if (logMil == 1)
                    {
                        if (resto % 100 == 0 || resto < 100)
                        {
                            separador = " e ";
                        }
                        else
                        {
                            separador = " ";
                        }
                    }
                    else
                    {
                        if (resto < 1000)
                        {
                            separador = " e ";
                        }
                    }

                    sb.Append(separador);
                    sb.Append(IntMilhares(resto));
                }
                return sb.ToString();
            }

        }
    }
}