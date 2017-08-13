using Data.Globals;
using Room;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Data.XML
{
    public static class XmlPlayer
    {
        //保存玩家数据
        public static void WritePlayer(string path, PlayerData player)
        {
            CheckExists(path, player);
            path = path + @"PlayerData\" + (player.GetQQ() + "_"+player.GetExID()) + @"\";
            WriteBase(path + "BaseData.xml", player);
            for (int i = 0; i < 7; i++)
            {
                WriteEquip(path + "Equip_" + i + ".xml", player.items[i]);
            }
        }
        private static void CheckExists(string path, PlayerData player)
        {
            string file = path + "PlayerData";
            //如果不存在就创建文件夹
            if (Directory.Exists(file) == false)
            {
                Directory.CreateDirectory(file);
            }

            file = path + @"PlayerData\" + (player.GetQQ() + "_" + player.GetExID());
            if (Directory.Exists(file) == false)
            {
                Directory.CreateDirectory(file);
            }
        }
        private static void WriteBase(string path, PlayerData player)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string[,] str = new string[,]
            {
                { "名字",player.GetName() },
                { "等级",player.GetLevel() },
                { "经验值",player.GetExp() },
                { "击落数",player.GetHit() },
                { "飘币",player.GetPiaoBi()+"" },
                { "Q点",player.GetQBi()+"" },

                //战绩
                { "胜",player.GetVS_Win() },
                { "负",player.GetVS_Lose() },
                { "平",player.GetVS_Peace() },

                //使用角色
                { "角色",player.GetTypes() }
            };
            XmlDocument xmlDoc = new XmlDocument();
            //创建Xml声明部分，即<?xml version="1.0" encoding="utf-8" ?>
            XmlDeclaration Declaration = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);

            //创建根节点
            XmlNode rootNode = xmlDoc.CreateElement("Root");
            for (int i = 0; i < str.Length / 2; i++)
            {
                //创建student子节点
                XmlNode childNode = xmlDoc.CreateElement(str[i, 0]);

                //创建一个属性
                XmlAttribute value = xmlDoc.CreateAttribute("Value");
                value.Value = str[i, 1];

                //xml节点附件属性
                childNode.Attributes.Append(value);

                //附加子节点到根节点
                rootNode.AppendChild(childNode);
            }

            //附加根节点
            xmlDoc.AppendChild(rootNode);
            xmlDoc.InsertBefore(Declaration, xmlDoc.DocumentElement);

            //保存Xml文档
            xmlDoc.Save(path);
        }
        private static void WriteEquip(string path, int[] items)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var list = Global.GetXmlManager().itemPics;
            XmlDocument xmlDoc = new XmlDocument();
            //创建Xml声明部分，即<?xml version="1.0" encoding="utf-8" ?>
            XmlDeclaration Declaration = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);

            //创建根节点
            XmlNode rootNode = xmlDoc.CreateElement("Root");
            for (int i = 1; i < items.Length; i++)
            {
                //空格对齐
                string s = list[i - 1].name;
                int t = (7 - list[i - 1].name.Length);
                for (int j = 0; j < t; j++)
                {
                    s = "_" + s + "_";
                }
                //创建student子节点
                XmlNode childNode = xmlDoc.CreateElement(s);

                //创建一个属性
                XmlAttribute id = xmlDoc.CreateAttribute("ID");
                id.Value = i + "";
                XmlAttribute type = xmlDoc.CreateAttribute("Type");
                type.Value = items[i] + "";
                XmlAttribute file = xmlDoc.CreateAttribute("File");
                XmlAttribute sale = xmlDoc.CreateAttribute("Sale");
                var len = list[i - 1].itemPics.Count;
                if (0 < items[i] && items[i] <= len)
                {
                    file.Value = list[i - 1].itemPics[items[i] - 1].item[0].file;
                    var p = list[i - 1].itemPics[items[i] - 1].shopFile;
                    sale.Value = "Yes";
                    if (p == null || p == "NULL" || p == "null") sale.Value = "No ";
                }
                else
                {
                    file.Value = "Error";
                    sale.Value = "   ";
                    if (items[i] == 0)
                    {
                        file.Value = "null";
                    }
                }


                //xml节点附件属性
                childNode.Attributes.Append(id);
                childNode.Attributes.Append(type);
                childNode.Attributes.Append(sale);
                childNode.Attributes.Append(file);

                //附加子节点到根节点
                rootNode.AppendChild(childNode);
            }

            //附加根节点
            xmlDoc.AppendChild(rootNode);
            xmlDoc.InsertBefore(Declaration, xmlDoc.DocumentElement);

            //保存Xml文档
            xmlDoc.Save(path);
        }

        public static bool ReadPlayer(string path, PlayerData player)
        {
            //path为账号文件夹
            if (player == null)
            {
                return false;
            }
            if (Directory.Exists(path) == false)
            {
                return false;
            }

            //提取账号
            var strs = path.Split('\\');
            if (strs.Length < 0) return false;
            var str = strs[strs.Length - 1];
            strs = str.Split('_');
            if (strs.Length != 2) return false;
            player.QQ = strs[0];
            player.ExID = strs[1];

            //加载基本信息
            if(ReadBase(path + @"\BaseData.xml", player) == false) return false;

            //加载装备信息
            for (int i = 0; i <= 6; i++)
            {
                if (ReadEquip(path + @"\Equip_" + i + ".xml", player.items[i]) == false)
                {
                    return false;
                }
            }
            return true;
        }
        private static bool ReadBase(string path,PlayerData player)
        {
            if (File.Exists(path) == false) return false;

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            foreach (XElement item1 in root.Elements())
            {
                var name = item1.Name.LocalName;
                if (name == "名字")
                {
                    player.name = XmlManager.Try_Get_Attribute_Value_Str(item1.Attribute("Value"));
                }
                else if (name == "等级")
                {
                    player.level = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "经验值")
                {
                    player.exp = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "击落数")
                {
                    player.hit = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "飘币")
                {
                    player.piaobi = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "Q点")
                {
                    player.Qbi = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "胜")
                {
                    player.vs_win = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "负")
                {
                    player.vs_lose = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "平")
                {
                    player.vs_peace = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
                else if (name == "角色")
                {
                    player.type = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Value"));
                }
            }
            return true;
        }
        private static bool ReadEquip(string path, int[] items)
        {
            if (File.Exists(path) == false) return false;

            //将XML文件加载进来
            XDocument document = XDocument.Load(path);
            //获取到XML的根元素进行操作
            XElement root = document.Root;

            foreach (XElement item1 in root.Elements())
            {
                int id = XmlManager.Try_Get_Attribute_Value(item1.Attribute("ID"));
                if (id > 0 && id < items.Length)
                {
                    items[id] = XmlManager.Try_Get_Attribute_Value(item1.Attribute("Type"));
                }
            }
            return true;
        }
    }
}
