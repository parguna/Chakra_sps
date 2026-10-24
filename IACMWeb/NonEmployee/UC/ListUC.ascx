<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.Non_Employee.UC.ListUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/NUserDetailUC.ascx" TagPrefix="uc" TagName="UserDetailUC" %>
<%@ Register Src="~/NonEmployee/UC/DetailUC.ascx" TagPrefix="uc" TagName="Extend" %>

<script type="text/javascript">
    $(function () {
        $("body").on("click", ".uploadFile", function () {
            $(".Import").find(".ruFileInput").trigger("click");
        });

    });

    function SetValidToTodayDate() {
        var datepicker = $find("<%=DatePickerValidTo.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
    }

    function OnDateSelected(sender, e) {
        var ValidFromPicker = null; //
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

    function deleteInvalid(sender) {
        var invalidIndexes = [];
        $telerik.$(".ruUploadSuccess, .ruUploadFailure", sender.get_element()).each(function (index) {
            if ($telerik.$(this).hasClass('ruUploadFailure')) {
                invalidIndexes.push(index);
            }
        });
        while (invalidIndexes.length) {
            sender.deleteFileInputAt(invalidIndexes.pop());
        }
    }

</script>
<style>
    #ctl00_cplMain_ListUC_Users_ctl00_ctl02_ctl03_FilterCheckBox_IsActive{
        position: inherit;
        opacity:100;
        transform:scale(1.2);
    }
     input#ctl00_cplMain_ListUC_Users_ctl00_ctl02_ctl03_FilterCheckBox_IsActive+label.lbl {
    display: none;
    }
</style>
<cc1:IACMRadGridControl ID="Users" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" AllowCustomPaging="true"  PageSize="50"
     ClientSettings-EnablePostBackOnRowClick="true" DisableRowSelect="true"  Width="100%" Height="100%"
    OnNeedDataSource="Users_NeedDataSource" OnSelectedIndexChanged="Users_SelectedIndexChanged" OnItemCommand="Users_ItemCommand">
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling AllowScroll="true" UseStaticHeaders="true" />
    </ClientSettings>
    <ClientSettings Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true">
    </ClientSettings>
    <MasterTableView AutoGenerateColumns="False" Width="230%" CommandItemDisplay="Top" ShowHeadersWhenNoRecords="true" DataKeyNames="Id" NoMasterRecordsText="No Records Available" TableLayout="Fixed"  PageSize="50">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
   
        <Columns>
            <telerik:GridTemplateColumn HeaderText="User Name" DataField="UserName" FilterControlAltText="UserName" SortExpression="UserName" UniqueName="UserName" HeaderStyle-Width="120px" FilterControlWidth="82%">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkUsername" runat="server" Text='<%#Eval("UserName") %>' CommandArgument='<%#Container.ItemIndex%>' OnCommand="lnkEmployeeId_Command" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="EmployeeId" DataFormatString="{0}" FilterControlAltText="EmployeeId" HeaderText="Employee Id" SortExpression="EmployeeId" UniqueName="EmployeeId" HeaderStyle-Width="100px" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DisplayName" FilterControlAltText="DisplayName" HeaderText="Display Name" SortExpression="DisplayName" UniqueName="DisplayName" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="Company" FilterControlAltText="Company" HeaderText="Company" SortExpression="Company" UniqueName="Company" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="Department" FilterControlAltText="Department" HeaderText="Department" SortExpression="Department" UniqueName="Department" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="Branch" FilterControlAltText="Branch" HeaderText="Branch" SortExpression="Branch" UniqueName="Branch" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="SectionName" FilterControlAltText="SectionName" HeaderText="Section Name" SortExpression="SectionName" UniqueName="SectionName" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="OrganisationUnit" FilterControlAltText="OrganisationUnit" HeaderText="Unit" SortExpression="OrganisationUnit" UniqueName="OrganisationUnit" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="IsActive" FilterControlAltText="IsActive" HeaderText="Is Active" SortExpression="IsActive" UniqueName="IsActive" HeaderStyle-Width="60%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="JoinDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="JoinDate" HeaderText="Join Date" SortExpression="JoinDate" UniqueName="JoinDate" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridDateTimeColumn>
            <telerik:GridDateTimeColumn DataField="TerminationDate" DataFormatString="{0:dd-MM-yyyy}" FilterControlAltText="Last Service Date" HeaderText="Last Service Date" SortExpression="TerminationDate" UniqueName="TerminationDate" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridDateTimeColumn> 
            <telerik:GridBoundColumn DataField="ReportingOfficer" FilterControlAltText="ReportingOfficer" HeaderText="Reporting Officer" SortExpression="ReportingOfficer" UniqueName="ReportingOfficer" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CounterSigningOfficer" DataFormatString="{0}" FilterControlAltText="CounterSigningOfficer" HeaderText="CounterSigning Officer" SortExpression="CounterSigningOfficer" UniqueName="CounterSigningOfficer" HeaderStyle-Width="100%" FilterControlWidth="82%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ReferenceUser" FilterControlAltText="ReferenceUser" HeaderText="Reference User" SortExpression="ReferenceUser" UniqueName="ReferenceUser" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EmailId" FilterControlAltText="EmailId" HeaderText="Email Id" SortExpression="EmailId" UniqueName="EmailId" HeaderStyle-Width="100%" FilterControlWidth="86.5%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="EmployeeTypeString" FilterControlAltText="EmployeeTypeString" HeaderText="Employee Type" SortExpression="EmployeeTypeString" UniqueName="EmployeeTypeString" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="NCompany" FilterControlAltText="NCompany" HeaderText="Non Employee Company" SortExpression="NCompany" UniqueName="NCompany" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CostCenter" FilterControlAltText="CostCenter" HeaderText="Cost Center" SortExpression="CostCenter" UniqueName="CostCenter" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ProjectDescription" FilterControlAltText="ProjectDescription" HeaderText="Project / Description" SortExpression="ProjectDescription" UniqueName="ProjectDescription" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="MobileNumber" FilterControlAltText="MobileNumber" HeaderText="Mobile Number" SortExpression="MobileNumber" UniqueName="MobileNumber" HeaderStyle-Width="100%" FilterControlWidth="86.5%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CompanyCode" FilterControlAltText="CompanyCode" HeaderText="Company Code" SortExpression="CompanyCode" UniqueName="CompanyCode" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="DepartmentCode" FilterControlAltText="DepartmentCode" HeaderText="Department Code" SortExpression="DepartmentCode" UniqueName="DepartmentCode" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
             <telerik:GridBoundColumn DataField="BranchCode" FilterControlAltText="BranchCode" HeaderText="Branch Code" SortExpression="BranchCode" UniqueName="BranchCode" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="SectionCode" FilterControlAltText="SectionCode" HeaderText="Section Code" SortExpression="SectionCode" UniqueName="SectionCode" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn> 
            <telerik:GridBoundColumn DataField="UnitCode" FilterControlAltText="UnitCode" HeaderText="Unit Code" SortExpression="UnitCode" UniqueName="UnitCode" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="OrganisationCode" FilterControlAltText="OrganisationCode" HeaderText="Organisation Code" SortExpression="OrganisationCode" UniqueName="OrganisationCode" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="CreatedDate" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" FilterControlAltText="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" UniqueName="CreatedDate" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="CreatedBy" FilterControlAltText="CreatedBy" HeaderText="Created By" SortExpression="CreatedBy" UniqueName="CreatedBy" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" HeaderText="Status" SortExpression="Status" UniqueName="Status" HeaderStyle-Width="100%" FilterControlWidth="100%">
            </telerik:GridBoundColumn>
            <%--<telerik:GridTemplateColumn UniqueName="TemplateInfoColumn" HeaderStyle-Width="7%" FilterControlWidth="52%" AllowFiltering="false" HeaderText="Detail / Info">
                <ItemTemplate>
                    <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo" src="../../Images/action-buttons/detail.png"></asp:ImageButton>
                </ItemTemplate>
                <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>--%>
        </Columns>
    </MasterTableView>
</cc1:IACMRadGridControl>
<telerik:RadWindow runat="server" ID="UserDetail" Modal="true" Width="500px" Height="100%" Title="User Detail"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">User Detail</h3>
                </div>
                <div class="modal-body">
                    <uc:UserDetailUC runat="server" ID="UserDetailUC" />
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow runat="server" ID="ExtendEmployeeWin" Modal="true" Width="800px" Height="100%" Title="Extend Termination Date"
    RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
    <ContentTemplate>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div class="modal-header">
                    <h3 class="modal-title">User Detail</h3>
                </div>
                <div class="modal-body">
                    <div class="row-fluid clearfix">
                        <uc:Extend ID="ExtendUC" runat="server" />
                        <div class="form-horizontal left clearfix">
                            <div class="row-fluid form-group">
                                <div class="col-xs-12">
                                    <div class="from-group">
                                        <div class="col-xs-6">
                                            <div class="form-group">
                                                <label class="col-xs-4">Join Date:</label>
                                                <div class="col-xs-8">
                                                    <asp:Label ID="joinDate" runat="server" class="description"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="form-group required">
                                                <label for="validto" class="col-xs-4 control-label"><%= T("App.Employee.TerminationDate") %></label>
                                                <div class="col-xs-7">
                                                    <telerik:RadDatePicker runat="server" ID="DatePickerValidTo" onkeydown="handleDateTimePickerClickEvent(this,event)" EnableTyping="false" ShowPopupOnFocus="false" Skin="IACMSkin" EnableEmbeddedBaseStylesheet="false" EnableEmbeddedSkins="false"
                                                        DatePopupButton-Visible="true" Width="100%" CssClass="form-control" DateInput-CausesValidation="true">
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
                                        </div>
                                        <div class="col-xs-6">
                                            <div class="form-group">
                                                <label class="col-xs-4">Current Termination Date:</label>
                                                <div class="col-xs-8">
                                                    <asp:Label ID="terminationDate" runat="server" class="description"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="form-group required" id="frmAttach" runat="server">
                                                <asp:Label ID="lblAttachFile" AssociatedControlID="AsyncUploadAttach" runat="server" Text="Attach Files" CssClass="col-xs-4 control-label"></asp:Label>
                                                <div class="col-xs-7">
                                                    <telerik:RadAsyncUpload ViewStateMode="Enabled" RenderMode="Lightweight" runat="server" ID="AsyncUploadAttach" OnClientValidationFailed="OnClientValidationFailed"
                                                        HideFileInput="true" MaxFileSize="10485760" MaxFileInputsCount="1" MultipleFileSelection="Disabled" AllowedFileExtensions=".jpeg,.jpg,.png,.doc,.docx,.xls,.xlsx,.csv,.pdf,.txt,.zip"
                                                        Width="100%" EnableInlineProgress="false"
                                                        PostbackTriggers="ExtendSave" />

                                                    <span style="display: none" class="allowed-attachments">Allowed File Formats <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUploadAttach.AllowedFileExtensions ) %>)</span></span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="ExtendSave" class="btn btn-primary bordered-button" Text="Save" OnClick="ExtendSave_Click" />
                    <asp:Button runat="server" ID="ExtedCancel" class="btn btn-primary bordered-button" Text="Cancel" OnClick="ExtedCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
