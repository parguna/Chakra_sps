<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdhocNovaStep2UC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.App.Step2UC.AdhocNovaStep2UC" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/NACR/Create/App/Step2UC/SecurityQuestionUC.ascx" TagPrefix="uc1" TagName="SecurityQuestionUC" %>
<%@ Register Src="~/NACR/Create/App/Special/SpecialAccountWarning.ascx" TagPrefix="uc1" TagName="SpecialAccountWarning" %>

<style type="text/css">
    .auto-style1 {
        font-size: xx-small;
    }

    .label {
        font-size: 100%;
    }
</style>
<script type="text/javascript">
    function OnClientRequesting(sender, args) {
        //debugger;
        var context = args.get_context()
        context["AdhocActionType"] = "<%= AdhocActionType %>";
        context["userId"] = "<%= UserId %>";
        context["endpointId"] = "<%= EndpointSystemId %>";
         
        var mType = $get('<%=ddlMatrixType.ClientID%>').value;
        context["matrixType"] = mType;
        if (mType == 2) {
            context["DepartmentCode"] = $get('<%=ddlDepartment.ClientID%>').value;
        } else {
            context["DepartmentCode"] = "";
        }

         if (mType == 1) {
             context["SAPSection"] = $get('<%=ddlSAPSection.ClientID%>').value;
         }
         else {
            context["SAPSection"] = "";
        }
    }
</script>
<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="row-fluid">
            <div class="col-xs-6">

                <div runat="server" visible="false" id="divExistingFlinsAccount">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">FLINS Account:</label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="ddlFlinsAccount" CssClass="form-control selectpicker" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>



                <div runat="server" visible="false" id="SharedPanelFLINS">


                    <div class="form-group required">
                        <label class="col-xs-3 control-label">SAP UserType</label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="SAPUserTypeDropDown" CssClass="form-control selectpicker" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Account Name</label>
                        <div class="col-xs-8">
                            <telerik:RadTextBox ID="txtAcntName" MaxLength="12" runat="server" CssClass="form-control">
                            </telerik:RadTextBox>
                        </div>
                         <div class="col-xs-3"></div>
                            <div class="col-xs-8">
                                <uc1:specialaccountwarning runat="server" id="SpecialAccountWarning" />
                            </div>
                    </div>

                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Description</label>
                        <div class="col-xs-8">
                            <telerik:RadTextBox ID="txtDiscription" TextMode="MultiLine" runat="server" CssClass="form-control" MaxLength="40">
                            </telerik:RadTextBox>
                        </div>
                    </div>

                </div>





                <div class="form-group required" runat="server" visible="false" id="divEndpointUser">
                    <label class="col-xs-3 control-label">EndPoint User Name</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlEndpointUser" CssClass="form-control selectpicker"  AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlEndpointUser_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>



                <div id="divSelectMatType" runat="server" class="form-group required">
                    <label class="col-xs-3 control-label">Select Matrix Type</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlMatrixType" AutoPostBack="true" CssClass="form-control selectpicker" runat="server" OnSelectedIndexChanged="ddlMatrixType_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="form-group required" runat="server" visible="false" id="divManagementDepartment">
                    <label class="col-xs-3 control-label">Department</label>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlDepartment" AutoPostBack="true" runat="server" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                </div>
                <div runat="server" id="divExceptionalRole" visible="false">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Company</label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="ddlCompanyCode" AutoPostBack="true" runat="server" CssClass="form-control selectpicker" OnSelectedIndexChanged="ddlCompanyCode_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Role</label>
                        <div class="col-xs-8">
                            <telerik:RadComboBox ID="ExceptionalRoleComboBox" Style="width: 100% !important" DropDownAutoWidth="Enabled" runat="server" CheckBoxes="true" CausesValidation="true"   AutoPostBack="true" OnSelectedIndexChanged="ExceptionalRoleComboBox_SelectedIndexChanged">
                            </telerik:RadComboBox>
                        </div>
                    </div>
                </div>
                <div runat="server" visible="false" id="divUserCategory">
                   
                    <div class="form-group">
                        <label class="col-xs-3 control-label">Company Name(Code)</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" Text="" ID="lblCompanyName"></asp:Label>
                        </div>
                    </div>

                     <div class="form-group">
                        <label class="col-xs-3 control-label">Organisation Name(Code)</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" ID="lblSectionName"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-xs-3 control-label">Section</label>
                        <div class="col-xs-8">
                            <asp:DropDownList ID="ddlSAPSection" CssClass="form-control selectpicker"  DropDownAutoWidth="Enabled" runat="server"   CausesValidation="true"   AutoPostBack="true" OnSelectedIndexChanged="ddlSAPSection_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                </div>
                <div runat="server" id="divRoleDescription" visible="false">
                    <div class="form-group required">
                        <label class="col-xs-3 control-label">Jobs(Positions)</label>
                        <div class="col-xs-8">
                            <telerik:RadAutoCompleteBox ID="PositionAutoCompleteBox"
                                EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting" OnEntryAdded="PositionAutoCompleteBox_EntryAdded" OnEntryRemoved="PositionAutoCompleteBox_EntryRemoved"
                                CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetNovaJobByMatrixType" Path="../../../../Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                        <%--<div class="col-xs-3">
                            <h4><span class="label glyphicon glyphicon-info-sign" aria-hidden="true">&nbsp;Enter * to view full list</span></h4>
                        </div>--%>
                    </div>
                </div>
                <div class="form-group" id="trSecurityQstns" runat="server" visible="false">
                    <div class="col-xs-3">
                    </div>
                    <div class="col-xs-8">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Panel ID="Panel2" runat="server">
                                    <uc1:SecurityQuestionUC runat="server" ID="SecurityQuestionUC1" OnSecurityQuestionPopUp="SecurityQuestionUC_SecurityQuestionPopUp" OnSecurityResponseSaveClicked="SecurityQuestion_SecurityResponseSaveClicked" LinkText="Service account compliance check" />
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-xs-3">
                    </div>
                    <div class="col-xs-8">
                        <asp:Button ID="btnAddRequest" runat="server" Text="Add" Visible="false" CssClass="btn btn-default" OnClick="btnAddRequest_Click"></asp:Button>
                        <asp:Button ID="btnSave" runat="server" OnClick="AddToCart" Text="Save Request" CssClass="btn btn-default" Visible="false"/>
                    </div>
                </div>

            </div>
        </div>
    </div>
    <div class="row-fluid">
        <div class="col-xs-12">
            <telerik:RadGrid ID="NovaRoleGrid" runat="server" Visible="false" AllowPaging="false" AllowSorting="false" CellSpacing="0"
                GridLines="None" ShowFooter="false" OnItemCommand="NovaRoleGrid_ItemCommand"
                MasterTableView-EnableNoRecordsTemplate="true" MasterTableView-NoDetailRecordsText="No Records Available" tablelayout="Fixed" CssClass="RadGridtextWrap">
                <GroupingSettings CaseSensitive="false" />
                <ClientSettings>
                    <Scrolling AllowScroll="true" ScrollHeight="80px" UseStaticHeaders="true" SaveScrollPosition="True" />
                </ClientSettings>
                <MasterTableView AutoGenerateColumns="false" DataKeyNames="Id" TableLayout="Fixed"  Width="100%">
                    <Columns>
                        <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="NovaMatrixType" HeaderText="MatrixType" ItemStyle-HorizontalAlign="Center" UniqueName="NovaMatrixType">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderStyle-Width="15%" DataField="JobDescription" HeaderText="Description" ItemStyle-HorizontalAlign="Center" UniqueName="JobDescription">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblRoleName" Text='<%# Convert.ToString(Eval("JobDescription"))+"("+Convert.ToString(Eval("RoleName"))+")" %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <%-- <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="RoleName" HeaderText="RoleName" ItemStyle-HorizontalAlign="Center" UniqueName="RoleName">
                                </telerik:GridBoundColumn>--%>
                        <%--<telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="JobDescription" HeaderText="Description" ItemStyle-HorizontalAlign="Center" UniqueName="JobDescription">
                                </telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="CompanyCode" HeaderText="CompanyCode" ItemStyle-HorizontalAlign="Center" UniqueName="CompanyCode">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="SectionCode" HeaderText="SectionCode" ItemStyle-HorizontalAlign="Center" UniqueName="SectionCode">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="15%" DataField="DepartmentCode" HeaderText="DepartmentCode" ItemStyle-HorizontalAlign="Center" UniqueName="DepartmentCode">
                        </telerik:GridBoundColumn>
                        <telerik:GridButtonColumn ConfirmDialogType="RadWindow" ConfirmTitle="Delete" UniqueName="Delete" ConfirmText="Are you sure you want to delete?" CommandName="Delete" ButtonType="ImageButton" ItemStyle-CssClass="action-delete" ItemStyle-Width="1px" HeaderText="">
                            <HeaderStyle Width="50px" />
                        </telerik:GridButtonColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </div>

</div>

