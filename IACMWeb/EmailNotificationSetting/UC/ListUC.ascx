<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.EmailNotificationSetting.UC.ListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadGrid ID="EmailNotificationSettingsGrid" runat="server" OnNeedDataSource="EmailNotificationSettingsGrid_NeedDataSource"
 AllowPaging="true" Width="50%" OnItemCommand ="EmailNotificationSettingsGrid_ItemCommand" 
    OnItemCreated ="EmailNotificationSettingsGrid_ItemCreated" OnItemDataBound ="EmailNotificationSettingsGrid_ItemDataBound">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="20" Height="100%" AlwaysVisible="true" />
    <ClientSettings EnablePostBackOnRowClick="true">
        <Scrolling UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <MasterTableView NoMasterRecordsText="No Data Found" AutoGenerateColumns="false" Width="100%" >
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="EventType" HeaderText="Event Type" UniqueName ="EventType">
            </telerik:GridBoundColumn>
             <telerik:GridTemplateColumn  HeaderText="By Job"   >  
          <ItemTemplate>  
              <asp:CheckBox ID ="chkJob" AutoPostBack ="true" runat="server" OnCheckedChanged ="chkJob_CheckedChanged" />
              
          </ItemTemplate>  
         </telerik:GridTemplateColumn>  
              <telerik:GridTemplateColumn  HeaderText="On Effective Date Execution"   >  
          <ItemTemplate>  
               <asp:CheckBox ID ="chkOnEffectiveDateExecution" AutoPostBack ="true" runat="server" OnCheckedChanged ="chkOnEffectiveDateExecution_CheckedChanged" />
  
          </ItemTemplate>  
         </telerik:GridTemplateColumn>  

            <telerik:GridTemplateColumn  HeaderText="PreConfigureDays">  
          <ItemTemplate>  
              <asp:TextBox ID ="txtPreconfigureNotificationDays" runat ="server" Enabled ="false"></asp:TextBox>
  
          </ItemTemplate>  
         </telerik:GridTemplateColumn>  

            <telerik:GridTemplateColumn  HeaderText="From">  
          <ItemTemplate>  
              <asp:TextBox ID ="txtFrom" runat ="server"></asp:TextBox>
  
          </ItemTemplate>  
         </telerik:GridTemplateColumn>  

             <telerik:GridTemplateColumn  HeaderText="To">  
          <ItemTemplate>  
              <asp:TextBox ID ="txtTo" runat ="server"></asp:TextBox>
  
          </ItemTemplate>  
         </telerik:GridTemplateColumn>  

             <telerik:GridTemplateColumn  HeaderText="CC">  
          <ItemTemplate>  
              <asp:TextBox ID ="txtCC" runat ="server"></asp:TextBox>
  
          </ItemTemplate>  
         </telerik:GridTemplateColumn>  
        </Columns>
    </MasterTableView>
</telerik:RadGrid>