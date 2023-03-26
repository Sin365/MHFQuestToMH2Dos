﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHFQuestToMH2Dos
{
    public static class ModifyQuest
    {

        const int cMax_MapID = 0x49;
        const int cMax_MonsterID = 0x49;
        const int cMax_ItemID = 0x031D;

        const int cMax_GuTi = 0x14;
        const int cMax_QuestStar = 8;

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
        /// 任务信息需偏移长度
        /// </summary>
        const int cQuestMhfToDosSetLenght = 64;

        /// <summary>
        /// 任务信息 指针组 总长度
        /// </summary>
        const int cQuest2DosInfoPtrGourpLenght = 72;
        /// <summary>
        /// 移动信息指针组 到的指定位置
        /// </summary>
        const int cSetInfoPtrGourpMoveToStarPos = 0x88;



        /// <summary>
        /// 任务内容 指针组 总长度
        /// </summary>
        const int cQuestContenPtrGourpLenght = 72;
        /// <summary>
        /// 任务内容 指针组 到的指定位置
        /// </summary>
        const int cQuestContenPtrGourpMoveToStarPos = 0xD0;



        /// <summary>
        /// 移动整个任务文本 到的指定位置
        /// </summary>
        const int cQuestTextAllMsgMoveToStarPos = 0xF0;
        /// <summary>
        /// 移动整个任务文本 到的指定的截止位置
        /// </summary>
        const int cQuestTextAllMsgMoveToEndPos = 0x1Ff;



        /// <summary>
        /// 任务_类型 偏移
        /// </summary>
        const int cQuestInfo_Type_Offset = 0;
        /// <summary>
        /// 任务_类型 长度
        /// </summary>
        const int cQuestInfo_Type_Lenght = 1;

        /// <summary>
        /// 任务_星级 偏移
        /// </summary>
        const int cQuestInfo_Star_Offset = 4;
        /// <summary>
        /// 任务_星级 长度
        /// </summary>
        const int cQuestInfo_Star_Lenght = 2;


        /// <summary>
        /// 任务_类型 偏移
        /// </summary>
        const int cQuestInfo_TargetMap_Offset = 32;


        /// <summary>
        /// 任务_类型 长度
        /// </summary>
        const int cQuestInfo_TargetMapID_Lenght = 1;

        /// <summary>
        /// 任务_类型 偏移
        /// </summary>
        const int cQuestInfo_QuestID_Offset = 42;
        /// <summary>
        /// 任务_类型 长度
        /// </summary>
        const int cQuestInfo_QuestID_Lenght = 2;

        public static bool ModifyQuset(byte[] src, out byte[] target)
        {
            target = HexHelper.CopyByteArr(src);//加载数据

            if (ModifyFileOffset(target, out byte[] out_ModifyFileOffset))
                target = out_ModifyFileOffset;

            if (ModifyTextOffset(target, out byte[] out_ModifyTextOffset))
                target = out_ModifyTextOffset;

            //if (ModifyQuestMap(target, out byte[] out_ModifyQuestMap))
            //    target = out_ModifyQuestMap;


            //if (ModifyQuestBOSS(target, out byte[] out_ModifyQuestBOSS))
            //    target = out_ModifyQuestBOSS;

            //if (ModifyQuestRewardItem(target, out byte[] out_ModifyQuestRewardItem))
            //    target = out_ModifyQuestRewardItem;


            return true;
        }

        public static bool ModifyFileOffset(byte[] src, out byte[] target)
        {
            try
            {
                target = HexHelper.CopyByteArr(src);//加载数据

                Log.HexTips(0x00, "从前4字节取出指针 定位任务信息位置");
                //从前4字节取出指针 定位任务信息位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);

                Log.HexTips(cNon0x00For2DosPtr, "清除对于2Dos来说 无意义的数据 置为0x00");
                //----Step---- 清除对于2Dos来说 无意义的数据
                target[cNon0x00For2DosPtr] = 0x00;

                //----Step---- 前移任务数据4字节 （MHF比2Dos后移了4字节，MHF：+12 2Dos： +8）
                //MHF偏移12的位置
                long QuestInfoPtr_MHFTarget = _QuestInfoPtr + cQuestMHFOffset;
                //目标2Dos偏移8的位置
                long QuestInfoPtr_2DosTarget = _QuestInfoPtr + cQuest2DosOffset;

                Log.HexInfo(QuestInfoPtr_MHFTarget, "取出原始数据");
                //取出原始数据
                byte[] temp = new byte[cQuestMhfToDosSetLenght];
                for (int i = 0; i < cQuestMhfToDosSetLenght; i++)
                {
                    temp[i] = target[QuestInfoPtr_MHFTarget + i];
                }
                Log.HexInfo(QuestInfoPtr_MHFTarget, "清理原始数据");
                //清理原始数据
                for (int i = 0; i < cQuestMhfToDosSetLenght; i++)
                {
                    target[QuestInfoPtr_MHFTarget + i] = 0x00;
                }
                Log.HexTips(QuestInfoPtr_2DosTarget, "将temp数据往前位移4字节");
                //将temp数据往前位移4字节
                for (int i = 0; i < cQuestMhfToDosSetLenght; i++)
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


        /// <summary>
        /// 迁移任务信息
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ModifyTextOffset(byte[] src, out byte[] target)
        {
            try
            {
                target = HexHelper.CopyByteArr(src);//加载数据

                Log.HexTips(0x00, "从前4字节取出指针 定位任务信息位置");
                //从前4字节取出指针 定位任务指针组起始位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);

                #region ----Step---- 前移72字节的任务信息指针组 到0x88h
                //----Step---- 前移72字节的任务信息指针组 到0x88h
                Log.HexInfo(_QuestInfoPtr, "取出原始数据");
                //取出原始数据
                byte[] temp = new byte[cQuest2DosInfoPtrGourpLenght];
                for (int i = 0; i < cQuest2DosInfoPtrGourpLenght; i++)
                    temp[i] = target[_QuestInfoPtr + i];

                Log.HexInfo(_QuestInfoPtr, "清理原始数据");
                //清理原始数据
                for (int i = 0; i < cQuest2DosInfoPtrGourpLenght; i++)
                    target[_QuestInfoPtr + i] = 0x00;

                Log.HexTips(cSetInfoPtrGourpMoveToStarPos, "将【任务信息】从{0}取出的数据，移动到{1}", _QuestInfoPtr, cSetInfoPtrGourpMoveToStarPos);
                //写入数据
                for (int i = 0; i < cQuest2DosInfoPtrGourpLenght; i++)
                {
                    target[cSetInfoPtrGourpMoveToStarPos + i] = temp[i];
                }

                temp = null;

                //----Step---- 修改原指针 到0x80h
                Log.HexTips(0x00, "将原来的指针组起始位置信息由{0}改为{1}", _QuestInfoPtr, cSetInfoPtrGourpMoveToStarPos);
                //修改原始指针
                HexHelper.ModifyIntHexToBytes(target, cSetInfoPtrGourpMoveToStarPos, 0x00, 4);

                #endregion

                Log.HexTips(0x00, "刷新指针变量值");
                //从前4字节取出指针 定位任务指针组起始位置
                _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);

                #region ----Step---- 修改任务内容指针组
                //----Step---- 修改任务内容指针组

                //从前4字节取出指针 定位任务信息位置
                int _QuestContentPtr = HexHelper.bytesToInt(target, 4, _QuestInfoPtr + 36);
                Log.HexTips(_QuestInfoPtr + 36, "读取【任务内容指针组】,指针->{0}", _QuestContentPtr);


                Log.HexInfo(_QuestContentPtr, "取出【任务内容指针组】原始数据");
                //取出原始数据
                temp = new byte[0x20];
                for (int i = 0; i < 0x20; i++)
                    temp[i] = target[_QuestContentPtr + i];

                //清理数据
                for (int i = 0; i < 0x20; i++)
                    target[_QuestContentPtr + i] = 0x00;

                Log.HexTips(cQuestContenPtrGourpMoveToStarPos, "将【任务内容指针组】从{0}取出的数据，移动到{1}", _QuestContentPtr, cQuestContenPtrGourpMoveToStarPos);
                //写入数据
                for (int i = 0; i < 0x20; i++)
                    target[cQuestContenPtrGourpMoveToStarPos + i] = temp[i];

                temp = null;

                //----Step---- 修改原指针 到0x88h
                Log.HexTips(_QuestInfoPtr + 36, "将原来的【任务内容指针组】起始位置信息由{0}改为{1}", _QuestInfoPtr + 36, cQuestContenPtrGourpMoveToStarPos);
                //修改原始指针
                HexHelper.ModifyIntHexToBytes(target, cQuestContenPtrGourpMoveToStarPos, _QuestInfoPtr + 36, 4);

                #endregion

                Log.HexTips(0x00, "刷新【任务内容指针组】指针变量值");
                //从前4字节取出指针 定位任务指针组起始位置
                _QuestContentPtr = HexHelper.bytesToInt(target, 4, _QuestInfoPtr + 36);

                #region  ----Step---- 修改任务文本内容
                //任务名称位置
                int _QuestNametPtr = HexHelper.bytesToInt(target, 4, _QuestContentPtr);
                Log.HexInfo(_QuestContentPtr, "确定【文本组】起始位置{0}", _QuestNametPtr);

                //委托说说明
                int _QuestQuestMsgPtr = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 28);

                //开始读取，直到文本结束,确定长度
                int QuestQuestMsgPtr_CurrIndex = _QuestQuestMsgPtr;
                while (true)
                {
                    if (QuestQuestMsgPtr_CurrIndex >= target.Length)
                        break;
                    if (target[QuestQuestMsgPtr_CurrIndex] == 0x00)
                        break;
                    QuestQuestMsgPtr_CurrIndex++;
                }

                //整个长度
                int QuestContenAllLenght = QuestQuestMsgPtr_CurrIndex - _QuestNametPtr;

                Log.HexInfo(_QuestContentPtr, "确定【文本组】结束位置{0}，整个长度{1}", _QuestNametPtr, QuestContenAllLenght);

                Log.HexInfo(_QuestNametPtr, "取出【任务文本】原始数据");
                //取出原始数据
                temp = new byte[QuestContenAllLenght];
                for (int i = 0; i < QuestContenAllLenght; i++)
                    temp[i] = target[_QuestNametPtr + i];

                string QuestName = HexHelper.ReadBytesToString(temp);
                Log.HexColor(ConsoleColor.Green, _QuestNametPtr, "任务文本:" + QuestName); ;


                Log.HexInfo(cQuestTextAllMsgMoveToStarPos, "清理数据旧位置【任务文本】");
                //清理数据
                for (int i = 0; i < QuestContenAllLenght; i++)
                    target[_QuestNametPtr + i] = 0xFF;//TODO 查看可视化效果

                int MoveMaxLenght = cQuestTextAllMsgMoveToEndPos - cQuestTextAllMsgMoveToStarPos - 1;

                Log.HexInfo(cQuestTextAllMsgMoveToStarPos, "写入数据【任务文本】到新位置");
                //写入数据
                for (int i = 0; i < QuestContenAllLenght && i < MoveMaxLenght; i++)
                    target[cQuestTextAllMsgMoveToStarPos + i] = temp[i];

                #endregion

                #region  ----Step---- 修正任务文本指针组，指向新文本位置

                Log.HexColor(ConsoleColor.Green,cQuestTextAllMsgMoveToStarPos, "修正【任务文本指针组】，指向新【文本组】位置8个嵌套指针");

                //文本偏移距离
                int MoveOffset = _QuestNametPtr - cQuestTextAllMsgMoveToStarPos;

                int _temp1 = HexHelper.bytesToInt(target, 4, _QuestContentPtr);
                HexHelper.ModifyIntHexToBytes(target, _temp1 - MoveOffset, _QuestContentPtr, 4);

                int _temp2 = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 4);
                HexHelper.ModifyIntHexToBytes(target, _temp2 - MoveOffset, _QuestContentPtr + 4, 4);

                int _temp3 = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 8);
                HexHelper.ModifyIntHexToBytes(target, _temp3 - MoveOffset, _QuestContentPtr + 8, 4);

                int _temp4 = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 12);
                HexHelper.ModifyIntHexToBytes(target, _temp4 - MoveOffset, _QuestContentPtr + 12, 4);

                int _temp5 = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 16);
                HexHelper.ModifyIntHexToBytes(target, _temp5 - MoveOffset, _QuestContentPtr + 16, 4);

                int _temp6 = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 20);
                HexHelper.ModifyIntHexToBytes(target, _temp6 - MoveOffset, _QuestContentPtr + 20, 4);

                int _temp7 = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 24);
                HexHelper.ModifyIntHexToBytes(target, _temp7 - MoveOffset, _QuestContentPtr + 24, 4);

                int _temp8 = HexHelper.bytesToInt(target, 4, _QuestContentPtr + 28);
                HexHelper.ModifyIntHexToBytes(target, _temp8 - MoveOffset, _QuestContentPtr + 28, 4);

                #endregion

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
                target = HexHelper.CopyByteArr(src);//加载数据

                //从前4字节取出指针 定位任务信息位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);
                Log.HexTips(0x00, "开始读取任务头部信息,指针->{0}", _QuestInfoPtr);


                //----Step---- 读取任务数据
                //任务类型
                int _QuestType = HexHelper.bytesToInt(target, cQuestInfo_Type_Lenght, _QuestInfoPtr + cQuestInfo_Type_Offset);
                Log.HexInfo(_QuestInfoPtr + cQuestInfo_Type_Offset, "任务类型->{0}", _QuestType);

                //任务星
                int _QuestStart = HexHelper.bytesToInt(target, cQuestInfo_Star_Lenght, _QuestInfoPtr + cQuestInfo_Star_Offset);
                Log.HexColor(ConsoleColor.Magenta,_QuestInfoPtr + cQuestInfo_Star_Offset, "任务星级->{0}", _QuestStart);

                int _QuestTargetMapID = HexHelper.bytesToInt(target, cQuestInfo_TargetMapID_Lenght, _QuestInfoPtr + cQuestInfo_TargetMap_Offset);
                if (_QuestTargetMapID > cMax_MapID)
                {
                    Log.HexWar(_QuestInfoPtr + cQuestInfo_TargetMap_Offset, "目的地地图,指针->{0} 超过最大 属于MHF地图", _QuestTargetMapID);
                }
                else
                {
                    Log.HexColor(ConsoleColor.Green, _QuestInfoPtr + cQuestInfo_TargetMap_Offset, "目的地地图,指针->{0} 【"+MHHelper.Get2MapName(_QuestTargetMapID)+ "】", _QuestTargetMapID);
                }

                uint _QuestID = HexHelper.bytesToUInt(target, cQuestInfo_QuestID_Lenght, _QuestInfoPtr + cQuestInfo_QuestID_Offset);
                Log.HexTips(_QuestInfoPtr + cQuestInfo_QuestID_Offset, "任务编号【{0}】", _QuestID);

                //从前4字节取出指针 定位任务信息位置
                int _QuestContentPtr = HexHelper.bytesToInt(target, 4, _QuestInfoPtr + 36);
                Log.HexTips(_QuestInfoPtr + 24, "读取任务内容指针,指针->{0}", _QuestContentPtr);

                int _QuestNametPtr = HexHelper.bytesToInt(target, 4, _QuestContentPtr);
                string QuestName = HexHelper.ReadBytesToString(src, _QuestNametPtr);
                Log.HexColor(ConsoleColor.Green,_QuestNametPtr, "任务名称:" + QuestName); ;


                //固体值
                int _GuTiValue = HexHelper.bytesToInt(target, 4, 0x48);

                if (_GuTiValue > cMax_GuTi)
                {

                    Log.HexColor(ConsoleColor.Blue, 0x48, "固体值 ->{0}", _GuTiValue);
                }
                else
                {
                    Log.HexColor(ConsoleColor.Blue, 0x48, "固体值 ->{0}", _GuTiValue);
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

        /// <summary>
        /// 报酬
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ModifyQuestRewardItem(byte[] src, out byte[] target)
        {
            try
            {
                target = HexHelper.CopyByteArr(src);//加载数据
                //任务报酬信息指针
                int _QuestRewardPtr = HexHelper.bytesToInt(target, 4, 0x0C);

                Log.HexTips(0x0C,"开始读取报酬组头部信息,指针->{0}", _QuestRewardPtr); ;

                //读取组报酬游标
                int CurrPtr = _QuestRewardPtr;
                bool isFinish = false;

                int GroupIndex = 0;
                //循环取道具组
                while (!isFinish)
                {
                    //若遇到结束符
                    if (MHHelper.CheckEnd(target, CurrPtr))
                    {
                        isFinish = true;
                        Log.HexInfo(CurrPtr,"遇报酬组头部信息结束符");
                    }
                    else
                    {
                        GroupIndex++;
                        //报酬组类型
                        int _RewardCondition = HexHelper.bytesToInt(target, 0x04, CurrPtr);
                        //报酬组指针
                        int _RewardGroupPtr = HexHelper.bytesToInt(target, 0x04, CurrPtr + 0x04);

                        Log.HexTips(CurrPtr,"第{0}报酬组，报酬类型->{1} 报酬组指针->{2}", GroupIndex, _RewardCondition, _RewardGroupPtr);

                        //取组内报酬
                        if (QuestRewardGroup(target, out byte[] target_RewardGroup, _RewardGroupPtr))
                            target = target_RewardGroup;
                        CurrPtr += 0x08;//前推游标 读取下一个报酬道具组
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);target = null;return false;
            }
        }
        /// <summary>
        /// 轮询单个报酬组的数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <param name="_RewardGroupPtr"></param>
        /// <returns></returns>
        static bool QuestRewardGroup(byte[] src, out byte[] target,int _RewardGroupPtr)
        {
            //加载数据
            target = HexHelper.CopyByteArr(src);
            //读取报酬游标
            int CurrPtr = _RewardGroupPtr;
            bool isFinish = false;
            int setCount = 0;
            while (!isFinish)
            {
                //若遇到结束符
                if (MHHelper.CheckEnd(target, CurrPtr))
                {
                    isFinish = true;
                    Log.HexInfo(CurrPtr, "遇报酬组结束符");
                }
                else
                {
                    setCount++;
                    int Pr = HexHelper.bytesToInt(target, 2, CurrPtr);//概率
                    int ItemID = HexHelper.bytesToInt(target, 2, CurrPtr + 0x02);//道具ID
                    int count = HexHelper.bytesToInt(target, 2, CurrPtr + 0x04);//数量

                    if (ItemID > cMax_ItemID)
                    {
                        Log.HexWar(CurrPtr, "第{0}个报酬道具，ID->{1}道具ID超出最大可能{2}，属于MHF道具【" + MHHelper.Get2MHFItemName(ItemID) + "】", setCount, ItemID, cMax_ItemID);
                    }
                    else
                    {
                        
                        Log.HexColor(ConsoleColor.Green,CurrPtr,"第{0}个报酬道具，道具ID->{1} 【"+ MHHelper.Get2DosItemName(ItemID) + "】 概率->{2} 数量->{3}", setCount, ItemID, Pr, count);
                    }

                    //TODO 判断道具ID是否超限
                    CurrPtr += 0x06;//前推游标
                }
            }
            return true;
        }

        public static bool ModifyQuestBOSS(byte[] src, out byte[] target)
        {
            try
            {
                target = HexHelper.CopyByteArr(src);//加载数据
                //BOSS(头部信息指针
                int _BOOSInFoPtr = HexHelper.bytesToInt(target, 4, 0x18);

                Log.HexTips(0x18, "开始读取BOSS(头部信息,指针->{0}", _BOOSInFoPtr);

                //BOSS组指针
                int _BOOSStarPtr = HexHelper.bytesToInt(target, 4, _BOOSInFoPtr + 0x08);

                Log.HexTips(_BOOSInFoPtr + 0x08, "第一个BOSS指针->{0}", _BOOSStarPtr);

                //读取BOSS组游标
                int CurrPtr = _BOOSStarPtr;
                bool isFinish = false;

                int BOSSIndex = 0;
                //循环取BOSS组
                while (!isFinish)
                {
                    //若遇到结束符或无数据
                    if (MHHelper.CheckEnd(target, CurrPtr)
                        ||
                        HexHelper.bytesToInt(target,1, CurrPtr) == 0
                        )
                    {
                        isFinish = true;
                        Log.HexInfo(CurrPtr, "遇BOSS组信息结束符或无数据");
                    }
                    else
                    {
                        BOSSIndex++;
                        //报酬组类型
                        int _BOSSID = HexHelper.bytesToInt(target, 0x04, CurrPtr);



                        if (_BOSSID > cMax_MonsterID)
                        {
                            Log.HexWar(CurrPtr, "第{0}个BOSS，ID->{1} 大于了 最大ID{2} 属于MHF怪物", BOSSIndex, _BOSSID, cMax_MonsterID);
                        }
                        else
                        {
                            Log.HexColor(ConsoleColor.Green, CurrPtr, "第{0}个BOSS，ID->{1} 【" + MHHelper.Get2BossName(_BOSSID) + "】", BOSSIndex, _BOSSID);
                        }

                        CurrPtr += 0x04;//前推游标
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); target = null; return false;
            }
        }
    }
}