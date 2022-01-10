//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    ///	AppMessage
    /// 通用讯息控制基类
    /// 
    /// 修改记录
    ///		2007.05.17 版本：1.0	JiRiGaLa 建立，为了提高效率分开建立了类。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.05.17</date>
    /// </author> 
    /// </summary>
    public partial class AppMessage
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        public static string Msg0000 = "提示信息";

        /// <summary>
        /// 发生未知错误。
        /// </summary>
        public static string Msg0001 = "发生未知错误。";

        /// <summary>
        /// 数据库连接不正常。
        /// </summary>
        public static string Msg0002 = "数据库连接不正常。";

        /// <summary>
        /// WebService连接不正常。
        /// </summary>
        public static string Msg0003 = "WebService连接不正常。";

        /// <summary>
        /// 无任何数据被修改。
        /// </summary>
        public static string Msg0004 = "无任何数据被修改。";

        /// <summary>
        /// 记录未找到，可能已被其他人删除。
        /// </summary>
        public static string Msg0005 = "记录未找到，请检查网络连接，也可能数据已被其他人删除，请联系系统管理员核实。";

        /// <summary>
        /// 数据已被其他人修改，请按F5键重新更新取得数据。
        /// </summary>
        public static string Msg0006 = "数据已被其他人修改，请按F5键重新更新取得数据。";

        /// <summary>
        /// 请输入{0}，不允许为空。
        /// </summary>
        public static string Msg0007 = "请输入{0}，不允许为空。";

        /// <summary>
        /// {0} 已重复。
        /// </summary>
        public static string Msg0008 = "{0}已重复。";

        /// <summary>
        /// 新增成功。
        /// </summary>
        public static string Msg0009 = "新增成功。";

        /// <summary>
        /// 更新成功。
        /// </summary>
        public static string Msg0010 = "更新成功。";

        /// <summary>
        /// 保存成功。
        /// </summary>
        public static string Msg0011 = "保存成功。";

        /// <summary>
        /// 批量储存成功。
        /// </summary>
        public static string Msg0012 = "批量保存成功。";

        /// <summary>
        /// 删除成功。
        /// </summary>
        public static string Msg0013 = "删除成功。";

        /// <summary>
        /// 批量删除成功。
        /// </summary>
        public static string Msg0014 = "批量删除成功。";

        /// <summary>
        /// 您确认删除吗？
        /// </summary>
        public static string Msg0015 = "您确认删除吗？";

        /// <summary>
        /// 您确认删除 '{0}' 吗？
        /// </summary>
        public static string Msg0016 = "您确认删除{0}吗？";

        /// <summary>
        /// 当前记录不允许被删除。
        /// </summary>
        public static string Msg0017 = "当前记录不允许被删除。";

        /// <summary>
        /// 当前记录 '{0}' 不允许被删除。
        /// </summary>
        public static string Msg0018 = "当前记录{0}不允许被删除。";

        /// <summary>
        /// 当前记录不允许被编辑,请按F5键重新获得最新资料。
        /// </summary>
        public static string Msg0019 = "当前记录不允许被编辑，请按F5键重新获得最新资料。";

        /// <summary>
        /// 当前记录 '{0}' 不允许被编辑，请按F5键重新获得最新资料。
        /// </summary>
        public static string Msg0020 = "当前记录{0}不允许被编辑，请按F5键重新获得最新资料。";

        /// <summary>
        /// 当前记录已是第一条记录。
        /// </summary>
        public static string Msg0021 = "当前记录已是第一条记录。";

        /// <summary>
        /// 当前记录已是最后一条记录。
        /// </summary>
        public static string Msg0022 = "当前记录已是最后一条记录。";

        /// <summary>
        /// 没有要复制的数据！
        /// </summary>
        public static string Msg0246 = "没有要复制的数据！";

        /// <summary>
        /// 请至少选择一项。
        /// </summary>
        public static string Msgc023 = "请至少选择一项。";

        /// <summary>
        /// 只能选择一条数据。
        /// </summary>
        public static string Msgc024 = "只能选择一条数据。";

        /// <summary>
        /// 请至少选择一项 '{0}'。
        /// </summary>
        public static string Msg0024 = "请至少选择一项{0}。";

        /// <summary>
        /// '{0}' 不能大于 '{1}'。
        /// </summary>
        public static string Msg0025 = "{0}不能大于{1}。";

        /// <summary>
        /// '{0}' 不能小于 '{1}'。
        /// </summary>
        public static string Msg0026 = "{0}不能小于{1}。";

        /// <summary>
        /// '{0}' 不能等于 '{1}'。
        /// </summary>
        public static string Msg0027 = "{0}不能等于 {1}。";

        /// <summary>
        ///'{0}' 不是有效的日期。
        /// </summary>
        public static string Msg0028 = "{0}不是有效的日期。";

        /// <summary>
        /// '{0}' 不是有效的字符。
        /// </summary>
        public static string Msg0029 = "{0}不是有效的字符。";

        /// <summary>
        /// '{0}' 不是有效的数字。
        /// </summary>
        public static string Msg0030 = "{0}不是有效的数字。";

        /// <summary>
        /// '{0}' 不是有效的金额。
        /// </summary>
        public static string Msg0031 = "{0}不是有效的金额。";

        /// <summary>
        /// '{0}'名不能包含
        /// </summary>
        // ：\ / : * ? " < > |
        public static string Msg0032 = "{0}名包含非法字符。";

        /// <summary>
        /// 数据已经被引用，有关联资料在。
        /// </summary>
        public static string Msg0033 = "资料已经被引用，有关联资料在。";

        /// <summary>
        /// 数据已经被引用，有关联资料在，是否强制删除资料？
        /// </summary>
        public static string Msg0034 = "资料已经被引用，有关联资料在，是否强制删除资料？";

        /// <summary>
        /// {0} 有子节点不允许被删除，有子节点还未被删除。
        /// </summary>
        public static string Msg0035 = "{0}有子节点不允许被删除，有子节点还未被删除。";

        /// <summary>
        /// {0} 不能移动到 {1}。
        /// </summary>
        public static string Msg0036 = "{0}不能移动到 {1}。";

        /// <summary>
        /// {0} 下的子节点不能移动到 {1}。
        /// </summary>
        public static string Msg0037 = "{0}下的子节点不能移动到{1}。";

        /// <summary>
        /// 确认移动 {0} 到 {1} 吗？
        /// </summary>
        public static string Msg0038 = "确认移动{0}到{1}吗？";

        /// <summary>
        /// '{0}' 不等于 '{1}'。
        /// </summary>
        public static string Msg0039 = "{0}不等于{1}。";

        /// <summary>
        /// {0} 错误。
        /// </summary>
        public static string Msg0040 = "{0}错误。";

        /// <summary>
        /// 确认审核通过吗？
        /// </summary>
        public static string Msg0041 = "确认审核通过吗？";

        /// <summary>
        /// 确认审核退回吗？
        /// </summary>
        public static string Msg0042 = "确认审核退回吗？";

        /// <summary>
        /// 不能锁定数据。
        /// </summary>
        public static string Msg0043 = "不能锁定数据。";

        /// <summary>
        /// 成功锁定数据。
        /// </summary>
        public static string Msg0044 = "成功锁定数据。";

        /// <summary>
        /// 数据已经改变，想储存数据吗？
        /// </summary>
        public static string Msg0045 = "数据已经改变，想储存数据吗？";

        /// <summary>
        /// 最近 {0} 次内密码不能重复。。
        /// </summary>
        public static string Msg0046 = "最近{0}次内密码不能重复。。";

        /// <summary>
        /// 密码已过期，账号被锁定，请联系系统管理员。
        /// </summary>
        public static string Msg0047 = "密码已过期，账号被锁定，请联系系统管理员。";

        /// <summary>
        /// 拒绝登入，用户已经在在线。
        /// </summary>
        public static string Msg0048 = "拒绝登录，用户已经在线。";

        /// <summary>
        /// 拒绝登入，网卡Mac地址不符合限制条件。
        /// </summary>
        public static string Msg0049 = "拒绝登录，网卡Mac地址不符合限制条件。";

        /// <summary>
        /// 拒绝登入，IP地址不符限制条件。
        /// </summary>
        public static string Msg0050 = "拒绝登录，IP:{0}被限制访问,请联系系统管理人员。";

        /// <summary>
        /// 已到在线用户最大数量限制。
        /// </summary>
        public static string Msg0051 = "已到在线用户最大数量限制。";

        /// <summary>
        /// IP地址格式不正确。
        /// </summary>
        public static string Msg0052 = "IP地址格式不正确。";

        /// <summary>
        /// MAC地址格式不正确。
        /// </summary>
        public static string Msg0053 = "MAC地址格式不正确。";

        /// <summary>
        /// 请填写IP地址或MAC地址信息。
        /// </summary>
        public static string Msg0054 = "请填写IP地址或MAC地址信息。";

        /// <summary>
        /// 存在相同的IP地址。
        /// </summary>
        public static string Msg0055 = "存在相同的IP地址。";

        /// <summary>
        /// IP地址新增成功。
        /// </summary>
        public static string Msg0056 = "IP地址新增成功。";

        /// <summary>
        /// IP地址新增失败。
        /// </summary>
        public static string Msg0057 = "IP地址新增失败。";

        /// <summary>
        /// 存在相同的MAC地址。
        /// </summary>
        public static string Msg0058 = "存在相同的MAC地址。";

        /// <summary>
        /// MAC地址新增成功。
        /// </summary>
        public static string Msg0059 = "MAC地址新增成功。";

        /// <summary>
        /// 请先新增该职员的登入系统用户信息。
        /// </summary>
        public static string Msg0060 = "请先新增该职员的登入系统用户信息。";

        /// <summary>
        /// MAC地址新增失败。
        /// </summary>
        public static string Msg0061 = "MAC地址新增失败。";

        /// <summary>
        /// 请设定新密码，原始密码未曾修改过。
        /// </summary>
        public static string Msg0062 = "请设定新密码，原始密码未曾修改过。";

        /// <summary>
        /// 请设定新密码，30天内未曾修改过密码。
        /// </summary>
        public static string Msg0063 = "请设定新密码，30天内未曾修改过密码。";

        /// <summary>
        /// 您输入的分钟数值不正确，请检查。
        /// </summary>
        public static string Msg0064 = "您输入的分钟数值不正确，请检查。";

        /// <summary>
        /// 数据已经改变，不储存数据？
        /// </summary>
        public static string Msg0065 = "数据已经改变，不储存数据？";

        /// <summary>
        /// 请设定新密码，系统要求修改密码。
        /// </summary>
        public static string Msg0066 = "请设定新密码，系统要求修改密码。";

        /// <summary>
        /// 您确认移除吗？
        /// </summary>
        public static string Msg0075 = "您确认移除吗？";

        /// <summary>
        /// 您确认移除 {0} 吗？
        /// </summary>
        public static string Msg0076 = "您确认移除{0}吗？";

        /// <summary>
        /// 成功删除 {0} 条记录。
        /// </summary>
        public static string Msg0077 = "成功删除{0}条记录。";

        /// <summary>
        /// 用户登入被拒，用户审核中。
        /// </summary>
        public static string Msg0078 = "用户登入被拒，用户审核中。";

        /// <summary>
        /// 用户被锁定，登入被拒绝，请联系系统管理员。
        /// </summary>
        public static string Msg0079 = "用户被锁定，登入被拒绝，1分钟后登录或联系系统管理员。";

        /// <summary>
        /// 用户账号未被激活，请及时激活用户账号。
        /// </summary>
        public static string Msg0080 = "用户账号未被激活，请及时激活用户账号。";

        /// <summary>
        /// 用户被锁定，登入被拒绝，不可早于：
        /// </summary>
        public static string Msg0081 = "用户被锁定，登入被拒绝，不可早于：";

        /// <summary>
        /// 用户被锁定，登入被拒绝，不可晚于：
        /// </summary>
        public static string Msg0082 = "用户被锁定，登入被拒绝，不可晚于：";

        /// <summary>
        /// 用户被锁定，登入被拒绝，锁定开始日期：
        /// </summary>
        public static string Msg0083 = "用户被锁定，登入被拒绝，锁定开始日期：";

        /// <summary>
        /// 用户被锁定，登入被拒绝，锁定结束日期：
        /// </summary>
        public static string Msg0084 = "用户被锁定，登入被拒绝，锁定结束日期：";

        /// <summary>
        /// IP Address 不正确。
        /// </summary>
        public static string Msg0085 = "IP Address 不正确。";

        /// <summary>
        /// MAC Address 不正确。
        /// </summary>
        public static string Msg0086 = "MAC Address 不正确。";

        /// <summary>
        /// 用户已经上线，不允许重复登入。
        /// </summary>
        public static string Msg0087 = "用户已经上线，不允许重复登入。";

        /// <summary>
        /// 密码错误，登入被拒绝。
        /// </summary>
        public static string Msg0088 = "密码错误，登入被拒绝。";

        /// <summary>
        /// 已超出用户在线数量上限：
        /// </summary>
        public static string Msg0089 = "已超出用户在线数量上限：";

        /// <summary>
        /// 登入被拒绝。
        /// </summary>
        public static string Msg0090 = "登入被拒绝。";

        /// <summary>
        /// 新增操作权限项。
        /// </summary>
        public static string Msg0091 = "新增操作权限项。";

        /// <summary>
        /// 登入开始时间
        /// </summary>
        public static string Msg0092 = "登入开始时间";

        /// <summary>
        /// 登入结束时间
        /// </summary>
        public static string Msg0093 = "登入结束时间";

        /// <summary>
        /// 暂停开始时间
        /// </summary>
        public static string Msg0094 = "暂停开始时间";

        /// <summary>
        /// 暂停结束日期
        /// </summary>
        public static string Msg0095 = "暂停结束日期";

        /// <summary>
        /// {0} 在在线，不允许删除。
        /// </summary>
        public static string Msg0100 = "{0}在在线，不允许删除。";

        /// <summary>
        /// 目前用户 {0} 不允许删除自己。
        /// </summary>
        public static string Msg0101 = "目前用户{0}不允许删除自己。";

        /// <summary>
        /// 不允许使用连续重复密码。
        /// </summary>
        public static string Msg0102 = "不允许使用连续重复密码。";

        /// <summary>
        /// 调试信息
        /// </summary>
        public static string Msg0750 = "调试信息";

        /// <summary>
        /// 您确认清除权限吗？
        /// </summary>
        public static string Msg0600 = "您确认清除权限吗？";

        /// <summary>
        /// 已经成功连接到目标数据。
        /// </summary>
        public static string Msg0700 = "已经成功连接到目标数据。";

        /// <summary>
        /// 访问被拒绝，未经授权的访问。
        /// </summary>
        public static string Msg0800 = "访问被拒绝，未经授权的访问。";

        /// <summary>
        /// 服务调用被拒绝，用户未登入。
        /// </summary>
        public static string Msg0900 = "服务调用被拒绝，用户未登入。";

        /// <summary>
        /// 系统设定讯息错误，请与软件开发商联系。
        /// </summary>
        public static string Msg1000 = "系统设定讯息错误，请与软件开发商联系。";

        /// <summary>
        /// 您确认重置功能选单吗？
        /// </summary>
        public static string Msg1001 = "您确认重置功能选单吗？";

        /// <summary>
        /// {0} 不正确，请重新输入。
        /// </summary>
        public static string Msg2000 = "{0}不正确，请重新输入。";

        /// <summary>
        /// 您确认初始化系统吗？
        /// </summary>
        public static string Msg3000 = "您确认初始化系统吗？";

        /// <summary>
        /// 操作成功。
        /// </summary>
        public static string Msg3010 = "操作成功。";

        /// <summary>
        /// 操作失败。
        /// </summary>
        public static string Msg3020 = "操作失败。";

        /// <summary>
        /// 值
        /// </summary>
        public static string Msg9800 = "值";

        /// <summary>
        /// 公司
        /// </summary>
        public static string Msg9900 = "公司";

        /// <summary>
        /// 部门
        /// </summary>
        public static string Msg9901 = "部门";

        /// <summary>
        /// 用户未设置电子邮件地址。
        /// </summary>
        public static string Msg9910 = "用户未设置电子邮件地址。";

        /// <summary>
        /// 用户账号被锁定，1分钟后登录或联系系统管理员。
        /// </summary>
        public static string Msg9911 = "用户账号被锁定，1分钟后登录或联系系统管理员。";

        /// <summary>
        /// 用户还未激活账号。
        /// </summary>
        public static string Msg9912 = "用户还未激活账号。";

        /// <summary>
        /// 用户账号已被激活。
        /// </summary>
        public static string Msg9913 = "用户账号已被激活。";

        /// <summary>
        /// 用户关联。
        /// </summary>
        public static string Msg9914 = "用户关联。";

        /// <summary>
        /// 请设置约束条件。
        /// </summary>
        public static string Msg9915 = "请设置约束条件。";

        /// <summary>
        /// 显示   ▽
        /// </summary>
        public static string Msg9916 = "显示   ▽";

        /// <summary>
        /// 隐藏   △
        /// </summary>
        public static string Msg9917 = "隐藏   △";

        /// <summary>
        /// 验证表达式成功。
        /// </summary>
        public static string Msg9918 = "验证表达式成功。";

        /// <summary>
        /// 请输入条件。
        /// </summary>
        public static string Msg9919 = "请输入条件。";

        /// <summary>
        /// 请输入内容。
        /// </summary>
        public static string Msg9920 = "请输入内容。";

        /// <summary>
        /// 缺少（ 符号。
        /// </summary>
        public static string Msg9921 = "缺少（ 符号。";

        /// <summary>
        /// 缺少 ）符号。
        /// </summary>
        public static string Msg9922 = "缺少 ）符号。";

        /// <summary>
        /// 签名私钥。
        /// </summary>
        public static string Msgs857 = "签名私钥。";

        /// <summary>
        /// 签名密码。
        /// </summary>
        public static string Msgs864 = "签名密码。";

        /// <summary>
        /// 通讯用户名称。
        /// </summary>
        public static string Msgs957 = "通讯用户名称。";

        /// <summary>
        /// 通讯密码。
        /// </summary>
        public static string Msgs964 = "通讯密码。";

        /// <summary>
        /// 验证码
        /// </summary>
        public static string Msgs965 = "验证码";

        /// <summary>
        /// 服务未开始
        /// </summary>
        public static string Msg9660 = "服务未开始";

        /// <summary>
        /// 服务过期
        /// </summary>
        public static string Msg9665 = "服务过期";

        /// <summary>
        /// 密码强度不符合要求，密码至少为8位数，且为数字加字母的组合。
        /// </summary>
        public static string Msg8000 = "密码强度不符合要求，密码至少为8位数，且为数字加字母的组合。";

        /// <summary>
        /// 手机号码
        /// </summary>
        public static string Msg8700 = "手机号码";

        /// <summary>
        /// 电子邮件
        /// </summary>
        public static string Msg8800 = "电子邮件";

        /// <summary>
        /// 工号
        /// </summary>
        public static string Msg8900 = "工号";

        /// <summary>
        /// 用户名称或密码错误
        /// </summary>
        public static string Msg9000 = "用户名称或密码错误";

        /// <summary>
        /// 唯一用户名
        /// </summary>
        public static string Msg9954 = "唯一用户名";

        /// <summary>
        /// 资料
        /// </summary>
        public static string Msg9955 = "资料";

        /// <summary>
        /// 未找到满足条件的记录
        /// </summary>
        public static string Msg9956 = "未找到满足条件的记录";

        /// <summary>
        /// 用户名
        /// </summary>
        public static string Msg9957 = "用户名";

        /// <summary>
        /// 数据验证错误
        /// </summary>
        public static string Msg9958 = "数据验证错误";

        /// <summary>
        /// 新密码
        /// </summary>
        public static string Msg9959 = "新密码";

        /// <summary>
        /// 确认密码
        /// </summary>
        public static string Msg9960 = "确认密码";

        /// <summary>
        /// 原密码
        /// </summary>
        public static string Msg9961 = "原密码";

        /// <summary>
        /// 修改{0}成功。
        /// </summary>
        public static string Msg9962 = "修改{0}成功。";

        /// <summary>
        /// 设置{0}成功。
        /// </summary>
        public static string Msg9963 = "设置{0}成功。";

        /// <summary>
        /// 密码
        /// </summary>
        public static string Msg9964 = "密码";

        /// <summary>
        /// 执行成功。
        /// </summary>
        public static string Msg9965 = "执行成功。";

        /// <summary>
        /// 用户没有找到，请注意大小写。
        /// </summary>
        public static string Msg9966 = "用户没有找到，请注意大小写。";

        /// <summary>
        /// 密码错误，请注意大小写。
        /// </summary>
        public static string Msg9967 = "密码错误，请注意大小写。";

        /// <summary>
        /// 登入被拒绝，帐户已被停用，请与系统管理员联系。
        /// </summary>
        public static string Msg9968 = "登入被拒绝，帐户已被停用，请与系统管理员联系。";

        /// <summary>
        /// 基础编码
        /// </summary>
        public static string Msg9969 = "基础编码";

        /// <summary>
        /// 职员
        /// </summary>
        public static string Msg9970 = "职员";

        /// <summary>
        /// 组织机构
        /// </summary>
        public static string Msg9971 = "组织机构";

        /// <summary>
        /// 角色
        /// </summary>
        public static string Msg9972 = "角色";

        /// <summary>
        /// 选单
        /// </summary>
        public static string Msg9973 = "选单";

        /// <summary>
        /// 文件夹
        /// </summary>
        public static string Msg9974 = "文件夹";

        /// <summary>
        /// 权限
        /// </summary>
        public static string Msg9975 = "权限";

        /// <summary>
        /// 主键
        /// </summary>
        public static string Msg9976 = "主键";

        /// <summary>
        /// 编号
        /// </summary>
        public static string Msg9977 = "编号";

        /// <summary>
        /// 名称
        /// </summary>
        public static string Msg9978 = "名称";

        /// <summary>
        /// 父节点主键
        /// </summary>
        public static string Msg9979 = "父节点主键";

        /// <summary>
        /// 父节点名称
        /// </summary>
        public static string Msg9980 = "父节点名称";

        /// <summary>
        /// 功能分类主键
        /// </summary>
        public static string Msg9981 = "功能分类主键";

        /// <summary>
        /// 唯一识别主键
        /// </summary>
        public static string Msg9982 = "唯一识别主键";

        /// <summary>
        /// 主题
        /// </summary>
        public static string Msg9983 = "主题";

        /// <summary>
        /// 内容
        /// </summary>
        public static string Msg9984 = "内容";

        /// <summary>
        /// 状态代码
        /// </summary>
        public static string Msg9985 = "状态代码";

        /// <summary>
        /// 次数
        /// </summary>
        public static string Msg9986 = "次数";

        /// <summary>
        /// 有效
        /// </summary>
        public static string Msg9987 = "有效";

        /// <summary>
        /// 备注
        /// </summary>
        public static string Msg9988 = "备注";

        /// <summary>
        /// 排序码
        /// </summary>
        public static string Msg9989 = "排序码";

        /// <summary>
        /// 建立者主键
        /// </summary>
        public static string Msg9990 = "建立者主键";

        /// <summary>
        /// 建立时间
        /// </summary>
        public static string Msg9991 = "建立时间";

        /// <summary>
        /// 最后修改者主键
        /// </summary>
        public static string Msg9992 = "最后修改者主键";

        /// <summary>
        /// 修改时间
        /// </summary>
        public static string Msg9993 = "修改时间";

        /// <summary>
        /// 排序
        /// </summary>
        public static string Msg9994 = "排序";

        /// <summary>
        /// 主键
        /// </summary>
        public static string Msg9995 = "主键";

        /// <summary>
        /// 索引
        /// </summary>
        public static string Msg9996 = "索引";

        /// <summary>
        /// 字段
        /// </summary>
        public static string Msg9997 = "字段";

        /// <summary>
        /// 数据表
        /// </summary>
        public static string Msg9998 = "数据表";

        /// <summary>
        /// 数据库
        /// </summary>
        public static string Msg9999 = "数据库";

        /// <summary>
        /// 您确认清除用户角色关联吗？
        /// </summary>
        public static string Msg0200 = "您确认清除用户角色关联吗？";

        /// <summary>
        /// 您选择的档案不存在，请重新选择。
        /// </summary>
        public static string Msg0201 = "您选择的档案不存在，请重新选择。";

        /// <summary>
        /// 提示信息
        /// </summary>
        public static string Msg0202 = "提示信息";

        /// <summary>
        /// 您确认移动 \"{0}\" 到 \"{1}\" 吗？
        /// </summary>
        public static string Msg0203 = "您确认移动 \"{0}\" 到 \"{1}\" 吗？";

        /// <summary>
        /// 您确认退出应用程序吗？
        /// </summary>
        public static string Msg0204 = "您确认退出应用程序吗？";

        /// <summary>
        /// 档案{0}已存在，要覆盖服务器上的档案吗？
        /// </summary>
        public static string Msg0205 = "档案{0}已存在，要覆盖服务器上的档案吗？";

        /// <summary>
        /// 已经超过了上传限制，请检查要上传的档案大小。
        /// </summary>
        public static string Msg0206 = "已经超过了上传限制，请检查要上传的档案大小。";

        /// <summary>
        /// 您确认要删除图片吗？
        /// </summary>
        public static string Msg0207 = "您确认要删除图片吗？";

        /// <summary>
        /// 开始时间不能大于结束时间。
        /// </summary>
        public static string Msg0208 = "开始时间不能大于结束时间。";

        /// <summary>
        /// 清除成功。
        /// </summary>
        public static string Msg0209 = "清除成功。";

        /// <summary>
        /// 重置成功。
        /// </summary>
        public static string Msg0210 = "重置成功。";

        /// <summary>
        /// 已输入{0}次错误密码，不再允许继续登入，请重新启动程序进行登入。
        /// </summary>
        public static string Msg0211 = "已输入{0}次错误密码，不再允许继续登入，请重新启动程序进行登入。";

        /// <summary>
        /// 查询内容
        /// </summary>
        public static string Msg0212 = "查询内容";

        /// <summary>
        /// 编号总长度不要超过40位。
        /// </summary>
        public static string Msg0213 = "编号总长度不要超过40位。";

        /// <summary>
        /// 编号产生成功。
        /// </summary>
        public static string Msg0214 = "编号产生成功。";

        /// <summary>
        /// 增序列
        /// </summary>
        public static string Msg0215 = "增序列";

        /// <summary>
        /// 减序列
        /// </summary>
        public static string Msg0216 = "减序列";

        /// <summary>
        /// 步调
        /// </summary>
        public static string Msg0217 = "步调";

        /// <summary>
        /// 序列重置成功。
        /// </summary>
        public static string Msg0218 = "序列重置成功。";

        /// <summary>
        /// 您确认重置序列吗？
        /// </summary>
        public static string Msg0219 = "您确认重置序列吗？";

        /// <summary>
        /// 用户名称不允许为空，请输入。
        /// </summary>
        public static string Msg0223 = "用户名称不允许为空，请输入。";

        /// <summary>
        /// 目前节点上有资料。
        /// </summary>
        public static string Msg0225 = "目前节点上有资料。";

        /// <summary>
        /// 无法删除自己。
        /// </summary>
        public static string Msg0226 = "无法删除自己。";

        /// <summary>
        /// 设置关联用户成功。
        /// </summary>
        public static string Msg0228 = "设置关联用户成功。";

        /// <summary>
        /// 所在单位不允许为空，请选择。
        /// </summary>
        public static string Msg0229 = "所在单位不允许为空，请选择。";

        /// <summary>
        /// 申请账号更新成功，请等待审核。
        /// </summary>
        public static string Msg0230 = "申请账号更新成功，请等待审核。";

        /// <summary>
        /// 密码不等于确认密码，请确认后重新输入。
        /// </summary>
        public static string Msg0231 = "密码不等于确认密码，请确认后重新输入。";

        /// <summary>
        /// 用户名称
        /// </summary>
        public static string Msg0232 = "用户名称";

        /// <summary>
        /// 姓名
        /// </summary>
        public static string Msg0233 = "姓名";

        /// <summary>
        /// E-mail 格式不正确，请重新输入。
        /// </summary>
        public static string Msg0234 = "E-mail 格式不正确，请重新输入。";

        /// <summary>
        /// 申请账号成功，请等待审核。
        /// </summary>
        public static string Msg0235 = "申请账号成功，请等待审核。";

        /// <summary>
        /// 导出的目标文件已存在，要覆盖 \"{0}\" 吗？
        /// </summary>
        public static string Msg0236 = "导出的目标文件已存在，要覆盖 \"{0}\" 吗？";

        /// <summary>
        /// 发送电子邮件成功。
        /// </summary>
        public static string Msg0237 = "发送电子邮件成功。";

        /// <summary>
        /// 清除异常信息成功。
        /// </summary>
        public static string Msg0238 = "清除异常信息成功。";

        /// <summary>
        /// 您确认清除异常信息吗？
        /// </summary>
        public static string Msg0239 = "您确认清除异常信息吗？";

        /// <summary>
        /// 内容不能为空
        /// </summary>
        public static string Msg0240 = "内容不能为空";

        /// <summary>
        /// 发送电子邮件失败。
        /// </summary>
        public static string Msg0241 = "发送电子邮件失败。";

        /// <summary>
        /// 移动成功。
        /// </summary>
        public static string Msg0242 = "移动成功。";

        /// <summary>
        /// 程序异常报告
        /// </summary>
        public static string Msg0243 = "程序异常报告";

        /// <summary>
        /// 您选择的文档不存在，请重新选择。
        /// </summary>
        public static string Msg0244 = "您选择的文档不存在，请重新选择。";

        /// <summary>
        /// 请选需要处理的数据。
        /// </summary>
        public static string Msg0274 = "请选需要处理的数据。";

        /// <summary>
        /// 您确认不输入退回理由吗？
        /// </summary>
        public static string Msg0275 = "您确认不输入退回理由吗？";

        /// <summary>
        /// 您确认撤销撤销审核流程中的单据吗？
        /// </summary>
        public static string Msg0276 = "您确认撤销撤销审核流程中的单据吗？";

        /// <summary>
        /// 请选择提交给哪个用户审核。
        /// </summary>
        public static string Msg0277 = "请选择提交给哪个用户审核。";

        /// <summary>
        /// 您确认提交给用户{0}审核吗？
        /// </summary>
        public static string Msg0278 = "您确认提交给用户{0}审核吗？";

        /// <summary>
        /// 工作流程发送成功。
        /// </summary>
        public static string Msg0279 = "工作流程发送成功。";

        /// <summary>
        /// 工作流程发送失败。
        /// </summary>
        public static string Msg0280 = "工作流程发送失败。";

        /// <summary>
        /// 您确认替换文件{0}吗？
        /// </summary>
        public static string Msg0281 = "您确认替换文件{0}吗？";

        /// <summary>
        /// 上级机构
        /// </summary>
        public static string Msg0282 = "上级机构";

        /// <summary>
        /// 编号产生成功
        /// </summary>
        public static string Msg0283 = "编号产生成功";

        /// <summary>
        /// 已修改配置信息，需要保存吗？
        /// </summary>
        public static string Msg0284 = "已修改配置信息，需要保存吗？";

        /// <summary>
        /// 没有要保存的资料！
        /// </summary>
        public static string Msg0285 = "没有要保存的资料！";

        /// <summary>
        /// 单位名称
        /// </summary>
        public static string Msg0286 = "单位名称";

        /// <summary>
        /// 请选择提交给哪个部门审核。
        /// </summary>
        public static string Msg0287 = "请选择提交给哪个部门审核。";

        /// <summary>
        /// 您确认提交给部门{0}审核吗？
        /// </summary>
        public static string Msg0288 = "您确认提交给部门{0}审核吗？";

        /// <summary>
        /// 请选择提交给哪个角色审核。
        /// </summary>
        public static string Msg0289 = "请选择提交给哪个角色审核。";

        /// <summary>
        /// 您确认提交给角色{0}审核吗？
        /// </summary>
        public static string Msg0290 = "您确认提交给角色{0}审核吗？";

        /// <summary>
        /// 请选择提交给哪个角色或部门或人员审核。
        /// </summary>
        public static string Msg0291 = "请选择提交给哪个角色或部门或人员审核。";

        /// <summary>
        /// 成功退回{0}项。
        /// </summary>
        public static string Msg0292 = "成功退回{0}项。";

        /// <summary>
        /// 退回失败。
        /// </summary>
        public static string Msg0293 = "退回失败。";

        /// <summary>
        /// 您确认要转发给{0}审核吗？
        /// </summary>
        public static string Msg0294 = "您确认要转发给{0}审核吗？";

        /// <summary>
        /// 转发成功{0}项。
        /// </summary>
        public static string Msg0295 = "转发成功{0}项。";

        /// <summary>
        /// 转发失败。
        /// </summary>
        public static string Msg0296 = "转发失败。";

        /// <summary>
        /// 下线通知，您的账号在另一地点登录，您被迫下线。
        /// </summary>
        public static string Msg0300 = "下线通知，您的账号在另一地点登录，您被迫下线。";

        /// <summary>
        /// 您的帐户登录异常，被系统锁定{0}分钟，若有疑问请联系系统管理员。
        /// </summary>
        public static string Msg0400 = "您的帐户登录异常，被系统锁定{0}分钟，若有疑问请联系系统管理员。";

        /// <summary>
        /// 您确定保存吗？
        /// </summary>
        public static string Msg0301 = "您确定保存吗？";

        /// <summary>
        /// 分类。
        /// </summary>
        public static string Msg0302 = "分类。";

        /// <summary>
        /// 请选择{0}。
        /// </summary>
        public static string Msg0303 = "请选择{0}。";

        /// <summary>
        /// 用户、组织机构、角色必须选择一个。
        /// </summary>
        public static string Msg0304 = "用户、组织机构、角色必须选择一个。";

        /// <summary>
        /// 重新登入时，登入窗口名称改变为重新登入
        /// </summary>
        public static string MsgReLogOn = "重新登入";

        /// <summary>
        /// 登入服务
        /// </summary>
        public static string BaseUserManager = "登入服务";

        /// <summary>
        /// 登入操作
        /// </summary>
        public static string BaseUserManagerLogOn = "登入操作";

        /// <summary>
        /// BaseUserManagerLogOnSuccess
        /// </summary>
        public static string BaseUserManagerLogOnSuccess = "登入成功";

        /// <summary>
        /// DataGridView右键选单
        /// </summary>
        public static string ClickToolStripMenuItem = "数据列设置";

        /// <summary>
        /// BaseForm加载窗口
        /// </summary>
        public static string LoadWindow = "加载窗口";
    }
}