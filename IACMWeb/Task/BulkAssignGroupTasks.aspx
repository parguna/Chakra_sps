<%@ Page Language="C#" AutoEventWireup="true" Title="Bulk Assign Group Task" CodeBehind="BulkAssignGroupTasks.aspx.cs" MasterPageFile="~/Master/ProtoMast4.master" Inherits="DI.Chakra.Web.Task.BulkAssignGroupTasks" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="content-wrapper">
                <div class="form">
                    <div class="form-horizontal left">
                        <div class="row-fluid">
                            <div class="col-xs-12">
                                <label class="col-xs-2 control-label required">Group</label>
                                <div class="col-xs-5">
                                    <asp:DropDownList ID="ddlGroups" CssClass="form-control selectpicker" runat="server" data-width="100%" AutoPostBack="true" OnSelectedIndexChanged="ddlGroupsIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-5 control-label"></label>
                                <div class="col-xs-7">
                                    &nbsp;
                                </div>
                            </div>
                            <div class="col-xs-12">
                                <div>
                                    <label class="col-xs-2 control-label required">Assign To</label>
                                    <div class="col-xs-5">
                                        <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" OnEntryRemoved="UserNameAutoCompleteBox_EntryRemoved" OnClientEntryAdding="OnClientEntryAddingHandler"
                                            OnClientRequesting="OnClientRequesting"
                                            EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                                            InputType="Token" OnEntryAdded="UserNameAutoCompleteBox_EntryAdded">
                                            <TextSettings SelectionMode="Single" />
                                            <WebServiceSettings Method="GetUsersByGroupId" Path="~/Services/AutoCompleteDataService.svc" />
                                        </telerik:RadAutoCompleteBox>

                                    </div>
                                    <div class="col-xs-5">
                                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-default btn-sm" OnClick="btnSubmit_Click" Text="Save" />
                                        &nbsp;&nbsp;
                                                    
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-xs-5 control-label"></label>
                                    <div class="col-xs-7">
                                        &nbsp;
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-xs-12">
                                        <cc2:IACMRadGridControl ID="grdTask" runat="server" FilterMenu-CssClass="RadMenu_IACMFilterSkin" OnNeedDataSource="grdTask_NeedDataSource" DisableRowSelect="true"
                                            OnItemCommand="grdTask_ItemCommand" PageSize="50">
                                            <MasterTableView AutoGenerateColumns="False" ShowHeadersWhenNoRecords="true" CommandItemDisplay="Top" Width="190%" NoMasterRecordsText="No Records Available" DataKeyNames="TaskId,AccessRequestID,Type,RequestType,IsCompleted,StatusType"  PageSize="50">
                                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                                                <Columns>
                                                    <telerik:GridTemplateColumn UniqueName="CheckTemp" HeaderText="CheckTemp" AllowFiltering="false" AllowSorting="false" Exportable="false" HeaderStyle-Width="40px" FilterControlWidth="65%">
                                                        <HeaderTemplate>
                                                            <asp:CheckBox ID="CheckBox2" AutoPostBack="true" Visible="true" runat="server" OnCheckedChanged="CheckBox2_CheckedChanged" style="position:absolute"/>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ClientSelectColumn" runat="server" AutoPostBack="true" OnCheckedChanged="ClientSelectColumn_CheckedChanged" />
                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="TaskId" HeaderText="Task ID" FilterControlAltText="TaskId" SortExpression="TaskId" UniqueName="TaskId" HeaderStyle-Width="10%" FilterControlWidth="65%">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkTaskId" runat="server" Text='<%#Eval("TaskId") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkTaskId_Command" />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn DataField="AccessRequestID" HeaderText="Request ID" SortExpression="AccessRequestID" UniqueName="AccessRequestID" HeaderStyle-Width="10%" FilterControlWidth="40%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblRequestId" runat="server" Text='<%#Eval("AccessRequestID") %>'
                                                                Visible='<%#Convert.ToInt32(Eval("AccessRequestID")) > 0 %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="RequestName" SortExpression="RequestName" HeaderText="Application Name" AllowFiltering="true" UniqueName="RequestName" HeaderStyle-Width="15%" FilterControlWidth="40%">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="Type" FilterControlAltText="Type" SortExpression="Type" HeaderText="Task" UniqueName="Type" HeaderStyle-Width="15%" FilterControlWidth="40%">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="TaskSource" FilterControlAltText="TaskSource" SortExpression="TaskSource" HeaderText="Task Source" UniqueName="TaskSource" FilterControlWidth="20%" HeaderStyle-Width="10%">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="AccountType" FilterControlAltText="AccountType" HeaderText="Account Type" SortExpression="AccountType" UniqueName="AccountType" FilterControlWidth="40%" HeaderStyle-Width="10%">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="RequestType" FilterControlAltText="RequestType" SortExpression="RequestType" HeaderText="Request Type" UniqueName="RequestType" FilterControlWidth="40%" HeaderStyle-Width="10%" Display="true">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="RequestedFor" FilterControlAltText="RequestedFor" SortExpression="RequestedFor" HeaderText="Requested For" UniqueName="RequestedFor" HeaderStyle-Width="15%" FilterControlWidth="40%">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="RequestedBy" FilterControlAltText="RequestedBy" SortExpression="RequestedBy" HeaderText="Request By" UniqueName="RequestedBy" HeaderStyle-Width="15%" FilterControlWidth="40%">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridTemplateColumn DataField="IsDelegated" HeaderText="Is Delegated" SortExpression="IsDelegated" UniqueName="IsDelegated" AllowFiltering="true" HeaderStyle-Width="10%" FilterControlWidth="80%" Display="true">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblisdelegate" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsDelegated"))? "Yes":"No") %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="DelegatedBy" FilterControlAltText="DelegatedBy" UniqueName="DelegatedBy" HeaderText="Delegated By" AllowFiltering="true" HeaderStyle-Width="15%" FilterControlWidth="40%" Display="true">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="Entitlement" FilterControlAltText="Entitlement" UniqueName="Entitlement" HeaderText="Entitlement" HeaderStyle-Width="25%">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridDateTimeColumn DataField="RequestedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="RequestedDate" SortExpression="RequestedDate" Display="true" HeaderText="Requested Date" UniqueName="RequestedDate" FilterControlWidth="10%" HeaderStyle-Width="15%">
                                                    </telerik:GridDateTimeColumn>
                                                    <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="CreatedDate" SortExpression="CreatedDate" HeaderText="Created Date" UniqueName="CreatedDate" HeaderStyle-Width="15%" FilterControlWidth="40%">
                                                    </telerik:GridDateTimeColumn>
                                                    <telerik:GridDateTimeColumn DataField="StartDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="StartDate" SortExpression="StartDate" Display="true" HeaderText="Start Date"
                                                        UniqueName="StartDate" FilterControlWidth="10%" HeaderStyle-Width="10%">
                                                    </telerik:GridDateTimeColumn>
                                                    <telerik:GridDateTimeColumn DataField="EndDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="EndDate" SortExpression="EndDate" HeaderText="End Date" UniqueName="EndDate" FilterControlWidth="40%" HeaderStyle-Width="10%">
                                                    </telerik:GridDateTimeColumn>
                                                    <telerik:GridBoundColumn DataField="StatusType" HeaderText="Status" FilterControlAltText="StatusType" UniqueName="StatusType" HeaderStyle-Width="10%" Display="true">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="AssignedByName" FilterControlAltText="AssignedByName" UniqueName="AssignedByName" HeaderText="Assigned By" AllowFiltering="true" HeaderStyle-Width="15%" FilterControlWidth="40%" Display="true">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="AssignedToName" FilterControlAltText="AssignedToName" UniqueName="AssignedToName" HeaderText="Assigned To" AllowFiltering="true" HeaderStyle-Width="15%" FilterControlWidth="40%" Display="true">
                                                    </telerik:GridBoundColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </cc2:IACMRadGridControl>
                                    </div>
                                </div>
                                <div class="form-group">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="form-group">
                        <div class="col-xs-12">
                            <div class="form-group">
                                <label class="col-xs-11 control-label"></label>
                                <div class="col-xs-1">
                                    <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-default btn-sm" OnClick="btnCancel_Click" Text="Cancel" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <asp:HiddenField ID="hdGroupId" runat="server" />
            <script>
                function OnClientEntryAddingHandler(sender, eventArgs) {

                    if (sender.get_entries().get_count() > 0) {
                        eventArgs.set_cancel(true);
                    }
                }
                function OnClientRequesting(sender, args) {
                    var context = args.get_context()
                    context["GroupId"] = document.getElementById("<%= hdGroupId.ClientID%>").value;
                }

            </script>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
