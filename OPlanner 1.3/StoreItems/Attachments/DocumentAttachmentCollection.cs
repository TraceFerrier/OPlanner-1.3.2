using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PlannerNameSpace
{
    public class DocumentAttachmentCollection : ItemAttachmentCollection<DocumentAttachment>
    {
        public DocumentAttachmentCollection(StoreItem item)
            : base(item)
        {

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Saves all the documents in this collection as RichTextFileAttachments.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void SaveAll()
        {
            if (Attachments.Count > 0)
            {
                foreach (KeyValuePair<string, DocumentAttachment> kvp in Attachments)
                {
                    string propName = kvp.Key;
                    DocumentAttachment itemDocument = kvp.Value;

                    if (itemDocument.IsDirty)
                    {
                        Item.Store.SaveRichTextFileAttachment(Item.DSItem, propName, itemDocument.AttachmentStream);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of the given property as a document represented by a stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        public MemoryStream GetDocumentValue(string propName)
        {
            if (!Attachments.ContainsKey(propName))
            {
                MemoryStream mStream = Item.Store.GetRichTextFileAttachmentStream(Item, propName);

                if (mStream != null)
                {
                    Attachments.Add(propName, new DocumentAttachment { AttachmentStream = mStream });
                }
                else
                {
                    return null;
                }
            }

            return Attachments[propName].AttachmentStream;
        }

    }
}
