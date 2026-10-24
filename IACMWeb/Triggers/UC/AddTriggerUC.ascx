<%@ Control Language="C#" AutoEventWireup="true" Inherits="DI.Chakra.Web.Triggers.UC.AddTriggerUC" CodeBehind="AddTriggerUC.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/UsrControl/Common/NUserDetailUC.ascx" TagPrefix="uc" TagName="NUserDetailUC" %>
<%@ Register Src="~/UsrControl/Common/OrgSearchUC.ascx" TagPrefix="uc1" TagName="OrgSearchUC" %>
<%@ Register Src="~/UsrControl/Common/DesignationSearchUC.ascx" TagPrefix="uc1" TagName="DesignationSearchUC" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>

<script type="text/javascript">
    function SetToday(Parameter) {
        var datepicker = $find("<%=EffectiveDatePicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();

        var datepicker = $find("<%=EffectiveDatePicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }
    function OnClientRequesting(sender, eventArgs) {
        var hiddenVal = $("#" + '<%= HiddenValue.ClientID %>').val();
        var context = eventArgs.get_context();
        context["hidden"] = hiddenVal;

    }

    $(function () {
        $("body").on("click", ".uploadFile", function () {
            $(".Import").find(".ruFileInput").trigger("click");
        });

    });


    function OnClientValidationFailed(sender, args) {
        var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);
        if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct
            if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                showAlert("Wrong Extension!", "Error");
            }
            else {
                showAlert("Wrong file size!", "Error");
            }
        }
        else {
            showAlert("not correct extension!", "Error");
        }
        setTimeout(function () { deleteInvalid(sender) }, 250);//wait for the file to actually be added to the async uploads array before trying to delete it
    }

</script>

<div class="content-wrapper">
    <div class="form">
        <asp:HiddenField ID="HiddenValue" runat="server" Value="" />
        <div class="form-horizontal left">
            <div class="row-fluid">
                <div class="col-xs-12">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-xs-3 control-label">Trigger</label>
                            <div class="col-xs-7">
                                <div id="groupBox" class="form-group" runat="server">
                                    <div style="width: 100%;">
                                        <div style="width: 45%; display: inline-block;">
                                            <asp:Label ID="Label5" runat="server" AssociatedControlID="OnBoardRadioButton">
                                                <asp:RadioButton ID="OnBoardRadioButton" runat="server" AutoPostBack="true" GroupName="EmployeeEvents" OnCheckedChanged="RadioButton_CheckedChanged" />
                                                <span class="lbl">On-Board</span>
                                            </asp:Label>
                                        </div>
                                        <%--<div style="width: 50%; display: inline-block;" id="divRejoin">
                                            <asp:Label ID="lblChkRejoin" runat="server" AssociatedControlID="chkRejoin">
                                                <asp:CheckBox runat="server" ID="chkRejoin"></asp:CheckBox>
                                                <span class="lbl">Rejoin</span>
                                            </asp:Label>
                                        </div>--%>
                                    </div>
                                    <asp:Label ID="Label1" runat="server" AssociatedControlID="RejoinRadioButton">
                                        <asp:RadioButton ID="RejoinRadioButton" runat="server" AutoPostBack="true" GroupName="EmployeeEvents" OnCheckedChanged="RadioButton_CheckedChanged" />
                                        <span class="lbl">Re-Join</span>
                                    </asp:Label>
                                    <br />
                                    <asp:Label ID="Label6" runat="server" AssociatedControlID="OffBoardRadioButton">
                                        <asp:RadioButton ID="OffBoardRadioButton" runat="server" AutoPostBack="true" GroupName="EmployeeEvents" OnCheckedChanged="RadioButton_CheckedChanged" />
                                        <span class="lbl">Off-Board</span>
                                    </asp:Label>
                                    <br />
                                    <asp:Label ID="Label7" runat="server" AssociatedControlID="TransferRadioButton">
                                        <asp:RadioButton ID="TransferRadioButton" runat="server" AutoPostBack="true" GroupName="EmployeeEvents" OnCheckedChanged="RadioButton_CheckedChanged" />
                                        <span class="lbl">Transfer</span>
                                    </asp:Label>
                                    <br />
                                    <asp:Label ID="Label8" runat="server" AssociatedControlID="ReorgRadioButton">
                                        <asp:RadioButton ID="ReorgRadioButton" runat="server" AutoPostBack="true" GroupName="EmployeeEvents" OnCheckedChanged="RadioButton_CheckedChanged" />
                                        <span class="lbl">Re-Organisation</span>
                                    </asp:Label>
                                    <br />
                                    <asp:Label ID="lblReJoin" runat="server" AssociatedControlID="PropagationRadioButton">
                                        <asp:RadioButton ID="PropagationRadioButton" runat="server" AutoPostBack="true" GroupName="EmployeeEvents" OnCheckedChanged="RadioButton_CheckedChanged" />
                                        <span class="lbl">Propagation</span>
                                    </asp:Label>
                                    <br />
                                </div>
                            </div>
                        </div>
                        <div class="form-group required">
                            <asp:Label ID="Label3" runat="server" CssClass="col-xs-3 control-label" AssociatedControlID="UserNameAutoCompleteBox">User Name</asp:Label>
                            <div class="col-xs-7 form-group">
                                <telerik:RadAutoCompleteBox ID="UserNameAutoCompleteBox" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                                    InputType="Token" AutoPostBack="true" OnEntryAdded="UserNameAutoCompleteBox_EntryAdded" OnClientRequesting="OnClientRequesting" OnEntryRemoved="UserNameAutoCompleteBox_EntryRemoved">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Path="../Services/AutoCompleteDataService.svc" Method="GetFullTimeUsers" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>
                        <div class="form-group required">
                            <label class="col-xs-3 control-label">Effective Date</label>
                            <div class="col-xs-7 form-group">
                                <telerik:RadDatePicker runat="server" ID="EffectiveDatePicker" onkeydown="handleDateTimePickerClickEvent(this,event)" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control">
                                    <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('EButton')" />
                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                        BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                        <div class="form-group required" id="terminationDateDIV" runat="server" visible="false">
                            <label class="col-xs-3 control-label">Termination Date</label>
                            <div class="col-xs-7 form-group">
                                <telerik:RadDatePicker runat="server" ID="TerminationDatePicker" onkeydown="handleDateTimePickerClickEvent(this,event)" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                    DatePopupButton-Visible="true" Width="100%" CssClass="form-control">
                                    <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                        <FooterTemplate>
                                            <div style="width: 100%; text-align: center; background-color: Gray;">
                                                <asp:Button ID="EButton1" runat="server" Width="100%" Text="Today" OnClientClick="SetToday('EButton1')" />
                                            </div>
                                        </FooterTemplate>
                                    </Calendar>
                                    <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                        BorderStyle="NotSet" Label="">
                                    </DateInput>
                                </telerik:RadDatePicker>
                            </div>
                        </div>
                        <div class="form-group required" id="divCompany" runat="server" visible="false">
                            <label class="col-xs-3 control-label"><%= T("App.Employee.Company") %></label>
                            <div class="col-xs-7 form-group">
                                <uc1:GenericAutoCompleteBox runat="server" ID="autoCompleteCompany" ServiceMethodName="GetCompanies" ServicePath="~/Services/AutoCompleteDataService.svc" OnClientRequesting="OnClientRequesting" OnEntryAdded="AutoCompleteBox_EntryAdded" OnEntryRemoved="AutoCompleteBox_EntryRemoved" />
                            </div>
                        </div>
                       <div class="form-group required" id="divDepartment" runat="server" visible="false">
                            <label class="col-xs-3 control-label">Department</label>
                            <div class="col-xs-7 form-group">
                                 <telerik:RadAutoCompleteBox ID="autoCompleteDepartment" 
                                EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting" OnEntryAdded="AutoCompleteBox_EntryAdded" OnEntryRemoved="AutoCompleteBox_EntryRemoved"
                                CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetDepartMents" Path="~/Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                                <%--<uc1:GenericAutoCompleteBox runat="server" ID="autoCompleteDepartment" ServiceMethodName="GetDepartMents" ServicePath="~/Services/AutoCompleteDataService.svc"  />--%>
                            </div>
                        </div>
                        <%--<div class="form-group" id="DivDesignationSearchUc" runat="server">
                            <label class="col-xs-3 control-label">Designation :</label>
                            <div class="">
                                <uc1:DesignationSearchUC runat="server" ID="DesignationSearchUC" />
                            </div>
                        </div>--%>
                         <div class="form-group required" id="divDivision" runat="server" visible="false">
                            <label class="col-xs-3 control-label">Branch</label>
                            <div class="col-xs-7 form-group">
                                <telerik:RadAutoCompleteBox ID="autoCompleteDivision" 
                                EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting" OnEntryAdded="AutoCompleteBox_EntryAdded" OnEntryRemoved="AutoCompleteBox_EntryRemoved"
                                CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetDivisionsByDeptID" Path="~/Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                                <%--<uc1:GenericAutoCompleteBox runat="server"
                                    ID="autoCompleteDivision" ServiceMethodName="GetDivisionsByDeptID" ServicePath="~/Services/AutoCompleteDataService.svc" />--%>
                            </div>
                        </div>

                         <div class="form-group required" id="divSection" runat="server" visible="false">
                            <label class="col-xs-3 control-label">Section</label>
                            <div class="col-xs-7 form-group">
                                <telerik:RadAutoCompleteBox ID="autoCompleteSection"
                                EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting" OnEntryAdded="AutoCompleteBox_EntryAdded" OnEntryRemoved="AutoCompleteBox_EntryRemoved"
                                CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetSectionsByBranchCode" Path="~/Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                                <%--<uc1:GenericAutoCompleteBox runat="server"
                                    ID="autoCompleteSection" ServiceMethodName="GetSectionsByBranchCode" ServicePath="~/Services/AutoCompleteDataService.svc" />--%>
                            </div>
                        </div>

                         <div class="form-group" id="div1" runat="server" visible="false">
                            <label class="col-xs-3 control-label">Unit</label>
                            <div class="col-xs-7 form-group">
                                 <telerik:RadAutoCompleteBox ID="autoCompleteUnit"
                                EmptyMessage="Enter * to view full list" runat="server" DropDownHeight="200px" DropDownWidth="300px" InputType="Token" OnClientRequesting="OnClientRequesting" OnEntryAdded="AutoCompleteBox_EntryAdded" OnEntryRemoved="AutoCompleteBox_EntryRemoved"
                                CssClass="form-control" AutoPostBack="true">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="GetUnitsBySectionCode" Path="~/Services/AutoCompleteDataService.svc" />
                                </telerik:RadAutoCompleteBox>
                                <%--<uc1:GenericAutoCompleteBox runat="server"
                                    ID="autoCompleteUnit" ServiceMethodName="GetUnitsBySectionCode" ServicePath="~/Services/AutoCompleteDataService.svc" />--%>
                            </div>
                        </div>

                        <div class="form-group required" id="divCostCenter" runat="server" visible="false">
                            <label class="col-xs-3 control-label"><%= T("App.Employee.CostCenter") %></label>
                            <div class="col-xs-7 form-group">
                                <uc1:GenericAutoCompleteBox runat="server"
                                    ID="autoCompleteCostCenter" ServiceMethodName="GetCostCenterCodeAndTextByCompanyID" ServicePath="~/Services/AutoCompleteDataService.svc" />
                            </div>
                        </div>
                        <div class="form-group" id="divCostDistribution" runat="server" visible="false">
                            <label class="col-xs-3 control-label">Cost Distribution</label>
                            <div class="col-xs-7 form-group">
                                <uc1:GenericAutoCompleteBox runat="server" ID="autoCompleteCostDistribution" ServiceMethodName="GetCostCenterCodeAndTextByCompanyID" ServicePath="~/Services/AutoCompleteDataService.svc" />
                            </div>
                        </div>
                       <%-- <div class="form-group required" id="employeeTypeDiv" runat="server" visible="false">
                            <label class="col-xs-3 control-label"><%= T("App.Employee.EmployeeType") %></label>
                            <div class="col-xs-7 form-group">
                                <asp:DropDownList ID="TypeofEmployee" runat="server" data-width="100%" CssClass="form-control selectpicker"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group required" id="SupervisorUser" runat="server" visible="false">
                            <asp:Label ID="Label2" runat="server" CssClass="col-xs-3 control-label" AssociatedControlID="UserNameAutoCompleteBox">Supervisor Name</asp:Label>
                            <div class="col-xs-7 form-group">
                                <telerik:RadAutoCompleteBox ID="SuperVisorAutoCompleteBox" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                                    InputType="Token" AutoPostBack="true" OnEntryAdded="UserNameAutoCompleteBox_EntryAdded" OnClientRequesting="OnClientRequesting" OnEntryRemoved="UserNameAutoCompleteBox_EntryRemoved">
                                    <TextSettings SelectionMode="Single" />
                                    <WebServiceSettings Path="../Services/AutoCompleteDataService.svc" Method="GetFullTimeUsers" />
                                </telerik:RadAutoCompleteBox>
                            </div>
                        </div>--%>
                        <%--<div class="form-group required" id="frmAttach" runat="server">
                            <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-3 control-label"></asp:Label>
                            <div class="col-xs-7  form-group">
                                <telerik:RadAsyncUpload ViewStateMode="Enabled" RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                                    HideFileInput="true" MaxFileSize="10485760" MaxFileInputsCount="1" MultipleFileSelection="Disabled" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                    Width="100%" EnableInlineProgress="false" />

                                <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                            </div>
                        </div>--%>
                    </div>
                    <div class="col-xs-6">
                        <uc:NUserDetailUC runat="server" ID="NUserDetailUC" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="buttons text-right nav-rectangle">
    <asp:Button ID="Button2" runat="server" CssClass="btn btn-default btn-sm cancel" Text="Cancel" PostBackUrl="~/Triggers/List.aspx" />
</div>
