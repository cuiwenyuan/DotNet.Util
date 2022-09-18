//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2022, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// DbOption
    /// 有关数据库选项
    /// 
    /// 修改记录
    /// 
    ///		2022.09.13 版本：1.0 Troy Cui 新增
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.09.13</date>
    /// </author> 
    /// </summary>
    /// <summary>
    /// 数据库选项
    /// </summary>
    public class DbOption
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public string DbId { get; set; }

        /// <summary>
        /// 当前数据库类型
        /// </summary>
        public CurrentDbType CurrentDbType { get; set; }

        /// <summary>
        /// 提供者名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 工厂类型装配合格名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

    }
}