//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizeManager
    /// 组织机构、部门表
    ///
    /// 修改记录
    ///
    ///		2012-05-17 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// 版本：1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012-05-17</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <returns></returns>
        public static List<BaseOrganizeEntity> GetTransferCenterByCache()
        {
            List<BaseOrganizeEntity> result = null;
            var cacheKey = "TransferCenter";
            result = CacheUtil.Cache(cacheKey, () => new BaseOrganizeManager().GetTransferCenter(), true);
            return result;
        }

        /// <summary>
        /// 获取中转站
        /// </summary>
        /// <returns></returns>
        public List<BaseOrganizeEntity> GetTransferCenter()
        {
            var result = new List<BaseOrganizeEntity>();
            var commandText = "SELECT * FROM " + BaseOrganizeEntity.TableName
                                 + " WHERE id IN (SELECT id FROM baseorganize_express WHERE is_transfer_center = 1) "
                                 + " AND enabled = 1 AND deletionstatecode = 0 ";
            // -- order by sortcode";
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                result = GetList<BaseOrganizeEntity>(dataReader);
            }
            return result;
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseOrganizeEntity entity)
        {
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldStatisticalName, entity.StatisticalName);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldWeightRatio, entity.WeightRatio);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldSendAir, entity.SendAir);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCalculateComeFee, entity.CalculateComeFee);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCalculateReceiveFee, entity.CalculateReceiveFee);

            sqlBuilder.SetValue(BaseOrganizeEntity.FieldBillBalanceSite, entity.BillBalanceSite);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldLevelTwoTransferCenter, entity.LevelTwoTransferCenter);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldProvinceSite, entity.ProvinceSite);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldBigArea, entity.BigArea);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldSendFee, entity.SendFee);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldLevelTwoTransferFee, entity.LevelTwoTransferFee);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldBillSubsidy, entity.BillSubsidy);

            sqlBuilder.SetValue(BaseOrganizeEntity.FieldMaster, entity.Master);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldMasterMobile, entity.MasterMobile);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldMasterQq, entity.MasterQq);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldManager, entity.Manager);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldManagerMobile, entity.ManagerMobile);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldManagerQq, entity.ManagerQq);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldEmergencyCall, entity.EmergencyCall);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldBusinessPhone, entity.BusinessPhone);
        }
    }
}
