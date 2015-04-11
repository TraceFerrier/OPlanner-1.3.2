using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Input;

namespace PlannerNameSpace
{
    public enum GlobalPreference
    {
        Unused,
        LastSelectedRibbonTab,
        LastOpenProductGroupKey,
        ShouldUseCloneStore,
    }

    public enum ProductPreferences
    {
        LastSelectedFeatureTeamInEditor,
        LastSelectedCapacityFeatureTeam,
        LastSelectedBoardFeatureTeam,
        BacklogViewLastSelectedPillar,
        BacklogViewLastSelectedTrain,
        BacklogViewLastSelectedScenario,
        LastSelectedFeatureTeam,
        FeatureTeamPillarFilterItem,
        FeatureTeamTrainFilterItem,
        LastSelectedGroupMember,
        LastSelectedExperienceViewPillarItem,
        LastSelectedExperienceViewQuarterItem,
        LastSelectedScenarioViewPillarItem,
        LastSelectedScenarioViewQuarterItem,
        BacklogViewLastSelectedFeatureTeam,
        BacklogViewLastSelectedStatus,
        LastSelectedExperienceViewPersonaItem,
        LastSelectedBurndownPillarItem,
        LastSelectedBurndownTrainItem,
        LastSelectedMemberAssignmentsFilterTrainItem,
        LastSelectedMemberAssignmentsFilterDisciplineValue,
        LastSelectedBugsStatusFilterValue,
        LastSelectedBugsAssignedToValue,
        LastSelectedBugsIssueTypeValue,
        LastSelectedBugsResolutionValue,
        LastSelectedForecastPillarFilterItem,
        LastSelectedForecastModeValue,
        LastSelectedForecastViewValue,
        LastSelectedProductGroupDisciplineValue,
        LastSelectedScenarioSortPropertyValue,
        LastSelectedExperienceSortPropertyValue,
        LastSelectedOverdueWorkItemAssignedToValue,
        LastSelectedBacklogViewValue,
        LastSelectedExperienceViewValue,
        LastSelectedFeatureTeamDetailsPillarFilterItem,
        LastSelectedStatusBarPositionValue,
        LastSelectedBacklogViewTypeValue,
        LastSelectedExperienceViewTypeValue,
        LastSelectedOverdueWorkItemsPillarFilterItem,
        LastSelectedOverdueWorkItemsTrainFilterItem,
        LastSelectedBurndownChartStyle,
        LastSelectedBacklogAssignToScenarioQuarterItem,
        LastSelectedMemberViewValue,
        LastSelectedMemberViewShowOnlyActiveItemsValue,
        LastSelectedKeyDatesTrain,
        LastSelectedShowHideBacklogSecondPanel,
        LastSelectedBacklogSecondPanelView,
        LastSelectedTrainCommitmentPillar,
        LastSelectedTrainCommitmentTrain,
        LastSelectedProductGroupMembersPillar,
        ReviewPagesReviewLastSelectedPillar,
        ReviewPagesReviewLastSelectedTrain,
        ReviewPagesLastSelectedPage,
        LastSelectedNewPillarPathID,
        LastSelectedTrainEditorTrain,
        LastSelectedBoardViewPillar,
        LastSelectedBoardViewScrumTeam,
        LastSelectedScrumTeamsViewPillar,
        LastSelectedScrumTeamsViewTeam,
        None
    }

    [Serializable]
    public class UserPreferences
    {
        public bool ShouldUseCloneHostStore
        {
            get { return GetGlobalPreference<bool>(GlobalPreference.ShouldUseCloneStore); }
            set { SetGlobalPreference(GlobalPreference.ShouldUseCloneStore, value); }
        }
        private Dictionary<GlobalPreference, object> GlobalPreferences;
        private Dictionary<string, Dictionary<ProductPreferences, object>> ProductPreferences;

        public UserPreferences()
        {
            GlobalPreferences = new Dictionary<GlobalPreference, object>();
            ProductPreferences = new Dictionary<string, Dictionary<ProductPreferences, object>>();
        }

        public void SetItemSelectionPreference(ProductPreferences selection, StoreItem item)
        {
            if (selection == PlannerNameSpace.ProductPreferences.None)
            {
                return;
            }

            string productKey = ProductGroupManager.Instance.CurrentProductGroupKey;
            if (!ProductPreferences.ContainsKey(productKey))
            {
                ProductPreferences.Add(productKey, new Dictionary<ProductPreferences, object>());
            }

            Dictionary<ProductPreferences, object> itemSelectionPreferences = ProductPreferences[productKey];
            if (!itemSelectionPreferences.ContainsKey(selection))
            {
                itemSelectionPreferences.Add(selection, null);
            }

            itemSelectionPreferences[selection] = item == null ? null : item.StoreKey;
            Serialize();
        }

        public ItemType GetItemSelectionPreference<ItemType>(ProductPreferences selection) where ItemType : StoreItem
        {
            string productKey = ProductGroupManager.Instance.CurrentProductGroupKey;
            if (ProductPreferences.ContainsKey(productKey))
            {
                Dictionary<ProductPreferences, object> itemSelectionPreferences = ProductPreferences[productKey];
                if (itemSelectionPreferences.ContainsKey(selection))
                {
                    string storeKey = (string) itemSelectionPreferences[selection];
                    return Globals.ItemManager.GetItem<ItemType>(storeKey);
                }
            }
            return null;
        }

        public void SetEnumSelectionPreference<T>(ProductPreferences selection, T enumVal)
        {
            string enumText = Utils.EnumToString<T>(enumVal);
            SetProductPreference(selection, enumText);
        }

        public T GetEnumSelectionPreference<T>(ProductPreferences selection)
        {
            string enumText = GetProductPreference(selection);
            return Utils.StringToEnum<T>(enumText);
        }

        public string GetProductPreference(ProductPreferences preference)
        {
            return GetProductPreference<string>(preference);
        }

        public T GetProductPreference<T>(ProductPreferences preferenceSelector)
        {
            if (ProductGroupManager.Instance.CurrentProductGroup != null)
            {
                string productKey = ProductGroupManager.Instance.CurrentProductGroup.StoreKey;
                if (ProductPreferences.ContainsKey(productKey))
                {
                    Dictionary<ProductPreferences, object> productPreferences = ProductPreferences[productKey];
                    if (productPreferences.ContainsKey(preferenceSelector))
                    {
                        return (T)productPreferences[preferenceSelector];
                    }
                }
            }

            return default(T);
        }

        public void SetProductPreference(ProductPreferences preference, object value)
        {
            string productKey = ProductGroupManager.Instance.CurrentProductGroupKey;
            if (!ProductPreferences.ContainsKey(productKey))
            {
                ProductPreferences.Add(productKey, new Dictionary<ProductPreferences, object>());
            }

            Dictionary<ProductPreferences, object> productPreferences = ProductPreferences[productKey];
            if (!productPreferences.ContainsKey(preference))
            {
                productPreferences.Add(preference, null);
            }

            productPreferences[preference] = value;
            Serialize();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the user preference of the specified type. If this preference has never 
        /// been set, null will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public ObjType GetGlobalPreference<ObjType>(GlobalPreference preference)
        {
            if (GlobalPreferences.ContainsKey(preference))
            {
                return (ObjType) GlobalPreferences[preference];
            }

            return default(ObjType);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the user preference of the specified type.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetGlobalPreference(GlobalPreference preference, object value)
        {
            if (GlobalPreferences == null)
            {
                GlobalPreferences = new Dictionary<GlobalPreference, object>();
            }

            if (GlobalPreferences.ContainsKey(preference))
            {
                GlobalPreferences[preference] = value;
            }
            else
            {
                GlobalPreferences.Add(preference, value);
            }

            Serialize();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Persists all current user preferences to local storage.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void Serialize()
        {
            IFormatter formatter = new BinaryFormatter();
            string fullPath = GetUserPreferencesFullPath();
            Stream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, Globals.UserPreferences);
            stream.Close();
        }

        public static UserPreferences Unserialize(bool shouldClearUserPreferences)
        {
            UserPreferences userPreferences = null;
            string preferencesFullPath = GetUserPreferencesFullPath();

            if (shouldClearUserPreferences)
            {
                File.Delete(preferencesFullPath);
            }

            if (File.Exists(preferencesFullPath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(preferencesFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                try
                {
                    userPreferences = (UserPreferences)formatter.Deserialize(stream);
                }
                catch (Exception e)
                {
                    Globals.ApplicationManager.HandleException(e);
                    userPreferences = null;
                }

                finally
                {
                    stream.Close();

                    if (userPreferences == null)
                    {
                        userPreferences = null;
                    }
                }
            }

            if (userPreferences == null || userPreferences.ProductPreferences == null || userPreferences.GlobalPreferences == null)
            {
                userPreferences = new UserPreferences();
            }

            return userPreferences;
        }

        public static string GetUserPreferencesFullPath()
        {
            string rootFolder = ApplicationManager.GetOPlannerFolder();
            return Path.Combine(rootFolder, "OPlannerPrefs.bin");
        }

    }
}
