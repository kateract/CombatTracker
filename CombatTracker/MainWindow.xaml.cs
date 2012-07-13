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
using System.IO;

namespace CombatTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _curpos;
        private bool _dropOnControl;

        public List<Combatant> combatantList = new List<Combatant>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded +=new RoutedEventHandler(MainWindow_Loaded);
            this.listBox2.Drop += new DragEventHandler(ListBox_Drop);
            this.listBox1.Drop += new DragEventHandler(ListBox_Drop);
            this.listBox3.Drop += new DragEventHandler(ListBox_Drop);
            
        }

        void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox lb = sender as ListBox;
            CombatantControl cmb = e.Data.GetData("Object") as CombatantControl;
            ListBox lbp = cmb.lb;

            lbp.Items.Remove(cmb);
            if (_dropOnControl && _curpos < lb.Items.Count)
                lb.Items.Insert(_curpos, cmb);
            else
                lb.Items.Insert(lb.Items.Count,cmb);
                
            cmb.lb = lb;
            e.Effects = DragDropEffects.Move;
            _dropOnControl = false;
            
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

        }

        void addCombatant(Combatant c)
        {
            CombatantControl cc = c.GetCombatantControl();
            cc.MouseMove += new MouseEventHandler(c_MouseMove);
            cc.PreviewDrop += new DragEventHandler(c_PreviewDrop);
            cc.DeleteRequested += new CombatantControl.DeleteRequestedHandler(cc_DeleteRequested); ;
            cc.lb = listBox1;
            cc.Position = listBox1.Items.Count;
            cc.combatantListIndex = combatantList.Count;
            this.listBox1.Items.Add(cc);
            combatantList.Add(c);
        }

        void cc_DeleteRequested(object sender, EventArgs e)
        {
            CombatantControl cc = sender as CombatantControl;
            combatantList.RemoveAt(cc.combatantListIndex);
            removeCombatant(cc);
        }



        void removeCombatant(CombatantControl c)
        {
            c.lb.Items.RemoveAt(c.Position);
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = sender as MenuItem;
            switch (m.Header.ToString())
            {
                case "Open":
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.DefaultExt = "dnd4e";
                    if (ofd.ShowDialog() == true)
                    {
                        PlayerCharacter pc = PlayerCharacter.Load(ofd.FileName);
                        addCombatant(pc);
                    }
                    break;
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                CombatantControl cc = listBox1.SelectedItem as CombatantControl;
                Combatant c = combatantList[cc.combatantListIndex];
                this.listBox4.Items.Clear();
                foreach (Combatant.attribute item in c.attList)
                {
                    this.listBox4.Items.Add(item.att_name + " " + item.value.ToString());
                }
            }
        }
    }
}
