<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BulkAccessUC.ascx.cs" Inherits="DI.Chakra.Web.NACR.Create.BulkRequest.BulkAccessUC" %>

<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocADStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocADStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapFlinsStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapFlinsStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapFlinsBIStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapFlinsBIStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapPiStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapPiStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapGrcStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapGrcStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapMsslStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapMsslStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocNpacStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocNpacStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocGemsStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocGemsStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapEbsStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapEbsStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocOffice365Step2UC_B.ascx" TagPrefix="uc1" TagName="AdhocOffice365Step2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapEpCeStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapEpCeStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocOneCardStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocOneCardStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocInHouseStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocInHouseStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSapSrmStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSapSrmStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSsidsStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSsidsStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocVPNStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocVPNStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocIdmsStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocIdmsStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSkypeForBusinessStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSkypeForBusinessStep2UC_B" %>
<%@ Register Src="~/NACR/Create/BulkRequest/Step2UC/AdhocSAPHANAStep2UC_B.ascx" TagPrefix="uc1" TagName="AdhocSAPHANAStep2UC_B" %>














<style type="text/css">
    .search {
        width: inherit;
        height: inherit;
        overflow: auto !important;
    }
</style>
<div class="form">
    <div class="form-horizontal left clearfix">
        <div class="col-xs-12 col-lg-12 col-md-12">
            <asp:Panel runat="server" ID="pnlStep2Controls">
                <uc1:AdhocADStep2UC_B runat="server" ID="AdhocADStep2UC_B" Visible="false" />
                <uc1:AdhocSapFlinsStep2UC_B runat="server" ID="AdhocSapFlinsStep2UC_B" Visible="false" />
                <uc1:AdhocSapFlinsBIStep2UC_B runat="server" ID="AdhocSapFlinsBIStep2UC_B" Visible="false" />
                <uc1:AdhocSapPiStep2UC_B runat="server" ID="AdhocSapPiStep2UC_B" Visible="false" />
                <uc1:AdhocSapGrcStep2UC_B runat="server" ID="AdhocSapGrcStep2UC_B" Visible="false" />
                <uc1:AdhocSapMsslStep2UC_B runat="server" ID="AdhocSapMsslStep2UC_B" Visible="false"  />
                <uc1:AdhocNpacStep2UC_B runat="server" ID="AdhocNpacStep2UC_B" Visible="false" />
                <uc1:AdhocGemsStep2UC_B runat="server" ID="AdhocGemsStep2UC_B" Visible="false" />
                <uc1:AdhocSapEbsStep2UC_B runat="server" ID="AdhocSapEbsStep2UC_B" Visible="false" />
                <uc1:AdhocOffice365Step2UC_B runat="server" ID="AdhocOffice365Step2UC_B" Visible="false" />
                <uc1:AdhocSapEpCeStep2UC_B runat="server" ID="AdhocSapEpCeStep2UC_B" Visible="false" />
                <uc1:AdhocOneCardStep2UC_B runat="server" ID="AdhocOneCardStep2UC_B" Visible="false" />
                <uc1:AdhocInHouseStep2UC_B runat="server" ID="AdhocInHouseStep2UC_B" Visible="false" />
                <uc1:AdhocSapSrmStep2UC_B runat="server" ID="AdhocSapSrmStep2UC_B" Visible="false" />
                <uc1:AdhocSsidsStep2UC_B runat="server" ID="AdhocSsidsStep2UC_B" Visible="false" />
                <uc1:AdhocVPNStep2UC_B runat="server" ID="AdhocVPNStep2UC_B" Visible="false" />
                <uc1:AdhocIdmsStep2UC_B runat="server" ID="AdhocIdmsStep2UC_B" Visible="false" />
                <uc1:AdhocSAPHANAStep2UC_B runat="server" ID="AdhocSAPHANAStep2UC_B" Visible="false" />
                <uc1:AdhocSkypeForBusinessStep2UC_B runat="server" ID="AdhocSkypeForBusinessStep2UC_B" Visible="false" />
            </asp:Panel>
        </div>
    </div>
</div>
