using MCTG.Cards.MonsterCards;
using MCTG.Cards.SpellCards;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace MCTG.Cards
{
    public static class CardHelpers
    {
        public static List<Card> PackageArrayToListCards(string JsonPackageString)
        {
            List<Card> Cardlist = new List<Card>();
            var items = JsonConvert.DeserializeObject<JsonPackages[]>(JsonPackageString);
            foreach (var cardprepare in items)
            {
                Card.CardelEmentEnum type;
                string CardK;

                if (Regex.IsMatch(cardprepare.Name, "^(Fire).*$"))
                {
                    type = Card.CardelEmentEnum.fire;
                    CardK = Regex.Match(cardprepare.Name, "Fire(.*)$").Groups[1].Value;

                }
                else if (Regex.IsMatch(cardprepare.Name, "^(Water).*$"))
                {
                    type = Card.CardelEmentEnum.water;
                    CardK = Regex.Match(cardprepare.Name, "Water(.*)$").Groups[1].Value;
                }
                else if (Regex.IsMatch(cardprepare.Name, "^(Regular).*$"))
                {
                    type = Card.CardelEmentEnum.normal;
                    CardK = Regex.Match(cardprepare.Name, "Regular(.*)$").Groups[1].Value;
                }
                else
                {
                    continue;
                }
                Cardlist.Add(Cardmaker((int)cardprepare.Damage, cardprepare.Name, cardprepare.ID, type, CardK));
            }
            return Cardlist;
        }
        public static Card Cardmaker(int Damage, string Name, string ID, Card.CardelEmentEnum ElemetnType, string whatcard)
        {
            if (whatcard == "Spell")
            {
                if (ElemetnType == Card.CardelEmentEnum.fire)
                {
                    return new FireSpell((int)Damage, Name, ID, ElemetnType);
                }
                else if (ElemetnType == Card.CardelEmentEnum.water)
                {
                    return new WaterSpell((int)Damage, Name, ID, ElemetnType);
                }
                else
                {
                    return new NormalSpell((int)Damage, Name, ID, ElemetnType);
                }
            }
            else
            {
                if (whatcard == "Dragon")
                {
                    return new Dragon((int)Damage, Name, ID, ElemetnType);
                }
                else if (whatcard == "Elf" || whatcard == "Elve" || whatcard == "FireElf" || whatcard == "FireElve")
                {
                    return new FireElve((int)Damage, Name, ID, ElemetnType);
                }
                else if (whatcard == "Goblin")
                {
                    return new Goblin((int)Damage, Name, ID, ElemetnType);
                }
                else if (whatcard == "Knight")
                {
                    return new Knight((int)Damage, Name, ID, ElemetnType);
                }
                else if (whatcard == "Kraken")
                {
                    return new Kraken((int)Damage, Name, ID, ElemetnType);
                }
                else if (whatcard == "Ork")
                {
                    return new Ork((int)Damage, Name, ID, ElemetnType);
                }
                else if (whatcard == "Wizzard")
                {
                    return new Wizzard((int)Damage, Name, ID, ElemetnType);
                }
                Console.WriteLine("could not create " + ID);
                return null;
            }
        }

    }
}
