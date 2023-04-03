using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHFQuestToMH2Dos
{
    public static class Log
    {
        public static void HexInfo(long HexPos,string log, params long[] arr)
        {
            log = "0x" + HexPos.ToString("X") + ":" +log;
            if(arr != null)
            {
                string[] strarr = new string[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    strarr[i] = arr[i] + "(0x" + arr[i].ToString("X") + ")";
                }
                log = String.Format(log, strarr);
            }
            //TODO 改成别的方式记录
            Console.WriteLine(log);
        }

        public static void HexTips(long HexPos, string log, params long[] arr)
        {
            ConsoleColor src_color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            HexInfo(HexPos, log, arr);
            Console.ForegroundColor = src_color;
        }

        public static void HexWar(long HexPos, string log, params long[] arr)
        {
            ConsoleColor src_color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            HexInfo(HexPos, log, arr);
            Console.ForegroundColor = src_color;
        }

        public static void HexColor(ConsoleColor color,long HexPos, string log, params long[] arr)
        {
            ConsoleColor src_color = Console.ForegroundColor;
            Console.ForegroundColor = color;
            HexInfo(HexPos, log, arr);
            Console.ForegroundColor = src_color;
        }

        public static void Info(string log)
        {
            Console.WriteLine(log);
        }
    }
}
