using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.HW1
{
    public static class WorkspaceMenus
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns>True the user is ok with whatever the fuck youre trying to do.</returns>
        public static bool PromptUserConfirmation(string title, string message)
        {
            return MessageBox.Show(
                message, 
                title, 
                MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Question
                ) == DialogResult.OK;
        }
    }
}
