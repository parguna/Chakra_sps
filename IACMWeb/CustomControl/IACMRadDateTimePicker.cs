using Application;
using DI.Chakra.DBConfigHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.CustomControl
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:IACMRadDateTimePicker runat=server></{0}:IACMRadDateTimePicker>")]
    public class IACMRadDateTimePicker : RadDateTimePicker
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]

        #region Members

        private string V_TIMEFORMAT = "V_TIMEFORMAT";
        private string V_TIMEVIEWFORMAT = "V_TIMEVIEWFORMAT";
        private string V_TIMEINTERVAL = "V_TIMEINTERVAL";
        private string V_CALENDAR = "V_CALENDAR";
        private string V_DATEFORMAT = "V_DATEFORMAT";
        private string V_ISDATETODAY = "V_ISDATETODAY";
        SessionStatusImpl session = new SessionStatusImpl();

        #endregion

        #region Properties
        public RadDateTimePicker RadDateTimePickerCustom { get; set; }

        public IACMRadDateTimePicker() : base()
        {
            
        }

        public string DateTimeFormat
        {
            get
            {
                if (ViewState[V_TIMEFORMAT] == null)
                    return "";
                return (string)ViewState[V_TIMEFORMAT];
            }
            set
            {
                ViewState[V_TIMEFORMAT] = value;
            }
        }

        public Nullable<int> TimeInterval
        {
            get
            {
                if (ViewState[V_TIMEINTERVAL] == null)
                    return null;
                return (int)ViewState[V_TIMEINTERVAL];
            }
            set
            {
                ViewState[V_TIMEINTERVAL] = value;
            }
        }

        private RadTimeView _RadTimeView
        {
            get
            {
                if (ViewState[V_TIMEVIEWFORMAT] == null)
                    RadDateTimePickerCustom.TimeView.Interval = new TimeSpan(1, 00, 0);
                return (RadTimeView)ViewState[V_TIMEVIEWFORMAT];
            }
            set
            {
                ViewState[V_TIMEVIEWFORMAT] = value;
            }
        }

        private RadCalendar _RadCalendar
        {
            get
            {
                if (ViewState[V_CALENDAR] == null)
                    new RadCalendar();
                return (RadCalendar)ViewState[V_CALENDAR];
            }
            set
            {
                ViewState[V_TIMEVIEWFORMAT] = value;
            }
        }

        private string DateFormat
        {
            get
            {
                if (ViewState[V_DATEFORMAT] == null)
                    return "dd/MM/yyyy";
                return (string)ViewState[V_TIMEFORMAT];
            }
            set
            {
                ViewState[V_DATEFORMAT] = value;
            }
        }

        public bool IsDateToday
        {
            get
            {
                if (ViewState[V_ISDATETODAY] == null)
                    return true;
                return (bool)ViewState[V_ISDATETODAY];
            }
            set
            {
                ViewState[V_ISDATETODAY] = value;
                SetFutureTime();
            }
        }

        private void SetFutureTime()
        {
            if(IsDateToday)
                RadDateTimePickerCustom.TimeView.StartTime = DateTime.Now.TimeOfDay;
            else
                RadDateTimePickerCustom.TimeView.StartTime = new TimeSpan(0, 0, 0); 
        }

        #endregion

        #region Page Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RadDateTimePickerCustom = this;
            HttpContext.Current.Session["DateTimePickerCustom"] = RadDateTimePickerCustom.ID;
            RadDateTimePickerCustom.Calendar.FooterTemplate = new FooterTemplate();           
        }

        protected override void OnLoad(EventArgs e)
        {
            RadDateTimePickerCustom = this;
            RadDateTimePickerCustom.ShowPopupOnFocus = true;
            RadDateTimePickerCustom.DatePopupButton.Visible = true;
            RadDateTimePickerCustom.Enabled = true;
            RadDateTimePickerCustom.DateInput.ReadOnly = true;
            RadDateTimePickerCustom.Calendar.EnableKeyboardNavigation = true;
            RadDateTimePickerCustom.Calendar.ShowRowHeaders = false;
            

            IEnumerator CustomKeys = RadDateTimePickerCustom.Attributes.Keys.GetEnumerator();
            while (CustomKeys.MoveNext())
            {
                String _Key = (String)CustomKeys.Current;
                string _KeyValue = RadDateTimePickerCustom.Attributes[_Key];

                if ((_Key.ToLower() == "DateTimeFormat") && (!string.IsNullOrEmpty(_KeyValue)))
                    DateTimeFormat = _KeyValue;
                if ((_Key.ToLower() == "TimeInterval") && (!string.IsNullOrEmpty(_KeyValue)))
                {
                    int intValue;
                    if (int.TryParse(_KeyValue, out intValue))
                        TimeInterval = Convert.ToInt32(_KeyValue);
                }
            }
            if ((DateTimeFormat == null) || (DateTimeFormat == ""))
            {
                DateTimeFormat = null;
                TimeInterval = null;
                
                RadDateTimePickerCustom.TimePopupButton.Visible = true;                      
            }
            if (DateTimeFormat.ToLower() == "date")
            {
                DateTimeFormat = DateTimeFormat;
                RadDateTimePickerCustom.DateInput.DateFormat = session.DateFormat;
                RadDateTimePickerCustom.DateInput.DisplayDateFormat = session.DateFormat;
                RadDateTimePickerCustom.TimePopupButton.Visible = false;
            }
            else
            {
                if (TimeInterval == null)
                    TimeInterval = MasterConfigHelper.DateTimePickerInterval;
                if(IsDateToday)
                    RadDateTimePickerCustom.TimeView.StartTime = DateTime.Now.TimeOfDay;

                if (DateTimeFormat.ToLower() == "date hh:mm")
                {
                    if (session.DateFormat != null)
                    {
                        RadDateTimePickerCustom.DateInput.DateFormat = session.DateFormat + " HH:mm";
                        RadDateTimePickerCustom.DateInput.DisplayDateFormat = session.DateFormat + " HH:mm";
                    }
                    else
                    {
                        RadDateTimePickerCustom.DateInput.DateFormat = "dd/MM/yyyy HH:mm";
                        RadDateTimePickerCustom.DateInput.DisplayDateFormat = "dd/MM/yyyy HH:mm";
                    }
                    RadDateTimePickerCustom.TimeView.Interval = new TimeSpan(0, (int)TimeInterval, 0);
                }
                if (DateTimeFormat.ToLower() == "hh:mm")
                {
                    RadDateTimePickerCustom.TimeView.Interval = new TimeSpan(0, (int)TimeInterval, 0);
                    RadDateTimePickerCustom.DatePopupButton.Visible = false;
                    RadDateTimePickerCustom.TimePopupButton.Visible = true;
                    RadDateTimePickerCustom.DateInput.DateFormat = "hh:mm tt";
                    RadDateTimePickerCustom.TimeView.TimeFormat = "hh:mm tt";
                    RadDateTimePickerCustom.TimeView.Columns = 6;
                    RadDateTimePickerCustom.DateInput.DisplayDateFormat = "hh:mm tt";
                }
            }
        }

        #endregion
    }

    public class FooterTemplate : ITemplate
    {
        public void InstantiateIn(Control container)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl divToday = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            divToday.ID = "divToday";
            Button btnToday = new Button();
            btnToday.Text = "Today";
            btnToday.ID = "btnToday";
            btnToday.Width=240;
            btnToday.Attributes.Add("onclick", "SetToday();");
            btnToday.UseSubmitBehavior = true;
            divToday.Controls.Add(btnToday);
            container.Controls.Add(divToday);
        }
    }
}