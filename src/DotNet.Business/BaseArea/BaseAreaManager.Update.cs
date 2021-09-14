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
    /// BaseAreaManager 
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2015.07.02 版本：1.0 JiRiGaLa  修改记录独立化。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.07.02</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaManager : BaseManager
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public int Update(BaseAreaEntity entity, out string statusCode)
        {
            var result = 0;
            // 检查是否已被其他人修改            
            //if (DbUtil.IsModifed(DbHelper, BaseOrganizeEntity.TableName, entity.Id, entity.ModifiedUserId, entity.ModifiedOn))
            //{
            //    // 数据已经被修改
            //    statusCode = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{

            var parameters = new List<KeyValuePair<string, object>>();
            if (entity.ParentId != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, entity.ParentId));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldFullName, entity.FullName));
            parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0));
            parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1));

            if (Exists(parameters, entity.Id))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                // 检查编号是否重复
                parameters = new List<KeyValuePair<string, object>>();
                if (entity.ParentId != null)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, entity.ParentId));
                }
                parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldCode, entity.Code));
                parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0));
                parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1));

                if (entity.Code.Length > 0 && Exists(parameters, entity.Id))
                {
                    // 编号已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    if (string.IsNullOrEmpty(entity.QuickQuery))
                    {
                        // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                        entity.QuickQuery = StringUtil.GetPinyin(entity.FullName).ToLower();
                    }
                    if (string.IsNullOrEmpty(entity.SimpleSpelling))
                    {
                        // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                        entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.FullName).ToLower();
                    }

                    // 获取原始实体信息
                    var entityOld = GetObject(entity.Id);
                    // 保存修改记录
                    UpdateEntityLog(entity, entityOld);

                    result = UpdateObject(entity);
                    if (result == 1)
                    {
                        statusCode = Status.OkUpdate.ToString();
                    }
                    else
                    {
                        statusCode = Status.ErrorDeleted.ToString();
                    }
                }
            }
            //}
            return result;
        }

        #region public void UpdateEntityLog(BaseAreaEntity newEntity, BaseAreaEntity oldEntity)
        /// <summary>
        /// 实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void UpdateEntityLog(BaseAreaEntity newEntity, BaseAreaEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseModifyRecordEntity.TableName;
            }
            var manager = new BaseModifyRecordManager(UserInfo, tableName);
            foreach (var property in typeof(BaseAreaEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(oldEntity, null));
                var newValue = Convert.ToString(property.GetValue(newEntity, null));
                //不记录创建人、修改人、没有修改的记录
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var record = new BaseModifyRecordEntity
                {
                    ColumnCode = property.Name.ToUpper(),
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,
                    TableCode = CurrentTableName.ToUpper(),
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseAreaEntity), "TableName"),
                    RecordKey = oldEntity.Id,
                    IpAddress = Utils.GetIp()
                };
                manager.Add(record, true, false);
            }
        }
        #endregion
    }
}