using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public enum WelcomeState
    {
        Ok,
        Open,
        Next,
        Back,
        Cancel,
        Restart,
    }

    public class ApplicationWelcome
    {
        public WelcomeState WelcomeState { get; set; }
        public ProductGroupItem SelectedProductGroupItem { get; set; }

        public ApplicationWelcome()
        {

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Begins the Welcome sequence that will enable the user to open an existing product
        /// group, or create a new group for their team.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void BeginWelcome()
        {
            WelcomeDialog welcomeDialog = new WelcomeDialog();

            WelcomeState = welcomeDialog.ShowWelcomeDialog();
            if (WelcomeState == WelcomeState.Next)
            {
                SelectProductGroupDialog dialog = new SelectProductGroupDialog();
                dialog.ShowDialog();

                WelcomeState = dialog.WelcomeState;
                if (WelcomeState == PlannerNameSpace.WelcomeState.Open)
                {
                    SelectedProductGroupItem = dialog.SelectedProductGroupItem;
                }
            }

        }
    }
}
