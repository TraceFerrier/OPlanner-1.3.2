using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.IO;

namespace PlannerNameSpace
{
    public class ImageAttachmentCollection : ItemAttachmentCollection<ImageAttachment>
    {
        HashSet<string> BackgroundProcessByProperty;
        public ImageAttachmentCollection(StoreItem item)
            : base(item)
        {
            BackgroundProcessByProperty = new HashSet<string>();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Saves all the images in this collection as image file attachments.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void SaveAll()
        {
            if (Attachments.Count > 0)
            {
                foreach (KeyValuePair<string, ImageAttachment> kvp in Attachments)
                {
                    string propName = kvp.Key;
                    ImageAttachment itemImage = kvp.Value;

                    if (itemImage.IsDirty)
                    {
                        Item.Store.SaveImageFileAttachment(Item.DSItem, propName, itemImage.AttachmentStream);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of the given property as a document represented by a stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        public BitmapSource GetFileAttachmentImageValue(string propName, [CallerMemberName] string publicPropName = null)
        {
            if (!Attachments.ContainsKey(propName))
            {
                if (!BackgroundProcessByProperty.Contains(propName))
                {
                    BackgroundProcessByProperty.Add(propName);
                    BackgroundGetFileAttachmentImageValue(propName, publicPropName);
                }

                return null;
            }
            else
            {
                return Attachments[propName].BitmapSource;
            }
        }

        public BitmapSource GetActiveDirectoryImageValue(string alias,[CallerMemberName] string publicPropName = null)
        {
            if (!Attachments.ContainsKey(publicPropName))
            {
                if (!BackgroundProcessByProperty.Contains(publicPropName))
                {
                    BackgroundProcessByProperty.Add(publicPropName);
                    BackgroundGetActiveDirectoryImageValue(alias, publicPropName);
                }

                return null;
            }
            else
            {
                return Attachments[publicPropName].BitmapSource;
            }
        }

        void BackgroundGetActiveDirectoryImageValue(string alias, string publicPropName)
        {
            BackgroundTask getImageValueTask = new BackgroundTask(false);
            getImageValueTask.DoWork += getActiveDirectoryImageValueTask_DoWork;
            getImageValueTask.TaskCompleted += getImageValueTask_TaskCompleted;
            getImageValueTask.TaskArgs = new ImageArgs { Alias = alias,PublicPropName = publicPropName };
            getImageValueTask.RunTaskAsync();
        }

        void getActiveDirectoryImageValueTask_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            ImageArgs imageProps = task.TaskArgs as ImageArgs;

            UserInformation userInfo = new UserInformation();
            userInfo.InitializeWithAlias(imageProps.Alias);
            MemoryStream stream = userInfo.UserPicture;
            BitmapSource source = null;
            if (stream != null)
            {
                source = Utils.GetBitmapSourceFromStream(stream);
            }

            if (source != null)
            {
                ImageAttachment imageAttachment = new ImageAttachment { BitmapSource = source };
                Attachments.Add(imageProps.PublicPropName, imageAttachment);
            }
        }

        void BackgroundGetFileAttachmentImageValue(string propName, string publicPropName)
        {
            BackgroundTask getImageValueTask = new BackgroundTask(false);
            getImageValueTask.DoWork += getFileAttachmentImageValue_DoWork;
            getImageValueTask.TaskCompleted += getImageValueTask_TaskCompleted;
            getImageValueTask.TaskArgs = new ImageArgs { PropName = propName, PublicPropName = publicPropName };
            getImageValueTask.RunTaskAsync();
        }

        void getFileAttachmentImageValue_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundTask task = e.Argument as BackgroundTask;
            ImageArgs imageProps = task.TaskArgs as ImageArgs;
            BitmapSource source = Item.Store.GetImageFileAttachment(Item, imageProps.PropName);
            if (source != null)
            {
                Attachments.Add(imageProps.PropName, new ImageAttachment { BitmapSource = source });
            }
        }

        void getImageValueTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            ImageArgs imageArgs = TaskArgs as ImageArgs;
            if (imageArgs.PublicPropName != null)
            {
                Item.NotifyPropertyChangedByName(imageArgs.PublicPropName);
            }
        }

    }
}
