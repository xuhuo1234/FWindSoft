using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace FWindSoft.WinForm
{
    public class CheckBoxEditor:UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                if (provider == null)
                    return value;
                IWindowsFormsEditorService edSvc =
                       (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                CheckBox check=new CheckBox();
                check.Checked = Convert.ToBoolean(context.PropertyDescriptor.GetValue(context.Instance));
                edSvc.DropDownControl(check);
                edSvc.CloseDropDown();
                return check.Checked;
            }
            catch (Exception)
            {
                
            }
            return base.EditValue(context, provider, value);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            var context = e.Context;
            bool check = Convert.ToBoolean(context.PropertyDescriptor.GetValue(context.Instance));
            
            ControlPaint.DrawCheckBox(e.Graphics, e.Bounds, check?ButtonState.Checked : ButtonState.Normal);
        }
    }
}
