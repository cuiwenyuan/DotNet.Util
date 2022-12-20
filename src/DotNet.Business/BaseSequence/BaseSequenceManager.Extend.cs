//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// 序列产生器
    /// BaseSequenceManager
    /// 
    /// 核心思想:
    /// 当前读取到的就是是最新的,每次读取后进行了更新
    /// 考虑到处理的简单方便以及新能的提高,可以采用多线程技术
    /// 
    /// 修改记录
    /// 
    ///		2010.07.04 版本：3.2 JiRiGaLa	用代码生成器产生序列生成器代码，规范化代码，用锁的机制防止B/S并发问题。
    ///		2010.06.03 版本：3.1 JiRiGaLa	去掉单实例的做法、防止并发问题发生。
    ///		2010.01.25 版本：3.0 JiRiGaLa	序号生成算法优化。
    ///		2008.09.09 版本：2.0 JiRiGaLa	主键整理。
    ///		2007.07.20 版本：1.9 JiRiGaLa	序列产生器，增加锁机制，并整理优化主键。
    ///		2006.02.07 版本：1.8 JiRiGaLa	重新调整主键的规范化。
    ///		2005.10.06 版本：1.7 JiRiGaLa	添加是否补充0位的属性。	
    ///		2005.08.08 版本：1.6 JiRiGaLa	命名方式等进行改进。
    ///		2005.07.15 版本：1.5 JiRiGaLa	主键格式进行改进。
    ///		2004.07.21 版本：1.4 JiRiGaLa	改进了主键的编排、参数名称规范化。
    ///		2004.06.29 版本：1.3 JiRiGaLa	将思路重新整理完整,把最得意的程序改进到更上一层楼。
    ///		2004.06.15 版本：1.2 JiRiGaLa	查询当前序号的优化，若找不到表自动添加一条。
    ///		2004.02.22 版本：1.1 JiRiGaLa	表字段名字进行了修改,一些继承属性也进行了修改。
    ///		2003.10.16 版本：1.0 JiRiGaLa	改进成以后可以扩展到多种数据库的结构形式。
    ///		 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010.01.25</date>
    /// </author> 
    /// </summary>
    public partial class BaseSequenceManager : BaseManager
    {
        /// <summary>
        /// 是否前缀补零
        /// </summary>
        public bool FillZeroPrefix = true;
        /// <summary>
        /// 默认升序序列号
        /// </summary>
        public int DefaultSequence = 10000000;

        /// <summary>
        /// 默认降序序列号
        /// </summary>
        public int DefaultReduction = 09999999;

        /// <summary>
        /// 默认的前缀
        /// </summary>
        public string DefaultPrefix = "";

        /// <summary>
        /// 默认分隔符
        /// </summary>
        public string DefaultDelimiter = "";

        /// <summary>
        /// 递增或者递减数步调
        /// </summary>
        public int DefaultStep = 1;

        /// <summary>
        /// 默认的排序码长度
        /// </summary>
        public int DefaultSequenceLength = 8;

        /// <summary>
        /// 序列长度
        /// </summary>
        public int SequenceLength = 8;

        /// <summary>
        /// 是否采用前缀
        /// </summary>
        public bool UsePrefix = true;

        /// <summary>
        /// 默认的可见性
        /// </summary>
        public int DefaultIsVisible = 1;

        private static readonly object SequenceLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="identity"></param>
        public BaseSequenceManager(IDbHelper dbHelper, bool identity)
            : this()
        {
            DbHelper = dbHelper;
            Identity = identity;
        }

        /// <summary>
        /// 按名称获取实体
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <returns>实体</returns>
        BaseSequenceEntity GetEntityByName(string name)
        {
            BaseSequenceEntity sequenceEntity = null;
            var dt = GetDataTable(new KeyValuePair<string, object>(BaseSequenceEntity.FieldName, name));
            if (dt != null && dt.Rows.Count > 0)
            {
                sequenceEntity = BaseEntity.Create<BaseSequenceEntity>(dt);
            }
            return sequenceEntity;
        }

        /// <summary>
        /// 获取添加
        /// </summary>
        /// <param name="name">序列名</param>
        /// /// <param name="defaultSequence">默认升序序列号</param>
        /// <param name="defaultReduction">默认降序序列号</param>
        /// <param name="defaultStep">递增或者递减数步调</param>
        /// <param name="defaultPrefix">默认的前缀</param>
        /// <param name="defaultDelimiter">默认分隔符</param>
        /// <param name="defaultIsVisable">默认的可见性</param>
        /// <returns>序列实体</returns>
        BaseSequenceEntity GetEntityByAdd(string name, int? defaultSequence = null, int? defaultReduction = null, int? defaultStep = null, string defaultPrefix = "", string defaultDelimiter = "", int? defaultIsVisable = null)
        {
            var sequenceEntity = GetEntityByName(name);
            if (sequenceEntity == null)
            {
                sequenceEntity = new BaseSequenceEntity
                {
                    Name = name,
                    SortCode = 1
                };
                if (defaultSequence == null)
                {
                    sequenceEntity.Sequence = DefaultSequence;
                }
                else
                {
                    sequenceEntity.Sequence = Convert.ToInt32(defaultSequence);
                }
                if (defaultReduction == null)
                {
                    sequenceEntity.Reduction = DefaultReduction;
                }
                else
                {
                    sequenceEntity.Reduction = Convert.ToInt32(defaultReduction);
                }
                if (defaultStep == null)
                {
                    sequenceEntity.Step = DefaultStep;
                }
                else
                {
                    sequenceEntity.Step = Convert.ToInt32(defaultStep);
                }
                if (!string.IsNullOrEmpty(defaultPrefix))
                {
                    sequenceEntity.Prefix = defaultPrefix;
                }
                else
                {
                    sequenceEntity.Prefix = DefaultPrefix;
                }
                if (!string.IsNullOrEmpty(defaultDelimiter))
                {
                    sequenceEntity.Delimiter = defaultDelimiter;
                }
                else
                {
                    sequenceEntity.Delimiter = DefaultDelimiter;
                }
                sequenceEntity.IsVisible = DefaultIsVisible;
                Add(sequenceEntity);
            }

            return sequenceEntity;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        public string Add(BaseSequenceEntity entity, out Status status)
        {
            var result = string.Empty;
            // 检查是否重复
            if (Exists(new KeyValuePair<string, object>(BaseSequenceEntity.FieldName, entity.Name)))
            {
                // 名称已重复
                status = Status.ErrorNameExist;
            }
            else
            {
                result = AddEntity(entity);
                // 运行成功
                status = Status.OkAdd;
            }
            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        public int Update(BaseSequenceEntity entity, out Status status)
        {
            var result = 0;
            // 检查名称是否重复
            if (Exists(new KeyValuePair<string, object>(BaseSequenceEntity.FieldName, entity.Name), entity.Id))
            {
                // 名称已重复
                status = Status.ErrorNameExist;
            }
            else
            {
                // 进行更新操作
                result = UpdateEntity(entity);
                if (result == 1)
                {
                    status = Status.OkUpdate;
                }
                else
                {
                    // 数据可能被删除
                    status = Status.ErrorDeleted;
                }
            }
            return result;
        }


        //
        // 读取序列的
        //


        /// <summary>
        /// 获取序列
        /// </summary>
        /// <param name="entity">序列实体</param>
        /// <returns>序列</returns>
        string Increment(BaseSequenceEntity entity)
        {
            var sequence = string.Empty;
            if (entity != null)
            {
                sequence = entity.Sequence.ToString();
                if (FillZeroPrefix)
                {
                    sequence = StringUtil.RepeatString("0", (SequenceLength - entity.Sequence.ToString().Length)) + entity.Sequence;
                }
                if (UsePrefix)
                {
                    sequence = entity.Prefix + entity.Delimiter + sequence;
                }
            }
            return sequence;
        }

        /// <summary>
        /// 获取降序列
        /// </summary>
        /// <param name="entity">序列实体</param>
        /// <param name="fillZeroPrefix">补齐零</param>
        /// <param name="usePrefix">使用前缀</param>
        /// <returns>降序列</returns>
        string Decrement(BaseSequenceEntity entity, bool fillZeroPrefix = false, bool usePrefix = false)
        {
            var reduction = entity.Reduction.ToString();
            if (fillZeroPrefix)
            {
                reduction = StringUtil.RepeatString("0", (SequenceLength - entity.Reduction.ToString().Length)) + entity.Reduction;
            }
            if (usePrefix)
            {
                reduction = entity.Prefix + entity.Delimiter + reduction;
            }
            return reduction;
        }


        //
        // 一 获取序列原值(没有序列时，涉及到并发问题、锁机制)
        //


        #region public string StoreCounter(string name) 获得原序列号
        /// <summary>
        /// 获得原序列号
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <returns>序列号</returns>
        public string StoreCounter(string name)
        {
            return StoreCounter(name, DefaultSequence, DefaultSequenceLength, FillZeroPrefix);
        }
        #endregion

        #region public string StoreCounter(string name, int defaultSequence) 获得原序列号
        /// <summary>
        /// 获得原序列号
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <returns>序列号</returns>
        public string StoreCounter(string name, int defaultSequence)
        {
            return StoreCounter(name, defaultSequence, DefaultSequenceLength, FillZeroPrefix);
        }
        #endregion

        #region public string StoreCounter(string name, int defaultSequence, int sequenceLength) 获得原序列号
        /// <summary>
        /// 获得原序列
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <returns>序列号</returns>
        public string StoreCounter(string name, int defaultSequence, int sequenceLength)
        {
            return StoreCounter(name, defaultSequence, sequenceLength, false);
        }
        #endregion

        #region public string StoreCounter(string name, int defaultSequence, int sequenceLength, bool fillZeroPrefix) 获取序原列号
        /// <summary>
        /// 获得原序列号
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <param name="fillZeroPrefix">是否填充补零</param>
        /// <returns>序列号</returns>
        public string StoreCounter(string name, int defaultSequence, int sequenceLength, bool fillZeroPrefix)
        {
            var sequence = string.Empty;
            // 这里用锁的机制，提高并发控制能力
            lock (SequenceLock)
            {
                SequenceLength = sequenceLength;
                FillZeroPrefix = fillZeroPrefix;
                DefaultReduction = defaultSequence;
                DefaultSequence = defaultSequence + 1;

                var entity = GetEntityByAdd(name);
                sequence = Increment(entity);
            }
            return sequence;
        }
        #endregion



        #region 三 获取新序列(没有序列时，涉及到并发问题、锁机制，更新序列时会有锁机制)

        /// <summary>
        /// 获取Oracle的序列
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        public string GetOracleSequence(string sequenceName)
        {
            // 当前是自增量，并且是Oracle数据库
            // Oracle的最大Sequence长度为30位
            sequenceName = sequenceName.ToUpper().Cut(30);
            DbHelper.SequenceExists(sequenceName);
            return DbHelper.ExecuteScalar("SELECT " + sequenceName + ".NEXTVAL FROM DUAL ")?.ToString();
        }

        /// <summary>
        /// 获取Oracle的当前序列
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        public string GetOracleStoreCounter(string sequenceName)
        {
            // 当前是自增量，并且是Oracle数据库
            // Oracle的最大Sequence长度为30位
            sequenceName = sequenceName.ToUpper().Cut(30);
            DbHelper.SequenceExists(sequenceName);
            return DbHelper.ExecuteScalar("SELECT " + sequenceName + ".CURRVAL FROM DUAL ")?.ToString();
        }

        /// <summary>
        /// 获取DB2的序列
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        public string GetDb2Sequence(string sequenceName)
        {
            // 当前是自增量，并且是DB2数据库
            return DbHelper.ExecuteScalar("SELECT NEXTVAL FOR " + sequenceName.ToUpper() + " FROM sysibm.sysdummy1").ToString();
        }

        #region public string Increment(string name) 获得序列号
        /// <summary>
        /// 获得序列号
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <returns>序列号</returns>
        public string Increment(string name)
        {
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                return GetOracleSequence(name);
            }
            if (DbHelper.CurrentDbType == CurrentDbType.Db2)
            {
                return GetDb2Sequence(name);
            }
            return Increment(name, DefaultSequence, DefaultSequenceLength, FillZeroPrefix);
        }
        #endregion

        #region public string Increment(string name, int defaultSequence) 获得序列号
        /// <summary>
        /// 获得序列号
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <returns>序列号</returns>
        public string Increment(string name, int defaultSequence)
        {
            return Increment(name, defaultSequence, DefaultSequenceLength, FillZeroPrefix);
        }
        #endregion

        #region public string Increment(string name, int defaultSequence, int sequenceLength) 获得序列号
        /// <summary>
        /// 获得序列
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <returns>序列号</returns>
        public string Increment(string name, int defaultSequence, int sequenceLength)
        {
            return Increment(name, defaultSequence, sequenceLength, false);
        }
        #endregion

        #region public string Increment(string name, int defaultSequence, int sequenceLength, bool fillZeroPrefix, string prefix = "", string delimiter = "") 获取序列号

        /// <summary>
        /// 获得序列
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <param name="fillZeroPrefix">是否填充零</param>
        /// <param name="prefix"></param>
        /// <param name="delimiter"></param>
        /// <returns>序列实体</returns>
        public string Increment(string name, int defaultSequence, int sequenceLength, bool fillZeroPrefix, string prefix = "", string delimiter = "")
        {
            DefaultSequence = defaultSequence;
            SequenceLength = sequenceLength;
            FillZeroPrefix = fillZeroPrefix;
            DefaultReduction = defaultSequence - 1;
            DefaultPrefix = prefix;
            DefaultDelimiter = delimiter;

            // 写入调试信息
#if (DEBUG)
            var milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now + " :Begin: " + MethodBase.GetCurrentMethod()?.ReflectedType?.Name + "." + MethodBase.GetCurrentMethod()?.Name);
#endif

            BaseSequenceEntity entity = null;

            // 这里用锁的机制，提高并发控制能力
            lock (SequenceLock)
            {

                switch (DbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.MySql:
                    case CurrentDbType.SqlServer:
                        entity = GetEntityByAdd(name);
                        UpdateSequence(name);
                        break;
                    case CurrentDbType.Oracle:
                        // 这里加锁机制。
                        if (DbHelper.InTransaction)
                        {
                            // 不可以影响别人的事务
                            entity = GetSequenceByLock(name, defaultSequence);
                            if (StatusCode == Status.LockOk.ToString())
                            {
                                if (UpdateSequence(name) > 0)
                                {
                                    StatusCode = Status.LockOk.ToString();
                                }
                                else
                                {
                                    StatusCode = Status.CanNotLock.ToString();
                                }
                            }
                        }
                        else
                        {
                            // 开始事务
                            var dbTransaction = DbHelper.BeginTransaction();
                            try
                            {
                                StatusCode = Status.CanNotLock.ToString();
                                entity = GetSequenceByLock(name, defaultSequence);
                                if (StatusCode == Status.LockOk.ToString())
                                {
                                    StatusCode = Status.CanNotLock.ToString();
                                    if (UpdateSequence(name) > 0)
                                    {
                                        // 提交事务
                                        dbTransaction.Commit();
                                        StatusCode = Status.LockOk.ToString();
                                    }
                                    else
                                    {
                                        // 回滚事务
                                        dbTransaction.Rollback();
                                    }
                                }
                                else
                                {
                                    // 回滚事务
                                    dbTransaction.Rollback();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                // 回滚事务
                                dbTransaction.Rollback();
                                StatusCode = Status.CanNotLock.ToString();
                            }
                            //Troy.Cui 2018.07.02
                            finally
                            {
                                // 关闭数据库库连接
                                DbHelper.Close();
                            }
                        }
                        break;
                }
            }

            // 写入调试信息
#if (DEBUG)
            var milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod()?.ReflectedType?.Name + "." + MethodBase.GetCurrentMethod()?.Name);
#endif

            return Increment(entity);
        }
        #endregion

        #region protected int UpdateSequence(string name) 更新升序序列
        /// <summary>
        /// 更新升序序列
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <returns>影响行数</returns>
        protected int UpdateSequence(string name)
        {
            return UpdateSequence(name, 1);
        }
        #endregion

        #region protected int UpdateSequence(string name, int sequenceCount) 更新升序序列
        /// <summary>
        /// 更新升序序列
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="sequenceCount">序列个数</param>
        /// <returns>影响行数</returns>
        protected int UpdateSequence(string name, int sequenceCount)
        {
            // 更新数据库里的值
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetFormula(BaseSequenceEntity.FieldSequence, BaseSequenceEntity.FieldSequence + " + " + sequenceCount + " * " + BaseSequenceEntity.FieldStep);
            sqlBuilder.SetWhere(BaseSequenceEntity.FieldName, name);
            return sqlBuilder.EndUpdate();
        }
        #endregion

        #endregion

        #region 三 获取降序序列(没有序列时，涉及到并发问题、锁机制，更新序列时会有锁机制)

        #region public string GetReduction(string name) 获取倒序序列号
        /// <summary>
        /// 获取倒序序列号
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <returns>序列号</returns>
        public string GetReduction(string name)
        {
            return GetReduction(name, DefaultSequence);
        }
        #endregion

        #region public string GetReduction(string name, int defaultSequence) 获取倒序序列号
        /// <summary>
        /// 获取倒序序列号
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="defaultSequence">默认序列值</param>
        /// <returns>序列号</returns>
        public string GetReduction(string name, int defaultSequence)
        {
            // 写入调试信息
#if (DEBUG)
            var milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now + " :Begin: " + MethodBase.GetCurrentMethod()?.ReflectedType?.Name + "." + MethodBase.GetCurrentMethod()?.Name);
#endif

            BaseSequenceEntity sequenceEntity = null;

            // 这里用锁的机制，提高并发控制能力
            lock (SequenceLock)
            {

                DefaultReduction = defaultSequence;
                DefaultSequence = defaultSequence + 1;

                switch (DbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.MySql:
                    case CurrentDbType.SqlServer:
                        sequenceEntity = GetEntityByAdd(name);
                        UpdateReduction(name);
                        break;
                    case CurrentDbType.Oracle:
                        if (DbHelper.InTransaction)
                        {
                            //不可以影响别人的事务
                            sequenceEntity = GetSequenceByLock(name, defaultSequence);
                            if (StatusCode == Status.LockOk.ToString())
                            {
                                if (UpdateReduction(name) > 0)
                                {
                                    StatusCode = Status.LockOk.ToString();
                                }
                                else
                                {
                                    StatusCode = Status.CanNotLock.ToString();
                                }
                            }
                        }
                        else
                        {
                            // 这里加锁机制。
                            try
                            {
                                // 开始事务
                                DbHelper.BeginTransaction();
                                StatusCode = Status.CanNotLock.ToString();
                                sequenceEntity = GetSequenceByLock(name, defaultSequence);
                                if (StatusCode == Status.LockOk.ToString())
                                {
                                    StatusCode = Status.CanNotLock.ToString();
                                    if (UpdateReduction(name) > 0)
                                    {
                                        // 提交事务
                                        DbHelper.CommitTransaction();
                                        StatusCode = Status.LockOk.ToString();
                                    }
                                    else
                                    {
                                        // 回滚事务
                                        DbHelper.RollbackTransaction();
                                    }
                                }
                                else
                                {
                                    // 回滚事务
                                    DbHelper.RollbackTransaction();
                                }
                            }
                            catch
                            {
                                // 回滚事务
                                DbHelper.RollbackTransaction();
                                StatusCode = Status.CanNotLock.ToString();
                            }
                            //Troy.Cui 2018.07.02
                            finally
                            {
                                // 关闭数据库库连接
                                DbHelper.Close();
                            }
                        }
                        break;
                }
            }

            // 写入调试信息
#if (DEBUG)
            var milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod()?.ReflectedType?.Name + "." + MethodBase.GetCurrentMethod()?.Name);
#endif

            return Decrement(sequenceEntity);
        }
        #endregion

        #region protected int UpdateReduction(string name)
        /// <summary>
        /// 更新降序序列
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <returns>影响行数</returns>
        protected int UpdateReduction(string name)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetFormula(BaseSequenceEntity.FieldReduction, BaseSequenceEntity.FieldReduction + " - " + BaseSequenceEntity.FieldStep);
            sqlBuilder.SetWhere(BaseSequenceEntity.FieldName, name);
            return sqlBuilder.EndUpdate();
        }
        #endregion

        #region protected BaseSequenceEntity GetSequenceByLock(string name, int defaultSequence) 获得序列
        /// <summary>
        /// 获得序列
        /// </summary>
        /// <param name="name">序列名</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <returns>序列实体</returns>
        protected BaseSequenceEntity GetSequenceByLock(string name, int defaultSequence)
        {
            var sequenceEntity = GetEntityByAdd(name);
            if (sequenceEntity == null)
            {
                // 这里添加记录时加锁机制。
                // 是否已经被锁住
                StatusCode = Status.CanNotLock.ToString();
                for (var i = 0; i < BaseSystemInfo.LockNoWaitCount; i++)
                {
                    // 被锁定的记录数
                    var lockCount = DbHelper.LockNoWait(BaseSequenceEntity.CurrentTableName, new KeyValuePair<string, object>(BaseSequenceEntity.FieldName, BaseSequenceEntity.CurrentTableName));
                    if (lockCount > 0)
                    {

                        sequenceEntity.Name = name;
                        sequenceEntity.Reduction = defaultSequence - 1;
                        sequenceEntity.Sequence = defaultSequence;
                        sequenceEntity.Step = DefaultStep;
                        AddEntity(sequenceEntity);

                        StatusCode = Status.LockOk.ToString();
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(RandomUtil.GetRandom(1, BaseSystemInfo.LockNoWaitTickMilliSeconds));
                    }
                }
                if (StatusCode == Status.LockOk.ToString())
                {
                    // JiRiGaLa 这个是否能省略
                    sequenceEntity = GetEntityByAdd(name);
                }
            }
            else
            {
                // 若记录已经存在，加锁，然后读取记录。
                // 是否已经被锁住
                StatusCode = Status.CanNotLock.ToString();
                for (var i = 0; i < BaseSystemInfo.LockNoWaitCount; i++)
                {
                    // 被锁定的记录数
                    var lockCount = DbHelper.LockNoWait(BaseSequenceEntity.CurrentTableName, new KeyValuePair<string, object>(BaseSequenceEntity.FieldName, name));
                    if (lockCount > 0)
                    {
                        sequenceEntity = GetEntityByAdd(name);
                        StatusCode = Status.LockOk.ToString();
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(RandomUtil.GetRandom(1, BaseSystemInfo.LockNoWaitTickMilliSeconds));
                    }
                }
            }
            return sequenceEntity;
        }
        #endregion

        #endregion

        #region 四 批量获取新序列(没有序列时，涉及到并发问题、锁机制，更新序列时会有锁机制)

        #region public string[] GetBatchSequence(string name, int sequenceCount) 获取序列号数组
        /// <summary>
        /// 获取序列号数组
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="sequenceCount">序列个数</param>
        /// <returns>序列号</returns>
        public string[] GetBatchSequence(string name, int sequenceCount)
        {
            return GetBatchSequence(name, sequenceCount, DefaultSequence);
        }
        #endregion

        #region private string[] Increment(BaseSequenceEntity entity, int sequenceCount) 批量产生主键
        /// <summary>
        /// 批量产生主键
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="sequenceCount">序列个数</param>
        /// <returns>主键数组</returns>
        private string[] Increment(BaseSequenceEntity entity, int sequenceCount)
        {
            var result = new string[sequenceCount];
            for (var i = 0; i < sequenceCount; i++)
            {
                result[i] = Increment(entity);
                entity.Sequence += entity.Step;
            }
            return result;
        }
        #endregion

        #region public string[] GetBatchSequence(string name, int sequenceCount, int defaultSequence) 获取序列号数组
        /// <summary>
        /// 获取序列号数组
        /// </summary>
        /// <param name="name">序列名称</param>
        /// <param name="sequenceCount">序列个数</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <returns>序列号</returns>
        public string[] GetBatchSequence(string name, int sequenceCount, int defaultSequence)
        {
            // 写入调试信息
#if (DEBUG)
            var milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now + " :Begin: " + MethodBase.GetCurrentMethod()?.ReflectedType?.Name + "." + MethodBase.GetCurrentMethod()?.Name);
#endif

            var result = new string[sequenceCount];

            // 这里用锁的机制，提高并发控制能力
            lock (SequenceLock)
            {
                DefaultSequence = defaultSequence;
                switch (DbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.MySql:
                    case CurrentDbType.SqlServer:
                        var entity = GetEntityByAdd(name);
                        UpdateSequence(name, sequenceCount);
                        // 这里循环产生ID数组
                        result = Increment(entity, sequenceCount);
                        break;
                    case CurrentDbType.Db2:
                        for (var i = 0; i < sequenceCount; i++)
                        {
                            result[i] = GetDb2Sequence(name);
                        }
                        break;
                    case CurrentDbType.Oracle:
                        for (var i = 0; i < sequenceCount; i++)
                        {
                            result[i] = GetOracleSequence(name);
                        }
                        break;
                }
            }

            // 写入调试信息
#if (DEBUG)
            var milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod()?.ReflectedType?.Name + "." + MethodBase.GetCurrentMethod()?.Name);
#endif

            return result;
        }
        #endregion


        #endregion

        #region 重置序列(暂不考虑并发问题)

        #region public int Reset(string[] ids) 批量重置
        /// <summary>
        /// 批量重置
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int Reset(string[] ids)
        {
            var result = 0;
            BaseSequenceEntity sequenceEntity = null;
            var sqlBuilder = new SqlBuilder(DbHelper);
            for (var i = 0; i < ids.Length; i++)
            {
                if (ids[i].Length > 0)
                {
                    // 若有相应的表，那就把序列号都计算好
                    sequenceEntity = GetEntity(ids[i]);
                    var commandText = string.Format(@"UPDATE BaseSequence
                                               SET Sequence = (SELECT MAX(SortCode) + 1  AS MaxSortCode FROM {0})
	                                               , Reduction = ( SELECT MIN(SortCode) -1 AS MinSortCode FROM {0})
                                             WHERE Name = '{0}' ", sequenceEntity.Name);
                    try
                    {
                        ExecuteNonQuery(commandText);
                    }
                    catch
                    {
                        sqlBuilder.BeginUpdate(CurrentTableName);
                        sqlBuilder.SetValue(BaseSequenceEntity.FieldSequence, DefaultSequence);
                        sqlBuilder.SetValue(BaseSequenceEntity.FieldReduction, DefaultReduction);
                        sqlBuilder.SetWhere(BaseSequenceEntity.FieldId, ids[i]);
                        result += sqlBuilder.EndUpdate();
                    }
                }
            }
            return result;
        }
        #endregion

        #region 重置

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="name">序列名（默认为表名）</param>
        /// <returns>影响行数</returns>
        public int Reset(string name)
        {
            var commandText = string.Format(@"UPDATE " + CurrentTableName + " SET Sequence = (SELECT ISNULL(MAX(SortCode),10000000) AS MaxSortCode FROM {0} WHERE SortCode > 0), Reduction = (SELECT ISNULL(MIN(SortCode),9999999) AS MinSortCode FROM {0} WHERE SortCode > 0) WHERE Name = N'{0}' ", name);
            var result = ExecuteNonQuery(commandText);
            return result;
        }
        #endregion

        #endregion
    }
}