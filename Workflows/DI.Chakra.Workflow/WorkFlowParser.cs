# region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using DeepIdentity.Facade;
using DeepIdentity.Infrastructure;
# endregion

namespace DeepIdentity.Workflow
{
    public class Wire
    {
        public int ModuleId { get; set; }
        public string Value { get; set; }

    }

    public class WorkFlowParser
    {
        # region VARIABLES

        string workflowContainerData = "";
        string workflowwiringsequence = "";
        string WorkflowWiringSequenceData = string.Empty;
        string[] arr;
        string[] wirearray;
        string Start = "Start";
        string StopWatch = "StopWatch";
        string Timer = "Timer";
        string Mail = "Mail";
        string Process = "Process";
        string Branch = "Branch";
        string Terminate = "Terminate";

        # endregion

        # region  CLASS INSTANCE

        MailActivity mailobj = null;
        ProcessActivity processobj = null;
        TimerActivity timerobj = null;
        WorkflowModule wfmoduleobj = null;
        StopWatchActivity stopwatchobj = null;
        Branch branchobj = null;
        WorkFlow workflow = null;
        WorkflowModule workflowmodule = null;
        WorkFlowFacade workflowfacade = null;
        Attestation attestation = null;

        #endregion


        public WorkflowModule GetNextModule(int moduleId, string branchName, Attestation _attestation)
        {
            try
            {
                workflow = new WorkFlow();
                workflowfacade = new WorkFlowFacade();
                workflow = workflowfacade.GetWorkflow(attestation.WorkflowConfigurationId);
                workflowwiringsequence = workflow.WorkflowWiringSequence;
                wirearray = workflowwiringsequence.Split(';');
                bool isDataFound = false;
                int indx = -1;
                string nextmod = string.Empty;
                for (int i = 0; i < wirearray.Length; i++)
                {
                    if (wirearray[i].StartsWith(moduleId.ToString() + '@' + branchName))
                    {
                        isDataFound = true;
                        indx = i + 1;
                        break;
                    }
                }
                if (isDataFound && wirearray.Length > indx)
                {
                    nextmod = wirearray[indx];

                }
                var moduleid = nextmod.Split('@');



                workflow = new WorkFlow();
                workflowfacade = new WorkFlowFacade();
                workflow = workflowfacade.GetWorkflow(_attestation.WorkflowConfigurationId);
                IList<WorkflowModule> listmodules = new List<WorkflowModule>();
                listmodules = GetWorkflowModules(workflow, _attestation);
                var r = (from l in listmodules
                         where Convert.ToInt32(l.ModuleId) == Convert.ToInt32(moduleid[0])
                         select listmodules.ElementAt(listmodules.IndexOf(l)));

                var modData = r.First();
                if (modData.ModuleName == r.First().ModuleName)
                {
                    branchobj = new Branch();
                    branchobj = GetBranchInfo(modData, _attestation);
                    modData.ModuleValue = branchobj;
                }
                wfmoduleobj = new WorkflowModule();
                wfmoduleobj.ModuleId = modData.ModuleId;
                wfmoduleobj.ModuleName = modData.ModuleName;
                wfmoduleobj.ModuleValue = modData.ModuleValue;


                return wfmoduleobj;


            }
            catch (Exception ex )
            {
                Log.Workflow.WriteMessage(LogLevel.FATAL, "Error occured while Getting GetNextModule", ex);
                throw ;
            }
           
        }

        public IList<WorkflowModule> GetWorkflowModules(WorkFlow _dto, Attestation _attestation)
        {


            try
            {
                IList<WorkflowModule> _moduleList = new List<WorkflowModule>();
                workflowContainerData = _dto.WorkflowContainerData;
                arr = workflowContainerData.Split(';', '$', '@');
                for (int j = 0; j <= arr.Length - 1; j++)
                {
                    wfmoduleobj = new WorkflowModule();
                    if (arr[j].Trim().ToString() == Start)
                    {
                        //wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        //wfmoduleobj.ModuleName = Start;
                        //_moduleList.Add(wfmoduleobj);

                        wfmoduleobj = new WorkflowModule();
                        processobj = new ProcessActivity();
                        processobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        processobj.ProcessName = arr[j + 1].Trim().ToString();
                        wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleName = arr[j].Trim().ToString();
                        wfmoduleobj.ModuleValue = processobj;
                        _moduleList.Add(wfmoduleobj);

                    }

                    else if (arr[j].Trim().ToString() == StopWatch)
                    {
                        wfmoduleobj = new WorkflowModule();
                        stopwatchobj = new StopWatchActivity();
                        stopwatchobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        stopwatchobj.StopTime = Convert.ToDouble(arr[j + 1]);
                        wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleName = arr[j].Trim().ToString();
                        wfmoduleobj.ModuleValue = stopwatchobj;
                        _moduleList.Add(wfmoduleobj);
                    }

                    else if (arr[j].Trim().ToString() == Timer)
                    {
                        wfmoduleobj = new WorkflowModule();
                        timerobj = new TimerActivity();
                        timerobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        timerobj.time = arr[j + 1].Trim().ToString();
                        wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleName = arr[j].Trim().ToString();
                        wfmoduleobj.ModuleValue = timerobj;
                        _moduleList.Add(wfmoduleobj);
                    }

                    else if (arr[j].Trim().ToString() == Mail)
                    {
                        wfmoduleobj = new WorkflowModule();
                        mailobj = new MailActivity();
                        mailobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        mailobj.ActivityName = arr[j].Trim().ToString();
                        mailobj.MailTemplateName = arr[j + 1].Trim().ToString();
                        wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleName = arr[j].Trim().ToString();
                        wfmoduleobj.ModuleValue = mailobj;
                        _moduleList.Add(wfmoduleobj);

                    }

                    else if (arr[j].Trim().ToString() == Process)
                    {
                        wfmoduleobj = new WorkflowModule();
                        processobj = new ProcessActivity();
                        processobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        processobj.ProcessName = arr[j + 1].Trim().ToString();
                        wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleName = arr[j].Trim().ToString();
                        wfmoduleobj.ModuleValue = processobj;
                        _moduleList.Add(wfmoduleobj);

                    }

                    else if (arr[j].Trim().ToString() == Branch)
                    {
                        branchobj = new Branch();
                        wfmoduleobj = new WorkflowModule();
                        branchobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleName = arr[j].Trim().ToString();
                        branchobj = ParseBranch(wfmoduleobj, _attestation);
                        wfmoduleobj.ModuleValue = branchobj;
                        _moduleList.Add(wfmoduleobj);
                    }

                    else if (arr[j].Trim().ToString() == Terminate)
                    {
                        wfmoduleobj = new WorkflowModule();
                        processobj = new ProcessActivity();
                        processobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        processobj.ProcessName = arr[j + 1].Trim().ToString();
                        wfmoduleobj.ModuleId = Convert.ToInt32(arr[j - 1].Trim());
                        wfmoduleobj.ModuleName = arr[j].Trim().ToString();
                        wfmoduleobj.ModuleValue = processobj;
                        _moduleList.Add(wfmoduleobj);

                    }
                }
                return _moduleList;

            }

             catch (Exception ex)
             {

                 Log.Workflow.WriteMessage(LogLevel.FATAL, "Error occured while Getting Workflow Modules List", ex);
                 throw ex;
             }

            finally
            {

            }

        }

        private Branch GetBranchInfo(WorkflowModule module, Attestation _attestation)
        {
            try
            {
                workflow = new WorkFlow();
                workflowfacade = new WorkFlowFacade();
                workflow = workflowfacade.GetWorkflow(_attestation.WorkflowConfigurationId);
                WorkflowWiringSequenceData = workflow.WorkflowWiringSequence;

                string[] seqdata = WorkflowWiringSequenceData.Split(';');
                string id = module.ModuleId.ToString();
                string B1 = id + "@" + "B1";
                string B2 = id + "@" + "B2";
                string B3 = id + "@" + "B3";
                string B4 = id + "@" + "B4";

                IList<string> liB1 = seqdata.Where(l => l.StartsWith(B1)).ToList();
                IList<string> liB2 = seqdata.Where(l => l.StartsWith(B2)).ToList();
                IList<string> liB3 = seqdata.Where(l => l.StartsWith(B3)).ToList();
                IList<string> liB4 = seqdata.Where(l => l.StartsWith(B4)).ToList();

                branchobj = new Branch();
                branchobj.ModuleId = Convert.ToInt32(id);
                if (liB1.Count > 0)
                {
                    string valueB1 = liB1[0].ToString();
                    string[] arrB1 = valueB1.Split('$');
                    branchobj.B1 = arrB1[1];

                }

                if (liB2.Count > 0)
                {
                    string valueB2 = liB2[0].ToString();
                    string[] arrB2 = valueB2.Split('$');
                    branchobj.B2 = arrB2[1];

                }

                if (liB3.Count > 0)
                {
                    string valueB3 = liB3[0].ToString();
                    string[] arrB3 = valueB3.Split('$');
                    branchobj.B3 = arrB3[1];
                }

                if (liB4.Count > 0)
                {
                    string valueB4 = liB4[0].ToString();
                    string[] arrB4 = valueB4.Split('$');
                    branchobj.B4 = arrB4[1];
                }

                return branchobj;

            }
            catch (Exception ex)
            {
                Log.Workflow.WriteMessage(LogLevel.FATAL, "Error occure while Getting Branchinfo", ex);
                throw;
            }
            
        }

        public IList<WorkflowModule> GetBranchNextModulesList(int moduleId, string branchName, Attestation _attestation)
        {

            try
            {
                workflow = new WorkFlow();
                workflowfacade = new WorkFlowFacade();
                workflow = workflowfacade.GetWorkflow(_attestation.WorkflowConfigurationId);
                workflowwiringsequence = workflow.WorkflowWiringSequence;
                wirearray = workflowwiringsequence.Split(';');
                bool isDataFound = false;
                int startindex = -1;
                IList<WorkflowModule> listnextmodules = new List<WorkflowModule>();
                for (int i = 0; i < wirearray.Length; i++)
                {

                    if (wirearray[i].StartsWith(moduleId.ToString() + '@' + branchName))
                    {

                        startindex = i;
                        isDataFound = true;
                        break;
                    }
                }
                int lastindex = -1;
                if (isDataFound && wirearray.Length > startindex)
                {
                    for (int j = startindex + 1; j < wirearray.Length; j++)
                    {
                        string[] modarray;
                        modarray = wirearray[j].Split('@');
                        if (modarray.Length > 1)
                        {
                            if (modarray[1].StartsWith("B"))
                            {
                                lastindex = j;
                                break;
                            }


                            else
                                lastindex = j;
                        }

                    }

                }
                List<string> excmod = new List<string>();
                for (int i = startindex + 1; i < lastindex; i++)
                {
                    excmod.Add(wirearray[i]);
                }

                if (excmod.Count == 0)
                {
                    excmod.Add(wirearray[lastindex]);
                }

                IList<int> mods = new List<int>();

                foreach (string str in excmod)
                {
                    int i = Convert.ToInt32(str.Split(new char[] { '@' }).First());

                    if (!mods.Contains(i))
                        mods.Add(i);


                }

                workflow = new WorkFlow();
                workflowfacade = new WorkFlowFacade();
                workflow = workflowfacade.GetWorkflow(_attestation.WorkflowConfigurationId);
                IList<WorkflowModule> listmodules = new List<WorkflowModule>();
                listmodules = GetWorkflowModules(workflow, _attestation);


                IList<WorkflowModule> NextList = new List<WorkflowModule>();

                for (int i = 0; i < mods.Count; i++)
                {
                    List<WorkflowModule> r = (from l in listmodules
                                              where Convert.ToInt32(l.ModuleId) == Convert.ToInt32(mods[i])
                                              select listmodules.ElementAt(listmodules.IndexOf(l))).ToList();

                    if (r.Count > 0)
                    {

                        NextList.Add(r.First());
                    }


                }

                return NextList;


            }
            catch (Exception ex)
            {
                Log.Workflow.WriteMessage(LogLevel.FATAL, "Error occure while Getting GetBranchNextModulesList", ex);
                throw;
            }


           
        }

        private Branch ParseBranch(WorkflowModule module, Attestation _attestation)
        {
            try
            {
                workflow = new WorkFlow();
                workflowfacade = new WorkFlowFacade();
                workflow = workflowfacade.GetWorkflow(_attestation.WorkflowConfigurationId);
                WorkflowWiringSequenceData = workflow.WorkflowWiringSequence;

                string[] seqdata = WorkflowWiringSequenceData.Split(';');
                string id = module.ModuleId.ToString();
                string B1 = id + "@" + "B1";
                string B2 = id + "@" + "B2";
                string B3 = id + "@" + "B3";
                string B4 = id + "@" + "B4";

                IList<string> liB1 = seqdata.Where(l => l.StartsWith(B1)).ToList();
                IList<string> liB2 = seqdata.Where(l => l.StartsWith(B2)).ToList();
                IList<string> liB3 = seqdata.Where(l => l.StartsWith(B3)).ToList();
                IList<string> liB4 = seqdata.Where(l => l.StartsWith(B4)).ToList();

                branchobj = new Branch();
                branchobj.ModuleId = Convert.ToInt32(id);
                if (liB1.Count > 0)
                {
                    string valueB1 = liB1[0].ToString();
                    string[] arrB1 = valueB1.Split('$');
                    branchobj.B1 = arrB1[1];

                }

                if (liB2.Count > 0)
                {
                    string valueB2 = liB2[0].ToString();
                    string[] arrB2 = valueB2.Split('$');
                    branchobj.B2 = arrB2[1];

                }

                if (liB3.Count > 0)
                {
                    string valueB3 = liB3[0].ToString();
                    string[] arrB3 = valueB3.Split('$');
                    branchobj.B3 = arrB3[1];
                }

                if (liB4.Count > 0)
                {
                    string valueB4 = liB4[0].ToString();
                    string[] arrB4 = valueB4.Split('$');
                    branchobj.B4 = arrB4[1];
                }

                return branchobj;

            }
            catch (Exception ex)
            {
                Log.Workflow.WriteMessage(LogLevel.FATAL, "Error occure while Getting  Data from  ParseBranch ", ex);
                throw;
            }

         
        }

        public IList<int> GetCompletedAndNextModuleId(int moduleId, string terminalname, Attestation _attestation)
        {
            try
            {
                workflow = new WorkFlow();
                workflowfacade = new WorkFlowFacade();
                workflow = workflowfacade.GetWorkflow(_attestation.WorkflowConfigurationId);
                workflowwiringsequence = workflow.WorkflowWiringSequence;
                wirearray = workflowwiringsequence.Split(';');
                bool isDataFound = false;
                int indx = -1;
                for (int i = 0; i < wirearray.Length; i++)
                {
                    if (wirearray[i].StartsWith(moduleId.ToString() + '@' + terminalname))
                    {
                        isDataFound = true;
                        indx = i;
                        break;
                    }
                }
                IList<int> prevnextmods = null;
                if (isDataFound && wirearray.Length > indx)
                {

                    List<string> prevnext = new List<string>();
                    prevnext.Add(wirearray[indx - 2]);
                    prevnext.Add(wirearray[indx + 1]);
                    prevnextmods = new List<int>();
                    foreach (string str in prevnext)
                    {
                        int i = Convert.ToInt32(str.Split(new char[] { '@' }).First());
                        prevnextmods.Add(i);

                    }

                }
                return prevnextmods;

            }
            catch (Exception ex)
            {
                Log.Workflow.WriteMessage(LogLevel.FATAL, "Error occure while Getting  Data from  GetCompletedAndNextModuleId ", ex); 
                throw;
            }


        }


     

    }
}
