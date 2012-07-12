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


    }
}
