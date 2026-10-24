<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Triggers.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>

        <div style="width: 75%; padding-bottom: 20px;">
            <div style="width: 10%; display: inline-block;">Event Type</div>
            <div style="width: 60%; display: inline;">
                <asp:DropDownList ID="ddlEventType" runat="server" Width="250px" OnSelectedIndexChanged="ddlEventType_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </div>

            <div style="width: 40%;display: inline; float: right;">
                <asp:CheckBox ID="rdBtnIncludeProcessed" runat="server" AutoPostBack="true" OnCheckedChanged="chkIncludeProcessed_CheckedChanged" />
                <span class="lbl">Include Processed & InActive</span>
            </div>
        </div>

        <cc1:IACMRadGridControl ID="TriggersGrid" runat="server" OnNeedDataSource="TriggersGrid_NeedDataSource" AllowCustomPaging="true" OnSelectedIndexChanged="TriggersGrid_SelectedIndexChanged"
            OnItemDataBound="TriggersGrid_ItemDataBound" OnItemCommand="TriggersGrid_ItemCommand">
            <MasterTableView DataKeyNames="Id,TriggerId" TableLayout="Fixed" Width="100%">
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Trigger ID" DataField="TriggerId" FilterControlAltText="TriggerId" SortExpression="TriggerId" UniqueName="TriggerId">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkTriggerId" runat="server" Text='<%#Eval("TriggerId") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkTriggerId_Command" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="UserName" FilterControlAltText="UserName" HeaderText="Employee Name" SortExpression="UserName" UniqueName="UserName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="EmployeeId" FilterControlAltText="EmployeeId" HeaderText="Employee Id" SortExpression="EmployeeId" UniqueName="EmployeeId">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="EventType" AllowFiltering="false" HeaderText="Event Type" SortExpression="EventType" UniqueName="EmployeeEvent">
                    </telerik:GridBoundColumn>

                    <telerik:GridTemplateColumn DataField="IsRequestCreated" FilterControlAltText="IsRequestCreated" HeaderText="Is Request Created" SortExpression="IsRequestCreated" UniqueName="IsRequestCreated">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsRequestCreated"))?"Yes":"No")%>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn DataField="IsAdminCheckRequired" FilterControlAltText="IsAdminCheckRequired" HeaderText="Is Admin Check Required" SortExpression="IsAdminCheckRequired" UniqueName="IsAdminCheckRequired">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsAdminCheckRequired"))?"Yes":"No") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="IsActive" FilterControlAltText="IsActive" HeaderText="Is Active" SortExpression="IsActive" UniqueName="IsActive">
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%#(Convert.ToBoolean(Eval("IsActive"))?"Yes":"No") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}" DataField="EffectiveDate" FilterControlAltText="EffectiveDate" HeaderText="Effective Date" SortExpression="EffectiveDate" UniqueName="EffectiveDate">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="Comments" AllowFiltering="true" HeaderText="Comments" SortExpression="Comments" UniqueName="Comments">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}" DataField="CreatedDate" FilterControlAltText="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="CreatedBy" AllowFiltering="true" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataFormatString="{0:dd-MM-yyyy hh:mm:ss tt}" DataField="LastModifiedDate" FilterControlAltText="LastModifiedDate" HeaderText="LastModified Date" SortExpression="LastModifiedDate" UniqueName="LastModifiedDate">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="LastModifiedBy" AllowFiltering="true" HeaderText="LastModified By" SortExpression="LastModifiedBy" UniqueName="LastModifiedBy">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ActualEventDate" AllowFiltering="true" HeaderText="Actual Event Date" SortExpression="ActualEventDate" UniqueName="ActualEventDate">
                    </telerik:GridBoundColumn>

                   

                </Columns>
            </MasterTableView>
        </cc1:IACMRadGridControl>

        <div class="buttons text-right nav-rectangle">
            <asp:Button runat="server" ID="RefreshButton" CssClass="btn btn-default btn-sm" Text="Refresh" OnClick="RefreshButton_Click"></asp:Button>
            <asp:Button runat="server" ID="BackButton" CssClass="btn btn-default btn-sm" Text="Cancel" OnClientClick="Redirect()"></asp:Button>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
