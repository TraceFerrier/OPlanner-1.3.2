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
    /// Interaction logic for ScenarioItemEditorDialog.xaml
    /// </summary>
    public partial class ScenarioItemEditorDialog : Window
    {
        ScenarioItem TargetScenarioItem { get; set; }
        bool DescriptionChanged { get; set; }

        public ScenarioItemEditorDialog(ScenarioItem scenarioItem)
        {
            InitializeComponent();

            this.Owner = Globals.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            RichTextContext.CurrentItemEditorWindow = this;

            DialogContext.DataContext = scenarioItem;
            DescriptionControl.ParentWindow = this;

            TargetScenarioItem = scenarioItem;
            TargetScenarioItem.LoadDocumentIntoRichTextBox(Datastore.PropNameDescriptionDocument, null, DescriptionControl.PlannerRichTextBox);
            DescriptionControl.PlannerRichTextBox.Tag = new RichTextContext { StoreItem = TargetScenarioItem, HeaderText = "Scenario Description" };
            DescriptionControl.PlannerRichTextBox.TextChanged += DescriptionCriteriaBox_TextChanged;
        }

        void DescriptionCriteriaBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DescriptionChanged = true;
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (DescriptionChanged)
            {
                string plainText = TargetScenarioItem.SaveDocumentFromRichTextBox(Datastore.PropNameDescriptionDocument, DescriptionControl.PlannerRichTextBox);
                TargetScenarioItem.Description = plainText;
            }

            TargetScenarioItem.NotifyPropertyChanged(()=> TargetScenarioItem.TestComments);
            TargetScenarioItem.NotifyPropertyChanged(()=> TargetScenarioItem.ResearchComments);
            Close();

        }
    }
}
