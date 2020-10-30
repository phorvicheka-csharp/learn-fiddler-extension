using FeederNetInspcetor.Model;
using System.Windows.Controls;

namespace FeederNetInspector.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Container : UserControl
    {
        private string hostName;

        public Container()
        {
            InitializeComponent();
            hostName = tbHostName.Text;
        }

        public string GetHostName()
        {
            return hostName;
        }

        public void SetTbRequests(RequestSessionModel requestSessionModel)
        {
            tbRequest.Text = requestSessionModel.RequestBody;
        }

        public void SetTbResponses(ResponseSessionModel responseSessionModel)
        {
            tbResponse.Text = responseSessionModel.ResponseBody;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Main.hostName = tbHostName.Text;
            if (tbHostName.Text == "")
            {
                Main.CaptureAll();
            } else
            {
                Main.CaptureWithHostName(tbHostName.Text);
            }
        }

        public void ToggleLabelLoading()
        {
            lblLoading.Visibility = lblLoading.IsVisible? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
        }
    }
}
