<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DesignationSearchUC.ascx.cs" Inherits="DI.Chakra.Web.UsrControl.Common.DesignationSearchUC" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

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
                <telerik:RadAutoCompleteBox ID="DesignationAutoCompleteBox" OnClientEntryAdding="OnClientEntryAddingHandler" EmptyMessage="Enter * to view full list" runat="server" CssClass="form-control"
                    InputType="Token" OnEntryAdded="DesignationAutoCompleteBox_EntryAdded">
                    <TextSettings SelectionMode="Single" />
                    <WebServiceSettings Method="GetNDesignations" Path="../../Services/AutoCompleteDataService.svc" />
                </telerik:RadAutoCompleteBox>
            </div>
<%--    </div>

    </div>

</div>
      </div>
    </div>
</div>--%>