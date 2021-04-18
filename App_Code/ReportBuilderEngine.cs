using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Summary description for ReportBuilderEngine
/// </summary>
public static class ReportEngine
{

    private static int count =2;
    private static  double TamanoTitulo = 0;
    #region Initialize
    public static Stream GenerateReport(ReportBuilder reportBuilder,string filter,bool showThisGrouped,int [] lista)
    {
        Stream ret = new MemoryStream(Encoding.UTF8.GetBytes(GetReportData(reportBuilder,filter,showThisGrouped,lista)));
        return ret;
    }
 
    static ReportBuilder InitAutoGenerateReport(ReportBuilder reportBuilder,int[] lista)
    {
        if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
        {
            DataSet ds = reportBuilder.DataSource;

            int _TablesCount = ds.Tables.Count;
            ReportTable[] reportTables = new ReportTable[_TablesCount];

            if (reportBuilder.AutoGenerateReport)
            {
                for (int j = 0; j < _TablesCount; j++)
                {
                    DataTable dt = ds.Tables[j];
                    ReportColumns[] columns = new ReportColumns[dt.Columns.Count]; 
                    ReportDimensions ColumnPadding = new ReportDimensions();
                    ColumnPadding.Default = 2;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ReportScale ColumnScale = new ReportScale();
                        // ColumnScale.Width = 4;
                        ColumnScale.Height = 1;
                        ColumnScale.Width = 0.25 * lista[i];
                        TamanoTitulo += (0.25 * lista[i]);
                        columns[i] = new ReportColumns() { ColumnCell = new ReportTextBoxControl() 
                        { Name = dt.Columns[i].ColumnName, Size = ColumnScale, Padding = ColumnPadding },
                          HeaderText = dt.Columns[i].ColumnName, HeaderColumnPadding = ColumnPadding   };                        
                    }
                    reportTables[j] = new ReportTable() { ReportName = dt.TableName, ReportDataColumns = columns };
                }
            }
            reportBuilder.Body = new ReportBody();
            reportBuilder.Body.ReportControlItems = new ReportItems();
            reportBuilder.Body.ReportControlItems.ReportTable = reportTables;
        }
        return reportBuilder;
    }
    static string GetReportData(ReportBuilder reportBuilder,string filter, bool showThisGrouped,int [] lista)
    {
        reportBuilder = InitAutoGenerateReport(reportBuilder,lista);
        string rdlcXML = "";
        rdlcXML += @"<?xml version=""1.0"" encoding=""utf-8""?> 
                        <Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition""  
                        xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner""> 
                      <Body>";

        string _tableData = GenerateTable(reportBuilder,filter, showThisGrouped);

        if (_tableData.Trim() != "")
        {
            rdlcXML += @"<ReportItems>" + _tableData + @"</ReportItems>";
        }
       byte[] imgBinary = File.ReadAllBytes(Application.StartupPath + "\\logo.png");
        rdlcXML += @"<Height>2.1162cm</Height> 
                        <Style /> 
                      </Body> 
                      <Width>20.8cm</Width> 
                      <Page> 
                        " + GetPageHeader(reportBuilder) + GetFooter(reportBuilder) + GetReportPageSettings() + @" 
                        <Style /> 
                      </Page> 
                      <AutoRefresh>0</AutoRefresh> 
                        " + GetDataSet(reportBuilder) + @" 
                      <EmbeddedImages> 
                        <EmbeddedImage Name=""Logo""> 
                          <MIMEType>image/png</MIMEType> 
                          <ImageData>" + System.Convert.ToBase64String(imgBinary) + @"</ImageData> 
                        </EmbeddedImage> 
                      </EmbeddedImages> 
                      <Language>es-PE</Language> 
                      <ConsumeContainerWhitespace>true</ConsumeContainerWhitespace> 
                      <rd:ReportUnitType>Cm</rd:ReportUnitType> 
                      <rd:ReportID>17efa4a3-5c39-4892-a44b-fbde95c96585</rd:ReportID> 
                    </Report>";
        return rdlcXML;
    }
    #endregion

    #region Page Settings
    static string GetReportPageSettings()
    {
        return @" <PageHeight>21cm</PageHeight> 
                    <PageWidth>29.7cm</PageWidth> 
                    <LeftMargin>0.1pt</LeftMargin> 
                    <RightMargin>0.1pt</RightMargin> 
                    <TopMargin>0.1pt</TopMargin> 
                    <BottomMargin>0.1pt</BottomMargin> 
                    <ColumnSpacing>1pt</ColumnSpacing>";
    }
    private static string GetPageHeader(ReportBuilder reportBuilder)
    {
        //< Image Name = ""Image1"" >   
        // < Source > Embedded </ Source >   
        // < Value > Logo </ Value >   
        // < Sizing > FitProportional </ Sizing >   
        // < Top > 0.05807in</ Top >      
        //    < Left > 1cm </ Left >         
        //       < Height > 0.4375in</ Height >            
        //          < Width > 1.36459in</ Width >               
        //             < ZIndex > 1 </ ZIndex >               
        //             < Style />               
        //           </ Image >
        string strHeader = "";
        if (reportBuilder.Page == null || reportBuilder.Page.ReportHeader == null) return "";
        ReportSections reportHeader = reportBuilder.Page.ReportHeader;
        strHeader = @"<PageHeader> 
                          <Height>0.56849in</Height> 
                          <PrintOnFirstPage>" + reportHeader.PrintOnFirstPage.ToString().ToLower() + @"</PrintOnFirstPage> 
                          <PrintOnLastPage>" + reportHeader.PrintOnLastPage.ToString().ToLower() + @"</PrintOnLastPage> 
                          <ReportItems>";
        ReportTextBoxControl[] headerTxt = reportBuilder.Page.ReportHeader.ReportControlItems.TextBoxControls;
        string left = "1cm";
        var titulo = headerTxt[0].ValueOrExpression;
        var empresa = headerTxt[1].ValueOrExpression;
        var ruc = headerTxt[2].ValueOrExpression;
        if (headerTxt != null)
            //for (int i = 0; i < headerTxt.Count(); i++)
            //{          
            //    strHeader += GetHeaderTextBox(headerTxt[i].Name, left, null, headerTxt[i].ValueOrExpression);
            //}
             strHeader += TituloReporte(titulo[0].ToString(), empresa[0].ToString(), ruc[0].ToString());

        strHeader += @" </ReportItems> 
                          <Style /> 
                        </PageHeader>";
        return strHeader;
    }
    
    private static string TituloReporte(string titulo, string empresa, string ruc)
    {
        string TitleHeader = @"<Textbox Name='txtReportTitle'> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns><TextRun> 
                  <Value>"+ empresa +@"</Value> 
                  <Style> 
                    <FontSize>10pt</FontSize> 
                    <FontWeight>Bold</FontWeight>                  
                  </Style> 
                </TextRun></TextRuns> 
              <Style /> 
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>txtReportTitle</rd:DefaultName> 
          <Top>0.5cm</Top> 
          <Left>1cm</Left>           
              <ZIndex>0</ZIndex> 
              <Style> 
                <Border> 
                  <Style>None</Style> 
                </Border></Style> 
      </Textbox>";

        string rucempresa = @" <Textbox Name='txtReportTitle1'> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns><TextRun> 
                             <Value>"+ ruc +@"</Value> 
                              <Style> 
                                <FontSize>10pt</FontSize> 
                                <FontWeight>Bold</FontWeight>                  
                              </Style> 
                            </TextRun></TextRuns> 
                          <Style /> 
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>txtReportTitle1</rd:DefaultName> 
                      <Top>0.6cm</Top> 
                      <Left>1cm</Left> 
                     
                      <ZIndex>1</ZIndex> 
                      <Style> 
                        <Border> 
                          <Style>None</Style> 
                        </Border></Style> 
             </Textbox>";
        TitleHeader += rucempresa;

        string tituloReporte = @"
        <Textbox Name='txtReportTitle2'> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns>   
            <TextRun>   
            <Value>"+ titulo +@"</Value> 
                  <Style>                                 
                    <FontSize>10pt</FontSize> 
                    <FontWeight>Bold</FontWeight>                  
                  </Style> 
                </TextRun>
             </TextRuns> 
                 <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>txtReportTitle2</rd:DefaultName> 
          <Top>0.9cm</Top> 
          <Left>2cm</Left> 
          <Height>0.6cm</Height> 
          <Width>"+ TamanoTitulo.ToString()+"cm" +@" </Width> 
          <ZIndex>2</ZIndex> 
          <Style> 
            <Border> 
              <Style>None</Style> 
            </Border></Style> 
        </Textbox>";
        TitleHeader += tituloReporte;
        return TitleHeader;
    }
    private static string GetFooter(ReportBuilder reportBuilder)
    {
        string strFooter = "";
        if (reportBuilder.Page == null || reportBuilder.Page.ReportFooter == null) return "";
        strFooter = @"<PageFooter> 
                          <Height>0.68425in</Height> 
                          <PrintOnFirstPage>true</PrintOnFirstPage> 
                          <PrintOnLastPage>true</PrintOnLastPage> 
                          <ReportItems>";
        ReportTextBoxControl[] footerTxt = reportBuilder.Page.ReportFooter.ReportControlItems.TextBoxControls;
        if (footerTxt != null)
            for (int i = 0; i < footerTxt.Count(); i++)
            {
                if (footerTxt[i] != null)
                {
                    strFooter += GetFooterTextBox(footerTxt[i].Name, null, footerTxt[i].ValueOrExpression);
                }
            }
        strFooter += @"</ReportItems> 
                          <Style /> 
                        </PageFooter>";
        return strFooter;
    }
    #endregion

    #region Dataset
    static string GetDataSet(ReportBuilder reportBuilder)
    {
        string dataSetStr = "";
        if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
        {
            string dsName = "rptCustomers";
            dataSetStr += @"<DataSources> 
                                <DataSource Name=""" + dsName + @"""> 
                                  <ConnectionProperties> 
                                    <DataProvider>System.Data.DataSet</DataProvider> 
                                    <ConnectString>/* Local Connection */</ConnectString> 
                                  </ConnectionProperties> 
                                  <rd:DataSourceID>944b21fd-a128-4363-a5fc-312a032950a0</rd:DataSourceID> 
                                </DataSource> 
                          </DataSources> 
                <DataSets>"
                         + GetDataSetTables(reportBuilder.Body.ReportControlItems.ReportTable, dsName) +
              @"</DataSets>";
        }
        return dataSetStr;
    }
    private static string GetDataSetTables(ReportTable[] tables, string DataSourceName)
    {
        string strTables = "";
        for (int i = 0; i < tables.Length; i++)
        {
            strTables += @"<DataSet Name=""" + tables[i].ReportName + @"""> 
              <Query> 
                <DataSourceName>" + DataSourceName + @"</DataSourceName> 
                <CommandText>/* Local Query */</CommandText> 
              </Query> 
             " + GetDataSetFields(tables[i].ReportDataColumns) + @" 
            </DataSet>";
        }
        return strTables;
    }
    private static string GetDataSetFields(ReportColumns[] reportColumns)
    {
        string strFields = "";

        strFields += @"<Fields>";
        for (int i = 0; i < reportColumns.Length; i++)
        {
            strFields += @"<Field Name=""" + reportColumns[i].ColumnCell.Name + @"""> 
          <DataField>" + reportColumns[i].ColumnCell.Name + @"</DataField> 
          <rd:TypeName>System.String</rd:TypeName> 
        </Field>";
        }
        strFields += @"</Fields>";
        return strFields;
    }
    #endregion

    #region Report Table Configuration
    static string GenerateTable(ReportBuilder reportBuilder,string filter, bool showThisGrouped)
    {
        string TableStr = "";
        if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
        {
            ReportTable table = new ReportTable();
            if (showThisGrouped)
            {
                for (int i = 0; i < reportBuilder.Body.ReportControlItems.ReportTable.Length; i++)
                {
                    table = reportBuilder.Body.ReportControlItems.ReportTable[i];
                    TableStr += @"<Tablix Name=""table_" + table.ReportName + @"""> 
                                <TablixBody>" + GetTableColumns(reportBuilder, table) + @" 
                                    <TablixRows> 
                                            " + GenerateTableHeaderRow(reportBuilder, table)
                                                  + GenerateFirstWhiteSpacesRow(reportBuilder, table, filter)
                                                  + GenerateTableRow(reportBuilder, table)
                                                  + GenerateWhiteSpacesRow(reportBuilder, table)
                                                  + @" 
                                    </TablixRows> 
                                 </TablixBody>" + GetTableColumnHeirarchy(reportBuilder, table) + @" 
                                 <TablixRowHierarchy> 
                                    <TablixMembers>
                                        <TablixMember/>
                                        <TablixMember>" +
                                                   GenerateGroupFilter(filter)
                                                    + @"<TablixMembers>                       
                                                        <TablixMember> 
                                                            <KeepWithGroup>After</KeepWithGroup> 
                                                        </TablixMember> 
                                                        <TablixMember> 
                                                         <Group Name=""" + table.ReportName + "_Details" + @""" /> 
                                                        </TablixMember> 
                                                        <TablixMember>
                                                          <KeepWithGroup>Before</KeepWithGroup>
                                                        </TablixMember>
                                                    </TablixMembers> 
                                        </TablixMember>
                                     </TablixMembers>
                                </TablixRowHierarchy> 
                <RepeatColumnHeaders>true</RepeatColumnHeaders> 
                <RepeatRowHeaders>true</RepeatRowHeaders> 
                <DataSetName>" + table.ReportName + @"</DataSetName>" + GetSortingDetails(reportBuilder) + @" 
                <Top>0.07056cm</Top> 
                <Left>1cm</Left> 
                <Height>1.2cm</Height> 
                <Width>7.5cm</Width> 
                <ZIndex>1</ZIndex>
                <Style> 
                  <Border> 
                    <Style>None</Style> 
                  </Border> 
                </Style> 
              </Tablix>";
                }
            }
            else {
                for (int i = 0; i < reportBuilder.Body.ReportControlItems.ReportTable.Length; i++) {
                    table = reportBuilder.Body.ReportControlItems.ReportTable[i];
                    TableStr += @"<Tablix Name=""table_" + table.ReportName + @"""> 
                                <TablixBody>" + GetTableColumns(reportBuilder, table) + @" 
                                    <TablixRows> 
                                            " + GenerateTableHeaderRow(reportBuilder, table)
                                                  + GenerateTableRow(reportBuilder, table)
                                                  + @" 
                                    </TablixRows> 
                                 </TablixBody>" + GetTableColumnHeirarchy(reportBuilder, table) + @" 
                                 <TablixRowHierarchy> 
                                    <TablixMembers>
                                            <TablixMember/>
                                            <TablixMember>
                                               <Group Name=""" + table.ReportName + "_Details" + @""" /> 
                                            </TablixMember> 
                                     </TablixMembers>
                                </TablixRowHierarchy> 
                <RepeatColumnHeaders>true</RepeatColumnHeaders> 
                <RepeatRowHeaders>true</RepeatRowHeaders> 
                <DataSetName>" + table.ReportName + @"</DataSetName>" + GetSortingDetails(reportBuilder) + @" 
                <Top>0.07056cm</Top> 
                <Left>1cm</Left> 
                <Height>1.2cm</Height> 
                <Width>7.5cm</Width> 
                <ZIndex>1</ZIndex>
                <Style> 
                  <Border> 
                    <Style>None</Style> 
                  </Border> 
                </Style> 
              </Tablix>";
                }
            }
        }
        return TableStr;
    }

    static string GenerateGroupFilter(string filter)
    {
        //string filter = "RUC";
        string strTableRow = "";
        strTableRow = @"<Group Name="""+filter+@""">
                        <GroupExpressions>
                            <GroupExpression>=Fields!"+filter+@".Value </GroupExpression>
                        </GroupExpressions>
                      </Group>";

        strTableRow += @"<SortExpressions>
                            <SortExpression>
                                <Value>=Fields!"+filter+@".Value</Value>
                            </SortExpression>
                         </SortExpressions>";
        return strTableRow;
    }

    static string GetSortingDetails(ReportBuilder reportBuilder)
    {
        return "";
        ReportTable[] tables = reportBuilder.Body.ReportControlItems.ReportTable;
        ReportColumns[] columns = reportBuilder.Body.ReportControlItems.ReportTable[0].ReportDataColumns;
        ReportTextBoxControl sortColumn = new ReportTextBoxControl();
        if (columns == null) return "";

        string strSorting = "";
        strSorting = @" <SortExpressions>";
        for (int i = 0; i < columns.Length; i++)
        {
            sortColumn = columns[i].ColumnCell;
            strSorting += "<SortExpression><Value>=Fields!" + sortColumn.Name + @".Value</Value>";
            if (columns[i].SortDirection == ReportSort.Descending)
                strSorting += "<Direction>Descending</Direction>";
            strSorting += @"</SortExpression>";
        }
        strSorting += "</SortExpressions>";
        return strSorting;
    }
    static string GenerateTableRow(ReportBuilder reportBuilder, ReportTable table)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();
        ReportScale colHeight = ColumnCell.Size;    
        ReportDimensions padding = new ReportDimensions();
        if (columns == null) return "";

        string strTableRow = "";
        strTableRow = @"<TablixRow> 
                        <Height>0.6cm</Height> 
                <TablixCells>";
                    for (int i = 0; i < columns.Length; i++)
                    {
                        ColumnCell = columns[i].ColumnCell;
                        padding = ColumnCell.Padding;
                        strTableRow += @"<TablixCell> 
                              <CellContents> 
                               " + GenerateTextBox("txtCell_" + table.ReportName + "_", ColumnCell.Name, "", true, padding) + @" 
                              </CellContents> 
                            </TablixCell>";
                    }
        strTableRow += @"</TablixCells></TablixRow>";
        return strTableRow;
    }

    static string GenerateFirstWhiteSpacesRow(ReportBuilder reportBuilder, ReportTable table,string filter)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();
        ReportScale colHeight = ColumnCell.Size;
        ReportDimensions padding = new ReportDimensions();
        if (columns == null) return "";

        string strTableRow = "";
        strTableRow += @"<TablixRow> 
                        <Height>0.6cm</Height> 
                <TablixCells>";
        for (int i = 0; i < columns.Length; i++)
        {
            if (i == 0)
            {
                ColumnCell = columns[i].ColumnCell;
                   padding = ColumnCell.Padding;
                strTableRow += @"<TablixCell> 
                              <CellContents> 
                               " + GenerateFirstRowWhiteTextBox(filter, filter, ColumnCell.Name, "", true, padding) + @" 
                              </CellContents> 
                            </TablixCell>";
            }
            else {
                count += 3;
                ColumnCell = columns[i].ColumnCell;
                padding = ColumnCell.Padding;
                strTableRow += @"<TablixCell />";
            }
        }
        strTableRow += @"</TablixCells>
                    </TablixRow>";

        return strTableRow;
    }

    static string GenerateWhiteSpacesRow(ReportBuilder reportBuilder, ReportTable table)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();
        ReportScale colHeight = ColumnCell.Size;
        ReportDimensions padding = new ReportDimensions();
        if (columns == null) return "";

        string strTableRow = "";
        strTableRow += @"<TablixRow> 
                        <Height>0.6cm</Height> 
                <TablixCells>";
        for (int i = 0; i < columns.Length; i++)
        {
                count += 3;
                ColumnCell = columns[i].ColumnCell;
                padding = ColumnCell.Padding;
                strTableRow += @"<TablixCell> 
                              <CellContents> 
                               " + GenerateWhiteTextBox("txtCell_" + table.ReportName + "_" + count + "_", ColumnCell.Name, "", true, padding) + @" 
                              </CellContents> 
                            </TablixCell>";            
        }
        strTableRow += @"</TablixCells></TablixRow>";
        return strTableRow;
    }
    
    static string GenerateTableHeaderRow(ReportBuilder reportBuilder, ReportTable table)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();
        ReportDimensions padding = new ReportDimensions();
        if (columns == null) return "";

        string strTableRow = "";
        strTableRow = @"<TablixRow>
                        <Height>0.6cm</Height> 
                            <TablixCells>";

        for (int i=0; i < columns.Length; i++)
        {
            ColumnCell = columns[i].ColumnCell;
            padding = columns[i].HeaderColumnPadding;
            strTableRow += @"<TablixCell> 
                  <CellContents> 
                   " + GenerateHeaderTableTextBox("txtHeader_" + table.ReportName + "_", ColumnCell.Name, columns[i].HeaderText == null || columns[i].HeaderText.Trim() == "" ? ColumnCell.Name : columns[i].HeaderText, false, padding) + @"               
                  </CellContents> 
                </TablixCell>";
        }
        strTableRow += @"</TablixCells></TablixRow>";

        return strTableRow;
    }

    static string GetTableColumns(ReportBuilder reportBuilder, ReportTable table)
    {

        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();

        if (columns == null) return "";

        string strColumnHeirarchy = "";
        strColumnHeirarchy = @" 
            <TablixColumns>";
        for (int i = 0; i < columns.Length; i++)
        {
            ColumnCell = columns[i].ColumnCell;

            strColumnHeirarchy += @" <TablixColumn> 
                                          <Width>" + ColumnCell.Size.Width.ToString() + @"cm</Width>  
                                        </TablixColumn>";
        }
        strColumnHeirarchy += @"</TablixColumns>";
        return strColumnHeirarchy;
    }
    static string GetTableColumnHeirarchy(ReportBuilder reportBuilder, ReportTable table)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        if (columns == null) return "";

        string strColumnHeirarchy = "";
        strColumnHeirarchy = @" 
            <TablixColumnHierarchy> 
                <TablixMembers>";
                    for (int i = 0; i < columns.Length; i++)
                    {
                        strColumnHeirarchy += "<TablixMember />";
                    }
                 strColumnHeirarchy += @"</TablixMembers> 
            </TablixColumnHierarchy>";
        return strColumnHeirarchy;
    }
    #endregion

    #region Report TextBox

    static string GenerateFirstRowWhiteTextBox(string filter, string strControlIDPrefix, string strName, string strValueOrExpression = "", bool isFieldValue = true, ReportDimensions padding = null, string tipoleta = "Calibri", string tamanioletra = "8pt")
    {
        string strTextBox = "";
        strTextBox += @"<Textbox Name=""" + filter+ @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs>  
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>
                             <Value>=""Agrupado por "+filter+@": ""  +Fields!"+filter+ @".Value</Value>";
  
                  strTextBox += @"<Style>
                                   <FontFamily>" + tipoleta + @"</FontFamily>
                                   <FontSize>" + tamanioletra + @"</FontSize>
                                 </Style> 
                            </TextRun> 
                          </TextRuns> 
                          <Style /> 
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + filter + @"</rd:DefaultName> 
                      <Style> 
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetFirstRowDimensions(padding,"LightBlue") + @"</Style> 
                    </Textbox>
                    <ColSpan>"+5+@"</ColSpan> 
                    <rd:Selected>true</rd:Selected>";
        return strTextBox;
    }

    static string GenerateWhiteTextBox(string strControlIDPrefix, string strName, string strValueOrExpression = "", bool isFieldValue = true, ReportDimensions padding = null, string tipoleta = "Calibri", string tamanioletra = "8pt")
    {
        string strTextBox = "";
        strTextBox += @"<Textbox Name="""+ strControlIDPrefix+strName+@"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs>  
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>
                             <Value/>";
                 strTextBox += @"<Style>
                                   <FontFamily>" + tipoleta + @"</FontFamily>
                                   <FontSize>" + tamanioletra + @"</FontSize>
                                 </Style> 
                            </TextRun> 
                          </TextRuns> 
                          <Style /> 
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + strName + @"</rd:DefaultName> 
                      <Style> 
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetDimensions(padding) + @"</Style> 
                    </Textbox>";
        return strTextBox;
    }
    static string GenerateTextBox(string strControlIDPrefix, string strName, string strValueOrExpression = "", bool isFieldValue = true, ReportDimensions padding = null, string tipoleta = "Calibri", string tamanioletra = "8pt")
    {
        string strTextBox = "";
        strTextBox = @" <Textbox Name=""" + strControlIDPrefix + strName + @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>";
        if (isFieldValue) strTextBox += @"<Value>=Fields!" + strName + @".Value</Value>";
        else strTextBox += @"<Value>" + strValueOrExpression + "</Value>";
        strTextBox += @"<Style>
                              <FontFamily>" + tipoleta + @"</FontFamily>
                           <FontSize>" + tamanioletra + @"</FontSize>
                         </Style> 
                            </TextRun> 
                          </TextRuns> 
                          <Style /> 
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + strName + @"</rd:DefaultName> 
                      <Style> 
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetDimensions(padding) + @"</Style> 
                    </Textbox>";
        return strTextBox;
    }
    static string GenerateHeaderTableTextBox(string strControlIDPrefix, string strName, string strValueOrExpression = "", bool isFieldValue = true, ReportDimensions padding = null,string tipoleta="Calibri",string tamanioletra ="8pt")
    {
        string strTextBox = "";
        strTextBox = @" <Textbox Name=""" + strControlIDPrefix + strName + @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>";
        if (isFieldValue) strTextBox += @"<Value>=Fields!" + strName + @".Value</Value>";
        else strTextBox += @"<Value>" + strValueOrExpression + "</Value>";
        strTextBox += @"<Style>
                           <FontFamily>"+tipoleta +@"</FontFamily>
                           <FontSize>"+ tamanioletra + @"</FontSize>
                         </Style>  
                            </TextRun> 
                          </TextRuns> 
                           <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + strName + @"</rd:DefaultName> 
                      <Style> 
                       <BackgroundColor>#DDDDDD</BackgroundColor>
                    <FontWeight>Bold</FontWeight>
                                            <Border> 
                                              <Color>LightGrey</Color> 
                                              <Style>Solid</Style> 
                                            </Border>" + GetDimensions(padding) + @"</Style> 
                                        </Textbox>";
        return strTextBox;
    }

    static string GetHeaderTextBox(string textBoxName, string left = "1cm", ReportDimensions padding = null, params string[] strValues)
    {
        string manao = "17.5cm";
        string strTextBox = "";
     //   string left = "1cm";
        strTextBox = @" <Textbox Name=""" + textBoxName + @"""> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns>";
        string widthxml = "7.93812cm";
        for (int i = 0; i < strValues.Length; i++)
        {           
            strTextBox += GetHeaderTextRun(strValues[i].ToString());          
        }
        //if (poss == 2)
        //{
        //    widthxml = TamanoTitulo.ToString() + "cm";
        //    left = "3cm";
        //}
        //else
        //{
        //    left = "1cm";
        //    widthxml = "7.93812cm";
        //}

        strTextBox += @"</TextRuns> 
              <Style /> 
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>" + textBoxName + @"</rd:DefaultName> 
          <Top>0.5cm</Top> 
          <Left>"+left+@"</Left> 
          <Height>0.6cm</Height> 
          <Width>"+ widthxml + @"</Width> 
          <ZIndex>2</ZIndex> 
          <Style> 
            <Border> 
              <Style>None</Style> 
            </Border>";

        strTextBox += GetDimensions(padding) + @"</Style> 
        </Textbox>";
        return strTextBox;
    }
    static string GetFooterTextBox(string textBoxName, ReportDimensions padding = null, params string[] strValues)
    {        

        string strTextBox = "";
        strTextBox = @" <Textbox Name=""" + textBoxName + @"""> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns>";

        for (int i = 0; i < strValues.Length; i++)
        {
            strTextBox += GetTextRun_fot(strValues[i].ToString());
        }

        strTextBox += @"</TextRuns> 
              <Style /> 
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>" + textBoxName + @"</rd:DefaultName> 
          <Top>1.83174cm</Top> 
          <Left>1cm</Left> 
          <Height>0.6cm</Height> 
          <Width>20.9cm</Width> 
          <ZIndex>2</ZIndex> 
          <Style> 
            <Border> 
              <Style>None</Style> 
            </Border>";

        strTextBox += GetDimensions(padding) + @"</Style> 
        </Textbox>";
        return strTextBox;
    }

    static string GetTextRun_fot(string ValueOrExpression)
    {
        //<Value>=""Page "" &amp; Globals!PageNumber &amp; "" of "" &amp; Globals!TotalPages</Value> 
        return "<TextRun>"
                  + "<Value>=&quot;" + ValueOrExpression + "</Value>" 
                  +"<Style>" 
                    +"<FontSize>8pt</FontSize>" 
                  +"</Style>" 
                +"</TextRun>";
    }


    static string GetTextRun(string ValueOrExpression)
    {
        return @"<TextRun> 
                  <Value>" + ValueOrExpression + @"</Value> 
                  <Style> 
                    <FontSize>8pt</FontSize> 
                  </Style> 
                </TextRun>";
    }

    static string GetHeaderTextRun(string ValueOrExpression)
    {
      
        return @"<TextRun> 
                  <Value>" + ValueOrExpression + @"</Value> 
                  <Style> 
                    <FontSize>10pt</FontSize> 
                    <FontWeight>Bold</FontWeight>                  
                  </Style> 
                </TextRun>";
    }

    #endregion

    #region Images
    static void GenerateReportImage(ReportBuilder reportBuilder)
    {



    }
    #endregion

    #region Settings

    private static string GetFirstRowDimensions(ReportDimensions padding = null,string backgroundColor = null)
    {
        string strDimensions = "";
        if (padding != null || backgroundColor != null)
        {
            if (padding.Default == 0)
            {
                strDimensions += string.Format("<BackgroundColor>"+ backgroundColor + @"</BackgroundColor>");
                strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Left);
                strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Right);
                strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Top);
                strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Bottom);
            }
            else
            { 
                strDimensions += string.Format("<BackgroundColor>"+ backgroundColor +@"</BackgroundColor>");
                strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Default);
                strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Default);
                strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Default);
                strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Default);
            }
        }
        return strDimensions;
    }
    private static string GetDimensions(ReportDimensions padding = null)
    {
        string strDimensions = "";
        if (padding != null)
        {
            if (padding.Default == 0)
            {
                strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Left);
                strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Right);
                strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Top);
                strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Bottom);
            }
            else
            {
                strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Default);
                strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Default);
                strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Default);
                strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Default);
            }
        }
        return strDimensions;
    }
    #endregion

}