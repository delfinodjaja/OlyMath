using System;
using System.Web.UI;
using System.Web;
using Microsoft.Owin;

namespace OlyMath
{
    public partial class ErrorPage : Page
    {
        protected global::System.Web.UI.WebControls.Panel RequestIdPanel;
        protected global::System.Web.UI.WebControls.Literal RequestIdLiteral;
        protected void Page_Load(object sender, EventArgs e)
        {
            var requestId = Request.ServerVariables["HTTP_X_REQUEST_ID"] 
                        ?? Guid.NewGuid().ToString(); 

            if (!string.IsNullOrEmpty(requestId))
            {
                RequestIdPanel.Visible = true;
                RequestIdLiteral.Text = requestId;
            }
        }
    }
}
