using NumeroExtenso;
using System;

namespace ValorPorExtenso
{
    class Program
    {
        static void Main(string[] args)
        {
            double numDouble = 0;   //Definir o dloube para extenso de moedas
            Int64 numInt = 0;       //Definir o int para extenso de inteiros
            decimal numDecimal = 0; //Definir o decimal para números decimais
            do
            {
                string valor = Console.ReadLine();
                numDouble = double.Parse(valor);
                numInt = (int)numDouble;
                numDecimal = decimal.Parse(valor);

                //   Console.WriteLine(numDouble.ToLongString()); //números com decimais
                //   Console.WriteLine(numInt.ToLongString()); //números inteiros
                Console.WriteLine(numDecimal.ToLongString());  //Extenso de moeda

            }
            while (numDouble != 0);  //zero sai
        }
    }
}
