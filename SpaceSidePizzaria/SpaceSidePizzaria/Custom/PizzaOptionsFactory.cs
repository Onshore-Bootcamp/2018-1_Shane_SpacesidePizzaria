using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceSidePizzaria.Custom
{
    /// <summary>
    /// Provides access to the available options for creating/updating a pizza.
    /// </summary>
    public static class PizzaOptionsFactory
    {
        public static Dictionary<string, string> GetCrustDictionary()
        {
            Dictionary<string, string> crustDictionary = new Dictionary<string, string>();

            crustDictionary.Add("Hand Tossed", "Hand Tossed");
            crustDictionary.Add("Thin", "Thin");
            crustDictionary.Add("Flatbread", "Flatbread");

            return crustDictionary;
        }

        public static Dictionary<string, string> GetSizeDictionary()
        {
            Dictionary<string, string> sizeDictionary = new Dictionary<string, string>();

            sizeDictionary.Add("Small (6 inch)", "6");
            sizeDictionary.Add("Medium (8 inch)", "8");
            sizeDictionary.Add("Large (12 inch)", "12");

            return sizeDictionary;
        }

        public static Dictionary<string, string> GetToppingsDictionary()
        {
            Dictionary<string, string> toppingsDictionary = new Dictionary<string, string>();

            toppingsDictionary.Add("Pepperoni", "Pepperoni");
            toppingsDictionary.Add("Sausage", "Sausage");
            toppingsDictionary.Add("Bacon", "Bacon");
            toppingsDictionary.Add("Mushroom", "Mushroom");
            toppingsDictionary.Add("Onion", "Onion");
            toppingsDictionary.Add("Ham", "Ham");
            toppingsDictionary.Add("Pineapple", "Pineapple");
            toppingsDictionary.Add("Spinach", "Spinach");
            toppingsDictionary.Add("Jalapeno", "Jalapeno");
            toppingsDictionary.Add("Tomato", "Tomato");

            return toppingsDictionary;
        }

        public static Dictionary<string, string> GetSauceDictionary()
        {
            Dictionary<string, string> sauceDictionary = new Dictionary<string, string>();

            sauceDictionary.Add("Tomato Sauce", "Tomato");
            sauceDictionary.Add("Barbecue Sauce", "Barbecue");
            sauceDictionary.Add("Hummus", "Hummus");

            return sauceDictionary;
        }
    }
}