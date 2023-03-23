using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHFQuestToMH2Dos
{
    public class HexHelper
    {
        /// <summary>
        /// 另一种16进制转10进制的处理方式，Multiplier参与*16的循环很巧妙，对Multiplier的处理很推荐，逻辑统一
        /// </summary>
        /// <param name="HexaDecimalString"></param>
        /// <returns></returns>
        public static int HexaToDecimal(string HexaDecimalString)
        {
            int Decimal = 0;
            int Multiplier = 1;

            for (int i = HexaDecimalString.Length - 1; i >= 0; i--)
            {
                Decimal += HexaToDecimal(HexaDecimalString[i]) * Multiplier;
                Multiplier *= 16;
            }
            return Decimal;
        }

        static int HexaToDecimal(char c)
        {
            switch (c)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
            }
            return -1;
        }
    }
}
