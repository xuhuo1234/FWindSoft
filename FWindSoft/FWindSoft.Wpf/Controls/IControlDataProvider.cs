namespace FWindSoft.Wpf
{
    public interface IControlDataProvider
    {
        /// <summary>
        /// 获取控件值
        /// </summary>
        /// <returns></returns>
        string GetControlData();
        /// <summary>
        /// 设置控件值
        /// </summary>
        /// <param name="controlData"></param>
        void SetControlData(string controlData);
    }
}
