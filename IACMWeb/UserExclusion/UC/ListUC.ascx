<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListUC.ascx.cs" Inherits="DI.Chakra.Web.UserExclusion.UC.ListUC" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/UserSearchUC.ascx" TagPrefix="uc1" TagName="UserSearchUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Src="~/UsrControl/Common/NUserDetailUC.ascx" TagPrefix="uc" TagName="NUserDetailUC" %>

<script type="text/javascript">
    $(function () {
        $("body").on("click", ".uploadFile", function () {
            $(".Import").find(".ruFileInput").trigger("click");
        });

    });
    <%--function SetToday(Parameter) {
        var datepicker = $find(Parameter == "VFButton" ? "<%=EffectiveDate.ClientID%>" : "<%=EffectiveDate.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }
    function OnDateSelected(sender, e) {
        var EffectiveDate = $find("<%= EffectiveDate.ClientID %>");
        var now = new Date();
        if (EffectiveDate != null) {
            EffectiveDate.set_minDate(new Date(now.getFullYear(), now.getMonth(), now.getDate()));
            now = e.get_newDate();
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
    }--%>

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

<cc1:IACMRadGridControl ID="UserExclutionGrid" runat="server" OnNeedDataSource="UserExclutionGrid_NeedDataSource" OnItemCommand="UserExclutionGrid_ItemCommand" AllowPaging="true" Width="100%"
    OnItemDataBound="UserExclutionGrid_ItemDataBound" OnSelectedIndexChanged="UserExclutionGrid_SelectedIndexChanged" >
    <PagerStyle Mode="NextPrevAndNumeric" PageSizes="5" Height="100%" AlwaysVisible="true" />
    <ClientSettings>
        <Scrolling UseStaticHeaders="true" />
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <MasterTableView NoMasterRecordsText="No Data Found" DataKeyNames="Id,UserId" AutoGenerateColumns="false"  Width="100%">
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false"></CommandItemSettings>
        <Columns>
            <telerik:GridBoundColumn DataField="UserName" HeaderText="User Name" HeaderStyle-Width="15%" FilterControlAltText="UserName" SortExpression="UserName" UniqueName="UserName" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DisplayName" HeaderText="Display Name" HeaderStyle-Width="15%" FilterControlAltText="DisplayName" SortExpression="DisplayName" UniqueName="DisplayName" FilterControlWidth="65%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EventType" HeaderText="Event Type" HeaderStyle-Width="12%" FilterControlWidth="61%">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="EffectiveDate" HeaderText="Effective Till Date" HeaderStyle-Width="17%">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="Remarks" HeaderText="Remarks" HeaderStyle-Width="20%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="IsDeleted" HeaderText="Is Active" DataType="System.Boolean" HeaderStyle-Width="8%" UniqueName="IsActive">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="HasAttachments" UniqueName="HasAttachments" Display="false">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Attachment" HeaderStyle-Width="8%" UniqueName="Attachment">
                <ItemTemplate>
                    <asp:UpdatePanel runat="server" ID="UpdatePanelDownload" UpdateMode="Conditional" ChildrenAsTriggers="True" OnUnload="UpdatePanelDownload_Unload">
                        <ContentTemplate>
                            <asp:ImageButton ID="AttachmentImageButton" runat="server" CommandName="file" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="AttachmentImageButton" />
                        </Triggers>
                    </asp:UpdatePanel>
                </ItemTemplate>
                <ItemStyle CssClass="action-attachment" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>
          <%--  <telerik:GridButtonColumn ConfirmText="Are you sure to delete the User ?" ConfirmDialogType="RadWindow" HeaderText="Delete" HeaderStyle-HorizontalAlign="Center"
                ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                UniqueName="DeleteColumn" Exportable="false" HeaderStyle-Width="5%">
                <ItemStyle HorizontalAlign="Center" CssClass="action-delete" />
            </telerik:GridButtonColumn>--%>
             <telerik:GridTemplateColumn UniqueName="TemplateInfoColumn" HeaderStyle-Width="7%" FilterControlWidth="52%" AllowFiltering="false" HeaderText ="Detail / Info">
                <ItemTemplate>
                    <asp:ImageButton ID="ShowIcon" runat="server" CommandName="ShowInfo" src="../../Images/action-buttons/detail.png"></asp:ImageButton>
                </ItemTemplate>
                <ItemStyle CssClass="action-info" HorizontalAlign="Center" />
            </telerik:GridTemplateColumn>
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
                    <%--<uc:UserDetailUC runat="server" ID="UserDetailUC" />--%>
                    <uc:NUserDetailUC runat="server" ID="NUserDetailUC" />
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancel" class="btn btn-primary bordered-button" Text="OK" OnClick="btnCancel_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>