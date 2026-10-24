<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttestationReviewHistoryUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Attestation.UC.AttestationReviewHistoryUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
 
<telerik:RadGrid ID="AttestationActionHistory" runat="server" AllowSorting="false" AllowPaging="true"
       
    CellSpacing="0" GridLines="None">  
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%"/>
                                
    <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="None" EnableHeaderContextMenu="false">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="Version" FilterControlAltText="Version" SortExpression="Version" HeaderText="Version" UniqueName="Version">
            </telerik:GridBoundColumn>                                   
           <telerik:GridBoundColumn DataField="TaskId" FilterControlAltText="TaskId" SortExpression="TaskId" HeaderText="TaskId" UniqueName="TaskId">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn UniqueName="ApproveGridTemplateColumn" HeaderText="Retain" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:RadioButton ID="ApproveRadioButton" runat="server" AutoPostBack="true" GroupName="ApproveRevoke" CssClass="RadGrid_SkinName"
                        Checked='<%#Convert.ToBoolean(Eval("Approve")) %>' Enabled="false" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn UniqueName="RevokeGridTemplateColumn" HeaderText="Revoke" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:RadioButton ID="RevokeRadioButton" runat="server" AutoPostBack="true" GroupName="ApproveRevoke" CssClass="RadGrid_SkinName"
                        Checked='<%#Convert.ToBoolean(Eval("Revoke")) %>' Enabled="false" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>  
            <telerik:GridBoundColumn DataField="EffectiveDate"  FilterControlAltText="EffectiveDate" SortExpression="EffectiveDate" HeaderText="Effective Date" UniqueName="EffectiveDate">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Comment"  FilterControlAltText="Comment" SortExpression="Comment" HeaderText="Comment" UniqueName="Comment">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="LastModifiedBy"  FilterControlAltText="LastModifiedBy" SortExpression="LastModifiedBy" HeaderText="Reviewed By" UniqueName="LastModifiedBy">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>