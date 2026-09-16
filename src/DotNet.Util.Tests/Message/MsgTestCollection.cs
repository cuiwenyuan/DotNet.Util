using Xunit;

namespace DotNet.Util.Tests
{
    /// <summary>
    ///	MsgTestCollection
    /// 消息层测试集合。
    /// 
    /// 背景：Msg.CurrentLanguage 是全局静态状态，而 xUnit 默认让不同测试类并行执行。
    /// 一旦某个类把语言切成 en，并行中另一个断言中文的测试就会偶发失败。
    /// 因此凡涉及「默认中文输出」或「切换语言」的测试类都归入本集合，由 xUnit 串行执行。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.16 版本：1.0	Troy.Cui 建立（P3 引入 BaseResult 默认消息本地化后出现竞争）。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.16</date>
    /// </author> 
    /// </summary>
    [CollectionDefinition(Name, DisableParallelization = true)]
    public class MsgTestCollection
    {
        /// <summary>
        /// 集合名称
        /// </summary>
        public const string Name = "MsgTestCollection";
    }
}
