using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace CombatTracker
{
    class PlayerCharacter : Combatant
    {
        public static PlayerCharacter Load(string filename)
        {

            PlayerCharacter pc = new PlayerCharacter();
            
            FileInfo fi = new FileInfo(filename);
            if (fi.Exists)
            {
                attribute at = new attribute();
                    
                XmlTextReader xtr = new XmlTextReader(fi.OpenRead());
                while ( xtr.Read())
                {
                    switch (xtr.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xtr.Name == "Stat")
                            {
                                at.value = int.Parse(xtr.GetAttribute("value"));
                            }
                            if (xtr.Name == "alias")
                            {
                                at.alias.Add(xtr.GetAttribute("name"));
                            }
                            if (xtr.Name == "Details")
                            {
                                xtr.Read();
                                while (xtr.Name != "Details")
                                {
                                    if (xtr.Name == "name")
                                    {
                                        xtr.Read();
                                        if (xtr.NodeType == XmlNodeType.Text)
                                        {
                                            pc.CombatantName = xtr.Value;
                                        }
                                    }
                                    xtr.Read();
                                }

                            }
                            if (xtr.Name == "RulesElement")
                            {
                                if (xtr.GetAttribute("type") == "Class")
                                    pc.PlayerClass = xtr.GetAttribute("name");
                                if (xtr.GetAttribute("type") == "Race")
                                    pc.PlayerRace = xtr.GetAttribute("name");
                            }

                            if (xtr.Name == "Power")
                            {
                                power p = new power();
                                p.name = xtr.GetAttribute("name");
                                xtr.Read();
                                while (xtr.Name != "Power")
                                {
                                    if (xtr.GetAttribute("name") == "Power Usage")
                                    {
                                        xtr.Read();
                                        if (xtr.NodeType == XmlNodeType.Text)
                                        {
                                            string s = xtr.Value.Trim();
                                            switch (s[0])
                                            {
                                                case 'A':
                                                    p.PowerUsage = power.PowerUsageType.ATWILL;
                                                    break;
                                                case 'E':
                                                    p.PowerUsage = power.PowerUsageType.ENCOUNTER;
                                                    break;
                                                case 'D':
                                                    p.PowerUsage = power.PowerUsageType.DAILY;
                                                    break;
                                                default:
                                                    throw new NotImplementedException(s);
                                            }
                                        }
                                    }
                                    if (xtr.GetAttribute("name") == "Action Type")
                                    {
                                        xtr.Read();
                                        string s = xtr.Value.Trim();
                                        if (s.Length > 0)
                                        {
                                            switch (s.Trim().Substring(0, s.IndexOf(' ')))
                                            {
                                                case "Move":
                                                    p.action = ActionType.MOVE;
                                                    break;
                                                case "Minor":
                                                    p.action = ActionType.MINOR;
                                                    break;
                                                case "Standard":
                                                    p.action = ActionType.STANDARD;
                                                    break;
                                                case "Immediate":
                                                    string y = s.Substring(s.Trim().IndexOf(' '), s.Trim().Substring(0, s.IndexOf(' ')).IndexOf(' ') >= 0 ? s.Trim().Substring(0, s.IndexOf(' ')).IndexOf(' ') : s.Trim().Substring(0, s.IndexOf(' ')).Length).Trim();
                                                    if (y.Substring(0,3) == "Int")
                                                        p.action = ActionType.INTERRUPT;
                                                    else if (y.Substring(0,3) == "Rea")
                                                        p.action = ActionType.REACTION;
                                                    else
                                                        throw new NotImplementedException(s);
                                                    break;
                                                default:
                                                    throw new NotImplementedException(s);

                                            }
                                        }
                                    }
                                    xtr.Read();
                                }
                                pc.Powers.Add(p);

                            }


                            break;
                        case XmlNodeType.EndElement:
                            if (xtr.Name == "Stat")
                            {
                                pc.attList.Add(at);
                                at = new attribute();
                            }
                            break;

                    }
                
                }

                


            }

            foreach (attribute item in pc.attList)
            {
                if (item.att_name == "Hit Points")
                {
                    pc.MaxHP = item.value;
                    pc.CurrentHP = item.value;
                }
                if (item.att_name == "Healing Surges")
                {
                    pc.MaxHealingSurges = item.value;
                    pc.CurrentHealingSurges = item.value;
                }
            }

            return pc;
        }
    }
}
