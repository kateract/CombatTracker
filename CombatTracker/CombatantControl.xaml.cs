using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CombatTracker
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CombatantControl : UserControl
    {
        private int position;
        public int Position
        {
            get { return position; }
            set
            { 
                position = value;
            }
        }
        
        private string _name;
        public string CombatantName {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                this.lblName.Content = _name;
            }
        }
        
        public ListBox lb;
        public CombatantControl()
        {
            InitializeComponent();
            this.Background = Brushes.White;
            this.BorderBrush = Brushes.Black;
            this.canvas1.Background = Brushes.RosyBrown;
        }
        public CombatantControl(int num) : this()
        {
            CombatantName = "Combatant " + num.ToString();
            position = num;

            
        }
    }
}
