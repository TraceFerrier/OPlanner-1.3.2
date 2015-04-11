using System.Windows;
using System.Windows.Controls.Ribbon;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for HelpWinow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow(Window owner, string helpTopicTitle)
        {
            InitializeComponent();
            this.Owner = owner;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            HelpContentItem helpItem = HelpManager.Instance.GetHelpContentItem();
            if (helpItem != null)
            {
                helpItem.LoadDocumentIntoRichTextBox(helpTopicTitle, null, HelpRichTextBox);
            }
        }
    }
}
