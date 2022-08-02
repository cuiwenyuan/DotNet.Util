//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public int Update(BaseUserEntity entity, bool checkUserExist = false, bool checkCodeExist = false)
        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="entity">用户实体</param>
        /// <param name="checkUserExist">检查用户名是否存在</param>
        /// <param name="checkCodeExist">检查编号是否存在</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserEntity entity, bool checkUserExist = false, bool checkCodeExist = false)
        {
            var result = 0;

            // 检查用户名是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, entity.UserName),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, entity.CompanyId)
            };
            if (checkUserExist && Exists(parameters, entity.Id))
            {
                // 用户名已重复
                StatusCode = Status.ErrorUserExist.ToString();
            }
            else
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, entity.CompanyId)
                };

                if (checkCodeExist && !string.IsNullOrEmpty(entity.Code)
                    && entity.Code.Length > 0
                    && Exists(parameters, entity.Id))
                {
                    // 编号已重复
                    StatusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    if (string.IsNullOrEmpty(entity.QuickQuery))
                    {
                        // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                        entity.QuickQuery = StringUtil.GetPinyin(entity.RealName).ToLower();
                    }
                    if (string.IsNullOrEmpty(entity.SimpleSpelling))
                    {
                        // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                        entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.RealName).ToLower();
                    }

                    // 获取原始实体信息
                    var entityOld = GetEntity(entity.Id);
                    if (entityOld != null)
                    {
                        // 保存修改记录
                        SaveEntityChangeLog(entity, entityOld);

                        // 01：先更新自己的数据
                        result = UpdateEntity(entity);
                        // 02：用户修改时，用户文件夹同步更新
                        // BaseFolderManager manager = new BaseFolderManager(this.DbHelper, this.UserInfo);
                        // manager.SetProperty(new KeyValuePair<string, object>(BaseFolderEntity.FieldFolderName, userEntity.RealName), new KeyValuePair<string, object>(BaseFolderEntity.FieldId, userEntity.Id));

                        if (result == 0)
                        {
                            AfterUpdate(entity);
                            Status = Status.OkUpdate;
                            StatusCode = Status.OkUpdate.ToString();                            
                        }
                        else
                        {
                            Status = Status.ErrorDeleted;
                            StatusCode = Status.ErrorDeleted.ToString();
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region SaveEntityChangeLog
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="entityNew">修改后的实体对象</param>
        /// <param name="entityOld">修改前的实体对象</param>
        /// <param name="tableName">表名称</param>
        /// <returns>影响行数</returns>
        public int SaveEntityChangeLog(BaseUserEntity entityNew, BaseUserEntity entityOld, string tableName = null)
        {
            var result = 0;

            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseUserEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(entityOld, null));
                var newValue = Convert.ToString(property.GetValue(entityNew, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                // 不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var entity = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = typeof(BaseUserEntity).FieldDescription("CurrentTableName"),
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    RecordKey = entityOld.Id.ToString(),
                    NewValue = newValue,
                    OldValue = oldValue
                };
                manager.Add(entity, true, false);
                result++;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 设置对象，若不存在就增加，有存在就更新
        /// </summary>
        /// <param name="userInfo">实体</param>
        /// <param name="userPassword">用户密码</param>
        /// <returns>更新、添加成功？</returns>
        public bool SetEntity(BaseUserInfo userInfo, string userPassword)
        {
            var result = false;

            var userEntity = GetEntity(userInfo.Id);
            if (userEntity == null)
            {
                userEntity = new BaseUserEntity
                {
                    Id = userInfo.UserId,
                    Enabled = 1,
                    SortCode = userInfo.UserId
                };
            }
            // 2015-07-14 吉日嘎拉 用户若没有排序码就会引起序列等问题，为了避免这样的问题，直接给他赋予排序码。
            if (userEntity.SortCode == 0)
            {
                userEntity.SortCode = userEntity.Id;
            }
            userEntity.NickName = userInfo.NickName;
            userEntity.UserName = userInfo.UserName;
            userEntity.Code = userInfo.Code;
            userEntity.RealName = userInfo.RealName;
            userEntity.CompanyId = userInfo.CompanyId.ToInt();
            userEntity.CompanyName = userInfo.CompanyName;
            userEntity.DepartmentId = userInfo.DepartmentId.ToInt();
            userEntity.DepartmentName = userInfo.DepartmentName;
            userEntity.Enabled = 1;
            // userEntity.ManagerAuditDate = DateTime.Now;
            // userEntity.SubCompanyId = result.SubCompanyId;
            // userEntity.SubCompanyName = result.SubCompanyName;
            // userEntity.SubDepartmentId = result.SubDepartmentId;
            // userEntity.SubDepartmentName = result.SubDepartmentName;
            // userEntity.WorkgroupId = result.WorkgroupId;
            // userEntity.WorkgroupName = result.WorkgroupName;
            // 若有主键就是先更新，没主键就是添加
            if (!string.IsNullOrEmpty(userEntity.Id.ToString()))
            {
                result = UpdateEntity(userEntity) > 0;
                // 若不存在，就是添加的意思
                if (!result)
                {
                    // 更新不成功表示没数据，需要添加数据，这时候要注意主键不能出错
                    result = !string.IsNullOrEmpty(AddEntity(userEntity));
                }
            }
            else
            {
                // 若没有主键就是添加数据
                result = !string.IsNullOrEmpty(AddEntity(userEntity));
            }
            SetPassword(userInfo.UserId, userPassword, true, true, false);

            /*
            BaseUserLogonManager userLogonManager = new BaseUserLogonManager(this.DbHelper, this.UserInfo);
            BaseUserLogonEntity userLogonEntity = userLogonManager.GetEntity(userInfo.Id);
            if (userLogonEntity == null)
            {
                userLogonEntity = new BaseUserLogonEntity();
                userLogonEntity.Id = userInfo.Id;
            }
            userLogonEntity.OpenId = userInfo.OpenId;
            userLogonEntity.IPAddress = userInfo.IPAddress;
            userLogonEntity.MACAddress = userInfo.MACAddress;
            if (!string.IsNullOrEmpty(userLogonEntity.Id))
            {
                result = userLogonManager.Update(userLogonEntity) > 0;
                // 若不存在，就是添加的意思
                if (!result)
                {
                    // 更新不成功表示没数据，需要添加数据，这时候要注意主键不能出错
                    result = !string.IsNullOrEmpty(userLogonManager.Add(userLogonEntity));
                }
            }
            else
            {
                // 若没有主键就是添加数据
                result = !string.IsNullOrEmpty(userLogonManager.Add(userLogonEntity));
            }
            */

            return result;
        }
    }
}