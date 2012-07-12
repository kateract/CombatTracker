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


        public List<attribute> attList = new List<attribute>();

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

                            break;
                        case XmlNodeType.EndElement:
                            if (xtr.Name  == "Stat")
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
