using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace espjs_gui
{
    internal class 配置
    {

        public string? Version { get; set; }
        public string? Modules { get; set; }
        public int BaudRate { get; set; }
        public Dictionary<string, string>? Flash { get; set; }
        public ArrayList? Ignore { get; set; }

        public static 配置 加载配置()
        {
            string 当前执行文件 = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string 当前目录 = Directory.GetParent(当前执行文件).FullName;
            string 配置内容 = File.ReadAllText(当前目录 + @"\配置.json");
            var 用户配置 = JsonConvert.DeserializeObject<配置>(配置内容);
            if (用户配置 == null)
            {
                return new 配置();
            }
            return 用户配置;
        }

        public static bool 检测配置文件是否存在()
        {
            string 当前执行文件 = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string 当前目录 = Directory.GetParent(当前执行文件).FullName;
            return File.Exists(当前目录 + @"\config.json");
        }

        public int 提取波特率(string 开发板类型)
        {
            string 参数 = this.Flash[开发板类型];
            string 波特率 = new Regex(@".*--baud\s+(\d+).*").Replace(参数, "$1");
            return Convert.ToInt32(波特率);
        }
    }
}
