using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHFQuestToMH2Dos
{

    public class MapAreaData
    {
        public MapAreaData(int Count)
        {
            targetDatas = new TargetData[Count];
            areaPosDatas = new List<byte[]>();
        }
        public TargetData[] targetDatas;
        public List<byte[]> areaPosDatas;
    }

    public class TargetData
    {
        public TargetData(List<byte[]> data)
        {
            targetData = data;
        }
        public List<byte[]> targetData;
    }


    public static class LoadToSaveTemplate
    {
        public static Dictionary<int, MapAreaData> DictMapAreaData = new Dictionary<int, MapAreaData>();
        public static Dictionary<int, string> DictMapIDFileName = new Dictionary<int, string>();
        public static Dictionary<int, string> DictMapIDFullFileName = new Dictionary<int, string>();
        public static Dictionary<int, string> DictGutiName = new Dictionary<int, string>();
        public static Dictionary<int, string> DictStarName = new Dictionary<int, string>();
        public static bool LoadMapTemplateAreaData(byte[] src,string FileName,string FullFileName)
        {
            byte[] target;

            int _QuestTargetMapID;
            //地图数据
            MapAreaData mapAreaData;
            try
            {
                target = HexHelper.CopyByteArr(src);//加载数据

                //从前4字节取出指针 定位任务信息位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);
                //Log.HexTips(0x00, "开始读取任务头部信息,指针->{0}", _QuestInfoPtr);

                //任务目的地MapID
                _QuestTargetMapID = HexHelper.bytesToInt(target, ModifyQuest.cQuestInfo_TargetMapID_Lenght, _QuestInfoPtr + ModifyQuest.cQuestInfo_TargetMap_Offset);
                //Log.HexColor(ConsoleColor.Green, _QuestInfoPtr + ModifyQuest.cQuestInfo_TargetMap_Offset, "目的地地图,指针->{0} 【" + MHHelper.Get2MapName(_QuestTargetMapID) + "】", _QuestTargetMapID);

                //区域数量
                int _AreaCount = MHHelper.GetMapAreaCount(_QuestTargetMapID);
                //Log.Info(MHHelper.Get2MapName(_QuestTargetMapID) + "的地图数量" + _AreaCount);
                mapAreaData = new MapAreaData(_AreaCount);

                #region 换区设置

                //换区设置指针
                int _CAreaSetTopPtr = HexHelper.bytesToInt(target, 4, 0x1C);
                //Log.HexInfo(0x1C, "换区设置指针->{0}", _CAreaSetTopPtr);

                //读取换区单个区域游标
                int _CAreaSetTop_CurrPtr = _CAreaSetTopPtr;

                for (int i = 0; i < _AreaCount; i++)
                {
                    int _One_CurrPtr = HexHelper.bytesToInt(target, 4, _CAreaSetTop_CurrPtr);

                    if (_One_CurrPtr == 0x0)
                    {
                        //Log.HexInfo(_CAreaSetTop_CurrPtr, "区域设置"+i+"指针为0，跳过");
                        break;
                    }

                    List<byte[]> datas = new List<byte[]>();
                    int Set_TargetIndex = 0;
                    while (true)
                    {
                        if (MHHelper.CheckEnd(target, _One_CurrPtr) 
                        ||
                        HexHelper.bytesToInt(target, 1, _One_CurrPtr) == 0)
                        {
                            //Log.HexInfo(_One_CurrPtr, "区域设置结束符");
                            break;
                        }
                        //Log.HexInfo(_CAreaSetTop_CurrPtr, "第" + i + "区，第" + Set_TargetIndex + "个目标，换区设置指针->{0}", _One_CurrPtr);
                        //Log.HexTips(_One_CurrPtr, "第" + i + "区，第" + Set_TargetIndex + "个目标，读取数据,长度{0}", 0x34);
                        datas.Add(HexHelper.ReadBytes(target, 0x34, _One_CurrPtr));
                        Set_TargetIndex++;
                        _One_CurrPtr += 0x34;
                    }
                    mapAreaData.targetDatas[i] = new TargetData(datas);

                    _CAreaSetTop_CurrPtr += 0x4;
                }
                #endregion

                #region 区域映射
                //区域映射指针
                int _CAreaPosTopPtr = HexHelper.bytesToInt(target, 4, 0x20);
                //Log.HexInfo(0x20, "换区映射指针->{0}", _CAreaPosTopPtr);
                //读取单个区域映射游标
                int _CAreaPosTop_CurrPtr = _CAreaPosTopPtr;
                for (int i = 0; i < _AreaCount; i++)
                {
                    //Log.HexTips(_CAreaPosTop_CurrPtr, "第" + i + "区的区域映射，读取数据,长度{0}", 0x20);
                    mapAreaData.areaPosDatas.Add(HexHelper.ReadBytes(target, 0x20, _CAreaPosTop_CurrPtr));
                    _CAreaPosTop_CurrPtr += 0x20;
                }
                #endregion
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                target = null;
                return false;
            }

            DictMapAreaData[_QuestTargetMapID] = mapAreaData;
            DictMapIDFileName[_QuestTargetMapID] = MHHelper.Get2MapName(_QuestTargetMapID) + FileName;
            if (DictMapIDFullFileName.ContainsKey(_QuestTargetMapID))
            {
                File.Delete(DictMapIDFullFileName[_QuestTargetMapID]);
            }
            DictMapIDFullFileName[_QuestTargetMapID] = FullFileName;
            //Log.HexColor(ConsoleColor.Green, _QuestTargetMapID, "成功，缓存地图 编号{0}" + MHHelper.Get2MapName(_QuestTargetMapID) + "的数据", _QuestTargetMapID);
            return true;
        }

        public static bool LoadMaxGuti(byte[] src)
        {
            try
            {
                byte[] target = HexHelper.CopyByteArr(src);//加载数据

                //从前4字节取出指针 定位任务信息位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);
                //从前4字节取出指针 定位任务信息位置
                int _QuestContentPtr = HexHelper.bytesToInt(target, 4, _QuestInfoPtr + 36);
                int _QuestNametPtr = HexHelper.bytesToInt(target, 4, _QuestContentPtr);
                string QuestName = HexHelper.ReadBytesToString(src, _QuestNametPtr);

                //固体值
                int _GuTiValue = HexHelper.bytesToInt(target, 4, 0x48);
                DictGutiName[_GuTiValue] = QuestName;


                //任务星 尝试处理方案
                int _QuestStart = HexHelper.bytesToInt(target, 1, _QuestInfoPtr + ModifyQuest.cQuestInfo_Star_Offset);
                DictStarName[_QuestStart] = QuestName;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public static Dictionary<string, int> DictTimeTypeCount = new Dictionary<string, int>();
        public static void GetModeType(byte[] src,string FileName)
        {
            try
            {
                byte[] target = HexHelper.CopyByteArr(src);//加载数据

                //从前4字节取出指针 定位任务信息位置
                int _QuestInfoPtr = HexHelper.bytesToInt(target, 4, 0x00);
                int _TimeType = HexHelper.bytesToInt(target, 1, _QuestInfoPtr + 2);

               
                string key = FileName.Substring(FileName.Length - 1 - 5) + "_" + "(0x" + _TimeType.ToString("X") + ")";
                if(!DictTimeTypeCount.ContainsKey(key))
                {
                    DictTimeTypeCount[key] = 0;
                }
                DictTimeTypeCount[key]++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
