<%@ Page Title="UO Change/ReOrg Request" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="DepartmentChangeRequest.aspx.cs" Inherits="DI.Chakra.Web.DepartmentChangeRequest" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxCtrl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">

    <script language="javascript" type="text/javascript">
        function TrimStr(StrValue) {
            return StrValue.replace(/(?:(?:^|\n)\s+|\s+(?:$|\n))/g, '').replace(/\s+/g, ' ');
        }

        function selectAll(invoker) {
            var inputElements = document.getElementsByTagName('input');
            for (var i = 0; i < inputElements.length; i++) {

                var myElement = inputElements[i];

                if (myElement.type === "checkbox") {

                    myElement.checked = invoker.checked;
                }
            }
        }
    </script>
    <style>
    input[type=checkbox] + .lbl, input[type=checkbox] + .rtIn, input[type=radio] + .lbl, input[type=checkbox] + label{
        padding-right:1px !important;
    }
</style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div class="form">
                <div class="form-horizontal left clearfix">
                    <div class="row-fluid">

                        <br />

                        <div class="form-control col-xs-6">
                            <div class="col-xs-3">
                                <asp:RadioButton ID="ouChangeRB" runat="server" AutoPostBack="true" Text="OU Change" GroupName="DepChange" Checked="true" OnCheckedChanged="ouChangeRB_CheckedChanged" />
                            </div>
                            <div class="col-xs-3">
                                <asp:RadioButton ID="reOrgRB" runat="server" AutoPostBack="true" Text="Re Org" GroupName="DepChange" OnCheckedChanged="ouChangeRB_CheckedChanged" />
                            </div>
                             <div class="col-xs-3">
                                <asp:RadioButton ID="onboardRB" runat="server" AutoPostBack="true" Text="OnBoard" GroupName="DepChange" OnCheckedChanged="ouChangeRB_CheckedChanged" />
                            </div>
                            <div class="col-xs-3">
                                <asp:RadioButton ID="rehireRB" runat="server" AutoPostBack="true" Text="Re-Hire" GroupName="DepChange" OnCheckedChanged="ouChangeRB_CheckedChanged" />
                            </div>
                        </div>

                        <br />

                    </div>
                </div>

                <div class="form-horizontal left clearfix">
                    <div class="row-fluid">
                        <asp:Panel ID="Panel4" runat="server" Height="90%" ScrollBars="Auto" Width="100%">

                            <cc1:IACMRadGridControl ID="grdDeptChangedUsers" runat="server" OnNeedDataSource="grdDeptChangedUsers_NeedDataSource"
                                OnItemCommand="grdDeptChangedUsers_ItemCommand"
                                AllowFilteringByColumn="true" AllowPaging="true" AllowSorting="true" CellSpacing="0" GridLines="None" AllowAutomaticUpdates="false"
                                EnableAjaxSkinRendering="true">
                                <ExportSettings
                                    HideStructureColumns="true"
                                    IgnorePaging="true" ExportOnlyData="true"
                                    Excel-Format="ExcelML"
                                    OpenInNewWindow="true">
                                    <Pdf PageHeight="210mm" AllowPrinting="true" PageWidth="297mm"
                                        DefaultFontFamily="Arial Unicode MS" PageTopMargin="45mm"
                                        BorderStyle="Medium" BorderColor="#666666" BorderType="AllBorders">
                                    </Pdf>
                                </ExportSettings>
                                <GroupingSettings CaseSensitive="false" />
                                <MasterTableView AutoGenerateColumns="False" CommandItemDisplay="Top"
                                    EnableHeaderContextMenu="true" DataKeyNames="UserId">
                                    <CommandItemSettings ShowAddNewRecordButton="false"
                                        ShowRefreshButton="true" ShowExportToExcelButton="true"
                                        ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>
                                    <Columns>

                                        <telerik:GridTemplateColumn HeaderStyle-Width="2%" HeaderText=""
                                            DataField="" FilterControlAltText="" SortExpression=""
                                            UniqueName="" AllowFiltering="false">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkheader" runat="server" OnClick="selectAll(this)" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkUser" runat="server" Checked="false" />
                                                <asp:HiddenField ID="hdnUserid" runat="server" Value='<%#Eval("UserId") %>' />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>

                                        <telerik:GridBoundColumn DataField="DisplayName"
                                            FilterControlAltText="DisplayName" HeaderText="Display Name"
                                            SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                            <ColumnValidationSettings>
                                                <ModelErrorMessage Text="" />
                                            </ColumnValidationSettings>
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="EmployeeNumber"
                                            FilterControlAltText="Employee Id" HeaderText="Employee Id"
                                            SortExpression="EmployeeNumber" UniqueName="EmployeeNumber" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                            <ColumnValidationSettings>
                                                <ModelErrorMessage Text="" />
                                            </ColumnValidationSettings>
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Company"
                                            FilterControlAltText="Company" HeaderText="Company"
                                            SortExpression="Company" UniqueName="Company" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="CostCenter"
                                            FilterControlAltText="CostCenter" HeaderText="Cost Center"
                                            SortExpression="CostCenter" UniqueName="CostCenter" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Department"
                                            FilterControlAltText="Department" HeaderText="Department"
                                            SortExpression="Department" UniqueName="Department" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Division"
                                            FilterControlAltText="Division" HeaderText="Division"
                                            SortExpression="Division" UniqueName="Division" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Section"
                                            FilterControlAltText="Section" HeaderText="Section"
                                            SortExpression="Section" UniqueName="Section" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                        </telerik:GridBoundColumn>


                                        <telerik:GridDateTimeColumn DataType="System.DateTime" EnableTimeIndependentFiltering="true" DataFormatString="{0:dd-MM-yyyy}" DataField="EffectiveDate"
                                            FilterControlAltText="EffectiveDate" HeaderText="Effective Date"
                                            SortExpression="EffectiveDate" UniqueName="EffectiveDate" HeaderStyle-Width="8%" FilterControlWidth="65%">
                                            <ColumnValidationSettings>
                                                <ModelErrorMessage Text="" />
                                            </ColumnValidationSettings>
                                        </telerik:GridDateTimeColumn>


                                        <telerik:GridButtonColumn CommandArgument="Details" CommandName="Details" DataTextField="" ButtonType="LinkButton" HeaderText="Old Details" SortExpression="" UniqueName="" Text="Details" Exportable="false" HeaderStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
                                        </telerik:GridButtonColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="8%" FilterControlWidth="65%" HeaderText="Status"
                                            DataField="" FilterControlAltText="" SortExpression=""
                                            UniqueName="LabelStatus" AllowFiltering="false">
                                            <ItemTemplate>
                                                <asp:Label Font-Bold="true" ForeColor="#0000EE" ID="LabelStatus" runat="server" />
                                            </ItemTemplate>

                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </cc1:IACMRadGridControl>
                        </asp:Panel>

                        <br />

                        <div style="float: right">
                             <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-default btn-sm" Text="Refresh"
                            ToolTip="Refresh the page." OnClick="btnRefresh_Click" />
                            <asp:Button ID="Button1" Text="Submit" CssClass="btn btn-default btn-sm"
                                ToolTip="Change Department" runat="server" OnClick="btnSubmit_Click" />
                            &nbsp;
                                    <asp:Button ID="Button2" runat="server" CssClass="btn btn-default btn-sm" Text="Cancel"
                                        ToolTip="Go back to Provisioning Home page." OnClick="btnAPACCancel_Click" />


                        </div>
                    </div>
                </div>
               
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ouChangeRB" EventName="CheckedChanged" />
            <asp:AsyncPostBackTrigger ControlID="reOrgRB" EventName="CheckedChanged" />
            <asp:AsyncPostBackTrigger ControlID="onboardRB" EventName="CheckedChanged" />
            <asp:AsyncPostBackTrigger ControlID="rehireRB" EventName="CheckedChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow runat="server" ID="UserOldNewDetailsModalPopupExtender" Modal="true"  minWidth="1100px"  Title="User Details:"
        AutoSizeBehaviors="Width, Height" AutoSize="true" KeepInScreenBounds="true" VisibleOnPageLoad="false"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">

        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div>
                        <div class="modal-header">
                            <h3 class="modal-title">User Details:</h3>
                        </div>

                        <div class="modal-body">
                            <div class="form-horizontal left clearfix">
                                <div class="row-fluid">
                                    <div class="col-xs-6">
                                        <div class="form-group">
                                            <label class="col-xs-5">User Name:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtUserName" runat="server" Enabled="false" CssClass="description"></asp:Label>
                                            </div>
                                        </div>

                                        </div>
                                    </div>
                                </div>

                              <div class="form-horizontal left clearfix">
                                <div class="row-fluid">
                                    <div class="col-xs-6">

                                        <div id="trDepartment1" runat="server" class="form-group">
                                            <label class="col-xs-5">Old Department:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtDepartment" runat="server" Enabled="false" CssClass="description"></asp:Label>
                                            </div>
                                        </div>

                                        <div id="trDivision1" runat="server" class="form-group">
                                            <label class="col-xs-5">Old Division:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtDivision" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>

                                        <div id="trCompany1" runat="server" class="form-group">
                                            <label class="col-xs-5">Old Company:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtCompany" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>

                                        <div id="trCostCenter1" runat="server" class="form-group">
                                            <label class="col-xs-5">Old CostCenter:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtCostCenter" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>

                                        <div id="trOrganisation1" runat="server" class="form-group">
                                            <label class="col-xs-5">Old Organisation:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtOrganisation" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>
                                    </div>

                                     <div class="col-xs-6">

                                        <div id="trDepartment2" runat="server" class="form-group">
                                            <label class="col-xs-5">New Department:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtNewDepartment" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>
                                        <div id="trDivision2" runat="server" class="form-group">
                                            <label class="col-xs-5">New Division:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtNewDivision" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>
                                        <div id="trCompany2" runat="server" class="form-group">
                                            <label class="col-xs-5">New Company:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtNewCompany" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>

                                        <div id="trCostCenter2" runat="server" class="form-group">
                                            <label class="col-xs-5">New CostCenter:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtNewCostCenter" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>

                                        <div id="trOrganisation2" runat="server" class="form-group">
                                            <label class="col-xs-5">New Organisation:</label>
                                            <div class="col-xs-7">
                                                <asp:Label ID="txtNewOrganisation" runat="server" CssClass="description"></asp:Label>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>

                        <div class="mBottom15" style="float: right;">

                            <asp:Button ID="Button3" runat="server" OnClick="btnOk_Close" CssClass="btn btn-default btn-sm" Text="Ok" />
                        </div>
                    </div>




                </ContentTemplate>

            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>


</asp:Content>

