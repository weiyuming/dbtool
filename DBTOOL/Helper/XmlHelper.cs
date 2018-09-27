using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using DBTOOL.Models;

namespace DBTOOL.Helper
{
    class XmlHelper
    {

        /// <summary>
        /// 获取所有要处理的表
        /// </summary>
        /// <returns></returns>
        public static List<Table> GetTable()
        {
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释
            XmlReader reader = XmlReader.Create(@"Table.xml", settings);
            doc.Load(reader);
            reader.Close();

            //获得跟节点  user
            XmlNode xn = doc.SelectSingleNode("tables");

            //获取子节点
            XmlNodeList xnl = xn.ChildNodes;

            List<Table> list = new List<Table>();
            //循环节点
            foreach (XmlNode xn1 in xnl)
            {
                Table table = new Table();
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)xn1;
                // 得到table节点的所有子节点
                XmlNodeList xnl0 = xe.ChildNodes;
                table.TableName = xnl0.Item(0).InnerText;
                table.TableMessage = xnl0.Item(1).InnerText;
                list.Add(table);
            }

            return list;

        }


        /// <summary>
        /// 获取路径
        /// </summary>
        /// <returns></returns>
        public static List<SettingVo> GetSetting()
        {
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释
            XmlReader reader = XmlReader.Create(@"setting.xml", settings);
            doc.Load(reader);
            reader.Close();

            //获得跟节点  user
            XmlNode xn = doc.SelectSingleNode("settings");

            //获取子节点
            XmlNodeList xnl = xn.ChildNodes;

            List<SettingVo> list = new List<SettingVo>();
            //循环节点
            foreach (XmlNode xn1 in xnl)
            {
                SettingVo vo = new SettingVo();
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)xn1;
                // 得到table节点的所有子节点
                XmlNodeList xnl0 = xe.ChildNodes;
                vo.TableSavePath = xnl0.Item(0).InnerText;
                vo.UserName = xnl0.Item(1).InnerText;
                vo.Hour = xnl0.Item(2).InnerText;
                vo.Min = xnl0.Item(3).InnerText;
                vo.MaxRow = Convert.ToInt32(xnl0.Item(4).InnerText);
                list.Add(vo);
            }

            return list;

        }

    }
}
