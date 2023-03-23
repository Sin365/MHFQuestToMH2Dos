namespace MHFQuestToMH2Dos
{
    internal class Program
    {
        static string loc = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\";

        const string InDir = "Input";
        const string OutDir = "Out";

        static void Main(string[] args)
        {
            if (!Directory.Exists(loc + InDir))
            {
                Console.WriteLine("Input文件不存在");
                Console.ReadLine();
                return;
            }

            if (!Directory.Exists(loc + OutDir))
            {
                Console.WriteLine("Out文件不存在");
                Console.ReadLine();
                return;
            }

            string[] files = FileHelper.GetDirFile(loc + InDir);

            int index= 0;
            for(int i = 0;i < files.Length;i++) 
            {
                string FileName = files[0].Substring(files[0].LastIndexOf("\\"));
                if (!FileName.ToLower().Contains(".mib") && !FileName.ToLower().Contains(".bin"))
                {
                    return;
                }
                index++;
                FileHelper.LoadFile(files[i], out byte[] data);
                ModifyQuest.ModifyFile(data, out byte[] targetdata);
                string newfileName = FileName + "_unpack";
                string outstring = loc + OutDir + "\\" + newfileName;
                FileHelper.SaveFile(outstring, targetdata);
                Console.WriteLine($"已处理第{index}个:{outstring}");
            }

            Console.WriteLine($"需处理{files.Length}个文件");
            Console.ReadLine();
        }
    }
}