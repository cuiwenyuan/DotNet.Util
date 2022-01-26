//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	Constraint
    /// 条件表达式运算
    /// 
    /// 修改记录
    /// 
    ///		2011.05.18 版本：1.0	JiRiGaLa 创建。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.05.18</date>
    /// </author> 
    /// </summary>
    public class ConstraintUtil
    {
        /// <summary>
        /// 参数参考
        /// </summary>
        public static string ParameterReference = @"当前用户主键：用户主键（CurrentUserId）
当前用户编号：用户编号（CurrentUserCode）
当前用户名  ：用户名（CurrentUserName）
当前用户姓名：用户姓名（CurrentRealName）
所在公司主键：公司主键（CurrentCompanyId）
所在公司名称：公司名称（CurrentCompanyName）
所在公司编号：公司编号（CurrentCompanyCode）
所在部门主键：部门主键（CurrentDepartmentId）
所在部门名称：部门名称（CurrentDepartmentName）
所在部门编号：部门编号（CurrentDepartmentCode）
所在工作组主键：工作组主键（CurrentWorkgroupId）
所在工作组名称：工作组名称（CurrentWorkgroupName）
所在工作组编号：工作组编号（CurrentWorkgroupCode）";

        /// <summary>
        /// 解析替换约束表达式标准函数
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        /// <param name="constraint">约束表达式</param>
        /// <returns>约束表达式</returns>
        public static string PrepareParameter(BaseUserInfo userInfo, string constraint)
        {
            constraint = constraint.Replace("用户主键", userInfo.Id.ToString());
            constraint = constraint.Replace("CurrentUserId", userInfo.Id.ToString());
            constraint = constraint.Replace("用户编号", userInfo.Code);
            constraint = constraint.Replace("CurrentUserCode", userInfo.Code);
            constraint = constraint.Replace("用户名", userInfo.UserName);
            constraint = constraint.Replace("CurrentUserName", userInfo.UserName);
            constraint = constraint.Replace("用户姓名", userInfo.RealName);
            constraint = constraint.Replace("CurrentRealName", userInfo.RealName);

            constraint = constraint.Replace("公司主键", userInfo.CompanyId.ToString());
            constraint = constraint.Replace("CurrentCompanyId", userInfo.CompanyId.ToString());

            constraint = constraint.Replace("部门主键", userInfo.DepartmentId.ToString());
            constraint = constraint.Replace("CurrentDepartmentId", userInfo.DepartmentId.ToString());

            // constraint = constraint.Replace("工作组主键", result.WorkgroupId);
            // constraint = constraint.Replace("CurrentWorkgroupId", result.WorkgroupId);
            
            return constraint;
        }

        /// <summary>
        /// 解析替换约束表达式标准函数
        /// </summary>
        /// <param name="constraint">约束表达式</param>
        /// <returns>约束表达式</returns>
        public static string PrepareParameter(string constraint)
        {
            return PrepareParameter(BaseSystemInfo.UserInfo, constraint);
        }
   }
}