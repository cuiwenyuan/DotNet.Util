//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Reflection;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// ServiceInfo
    /// 当前操作类
    /// 
    /// 修改记录
    /// 
    ///		2016.02.16 版本：1.0 JiRiGaLa 整理文件、完善日志记录功能。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.16</date>
    /// </author> 
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime = DateTime.Now;

        /// <summary>
        /// 任务唯一标识
        /// </summary>
        public string TaskId = string.Empty;

        /// <summary>
        /// 方法运行耗时
        /// </summary>
        public long ElapsedTicks = 0;

        /// <summary>
        /// 是否记录操作日志
        /// </summary>
        public bool RecordLog { set; get; } = false;

        /// <summary>
        /// 用户信息
        /// </summary>
        public BaseUserInfo UserInfo { set; get; } = null;

        /// <summary>
        /// 当前方法
        /// </summary>
        public MethodBase CurrentMethod { set; get; } = null;

        /// <summary>
        /// 服务信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="currentMethod"></param>
        private ServiceInfo(BaseUserInfo userInfo, MethodBase currentMethod)
        {
            UserInfo = userInfo;
            CurrentMethod = currentMethod;
        }

        private ServiceInfo(MethodBase currentMethod)
        {
            CurrentMethod = currentMethod;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="currentMethod"></param>
        /// <returns></returns>
        public static ServiceInfo Create(BaseUserInfo userInfo, MethodBase currentMethod)
        {
            return new ServiceInfo(userInfo, currentMethod) { RecordLog = false, StartTime = DateTime.Now };
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentMethod"></param>
        /// <returns></returns>
        public static ServiceInfo Create(string taskId, MethodBase currentMethod)
        {
            return new ServiceInfo(currentMethod) { RecordLog = false, TaskId = taskId, StartTime = DateTime.Now };
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="userInfo"></param>
        /// <param name="currentMethod"></param>
        /// <returns></returns>
        public static ServiceInfo Create(string taskId, BaseUserInfo userInfo, MethodBase currentMethod)
        {
            return new ServiceInfo(userInfo, currentMethod) { RecordLog = false, TaskId = taskId, StartTime = DateTime.Now };
        }
    }
}
