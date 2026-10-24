<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrgSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.OrgSearchUC" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UsrControl/Common/NOrgDetail.ascx" TagPrefix="uc1" TagName="NOrgDetail" %>

<script>
    function OnClientEntryAddingHandler(sender, eventArgs) {
        if (sender.get_entries().get_count() > 0) {

            eventArgs.set_cancel(true);
        }

    }
</script>

<%--<div class="content-wrapper">

    <div class="form clearfix">
        <div class="form-horizontal left">
<div class="row-fluid">
    <div class="col-xs-12">
        <div class="form-group">--%>
            <div class="col-xs-7 form-group">
                <telerik:RadAutoCompleteBox ID="OrgAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                    InputType="Token" OnEntryAdded="OrgAutoCompleteBox_EntryAdded">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Method="GetNOrganisations" Path="../../Services/AutoCompleteDataService.svc" />
                </telerik:RadAutoCompleteBox>
            </div>
            <div class="col-xs-1">
                <div class="whats-this bottom showTooltip action-info">
                    <a class="di_tooltip" data-toggle="tooltip" href="#"><span class="icon info t-vertical-align"></span></a>
                    <span role="tooltip" class="tooltip left info" style="display: none">
                        <h5>Organisation Detail</h5>
                        <label class="col-xs-4">Organisation:</label>
                        <div class="col-xs-8">
                            <uc1:NOrgDetail runat="server" ID="NOrgDetail" />
                        </div>
                    </span>
                </div>
            </div>
<%--   </div>

    </div>

</div>
      </div>
    </div>
</div>--%>