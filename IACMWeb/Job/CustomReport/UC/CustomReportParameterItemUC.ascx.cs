using Application;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;


namespace DI.Chakra.Web.Job.CustomReport.UC
{
    public partial class CustomReportParameterItemUC : UIBaseUC
    {
        private const string V_PARAMETER = "V_PARAMETER";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                 
                parameterType.SelectedValue = Convert.ToString( (int) CustomReportParameterItem.ParameterType);
            }
        }

        protected void OnLoadHandler(object sender, EventArgs e)
        {
            //RadComboBox combo = (RadComboBox)sender;
            //combo.Items.Add(new RadComboBoxItem("", ""));
            //foreach (int index in Enum.GetValues(typeof(CustomReportParameterTypes)))
            //{
            //    RadComboBoxItem item = new RadComboBoxItem();
            //    item.Value = Enum.GetName(typeof(CustomReportParameterTypes), index);
            //    item.Text = Enum.GetName(typeof(CustomReportParameterTypes), index);
            //    combo.Items.Add(item);
            //}           
        }

        public Dto.CustomReportParameter CustomReportParameterItem
        {
            get
            {
                if (ViewState[V_PARAMETER] == null)
                    ViewState[V_PARAMETER] = new Dto.CustomReportParameter();

                return (Dto.CustomReportParameter)ViewState[V_PARAMETER];
            }
            set
            {
                ViewState[V_PARAMETER] = value;
            }
        }

        public void Initialize()
        {

        }

        protected void parameterType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            CustomReportParameterItem.ParameterType = (CustomReportParameterTypes)Enum.Parse(typeof(CustomReportParameterTypes), e.Text);

            if(CustomReportParameterItem.ParameterType == CustomReportParameterTypes.Bit)
            {
                bitDataType.Visible = true;
                longDataType.Visible = false;
                dateDataType.Visible = false;
                defaulttype.Visible = false;
            }
            else if (CustomReportParameterItem.ParameterType == CustomReportParameterTypes.DateTime)
            {
                bitDataType.Visible = false;
                longDataType.Visible = false;
                dateDataType.Visible = true;
                defaulttype.Visible = false;
            }
            else if (CustomReportParameterItem.ParameterType == CustomReportParameterTypes.Long)
            {
                bitDataType.Visible = false;
                longDataType.Visible = true;
                dateDataType.Visible = false;
                defaulttype.Visible = false;
            }
            else if (CustomReportParameterItem.ParameterType == CustomReportParameterTypes.varchar || CustomReportParameterItem.ParameterType == CustomReportParameterTypes.EmployeeId || CustomReportParameterItem.ParameterType == CustomReportParameterTypes.EndpointSystem)
            {
                bitDataType.Visible = false;
                longDataType.Visible = false;
                dateDataType.Visible = false;
                defaulttype.Visible = true;
            }
            else
            {
                bitDataType.Visible = false;
                longDataType.Visible = false;
                dateDataType.Visible = false;
                defaulttype.Visible = true;
            }
        }
 
        protected void txtdescription_TextChanged(object sender, EventArgs e)
        {
            CustomReportParameterItem.Description = ((TextBox)sender).Text;
        }

        protected void defaultvalue_TextChanged(object sender, EventArgs e)
        {
            CustomReportParameterItem.DefaultValue= ((TextBox)sender).Text;
        }

        protected void dataformat_TextChanged(object sender, EventArgs e)
        {
            CustomReportParameterItem.DataFormatString = ((TextBox)sender).Text;
        }

        protected void longDataType_TextChanged(object sender, EventArgs e)
        {
            var enteredText = ((RadNumericTextBox)sender).Text;
            if(enteredText.Contains("."))
                longDataType.NumberFormat.DecimalDigits = 1;
            CustomReportParameterItem.DefaultValue = enteredText;
        }        

        protected void dateValue_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
        {
            CustomReportParameterItem.DefaultValue = ((RadDatePicker)sender).SelectedDate.Value.ToString("dd-MM-yyyy");
        }

        protected void bitDataType_TextChanged(object sender, EventArgs e)
        {
            var enteredText = ((RadNumericTextBox)sender).Text;
            if (enteredText.Contains("."))
                longDataType.NumberFormat.DecimalDigits = 1;
            CustomReportParameterItem.DefaultValue = enteredText;
        }
    }
}