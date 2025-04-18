﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
#if NET452_OR_GREATER
#else
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
#endif

namespace DotNet.Util
{
    /// <summary>
    /// XML工具
    /// </summary>
    public static class XmlUtil
    {
        #region 增、删、改操作==============================================

        /// <summary>
        /// 追加节点
        /// </summary>
        /// <param name="filePath">XML文档绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="xmlNode">XmlNode节点</param>
        /// <returns></returns>
        public static bool AppendChild(string filePath, string xPath, XmlNode xmlNode)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER

                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    var xn = doc.SelectSingleNode(xPath);
                    var n = doc.ImportNode(xmlNode, true);
                    xn?.AppendChild(n);
                    doc.Save(filePath);
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 从XML文档中读取节点追加到另一个XML文档中
        /// </summary>
        /// <param name="filePath">需要读取的XML文档绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="toFilePath">被追加节点的XML文档绝对路径</param>
        /// <param name="toXPath">范例: @"Skill/First/SkillItem"</param>
        /// <returns></returns>
        public static bool AppendChild(string filePath, string xPath, string toFilePath, string toXPath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(toFilePath);
                    var xn = doc.SelectSingleNode(toXPath);

                    var xnList = ReadNodes(filePath, xPath);
                    if (xnList != null)
                    {
                        foreach (XmlElement xe in xnList)
                        {
                            var n = doc.ImportNode(xe, true);
                            if (xn != null) xn.AppendChild(n);
                        }
                        doc.Save(toFilePath);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 修改节点的InnerText的值
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="value">节点的值</param>
        /// <returns></returns>
        public static bool UpdateNodeInnerText(string filePath, string xPath, string value)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    //IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    var xn = doc.SelectSingleNode(xPath);
                    var xe = (XmlElement)xn;
                    if (xe != null) xe.InnerText = value;
                    doc.Save(filePath);
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 读取XML文档
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <returns></returns>
        public static XmlDocument LoadXmlDoc(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    return doc;
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                    return null;
                }
            }
            return null;
        }
        #endregion 增、删、改操作

        #region 扩展方法===================================================
        /// <summary>
        /// 读取XML的所有子节点
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <returns></returns>
        public static XmlNodeList ReadNodes(string filePath, string xPath)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);
                var xn = doc.SelectSingleNode(xPath);
                if (xn != null)
                {
                    var xnList = xn.ChildNodes;  //得到该节点的子节点
                    return xnList;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return null;
            }
        }

        #endregion 扩展方法

        #region Troy扩展，参考http://www.cnblogs.com/wangchuang/p/3152687.html

        #region 读取模版
        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="filePath">模版文件路径</param>
        /// <returns>模版内容</returns>
        public static string GetTemplate(string filePath)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                    //result = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    result += "The template file is not found: " + filePath;
                }
            }
            return result;
        }
        #endregion

        #region 创建XML文件

        /// <summary>
        /// 写入主键
        /// </summary>
        /// <param name="filePath">文件名及路径</param>
        /// <param name="fileContent">文件内容</param>
        /// <param name="templatePath">模板文件路径</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <returns>成功</returns>
        public static bool CreateXmlFile(string filePath, string fileContent, string templatePath, bool overwrite = true)
        {
            var result = overwrite;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }

                if (File.Exists(filePath))
                {
                    if (!overwrite)
                    {
                        return result;
                    }
                }
                else
                {
                    var path = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
                using var sw = new StreamWriter(filePath, false, Encoding.UTF8);
                if (string.IsNullOrEmpty(fileContent))
                {
                    fileContent = GetTemplate(templatePath);
                }
                sw.WriteLine(fileContent);
            }
            return result;
        }
        #endregion

        #region public static string Read(string filePath, string node, string attribute, string nameSpace = "")

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XmlUtil.Read(path, "/Node", "")
         * XmlUtil.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string Read(string filePath, string node, string attribute, string nameSpace = "")
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    var xn = doc.SelectSingleNode(node);
                    if (!string.IsNullOrEmpty(nameSpace))
                    {
                        var xmlnam = new XmlNamespaceManager(doc.NameTable);
                        xmlnam.AddNamespace("a", nameSpace);
                        node = node.Replace("/", "/a:");
                        xn = doc.SelectSingleNode(node, xmlnam);
                    }
                    if (xn != null)
                    {
                        if (xn.Attributes != null)
                        {
                            value = (string.IsNullOrEmpty(attribute) ? xn.InnerText : xn.Attributes[attribute].Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            }
            return value;
        }

        #endregion

        #region public static void Insert(string filePath, string node, string element, string attribute, string value, string nameSpace = "")
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlUtil.Insert(path, "/Node", "Element", "", "Value")
         * XmlUtil.Insert(path, "/Node", "Element", "Attribute", "Value")
         * XmlUtil.Insert(path, "/Node", "", "Attribute", "Value")
         ************************************************/
        public static void Insert(string filePath, string node, string element, string attribute, string value, string nameSpace = "")
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    var xn = doc.SelectSingleNode(node);
                    if (!string.IsNullOrEmpty(nameSpace))
                    {
                        var xmlnam = new XmlNamespaceManager(doc.NameTable);
                        xmlnam.AddNamespace("a", nameSpace);
                        node = node.Replace("/", "/a:");
                        xn = doc.SelectSingleNode(node, xmlnam);
                    }

                    if (string.IsNullOrEmpty(element))
                    {
                        if (!string.IsNullOrEmpty(attribute))
                        {
                            var xe = (XmlElement)xn;
                            xe?.SetAttribute(attribute, value);
                        }
                    }
                    else
                    {
                        var xe = doc.CreateElement(element);
                        if (string.IsNullOrEmpty(attribute))
                        {
                            xe.InnerText = value;

                        }
                        else
                        {
                            xe.SetAttribute(attribute, value);

                        }
                        if (xn != null) xn.AppendChild(xe);
                    }
                    //doc.Save(filePath);//空白标签会产生多余换行符
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.IndentChars = "\t";
                    settings.OmitXmlDeclaration = false;
                    using (var xmlWriter = XmlWriter.Create(filePath, settings))
                    {
                        doc.Save(xmlWriter);
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            }
        }

        #endregion

        #region public static void Update(string filePath, string node, string attribute, string value, string nameSpace = "")

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlUtil.Insert(path, "/Node", "", "Value")
         * XmlUtil.Insert(path, "/Node", "Attribute", "Value")
         ************************************************/
        public static void Update(string filePath, string node, string attribute, string value, string nameSpace = "")
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    var xn = doc.SelectSingleNode(node);
                    if (!string.IsNullOrEmpty(nameSpace))
                    {
                        var xmlnam = new XmlNamespaceManager(doc.NameTable);
                        xmlnam.AddNamespace("a", nameSpace);
                        node = node.Replace("/", "/a:");
                        xn = doc.SelectSingleNode(node, xmlnam);
                    }

                    var xe = (XmlElement)xn;
                    if (xe != null)
                    {
                        if (string.IsNullOrEmpty(attribute))
                        {
                            xe.InnerText = value;
                        }
                        else
                        {
                            xe.SetAttribute(attribute, value);
                        }
                        //doc.Save(filePath);//空白标签会产生多余换行符
                        var settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.ConformanceLevel = ConformanceLevel.Auto;
                        settings.IndentChars = "\t";
                        settings.OmitXmlDeclaration = false;
                        using (var xmlWriter = XmlWriter.Create(filePath, settings))
                        {
                            doc.Save(xmlWriter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            }
        }

        #endregion

        #region public static void Delete(string filePath, string node, string attribute, string nameSpace = "")

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlUtil.Delete(path, "/Node", "")
         * XmlUtil.Delete(path, "/Node", "Attribute")
         ************************************************/
        public static void Delete(string filePath, string node, string attribute, string nameSpace = "")
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    var xn = doc.SelectSingleNode(node);
                    if (!string.IsNullOrEmpty(nameSpace))
                    {
                        var xmlnam = new XmlNamespaceManager(doc.NameTable);
                        xmlnam.AddNamespace("a", nameSpace);
                        node = node.Replace("/", "/a:");
                        xn = doc.SelectSingleNode(node, xmlnam);
                    }
                    var xe = (XmlElement)xn;
                    if (xn != null)
                    {
                        if (string.IsNullOrEmpty(attribute))
                        {
                            xn.ParentNode?.RemoveChild(xn);
                        }
                        else
                        {
                            xe.RemoveAttribute(attribute);
                        }
                        doc.Save(filePath);
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            }
        }

        #endregion

        #region public static void BatchUpdateNodeValue(string filePath, List<KeyValuePair<string, string>> parameters, string nameSpace = "")
        /// <summary>
        /// 批量修改节点值
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="parameters">节点,值</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlUtil.BatchUpdateNodeValue(path, parameters, nameSpace)
         ************************************************/
        public static void BatchUpdateNodeValue(string filePath, List<KeyValuePair<string, string>> parameters, string nameSpace = "")
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!filePath.Contains(@":\") && filePath.Contains(@"/"))
                {
#if NET452_OR_GREATER
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
#else
                    IFileProvider fileProvider = Microsoft.Extensions.Configuration.ConfigurationBuilder.GetFileProvider();
                    filePath = fileProvider.GetFileInfo(filePath).PhysicalPath;
#endif
                }
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            if (!string.IsNullOrEmpty(parameter.Key) && !string.IsNullOrEmpty(parameter.Value))
                            {
                                var node = parameter.Key;
                                var value = parameter.Value;
                                var xn = doc.SelectSingleNode(node);
                                if (!string.IsNullOrEmpty(nameSpace))
                                {
                                    var xmlnam = new XmlNamespaceManager(doc.NameTable);
                                    xmlnam.AddNamespace("a", nameSpace);
                                    node = node.Replace("/", "/a:");
                                    xn = doc.SelectSingleNode(node, xmlnam);
                                }
                                var xe = (XmlElement)xn;
                                if (xe != null)
                                {
                                    xe.InnerText = value;
                                }
                            }
                        }
                    }
                    //最后一次性保存
                    //doc.Save(filePath);//空白标签会产生多余换行符
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.IndentChars = "\t";
                    settings.OmitXmlDeclaration = false;
                    settings.Encoding = Encoding.UTF8;
                    using (var xmlWriter = XmlWriter.Create(filePath, settings))
                    {
                        doc.Save(xmlWriter);
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            }
        }
        #endregion

        #endregion
    }
}
