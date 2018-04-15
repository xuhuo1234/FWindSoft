using System;
using System.ComponentModel;

namespace FWindSoft.WinForm
{
    internal class PropDescriptor : PropertyDescriptor
    {
        private PropertyItem m_Prop;

        public PropDescriptor(PropertyItem prop, Attribute[] attrs)
            : base(prop.Name, attrs)
        {
            m_Prop = prop;
        }

        #region 重写抽象方法

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return this.m_Prop.Value;
        }


        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {
            this.m_Prop.Value = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override object GetEditor(Type editorBaseType)
        {
            return this.m_Prop.Editor ?? base.GetEditor(editorBaseType);
        }

        #endregion

        #region 重写属性

        public override Type ComponentType
        {
            get { return this.GetType(); }
        }

        public override bool IsReadOnly
        {
            get { return this.m_Prop.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return m_Prop.Value.GetType(); }
        }

        /// <summary>
        /// 重写分类
        /// </summary>
        public override string Category
        {
            get { return this.m_Prop.Category; }
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        public override string DisplayName
        {
            get { return this.m_Prop.DisplayName; }
        }

        public override TypeConverter Converter
        {
            get { return this.m_Prop.Convert; }
        }

        public override string Description
        {
            get { return this.m_Prop.Description; }
        }

        public override bool IsBrowsable
        {
            get { return this.m_Prop.IsVisible; }
        }

        #endregion
    }
}
