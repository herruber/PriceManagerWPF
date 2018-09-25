using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PriceManagerWPF
{
    public class WindowViewModel
    {

        public void ParseVariable()
        {

        }

        public WindowViewModel()
        {
            setDictionaries(window.textBox_base_Name, typeof(ModelData).GetProperty("Name"));
            setDictionaries(window.textBox_base_Type, typeof(ModelData).GetProperty("Type"));
        }

        public static MainWindow window = (MainWindow)Application.Current.MainWindow;

        private ModelData _item { get; set; }

        public ModelData Item {
            get { return _item; }
            set { _item = value;

                foreach (var property in typeof(ModelData).GetProperties())
                {

                    Type declaringType = property.DeclaringType;

                    Set(property, property.GetValue(_item));

                }


            }
        }

        public void Set(PropertyInfo property, object value)
        {
         

            Type type = property.PropertyType;
            Type declaringType = property.DeclaringType;

            object returnedValue = null;

            if (declaringType == typeof(SizeData))
            {
                if (type == typeof(int))
                {
                    property.SetValue(Item.SizeData, (int)value);
                }
                else if (type == typeof(double))
                {
                    property.SetValue(Item.SizeData, (double)value);
                }
                else if (type == typeof(float))
                {
                    property.SetValue(Item.SizeData, (float)value);
                }
                else if (type == typeof(string))
                {
                    property.SetValue(Item.SizeData, (string)value);
                }

                returnedValue = property.GetValue(Item.SizeData);
            }
            else if (declaringType == typeof(PriceData))
            {
                if (type == typeof(int))
                {
                    property.SetValue(Item.PriceData, (int)value);
                }
                else if (type == typeof(double))
                {
                    property.SetValue(Item.PriceData, (double)value);
                }
                else if (type == typeof(float))
                {
                    property.SetValue(Item.PriceData, (float)value);
                }
                else if (type == typeof(string))
                {
                    property.SetValue(Item.PriceData, (string)value);
                }

                returnedValue = property.GetValue(Item.PriceData);
            }
            else if (declaringType == typeof(ModelData))
            {
                if (type == typeof(int))
                {
                    property.SetValue(Item, (int)value);
                }
                else if (type == typeof(double))
                {
                    property.SetValue(Item, (double)value);
                }
                else if (type == typeof(float))
                {
                    property.SetValue(Item, (float)value);
                }
                else if (type == typeof(string))
                {
                    property.SetValue(Item, (string)value);
                }

                returnedValue = property.GetValue(Item);
            }

            UpdateUi(property, returnedValue);
        }

        public void UpdateUi(PropertyInfo property, object value)
        {

            if (DataToUi.ContainsKey(property))
            {
                var field = DataToUi[property];
                Type type = field.GetType();

                if (value != null)
                {
                    if (type == typeof(TextBox))
                    {
                        ((TextBox)field).Text = value.ToString();
                    }
                    else if (type == typeof(Slider))
                    {
                        ((Slider)field).Value = (float)value;
                    }
                }
            }
        }

        public void UpdateData(object field)
        {

            Type type = field.GetType();

            if (type == typeof(TextBox))
            {
                PropertyInfo prop = UiToData[field];

            }
            else if (type == typeof(Slider))
            {

            }

        }

        public Dictionary<object, PropertyInfo> UiToData = new Dictionary<object, PropertyInfo>();

        public Dictionary<PropertyInfo, object> DataToUi = new Dictionary<PropertyInfo, object>();

        public void setDictionaries(object field, PropertyInfo prop)
        {

            DataToUi.Add(prop, field);
            UiToData.Add(field, prop);

        }
    }
}
