using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections;

namespace PlannerNameSpace.Views
{
    public enum BacklogValidationErrorValue
    {
        NoError,
        CommitmentChangedOnClosedItem,
        InvalidCommitmentSetting,
        InvalidNoWorkCommitmentSetting,
        MustBeAdminError,
        InvalidSpecSetting,
    }

    public partial class BacklogManagerView
    {
        void HandleCommitmentSettingChanged(BacklogItem backlogItem)
        {
            if (!Globals.ApplicationManager.IsCurrentUserAdmin())
            {
                SetBacklogValidationError(BacklogValidationErrorValue.MustBeAdminError, backlogItem);
            }
            else
            {
                CommitmentSettingValues commitmentSetting = backlogItem.CommitmentSetting;
                if (!backlogItem.IsActive && commitmentSetting != CommitmentSettingValues.Completed)
                {
                    SetBacklogValidationError(BacklogValidationErrorValue.CommitmentChangedOnClosedItem, backlogItem);
                }

                else if (commitmentSetting == CommitmentSettingValues.Committed || commitmentSetting == CommitmentSettingValues.In_Progress)
                {
                    StoreSpecStatusValue specStatusValue = StoreSpecStatus.GetStoreSpecStatus(backlogItem.StoreSpecStatusText);
                    if (specStatusValue != StoreSpecStatusValue.ReadyForCoding && specStatusValue != StoreSpecStatusValue.SpecFinalized)
                    {
                        SetBacklogValidationError(BacklogValidationErrorValue.InvalidCommitmentSetting, backlogItem);
                    }
                }

                else if (commitmentSetting == CommitmentSettingValues.In_Progress && backlogItem.TotalWorkAvailable == 0)
                {
                    SetBacklogValidationError(BacklogValidationErrorValue.InvalidCommitmentSetting, backlogItem);
                }
            }
        }

        void SetBacklogValidationError(BacklogValidationErrorValue errorValue, BacklogItem invalidBacklogItem)
        {
            switch (errorValue)
            {
                case BacklogValidationErrorValue.InvalidSpecSetting:
                    UserMessage.Show(PlannerContent.BacklogValidationInvalidSpecMessage);
                    invalidBacklogItem.StoreSpecStatusText = invalidBacklogItem.OriginalStoreSpecStatusText;
                    invalidBacklogItem.NotifyPropertyChanged(() => invalidBacklogItem.StoreSpecStatusText);
                    invalidBacklogItem.OriginalStoreSpecStatusText = null;
                    break;

                case BacklogValidationErrorValue.MustBeAdminError:
                    if (!Globals.ApplicationManager.ConfirmIsAdmin("Change Backlog Item Commitment Setting"))
                    {
                        invalidBacklogItem.CommitmentSetting = invalidBacklogItem.PreviousCommitmentStatus;
                    }
                    break;

                case BacklogValidationErrorValue.InvalidCommitmentSetting:
                    UserMessage.Show(PlannerContent.BacklogValidationInvalidCommitmentMessage);
                    invalidBacklogItem.CommitmentSetting = invalidBacklogItem.PreviousCommitmentStatus;
                    break;

                case BacklogValidationErrorValue.InvalidNoWorkCommitmentSetting:
                    UserMessage.Show(PlannerContent.BacklogValidationInvalidNoWorkCommitmentMessage);
                    invalidBacklogItem.CommitmentSetting = invalidBacklogItem.PreviousCommitmentStatus;
                    break;

                case BacklogValidationErrorValue.CommitmentChangedOnClosedItem:
                    if (UserMessage.ShowTwoLines(PlannerContent.BacklogValidationCommitmentChangedOnClosedMessage, "Title: " + invalidBacklogItem.FullyQualifiedTitle, MessageBoxButton.OKCancel))
                    {
                        invalidBacklogItem.ActivateItem();
                    }
                    else
                    {
                        invalidBacklogItem.CommitmentSetting = invalidBacklogItem.PreviousCommitmentStatus;
                        invalidBacklogItem.LandingDate = invalidBacklogItem.PreviousLandingDate;
                    }
                    break;
            }
        }
    }
}
