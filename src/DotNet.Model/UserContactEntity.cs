using System;

namespace DotNet.Model
{
    using Util;

    /// <remarks>
    /// UserContactEntity
    /// 用户联系信息
    /// 
    /// 修改记录
    /// 
    ///	版本：1.0 2020.02.02    Troy.Cui    创建。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2020.02.02</date>
    /// </author> 
    /// </remarks>
    public partial class UserContactEntity
    {
        /// <summary>
        /// 用户联系人实体
        /// </summary>
        public UserContactEntity()
        {
        }
        /// <summary>
        /// 用户联系人实体
        /// </summary>
        /// <param name="userContact"></param>
        public UserContactEntity(BaseUserContactEntity userContact)
        {
            if (userContact != null)
            {
                BaseUtil.CopyObjectProperties(userContact, this);
            }

        }
        /// <summary>
        /// 用户联系人实体
        /// </summary>
        /// <param name="userContactEntity"></param>
        /// <param name="userEntity"></param>
        public UserContactEntity(BaseUserContactEntity userContactEntity, BaseUserEntity userEntity)
        {
            if (userContactEntity != null)
            {
                BaseUtil.CopyObjectProperties(userContactEntity, this);
            }
            if (userEntity != null)
            {
                BaseUtil.CopyObjectProperties(userEntity, this);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;
        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        public string CompanyId { get; set; } = null;
        /// <summary>
        /// 手机
        /// </summary>
        [FieldDescription("手机")]
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 手机验证通过
        /// </summary>
        [FieldDescription("手机验证通过")]
        public int MobileValiated { get; set; } = 0;

        /// <summary>
        /// 手机验证日期
        /// </summary>
        [FieldDescription("手机验证日期")]
        public DateTime? MobileVerificationDate { get; set; } = null;

        /// <summary>
        /// 显示手机号码
        /// </summary>
        [FieldDescription("显示手机号码")]
        public int ShowMobile { get; set; } = 1;

        /// <summary>
        /// 短号
        /// </summary>
        [FieldDescription("短号")]
        public string ShortNumber { get; set; } = string.Empty;

        /// <summary>
        /// 旺旺号码
        /// </summary>
        [FieldDescription("旺旺号码")]
        public string Ww { get; set; } = string.Empty;

        /// <summary>
        /// 微信号码
        /// </summary>
        [FieldDescription("微信号码")]
        public string WeChat { get; set; } = string.Empty;

        /// <summary>
        /// 微信识别码
        /// </summary>
        [FieldDescription("微信识别码")]
        public string WeChatOpenId { get; set; } = string.Empty;

        /// <summary>
        /// 微信号码验证通过
        /// </summary>
        [FieldDescription("微信号码验证通过")]
        public int WeChatValiated { get; set; } = 0;

        /// <summary>
        /// 易信号码
        /// </summary>
        [FieldDescription("易信号码")]
        public string YiXin { get; set; } = string.Empty;

        /// <summary>
        /// 易信号码验证通过
        /// </summary>
        [FieldDescription("易信号码验证通过")]
        public int YiXinValiated { get; set; } = 0;

        /// <summary>
        /// 电话号码
        /// </summary>
        [FieldDescription("电话号码")]
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// 分机号码
        /// </summary>
        [FieldDescription("分机号码")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// QQ号码
        /// </summary>
        [FieldDescription("QQ号码")]
        public string Qq { get; set; } = null;

        /// <summary>
        /// 电子邮件
        /// </summary>
        [FieldDescription("电子邮件")]
        public string Email { get; set; } = null;

        /// <summary>
        /// 电子邮箱验证通过
        /// </summary>
        [FieldDescription("电子邮箱验证通过")]
        public int EmailValiated { get; set; } = 0;

        /// <summary>
        /// 公司邮件
        /// </summary>
        [FieldDescription("电子邮件")]
        public string CompanyMail { get; set; } = null;

        /// <summary>
        /// YY
        /// </summary>
        [FieldDescription("YY")]
        public string Yy { get; set; } = null;

        /// <summary>
        /// 紧急联系
        /// </summary>
        [FieldDescription("紧急联系")]
        public string EmergencyContact { get; set; } = null;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间", false)]
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        [FieldDescription("创建人用户编号", false)]
        public string CreateUserId { get; set; } = null;

        /// <summary>
        /// 创建人
        /// </summary>
        [FieldDescription("创建人", false)]
        public string CreateBy { get; set; } = null;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间", false)]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        [FieldDescription("修改人用户编号", false)]
        public string ModifiedUserId { get; set; } = null;

        /// <summary>
        /// 修改人
        /// </summary>
        [FieldDescription("修改人", false)]
        public string ModifiedBy { get; set; } = null;

        /// <summary>
        /// 真实姓名
        /// </summary>
        [FieldDescription("真实姓名", false)]
        public string RealName { get; set; }

        /// <summary>
        /// 编号（可扩展为工号、供应商编码、客户编码等）
        /// </summary>
        [FieldDescription("编号", false)]
        public string Code { get; set; }

        /// <summary>
        /// 生日（格式2020-01-01）
        /// </summary>
        [FieldDescription("生日", false)]
        public string Birthday { get; set; } = string.Empty;

        /// <summary>
        /// 性别（男、女）
        /// </summary>
        [FieldDescription("性别", false)]
        public string Gender { get; set; } = string.Empty;
    }
}
