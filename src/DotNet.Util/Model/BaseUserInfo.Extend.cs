//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseUserInfo
    /// 用户核心基础信息
    /// 
    /// 修改纪录
    /// 
    ///     2014.07.24 JiRiGaLa  版本：1.0 进行分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.07.24</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserInfo
    {
        private string _targetUserId = string.Empty;
        /// <summary>
        /// 目标用户
        /// </summary>
        public virtual string TargetUserId
        {
            get => _targetUserId;
            set => _targetUserId = value;
        }

        private string _subCompanyId = null;
        /// <summary>
        /// 分支机构主键
        /// </summary>
        public virtual string SubCompanyId
        {
            get => _subCompanyId;
            set => _subCompanyId = value;
        }

        private string _subCompanyCode = string.Empty;
        /// <summary>
        /// 分支机构编号
        /// </summary>
        public virtual string SubCompanyCode
        {
            get => _subCompanyCode;
            set => _subCompanyCode = value;
        }

        private string _subCompanyName = string.Empty;
        /// <summary>
        /// 分支机构名称
        /// </summary>
        public virtual string SubCompanyName
        {
            get => _subCompanyName;
            set => _subCompanyName = value;
        }

        private string _subDepartmentId = null;
        /// <summary>
        /// 当前的组织结构子部门主键
        /// </summary>
        public virtual string SubDepartmentId
        {
            get => _subDepartmentId;
            set => _subDepartmentId = value;
        }

        private string _subDepartmentCode = string.Empty;
        /// <summary>
        /// 当前的组织结构子部门编号
        /// </summary>
        public virtual string SubDepartmentCode
        {
            get => _subDepartmentCode;
            set => _subDepartmentCode = value;
        }

        private string _subDepartmentName = string.Empty;
        /// <summary>
        /// 当前的组织结构子部门名称
        /// </summary>
        public virtual string SubDepartmentName
        {
            get => _subDepartmentName;
            set => _subDepartmentName = value;
        }

        private string _workgroupId = null;
        /// <summary>
        /// 当前的组织结构工作组主键
        /// </summary>
        public virtual string WorkgroupId
        {
            get => _workgroupId;
            set => _workgroupId = value;
        }

        private string _workgroupCode = string.Empty;
        /// <summary>
        /// 当前的组织结构工作组编号
        /// </summary>
        public virtual string WorkgroupCode
        {
            get => _workgroupCode;
            set => _workgroupCode = value;
        }

        private string _workgroupName = string.Empty;
        /// <summary>
        /// 当前的组织结构工作组名称
        /// </summary>
        public virtual string WorkgroupName
        {
            get => _workgroupName;
            set => _workgroupName = value;
        }

        private int _securityLevel = 0;
        /// <summary>
        /// 安全级别
        /// </summary>
        public virtual int SecurityLevel
        {
            get => _securityLevel;
            set => _securityLevel = value;
        }

        private string _currentLanguage = string.Empty;
        /// <summary>
        /// 当前语言选择
        /// </summary>
        public virtual string CurrentLanguage
        {
            get => _currentLanguage;
            set => _currentLanguage = value;
        }

        private string _themes = string.Empty;
        /// <summary>
        /// 当前布局风格选择
        /// </summary>
        public virtual string Themes
        {
            get => _themes;
            set => _themes = value;
        }
    }
}