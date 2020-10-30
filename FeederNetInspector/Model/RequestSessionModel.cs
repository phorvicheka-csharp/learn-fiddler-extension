using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FeederNetInspcetor.Model
{
    public class RequestSessionModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region index
        private string requestBody;
        public string RequestBody
        {
            get { return requestBody; }
            set
            {
                if (requestBody != value)
                {
                    requestBody = value;
                    NotifyPropertyChanged("RequestBody");
                }
            }
        }
        #endregion

        #region UI
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
