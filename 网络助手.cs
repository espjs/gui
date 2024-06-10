using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace espjs_gui
{
    internal class 网络助手
    {
        public static async Task<string> 获取网页源代码(string 网页地址)
        {
            var 客户端 = new HttpClient();
            var 输出 = await 客户端.GetAsync(网页地址);
            var 返回值 = "";
            if (输出.IsSuccessStatusCode)
            {
                返回值 = await 输出.Content.ReadAsStringAsync();
            }
            return 返回值;
        }

        public static async Task<string> 获取模块源代码(string 模块)
        {
            if (缓存助手.缓存存在(模块))
            {
                return 缓存助手.读取缓存(模块);
            }
            var 模块地址 = 模块;
            var 模块代码 = "";
            if (模块.StartsWith("http:") || 模块.StartsWith("https:"))
            {
                模块地址 = 模块;
                模块代码 = await 获取网页源代码(模块);
            }
            else
            {
                var 默认配置 = 配置.加载配置();
                模块地址 = 默认配置.Modules.Replace("[name]", 模块);

            }
            模块代码 = await 获取网页源代码(模块地址);
            缓存助手.写入缓存(模块, 模块代码);
            return 模块代码;

        }

    }
}
