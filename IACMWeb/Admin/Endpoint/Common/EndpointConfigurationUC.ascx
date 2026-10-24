<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EndpointConfigurationUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.Endpoint.Common.EndpointConfigurationUC" %>
<asp:UpdatePanel runat="server" UpdateMode="Always">
    <ContentTemplate>
        <div class="content-wrapper" runat="server" id="contentwrapper">
            <div class="form clearfix">
                <div class="form-horizontal left">
                    <div class="row-fluid">
                        <div id="FormLeftHorizontal" runat="server" class="col-xs-5">
                        </div>
                        <div id="FormRightHorizontal" runat="server" class="col-xs-7" visible="false">
                            <div class="form-group">
                                <asp:HyperLink ID="btnExportToExcel" NavigateUrl="~/Files/CSV/Endpoint/FILEHRMS_Employees.csv" CssClass="valign-bottom" runat="server" Target="_blank">Employee Sample Template</asp:HyperLink>
                            </div>
                            <div class="form-group">
                                <asp:HyperLink ID="HyperLink1" NavigateUrl="~/Files/CSV/Endpoint/FILEHRMS_Organisation.csv" CssClass="valign-bottom" runat="server" Target="_blank">Organization Sample Template</asp:HyperLink>
                            </div>
                            <div class="form-group">
                                <asp:HyperLink ID="HyperLink2" NavigateUrl="~/Files/CSV/Endpoint/FILEHRMS_BusinessRole.csv" CssClass="valign-bottom" runat="server" Target="_blank">BusinessRole Sample Template</asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>