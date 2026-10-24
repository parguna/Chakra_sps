 
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.ProvisioningProcessor;
using DI.Chakra.Services.MiscProcessor;
using System;
using System.Windows.Forms;

namespace TestHarness
{
    public partial class TestForm : Form
    {
        private long userid = 5250;

        public TestForm()
        {
            InitializeComponent();
        }

       
       

        private void button1_Click(object sender, EventArgs e)
        {

            
            User user = User.GetById(userid);

            EmployeeEventTrigger trigger = EmployeeEventTrigger.GetByUserAndEvent(userid, EmployeeEvents.NewJoinee,DateTime.Now.AddDays(-30));

            if (trigger == null)
            {
                trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = EmployeeEvents.NewJoinee,
                    EffectiveDate = DateTime.Now,
                    IsRequestCreated = false,
                    UserId = userid,
                    IsActive = true,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                };

            }
            trigger.IsRequestCreated = false;
            trigger.Save(MasterConfigHelper.ServiceUser);
            MessageBox.Show("Done");

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            User user = User.GetById(userid);

            EmployeeEventTrigger trigger = EmployeeEventTrigger.GetByUserAndEvent(userid, EmployeeEvents.Resignation, DateTime.Now.AddDays(-30));
            if (trigger == null)
            {
                trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = EmployeeEvents.Resignation,
                    EffectiveDate = DateTime.Now,
                    IsRequestCreated = false,
                    UserId = userid,
                    IsActive = true,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                };

            }
            trigger.IsRequestCreated = false;
            trigger.Save(MasterConfigHelper.ServiceUser);
            MessageBox.Show("Done");


         
        }

        private void button4_Click(object sender, EventArgs e)
        {
            User user = User.GetByIdIncludeNBusinessRoleAndNOrganisation(userid);

            txtCurrentOrg.Text = user.NOrganisationId.ToString();




        }

        private void button5_Click(object sender, EventArgs e)
        {

            User user = User.GetByIdIncludeNBusinessRoleAndNOrganisation(userid);

            EmployeeEventTrigger trigger = EmployeeEventTrigger.GetByUserAndEvent(userid, EmployeeEvents.OUChange, DateTime.Now.AddDays(-30));

            if (trigger == null)
            {
                trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = EmployeeEvents.OUChange,
                    EffectiveDate = DateTime.Now,
                    IsRequestCreated = false,
                    UserId = userid,
                    IsActive = true,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                };

            }

            trigger.OldUserDetails = UtilityMethods.Serialize<User>(user);


            user.NOrganisationId = Convert.ToInt32(txtNewOrg.Text);
            NOrganisation newOrg = NOrganisation.GetById(Convert.ToInt32(txtNewOrg.Text));
            user.NOrganisation = newOrg;
            trigger.UserDetails = UtilityMethods.Serialize<User>(user);


            user.NOrganisationId = Convert.ToInt32(txtNewOrg.Text);
            user.Save(MasterConfigHelper.ServiceUser);

            trigger.IsRequestCreated = false;
            trigger.Save(MasterConfigHelper.ServiceUser);
            MessageBox.Show("Done");
        }

        private void button6_Click(object sender, EventArgs e)
        {
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ProvisioningEngine proveng = new ProvisioningEngine();
            proveng.StartTrustedSourcePropagation();
            MessageBox.Show("Done.");
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            ProvisioningEngine peManager = new ProvisioningEngine();
            peManager.StartProvisioning();

            MessageBox.Show("Done.");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            EmployeeEventTrigger trigger = EmployeeEventTrigger.GetByUserAndEvent(userid, EmployeeEvents.NewJoinee, DateTime.Now.AddDays(-30));
            trigger.IsRequestCreated = true;
            trigger.Save(MasterConfigHelper.ServiceUser);
            trigger = EmployeeEventTrigger.GetByUserAndEvent(userid, EmployeeEvents.Resignation, DateTime.Now.AddDays(-30));
            trigger.IsRequestCreated = true;
            trigger.Save(MasterConfigHelper.ServiceUser);
            trigger = EmployeeEventTrigger.GetByUserAndEvent(userid, EmployeeEvents.OUChange, DateTime.Now.AddDays(-30));
            trigger.IsRequestCreated = true;
            trigger.Save(MasterConfigHelper.ServiceUser);
            MessageBox.Show("Done");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //long req = Convert.ToInt64(txtReq.Text);

            //AccessRequest acr = AccessRequest.GetByIdIncludeAdHocAccessRequest(req);
            //EventProcessor eventProcessor = new EventProcessor();
            //eventProcessor.Process(acr);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            User user = User.GetById(userid);

            EmployeeEventTrigger trigger = EmployeeEventTrigger.GetByUserAndEvent(5266, EmployeeEvents.Rehire, DateTime.Now.AddDays(-30));

            if (trigger == null)
            {
                trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = EmployeeEvents.Rehire,
                    EffectiveDate = DateTime.Now,
                    IsRequestCreated = false,
                    UserId = userid,
                    IsActive = true,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                };

            }
            trigger.IsRequestCreated = false;
            trigger.Save(MasterConfigHelper.ServiceUser);
            MessageBox.Show("Done");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            PresentationSystemDecommission.StartDecommisson();
            MessageBox.Show("Done.");

        }
    }
}
