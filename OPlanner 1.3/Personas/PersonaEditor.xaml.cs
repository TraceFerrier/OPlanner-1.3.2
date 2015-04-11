using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for PersonaEditor.xaml
    /// </summary>
    public partial class PersonaEditor : Window
    {
        PersonaItem SelectedPersonaItem { get; set; }

        public PersonaEditor()
        {
            InitializeComponent();
            Globals.ApplicationManager.SetStartupLocation(this);

            PersonaEditorGrid.ItemsSource = Globals.ItemManager.PersonaItems;
            PersonaEditorGrid.SelectionChanged += PersonaEditorGrid_SelectionChanged;

            OkButton.Click += OkButton_Click;
            NewButton.Click += NewButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        void PersonaEditorGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPersonaItem = PersonaEditorGrid.SelectedItem as PersonaItem;
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPersonaItem != null)
            {
                SelectedPersonaItem.RequestDeleteItem();
            }
        }

        void NewButton_Click(object sender, RoutedEventArgs e)
        {
            PersonaItem.CreateItem();
        }
    }
}
