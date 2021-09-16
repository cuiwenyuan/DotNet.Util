//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// IAreaService
    /// 
    /// 修改记录
    /// 
    ///		2014.03.07 版本：1.0 JiRiGaLa 添加权限。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.03.07</date>
    /// </author> 
    /// </summary>
    public partial interface IAreaService
    {
        /// <summary>
        /// 判断字段是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名称，值</param>
        /// <param name="id">主键</param>
        /// <returns>已存在</returns>
        bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id);

        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获取按省路由大头笔信息（输入）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        DataTable GetAreaRouteMarkEdit(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 获取按省路由大头笔信息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        DataTable GetAreaRouteMarkByCache(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 刷新列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">节点主键</param>
        /// <returns>数据表</returns>
        void Refresh(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父亲节点主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        DataTable Search(BaseUserInfo userInfo, string searchKey);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseAreaEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 更新一个
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, BaseAreaEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int SetDeleted(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        int BatchSave(BaseUserInfo userInfo, DataTable dt);

        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>影响行数</returns>
        int MoveTo(BaseUserInfo userInfo, string id, string parentId);

        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>影响行数</returns>
        int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string parentId);

        /// <summary>
        /// 保存组织机构编号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="codes">编号数组</param>
        /// <returns>影响行数</returns>
        int BatchSetCode(BaseUserInfo userInfo, string[] ids, string[] codes);

        /// <summary>
        /// 保存组织机构排序顺序
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchSetSortCode(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, BaseAreaEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 按父结点获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>区域列表</returns>        
        List<BaseAreaEntity> GetListByParent(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 获取省份
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>省份列表</returns>
        List<BaseAreaEntity> GetProvinceList(BaseUserInfo userInfo);

        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="provinceId">省份主键</param>
        /// <returns>城市列表</returns>
        List<BaseAreaEntity> GetCityList(BaseUserInfo userInfo, string provinceId);

        /// <summary>
        /// 获取县区
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="cityId">城市主键</param>
        /// <returns>县区数组</returns>
        List<BaseAreaEntity> GetDistrictList(BaseUserInfo userInfo, string cityId);

        /// <summary>
        /// 获取街道
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="districtId">县区主键</param>
        /// <returns>街道列表</returns>
        List<BaseAreaEntity> GetStreetList(BaseUserInfo userInfo, string districtId);
    }
}