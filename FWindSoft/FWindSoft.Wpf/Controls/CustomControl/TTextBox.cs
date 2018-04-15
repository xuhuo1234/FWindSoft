using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FWindSoft.Wpf.Controls
{
    public class TTextBox : TextBox
    {
        public static readonly DependencyProperty TextControlProperty;
         static TTextBox()
        {
            TextControlProperty = DependencyProperty.Register("TextControl", typeof (ITextInputControl),
                typeof(TTextBox));
             //InputMethod.IsInputMethodEnabledProperty.OverrideMetadata(typeof(TSZTextBox),new FrameworkPropertyMetadata(false));
             //禁用键盘和粘贴复制
        }

        public TTextBox()
        {
            this.UndoLimit = 0;
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(m_DataObjectPastingEventHandler));
         
        }
        #region 屏蔽复制命令
        public void m_DataObjectPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }
        #endregion
        #region 包装属性

        public ITextInputControl TextControl {
            set { SetValue(TextControlProperty,value);}
            get { return (ITextInputControl)GetValue(TextControlProperty); }
        }

        #endregion
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.ImeProcessed || e.Key == Key.Space)
                e.Handled = true;

        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            string strOld =this.Text;
            string chaNew = e.Text;
            string strFull = strOld + chaNew;
            //12.这样的数字，在1和2中在插入点无法识别。不明原因的末尾小数点舍掉
            int charNum = 0;
            for (int i = 0; i < this.LineCount; i++)
            {
                charNum += this.GetLineLength(i);
            }
            if (charNum > strOld.Length)
            {
                if (strOld.IndexOf(".") == -1)
                {
                    strOld += string.Format(".");
                }               
            }
            while (charNum > strOld.Length)
            {
                strOld += string.Format("0");
            }
            try
            {
                //((System.Windows.Controls.TextBox)(this)).EndPosition
                 //(typeof(TextBox)).GetProperty("EndPosition", BindingFlags.NonPublic);
                
                //末尾小数点抹掉时可能会出异常
                string strFront = strOld.Substring(0, this.SelectionStart);
                string strAfter = strOld.Substring(this.SelectionStart + this.SelectionLength);
                strFull = string.Format(strFront + chaNew + strAfter); //拼接新值
            }
            catch (ArgumentOutOfRangeException)
            {
                //strOld = strOld + ".";
                //string strFront = strOld.Substring(0, this.SelectionStart);
                //string strAfter = strOld.Substring(this.SelectionStart + this.SelectionLength);
                //strFull = string.Format(strFront + chaNew + strAfter); //拼接新值
            }
            catch (Exception)
            {

            }


            #region 加载验证

            if (TextControl != null)
            {
                e.Handled = !TextControl.InputControl(strFull.Trim());
            }

            #endregion
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            //if (TextControl == null)
            //{
            //    return;
            //}
            //TextChange[] change = new TextChange[e.Changes.Count];
            //e.Changes.CopyTo(change, 0);
            //var dd= this.GetValue(TextProperty);
            //string strFull = this.Text;//处理绑定时，最后小数点可能会过滤的情况
            //strFull = strFull.Trim();
            //int offset = change[0].Offset;
            //if (change[0].AddedLength > 0)
            //{
            //    if (!TextControl.InputControl(strFull))
            //    {
            //        this.Text = this.m_OldText;//记录历史值，使用历史值
            //        this.Select(offset, 0);
            //    }
            //}
            //this.m_OldText = strFull;
           
           
        }
    }
}
