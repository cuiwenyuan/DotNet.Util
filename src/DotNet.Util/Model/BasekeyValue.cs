namespace DotNet.Util
{
    /// <summary>
    /// KeyValue结构的实体
    /// </summary>
    public class BaseKeyValue
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
