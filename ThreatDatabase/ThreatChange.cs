using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreatDatabase
{
    public class ThreatChange
    {
        public enum ChangeType { Add, Remove, Edit }

        public static string GetChangeText(string prev, string curr)
        {
            if (prev == curr)
                return curr;

            return $"Было: {prev}\nСтало: {curr}";
        }

        public Threat Previous;
        public Threat Current;
        public ChangeType Type;

        public ThreatChange(Threat previous, Threat current)
        {
            Previous = previous;
            Current = current;

            if (Previous == null) Type = ChangeType.Add;
            else if (Current == null) Type = ChangeType.Remove;
            else Type = ChangeType.Edit;
        }

        public string NumberChange()
        {
            return GetChangeText(Previous.FullNumber, Current.FullNumber);
        }
        public string NameChange()
        {
            return GetChangeText(Previous.Name, Current.Name);

        }
        public string DiscriptionChange()
        {
            return GetChangeText(Previous.Discription, Current.Discription);
        }
        public string SourceChange()
        {
            return GetChangeText(Previous.Source, Current.Source);
        }
        public string ObjectChange()
        {
            return GetChangeText(Previous.Object, Current.Object);
        }
        public string IsPrivacyViolationChange()
        {
            return GetChangeText(Previous.IsPrivacyViolation ? "Да" : "Нет", Current.IsPrivacyViolation ? "Да" : "Нет");
        }
        public string IsIntegrityViolationCHange()
        {
            return GetChangeText(Previous.IsIntegrityViolation ? "Да" : "Нет", Current.IsIntegrityViolation ? "Да" : "Нет");
        }
        public string IsAccessibilityViolationChange()
        {
            return GetChangeText(Previous.IsAccessibilityViolation ? "Да" : "Нет", Current.IsAccessibilityViolation ? "Да" : "Нет");
        }
    }
}
