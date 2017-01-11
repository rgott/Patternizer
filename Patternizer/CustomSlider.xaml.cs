using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Patternizer
{
    /// <summary>
    /// Interaction logic for Slider.xaml
    /// </summary>
    public partial class CustomSlider : UserControl
    {
        public string SliderText
        {
            get { return (string)GetValue(SliderTextProperty); }
            set { SetValue(SliderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SliderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SliderTextProperty =
            DependencyProperty.Register("SliderText", typeof(string), typeof(CustomSlider), new PropertyMetadata(default(string)));



        public int MaximumValue
        {
            get { return (int)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("MaximumValue", typeof(int), typeof(CustomSlider), new PropertyMetadata(default(int)));



        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value);}
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CustomSlider), new PropertyMetadata(default(double)));


        public delegate void ValueChangedEventHandler(object sender, EventArgs e);

        public event ValueChangedEventHandler ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        public CustomSlider()
        {
            DataContext = this;
            
            InitializeComponent();
        }

        public CustomSlider(string SliderText, int MaximumValue) : base()
        {
            this.SliderText = SliderText;
            this.MaximumValue = MaximumValue;
        }
        double fallbackValue = 1;
        private bool UI_TextBox_Value_TextChanged()
        {
            bool handled;
            Regex regex = new Regex(@"^[0-9\.]+$"); //allowed text
            handled = (!regex.IsMatch(UI_TextBox_Value.Text));// if text allowed then false

            if (handled) return true; // if handled return

            double parsedValue;
            handled = double.TryParse(UI_TextBox_Value.Text,out parsedValue);
            fallbackValue = Math.Ceiling(parsedValue);
            return false;
        }

        private void UI_TextBox_Value_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UI_TextBox_Value_TextChanged())
            {
                Value = fallbackValue;
            }
            else
            {
                double parsedValue;
                double.TryParse(UI_TextBox_Value.Text, out parsedValue);
                Value = Math.Ceiling(parsedValue);
            }
            OnValueChanged(EventArgs.Empty);
        }

        private void UI_TextBox_Value_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if(UI_TextBox_Value_TextChanged())
            {
                Value = fallbackValue;
            }
            else
            {
                double parsedValue;
                double.TryParse(UI_TextBox_Value.Text, out parsedValue);
                Value = Math.Ceiling(parsedValue);
            }
            OnValueChanged(EventArgs.Empty);
        }

        private void UI_TextBox_Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            double parsedValue;
            double.TryParse(UI_TextBox_Value.Text, out parsedValue);
            fallbackValue = Math.Ceiling(parsedValue);
        }

        private void UI_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnValueChanged(EventArgs.Empty);
        }
    }
}
