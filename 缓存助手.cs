using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace espjs_gui
{
    internal class 缓存助手
    {
        public static string 缓存目录 = "./缓存";

        public static void 初始化()
        {
            if (!Directory.Exists(缓存目录))
            {
                Directory.CreateDirectory(缓存目录);
            }
        }

        public static void 设置缓存目录(string 目录)
        {
            缓存目录 = 目录;
        }
        public static void 写入缓存(string 缓存名称, string 缓存内容)
        {
            初始化();
            var 缓存文件 = 获取MD5(缓存名称);
            var 缓存文件路径 = 缓存目录 + "/" + 缓存文件;
            File.WriteAllText(缓存文件路径, 缓存内容);
        }

        public static string 读取缓存(string 缓存名称)
        {
            var 缓存文件 = 获取MD5(缓存名称);
            var 缓存文件路径 = 缓存目录 + "/" + 缓存文件;
            if (File.Exists(缓存文件路径))
            {
                return File.ReadAllText(缓存文件路径);
            }
            return "";
        }

        public static bool 缓存存在(string 缓存名称)
        {
            var 缓存文件 = 获取MD5(缓存名称);
            var 缓存文件路径 = 缓存目录 + "/" + 缓存文件;
            return File.Exists(缓存文件路径);
        }

        public static void 删除缓存(string 缓存名称)
        {
            var 缓存文件 = 获取MD5(缓存名称);
            var 缓存文件路径 = 缓存目录 + "/" + 缓存文件;
            if (File.Exists(缓存文件路径))
            {
                File.Delete(缓存文件路径);
            }
        }

        public static void 清空缓存()
        {
            var 缓存文件夹 = 缓存目录 + "/";
            if (Directory.Exists(缓存文件夹))
            {
                Directory.Delete(缓存文件夹, true);
            }
            Directory.CreateDirectory(缓存文件夹);
        }

        static string 获取MD5(string 原始字符串)
        {
            var md5 = MD5.Create();
            var 输入 = Encoding.UTF8.GetBytes(原始字符串);
            var hashBytes = md5.ComputeHash(输入);

            var 返回结果 = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                返回结果.Append(hashBytes[i].ToString("x2"));
            }
            return 返回结果.ToString();
        }

    }
}
