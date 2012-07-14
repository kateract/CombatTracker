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
            this.lbInitiative.Drop += new DragEventHandler(ListBox_Drop);
            this.lbHolding.Drop += new DragEventHandler(ListBox_Drop);
            this.lbReadied.Drop += new DragEventHandler(ListBox_Drop);
            this.lbInitiative.SelectionChanged += new SelectionChangedEventHandler(CombatantList_SelectionChanged);
            this.lbHolding.SelectionChanged += new SelectionChangedEventHandler(CombatantList_SelectionChanged);
            this.lbReadied.SelectionChanged += new SelectionChangedEventHandler(CombatantList_SelectionChanged);
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.lbInitiative.GotFocus += new RoutedEventHandler(FocusChange);
            this.lbHolding.GotFocus += new RoutedEventHandler(FocusChange);
            this.lbReadied.GotFocus += new RoutedEventHandler(FocusChange);
        }

        private void FocusChange(object sender, RoutedEventArgs e)
        {
            CombatantList_SelectionChanged(sender, e);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //do some cleanup
            foreach (Combatant item in combatantList)
            {
                CombatantControl c = item.GetCombatantControl();
                removeCombatant(c);
            }
            combatantList.Clear();
        }
        ~MainWindow()
        {
            //get rid of the circular dependancy created by the lb property
            foreach (Combatant item in combatantList)
            {
                CombatantControl c = item.GetCombatantControl();
                removeCombatant(c);
            }
            try
            {
                combatantList.Clear();
                lbReadied.Items.Clear();
                lbHolding.Items.Clear();
                lbInitiative.Items.Clear();
                lbPowers.Items.Clear();
                lbAttributes.Items.Clear();
            }
            catch
            {
 
            }
           
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
            cmb.lb.SelectedIndex = cmb.Position;
            cmb.lb.Focus();
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
            cc.lb = lbInitiative;
            cc.Position = lbInitiative.Items.Count;
            cc.combatantListIndex = combatantList.Count;
            this.lbInitiative.Items.Add(cc);
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
            for (int i = c.Position; i < c.lb.Items.Count; i++)
            {
                CombatantControl d = c.lb.Items[i] as CombatantControl;
                d.Position--;
            }
            c.lb = null;
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
                c.lb.SelectedIndex = c.Position;
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

        private void CombatantList_SelectionChanged(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex >= 0)
            {

                CombatantControl cc = lb.SelectedItem as CombatantControl;
                Combatant c = combatantList[cc.combatantListIndex];
                DisplayCombatant(c);
            }
        }

        private void DisplayCombatant(Combatant c)
        {
            this.lbAttributes.Items.Clear();
            foreach (Combatant.attribute item in c.attList)
            {
                if (item.att_name.IndexOf(' ') < 0 && item.att_name.IndexOf('-') < 0 && item.att_name.IndexOf('_') < 0 || item.att_name.EndsWith("Defense"))
                    this.lbAttributes.Items.Add(item.att_name + " " + item.value.ToString());
            }
            this.lbPowers.Items.Clear();
            foreach (Combatant.power item in c.Powers)
            {
                int num = lbPowers.Items.Count;
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = item.name + " " + item.action.ToString();
                switch (item.PowerUsage)
                {
                    case Combatant.power.PowerUsageType.ATWILL:
                        lbi.Background = Brushes.Green;
                        lbi.Foreground = Brushes.White;
                        break;
                    case Combatant.power.PowerUsageType.ENCOUNTER:
                        lbi.Background = Brushes.Red;
                        lbi.Foreground = Brushes.White;
                        break;
                    case Combatant.power.PowerUsageType.DAILY:
                        lbi.Background = Brushes.Black;
                        lbi.Foreground = Brushes.White;
                        break;
                    default:
                        break;
                }
                lbPowers.Items.Add(lbi);
            }
        }
    }
}