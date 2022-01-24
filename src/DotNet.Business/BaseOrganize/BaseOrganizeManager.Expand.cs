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
    /// BaseOrganizationManager
    /// 组织机构、部门表
    ///
    /// 修改记录
    ///
    ///		2012-05-17 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// 版本：1.0
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012-05-17</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizationManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <returns></returns>
        public static List<BaseOrganizationEntity> GetTransferCenterByCache()
        {
            List<BaseOrganizationEntity> result = null;
            var cacheKey = "TransferCenter";
            result = CacheUtil.Cache(cacheKey, () => new BaseOrganizationManager().GetTransferCenter(), true);
            return result;
        }

        /// <summary>
        /// 获取中转站
        /// </summary>
        /// <returns></returns>
        public List<BaseOrganizationEntity> GetTransferCenter()
        {
            var result = new List<BaseOrganizationEntity>();
            var commandText = "SELECT * FROM " + BaseOrganizationEntity.TableName
                                 + " WHERE id IN (SELECT id FROM baseorganize_express WHERE is_transfer_center = 1) "
                                 + " AND enabled = 1 AND deletionstatecode = 0 ";
            // -- order by sortcode";
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                result = GetList<BaseOrganizationEntity>(dataReader);
            }
            return result;
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseOrganizationEntity entity)
        {
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStatisticalName, entity.StatisticalName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldWeightRatio, entity.WeightRatio);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldSendAir, entity.SendAir);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCalculateComeFee, entity.CalculateComeFee);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCalculateReceiveFee, entity.CalculateReceiveFee);

            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBillBalanceSite, entity.BillBalanceSite);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLevelTwoTransferCenter, entity.LevelTwoTransferCenter);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldProvinceSite, entity.ProvinceSite);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBigArea, entity.BigArea);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldSendFee, entity.SendFee);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLevelTwoTransferFee, entity.LevelTwoTransferFee);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBillSubsidy, entity.BillSubsidy);

            sqlBuilder.SetValue(BaseOrganizationEntity.FieldMaster, entity.Master);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldMasterMobile, entity.MasterMobile);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldMasterQq, entity.MasterQq);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldManager, entity.Manager);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldManagerMobile, entity.ManagerMobile);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldManagerQq, entity.ManagerQq);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldEmergencyCall, entity.EmergencyCall);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBusinessPhone, entity.BusinessPhone);
        }
    }
}
