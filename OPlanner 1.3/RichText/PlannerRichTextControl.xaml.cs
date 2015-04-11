using System;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace PlannerNameSpace.UserControls
{
    /// <summary>
    /// Interaction logic for RichTextToolBar.xaml
    /// </summary>
    public partial class PlannerRichTextControl : UserControl
    {
        public PlannerRichTextControl()
        {
            InitializeComponent();
            PlannerRichTextBox.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(HandleRequestNavigate));
            PlannerRichTextBox.SelectionChanged += PlannerRichTextBox_SelectionChanged;
        }

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "HeaderText", typeof(string), typeof(PlannerRichTextControl),
            new FrameworkPropertyMetadata());

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public Window ParentWindow { get; set; }

        private static void HandleRequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            string url = args.Uri.ToString();

            try
            {
                Process.Start(url);
            }

            catch (Exception)
            {
                UserMessage.Show("Unable to navigate to the given URL: " + url);
            }

        }

        void PlannerRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(PlannerRichTextBox.Selection.Start, PlannerRichTextBox.Selection.End);
            CreateHyperlinkButton.IsEnabled = !range.IsEmpty;
        }

        private void CreateHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox richTextBox = PlannerRichTextBox;
            TextRange range = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);

            string hyperlinkText = null;
            if (!range.IsEmpty)
            {
                hyperlinkText = range.Text;
            }

            EditHyperlink dialog = new EditHyperlink(ParentWindow, hyperlinkText);
            dialog.ShowDialog();
            if (!dialog.IsCancelled)
            {
                Hyperlink link = new Hyperlink(range.Start, range.End);
                link.NavigateUri = new System.Uri(dialog.HyperlinkAddress);
            }
        }

        private void UndockButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextEditDialog dialog = new RichTextEditDialog(RichTextContext.CurrentItemEditorWindow, PlannerRichTextBox);
            dialog.Closed += dialog_Closed;
            dialog.ShowDialog();
        }

        void dialog_Closed(object sender, EventArgs e)
        {
            RichTextEditDialog dialog = sender as RichTextEditDialog;

            TextRange range = new TextRange(dialog.DialogRichTextBox.Document.ContentStart, dialog.DialogRichTextBox.Document.ContentEnd);
            MemoryStream mStream = new MemoryStream();
            range.Save(mStream, DataFormats.Rtf);

            TextRange newRange = new TextRange(PlannerRichTextBox.Document.ContentStart, PlannerRichTextBox.Document.ContentEnd);
            newRange.Load(mStream, DataFormats.Rtf);

        }
    }
}
