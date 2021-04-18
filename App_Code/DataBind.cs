
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace SiscorpSharp.App_Code
{
  public  class DataBind
    {

        //NPtEntidad en = new NPtEntidad();
        //NAnexoPrincipal an = new NAnexoPrincipal();
        //NPlanCuenta pl = new NPlanCuenta();
        //CapaEstilo.ClsEstilo lo_estilo = new CapaEstilo.ClsEstilo();
        //NHojaTrabajo ht = new NHojaTrabajo();
        public String ReportName { get; set; }
        public String ReportTitle { get; set; }
        public String Empresa { get; set; }
        public String Ruc { set; get; }
        public string tipoletra { set; get; }
        public string tamanioletra { set; get; }
        public void RDataBind(DataTable dt,string filter, bool showThisGrouped)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt.Copy());

            int longitud = dt.Columns.Count;
            int[] lista = new int[longitud];

            int tamano = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Length <= 3)
                {
                    tamano = 3;
                }
                else
                {
                    tamano = dt.Rows[0][i].ToString().Length;
                }
                lista[i] = tamano;
            }
            RDataBind(ds,filter, showThisGrouped,lista);
        }

        /// <summary>
        /// Bind Report With DataSet
        /// </summary>
        /// <param name="ds">DataSet</param>
        public void RDataBind(DataSet ds,string filter, bool showThisGrouped,int[] lista)
        {

            int count = 0;
            foreach (DataTable dt in ds.Tables)
            {
                count++;
                var report_name = "Report" + count;
                DataTable dt1 = new DataTable(report_name.ToString());
                dt1 = ds.Tables[count - 1];
                dt1.TableName = report_name.ToString();
            }


            //Report Viewer, Builder and Engine 
            ReportViewer ReportViewer1 = new ReportViewer();
            ReportViewer1.Reset();
            for (int i = 0; i < ds.Tables.Count; i++)
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(ds.Tables[i].TableName, ds.Tables[i]));

            ReportBuilder reportBuilder = new ReportBuilder();
            reportBuilder.DataSource = ds;

            reportBuilder.Page = new ReportPage();
            ReportSections reportFooter = new ReportSections();
            ReportItems reportFooterItems = new ReportItems();
            ReportTextBoxControl[] footerTxt = new ReportTextBoxControl[3];
            //string footer = string.Format("Copyright {0}         Report Generated On {1}          Page {2}", DateTime.Now.Year, DateTime.Now, ReportGlobalParameters.CurrentPageNumber);
            string footer = string.Format("Copyright  {0}         Report Generated On  {1}          Page  {2}  of {3} ", DateTime.Now.Year, DateTime.Now, ReportGlobalParameters.CurrentPageNumber, ReportGlobalParameters.TotalPages);
            footerTxt[0] = new ReportTextBoxControl() { Name = "txtCopyright", ValueOrExpression = new string[] { footer } };



            reportFooterItems.TextBoxControls = footerTxt;
            reportFooter.ReportControlItems = reportFooterItems;
            reportBuilder.Page.ReportFooter = reportFooter;

            ReportSections reportHeader = new ReportSections();
            reportHeader.Size = new ReportScale();
            reportHeader.Size.Height = 0.56849;

            ReportItems reportHeaderItems = new ReportItems();
            
            ReportTextBoxControl[] headerTxt = new ReportTextBoxControl[3];
            headerTxt[0] = new ReportTextBoxControl() { Name = "txtReportTitle", ValueOrExpression = new string[] { "NOMBRE DEL REPORTE: 2" + ReportTitle  } };       
           
            headerTxt[1] = new ReportTextBoxControl() { Name = "txtReportTitle2", ValueOrExpression = new string[] { "Razon Social: 0 " + Empresa } };
            headerTxt[2] = new ReportTextBoxControl() { Name = "txtReportTitle1", ValueOrExpression = new string[] { "Ruc: 1 " + Ruc } };
            reportHeaderItems.TextBoxControls = headerTxt;
            reportHeader.ReportControlItems = reportHeaderItems;
            reportBuilder.Page.ReportHeader = reportHeader;
              
            ReportViewer1.LocalReport.LoadReportDefinition(ReportEngine.GenerateReport(reportBuilder,filter, showThisGrouped,lista));
            ReportViewer1.LocalReport.DisplayName = ReportName;
            ReportViewer1.Dock = DockStyle.Fill;
            Form Frm = new Form();
            Frm.Size = new Size(750, 550);
            Frm.Controls.Add(ReportViewer1);
            Frm.Show();
            ReportViewer1.RefreshReport();
        }

    }
}
