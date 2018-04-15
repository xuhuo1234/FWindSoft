using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FWindSoft.Wpf
{
    public static class WindowControlHistoryData
    {
        /// <summary>
        /// 保存控件数据
        /// </summary>
        public static void SaveControlData(this Window win, List<SaveData> perDatas =null)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(Encoding.Default.GetBytes(win.GetType().FullName + win.Title));
            string winFileName =string.Join("", md5.Hash.Select(b=>b.ToString()));
            var path = Path.Combine(BaseApp.AppTempFilePath,
                             @"HistoryData\" + winFileName + ".txt");
            var datas = new List<SaveData>();
            if(perDatas!=null)
                datas.AddRange(perDatas);
            GetControlData(win, datas);
            string saveData = SerializerUnit.ObjectToJson(datas);
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllText(path, saveData, Encoding.Default);
        }

        public static void GetControlData(this Window win,ref List<SaveData> datas)
        {
            GetControlData(win, datas);
        }

       
        /// <summary>
        /// 保存控件数据
        /// </summary>
        public static void SaveControlData(this FrameworkElement element,Window win, List<SaveData> perDatas =null)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string addInfo = element.Name.Equals("") ? element.GetType().FullName :string.Format("_" +element.Name);
            md5.ComputeHash(Encoding.Default.GetBytes(win.GetType().FullName + win.Title+addInfo));
            string winFileName = string.Join("", md5.Hash.Select(b => b.ToString()));
            var path = Path.Combine(BaseApp.AppTempFilePath,
                             @"HistoryData\" + winFileName + ".txt");
            var datas = new List<SaveData>();
            if (perDatas != null)
                datas.AddRange(perDatas);
            GetControlData(element, datas);
            string saveData = SerializerUnit.ObjectToJson(datas);
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllText(path, saveData, Encoding.Default);
        }
        /// <summary>
        /// 设置控件数据
        /// </summary>
        public static void SetControlData(this Window win)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.Default.GetBytes(win.GetType().FullName + win.Title));
            string winFileName = string.Join("", md5.Hash.Select(b => (b).ToString()));

            var path = Path.Combine(BaseApp.AppTempFilePath,
                  @"HistoryData\" + winFileName + ".txt");
            if (File.Exists(path))
            {
                string str = File.ReadAllText(path, Encoding.Default);
                var datas = SerializerUnit.JsonToObj<List<SaveData>>(str);
                SetControlData(win, datas);
            }
        }
        /// <summary>
        /// 设置控件数据
        /// </summary>
        public static void SetControlData(this FrameworkElement element, Window win)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string addInfo = element.Name.Equals("") ? element.GetType().FullName : string.Format("_" + element.Name);
            md5.ComputeHash(Encoding.Default.GetBytes(win.GetType().FullName + win.Title + addInfo));
            string winFileName = string.Join("", md5.Hash.Select(b => (b).ToString()));

            var path = Path.Combine(BaseApp.AppTempFilePath,
                  @"HistoryData\" + winFileName + ".txt");
            if (File.Exists(path))
            {
                string str = File.ReadAllText(path, Encoding.Default);
                var datas = SerializerUnit.JsonToObj<List<SaveData>>(str);
                SetControlData(element, datas);
            }
        }
        /// <summary>
        /// 设置控件数据
        /// </summary>
        public static void SetControlData(this Window win,out List<SaveData> datas)
        {
            datas=new List<SaveData>();
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.Default.GetBytes(win.GetType().FullName+win.Title));
            string winFileName = string.Join("", md5.Hash.Select(b => (b).ToString()));

            var path = Path.Combine(BaseApp.AppTempFilePath,
                  @"HistoryData\" + winFileName + ".txt");
            if (File.Exists(path))
            {
                string str = File.ReadAllText(path, Encoding.Default);
                datas = SerializerUnit.JsonToObj<List<SaveData>>(str);
                SetControlData(win, datas);
            }
        }
        /// <summary>
        /// 设置控件数据
        /// </summary>
        public static void SetControlData(this FrameworkElement element, Window win,out List<SaveData> datas)
        {
            datas = new List<SaveData>();
            MD5 md5 = new MD5CryptoServiceProvider();
            string addInfo = element.Name.Equals("") ? element.GetType().FullName : string.Format("_" + element.Name);
            md5.ComputeHash(Encoding.Default.GetBytes(win.GetType().FullName + win.Title + addInfo));
            string winFileName = string.Join("", md5.Hash.Select(b => (b).ToString()));

            var path = Path.Combine(BaseApp.AppTempFilePath,
                  @"HistoryData\" + winFileName + ".txt");
            if (File.Exists(path))
            {
                string str = File.ReadAllText(path, Encoding.Default);
                datas = SerializerUnit.JsonToObj<List<SaveData>>(str);
                SetControlData(element, datas);
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="datas"></param>
        private static void GetControlData(Visual visual, List<SaveData> datas)
        {

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (childVisual != null)
                {
                    if (childVisual is TextBox)
                    {
                        var txt = childVisual as TextBox;
                        if ("".Equals(txt.Name) || txt.Name.Contains("_"))
                            continue;
                        datas.Add(new SaveData("", "TextBox", txt.Name, txt.Text));
                    }
                    else if (childVisual is ComboBox)
                    {
                        var cmb = childVisual as ComboBox;
                        if ("".Equals(cmb.Name) || cmb.Name.Contains("_"))
                            continue;
                        datas.Add(new SaveData("", "ComboBox", cmb.Name, cmb.Text));
                    }
                    else if (childVisual is RadioButton)
                    {
                        var cmb = childVisual as RadioButton;
                        if ("".Equals(cmb.Name) || cmb.Name.Contains("_"))
                            continue;
                        datas.Add(new SaveData("", "RadioButton", cmb.Name, cmb.IsChecked.ToString()));
                    }
                    else if (childVisual is CheckBox)
                    {
                        var cmb = childVisual as CheckBox;
                        if ("".Equals(cmb.Name) || cmb.Name.Contains("_"))
                            continue;
                        datas.Add(new SaveData("", "CheckBox", cmb.Name, cmb.IsChecked.ToString()));
                    }
                    else if (childVisual is ListBox)
                    {
                        var cmb = childVisual as ListBox;
                        if ("".Equals(cmb.Name) || cmb.Name.Contains("_"))
                            continue;
                        datas.Add(new SaveData("", "ListBox", cmb.Name, cmb.SelectedIndex.ToString()));
                    }
                    else if (visual is UserControl&&visual is IControlDataProvider)
                    {
                        
                        Control control = visual as Control;
                        if ("".Equals(control.Name) || control.Name.Contains("_"))
                            continue;
                        IControlDataProvider g = visual as IControlDataProvider;
                        datas.Add(new SaveData("", "IControlDataProvider", control.Name, g.GetControlData()));
                       
                    }

                    GetControlData(childVisual, datas);
                }
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="datas"></param>
        private static void SetControlData(Visual visual, List<SaveData> datas)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual childVisual = (Visual) VisualTreeHelper.GetChild(visual, i);
                if (childVisual != null)
                {
                    if (childVisual is TextBox)
                    {
                        var txt = childVisual as TextBox;
                        if (txt.Name != null && !"".Equals(txt.Name))
                        {
                            var data = datas.FirstOrDefault(t => t.ControlId == txt.Name) ?? new SaveData();
                            if (data.ControlValue.IsNullOrEmptyImpl())
                            {
                                txt.SetValue(TextBox.TextProperty, data.ControlValue);
                                //txt.Text = data.ControlValue;
                            }


                        }

                    }
                    else if (childVisual is ComboBox)
                    {
                        var cmb = childVisual as ComboBox;
                        if (cmb.Name != null && !"".Equals(cmb.Name))
                        {
                            var data = datas.FirstOrDefault(t => t.ControlId == cmb.Name) ?? new SaveData();
                            if (data.ControlValue.IsNullOrEmptyImpl())
                            {
                                cmb.Text = data.ControlValue;
                                if (cmb.SelectedIndex == -1 && cmb.Items.Count > 0 && !cmb.IsEditable)
                                {
                                    cmb.SelectedIndex = 0;
                                }

                                // cmb.Text = data.ControlValue;
                            }

                        }
                    }
                    else if (childVisual is RadioButton)
                    {
                        var cmb = childVisual as RadioButton;
                        var data = datas.FirstOrDefault(t => t.ControlId == cmb.Name);
                        if (data != null && cmb.Name != null && data.ControlValue.IsNullOrEmptyImpl())
                        {
                            bool isCheck = ConvertToBool(data.ControlValue);
                            if (isCheck) //如果是false则不给相应radio赋值，避免不必要的界面交互造成的bug
                                cmb.IsChecked = isCheck;
                        }

                    }
                    else if (childVisual is CheckBox)
                    {
                        var cmb = childVisual as CheckBox;
                        var data = datas.FirstOrDefault(t => t.ControlId == cmb.Name);
                        if (data != null && cmb.Name != null && data.ControlValue != "")
                            cmb.IsChecked = ConvertToBool(data.ControlValue);
                    }
                    else if (childVisual is ListBox)
                    {
                        var cmb = childVisual as ListBox;
                        var data = datas.FirstOrDefault(t => t.ControlId == cmb.Name);
                        if (data != null && cmb.Name != null && data.ControlValue != "")
                        {
                            int index = 0;
                            if (int.TryParse(data.ControlValue, out index))
                            {
                                if (cmb.Items.Count > i)
                                {
                                    cmb.SelectedIndex = index;
                                }
                            }
                        }
                    }
                    else if (visual is UserControl && visual is IControlDataProvider)
                    {

                        Control control = visual as Control;
                        var data = datas.FirstOrDefault(t => t.ControlId == control.Name);
                        if (data != null && control.Name != null && data.ControlValue != "")
                        {
                            IControlDataProvider g = visual as IControlDataProvider;
                            g.SetControlData(data.ControlValue);
                        }
                    }
                    else
                    {
                        SetControlData(childVisual, datas);
                    }

                }
            }
        }

        private static bool ConvertToBool(string str)
        {
            if (Regex.IsMatch(str, @"True|False"))
            {
                return Convert.ToBoolean(str);
            }
            else
            {
                return false;
            }
        }
    }

    [DataContract]
    public class SaveData
    {
        public SaveData() { }

        public SaveData(string name, string type, string controlId, string value)
        {
            FormName = name;
            ControlType = type;
            ControlId = controlId;
            ControlValue = value;
        }
        [DataMember]
        public string FormName { get; set; }
        [DataMember]
        public string ControlId { get; set; }

        [DataMember]
        public string ControlType { get; set; }
        [DataMember]
        public string ControlValue { get; set; }
    }
}
