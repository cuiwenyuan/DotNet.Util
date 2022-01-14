//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    ///	BaseUtil
    /// 通用基类
    /// 
    /// 这个类可是修改了很多次啊，已经比较经典了，随着专业的提升，人也会不断提高，技术也会越来越精湛。
    /// 
    /// 修改记录
    ///
    ///     2021-04-15 版本：4.0 Troy.Cui 移到ObjectUtil类中
    ///		2012-05-07 版本：3.7 Serwif 改进ObjectsToList(CurrentDbType DbType ,object ids) 字段值数组转换为字符串列表时，增加末尾去掉逗号功能CutLastDot(string input)
    ///		2009.09.08 版本：4.4	JiRiGaLa 改进 GetPermissionScope(string[] organizationIds)。
    ///		2008.08.29 版本：4.3	JiRiGaLa 改进 DataTableToString 的 null值处理技术。
    ///		2007.11.08 版本：4.2	JiRiGaLa 改进 DataTableToStringList 为 FieldToList。
    ///		2007.11.05 版本：4.1	JiRiGaLa 改进 GetDS、GetDataTable 功能，整体思路又上一个台阶，基类的又一次飞跃。
    ///		2007.11.05 版本：4.0	JiRiGaLa 改进 支持不是“Id”为字段的主键表。
    ///		2007.11.01 版本：3.9	JiRiGaLa 改进 BUOperatorInfo 去掉这个变量，可以提高性能，提高速度，基类的又一次飞跃。
    ///		2007.09.13 版本：3.8	JiRiGaLa 改进 BaseUtil.SQLLogicConditional 错误。
    ///		2007.08.14 版本：3.7	JiRiGaLa 改进 WebService 模式下 DataSet 传输数据的速度问题。
    ///		2007.07.20 版本：3.6	JiRiGaLa 改进 DataSet 修改为 DataTable 应该能提高一些速度吧。
    ///		2007.05.20 版本：3.6	JiRiGaLa 改进 GetList() 方法整理，这次又应该是一次升华，质的飞跃很不容易啊，身心都有提高了。
    ///		2007.05.20 版本：3.4	JiRiGaLa 改进 Exists() 方法整理。
    ///		2007.05.13 版本：3.3	JiRiGaLa 改进 GetProperty()，SetProperty()，Delete() 方法整理。
    ///		2007.05.10 版本：3.2	JiRiGaLa 改进 GetList() 方法整理。
    ///		2007.04.10 版本：3.1	JiRiGaLa 添加 GetNextId，GetPreviousId 方法整理。
    ///		2007.03.03 版本：3.0	JiRiGaLa 进行了一次彻底的优化工作，方法的位置及功能整理。
    ///		2007.03.01 版本：2.0	JiRiGaLa 重新调整主键的规范化。
    ///		2006.02.05 版本：1.1	JiRiGaLa 重新调整主键的规范化。
    ///		2005.12.30 版本：1.0	JiRiGaLa 数据库连接方式都进行改进
    ///		2005.09.04 版本：1.0	JiRiGaLa 执行数据库脚本
    ///		2005.08.19 版本：1.0	整理一下编排	
    ///		2005.07.10 版本：1.0	修改了程序，格式以及理念都有些提高，应该是一次大突破
    ///		2004.11.12 版本：1.0	添加了最新的GetParent、GetChildren、GetParentChildren 方法
    ///		2004.07.21 版本：1.0	UpdateRecord、Delete、SetProperty、GetProperty、ExecuteNonQuery、GetRecord 方法进行改进。
    ///								还删除一些重复的主键，提取了最优化的方法，有时候写的主键真的是垃圾，可能自己也没有注意时就写出了垃圾。
    ///								GetRepeat、GetDayOfWeek、ExecuteProcedure、GetFromProcedure 方法进行改进，基本上把所有的方法都重新写了一遍。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2009.09.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUtil
    {
        /// <summary>
        /// 当发布评论时
        /// </summary>
        /// <param name="categoryId">类别主键</param>
        /// <param name="id">选择的主键</param>
        /// <returns>是否成功</returns>
        public delegate bool OnCommnetEventHandler(string categoryId, string id);

        /// <summary>
        /// 检查转移
        /// </summary>
        /// <param name="selectedId">选择的主键</param>
        /// <returns>是否成功</returns>
        public delegate bool CheckMoveEventHandler(string selectedId);

        /// <summary>
        /// 选择主键发生变化
        /// </summary>
        /// <param name="selectedId">选择的主键</param>
        public delegate void SelectedIndexChangedEventHandler(string selectedId);

        /// <summary>
        /// 将字符类型主键转换为数值类型主键
        /// </summary>
        /// <param name="ids">字符类型主键</param>
        /// <returns>数值型主键数组</returns>
        public static int[] GetIntKeys(string[] ids)
        {
            var keys = new List<int>();
            foreach (var key in ids)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    keys.Add(int.Parse(key));
                }
            }
            return keys.ToArray();
        }
        /// <summary>
        /// 获取审核状态
        /// </summary>
        /// <param name="auditStatus"></param>
        /// <returns></returns>
        public static string GetAuditStatus(AuditStatus auditStatus)
        {
            return GetAuditStatus(auditStatus.ToString());
        }
        /// <summary>
        /// 获取审核状态
        /// </summary>
        /// <param name="auditStatus"></param>
        /// <returns></returns>
        public static string GetAuditStatus(int auditStatus)
        {
            var result = auditStatus.ToString();
            var enumStatus = AuditStatus.Draft;
            var auditStatusType = typeof(AuditStatus);
            foreach (int auditStatusValue in Enum.GetValues(auditStatusType))
            {
                if (auditStatusValue == auditStatus)
                {
                    enumStatus = (AuditStatus)auditStatus;
                    break;
                }
            }
            result = enumStatus.ToDescription();
            return result;
        }
        /// <summary>
        /// 获取审核状态
        /// </summary>
        /// <param name="auditStatus"></param>
        /// <returns></returns>
        public static string GetAuditStatus(string auditStatus)
        {
            var result = auditStatus;
            var enumStatus = AuditStatus.Draft;
            var auditStatusType = typeof(AuditStatus);
            foreach (var auditStatusName in Enum.GetNames(auditStatusType))
            {
                if (auditStatusName.Equals(auditStatus))
                {
                    enumStatus = (AuditStatus)Enum.Parse(auditStatusType, auditStatus, true);
                    break;
                }
            }
            result = enumStatus.ToDescription();

            // result = "草稿";
            //if (AuditStatus.Draft.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.Draft.ToDescription();
            //}
            //else if (AuditStatus.StartAudit.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.StartAudit.ToDescription();
            //}
            //else if (AuditStatus.WaitForAudit.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.WaitForAudit.ToDescription();
            //}
            //else if (AuditStatus.AuditPass.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.AuditPass.ToDescription();
            //}
            //else if (AuditStatus.AuditReject.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.AuditReject.ToDescription();
            //}
            //else if (AuditStatus.Transmit.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.Transmit.ToDescription();
            //}
            //else if (AuditStatus.AuditComplete.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.AuditComplete.ToDescription();
            //}
            //else if (AuditStatus.AuditQuash.ToString().Equals(auditStatus))
            //{
            //    result = AuditStatus.AuditQuash.ToDescription();
            //}
            return result;
        }

        /// <summary>
        /// 判断是否关键字
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>是关键字</returns>
        public static bool IsKeywords(string field)
        {
            var result = false;
            // 首字母进行强制大写改进
            field = field.Substring(0, 1).ToUpper() + field.Substring(1);

            var keywords = new string[] { "Id", "SortCode", "DeletionStateCode", "Enabled", "CreateOn", "CreateUserId", "CreateBy", "ModifiedOn", "ModifiedUserId", "ModifiedBy" };

            foreach (var t in keywords)
            {
                // 防止大小写问题发生
                if (t.ToUpper().Equals(field.ToUpper()))
                {
                    // 进行大小写转换
                    field = t;
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 过滤表中的字段
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="columns">过滤字段</param>
        /// <returns>过滤后的结果</returns>
        public static DataTable SetColumnsFilter(DataTable dt, string[] columns)
        {
            for (var i = dt.Columns.Count - 1; i > 0; i--)
            {
                // 不是关键字的才过滤，全过滤了，没法用了
                if (!IsKeywords(dt.Columns[i].ColumnName))
                {
                    // 都统一转为大写，比较省事一些，有必要时再改
                    if (!StringUtil.Exists(columns, dt.Columns[i].ColumnName))
                    {
                        dt.Columns.RemoveAt(i);
                    }
                }
            }
            return dt;
        }

        #region public static bool IsAuthorized(DataTable dt, string permissionCode) 是否有相应的权限
        /// <summary>
        /// 是否有相应的权限
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>是否有权限</returns>
        public static bool IsAuthorized(DataTable dt, string permissionCode)
        {
            return Exists(dt, FieldCode, permissionCode);
        }
        #endregion

        #region  public static bool Exists(DataTable dt, string fieldName, string fieldValue) 从内存判断是否有相应的权限
        /// <summary>
        /// 从内存判断是否有相应的权限
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public static bool Exists(DataTable dt, string fieldName, string fieldValue)
        {
            var result = false;
            if (dt == null)
            {
                return false;
            }
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[fieldName].ToString().Equals(fieldValue, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        #endregion

        #region  public static PermissionOrganizeScope GetPermissionScope(string[] organizationIds) 获取权限范围的设置
        /// <summary>
        /// 获取权限范围的设置
        /// </summary>
        /// <param name="organizationIds">有权限的组织机构</param>
        /// <returns>权限范围</returns>
        public static PermissionOrganizeScope GetPermissionScope(string[] organizationIds)
        {
            var result = PermissionOrganizeScope.NotAllowed;
            //foreach (PermissionOrganizeScope permissionScope in (PermissionOrganizeScope[])Enum.GetValues(typeof(PermissionOrganizeScope)))
            //{
            //    if (StringUtil.Exists(organizationIds, permissionScope.ToString()))
            //    {
            //        result = permissionScope;
            //        break;
            //    }
            //}
            #region BUG修复
            foreach (var permissionScope in (PermissionOrganizeScope[])Enum.GetValues(typeof(PermissionOrganizeScope)))
            {
                var scope = Convert.ToInt32(permissionScope);
                if (StringUtil.Exists(organizationIds, scope.ToString()))
                {
                    result = permissionScope;
                    break;
                }
            }
            #endregion
            return result;
        }
        #endregion

        //
        // WebService 传递参数的专用方法
        //

        #region private static int SetClassValue(object sourceObject, string field, object targetObject) 设置对象的属性
        /// <summary>
        /// 设置对象的属性
        /// </summary>
        /// <param name="sourceObject">目标对象</param>
        /// <param name="field">属性名称</param>
        /// <param name="targetValue">目标值</param>
        /// <returns>影响的属性个数</returns>
        private static int SetClassValue(object sourceObject, string field, object targetValue)
        {
            var result = 0;
            var currentType = sourceObject.GetType();
            var fieldInfo = currentType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo currentFieldInfo;
            for (var i = 0; i < fieldInfo.Length; i++)
            {
                if (field.Equals(fieldInfo[i].Name))
                {
                    currentFieldInfo = currentType.GetField(field);
                    currentFieldInfo.SetValue(sourceObject, targetValue);
                    // FieldInfo[i].SetValue(TargetObject, value);
                    result++;
                    break;
                }
            }
            return result;
        }
        #endregion

        #region public static object CopyObjectValue(object sourceObject, object targetObject) 复制类对象的对应的值
        /// <summary>
        /// 复制类对象的对应的值
        /// </summary>
        /// <param name="sourceObject">当前对象</param>
        /// <param name="targetObject">目标对象</param>
        /// <returns>对象</returns>
        public static object CopyObjectValue(object sourceObject, object targetObject)
        {
            var result = 0;
            var name = string.Empty;
            var type = sourceObject.GetType();
            var fieldInfo = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo currentFieldInfo;
            foreach (var t in fieldInfo)
            {
                name = t.Name;
                currentFieldInfo = t;
                result = SetClassValue(targetObject, name, currentFieldInfo.GetValue(sourceObject));
            }
            return targetObject;
        }
        #endregion

        #region public static object CopyObjectProperties(object sourceObject, object targetObject)
        /// <summary>
        /// 复制属性
        /// </summary>
        /// <param name="sourceObject">类</param>
        /// <param name="targetObject">目标类</param>
        /// <returns>类</returns>
        public static object CopyObjectProperties(object sourceObject, object targetObject)
        {
            var typeSource = sourceObject.GetType();
            var typeTarget = targetObject.GetType();
            var propertyInfoSource = typeSource.GetProperties();
            var propertyInfoTarget = typeTarget.GetProperties();
            foreach (var t in propertyInfoTarget)
            {
                foreach (var s in propertyInfoSource)
                {
                    if (!s.CanRead)
                    {
                        continue;
                    }
                    if (t.Name.Equals(s.Name))
                    {
                        if (t.CanWrite)
                        {
                            var pValue = s.GetValue(sourceObject, null);
                            t.SetValue(targetObject, pValue, null);
                        }
                        break;
                    }
                }
            }
            return targetObject;
        }
        #endregion

        /// <summary>
        /// 写入开始调试信息
        /// </summary>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        public static int StartDebug(MethodBase methodBase)
        {
            // 输出访问日志
            // 写入调试信息
#if (DEBUG)
            Console.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + " :Begin: " + methodBase.ReflectedType.Name + "." + methodBase.Name);
            // Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + " :Begin: " + methodBase.ReflectedType.Name + "." + methodBase.Name);
#endif

            return Environment.TickCount;
        }
        /// <summary>
        /// 开始调试
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        public static int StartDebug(BaseUserInfo userInfo, MethodBase methodBase)
        {
            // 写入访问日志
            // userInfo.OperationId = MethodBase.Name;
            // userInfo.Description = MethodBase.ReflectedType.Name;
            // 输出访问日志
#if (DEBUG)
            if (userInfo != null)
            {
                // Console.WriteLine("Company: " + userInfo.CompanyName + "Department: " + userInfo.DepartmentName + "User: " + userInfo.RealName + " IP: " + userInfo.IPAddress);
                // Console.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + " :Begin: " + methodBase.ReflectedType.Name + "." + methodBase.Name);
                // Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + " :Begin: " + methodBase.ReflectedType.Name + "." + methodBase.Name);
            }
#endif

            return Environment.TickCount;
        }

        /// <summary>
        /// 写入结束调试信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="methodBase"></param>
        /// <param name="milliStart"></param>
        /// <param name="consoleColor"></param>
        /// <returns></returns>
        public static int EndDebug(BaseUserInfo userInfo, MethodBase methodBase, int milliStart, ConsoleColor consoleColor)
        {
            var milliEnd = Environment.TickCount;

#if (DEBUG)

            if (userInfo != null)
            {
                Console.WriteLine(userInfo.IpAddress + " " + userInfo.CompanyName + ":" + userInfo.DepartmentName + ":" + userInfo.RealName);
            }

            Console.Write(DateTime.Now.ToString("MM-dd HH:mm:ss") + " : "
                    + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString()
                    + ":");
            Console.ForegroundColor = consoleColor;
            Console.Write(methodBase.ReflectedType.Name + "." + methodBase.Name);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);

            // Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) 
            //    + " : " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() 
            //    + " : " + methodBase.ReflectedType.Name + "." + methodBase.Name);
#endif

            return milliEnd - milliStart;
        }

        /// <summary>
        /// 写入结束调试信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="methodBase"></param>
        /// <param name="milliStart"></param>
        /// <returns></returns>
        public static int EndDebug(BaseUserInfo userInfo, MethodBase methodBase, int milliStart)
        {
            return EndDebug(userInfo, methodBase, milliStart, ConsoleColor.White);
        }

        /// <summary>
        /// 写入调试信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="methodBase"></param>
        public static void WriteDebug(BaseUserInfo userInfo, MethodBase methodBase)
        {
#if (DEBUG)
            Console.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + " " + userInfo.IpAddress + methodBase.ReflectedType.Name + "." + methodBase.Name);
#endif
        }
    }
}