
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FWindSoft.Wpf.Controls
{
   /// <summary>
   /// 可控制输入值的列
   /// </summary>
    public class TDataGridTextColumn : DataGridTextColumn
    {
        public static readonly DependencyProperty TextControlProperty;

        static TDataGridTextColumn()
        {
            TextControlProperty = DependencyProperty.Register("TextControl", typeof (ITextInputControl),
                typeof (TDataGridTextColumn));
          

        }

        #region 包装属性

        public ITextInputControl TextControl
        {
            set { SetValue(TextControlProperty, value); }
            get { return (ITextInputControl) GetValue(TextControlProperty); }
        }

        #endregion
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            FrameworkElement tempElement = base.GenerateElement(cell, dataItem);
            TextBlock textBlock = tempElement as TextBlock;
            if (textBlock != null)
            {
                textBlock.HorizontalAlignment=cell.HorizontalContentAlignment;
                textBlock.VerticalAlignment = cell.VerticalContentAlignment;
            }

            return tempElement;
        }
        protected override System.Windows.FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            FrameworkElement tempElement = base.GenerateEditingElement(cell, dataItem);
            TextBox textBox = tempElement as TextBox;
            if (textBox == null) return tempElement;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            if (textBox != null)
            {
                textBox.UndoLimit = 0;
                DataObject.AddPastingHandler(textBox, new DataObjectPastingEventHandler((o,e)=>e.CancelCommand()));
                textBox.HorizontalContentAlignment = cell.HorizontalContentAlignment;
                textBox.VerticalContentAlignment = cell.VerticalContentAlignment;
                textBox.PreviewKeyDown += textBox_PreviewKeyDown;
                textBox.PreviewTextInput += textBox_PreviewTextInput;
            }
            
            return tempElement;
        }

        #region 绑定事件
        private void textBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;
            string strOld = textBox.Text;
            string chaNew = e.Text;
            string strFull = strOld + chaNew;
            //12.这样的数字，在1和2中在插入点无法识别。不明原因的末尾小数点舍掉
            int charNum = 0;
            for (int i = 0; i < textBox.LineCount; i++)
            {
                charNum += textBox.GetLineLength(i);
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
                string strFront = strOld.Substring(0, textBox.SelectionStart);
                string strAfter = strOld.Substring(textBox.SelectionStart + textBox.SelectionLength);
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

        private void textBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.ImeProcessed || e.Key == Key.Space)
                e.Handled = true;
        } 
        #endregion
    }
}
