<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WizardGuide.ascx.cs" Inherits="Provisioning_adhocuc_WizardGuide" %>
<table border="0" cellpadding="3" cellspacing="3" width="99%">
    <tr>
        <td>
            <img src="../Images/Wizard.jpg" alt="wizard" align="middle" />
            <span class="headertitle"><b>Introduction</b></span>
            <div id="line">
            </div>
        </td>
    </tr>
</table>
<table border="0" cellpadding="3" cellspacing="3" width="99%">
    <tr>
        <td>
            <p style="font-family: Verdana,Times New Roman; font-weight: normal; font-size: 10pt;">
                This wizard will help you to place a access request for various systems.
                                                                <br />
                <br />
                You have to do the following in each steps:<br />
                <p style="font-family: Verdana,Times New Roman; font-weight: normal; font-size: 10pt; line-height: 20px; color: Blue">
                    Step1.&nbsp;&nbsp;Select Requester and Endpoint System details.
                                                                    type.<br />
                    Step2.&nbsp;&nbsp;Select the role/group.<br />
                </p>
        </td>
    </tr>
    <tr>
        <td align="right">
            <asp:Button ID="btnCancelWizard" runat="server" CssClass="uiboxgray" Text="Cancel"
                ToolTip="Cancel" Width="70px" />
            &nbsp;
            <asp:Button ID="btnStartWizard" runat="server" CssClass="uiboxgray" Text="Next" ToolTip="Next Step"
                Width="70px" OnClick="btnStartWizard_Click" />
        </td>
    </tr>
</table>
