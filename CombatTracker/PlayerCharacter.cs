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
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            //Get Character Details
            XmlNodeList details = doc.SelectNodes("//Details");
            foreach (XmlNode item in details)
            {
                pc.CombatantName = item["name"].InnerText.Trim();
            }

            //Get Powers List
            XmlNodeList powers = doc.SelectNodes("//PowerStats/Power");
            foreach (XmlNode item in powers)
            {
                power p = new power();
                p.name = item.Attributes["name"].InnerText.Trim();
                XmlNodeList spec = item.SelectNodes("specific");
                foreach (XmlNode note in spec)
                {
                    if (note.Attributes["name"].InnerText == "Power Usage")
                    {
                        switch (note.InnerText.Trim())
                        {
                            case "At-Will":
                                p.PowerUsage = power.PowerUsageType.ATWILL;
                                break;
                            case "Encounter":
                            case "Encounter (Special)":
                                p.PowerUsage = power.PowerUsageType.ENCOUNTER;
                                break;
                            case "Daily":
                                p.PowerUsage = power.PowerUsageType.DAILY;
                                break;
                            default:
                                throw new NotImplementedException(note.InnerText.Trim());
                        }
                    }
                    if (note.Attributes["name"].InnerText == "Action Type")
                    {
                        switch (note.InnerText.Trim().ToLower())
                        {
                            case "standard action":
                                p.action = ActionType.STANDARD;
                                break;
                            case "minor action":
                                p.action = ActionType.MINOR;
                                break;
                            case "move action":
                                p.action = ActionType.MOVE;
                                break;
                            case "immediate interrupt":
                                p.action = ActionType.INTERRUPT;
                                break;
                            case "immediate reaction":
                                p.action = ActionType.REACTION;
                                break;
                            default:
                                throw new NotImplementedException(note.InnerText.Trim());
                        }
                    }
                }
                pc.Powers.Add(p);
            }

            //Get Race and Class
            XmlNodeList rules = doc.SelectNodes ("//CharacterSheet/RulesElementTally/RulesElement");
            foreach (XmlNode rule in rules)
            {
                if (rule.Attributes["type"].InnerText == "Race")
                    pc.PlayerRace = rule.Attributes["name"].InnerText;
                if (rule.Attributes["type"].InnerText == "Class")
                    pc.PlayerClass = rule.Attributes["name"].InnerText;
            }
            
            //Get Stat Block
            XmlNodeList Stats = doc.SelectNodes("//CharacterSheet/StatBlock/Stat");
            foreach (XmlNode stat in Stats)
            {
                attribute a = new attribute();
                a.value = int.Parse(stat.Attributes["value"].InnerText);
                foreach (XmlNode alias in stat.SelectNodes("alias"))
                    a.alias.Add(alias.Attributes["name"].InnerText);
                pc.attList.Add(a);
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
            
            doc = null;
            return pc;
        }
    }
}
