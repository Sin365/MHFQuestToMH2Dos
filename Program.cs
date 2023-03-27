using System.Text;

namespace MHFQuestToMH2Dos
{
    internal class Program
    {
        static string loc = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\";

        const string InDir = "Input";
        const string OutDir = "Out";
        const string Ver = "0.2.1";

        static void Main(string[] args)
        {
            string title = $"MHFQuestToMH2Dos Ver.{Ver} By 皓月云 axibug.com";
            Console.Title = title;
            Console.WriteLine(title);

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

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 


            string[] files = FileHelper.GetDirFile(loc + InDir);
            Console.WriteLine($"共{files.Length}个文件，是否处理? (y/n)");


            string yn = Console.ReadLine();
            if (yn.ToLower() != "y")
                return;

            int index= 0;
            int errcount = 0;
            for(int i = 0;i < files.Length;i++) 
            {
                string FileName = files[i].Substring(files[i].LastIndexOf("\\"));

                if (!FileName.ToLower().Contains(".mib") && !FileName.ToLower().Contains(".bin"))
                {
                    continue;
                }
                index++;

                Console.WriteLine($">>>>>>>>>>>>>>开始处理 第{index}个文件  {FileName}<<<<<<<<<<<<<<<<<<<");
                FileHelper.LoadFile(files[i], out byte[] data);
                if (ModifyQuest.ModifyQuset(data, out byte[] targetdata))
                {
                    string newfileName = FileName + "_fix";
                    string outstring = loc + OutDir + "\\" + newfileName;
                    FileHelper.SaveFile(outstring, targetdata);
                    Console.WriteLine($">>>>>>>>>>>>>>成功处理 第{index}个:{outstring}");
                }
                else
                {
                    errcount++;
                    Console.WriteLine($">>>>>>>>>>>>>>处理失败 第{index}个: 输出到{files[i]}");
                }
            }

            Console.WriteLine($"已处理{files.Length}个文件，其中{errcount}个失败");
            Console.ReadLine();
        }
    }
}