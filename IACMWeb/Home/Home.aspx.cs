using Application;
using DI.Chakra.Dto;
using DI.Chakra.Web.Dashboard;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;

namespace DI.Chakra.Web
{
    public partial class Home_Home : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        #region Class MemberVariable
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        #endregion

        #region Properties
        //Store the info about the added docks in the session.
        private List<DockState> CurrentDockStates
        {
            get
            {
                List<DockState> _currentDockStates = (List<DockState>)ViewState["CurrentDockStates"];
                if (Object.Equals(_currentDockStates, null))
                {
                    _currentDockStates = new List<DockState>();
                    ViewState["CurrentDockStates"] = _currentDockStates;
                }
                return _currentDockStates;
            }
            set
            {
                ViewState["CurrentDockStates"] = value;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Init(object sender, EventArgs e)
        {
            //LoadDashBoards();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // To do Implement.
        }
        protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            //string[] args = e.Argument.Split('~');
            //string chartName = args[0].Replace(" ", string.Empty);
            //string RequestSource = args[1].Split(':')[0] != "Name" ? args[1].Split(':')[0] : args[1].Split(':')[1];
            //if (chartName.Equals("AccessRequestSourcesDashboard"))
            //{
            //    RadDockZone zone = RadDockLayout1.RegisteredZones[0];
            //    RadDock dock = GetDock(zone, chartName);
            //    if (dock != null)
            //    {
            //        IACMPieChart pieChart = (IACMPieChart)dock.ContentContainer.Controls[0];
            //        pieChart.RequestSource = (AccessRequestSource)Enum.Parse(typeof(AccessRequestSource), RequestSource);
            //        pieChart.BindData();
            //    }

            //}
            //else if (chartName.Equals("AccessRequestByMonth"))
            //{
            //    RadDockZone zone = RadDockLayout1.RegisteredZones[0];
            //    RadDock dock = GetDock(zone, chartName);
            //    if (dock != null)
            //    {
            //        IACMColumnChart pieChart = (IACMColumnChart)dock.ContentContainer.Controls[0];
            //        pieChart.RequestSource = (AccessRequestSource)Enum.Parse(typeof(AccessRequestSource), RequestSource);
            //        int Month = DateTime.ParseExact(args[1].Split(':')[1], "MMMM", CultureInfo.InvariantCulture).Month;
            //        pieChart.MonthNumber = Month;
            //        pieChart.BindData();
            //    }
            //}
        }
        #endregion

        #region Helpers
        private void LoadDashBoards()
        {
            if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
            {
                var roleList = sessionStatus.User.Roles;
                Role role = sessionStatus.User.Roles[0];
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", sessionStatus.User.Id.ToString());
                parameter.Add("roleId", role.Id.ToString());

                IList<Dto.Presentation.PresentationDashBoardPreference> dashBoards =
                    DomainApiClient<Dto.Presentation.PresentationDashBoardPreference>.
                    GetByAction<List<Dto.Presentation.PresentationDashBoardPreference>>
                    (DomainApiAction.Preference.PresentationDashBoardPreference.GetUserDashboards, parameter);
                                
                if (dashBoards.Count > 0)
                {
                    try
                    {
                        //re-arrange to chart-detail-chart
                        dashBoards = dashBoards.OrderByDescending(a => a.Position).Reverse().ToList();
                        
                        if (dashBoards[dashBoards.Count - 1].DashBoardType == DashboardType.List)
                        {
                            var listItem = dashBoards[dashBoards.Count - 1];
                            dashBoards.Remove(listItem);
                            dashBoards.Insert(1, listItem);                           
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                int layoutNumber = 1;
                short tabIndex = 1;
                DashboardType previousDashBoard = DashboardType.Chart;
                bool isPreviouslyDrawnWasChart = false;
                foreach (Dto.Presentation.PresentationDashBoardPreference dboard in dashBoards)
                {
                    RadDock dock = CreateRadDock(dboard.DashboardName, dboard.DashboardName, dboard.DashboardId.ToString(), layoutNumber, previousDashBoard, isPreviouslyDrawnWasChart);
                    previousDashBoard = dboard.DashBoardType;
                    dock.TabIndex = tabIndex++;
                    RadDockLayout1.Controls.Add(dock);
                    Control ctrl = this.Page.LoadControl(dboard.TargetUrl);
                    DashBoardHeader header = GetHeader(ctrl);
                    header.DashBoardTitle = dboard.DashboardCategory == "Attestation" ? "Attestation Task" : dboard.DashboardName;
                    //header.DashBoardCaption = dboard.DashboardCategory;
                    header.NavigateURL = dboard.NavigateUrl != null ? dboard.NavigateUrl : "#";

                    if (FindRadHtmlChart(ctrl) != null)                    
                        isPreviouslyDrawnWasChart = true;                    
                    else
                        isPreviouslyDrawnWasChart = false;


                    dock.ContentContainer.Controls.Add(ctrl);
                    dock.Dock(RadDockZoneDashboard);
                    layoutNumber++;
                    //RadHtmlChart chart = FindRadHtmlChart(ctrl);
                    //if (chart != null)
                    //{
                    //    chart.ChartTitle.Text = dboard.DashboardName;
                    //    // dock.Width = new Unit(chart.Width.Value + 40);
                    //    RadGrid grid = FindRadGrid(ctrl);
                    //}
                    //else
                    //{
                    //    //  dock.Style.Add("width", "51% !important");
                    //    //dock.Width = new Unit(850);
                    //}
                }
            }
        }
        private DashBoardHeader GetHeader(Control control)
        {
            DashBoardHeader header = null;
            foreach (Control childControl in control.Controls)
            {
                if (childControl is DashBoardHeader)
                    header = (DashBoardHeader)childControl;
                break;
            }
            return header;
        }
        private RadDock GetDock(RadDockZone zone, string chartName)
        {
            RadDock _dock = null;
            foreach (RadDock dock in zone.Docks)
            {
                if (dock.UniqueName.Equals(chartName))
                {
                    _dock = dock;
                    break;
                }
            }
            return _dock;
        }
        private RadDock CreateRadDock(string title, string Text, string Id, int layoutNumber, DashboardType previousDashboardType, bool isPreviouslyDrawnWasChart)
        {
            int docksCount = CurrentDockStates.Count;
            RadDock dock = new RadDock();
            dock.ID = string.Format("RadDock{0}", layoutNumber);
            dock.Title = title;
            dock.Text = Text;
            dock.UniqueName = string.Format("{0}{1}", Text.Replace(" ", string.Empty), layoutNumber);
            //dock.Width = Unit.Pixel(300);
            dock.Resizable = true;
            dock.Skin = "IACMSkin";
            dock.DockHandle = DockHandle.TitleBar;
            dock.EnableDrag = false;

            dock.Style.Add("width", "48% !important");

            //if (layoutNumber.Equals(1))
            //{
            //    dock.Style.Add("width", "48% !important");
            //}
            //else if (layoutNumber.Equals(2) || layoutNumber.Equals(3))
            //{
            //    dock.Style.Add("width", "48% !important");
            //    if(layoutNumber.Equals(3))
            //        dock.Style.Add("clear", "both");
            //}

            if (layoutNumber.Equals(3) && (previousDashboardType == DashboardType.Chart || isPreviouslyDrawnWasChart))
                dock.Style.Add("clear", "both");

            return dock;
        }


        private RadHtmlChart FindRadHtmlChart(Control control)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl.GetType() == typeof(RadHtmlChart))
                {
                    return (RadHtmlChart)childControl;
                }
                else
                {
                    FindRadHtmlChart(childControl);
                }
            }
            return null;
        }
        private RadGrid FindRadGrid(Control control)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl.GetType() == typeof(RadGrid))
                {
                    return (RadGrid)childControl;
                }
                else
                {
                    FindRadGrid(childControl);
                }
            }
            return null;
        }
        #endregion
    }
}