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
        const int cNon0x00For2DosPtr = 19;
        /// <summary>
        /// MHF任务信息偏移
        /// </summary>
        const int cQuestMHFOffset = 12;
        /// <summary>
        /// 2Dos任务信息偏移
        /// </summary>
        const int cQuest2DosOffset = 8;
        /// <summary>
        /// 任务信息长度
        /// </summary>
        const int cQuestInfoLenght = 64;
        /// <summary>
        /// 任务_类型 偏移
        /// </summary>
        const int cQuestInfo_Type_Offset = 0;
        /// <summary>
        /// 任务_类型 长度
        /// </summary>
        const int cQuestInfo_Type_Lenght = 1;
        /// <summary>
        /// 任务_类型 偏移
        /// </summary>
        const int cQuestInfo_TargetMap_Offset = 32;
        /// <summary>
        /// 任务_类型 长度
        /// </summary>
        const int cQuestInfo_TargetMapID_Lenght = 1;

        public static bool ModifyQuset(byte[] src, out byte[] target)
        {
            target = null;

            if (!ModifyFileOffset(src, out byte[] Setp1out))
                return false;

            if (!ModifyQuestMap(Setp1out, out byte[] Setp2out))
                return false;

            target = Setp2out;
            return true;
        }


        public static bool ModifyFileOffset(byte[] src, out byte[] target)
        {
            try
            {
                //加载数据
                target = new byte[src.Length];
                for (int i = 0; i < src.Length; i++)
                    target[i] = src[i];

                //从前4字节取出指针 定位任务信息位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);

                //----Step---- 清除对于2Dos来说 无意义的数据
                target[cNon0x00For2DosPtr] = 0x00;

                //----Step---- 前移任务数据4字节 （MHF比2Dos后移了4字节，MHF：+12 2Dos： +8）
                //MHF偏移12的位置
                long QuestInfoPtr_MHFTarget = _QuestInfoPtr + cQuestMHFOffset;
                //目标2Dos偏移8的位置
                long QuestInfoPtr_2DosTarget = _QuestInfoPtr + cQuest2DosOffset;
                //取出原始数据
                byte[] temp = new byte[cQuestInfoLenght];
                for (int i = 0; i < cQuestInfoLenght; i++)
                {
                    temp[i] = target[QuestInfoPtr_MHFTarget + i];
                }
                //清理原始数据
                for (int i = 0; i < cQuestInfoLenght; i++)
                {
                    target[QuestInfoPtr_MHFTarget + i] = 0x00;
                }
                //将temp数据往前位移4字节
                for (int i = 0; i < cQuestInfoLenght; i++)
                {
                    target[QuestInfoPtr_2DosTarget + i] = temp[i];
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


        public static bool ModifyQuestMap(byte[] src, out byte[] target)
        {
            try
            {
                //加载数据
                target = new byte[src.Length];
                for (int i = 0; i < src.Length; i++)
                    target[i] = src[i];

                //从前4字节取出指针 定位任务信息位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);

                //----Step---- 读取任务数据
                //任务类型
                int _QuestType = HexHelper.bytesToInt(target, cQuestInfo_Type_Lenght, _QuestInfoPtr + cQuestInfo_Type_Offset);
                int _QuestTargetMapID = HexHelper.bytesToInt(target, cQuestInfo_TargetMapID_Lenght, _QuestInfoPtr + cQuestInfo_TargetMap_Offset);

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
