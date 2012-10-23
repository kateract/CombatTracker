using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatTracker
{
    public class Combatant 
    {
        private string _keywords;
        public string KeyWords
        {
            get { return _keywords; }
            set
            {
                _keywords = value;
                _control.KeyWords = _keywords;
            }
        }

        private string _playerRace;
        public string PlayerRace
        {
            get { return _playerRace; }
            set
            {
                _playerRace = value;
                setCombatantKeywords();
            }
        }
        
        private string _playerClass;
        public string PlayerClass
        {
            get { return _playerClass; }
            set 
            { 
                _playerClass = value;
                setCombatantKeywords(); 
            }
        }

        private int _maxHP;
        public int MaxHP
        {
            get { return _maxHP; }
            set { 
                _maxHP = value;
                _control.MaxHP = value;
            }
        }

        private int _curHP;
        public int CurrentHP
        {
            get { return _curHP; }
            set { 
                _curHP = value;
                _control.CurrentHP = value;
                if (_curHP > BloodiedValue)
                    _control.Bloodied = false;
                else
                    _control.Bloodied = true;
            }
        }

        private int _curTempHP;
        public int CurrentTempHP
        {
            get { return _curTempHP; }
            set {
                _curTempHP = value;
            }
        }

        private string _name;
        public string CombatantName
        {
            get 
            { 
                return _name; 
            }
            set 
            {
                _name = value;
                _control.CombatantName = _name;
            }
        }

        private CombatantControl _control;
        public CombatantControl GetCombatantControl()
        {
            return _control;
        }

        private int _curHS;
        public int CurrentHealingSurges
        {
            get { return _curHS; }
            set { _curHS = value; }
        }

        private int _maxHS;
        public int MaxHealingSurges
        {
            get { return _maxHS; }
            set { _maxHS = value; }
        }

        public int BloodiedValue
        {
            get { return _maxHP / 2; }
        }

        private int _hsBonus;
        public int HealingSurgeBonus
        {
            get { return _hsBonus; }
            set { _hsBonus = value; }
        }

        public int HealingSurgeValue
        {
            get { return _hsBonus + (BloodiedValue / 2); }
        }

        public void DamageCombatant(int amount)
        {
            if (amount > CurrentTempHP)
            {
                amount -= CurrentTempHP;
                CurrentTempHP = 0;
            }
            else
            {
                CurrentTempHP -= amount;
                amount = 0;
            }
            CurrentHP -= amount;
        }

        public void SetTemps(int amount)
        {
            if (amount > CurrentTempHP)
                CurrentTempHP = amount;
        }

        public void HealCombatant(int amount, bool addSurgeValue, bool spendHealingSurge)
        {
            if (addSurgeValue)
                amount += HealingSurgeValue + HealingSurgeBonus;
            if (spendHealingSurge)
                CurrentHealingSurges--;
            if (CurrentHP < 0 && amount > 0)
                CurrentHP = 0;
            CurrentHP += amount;
            if (CurrentHP > MaxHP)
                CurrentHP = MaxHP;
        }

        public List<attribute> attList = new List<attribute>();

        public EffectList effects = new EffectList();

        public Combatant()
        {
            this._control = new CombatantControl();

        }

        private void setCombatantKeywords()
        {
            KeyWords = _playerRace + " " + _playerClass;
        }

        public class attribute : IComparable<attribute>
        {
            public string att_name
            {
                get { return alias[0]; }
            }
            public List<string> alias = new List<string>();
            public int value;

            

            #region IComparable<attribute> Members

            public int CompareTo(attribute other)
            {
                return this.att_name.CompareTo(other.att_name);
            }

            #endregion
        }
        public List<power> Powers = new List<power>();
        public class power
        {
            public string name;
            public ActionType action;
            public enum PowerUsageType
            {
                ATWILL,
                ENCOUNTER,
                DAILY
            }
            public PowerUsageType PowerUsage;
        }

        public enum ActionType
        {
            STANDARD,
            MOVE,
            MINOR,
            FREE,
            REACTION,
            INTERRUPT
        }

        public delegate void TurnEndingHandler(Combatant sender);
        public event TurnEndingHandler TurnEnding;

        public delegate void TurnStartingHandler(Combatant sender);
        public event TurnStartingHandler TurnStarting;



    }
}
