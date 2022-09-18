using System;
using System.IO;
using System.Xml.Serialization;

namespace DotNet.Util
{
    /// <summary>
    /// XML序列化工具
    /// </summary>
    public class XmlSerializationUtil
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public XmlSerializationUtil() { }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static object Load(Type type, string filePath)
        {
            object result = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var serializer = new XmlSerializer(type);
                result = serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return result;
        }


        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filePath">文件路径</param>
        public static void Save(object obj, string filePath)
        {
            FileStream fs = null;
            // serialize it...
            try
            {
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }

            }

        }

    }
}
