//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// IFileService
    /// 
    /// 修改记录
    /// 
    ///		2008.04.29 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.29</date>
    /// </author> 
    /// </summary>
    public partial interface IFileService
    {
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件内容</param>
        /// <param name="toUserId">发送给谁主键</param>
        /// <returns>文件主键</returns>
        string Send(BaseUserInfo userInfo, string fileName, byte[] file, string toUserId);

        /// <summary>
        /// 文件是否已存在
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <returns>是否已存在</returns>
        bool Exists(BaseUserInfo userInfo, string folderId, string fileName);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>文件</returns>
        byte[] Download(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>文件</returns>
        byte[] Downloads(BaseUserInfo userInfo, string id);
        /// <summary>
        /// 分块上传文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="filesize">文件大小</param>
        /// <param name="enabled">是否有效</param>
        /// <returns>主键</returns>
        string UploadByBlock(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, int filesize, bool enabled);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="enabled">是否有效</param>
        /// <returns>主键</returns>
        string Upload(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, bool enabled);

        /// <summary>
        /// 分段上传文件 ，2012-10-14 HJC Add 暂时支持SQL数据库 
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="Id"></param>
        /// <param name="fileName"></param>
        /// <param name="length"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        string UploadByBlockUpdate(BaseUserInfo userInfo, string Id, string fileName, int length, byte[] file);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseFileEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 按文件夹获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>数据权限</returns>
        DataTable GetDataTableByFolder(BaseUserInfo userInfo, string folderId);

        /// <summary>
        /// 按文件夹删除文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>影响的行数</returns>
        int DeleteByFolder(BaseUserInfo userInfo, string folderId);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="description"></param>
        /// <param name="enabled">是否有效</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, string description, bool enabled, out string statusCode, out string statusMessage);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="folderId">文件夹</param>
        /// <param name="fileName">文件名</param>
        /// <param name="description">备注</param>
        /// <param name="enabled">是否有效</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, string id, string folderId, string fileName, string description, bool enabled, out string statusCode, out string statusMessage);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        int UpdateFile(BaseUserInfo userInfo, string id, string fileName, byte[] file, out string statusCode, out string statusMessage);

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="newName">新名称</param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        int Rename(BaseUserInfo userInfo, string id, string newName, bool enabled, out string statusCode, out string statusMessage);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="searchKey">查询关键词</param>
        /// <returns>数据表</returns>
        DataTable Search(BaseUserInfo userInfo, string searchKey);

        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>影响行数</returns>
        int MoveTo(BaseUserInfo userInfo, string id, string folderId);

        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>影响行数</returns>
        int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string folderId);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响的行数</returns>
        int BatchSave(BaseUserInfo userInfo, DataTable dt);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        int Delete(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchDelete(BaseUserInfo userInfo, string[] ids);
    }
}
