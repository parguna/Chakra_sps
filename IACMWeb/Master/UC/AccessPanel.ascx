<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessPanel.ascx.cs" Inherits="DI.Chakra.Web.Master.UC.AccessPanel" %>

<div class="row-fluid" id="exportmessage">
    <div class="exportalert alert-success col-md-3 " id="message">
        <span class="alert-icon"></span>
        <h5>
            <label class="">Success</label>
        </h5>
        <div>Your file is generating. Please wait.</div>
    </div>
</div>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:UpdateProgress ID="UpdateProgressBar" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="10">
            <ProgressTemplate>
                <div id="divBackground" ondblclick="BackgroundOnDoubeClick()"
                    style="position: fixed; top: 0px; left: 0px; z-index: 100000001; height: 100%; width: 100%; background-color: black; opacity: 0.4; filter: alpha(opacity=40)">
                </div>
                <div id="divLoadImg" align="center" valign="middle" runat="server">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Common/loading.gif" Style="padding: 5px" border="0" Width="40" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <div class="floating-menu-right" id="AccessControls" runat="server">
            <asp:Repeater ID="rptPermissions" runat="server" OnItemCreated="rptPermissions_ItemCreated">
                <ItemTemplate>
                    <asp:Panel runat="server" ID="PermissionPanel" ToolTip='<%# DataBinder.Eval(Container.DataItem, "Permission.Name") %>'>
                        <div class="expand" id='tab<%# DataBinder.Eval(Container.DataItem, "Permission.Name") %>' <%#  Convert.ToString(DataBinder.Eval(Container.DataItem, "Permission.Name")) != "Export" ? "style='display:none'" : string.Empty  %>>
                            <a href="#" class="floating-icon  expand-link" id="Export">
                                <input type="image" name="export" src="/Images/action-buttons/export.png" title="Export">
                            </a>
                            <div class="expand-div" style="width:100px !important">
                                <a href="#" class="floating-icon" id="Excel1">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="lkActionE" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/export-icons/All.png" CommandName='ExcelAll' OnClick="lkAction_Click" ToolTip="Excel - All Columns" />

                                </a>
                               <%-- <a href="#" class="floating-icon" id="Excel">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="ImageButton1" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/export-icons/Selected.png" CommandName='Excel' OnClick="lkAction_Click" ToolTip="Excel - Selected Columns" />

                                </a>--%>
                                <span class="separator">|&nbsp;</span>
                                <a href="#" class="floating-icon" id="CSV1">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="lkActionC" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/export-icons/All.png" CommandName='CSVAll' OnClick="lkAction_Click" ToolTip="CSV - All Columns" />
                                </a>
                                <%--<a href="#" class="floating-icon" id="CSV">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="ImageButton2" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/export-icons/Selected.png" CommandName='CSV' OnClick="lkAction_Click" ToolTip="CSV - Selected Columns" />
                                </a>--%>

                            </div>
                        </div>
                        <a href="#" class="floating-icon" id='<%# DataBinder.Eval(Container.DataItem, "Permission.Name") %>' <%# (new List<string>() { "Export", "AttestationView", "AttestationReport", }).Contains(Convert.ToString(DataBinder.Eval(Container.DataItem, "Permission.Name"))) ? "style='display:none'" :  string.Empty  %>>
                            <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="lkAction" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                ImageUrl='<%# DataBinder.Eval(Container.DataItem, "Permission.IconURL") %>' CommandName='<%# DataBinder.Eval(Container.DataItem, "Permission.Name") %>'
                                OnClick="lkAction_Click" ToolTip='<%# DataBinder.Eval(Container.DataItem, "Permission.Name") %>' 
                                CssClass='<%# DataBinder.Eval(Container.DataItem, "Permission.Name") %>'
                                />
                        </a>

                       <%-- <div class="expand" id='tab<%# DataBinder.Eval(Container.DataItem, "Permission.Name") + "_View" %>' <%#  Convert.ToString(DataBinder.Eval(Container.DataItem, "Permission.Name")) != "AttestationView" ? "style='display:none'" : string.Empty  %>>
                            <a href="#" class="floating-icon  expand-link" id="AttestationView">
                                <input type="image" name="export" src="/Images/action-buttons/view.png" title="Attestation View">
                            </a>
                            <div class="expand-div-2-button">
                                <a href="#" class="floating-icon" id="userview">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="imgUserview" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/action-buttons/userview.png" CommandName='UserView' OnClick="lkAction_Click" ToolTip="User View" />
                                    <%# DataBinder.Eval(Container.DataItem, "Permission.Name") %>
                                </a>
                                <span class="separator" style="visibility:hidden">|&nbsp;</span>
                                <a href="#" class="floating-icon" id="accessview" style="visibility:hidden">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="imgAccessview" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/action-buttons/accessview.png" CommandName='AccessView' OnClick="lkAction_Click" ToolTip="Entitlement View" Visible="false"/>

                                </a>

                            </div>
                        </div>--%>

                        <div class="expand" id='tab<%# DataBinder.Eval(Container.DataItem, "Permission.Name") + "_Report" %>' <%#  Convert.ToString(DataBinder.Eval(Container.DataItem, "Permission.Name")) != "AttestationReport" ? "style='display:none'" : string.Empty  %>>
                            <a href="#" class="floating-icon  expand-link" id="AttestationReport">
                                <input type="image" name="export" src="/Images/action-buttons/export.png" title="Attestation Report">
                            </a>
                            <div class="expand-div-2-button">
                                <a href="#" class="floating-icon" id="FlatReport">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="imgFlatReport" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/export-icons/excelFlat.png" CommandName='ExportFlatReport' OnClick="lkAction_Click" ToolTip="Flat Report" />

                                </a>
                               <%-- <span class="separator">|&nbsp;</span>
                                <a href="#" class="floating-icon" id="HierarchyReport">
                                    <asp:ImageButton OnClientClick='<%# DataBinder.Eval(Container.DataItem, "Permission.ClientScript") %>' ID="imgHierarchyReport" runat="server" PostBackUrl='<%# DataBinder.Eval(Container.DataItem, "Url") %>'
                                        ImageUrl="~/Images/export-icons/excelHierarchy.png" CommandName='ExportHierarchyReport' OnClick="lkAction_Click" ToolTip="Hierarchy Report" />

                                </a>--%>

                            </div>
                        </div>
                    </asp:Panel>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    var baseUrl = document.location.origin;
    $(".exportalert").hide();
    $(".expand").on('mouseenter mouseleave', function () {
        $(this).find(".expand-div").stop().animate({ width: 'toggle' }, { "left": "0px" }, "fast");
    });

    $(".expand").on('mouseenter mouseleave', function () {
        $(this).find(".expand-div-2-button").stop().animate({ width: 'toggle' }, { "left": "0px" }, "fast");
    });

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(RunThisAfterEachAsyncPostback);
    function RunThisAfterEachAsyncPostback() {
        $(".expand").off("mouseenter mouseleave");
        $(".exportalert").hide();
        $(".expand").on('mouseenter mouseleave', function () {
            $(this).find(".expand-div").css('width', '170px').stop().animate({ width: "toggle" }, { "left": "0px" }, "fast");
        });

        $(".expand").on('mouseenter mouseleave', function () {
            $(this).find(".expand-div-2-button").stop().animate({ width: 'toggle' }, { "left": "0px" }, "fast");
        });

        function ShowProgress() {
            setTimeout(function () {
                $("#UpdateProgressBar").show();
            }, 200);

            var timer = setInterval(function () {
                $.ajax({
                    type: "POST",
                    url: baseUrl + "/Master/ExportStatusMoniter.aspx/GetExportStatus",
                    data: null,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnSuccess,
                    failure: function (response) {
                        alert(response.d);
                    }
                });

                function OnSuccess(response) {
                    if (response.d == "true") {
                        $("#UpdateProgressBar").hide();
                        clearInterval(timer);
                    }
                }
            }, "1000");
        }

    }

    function ShowProgress() {
        setTimeout(function () {
            $("#UpdateProgressBar").show();
        }, 200);


        var timer = setInterval(function () {
            console.log("expot start");
            $.ajax({
                type: "POST",
                url: baseUrl + "/Master/ExportStatusMoniter.aspx/GetExportStatus",
                data: null,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                    alert(response.d);
                }
            });

            function OnSuccess(response) {
                if (response.d == "true") {
                    $("#UpdateProgressBar").hide();
                    clearInterval(timer);
                }
            }
        }, "1000");
    }

</script>
