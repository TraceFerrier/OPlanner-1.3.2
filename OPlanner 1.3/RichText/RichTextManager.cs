using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;

namespace PlannerNameSpace
{
    public class LoadDocumentArgs
    {
        public StoreItem TargetStoreItem { get; set; }
        public string PropName { get; set; }
        public string PlainPropName { get; set; }
        public RichTextBox RichTextBox { get; set; }
        public MemoryStream Document { get; set; }
    }

    public class RichTextManager
    {
        public RichTextManager()
        {
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Loads the contents of the given rich text box from the backing store, using the
        /// given property name to locate the content.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void LoadDocumentIntoRichTextBox(StoreItem storeItem, string propName, string plainPropName, RichTextBox richTextBox)
        {
            SetRichTextBoxText(richTextBox, "<Loading..>");
            LoadDocumentArgs args = new LoadDocumentArgs { PropName = propName, PlainPropName = plainPropName, RichTextBox = richTextBox, TargetStoreItem = storeItem };
            BackgroundTask loadDocumentWorker = new BackgroundTask(false);
            loadDocumentWorker.DoWork += loadDocumentWorker_DoWork;
            loadDocumentWorker.TaskCompleted+=loadDocumentWorker_TaskCompleted;
            loadDocumentWorker.TaskArgs = new LoadDocumentArgs { PropName = propName, PlainPropName = plainPropName, TargetStoreItem = storeItem, RichTextBox = richTextBox };
            loadDocumentWorker.RunTaskAsync();
        }

        void loadDocumentWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            LoadDocumentArgs args = task.TaskArgs as LoadDocumentArgs;
            args.Document = args.TargetStoreItem.GetDocumentValue(args.PropName);
        }

        void loadDocumentWorker_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            LoadDocumentArgs args = TaskArgs as LoadDocumentArgs;
            if (args.Document != null)
            {
                TextRange range = range = new TextRange(args.RichTextBox.Document.ContentStart, args.RichTextBox.Document.ContentEnd);
                range.Load(args.Document, DataFormats.Rtf);
            }
            else
            {
                if (args.PlainPropName != null)
                {
                    string content = args.TargetStoreItem.GetStringValue(args.PlainPropName);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        SetRichTextBoxText(args.RichTextBox, content);
                    }
                    else
                    {
                        SetRichTextBoxText(args.RichTextBox, "");
                    }
                }
            }
        }

        void SetRichTextBoxText(RichTextBox richTextBox, string text)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            range.Text = text;
        }

    }
}
