using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeederNetInspcetor.Model
{
    public class ResponseSessionModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region index
        private string reponseBody;
        public string ResponseBody
        {
            get { return reponseBody; }
            set
            {
                if (reponseBody != value)
                {
                    reponseBody = value;
                    NotifyPropertyChanged("ResponseBody");
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
