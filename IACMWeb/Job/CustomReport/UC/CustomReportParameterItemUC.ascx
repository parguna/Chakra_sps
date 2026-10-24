<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomReportParameterItemUC.ascx.cs" Inherits="DI.Chakra.Web.Job.CustomReport.UC.CustomReportParameterItemUC" %>
<%@ Register Assembly="DI.Chakra.Web" Namespace="DI.Chakra.Web.CustomControl" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<tr class="rgRow">
    <td align="left" class="endpoint-system detail">
        <asp:Label ID="sequence" runat="server" Text='<%# Bind("Sequence") %>' />

    </td>
    <td>
        <asp:Label ID="Label1" runat="server" Text='<%# Bind("ParameterName") %>' />
    </td>
    <td>
        <asp:TextBox ID="txtdescription" runat="server" Text='<%# Bind("Description") %>' OnTextChanged="txtdescription_TextChanged" />
    </td>
    <td>
        <telerik:RadComboBox ID="parameterType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="parameterType_SelectedIndexChanged" Width="100%" Style="width:100%;" >
            <Items>
                <telerik:RadComboBoxItem runat="server" Text="varchar" Value="1" />
                <telerik:RadComboBoxItem runat="server" Text="DateTime" Value="2" />
                <telerik:RadComboBoxItem runat="server" Text="Long" Value="3" />
                <telerik:RadComboBoxItem runat="server" Text="Bit" Value="4" />
                 <telerik:RadComboBoxItem runat="server" Text="UserName" Value="5" />
                  <telerik:RadComboBoxItem runat="server" Text="EmployeeId" Value="6" />
                  <telerik:RadComboBoxItem runat="server" Text="EndpointSystem" Value="7" />
                 <%-- <telerik:RadComboBoxItem runat="server" Text="User" Value="5" />--%>
            </Items>
        </telerik:RadComboBox>
    </td>

    <td>
        <asp:TextBox ID="defaulttype" runat="server" Text='<%# Bind("DefaultValue") %>' OnTextChanged="defaultvalue_TextChanged" Style="width:100%;" />
        <telerik:RadNumericTextBox ID="longDataType" runat="server" OnTextChanged="longDataType_TextChanged" Visible="false" Width="100%" Style="width:100%;">
            <NumberFormat GroupSeparator="" DecimalDigits="0" />
        </telerik:RadNumericTextBox>
       <telerik:RadNumericTextBox ID="bitDataType" runat="server" OnTextChanged="bitDataType_TextChanged" Visible="false" Width="100%" Style="width:100%;" MinValue="0" MaxValue="1">
            <NumberFormat GroupSeparator="" DecimalDigits="0" />
        </telerik:RadNumericTextBox>
       <telerik:RadDatePicker runat="server" ID="dateDataType" DateTimePickerElement="" NullText="No Date" NullDate="DateTime.Now()" OnSelectedDateChanged="dateValue_SelectedDateChanged"
            EnableTyping="false" Skin="IACMSkin" ShowPopupOnFocus="false" CssClass="form-control" Enabled="true" DatePopupButton-Visible="true" AutoPostBack="true" Visible="false">
        </telerik:RadDatePicker>
        
        
    </td>

    <td>
        <asp:TextBox ID="dataformat" runat="server" Text='<%# Bind("DataFormatString") %>' OnTextChanged="dataformat_TextChanged" />
    </td>
</tr>
