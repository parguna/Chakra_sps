<%@ Page Title="" Language="C#" MasterPageFile="~/Master/ProtoMast4.master" AutoEventWireup="true" CodeBehind="CustomReportViewer.aspx.cs" Inherits="DI.Chakra.Web.Report.CustomReport.CustomReportViewer" %>

<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%--<%@ Register Src="~/UsrControl/Common/ReportScheduler.ascx" TagPrefix="ReportScheduler" TagName="RptSchd" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="cplMain" runat="server">
    <style>
        .txtUsername {
            width: 100% !important;
        }

        td {
            padding: 10px;
        }

         .operand {
             width: 100px !important;
        }

        .endpointSystem {
             width: 100% !important;
        }

        .RadPicker_IACMSkin {
            width: 100% !important;
            border: 1px solid #207bbd;
        }

        .imgShowFilter_Css {
            align-self: flex-end;
            text-align: right;
            float: right;
            padding-right: 2px;
            padding-bottom: 5px;
            margin-top: -5px;
            height: 25px;
            width: auto;
        }

        .demo-containers {
            text-align: right;
            padding-right: 20px;
        }

        #tblParameter {
            margin: 0 10px 0 10px;
        }

        .panel-title > a:before {
            float: right !important;
            font-family: FontAwesome;
            content: "\f002";
            padding-right: 5px;
        }

        .panel-title > a.collapsed:before {
            float: right !important;
            content: "\f002";
        }

        .panel-title > a:hover,
        .panel-title > a:active,
        .panel-title > a:focus {
            text-decoration: none;
        }

        .in {
            height: auto !important;
        }

        .ImpDateRangeFilter {
            width: 140px !important;
        }

        .rgFilterRow .RadPicker {
            width: 120px !important;
        }

        .RadAutoCompleteBox {
            cursor: default
        }

            .RadAutoCompleteBox input,
            .RadAutoCompleteBox input:focus {
                box-shadow: none;
                outline: 0;
                border: none !important;
            }

            .RadAutoCompleteBox .racTokenList {
                /*padding: 1px 20px 1px 1px;*/
                border-width: 1px;
                border-style: solid;
                overflow: hidden;
                position: relative;
            }

            .RadAutoCompleteBox .racToken {
                padding: 0 5px;
                border-width: 1px;
                border-style: solid;
                background-position: 0 0;
                background-repeat: repeat-x;
                white-space: nowrap;
                /*display: inline-block;*/
                vertical-align: middle
            }

                .RadAutoCompleteBox .racToken + .racToken {
                    margin-left: 1px
                }

            .RadAutoCompleteBox .racDisabled .racToken {
                filter: alpha(opacity=80);
                opacity: .8
            }

            .RadAutoCompleteBox .racRemoveTokenLink {
                margin: -4px -4px 0 2px;
                width: 16px;
                height: 16px;
                background-repeat: no-repeat;
                display: inline-block;
                vertical-align: middle;
                cursor: pointer
            }

            .RadAutoCompleteBox .racDisabled .racRemoveTokenLink {
                display: none
            }

            .RadAutoCompleteBox .racTextToken {
                padding: 0 0 0 4px;
                white-space: nowrap;
                display: inline-block;
                vertical-align: middle
            }

                .RadAutoCompleteBox .racTextToken + .racTextToken {
                    margin-left: 1px
                }

            .RadAutoCompleteBox .racInput {
                margin: 0;
                padding: 0;
                width: 100%;
                height: 18px;
                border: 0;
                color: inherit;
                background: 0;
                font: inherit;
                text-indent: 4px;
                display: inline-block;
                vertical-align: middle
            }

            * + html .RadAutoCompleteBox .racInput {
                font-size: 1em
            }

            .RadAutoCompleteBox .racEmptyMessage {
                font-style: italic
            }

            .RadAutoCompleteBox .racEditTokenInput {
                margin: 0;
                padding: 0;
                height: 16px;
                border: 0;
                background: 0;
                font: inherit
            }

            .RadAutoCompleteBox .racLoadingIcon {
                position: absolute;
                right: 2px;
                bottom: 2px;
                width: 16px;
                height: 16px
            }

            .RadAutoCompleteBox .racTokenHovered {
                background-position: 0 -1000px\9
            }

            .RadAutoCompleteBox .racTokenSelected {
                background-position: 0 -2000px\9
            }

            .RadAutoCompleteBox .racTokenDisabled {
                background-position: 0 -3000px\9
            }

        .racSlide {
            position: absolute;
            z-index: 10000001 !important;
        }

        .RadAutoCompleteBoxPopup {
            padding: 2px;
            width: 100%;
            box-sizing: border-box;
            border-width: 1px;
            border-style: solid;
            cursor: default;
            position: relative
        }

            .RadAutoCompleteBoxPopup .racList {
                margin: 0;
                padding: 0;
                height: 100%;
                list-style: none;
                overflow: auto;
                max-height: 100px;
                width: 100%;
                max-width: 350px;
            }

            .RadAutoCompleteBoxPopup .racHeader,
            .RadAutoCompleteBoxPopup .racFooter {
                *zoom: 1;
                padding: 2px 7px;
                border-style: solid
            }

                .RadAutoCompleteBoxPopup .racHeader:after,
                .RadAutoCompleteBoxPopup .racFooter:after {
                    content: "";
                    height: 0;
                    clear: both;
                    display: block
                }

            .RadAutoCompleteBoxPopup .racHeader {
                border-width: 0 0 1px
            }

            .RadAutoCompleteBoxPopup .racFooter {
                border-width: 1px 0 0
            }

            .RadAutoCompleteBoxPopup .racShowAll {
                margin: 2px -2px -2px;
                padding: 2px 8px;
                border-width: 1px 0 0;
                border-style: solid;
                text-align: center
            }

            .RadAutoCompleteBoxPopup .racItem {
                *zoom: 1;
                padding: 2px 6px
            }

                .RadAutoCompleteBoxPopup .racItem:after {
                    content: "";
                    height: 0;
                    clear: both;
                    display: block
                }

        .RadAutoCompleteBox.RadAutoCompleteBoxWithLabel .racLabel {
            display: inline-block;
            padding-right: 7px;
            vertical-align: middle
        }

        .RadAutoCompleteBox.RadAutoCompleteBoxWithLabel .racTokenList {
            display: inline-block;
            vertical-align: middle;
            *display: inline;
            *zoom: 1
        }
    </style>
    <script type="text/javascript">
        
        $(document).ready(function () {
            $(document).on("click", ".accFilter", function () {
                //debugger;
                if ($(".collapse").hasClass("in")) {
                    $(".collapse").removeClass("in");
                    $(this).addClass("collapsed");
                } else {
                    $(".collapse").addClass("in");
                    $(this).removeClass("collapsed");
                }
            });
        });
        function Submitclick() {
            
            try {
                jsonObj = [];
                item = {}
                var counter = 0;
                //debugger;
                $('#tblParameter > tbody > tr').each(function (i) {
                    var name = $(this).find('td:eq(0)').html();
                    if (!name.trim()) return true;
                    console.log(name);
                    item["Parameter"] = name;
                    var op = $("#tblParameter").find("#op_" + name);
                    if (op == null || op.length == 0) return true;
                    item["Operator"] = op[0].value;

                    var opval = $("#tblParameter").find("#" + name);
                    if (opval == null || opval.length == 0) return true;
                    item["Value"] = opval[0].value;

                    //var andOr = $("#tblParameter").find("#andOr_" + name);
                    //if (andOr == null || andOr.length == 0)
                    //{
                    //    item["AndOr"] = "";
                    //}
                    //else
                    //{
                    //    item["AndOr"] = andOr[0].value;
                    //}

                    jsonObj[counter] = { "Parameter": name, "Operator": op[0].value, "Value": opval[0].value };
                    counter = counter + 1;
                });
                var inputvalue = JSON.stringify(jsonObj);
                $("#getParameter").val(JSON.stringify(jsonObj));
            }
            catch (err) {
                var inputvalue = JSON.stringify(jsonObj);
                $("#getParameter").val(JSON.stringify(jsonObj));
            }
        }
         


    </script>

    <asp:Panel ID="Panel1" runat="server">
        <div>
            <div>
                <div class="from" id="divSearch" runat="server">
                    <div class="panel-group" id="accordion">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a id="hrefid" class="accFilter" href="javascript:void(0);" style="color: #666; font-size: 15px;">Parameter Values</a> </h4>
                            </div>
                            <div class="accordion-body" id="mappingContainer" style="padding: 1%;" data-parent="#accordion">
                                <div class="panel-body" style="padding: 0 0 15px 0 !important;">
                                    <input type="hidden" runat="server" id="getParameter" />
                                    <asp:Panel ID="panel" runat="server">
                                        <div style="padding: 1px; max-height: 130px; overflow-y: auto;">
                                            <table id="tblParameter" runat="server">
                                                <tr>
                                                    <td>
                                                        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <br />
                                        <div class="demo-containers">
                                            <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnSubmit" Text="Submit" OnClientClick="Submitclick();" UseSubmitBehavior="true" OnClick="btnSubmit_Click" />

                                            <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
                                        </div>
                                    </asp:Panel>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--<asp:ImageButton ID="imgShowFilter" runat="Server" ImageUrl="~/Images/tools-icons/custom-queries.png" OnClick="imageClick_Event" CssClass="imgShowFilter_Css" />--%>

            <%--<img runat="server" src="../../Images/action-buttons/submit.png" style="align-self:flex-end ; text-align:right; float: right; padding-right: 10px; padding-bottom: 10px; margin-top : -10px"  />
            --%>
            <telerik:RadGrid ID="RadReportJobs" OnItemCommand="RadReportJobs_ItemCommand" OnInit="RadReportJobs_Init" OnPreRender="RadReportJobs_PreRender" runat="server" OnNeedDataSource="RadReportJobs_NeedDataSource" EnableLinqExpressions="false" MasterTableView-TableLayout="Auto"
                OnDataBinding="RadReportJobs_DataBinding" AutoGenerateColumns="true" GroupPanelPosition="Top" AllowPaging="true" AllowCustomPaging="true" AllowFilteringByColumn="true" AllowSorting="true" FilterMenu-CssClass="RadMenu_IACMFilterSkin">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Center" />
                <PagerStyle Mode="NextPrevAndNumeric" PageSizes="10" Height="100%" AlwaysVisible="true" />
                <GroupingSettings CaseSensitive="false" />

                <ClientSettings>
                    <Scrolling UseStaticHeaders="false" AllowScroll="true" SaveScrollPosition="true" />
                </ClientSettings>
                <MasterTableView TableLayout="Auto" EnableNoRecordsTemplate="true" NoMasterRecordsText="No Records Available" ShowHeadersWhenNoRecords="true" Width="100%">
                    <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToPdfButton="False" ShowExportToCsvButton="false"></CommandItemSettings>

                </MasterTableView>
            </telerik:RadGrid>
        </div>
    </asp:Panel>

    <%--<telerik:RadWindow runat="server" ID="ParmeterDetails" Modal="true" Width="1000px" Height="200%" Title="Report Parameters"
        AutoSizeBehaviors="Width, Height" AutoSize="true" KeepInScreenBounds="true"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">

        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <input type="hidden" runat="server" id="getParameter" />
                    <asp:PlaceHolder ID="header" runat="server">
                        <div class="modal-header">
                            <h3 class="modal-title">Please enter parameter values</h3>
                        </div>
                    </asp:PlaceHolder>
                    <asp:Panel ID="panel" runat="server">
                        <table id="tblParameter" runat="server">
                            <tr>
                                <td>
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <div class="demo-containers">
                            <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnSubmit" Text="Submit" OnClientClick="Submitclick();" UseSubmitBehavior="true" OnClick="btnSubmit_Click" />

                            <asp:Button runat="server" class="btn btn-default btn-sm" ID="btnCancel" Text="Cancel" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>--%>
    <%--<telerik:RadWindow runat="server" Modal="true" ID="usrModal" Width="1050px" Height="100%" Title="Cart Items"
        RenderMode="Lightweight" Behaviors="Close" EnableViewState="true" VisibleTitlebar="false" VisibleStatusbar="false">
        <ContentTemplate>
            <div class="modal-header">
                <h3 class="modal-title" id="H1">Schedule Report</h3>
            </div>
            <div class="modal-body">
                <ReportScheduler:RptSchd ID="RptSchd" runat="server" />
            </div>
            <div class="modal-footer">
                <asp:Button runat="server" class="btn btn-primary transparent-button" ID="btnSchCancel" Text="Cancel" OnClick="btnSchCancel_Click"></asp:Button>
                <asp:Button runat="server" class="btn btn-primary bordered-button" ID="btnSave" Text="Save" OnClientClick="OnClientButtonClicking" OnClick="btnSave_Click" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>--%>
</asp:Content>
