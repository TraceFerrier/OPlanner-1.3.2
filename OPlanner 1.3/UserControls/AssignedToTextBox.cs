using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlannerNameSpace
{
    public class AssignedToTextBoxHandler
    {
        string CurrentText;
        Brush OriginalBackground;
        static HashSet<string> m_allowedAliases = null;
        static HashSet<string> AllowedAliases
        {
            get
            {
                if (m_allowedAliases == null)
                {
                    m_allowedAliases = HostItemStore.Instance.GetFieldAllowedValuesSet(Datastore.PropNameAssignedTo);
                }

                return m_allowedAliases;
            }
        }

        public bool IsValid()
        {
            return AllowedAliases.Contains(CurrentText);
        }

        public AssignedToTextBoxHandler(TextBox baseTextBox)
        {
            CurrentText = baseTextBox.Text;
            OriginalBackground = baseTextBox.Background;

            UpdateTextBox(baseTextBox);
            baseTextBox.TextChanged += baseTextBox_TextChanged;
        }

        void baseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            CurrentText = textBox.Text;

            UpdateTextBox(textBox);
        }

        void UpdateTextBox(TextBox textBox)
        {
            if (IsValid())
            {
                textBox.Background = OriginalBackground;
            }
            else
            {
                textBox.Background = Brushes.LightPink;
            }
        }
    }
}
