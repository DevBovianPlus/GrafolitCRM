﻿using AnalizaProdaje.Domain.Concrete;
using DatabaseWebService.Models;
using DevExpress.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace AnalizaProdaje.Common
{
    public class ServerMasterPage : System.Web.UI.Page
    {
        protected virtual DataTable CreateDataSource()
        {
            return new DataTable();
        }

        #region Session handeling

        protected void AddValueToSession(object sesionName, object value)
        {
            Session[sesionName.ToString()] = value;
        }

        protected object GetValueFromSession(object sessionName)
        {
            return Session[sessionName.ToString()];
        }

        protected string GetStringValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return "";

            return Session[sessionName.ToString()].ToString();
        }

        protected int GetIntValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return -1;

            return CommonMethods.ParseInt(GetStringValueFromSession(sessionName));
        }

        protected decimal GetDecimalValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return -1;

            return CommonMethods.ParseDecimal(GetStringValueFromSession(sessionName));
        }

        protected double GetDoubleValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return -1.0;

            return CommonMethods.ParseDouble(GetStringValueFromSession(sessionName));
        }

        protected bool GetBoolValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return false;

            return CommonMethods.ParseBool(Session[sessionName.ToString()].ToString());
        }

        protected bool SessionHasValue(object sessionName)
        {
            if (Session[sessionName.ToString()] != null)
                return true;

            return false;
        }

        protected void RemoveAllSesions()
        {
            Session.RemoveAll();
        }

        protected void RemoveSession(object sessionName)
        {
            Session.Remove(sessionName.ToString());
        }

        protected void ClearAllSessions<T>(List<T> sessionList)
        {
            foreach (var item in sessionList)
            {
                RemoveSession(item.ToString());
            }
        }

        protected void ClearAllSessions<T>(List<T> sessionList, string redirectPageUrl, bool isCallback = false)
        {
            foreach (var item in sessionList)
            {
                RemoveSession(item.ToString());
            }

            if(isCallback)
                ASPxWebControl.RedirectOnCallback(redirectPageUrl);
            else
                Response.Redirect(redirectPageUrl);
        }
        #endregion

        #region Generating URI and redirection
        protected void RedirectWithCustomURI(string pageName, int userAction, object recordID)
        {
            Response.Redirect(GenerateURI(pageName, userAction, recordID));
        }

        protected void RedirectWithCustomURI(string pageName, List<QueryStrings> queryList)
        {
            Response.Redirect(GenerateURI(pageName, queryList));
        }

        /// <summary>
        /// Method using for generating uri based on user action(add, edit, delete) and which entity record we want to manipulate.
        /// </summary>
        /// <param name="pageName">Page name.</param>
        /// <param name="userAction">Enums user action (add, edit, delete).</param>
        /// <param name="recordID">Entity record we want to manipulate.</param>
        /// <returns>Returns url for redirection.</returns>
        protected string GenerateURI(string pageName, int userAction, object recordID)
        {
            return pageName + "?" + Enums.QueryStringName.action.ToString() + "=" + userAction.ToString() + "&" + Enums.QueryStringName.recordId.ToString() + "=" + recordID.ToString();
        }

        /// <summary>
        /// Method using for generating uri with custom attributes.
        /// </summary>
        /// <param name="pageName">Page name.</param>
        /// <param name="item">QuerString item which contains atttribute and value.</param>
        /// <returns>Return query string.</returns>
        protected string GenerateURI(string pageName, QueryStrings item)
        {
            string querystring = "";
            if (item != null)
                querystring = GetQueryStringBuilderInstance().AddQueryItem(item);
            return pageName + "?" + querystring;
        }
        /// <summary>
        /// Method using for generating uri with custom multiple attributes.
        /// </summary>
        /// <param name="pageName">Page name.</param>
        /// <param name="item">QuerString list which contains atttribute and value.</param>
        /// <returns>Return query string.</returns>
        protected string GenerateURI(string pageName, List<QueryStrings> queryList)
        {
            string querystring = "";
            if (queryList.Count > 0)
                querystring = GetQueryStringBuilderInstance().AddQueryList(queryList);
            return pageName + "?" + querystring;
        }

        /// <summary>
        /// If the user doesn't have rights for opening page than we redirect user to Home page
        /// </summary>
        protected void RedirectHome()
        {
            Session["PreviousPage"] = Request.RawUrl;
            bool isCallback = CommonMethods.IsCallbackRequest(this.Request);
            if (isCallback)
                ASPxWebControl.RedirectOnCallback("~/Default.aspx");
            else
                Response.Redirect("~/Default.aspx");
        }
        #endregion

        #region Instance Extractor
        protected DatabaseConnection GetDatabaseConnectionInstance()
        {
            DatabaseConnection dbConnection = null;

            if (dbConnection == null)
                return new DatabaseConnection();

            return dbConnection;
        }
        protected ClientDataProvider GetClientDataProviderInstance()
        {
            ClientDataProvider clientProvider = null;

            if (clientProvider == null)
                return new ClientDataProvider();

            return clientProvider;
        }

        protected QueryStringBuilder GetQueryStringBuilderInstance()
        {
            QueryStringBuilder queryStringBuilder = null;

            if (queryStringBuilder == null)
                return new QueryStringBuilder();

            return queryStringBuilder;
        }

        protected EmployeeDataProvider GetEmployeeDataProviderInstance()
        {
            EmployeeDataProvider employeeProvider = null;

            if (employeeProvider == null)
                return new EmployeeDataProvider();

            return employeeProvider;
        }
        protected EventDataProvider GetEventDataProviderInstance()
        {
            EventDataProvider eventProvider = null;

            if (eventProvider == null)
                return new EventDataProvider();

            return eventProvider;
        }
        #endregion

        #region Client POP UP handeling
        protected void ShowClientPopUp(string message, int popUpWindow = 0, string title = "")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CommonJS", String.Format("ShowErrorPopUp({0}, {1}, {2});", message, popUpWindow, title), true);
        }

        /// <summary>
        /// We are using this popup if the session showWarning contains value
        /// </summary>
        /// <param name="message">Message that will show on popup.</param>
        protected void ShowWarningPopUp(string message, int popUpWindow = 0, string title = "")
        {
            if (SessionHasValue(Enums.CommonSession.ShowWarning))
            {
                if (GetBoolValueFromSession(Enums.CommonSession.ShowWarning))
                    ShowClientPopUp(message, popUpWindow, title);

                RemoveSession(Enums.CommonSession.ShowWarning);
            }
        }
        #endregion

        #region Spinner Loading
        protected void ShowSpinnerLoader()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CommonJS", String.Format("ShowSpinnerLoader();"), true);
        }
        protected void HideSpinnerLoader()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CommonJS", String.Format("HideSpinnerLoader();"), true);
        }
        #endregion

        protected DataTable SerializeToDataTable<T>(List<T> list, string keyFieldName = "", string visibleColumn = "")
        {
            DataTable dt = new DataTable();
            string json = JsonConvert.SerializeObject(list);
            dt = JsonConvert.DeserializeObject<DataTable>(json);

            if (keyFieldName != "" && visibleColumn != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.NewRow();
                row[keyFieldName] = -1;
                row[visibleColumn] = "Izberi...";
                dt.Rows.InsertAt(row, 0);
            }

            return dt;
        }

        protected T CheckModelValidation<T>(WebResponseContentModel<T> instance)
        {
            object obj = default(T);

            if (!instance.IsRequestSuccesful)
            {
                string requestFailedError = "";

                if (!String.IsNullOrEmpty(instance.ValidationError))
                {
                    instance.ValidationError = instance.ValidationError.Replace("'", "");
                    instance.ValidationError = instance.ValidationError.Insert(0, "'");
                    instance.ValidationError += "'";
                    instance.ValidationError = instance.ValidationError.Replace("\\", "\\\\");
                    instance.ValidationError = instance.ValidationError.Replace("\r\n", "");
                    requestFailedError = instance.ValidationError;
                }
                else if (!String.IsNullOrEmpty(instance.ValidationErrorAppSide))
                    requestFailedError = instance.ValidationErrorAppSide;

                CommonMethods.LogThis("Request failed! => Message: \r\n" + requestFailedError);
                ShowClientPopUp(requestFailedError);

                return (T)obj;
            }
            else
            {
                obj = instance.Content;
            }

            return (T)obj;
        }


        protected void UserActionConfirmBtnUpdate(ASPxButton button, int userAction, bool popUpBtn = false)
        {
            if (userAction == (int)Enums.UserAction.Delete)
            {
                button.ImageUrl = popUpBtn ? "~/Images/trashPopUp.png" : "~/Images/trash2.png";
                button.Text = "Izbrisi";
            }
            else if (userAction == (int)Enums.UserAction.Add)
            {
                button.ImageUrl = popUpBtn ? "~/Images/addPopUp.png" : "~/Images/add2.png";
                button.Text = "Shrani";
            }
            else
            {
                button.ImageUrl = popUpBtn ? "~/Images/editPopup.png" : "~/Images/edit2.png";
                button.Text = "Shrani";
            }
        }

        protected void EnabledDeleteAndEditBtnPopUp(ASPxButton buttonEdit, ASPxButton buttonDelete, bool disable = true)
        {
            if (disable)
            {
                buttonEdit.ImageUrl = "~/Images/btnPopUpEditDisabled.png";
                buttonEdit.Text = "Spremeni";
                buttonEdit.Enabled = false;

                buttonDelete.ImageUrl = "~/Images/btnPopUpDeleteDisabled.png";
                buttonDelete.Text = "Izbrisi";
                buttonDelete.Enabled = false;
            }
            else
            {
                buttonEdit.ImageUrl = "~/Images/editForPopup.png";
                buttonEdit.Text = "Spremeni";
                buttonEdit.Enabled = true;

                buttonDelete.ImageUrl = "~/Images/trashForPopUp.png";
                buttonDelete.Text = "Izbrisi";
                buttonDelete.Enabled = true;
            }
        }
        protected void EnabledAddBtnPopUp(ASPxButton buttonAdd, bool disable = true)
        {
            if (disable)
            {
                buttonAdd.ImageUrl = "~/Images/addPopupDisabled.png";
                buttonAdd.Text = "Spremeni";
                buttonAdd.Enabled = false;
            }
            else
            {
                buttonAdd.ImageUrl = "~/Images/addPopUp.png";
                buttonAdd.Text = "Spremeni";
                buttonAdd.Enabled = true;
            }
        }
    }
}