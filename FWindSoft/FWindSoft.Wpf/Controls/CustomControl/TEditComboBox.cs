
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OBR.Wpf.Extensions;

namespace FWindSoft.Wpf.Controls
{
    public class TEditComboBox:ComboBox
    {
        private TextBox m_PartTextBox;
        public static readonly DependencyProperty TextControlProperty;
        static TEditComboBox()
        {
            TextControlProperty = DependencyProperty.Register("TextControl", typeof(ITextInputControl),
                typeof(TEditComboBox));
            //InputMethod.IsInputMethodEnabledProperty.OverrideMetadata(typeof(TSZEditComboBox), new FrameworkPropertyMetadata(false));
        }

        public TEditComboBox()
        {
            
            this.AddHandler(TextBoxBase.TextChangedEvent,new RoutedEventHandler(TextBoxBase_OnTextChanged));
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(m_DataObjectPastingEventHandler));
         
        }
        #region 屏蔽复制命令
        public void m_DataObjectPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }
        #endregion

        #region 文本change
        private void TextBoxBase_OnTextChanged(object sender, RoutedEventArgs e)
        {
            //#region 获取关联textBox
            //var element = this.GetSpecifyType<TextBox>();
            //if (m_PartTextBox==null&&element.Count > 0)
            //    m_PartTextBox = element[0];
            //#endregion
            //TextChangedEventArgs args = e as TextChangedEventArgs;
            //if (args == null || this.m_PartTextBox == null)
            //    return;
            //if (TextControl == null)
            //{
            //    return;
            //}
            //TextChange[] change = new TextChange[args.Changes.Count];
            //args.Changes.CopyTo(change, 0);

            //int offset = change[0].Offset;
            //if (change[0].AddedLength > 0)
            //{
            //    if (!TextControl.InputControl(this.Text))
            //    {
            //        this.Text = this.m_OldText;//记录历史值，使用历史值
            //        m_PartTextBox.Select(offset, 0);
            //    }
            //}
            //this.m_OldText = this.Text;
        }
        #endregion
        #region 包装属性

        public ITextInputControl TextControl
        {
            set { SetValue(TextControlProperty, value); }
            get { return (ITextInputControl)GetValue(TextControlProperty); }
        }

        #endregion
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.ImeProcessed ||e.Key== Key.Space)
                e.Handled = true;

        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            #region 获取关联textBox
            var element = this.GetSpecifyType<TextBox>();
            if (m_PartTextBox == null && element.Count > 0)
                m_PartTextBox = element[0];
            #endregion
            string strOld = this.Text;
            string chaNew = e.Text;
            string strFull = string.Format(strOld + chaNew);
            if (this.m_PartTextBox != null)
            {
                strOld = this.m_PartTextBox.Text;
                //this.m_PartTextBox.
                try
                {
                    string strFront = strOld.Substring(0, m_PartTextBox.SelectionStart);
                    string strAfter = strOld.Substring(m_PartTextBox.SelectionStart + m_PartTextBox.SelectionLength);
                    strFull = string.Format(strFront + chaNew + strAfter);//拼接新值
                }
                catch (ArgumentOutOfRangeException)
                {
                    strOld = strOld + ".";
                    string strFront = strOld.Substring(0, m_PartTextBox.SelectionStart);
                    string strAfter = strOld.Substring(m_PartTextBox.SelectionStart + m_PartTextBox.SelectionLength);
                    strFull = string.Format(strFront + chaNew + strAfter); //拼接新值
                }
                catch (Exception)
                {
                }
            }
           


            #region 加载验证

            if (TextControl != null)
            {
                e.Handled = !TextControl.InputControl(strFull.Trim());
            }

            #endregion
        }
    }
}
