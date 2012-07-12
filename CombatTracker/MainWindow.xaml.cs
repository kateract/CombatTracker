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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _curpos;
        private bool _dropOnControl;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded +=new RoutedEventHandler(MainWindow_Loaded);
            this.listBox2.Drop += new DragEventHandler(listBox2_Drop);
            this.listBox1.Drop += new DragEventHandler(listBox2_Drop);
            this.listBox3.Drop += new DragEventHandler(listBox2_Drop);
        }

        void listBox2_Drop(object sender, DragEventArgs e)
        {
            ListBox lb = sender as ListBox;
            CombatantControl cmb = e.Data.GetData("Object") as CombatantControl;
            ListBox lbp = cmb.lb;
            if (lb == lbp)
            {
                lb.Items.Remove(cmb);
                lb.Items.Insert(_curpos, cmb);
            }
            else
            {
                lbp.Items.Remove(cmb);
                if (_dropOnControl && _curpos > lb.Items.Count)
                    lb.Items.Insert(lb.Items.Count,cmb);
                else
                    lb.Items.Insert(_curpos,cmb);
                cmb.lb = lb;
                e.Effects = DragDropEffects.Move;
                
            }
            for (int i = 0; i < lb.Items.Count; i++)
            {
                ((CombatantControl)lb.Items[i]).Position = i;
            }
            for (int i = 0; i < lbp.Items.Count; i++)
            {
                ((CombatantControl)lbp.Items[i]).Position = i;
            }
        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 10; i++)
            {

                CombatantControl c = new CombatantControl(i);
                c.MouseMove += new MouseEventHandler(c_MouseMove);
                c.PreviewDrop += new DragEventHandler(c_PreviewDrop);
                c.lb = listBox1;
                c.Position = listBox1.Items.Count;
                this.listBox1.Items.Add(c);
                
            }
        }

        void c_PreviewDrop(object sender, DragEventArgs e)
        {
            CombatantControl c = sender as CombatantControl;
            _curpos = c.Position;
            _dropOnControl = true;
        }

        void c_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        void c_MouseMove(object sender, MouseEventArgs e)
        {
            CombatantControl c = sender as CombatantControl;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData(DataFormats.StringFormat, c.CombatantName);
                data.SetData("Object", c);
                DragDrop.DoDragDrop(c,
                data, DragDropEffects.Move);
            }
        }
    }
}
