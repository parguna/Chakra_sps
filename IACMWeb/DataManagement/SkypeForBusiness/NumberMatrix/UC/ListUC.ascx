<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.DataManagement.SkypeForBusiness.NumberMatrix.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style>
    .label{
        font-size:100%;
    }
</style>
<div class="content-wrapper">
    <div class="row">
        <div class="col-xs-12">
            <cc1:IACMRadGridControl ID="NumberMatrixRadGrid"  runat="server"  AllowMultiRowEdit="false" AllowMultiRowSelection="false" OnItemCommand="NumberMatrixRadGrid_ItemCommand" OnNeedDataSource="NumberMatrixRadGrid_NeedDataSource" OnSelectedIndexChanged="NumberMatrixRadGrid_SelectedIndexChanged" OnItemDataBound="NumberMatrixRadGrid_ItemDataBound" Visible="true">
                 <GroupingSettings CaseSensitive="false" />
            <MasterTableView ShowHeadersWhenNoRecords="true"  NoMasterRecordsText="No Records Available" EditMode="InPlace" InsertItemPageIndexAction="ShowItemOnFirstPage"
                DataKeyNames="Id,Reserved,Assigned" AutoGenerateColumns="false" CommandItemDisplay="Top" EnableHeaderContextMenu="true" Width="100%">
                <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="true"></CommandItemSettings>
               <Columns>
                    <telerik:GridTemplateColumn DataField="Number" FilterControlAltText="Number" HeaderText="Extension"
                    SortExpression="Number" UniqueName="Number" HeaderStyle-Width="15%" FilterControlWidth="65%">
                       <ItemTemplate>
                           <asp:Label ID="lblNumber"  Text='<%# Eval("Number") %>' runat="server"></asp:Label>
                       </ItemTemplate>
                        <EditItemTemplate>
                             <asp:Label ID="lblNumberEdit" Text='<%# Eval("Number") %>' runat="server"></asp:Label>
                        </EditItemTemplate>
                   </telerik:GridTemplateColumn>
                  
                   <telerik:GridTemplateColumn DataField="Assigned" FilterControlAltText="Assigned" HeaderText="Assigned"
                    SortExpression="Assigned" UniqueName="Assigned" HeaderStyle-Width="15%" FilterControlWidth="65%">
                       <ItemTemplate>
                           <asp:Label ID="lblAssigned" Text='<%#Convert.ToBoolean( Eval("Assigned")) ? "Yes":"No" %>' runat="server"  CssClass='<%#Convert.ToBoolean( Eval("Assigned")) ? "label label-success":"label label-danger" %>'></asp:Label>
                       </ItemTemplate>
                   </telerik:GridTemplateColumn>
                   <telerik:GridTemplateColumn DataField="CreatedBy" HeaderText="Assigned To" FilterControlAltText="AccountName" SortExpression="CreatedBy" UniqueName="CreatedBy">
                        <ItemTemplate>
                           <asp:Label ID="lblCreatedBy"  Text='<%# Eval("CreatedBy") %>' runat="server"></asp:Label>
                       </ItemTemplate>
                        <EditItemTemplate>
                             <asp:Label ID="lblCreatedByEdit" Text='<%# Eval("CreatedBy") %>' runat="server"></asp:Label>
                        </EditItemTemplate>
                   </telerik:GridTemplateColumn>
                   <telerik:GridTemplateColumn DataField="Reserved" FilterControlAltText="Reserved" HeaderText="Reserved"
                    SortExpression="Reserved" UniqueName="Reserved" HeaderStyle-Width="15%" FilterControlWidth="65%">
                       <HeaderTemplate>
                        <asp:CheckBox ID="chkReservedNumber" AutoPostBack="true" Visible="true" runat="server" OnCheckedChanged="CheckBox2_CheckedChanged" />
                    </HeaderTemplate>
                       <ItemTemplate>
                           <asp:CheckBox ID="chkReserved" Checked='<%# Convert.ToBoolean(Eval("Reserved")) %>' Enabled='<%# !Convert.ToBoolean( Eval("Assigned"))%>' runat="server"></asp:CheckBox>
                       </ItemTemplate>
                   </telerik:GridTemplateColumn>
                   <telerik:GridEditCommandColumn FilterControlWidth="65%" HeaderStyle-Width="10%" UniqueName="Edit" />
                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>
        </div>
    </div>
</div>