using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Data.Globals
{
    public static class GlobalB
    {
        //当前工作路径
        private static string RootPath = "";

        public static string GetRootPath()
        {
            if (RootPath != "")
            {
                //不为空直接返回
                return RootPath;
            }

            //获取当前工作路径
            string path = System.Environment.CurrentDirectory;

            //判断balloon资源文件夹是否存在
            if (Directory.Exists(path + @"\ballo0n"))
            {
                return path;
            }

            //再次查找
            string[] temp = path.Split('\\');
            if (temp.Length < 4)
            {
                return "";
            }

            string a = "";
            for (int i = 0; i < temp.Length - 3; i++)
            {
                if (i != 0)
                    a += "\\";
                a += temp[i];
            }
            RootPath = a;
            return a;
        }
        public static string IntToString(int x,int zeroNum = 0)
        {
            //int转string，不够补0，补0位数zeroNum
            string ret = x + "";

            int n = 10;
            for (int i = 0; i < zeroNum; i++)
            {
                if(x<n)
                {
                    ret = "0" + ret;
                }
                n *= 10;
            }

            return ret;
        }
    }
}
