using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public class FullyQualifiedADName
    {
        public string Name { get; set; }
        public string DomainName { get; set; }
    }

    public class DirectReportCollection : List<UserInformation> { }

    public class UserInformation
    {
        private static Dictionary<string, DirectorySearcher> s_searchers;
        private DirectoryEntry UserEntry { get; set; }
        private string InitName;

        public UserInformation()
        {
            DirectReports = new DirectReportCollection();
            InitName = null;
        }

        public static string GetDisplayNameFromAlias(string alias)
        {
            DirectoryEntry userEntry = GetUserDirectoryEntryFromAlias(alias);
            return GetStringValue(userEntry, "displayName");
        }

        public static string GetTitleFromAlias(string alias)
        {
            DirectoryEntry userEntry = GetUserDirectoryEntryFromAlias(alias);
            return GetStringValue(userEntry, "title");
        }

        public static BitmapSource GetImageFromAlias(string alias)
        {
            DirectoryEntry userEntry = GetUserDirectoryEntryFromAlias(alias);
            MemoryStream userPicture = GetUserPicture(userEntry);
            if (userPicture == null)
            {
                return Globals.ApplicationManager.GenericProfileBitmap;
            }

            return Utils.GetBitmapSourceFromStream(userPicture);
        }

        public DirectReportCollection DirectReports { get; private set; }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes this object, given a valid e-mail alias.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool InitializeWithAlias(string alias)
        {
            if (InitName == null)
            {
                InitName = alias;
                UserEntry = GetUserDirectoryEntryFromAlias(alias);
                InitializeDirectReports();
            }

            return IsValid;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes this object, given a valid full name ("first last").
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool InitializeWithFullName(string fullName, string domainName)
        {
            if (InitName == null)
            {
                InitName = fullName;
                UserEntry = GetUserDirectoryEntryFromFullName(fullName, domainName);
                InitializeDirectReports();
            }

            return IsValid;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this object has been initialized with a valid alias or full name.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsValid { get { return UserEntry != null; } }

        private void InitializeDirectReports()
        {
            if (UserEntry != null)
            {
                PropertyValueCollection reports = UserEntry.Properties["directReports"];
                foreach (string report in reports)
                {
                    FullyQualifiedADName adName = GetADNameFromDescriptor(report);

                    UserInformation uiReport = new UserInformation();
                    if (!uiReport.InitializeWithFullName(adName.Name, adName.DomainName))
                    {
                        Globals.ApplicationManager.WriteToEventLog("Updating team members: fullname did not resolve: " + adName.Name);
                    }

                    DirectReports.Add(uiReport);
                }
            }
        }

        public static FullyQualifiedADName GetADNameFromDescriptor(string descriptor)
        {
            FullyQualifiedADName adName = new FullyQualifiedADName();
            StringBuilder domainName = new StringBuilder();
            int domainNamePartCount = 0;

            string[] fields = descriptor.Split(",".ToCharArray());
            foreach (string field in fields)
            {
                string[] nameValue = field.Split("=".ToCharArray());

                if (nameValue[0] == "CN")
                {
                    adName.Name = nameValue[1];
                }
                else if (nameValue[0] == "DC")
                {
                    if (domainNamePartCount > 0)
                    {
                        domainName.Append(".");
                    }
                    
                    domainName.Append(nameValue[1]);
                    domainNamePartCount++;
                }
            }

            if (string.IsNullOrWhiteSpace(adName.Name) || domainNamePartCount != 4)
            {
                Globals.ApplicationManager.WriteToEventLog("User Information string not recognized: " + descriptor);
            }
            adName.DomainName = domainName.ToString();
            return adName;
        }

        public string Alias
        {
            get { return GetStringValue("mailNickname"); }
        }

        public string DisplayName
        {
            get { return GetStringValue("displayName"); }
        }

        public string JobTitle
        {
            get { return GetStringValue("title"); }
        }

        public string OfficeName
        {
            get { return GetStringValue("physicalDeliveryOfficeName"); }
        }

        public string Department
        {
            get { return GetStringValue("department"); }
        }

        public string Telephone
        {
            get { return GetStringValue("telephoneNumber"); }
        }

        public MemoryStream UserPicture
        {
            get
            {
                return GetUserPicture(UserEntry);
            }
        }

        private static MemoryStream GetUserPicture(DirectoryEntry userEntry)
        {
            if (userEntry == null)
            {
                return null;
            }

            if (userEntry.Properties["thumbnailPhoto"].Value == null)
            {
                return null;
            }

            return new MemoryStream(userEntry.Properties["thumbnailPhoto"].Value as byte[]);
        }

        private string GetStringValue(string propertyName)
        {
            return GetStringValue(UserEntry, propertyName);
        }

        private static string GetStringValue(DirectoryEntry userEntry, string propertyName)
        {
            return userEntry == null ? "" : userEntry.Properties[propertyName].Value as string;
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves the DirectoryEntry object for the specified user, given an e-mail alias.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DirectoryEntry GetUserDirectoryEntryFromAlias(string alias)
        {
            return GetUserDirectoryEntryFromFilter(string.Format("(sAMAccountName={0})", alias), "redmond.corp.microsoft.com");
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves the DirectoryEntry object for the specified user, given a full first
        /// and last name.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DirectoryEntry GetUserDirectoryEntryFromFullName(string adName, string domainName)
        {
            return GetUserDirectoryEntryFromFilter(string.Format("(name={0})", adName), domainName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a user DirectoryEntry, given a filter string that contains an appropriate
        /// user property specification and value.
        /// </summary>
        //------------------------------------------------------------------------------------
        private static DirectoryEntry GetUserDirectoryEntryFromFilter(string filter, string domainName)
        {
            try
            {
                DirectorySearcher searcher = GetDirectorySearcher(domainName);
                searcher.Filter = filter;
                searcher.Asynchronous = false;

                SearchResultCollection results = searcher.FindAll();
                DirectoryEntry directoryEntry = results.Count > 0 ? results[0].GetDirectoryEntry() : null;

                return directoryEntry;
            }

            catch (Exception e)
            {
                Globals.ApplicationManager.HandleException(e);
                return null;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an Active Directory searcher for the Microsoft domain.
        /// </summary>
        //------------------------------------------------------------------------------------
        private static DirectorySearcher GetDirectorySearcher(string domainName)
        {
            Domain domain = Domain.GetDomain(new DirectoryContext(DirectoryContextType.Domain, domainName));
            if (domain != null)
            {
                DomainController dc = domain.FindDomainController();
                if (dc != null)
                {
                    return dc.GetDirectorySearcher();
                }
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an Active Directory searcher for the Microsoft domain.
        /// </summary>
        //------------------------------------------------------------------------------------
        private static DirectorySearcher GetCachedDirectorySearcher(string domainName)
        {
            s_searchers = null;

            if (s_searchers == null)
            {
                s_searchers = new Dictionary<string, DirectorySearcher>();
            }

            if (!s_searchers.ContainsKey(domainName))
            {
                Domain domain = Domain.GetDomain(new DirectoryContext(DirectoryContextType.Domain, domainName));
                if (domain != null)
                {
                    DomainController dc = domain.FindDomainController();
                    if (dc != null)
                    {
                        DirectorySearcher searcher = dc.GetDirectorySearcher();
                        s_searchers.Add(domainName, searcher);
                    }
                }
            }

            if (!s_searchers.ContainsKey(domainName))
            {
                return null;
            }

            return s_searchers[domainName];
        }

    }
}
