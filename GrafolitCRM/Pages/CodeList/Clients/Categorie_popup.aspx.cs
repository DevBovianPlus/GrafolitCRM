﻿using AnalizaProdaje.Common;
using AnalizaProdaje.Domain.Concrete;
using AnalizaProdaje.Domain.Helpers;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DevExpress.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AnalizaProdaje.Pages.CodeList.Clients
{
    public partial class Categorie_popup : ServerMasterPage
    {
        ClientCategorieModel model = null;
        int clientCategorieID = -1;
        int action = -1;
        int clientID = -1;
        int categorieID = -1;
        protected void Page_Init(object sender, EventArgs e)
        {
            clientID = CommonMethods.ParseInt(GetStringValueFromSession(Enums.ClientSession.ClientId));
            action = CommonMethods.ParseInt(GetStringValueFromSession(Enums.CommonSession.UserActionPopUp));
            clientCategorieID = CommonMethods.ParseInt(GetStringValueFromSession(Enums.ClientSession.ClientCategoriePopUpID));
            categorieID = CommonMethods.ParseInt(GetStringValueFromSession(Enums.ClientSession.CategorieID));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ComboBoxKategorije.DataBind();
                if (action == (int)Enums.UserAction.Edit || action == (int)Enums.UserAction.Delete)
                {
                    if (clientCategorieID > 0 && SessionHasValue(Enums.ClientSession.ClientModel))
                    {
                        model = GetClientDataProviderInstance().GetCategorieFromClientModelSession(clientCategorieID, clientID);
                        FillForm();
                    }
                }
                else if (action == (int)Enums.UserAction.Add)//acion ADD
                {
                    txtIdStranke.Text = clientID.ToString();
                    ComboBoxKategorije.SelectedIndex = 0;
                }
                UserActionConfirmBtnUpdate(btnConfirmPopUp, action, true);
            }
            Initialize();
            
        }

        private void FillForm()
        {
            txtClientCategorieID.Text = model.idStrankaKategorija.ToString();
            txtIdStranke.Text = model.idStranka.ToString();
            ComboBoxKategorije.Value = model.idKategorija;
            ComboBoxKategorije.SelectedIndex = ComboBoxKategorije.Items.IndexOfValue(model.idKategorija.ToString());
        }

        private bool AddOrEditEntityObject(bool add = false)
        {
            if (add)
            {
                model = new ClientCategorieModel();

                model.idStrankaKategorija = 0;
                model.idStranka = clientID;
                model.tsIDOseba = PrincipalHelper.GetUserPrincipal().ID;
                model.ts = DateTime.Now;
            }
            else if (model == null && !add)
            {
                model = GetClientDataProviderInstance().GetCategorieFromClientModelSession(clientCategorieID, clientID);
            }

            int selectedValue = CommonMethods.ParseInt(ComboBoxKategorije.Value.ToString());
            if (selectedValue <= 0)
                return false;
            else
                model.idKategorija = selectedValue;


            ClientCategorieModel newModel = CheckModelValidation(GetDatabaseConnectionInstance().SaveClientCategorieChanges(model));

            if (newModel != null)//If new record is added we need to refresh aspxgridview. We add new record to session model.
            {
                if (add)
                    return GetClientDataProviderInstance().AddCategorieToClientModelSession(newModel);
                else
                    return GetClientDataProviderInstance().UpdateCategorieToClientModelSession(newModel);
            }
            else
            {
                return false;
            }
        }

        protected void btnCancelPopUp_Click(object sender, EventArgs e)
        {
            RemoveSessionsAndClosePopUP();
        }

        protected void btnConfirmPopUp_Click(object sender, EventArgs e)
        {
            bool isValid = false;

            switch (action)
            {
                case (int)Enums.UserAction.Add:
                    isValid = AddOrEditEntityObject(true);
                    break;
                case (int)Enums.UserAction.Edit:
                    isValid = AddOrEditEntityObject();
                    break;
                case (int)Enums.UserAction.Delete:
                    isValid = DeletePlanObject();
                    break;
            }

            if (isValid)
                RemoveSessionsAndClosePopUP(true);
            else
                ShowClientPopUp("'Something went wrong. Contact administrator'", 1);
        }

        private bool DeletePlanObject()
        {
            bool isDeleted = CheckModelValidation(GetDatabaseConnectionInstance().DeleteClientCategorie(clientID, clientCategorieID));

            GetClientDataProviderInstance().DeleteCategorieFromClientModelSession(clientCategorieID, clientID);

            return isDeleted;
        }

        private void RemoveSessionsAndClosePopUP(bool confirm = false)
        {
            string confirmCancelAction = "Preklici";

            if (confirm)
                confirmCancelAction = "Potrdi";

            //RemoveSession(Enums.ClientSession.PlanPopUpID);
            RemoveSession(Enums.ClientSession.CategorieID);
            RemoveSession(Enums.CommonSession.UserActionPopUp);
            RemoveSession(Enums.ClientSession.ClientId);
            ClientScript.RegisterStartupScript(GetType(), "ANY_KEY", string.Format("window.parent.OnClosePopupEventHandler_Categorie('{0}');", confirmCancelAction), true);

        }

        protected void ComboBoxKategorije_DataBinding(object sender, EventArgs e)
        {
            List<CategorieModel> categories = CheckModelValidation<List<CategorieModel>>(GetDatabaseConnectionInstance().GetAllFreeClientCategories(clientID, categorieID));
            DataTable dt = new DataTable();

            if (categories != null)
            {
                string titleValue = "";
                string listCat = JsonConvert.SerializeObject(categories);
                dt = JsonConvert.DeserializeObject<DataTable>(listCat);

                if (categories.Count <= 0)
                {
                    dt.Columns.Add("idKategorija");
                    dt.Columns.Add("Naziv");
                    titleValue = "Vse kategorije že imate dodane...";
                    EnabledAddBtnPopUp(btnConfirmPopUp);
                }
                else
                {
                    titleValue = "Izberi...";
                    EnabledAddBtnPopUp(btnConfirmPopUp, false);
                }

                DataRow dr = dt.NewRow();
                dr["Naziv"] = titleValue;
                dr["idKategorija"] = -1;
                dt.Rows.InsertAt(dr, 0);
            }

            (sender as ASPxComboBox).DataSource = dt;
        }

        private void Initialize()
        {
            //txtLetnoZnesek.Attributes.Add("Onkeypress", "return isNumberKey_decimal(event);");
            //txtLeto.Attributes.Add("Onkeypress", "return isNumberKey_int(event);");
        }
    }
}