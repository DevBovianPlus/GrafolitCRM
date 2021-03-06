﻿using AnalizaProdaje.UserControls;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AnalizaProdaje.Domain.Concrete;
using AnalizaProdaje.Domain.Helpers;

namespace AnalizaProdaje.Pages
{
    public partial class SalesGraphs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.IsAuthenticated)
            {
                //we add the path of menu here because before there is no info about user role
                Session["MainMenuSaleAnalysis"] = "P:\\Projects\\TempProjects\\AnalizaProdaje\\AnalizaProdaje\\App_Data\\Nav_bar\\MainMenu.xml";
            }

            if (Session["GraphCollection"] != null)
            {
                ASPxCallbackPanel1.Controls.Clear();
                AddControlsToPanel((List<GraphBinding>)Session["GraphCollection"]);
            }
        }


        private void AddControlsToPanel(List<GraphBinding> collection)
        {
            foreach (GraphBinding item in collection)
            {
                UserControlGraph ucf2 = (UserControlGraph)LoadControl("~/UserControls/UserControlGraph.ascx");
                ucf2.btnPostClk += ucf2_btnPostClk;
                ucf2.btnDeleteGraphClick += ucf2_btnDeleteGraphClick;
                ucf2.btnAddEventClick += ucf2_btnAddEventClick;
                ASPxCallbackPanel1.Controls.Add(ucf2);
            }
        }

        protected void ASPxCallbackPanel1_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            List<GraphBinding> bindingCollection = new List<GraphBinding>();

            bindingCollection.Select(item => item.control).ToList();//get all controls and only controls from list

            Session["MainContentDivWidth"] = hiddenField["browserWidth"];

            UserControlGraph ucf2 = (UserControlGraph)LoadControl("~/UserControls/UserControlGraph.ascx");
            
            ucf2.btnPostClk += ucf2_btnPostClk;
            ucf2.btnDeleteGraphClick += ucf2_btnDeleteGraphClick;
            ucf2.btnAddEventClick += ucf2_btnAddEventClick;
            if (Session["GraphCollection"] != null)
            {
                bindingCollection = (List<GraphBinding>)Session["GraphCollection"];

                RefresGraphsCallbackPanel();

                //bindingCollection.Add(new GraphBinding() { control = ucf2, graphDataTable = null });
                ASPxCallbackPanel1.Controls.Add(ucf2);
            }
            else
            {
                //bindingCollection.Add(new GraphBinding() { control = ucf2, graphDataTable = null });
                ASPxCallbackPanel1.Controls.Add(ucf2);
            }

            Session["GraphCollection"] = bindingCollection;//ASPxCallbackPanel1.Controls;

        }

        void ucf2_btnAddEventClick(object sender, EventArgs e)
        {
            
        }

        public void ucf2_callbackTest(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {

        }


        private void ucf2_btnPostClk(object sender, EventArgs e)
        {
            DatabaseConnection dbconnection = new DatabaseConnection();

            UserControlGraph ucf2 = ((UserControlGraph)(sender));

            int index = ASPxCallbackPanel1.Controls.IndexOf(ucf2);
            DataTable dt = dbconnection.GetDataForGraphRendering();
            
            if (index >= 0)
            {
                //TODO: Call web service metods to get data for grapf rendering
                List<GraphBinding> bindingCollection = (List<GraphBinding>)Session["GraphCollection"];
                //bindingCollection.Find(p => p.control.UniqueID == ASPxCallbackPanel1.Controls[index].UniqueID).graphDataTable = dt;
                Session["GraphCollection"] = bindingCollection;
            }

           // ucf2.CreateGraph(/*ucf2.Points.Text, ucf2.LineColor.Text*/dt);
            RefresGraphsCallbackPanel();
        }

        private void ucf2_btnDeleteGraphClick(object sender, EventArgs e)
        {
            UserControlGraph id = (UserControlGraph)sender;

            ASPxCallbackPanel1.Controls.Remove(id);
            
            List<GraphBinding> bindingCollection = new List<GraphBinding>();
            if (Session["GraphCollection"] != null)
            {
                bindingCollection = (List<GraphBinding>)Session["GraphCollection"];


                Control[] array = new Control[ASPxCallbackPanel1.Controls.Count];
                ASPxCallbackPanel1.Controls.CopyTo(array, 0);
                List<Control> collection = array.ToList();

                bool eraseControl = false;
                for (int i = bindingCollection.Count - 1; i >= 0; i--)
                {
                    if (collection.Count == 0)//TODO: Ulovi robni primer če izbišre zadnjega!
                        bindingCollection.RemoveAt(i);

                    foreach (Control callbackControls in collection)
                    {
                        if (bindingCollection[i].control.UniqueID.Equals(callbackControls.UniqueID))
                        {
                            collection.Remove(callbackControls);
                            break;
                        }
                        else
                        {
                            int index = collection.IndexOf(callbackControls);
                            if ((index + 1) == collection.Count)
                                eraseControl = true;
                        }
                    }

                    if (eraseControl)
                    {
                        bindingCollection.RemoveAt(i);
                        eraseControl = false;
                    }
                }
            }
            //ControlCollection collection = ASPxCallbackPanel1.Controls;
            Session["GraphCollection"] = bindingCollection;//collection.Count > 0 ? collection : null;
            RefresGraphsCallbackPanel();
        }

        private void RefresGraphsCallbackPanel()
        {
            if (Session["GraphCollection"] != null)
            {
                List<GraphBinding> bindingCollection = (List<GraphBinding>)Session["GraphCollection"];

                foreach (Control item in ASPxCallbackPanel1.Controls)
                {
                   /* DataTable dt = bindingCollection.Find(gb => gb.control.UniqueID == item.UniqueID).graphDataTable;
                    UserControlGraph ucf2as = (UserControlGraph)item;
                    ucf2as.CreateGraph(dt);*/
                }
            }
        }
    }
}