//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseAreaProvinceMarkManager
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2015-07-03 版本：1.1 JiRiGaLa 代码重新构造。
    ///		2015-06-23 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015-07-03</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaProvinceMarkManager : BaseManager, IBaseManager
    {
        #region public DataTable GetAreaRouteMarkEdit(string parentId)
        /// <summary>
        /// 获取按省路由大头笔信息（输入）
        /// </summary>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        public DataTable GetAreaRouteMarkEdit(string parentId)
        {
            DataTable result = null;

            var commandText = @"SELECT basearea_provincemark.Id
                                    , BaseArea.Id as AreaId
                                    , BaseArea.SORTCODE
                                    , BaseArea.Fullname as AREAID
                                    , BaseArea.Fullname as AREANAME
                                    , basearea_provincemark.MARK
                                    , basearea_provincemark.DESCRIPTION
                                    , basearea_provincemark.createon
                                    , basearea_provincemark.createby
                                    , basearea_provincemark.modifiedon
                                    , basearea_provincemark.modifiedby
 FROM (SELECT id, code, fullname, SORTCODE FROM basearea
                                WHERE basearea.layer = 4
                                AND basearea.enabled = 1) basearea LEFT OUTER JOIN                     
                      (SELECT * FROM 
                      basearea_provincemark 
                      WHERE   basearea_provincemark.areaid = " + parentId + @" 
                      ) basearea_provincemark
                      ON BaseArea.Id = basearea_provincemark.provinceid   
                      ORDER BY basearea.code ";

            result = Fill(commandText);

            return result;
        }
        #endregion

        /// <summary>
        /// 获取清单
        /// </summary>
        /// <returns></returns>
        public List<BaseAreaProvinceMarkEntity> GetList()
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaProvinceMarkEntity.FieldEnabled, 1)
            };
            return GetList<BaseAreaProvinceMarkEntity>(parameters);
        }

        /// <summary>
        /// 获取清单
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<BaseAreaProvinceMarkEntity> GetListByArea(int areaId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaProvinceMarkEntity.FieldAreaId, areaId),
                new KeyValuePair<string, object>(BaseAreaProvinceMarkEntity.FieldEnabled, 1)
            };
            return GetList<BaseAreaProvinceMarkEntity>(parameters);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public List<BaseAreaProvinceMarkEntity> SearchByList(string searchKey)
        {
            List<BaseAreaProvinceMarkEntity> result = null;

            var sql = "SELECT * "
                    + " FROM " + CurrentTableName
                    + "  WHERE " + BaseAreaProvinceMarkEntity.FieldEnabled + " = 1 ";

            var dbParameters = new List<IDbDataParameter>();

            searchKey = searchKey.Trim();
            if (!string.IsNullOrEmpty(searchKey))
            {
                // 六、这里是进行支持多种数据库的参数化查询
                sql += string.Format(" AND ({0} LIKE {1} ", BaseAreaProvinceMarkEntity.FieldProvince, DbHelper.GetParameter(BaseAreaProvinceMarkEntity.FieldProvince));
                sql += string.Format(" OR {0} LIKE {1} ", BaseAreaProvinceMarkEntity.FieldArea, DbHelper.GetParameter(BaseAreaProvinceMarkEntity.FieldArea));
                sql += string.Format(" OR {0} LIKE {1} ", BaseAreaProvinceMarkEntity.FieldMark, DbHelper.GetParameter(BaseAreaProvinceMarkEntity.FieldMark));
                sql += string.Format(" OR {0} LIKE {1}) ", BaseAreaProvinceMarkEntity.FieldDescription, DbHelper.GetParameter(BaseAreaProvinceMarkEntity.FieldDescription));

                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = string.Format("%{0}%", searchKey);
                }

                dbParameters.Add(DbHelper.MakeParameter(BaseAreaProvinceMarkEntity.FieldProvince, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaProvinceMarkEntity.FieldArea, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaProvinceMarkEntity.FieldMark, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaProvinceMarkEntity.FieldDescription, searchKey));
            }
            sql += " ORDER BY " + BaseAreaProvinceMarkEntity.FieldProvince;

            using (var dataReader = DbHelper.ExecuteReader(sql, dbParameters.ToArray()))
            {
                result = GetList<BaseAreaProvinceMarkEntity>(dataReader);
            }

            return result;
        }
    }
}
