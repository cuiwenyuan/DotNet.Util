//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// ISequenceService
    /// 序列接口
    /// 
    /// 修改记录
    /// 
    ///		2011.02.24 版本：1.1 JiRiGaLa 获取原来序列号的功能完善。
    ///		2008.10.10 版本：1.0 JiRiGaLa 创建主键。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.02.24</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseSequenceService
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseSequenceEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, BaseSequenceEntity entity, out Status status, out string statusMessage);

        /// <summary>
        /// 更新序列
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">序列实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, BaseSequenceEntity entity, out Status status, out string statusMessage);
        
        /// <summary>
        /// 获取序列号列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo);

        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <returns>序列号</returns>
        string Increment(BaseUserInfo userInfo, string fullName);

        /// <summary>
        /// 获取原序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <param name="fillZeroPrefix">是否填充补零</param>
        /// <returns>序列号</returns>
        string GetOldSequence(BaseUserInfo userInfo, string fullName, int defaultSequence, int sequenceLength, bool fillZeroPrefix);

        /// <summary>
        /// 获取新序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <param name="fillZeroPrefix">是否填充补零</param>
        /// <returns>序列号</returns>
        string GetNewSequence(BaseUserInfo userInfo, string fullName, int defaultSequence, int sequenceLength, bool fillZeroPrefix);
       
        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <param name="count">个数</param>
        /// <returns>序列号</returns>
        string[] GetBatchSequence(BaseUserInfo userInfo, string fullName, int count);
        
        /// <summary>
        /// 获取倒序序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <returns>序列号</returns>
        string GetReduction(BaseUserInfo userInfo, string fullName);

        /// <summary>
        /// 批量重置
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        int Reset(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 批量删除日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        int Delete(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 批量删除权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        int BatchDelete(BaseUserInfo userInfo, string[] ids);
    }
}