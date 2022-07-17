using System.Windows;
using System.Windows.Controls;

namespace CalibrationTool.UserControl
{
    public partial class UC_Equation_Row : System.Windows.Controls.UserControl
    {
        public UC_Equation_Row()
        {
            InitializeComponent();
        }
        
        public string Eqiation_Num
        {
            get => (string)GetValue(Eqiation_NumProperty);
            set => SetValue(Eqiation_NumProperty, value);
        }
        
        public string Formula
        {
            get => (string)GetValue(Formula_Property);
            set => SetValue(Formula_Property, value);
        }

        public static readonly DependencyProperty Eqiation_NumProperty =
            DependencyProperty.Register("Eqiation_Num", typeof(string),
                typeof(UC_Equation_Row), new PropertyMetadata(""));
        
        public static readonly DependencyProperty Formula_Property =
            DependencyProperty.Register("Formula", typeof(string),
                typeof(UC_Equation_Row), new PropertyMetadata(""));

        public event RoutedEventHandler Click
        {
            add { DEl_button.Click += value; }
            remove { DEl_button.Click -= value; }
        }
    }
}