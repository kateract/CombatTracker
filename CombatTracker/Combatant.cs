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

        public List<attribute> attList = new List<attribute>();

        
        public enum Condition
        {
            BLOODIED,
            PRONE,
            BLIND,
            IMMOBILIZED,
            RESTRAINED,
            GRAPPLED,
            SLOWED,
            DAZED,
            STUNNED,
            UNCONCIOUS,
            DYING,
            HELPLESS
        }

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
 
    }
}
