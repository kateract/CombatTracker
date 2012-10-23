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
        //fields and properties
        private readonly Brush CUR_INIT_BG_COLOR = Brushes.AliceBlue;

        private int _curpos;

        private bool _dropOnControl;

        private int _curInitiative;

        public int CurInitiative
        {
            get { return _curInitiative; }
            set { _curInitiative = value; }
        }

        public List<Combatant> combatantList = new List<Combatant>();

        private Combatant _displayedCombatant;

        //window specific event handlers
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

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurInitiative = -1;
            ClearCombatantDisplay();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = sender as MenuItem;
            switch (m.Header.ToString())
            {
                case "Open":
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.DefaultExt = "dnd4e";
                    ofd.Multiselect = true;
                    if (ofd.ShowDialog() == true)
                    {
                        foreach (string item in ofd.FileNames)
                        {
                            PlayerCharacter pc = PlayerCharacter.Load(item);
                            addCombatant(pc);
                        }

                    }
                    break;
                case "Clear":
                    lbInitiative.Items.Clear();
                    lbHolding.Items.Clear();
                    lbReadied.Items.Clear();
                    combatantList.Clear();
                    ClearCombatantDisplay();
                    break;
                case "Exit":
                    this.Close();
                    break;

            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //do some cleanup
            foreach (Combatant item in combatantList)
            {
                CombatantControl c = item.GetCombatantControl();
                removeCombatantControl(c);
            }
            combatantList.Clear();
        }

        ~MainWindow()
        {
            //get rid of the circular dependancy created by the lb property
            foreach (Combatant item in combatantList)
            {
                CombatantControl c = item.GetCombatantControl();
                removeCombatantControl(c);
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

        //drag and drop event handlers
        void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox lb = sender as ListBox;
            CombatantControl cmb = e.Data.GetData("Object") as CombatantControl;
            ListBox lbp = cmb.lb;
            if (lbp != lb)
            {
                if (lbp != null)
                    removeCombatantControl(cmb);
                if (_dropOnControl && _curpos < lb.Items.Count)
                    lb.Items.Insert(_curpos, cmb);
                else
                    lb.Items.Insert(lb.Items.Count, cmb);

                cmb.lb = lb;
                e.Effects = DragDropEffects.Move;
                _dropOnControl = false;

                for (int i = 0; i < lb.Items.Count; i++)
                {
                    ((CombatantControl)lb.Items[i]).Position = i;
                }
                cmb.lb.SelectedIndex = cmb.Position;
                cmb.lb.Focus();
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
                c.lb.SelectedIndex = c.Position;
                DataObject data = new DataObject();
                data.SetData(DataFormats.StringFormat, c.CombatantName);
                data.SetData("Object", c);
                DragDrop.DoDragDrop(c,
                data, DragDropEffects.Move);
            }
        }

        //Focus and Selection Event Handlers
        private void FocusChange(object sender, RoutedEventArgs e)
        {
            CombatantList_SelectionChanged(sender, e);
        }

        private void CombatantList_SelectionChanged(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex >= 0)
            {
                if (lb.SelectedIndex < lb.Items.Count)
                {
                    CombatantControl cc = lb.SelectedItem as CombatantControl;
                    Combatant c = combatantList[cc.combatantListIndex];
                    DisplayCombatant(c);
                }
                else
                    lb.SelectedIndex--;
            }
        }

        //Combatant Manipulation event handlers
        void cc_DeleteRequested(object sender, EventArgs e)
        {
            CombatantControl cc = sender as CombatantControl;
            removeCombatantControl(cc);
        }



        //combatant Manipulation
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

        void removeCombatantControl(CombatantControl c)
        {
            if (c.lb == lbInitiative)
            {
                if (c.Position == CurInitiative)
                {
                    c.rectangle1.Fill = Brushes.Transparent;
                }
                else if (c.Position < CurInitiative || lbInitiative.Items.Count == 1) CurInitiative--;

            }
            c.lb.Items.RemoveAt(c.Position);
            for (int i = c.Position; i < c.lb.Items.Count; i++)
            {
                CombatantControl d = c.lb.Items[i] as CombatantControl;
                d.Position--;
                if (i == CurInitiative)
                {
                    d.rectangle1.Fill = CUR_INIT_BG_COLOR;
                }
            }
            c.lb = null;
        }

        private void ClearCombatantDisplay()
        {
            this.lbAttributes.Items.Clear();
            this.lbPowers.Items.Clear();
            this.lblName.Content = "No Combatant Selected";
            this.lblCurrentHP.Content = "";
            this.lblMaxHP.Content = "";
            this.lblTempHP.Content = "";
            this.lblCurrentSurges.Content = "";
            this.lblMaxSurges.Content = "";
            _displayedCombatant = null;
            this.btnDamage.IsEnabled = false;
            this.btnDamage1.IsEnabled = false;
            this.btnDamage10.IsEnabled = false;
            this.btnDamage20.IsEnabled = false;
            this.btnDamage5.IsEnabled = false;
            this.btnHeal.IsEnabled = false;
            this.btnSetTemps.IsEnabled = false;
            this.cbxAddSurgeValue.IsEnabled = false;
            this.cbxSpendSurge.IsEnabled = false;
            this.btnAddEffect.IsEnabled = false;

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
            this.lblName.Content = c.CombatantName;
            this.lblCurrentHP.Content = c.CurrentHP;
            this.lblMaxHP.Content = c.MaxHP;
            this.lblTempHP.Content = c.CurrentTempHP;
            this.lblCurrentSurges.Content = c.CurrentHealingSurges;
            this.lblMaxSurges.Content = c.MaxHealingSurges;
            this.btnDamage.IsEnabled = true;
            this.btnDamage1.IsEnabled = true;
            this.btnDamage10.IsEnabled = true;
            this.btnDamage20.IsEnabled = true;
            this.btnDamage5.IsEnabled = true;
            this.btnHeal.IsEnabled = true;
            this.btnSetTemps.IsEnabled = true;
            this.cbxAddSurgeValue.IsEnabled = true;
            this.cbxSpendSurge.IsEnabled = true;
            this.btnAddEffect.IsEnabled = true;
            _displayedCombatant = c;
        }

        private void btnInitiativeNext_Click(object sender, RoutedEventArgs e)
        {
            if (CurInitiative < 0)
            {
                if (lbInitiative.Items.Count > 0)
                {
                    CurInitiative = 0;
                    CombatantControl cc = lbInitiative.Items[CurInitiative] as CombatantControl;
                    cc.rectangle1.Fill = CUR_INIT_BG_COLOR;
                    btnInitiativeNext.Content = "Next";
                }
            }
            else
            {
                CombatantControl cc = lbInitiative.Items[CurInitiative] as CombatantControl;
                cc.rectangle1.Fill = Brushes.Transparent;
                CurInitiative++;
                if (CurInitiative < lbInitiative.Items.Count)
                {
                    cc = lbInitiative.Items[CurInitiative] as CombatantControl;
                    cc.rectangle1.Fill = CUR_INIT_BG_COLOR;
                }
                else
                {
                    CurInitiative = -1;
                    btnInitiativeNext.Content = "Start";
                }
            }
        }

        private void DamageCombatant(Combatant c, int amount)
        {
            c.DamageCombatant(amount);
            lblCurrentHP.Content = c.CurrentHP;
            lblTempHP.Content = c.CurrentTempHP;
        }

        private void btnDamage_Click(object sender, RoutedEventArgs e)
        {
            int d;
            if (int.TryParse(txtDamage.Text, out d))
                DamageCombatant(_displayedCombatant, d);
            else
                txtDamage.Clear();
        }

        private void btnDamage1_Click(object sender, RoutedEventArgs e)
        {
            DamageCombatant(_displayedCombatant, 1);
        }

        private void btnDamage5_Click(object sender, RoutedEventArgs e)
        {
            DamageCombatant(_displayedCombatant, 5);
        }

        private void btnDamage10_Click(object sender, RoutedEventArgs e)
        {
            DamageCombatant(_displayedCombatant, 10);
        }

        private void btnDamage20_Click(object sender, RoutedEventArgs e)
        {
            DamageCombatant(_displayedCombatant, 20);
        }

        private void btnSetTemps_Click(object sender, RoutedEventArgs e)
        {
            int t;
            if (int.TryParse(txtTemps.Text, out t))
                SetTemps(_displayedCombatant, t);
            else
                txtTemps.Clear();
        }

        private void SetTemps(Combatant c, int t)
        {
            c.SetTemps(t);
            lblTempHP.Content = c.CurrentTempHP;
            lblCurrentHP.Content = c.CurrentHP;
        }

        private void btnHeal_Click(object sender, RoutedEventArgs e)
        {
            int h;
            if (int.TryParse(txtHeal.Text, out h))
                HealCombatant(_displayedCombatant, h, cbxAddSurgeValue.IsChecked, cbxSpendSurge.IsChecked);
            else if ((bool)cbxAddSurgeValue.IsChecked)
                HealCombatant(_displayedCombatant, 0, cbxAddSurgeValue.IsChecked, cbxSpendSurge.IsChecked);
            else
                txtHeal.Clear();
        }

        private void HealCombatant(Combatant c, int amount, bool? addSurgeValue, bool? spendHealingSurge)
        {
            bool surge, spend;
            surge = (addSurgeValue == null) ? true : (bool)addSurgeValue;
            spend = (spendHealingSurge == null) ? true : (bool)spendHealingSurge;
            c.HealCombatant(amount, surge, spend);
            lblTempHP.Content = c.CurrentTempHP;
            lblCurrentHP.Content = c.CurrentHP;
            lblCurrentSurges.Content = c.CurrentHealingSurges;
        }
    }
}