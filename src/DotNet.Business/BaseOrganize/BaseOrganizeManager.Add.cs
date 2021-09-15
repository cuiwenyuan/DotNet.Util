//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizeManager
    /// 组织机构
    ///
    /// 修改记录
    /// 
    ///		2016.02.29 版本：1.1 JiRiGaLa	添加方法优化。
    ///		2016.01.28 版本：1.0 JiRiGaLa	进行独立。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.02.29</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizeManager : BaseManager //, IBaseOrganizeManager
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string Add(BaseOrganizeEntity entity)
        {
            var result = string.Empty;

            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrEmpty(entity.ParentId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizeEntity.FieldParentId, entity.ParentId));
                //父项不等于空的时候，才检查名称重复
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizeEntity.FieldFullName, entity.FullName));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizeEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizeEntity.FieldDeleted, 0));
            }
            

            if (!string.IsNullOrEmpty(entity.ParentId) && Exists(parameters))
            {
                //名称已重复
                StatusCode = Status.ErrorNameExist.ToString();
                StatusMessage = Status.ErrorNameExist.ToDescription();
            }
            else
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizeEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseOrganizeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizeEntity.FieldDeleted, 0)
                };

                if (entity.Code.Length > 0 && Exists(parameters))
                {
                    //编号已重复
                    StatusCode = Status.ErrorCodeExist.ToString();
                    StatusMessage = Status.ErrorCodeExist.ToDescription();
                }
                else
                {
                    result = AddEntity(entity);
                    //运行成功
                    StatusCode = Status.OkAdd.ToString();
                    StatusMessage = Status.OkAdd.ToDescription();
                    AfterAdd(entity);
                }
            }

            return result;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="code"></param>
        /// <param name="fullName"></param>
        /// <param name="categoryCode"></param>
        /// <param name="outerPhone"></param>
        /// <param name="innerPhone"></param>
        /// <param name="fax"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public string AddByDetail(string parentId, string code, string fullName, string categoryCode, string outerPhone, string innerPhone, string fax, bool enabled)
        {
            var entity = new BaseOrganizeEntity
            {
                ParentId = parentId,
                Code = code,
                FullName = fullName,
                CategoryCode = categoryCode,
                OuterPhone = outerPhone,
                InnerPhone = innerPhone,
                Fax = fax,
                Enabled = enabled ? 1 : 0
            };
            return Add(entity);
        }
    }
}