using FeederNetInspcetor.Model;
using FeederNetInspector.UI;
using Fiddler;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

[assembly: RequiredVersion("5.0.0.0")]

namespace FeederNetInspector
{
    public class Main : IAutoTamper //IFiddlerExtension //IAutoTamper
    {
        public static Container container;
        public static string hostName;
        //public static string responseBodyText = "";
        //public static string requestBodyText = "";
        //public static Session[] oSessions;

        public void OnBeforeUnload()
        {
        }

        public void OnLoad()
        {
            container = new Container();
            hostName = container.GetHostName();

            TabPage page = new TabPage("FeederNetInspector");
            page.ImageIndex = (int)Fiddler.SessionIcons.Inspector;
            FiddlerApplication.UI.tabsViews.TabPages.Add(page);

            ElementHost element = new ElementHost();
            element.Child = container;
            element.Dock = DockStyle.Fill;

            page.Controls.Add(element);

            //Register an event handdler to CalculateReport event
            FiddlerApplication.CalculateReport += new CalculateReportHandler(DisplayResults);
        }

        public static void CaptureAll()
        {
            try
            {
                SelectAllSession();
            }
            catch (Exception e)
            {
                FiddlerApplication.Log.LogString(e.ToString());
                MessageBox.Show(e.Message, "Errors");
            }

        }

        private void DisplayResults(Session[] oSessions)
        {

            // If we're not showing the Stats tab right now, bail out.
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                return;
            }

            try
            {
                if (oSessions.Length < 1)
                {
                    return;
                }
                container.ToggleLabelLoading();
                Tuple<string, string> captureOutputTuple = getCaptureOutputTuple(oSessions);
                RequestSessionModel requestSessionModel = new RequestSessionModel();
                ResponseSessionModel responseSessionModel = new ResponseSessionModel();
                requestSessionModel.RequestBody = captureOutputTuple.Item1;
                responseSessionModel.ResponseBody = captureOutputTuple.Item2;
                container.SetTbRequests(requestSessionModel);
                container.SetTbResponses(responseSessionModel);
                container.ToggleLabelLoading();
            }
            catch (Exception e)
            {
                FiddlerApplication.Log.LogString(e.ToString());
                MessageBox.Show(e.Message, "Errors");
            }


            //Main.oSessions = oSessions;
            //FiddlerObject.uiInvoke(new MethodInvoker(UpdateUi));


            // must marshal and synchronize to UI thread
            //FiddlerApplication.UI.BeginInvoke(new Action<string>(text =>
            //{
            //container.SetTbRequests(requestSessionModel);
            //container.SetTbResponses(responseSessionModel);
            //}), captureOutputTuple.Item1);


        }

        /*private static void UpdateUi()
        {
            container.ToggleLabelLoading();
            Tuple<string, string> captureOutputTuple = getCaptureOutputTuple(oSessions);
            RequestSessionModel requestSessionModel = new RequestSessionModel();
            ResponseSessionModel responseSessionModel = new ResponseSessionModel();
            requestSessionModel.RequestBody = captureOutputTuple.Item1;
            responseSessionModel.ResponseBody = captureOutputTuple.Item2;
            container.SetTbRequests(requestSessionModel);
            container.SetTbResponses(responseSessionModel);
            container.ToggleLabelLoading();
        }*/

        private static Tuple<string, string> getCaptureOutputTuple(Session[] oSessions)
        {
            string requestOutput = "";
            string responseOutput = "";
            foreach (Session oSession in oSessions)
            {
                // Request
                requestOutput += "======= Session Object As String =======\n\n";
                requestOutput += oSession.ToString();
                requestOutput += "\n\n----- Host name -----\n\n";
                requestOutput += oSession.hostname;
                requestOutput += "\n\n----- Request Body As String -----\n\n";
                requestOutput += oSession.GetRequestBodyAsString();
                requestOutput += "\n\n\n=====================================\n\n\n";

                // Response
                responseOutput += "======= Session Object As String =======\n\n";
                responseOutput += oSession.ToString();
                responseOutput += "\n\n----- Host name -----\n\n";
                responseOutput += oSession.hostname;
                responseOutput += "\n\n----- Response Body As String -----\n\n";
                responseOutput += oSession.GetResponseBodyAsString();
                responseOutput += "\n\n========================================\n\n\n";
            }

            return new Tuple<string, string>(requestOutput, responseOutput);
        }

        public static void CaptureWithHostName(string hostName)
        {
            try
            {
                SelectAllSessionWithHostName(hostName);
            }
            catch (Exception e)
            {
                FiddlerApplication.Log.LogString(e.ToString());
                MessageBox.Show(e.Message, "Errors");
            }

        }

        public static void SelectAllSession()
        {
            ListView.ListViewItemCollection lvItems = FiddlerApplication.UI.lvSessions.Items;
            foreach (ListViewItem item in lvItems)
            {
                item.Selected = true;
                item.BackColor = Color.Yellow;
            }
        }

        public static void SelectAllSessionWithHostName(string hostName)
        {
            ListView.ListViewItemCollection lvItems = FiddlerApplication.UI.lvSessions.Items;
            foreach (ListViewItem item in lvItems)
            {
                // 3 -> index of column host name
                if (item.SubItems[3].Text.Contains(hostName))
                {
                    item.Selected = true;
                    item.BackColor = Color.Yellow;
                }
                else
                {
                    item.BackColor = FiddlerApplication.UI.lvSessions.BackColor;
                    item.Selected = false;
                }
            }
        }

        // Called before the user can edit a request using the Fiddler Inspectors
        public void AutoTamperRequestBefore(Session oSession)
        {
            /*if (oSession.hostname.Contains("feedernet"))
            {
                requestBodyText += "======= Session Object As String =======\n\n";
                requestBodyText += oSession.ToString();
                requestBodyText += "\n\n----- Host name -----\n\n";
                requestBodyText += oSession.hostname;
                requestBodyText += "\n\n----- Request Body As String -----\n\n";
                requestBodyText += oSession.GetRequestBodyAsString();
                requestBodyText += "\n\n\n=====================================\n\n\n";
            }*/
        }

        // Called after the user has had the chance to edit the request using the Fiddler Inspectors, but before the request is sent
        public void AutoTamperRequestAfter(Session oSession)
        {
            /*noop*/
         }

        // Called before the user can edit a response using the Fiddler Inspectors, unless streaming.
        public void AutoTamperResponseBefore(Session oSession)
        {
            // If we're not showing the Stats tab right now, bail out.
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                return;
            }
            // Filter items by hostName in Feedernet web seesion list
            if (hostName != "")
            {
                if (oSession.responseCode != 200 || !oSession.hostname.Contains("feedernet"))
                {
                    oSession["ui-hide"] = "true";
                }
            }
        }

        // Called after the user edited a response using the Fiddler Inspectors.  Not called when streaming.
        public void AutoTamperResponseAfter(Session oSession)
        {
            /*if (oSession.hostname.Contains("feedernet"))
            {
                responseBodyText += "======= Session Object As String =======\n\n";
                responseBodyText += oSession.ToString();
                responseBodyText += "\n\n----- Host name -----\n\n";
                responseBodyText += oSession.hostname;
                responseBodyText += "\n\n----- Response Body As String -----\n\n";
                responseBodyText += oSession.GetResponseBodyAsString();
                responseBodyText += "\n\n========================================\n\n\n";
            }*/
        }

        // Called Fiddler returns a self-generated HTTP error (for instance DNS lookup failed, etc)
        public void OnBeforeReturningError(Session oSession)
        {
            /*noop*/
        }
    }

}