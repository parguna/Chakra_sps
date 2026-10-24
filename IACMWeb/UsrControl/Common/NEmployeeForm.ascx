<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NEmployeeForm.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.NEmployeeForm" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/OrgSearchUC.ascx" TagPrefix="uc1" TagName="OrgSearchUC" %>
<%@ Register Src="~/UsrControl/Common/DesignationSearchUC.ascx" TagPrefix="uc1" TagName="DesignationSearchUC" %>
<%@ Register Src="~/UsrControl/Common/EnterprisePasswordPolicy.ascx" TagPrefix="uc1" TagName="EnterprisePasswordPolicy" %>
<%@ Register Src="~/UsrControl/Common/GenericAutoCompleteBox.ascx" TagPrefix="uc1" TagName="GenericAutoCompleteBox" %>


<script type="text/javascript">
    $(function () {
        $("body").on("click", ".uploadFile", function () {
            $(".Import").find(".ruFileInput").trigger("click");
        });

    });

    jQuery(function ($) {
        var focusedElementId = "";
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        prm.add_beginRequest(function (source, args) {
            var fe = document.activeElement;
            if (fe != null) {
                focusedElementId = fe.id;
            } else {
                focusedElementId = "";
            }
        });

        prm.add_endRequest(function (source, args) {
            if (focusedElementId != "") {
                $("#" + focusedElementId).focus();
            }
        });
    });

    function clientFileUploaded(sender, args) {
    }


    function SetValidFromTodatDate(obj) {
        var datepicker = $find("<%=DatePickerValidFrom.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);

    }

    function SetValidToTodayDate() {
        var datepicker = $find("<%=DatePickerValidTo.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
    }

    function OnDateSelected(sender, e) {
        var ValidFromPicker = $find("<%= DatePickerValidFrom.ClientID %>");
        var ValidToPicker = $find("<%= DatePickerValidTo.ClientID %>");
        var now = new Date();
        if (ValidFromPicker != null && ValidToPicker != null) {
            ValidToPicker.clear();
            now = e.get_newDate() > now ? e.get_newDate() : now;
            console.log("now", now);
            if (now != null) {
                ValidToPicker.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            }
        }
    }

</script>
<div class="content-wrapper">
    <div class="form">
        <div class="form-horizontal left readonly">
            <div class="row-fluid">

                <%-- <div id="photoDiv" runat="server" class="col-xs-2">
                    <div class="form-group photo-container" id="PhotoContainer" runat="server">
                        <div class="profile-image">
                            <asp:ImageButton ID="imgDelete" ImageUrl="~/Images/action-buttons/remove.png" runat="server" CssClass="remove_button remove_close"
                                Width="12px" Height="12px" Style="display: none;" OnClick="imgDelete_Click" />
                            <img id="ProfileImg" runat="server" src="../../images/profile-placeholder.png" width="130" height="160" />
                        </div>
                        <span>
                            <a href="javascript:;" id="uploadFile" class="uploadFile">Edit</a>
                        </span>
                        <telerik:RadAsyncUpload runat="server" CssClass="Import" ID="fileUploadField" MultipleFileSelection="Disabled" OnFileUploaded="fileUploadField_FileUploaded"
                            HideFileInput="true" AllowedFileExtensions="png,jpeg,jpg" AutoAddFileInputs="false" OnClientFilesUploaded="clientFileUploaded" Style="opacity: 0;">
                            <Localization Select="Edit" />
                        </telerik:RadAsyncUpload>
                        <span>
                            <a href="../../OrgChart/OrgChartList.aspx" runat="server" id="OrgChartLink">My Organization Chart</a>
                        </span>
                    </div>
                </div>--%>

                <div class="col-xs-5">
                    <%--<div class="form-group required">
                        <label for="identificationno" class="col-xs-4 control-label"><%= T("App.Employee.IdentificationNo") %></label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtIdentificationNo" MaxLength="25" CssClass="form-control numeric" CausesValidation="true"></asp:TextBox>
                        </div>
                    </div>--%>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                        <ContentTemplate>
                            <%-- <div class="form-group required">
                                <label for="projectdescription" class="col-xs-4 control-label">First Name</label>
                                <div class="col-xs-7">
                                    <asp:TextBox runat="server" ID="txtFirstName" CssClass="form-control" CausesValidation="true" AutoPostBack="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group required">
                                <label for="projectdescription" class="col-xs-4 control-label">Last Name</label>
                                <div class="col-xs-7">
                                    <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" CausesValidation="true" AutoPostBack="true"></asp:TextBox>
                                </div>
                            </div>--%>
                            <div class="form-group required">
                                <label for="knownas" class="col-xs-4 control-label">Name (as per NRIC)</label>
                                <div class="col-xs-7">
                                    <asp:TextBox runat="server" ID="txtDisplayName" CssClass="form-control numeric" CausesValidation="true"></asp:TextBox>
                                </div>
                            </div>
                        </ContentTemplate>
                        <%--  <triggers>
                            <asp:AsyncPostBackTrigger ControlID="txtFirstName" EventName="TextChanged" />
                            <asp:AsyncPostBackTrigger ControlID="txtLastName" EventName="TextChanged" />
                        </triggers>--%>
                    </asp:UpdatePanel>
                    <div class="form-group required">
                        <label for="projectdescription" class="col-xs-4 control-label">Project/Description</label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtProjectDescription" CssClass="form-control numeric" CausesValidation="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group required" id="SupervisorUser" runat="server">
                        <label for="supervisoruser" class="col-xs-4 control-label">Reference User</label>
                        <div class="col-xs-7">
                            <telerik:RadAutoCompleteBox ID="ReferenceUsersAutoCompleteBox" CssClass="form-control" runat="server"
                                OnClientEntryAdding="OnClientEntryAddingHandler" InputType="Token" OnEntryAdded="ReferenceUsersAutoCompleteBox_EntryAdded"
                                OnEntryRemoved="ReferenceUsersAutoCompleteBox_EntryRemoved">
                                <TextSettings SelectionMode="Single" />
                                <WebServiceSettings Method="getReferencePermanentUser" Path="~/Services/AutoCompleteDataService.svc" />
                            </telerik:RadAutoCompleteBox>
                        </div>
                    </div>
                    <div class="form-group " id="frmCompany" runat="server">
                        <label for="company" class="col-xs-4 control-label"><%= T("App.Employee.Company") %></label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtCompany" CssClass="form-control" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group ">
                        <label for="organizationunit" class="col-xs-4 control-label"><%= T("App.Employee.Department") %></label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtDepartment" CssClass="form-control" Enabled="false"></asp:TextBox>
                        </div>
                    </div> 
                    
                   
                    <div class="form-group ">
                        <label for="division" class="col-xs-4 control-label">Branch</label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtDivision" CssClass="form-control numeric"></asp:TextBox>
                        </div>
                    </div>

                     <div class="form-group ">
                        <label for="division" class="col-xs-4 control-label">Section</label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtSection" CssClass="form-control numeric"></asp:TextBox>
                        </div>
                    </div>

                      <div class="form-group ">
                        <label for="division" class="col-xs-4 control-label">Unit</label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtUnit" CssClass="form-control numeric"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group ">
                        <label for="orgunit" class="col-xs-4 control-label">Organisation</label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtOrgUnit" CssClass="form-control numeric"></asp:TextBox>
                        </div>
                    </div>

                </div>
            </div>

            <div class="col-xs-5">
                <%--<div class="form-group">
                    <label for="employeeno" class="col-xs-4 control-label"><%= T("App.Employee.EmployeeNo") %></label>
                    <div class="col-xs-7">
                        <asp:TextBox runat="server" ID="txtEmployeeNo" CssClass="form-control" MaxLength="25" Enabled="true"></asp:TextBox>
                    </div>
                </div>--%>
                <div class="form-group required">
                    <label for="username" class="col-xs-4 control-label">Non Employee Company</label>
                    <div class="col-xs-7">
                        <asp:TextBox runat="server" ID="txtCompanyOfNonEmployee" CssClass="form-control" MaxLength="100"></asp:TextBox>
                    </div>
                </div>



                <div class="form-group required">
                    <label for="employeetype" class="col-xs-4 control-label"><%= T("App.Employee.EmployeeType") %></label>
                    <div class="col-xs-7">
                        <asp:DropDownList ID="TypeofEmployee" runat="server" data-width="100%" CssClass="form-control selectpicker">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="form-group required">
                    <label for="txtMobileNumber" class="col-xs-4 control-label numeric"><%= T("App.Employee.MobileNumber") %></label>
                    <div class="col-xs-7">
                        <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="form-control"
                            MaxLength="8" onkeypress='validate(event)'></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label for="knownas" class="col-xs-4 control-label">Email</label>
                    <div class="col-xs-7">
                        <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control numeric"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group">
                    <label for="telephoneno" class="col-xs-4 control-label numeric"><%= T("App.Employee.TelephoneNumber") %></label>
                    <div class="col-xs-7">
                        <asp:TextBox ID="txtTelephoneNo" runat="server" CssClass="form-control" Enabled="false"
                            MaxLength="25" onkeypress='validate(event)'></asp:TextBox>
                    </div>
                </div>
                <div class="form-group required">
                    <label for="joindate" class="col-xs-4 control-label"><%= T("App.Employee.JoinDate") %></label>
                    <div class="col-xs-7">
                        <telerik:RadDatePicker runat="server" onkeydown="handleDateTimePickerClickEvent(this,event)" ID="DatePickerValidFrom" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control">
                            <ClientEvents OnDateSelected="OnDateSelected"></ClientEvents>
                            <Calendar ID="Calendar1" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                <FooterTemplate>
                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetValidFromTodatDate()" />

                                    </div>
                                </FooterTemplate>
                            </Calendar>
                            <DateInput ID="DateInput1" ReadOnly="false" runat="server" ToolTip="Date input For Start Date"
                                BorderStyle="NotSet" Label="">
                            </DateInput>
                        </telerik:RadDatePicker>
                        <asp:RequiredFieldValidator runat="server" ID="RFVL" ControlToValidate="DatePickerValidFrom"
                            ErrorMessage="Please Enter the Date" class="error-message"></asp:RequiredFieldValidator>
                    </div>
                </div>
              
            </div>
            <div class="col-xs-5">
                <div class="form-group required">
                    <label for="validto" class="col-xs-4 control-label"><%= T("App.Employee.TerminationDate") %></label>
                    <div class="col-xs-7">
                        <telerik:RadDatePicker runat="server" ID="DatePickerValidTo" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                            DatePopupButton-Visible="true" Width="100%" CssClass="form-control" DateInput-CausesValidation="true" MaxDate="2099-12-31">
                            <Calendar ID="Calendar2" runat="server" ShowRowHeaders="False" EnableKeyboardNavigation="true" CalendarTableStyle-VerticalAlign="Middle" DayStyle-VerticalAlign="Middle" DayOverStyle-VerticalAlign="Middle">
                                <FooterTemplate>
                                    <div style="width: 100%; text-align: center; background-color: Gray;">
                                        <asp:Button ID="EButton" runat="server" Width="100%" Text="Today" OnClientClick="SetValidToTodayDate()" />

                                    </div>
                                </FooterTemplate>
                            </Calendar>
                            <DateInput ID="DateInput2" ReadOnly="false" runat="server" ToolTip="Date input For End Date"
                                BorderStyle="NotSet" Label="">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </div>
                </div>
                <div class="form-group required" id="divExtension" runat="server">
                    <label for="ExtensionCheckBox" class="col-xs-4 control-label">Extension / Shortening</label>
                    <div class="col-xs-7">
                        <asp:CheckBox ID="ExtensionCheckBox" runat="server" AutoPostBack="true" OnCheckedChanged="ExtensionCheckBox_CheckedChanged" />
                    </div>
                </div>

                <%--<div class="form-group">
                    <div class="col-xs-4">
                    </div>
                    <div class="col-xs-7">
                        <asp:HyperLink Target="_blank" ID="AddNewCompany" NavigateUrl="~/Admin/CompanyMaster/List.aspx" runat="server" Text="Add New Company">
                        </asp:HyperLink>
                    </div>
                </div>--%>

                <%--<div class="form-group">
                    <label for="txtMobileNo" class="col-xs-4 control-label numeric"><%= T("App.Employee.MobileNumber") %></label>
                    <div class="col-xs-7">
                        <asp:TextBox ID="txtMobileNo" runat="server" CssClass="form-control" MaxLength="25" onkeypress='validate(event)'></asp:TextBox><br />
                    </div>
                </div>--%>
                   <div class="form-group">
                        <label for="costcenter" class="col-xs-4 control-label">Cost Center</label>
                        <div class="col-xs-7">
                            <asp:TextBox runat="server" ID="txtCostCenter" CssClass="form-control numeric"></asp:TextBox>
                        </div>
                    </div>
            </div>
            <div class="row-fluid" id="divInfoMsg" runat="server">
                <div class="col-xs-12">
                     <b>
                    <p>  <span class="glyphicon glyphicon-info-sign" style="font-size: 24px"></span> <asp:Label ID="lblInformationMessage" runat="server"></asp:Label> </p></b>
                </div>
            </div>
        </div>
    </div>
</div>

<telerik:RadWindow ID="DisplayPolicy" runat="server" VisibleStatusbar="false" Height="550px" Width="500px" RenderMode="Lightweight" Behaviors="Close"
    Modal="true" EnableViewState="true" VisibleTitlebar="false">
    <ContentTemplate>
        <uc1:EnterprisePasswordPolicy runat="server" ID="EnterprisePasswordPolicy1" />
        <div class="modal-footer">
            <asp:Button runat="server" ID="ok" CssClass="btn btn-primary bordered-button" Text="OK" UseSubmitBehavior="false" OnClick="ok_Click" />
        </div>
    </ContentTemplate>
</telerik:RadWindow>



<div class="buttons text-right nav-rectangle">
    <asp:Button ID="btn_Cancel" runat="server" CssClass="btn btn-default btn-sm" UseSubmitBehavior="false" Text="Cancel" OnClick="btn_Cancel_Click" />
</div>
