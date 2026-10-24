//---------------------------------------------------------------------
//  
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
//This source code is intended only as a supplement to Microsoft
//Development Tools and/or on-line documentation.  See these other
//materials for detailed information regarding Microsoft code samples.
// 
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Workflow.ComponentModel.Compiler;

namespace DI.Chakra.Ruleset
{
    internal partial class ValidationErrorsForm : Form
    {

        #region Constructor and variables
        
        internal ValidationErrorsForm()
        {
            InitializeComponent();
        }

        private bool promptForContinue;
        private bool continueWithChange;

        #endregion

        #region Form load

        private void ValidationErrorsForm_Load(object sender, EventArgs e)
        {
            if (!promptForContinue) //just showing the errors, so change the representation in the UI
            {
                continueButton.Hide();
                cancelButton.Text = "OK";
            }
        }

        internal void SetValidationErrors(ValidationErrorCollection errors)
        {

                foreach(ValidationError error in errors)
                {
                    validationErrorsBox.Items.Add(error);
                }
        }

        #endregion

        #region Properties

        internal bool PromptForContinue
        {
            get { return promptForContinue; }
            set { promptForContinue = value; }
        }

        internal string ErrorText
        {
            get { return errorsMessageBox.Text; }
            set { errorsMessageBox.Text = value; }
        }

        internal bool ContinueWithChange
        {
            get { return continueWithChange; }
        }

        #endregion

        #region Event handlers

        private void continueButton_Click(object sender, EventArgs e)
        {
            continueWithChange = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}