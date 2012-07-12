using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatTracker
{
    class Combatant 
    {
        private int _maxHP;
        public int MaxHP
        {
            get { return _maxHP; }
            set { _maxHP = value; }
        }

        private int _curHP;
        public int CurrentHP
        {
            get { return _curHP; }
            set { _curHP = value; }
        }

        private int _curTempHP;
        public int CurrentTempHP
        {
            get { return _curTempHP; }
            set { _curTempHP = value; }
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
        public int CurrentHealingSUrges
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
    }
}
