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
using System.IO;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for RichTextEditDialog.xaml
    /// </summary>
    public partial class RichTextEditDialog : Window
    {
        StoreItem TargetStoreItem;
        RichTextContext RichTextContext;

        public RichTextBox DialogRichTextBox { get { return RichTextControl.PlannerRichTextBox; } }

        public RichTextEditDialog(Window parentWindow, RichTextBox richTextBox)
        {
            InitializeComponent();
            Utils.FitWindowToScreen(this);

            this.Owner = parentWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            RichTextContext = richTextBox.Tag as RichTextContext;

            TargetStoreItem = RichTextContext.StoreItem;
            DialogContext.DataContext = TargetStoreItem;

            RichTextControl.HeaderText = RichTextContext.HeaderText;
            TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            MemoryStream mStream = new MemoryStream();
            range.Save(mStream, DataFormats.Rtf);

            TextRange newRange = new TextRange(RichTextControl.PlannerRichTextBox.Document.ContentStart, RichTextControl.PlannerRichTextBox.Document.ContentEnd);
            newRange.Load(mStream, DataFormats.Rtf);
        }

        private void OKButton_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
