<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceMonitorUC.ascx.cs" Inherits="DI.Chakra.Web.Admin.AppSetting.UC.ServiceMonitorUC" %>
<%@ Register Src="~/Admin/AppSetting/UC/ServiceControlUC.ascx" TagPrefix="uc1" TagName="ServiceControlUC" %>





<div class="form-horizontal left clearfix">
 
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC1" ServiceDisplayName="Discovery1" ServiceName="DISCOVERY_SVCCONTROLLER" Product="IACM" />
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC7" ServiceDisplayName="Discovery2" ServiceName="DISCOVERY_SVCCONTROLLER2" Product="IACM" />
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC2" ServiceDisplayName="Attestation" ServiceName="ATTESTATION_SVCCONTROLLER" Product="IACM" />
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC3" ServiceDisplayName="Email" ServiceName="EMAIL_SVCCONTROLLER" Product="IACM" />
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC" ServiceDisplayName="Provisioning" ServiceName="PROVISIONING_SVCCONTROLLER" Product="IM" />
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC4" ServiceDisplayName="Mischellaneous" ServiceName="MISC_SVCCONTROLLER" Product="IACM" />
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC5" ServiceDisplayName="AdHoc" ServiceName="ADHOC_SVCCONTROLLER" Product="IACM" />
    <uc1:ServiceControlUC runat="server" ID="ServiceControlUC6" ServiceDisplayName="Reporting" ServiceName="REPORT_SVCCONTROLLER" Product="IACM" />
</div>


