using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHFQuestToMH2Dos
{
    public static class ModifyQuest
    {
        /// <summary>
        /// Dos中无意义数据
        /// </summary>
        const int fixindex_1 = 19;

        const int Offset = 12;
        const int Offset_2nd = 8;

        const int moveLenght = 64;

        public static bool ModifyFile(byte[] src,out byte[] target)
        {
            try
            {
                target = new byte[src.Length];

                for (int i = 0; i < src.Length; i++)
                {
                    target[i] = src[i];
                }

                //清除对于2Dos来说 无意义的数据
                target[fixindex_1] = 0x00;

                string IntPtrHex = "";
                bool flag = false;

                for (int i = 3; i >= 0; i--)
                {
                    if (target[i] != 0x00)
                        flag = true;

                    if (flag)
                        IntPtrHex += target[i].ToString("X");
                    else if (target[i] != 0x00)
                        IntPtrHex += target[i].ToString("X");
                }


                //从前4字节取出指针 定位任务信息位置
                long PtrIndex = HexHelper.HexaToDecimal(IntPtrHex);

                //MHF偏移12的位置
                long MoveStarPtr = PtrIndex + Offset;
                //目标2Dos偏移8的位置
                long targetStarPtr = PtrIndex + Offset_2nd;


                //取出原始数据
                byte[] temp = new byte[moveLenght];
                for (int i = 0; i < moveLenght; i++)
                {
                    temp[i] = target[MoveStarPtr + i];
                }

                //清理原始数据
                for (int i = 0; i < moveLenght; i++)
                {
                    target[MoveStarPtr + i] = 0x00;
                }
                //将temp数据往前位移4字节
                for (int i = 0; i < moveLenght; i++)
                {
                    target[targetStarPtr + i] = temp[i];
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                target = null;
                return false;
            }
        }
    }
}
