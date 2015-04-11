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
    /// Interaction logic for ExperienceItemEditorDialog.xaml
    /// </summary>
    public partial class ExperienceItemEditorDialog : Window
    {
        ExperienceItem TargetExperienceItem { get; set; }
        bool DescriptionChanged { get; set; }

        public ExperienceItemEditorDialog(ExperienceItem experienceItem)
        {
            InitializeComponent();

            Globals.ApplicationManager.SetStartupLocation(this);
            RichTextContext.CurrentItemEditorWindow = this;

            DescriptionChanged = false;
            DialogContext.DataContext = experienceItem;
            DescriptionControl.ParentWindow = this;

            TargetExperienceItem = experienceItem;
            TargetExperienceItem.LoadDocumentIntoRichTextBox(Datastore.PropNameDescriptionDocument, null, DescriptionControl.PlannerRichTextBox);
            DescriptionControl.PlannerRichTextBox.Tag = new RichTextContext { StoreItem = TargetExperienceItem, HeaderText = "Experience Description" };
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
                string plainText = TargetExperienceItem.SaveDocumentFromRichTextBox(Datastore.PropNameDescriptionDocument, DescriptionControl.PlannerRichTextBox);
                TargetExperienceItem.Description = plainText;
            }

            Close();

        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            TargetExperienceItem.UndoChanges();
            Close();
        }
    }
}
