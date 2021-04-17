using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SiscorpSharp.App_Code;

namespace TestRDL
{
    public partial class Form1 : Form
    {
        string[] Header = { "Id", "Nombre", "RUC", "Direccion", "Telefono" };

        public Form1()
        {
            InitializeComponent();
            rdb_NotGrouped.Checked = true;
            panelFilter.Visible = false;
            List<string> optionFilter = new List<string>();
            cmbFilters.Items.AddRange(Header);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable custTable = new DataTable();
            for (int y = 0; y < Header.Length; y++) {
                custTable.Columns.Add(Header[y], typeof(string));
            }

            DataRow myDataRow;
            myDataRow = custTable.NewRow();
            myDataRow[Header[0]] = "1";
            myDataRow[Header[1]] = "Juan manuel calderon robles alias el cotorro";
            myDataRow[Header[2]] = "11111111111";
            myDataRow[Header[3]] = "Avenida leguia sin numero";
            myDataRow[Header[4]] = "928364728";
            custTable.Rows.Add(myDataRow);

            myDataRow = custTable.NewRow();
            myDataRow[Header[0]] = "2";
            myDataRow[Header[1]] = "Brandon";
            myDataRow[Header[2]] = "11111111111";
            myDataRow[Header[3]] = "AvX";
            myDataRow[Header[4]] = "928364728";
            custTable.Rows.Add(myDataRow);

            myDataRow = custTable.NewRow();
            myDataRow[Header[0]] = "3";
            myDataRow[Header[1]] = "Juan";
            myDataRow[Header[2]] = "22222222222";
            myDataRow[Header[3]] = "AvY";
            myDataRow[Header[4]] = "928364728";
            custTable.Rows.Add(myDataRow);

            myDataRow = custTable.NewRow();
            myDataRow[Header[0]] = "4";
            myDataRow[Header[1]] = "Jean Carlos";
            myDataRow[Header[2]] = "22222222222";
            myDataRow[Header[3]] = "AvZ";
            myDataRow[Header[4]] = "928364728";
            custTable.Rows.Add(myDataRow);

            myDataRow = custTable.NewRow();
            myDataRow[Header[0]] = "5";
            myDataRow[Header[1]] = "Brandon";
            myDataRow[Header[2]] = "33333333333";
            myDataRow[Header[3]] = "AvZ";
            myDataRow[Header[4]] = "928364728";
            custTable.Rows.Add(myDataRow);


            DataBind databind = new DataBind();
            databind.Empresa = "AVEO International"; // Razon Social
            databind.ReportTitle = " Reporte usuarios"; // Titulo 
            databind.Ruc = "12345678912";

            bool showThisGrouped = rdb_Grouped.Checked;

                     

            databind.RDataBind(custTable,cmbFilters.Text, showThisGrouped);
        }

        private void rdb_Grouped_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_Grouped.Checked)
            {
                panelFilter.Show();
            }
            else {
                panelFilter.Visible = false;
            }

            if (!(rdb_Grouped.Checked)) panelFilter.Visible = false; 
        }
    }
}
