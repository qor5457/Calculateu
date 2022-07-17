using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using CalibrationToolLibrary.Model;
using Microsoft.Win32;
using Syncfusion.XlsIO;

namespace CalibrationToolLibrary.Equation
{
    public class Excel : EquationFactory
    {
        /// <summary>
        /// 엑셀 데이터 값 저장용 데이터
        /// </summary>
        /// <param name="Vitamin">비타민</param>
        /// <param name="T_NC">값</param>+-
        private record ExcelData(string Vitamin, double T_NC);
        /// <summary>
        /// 엑셀 데이터 저장용 리스트
        /// </summary>
        private List<ExcelData> ExcelDatas;
        /// <summary>
        /// 엑셀 값 결과 저장
        /// </summary>
        private DataTable Calculation_result_table;
        /// <summary>
        /// 원본 파일 경로
        /// </summary>
        public string Filepath = String.Empty;

        /// <summary>
        /// 방정식 저장
        /// </summary>
        /// <param name="filepath">저장 파일 경로</param>
        /// <param name="Em">방정식 리스트</param>
        public void EquationsImportSave(string filepath, List<Equation_Model> Em)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                
                worksheet.Range["A1"].Text = "Index";
                worksheet.Range["B1"].Text = "Type";
                worksheet.Range["C1"].Text = "A";
                worksheet.Range["D1"].Text = "B";
                worksheet.Range["E1"].Text = "C";
                worksheet.Range["F1"].Text = "D";
                worksheet.Range["G1"].Text = "E";

                int row = 2;
                for (int i = 0; i < Em.Count; i++)
                {
                    int col = 1;
                    worksheet.Range[row,col++].Text = i.ToString();
                    worksheet.Range[row,col++].Text = Em[i].Eqiation_Num;
                    foreach (var value in Em[i].Value)
                    {
                        worksheet.Range[row,col++].Text = value;
                    }
                    row++;
                }
                
                IStyle headerStyle = workbook.Styles.Add("HeaderStyle");
                headerStyle.BeginUpdate();
                headerStyle.Color = Color.FromArgb(217, 217, 217);
                headerStyle.Font.Bold = true;
                headerStyle.EndUpdate();
                
                
                worksheet.Rows[0].CellStyle = headerStyle;
                
                worksheet.Range[1,1,Em.Count+1,7].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                worksheet.Range[1,1,Em.Count+1,7].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                worksheet.Range[1,1,Em.Count+1,7].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                worksheet.Range[1,1,Em.Count+1,7].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                
                worksheet.Range[1,1,Em.Count+1,7].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                worksheet.UsedRange.AutofitColumns();
                
                workbook.SaveAs(filepath);
            }
        }
        
        /// <summary>
        /// 엑셀 열기
        /// </summary>
        /// <param name="filepath">원본 파일 경로</param>
        /// <returns></returns>
        public bool OpenFile(string filepath)
        {
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;
                    IWorkbook workbook = application.Workbooks.Open(filepath);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    ExcelDatas = new List<ExcelData>();
                
                    for (int i = 1; i < worksheet.Columns[0].Cells.Length; i++)
                    {
                        var EexcelData = new ExcelData
                        (
                            worksheet.Columns[0].Cells[i].Value,
                            double.Parse(worksheet.Columns[1].Cells[i].Value)
                        );
                        ExcelDatas.Add(EexcelData);
                    }
                }

                Filepath = filepath;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// 엑셀 저장
        /// </summary>
        /// <param name="filepath">저장할 경로</param>
        /// <param name="Em">방정식 리스트</param>
        public override void Savefile(string filepath,  List<Equation_Model> Em)
        {
            Calculation_result_table = new DataTable();
            var OrderBy_equations = new List<Equation_Model>(Em.OrderBy(x => x.Eqiation_Num));
            foreach (var equationModel in OrderBy_equations)
            {
                Calculation_result_table.Columns.Add(equationModel.Formula);
            }

            CalculationSet(OrderBy_equations);

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(Filepath);
                workbook.Worksheets.AddCopy(workbook.Worksheets[0]);
                workbook.Worksheets[1].Name = "OutputData";
                workbook.Worksheets[1].InsertRow(1, 1, ExcelInsertOptions.FormatAsBefore);

                int cell = 2;
                var GroupBy_equations = OrderBy_equations.GroupBy(x => x.Eqiation_Num);
                foreach (var GroupBy_equation in GroupBy_equations)
                {
                    int sum = GroupBy_equation.Count();
                    workbook.Worksheets[1].SetValue(1, cell + 1, GroupBy_equation.Key);
                    workbook.Worksheets[1].Range[1, cell + 1, 1, cell += sum].Merge();
                }

                DataView view = Calculation_result_table.DefaultView;
                workbook.Worksheets[1].ImportDataView(view, true, 2, 3);
                
                workbook.Worksheets[1].Range[1,1,Calculation_result_table.Rows.Count+2,Calculation_result_table.Columns.Count+2].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                workbook.Worksheets[1].Range[1,1,Calculation_result_table.Rows.Count+2,Calculation_result_table.Columns.Count+2].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                workbook.Worksheets[1].Range[1,1,Calculation_result_table.Rows.Count+2,Calculation_result_table.Columns.Count+2].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                workbook.Worksheets[1].Range[1,1,Calculation_result_table.Rows.Count+2,Calculation_result_table.Columns.Count+2].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                
                workbook.Worksheets[1].Range[1,1,Calculation_result_table.Rows.Count+2,Calculation_result_table.Columns.Count+2].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                workbook.Worksheets[1].UsedRange.AutofitColumns();
                workbook.SaveAs(filepath);
            }
        }
        /// <summary>
        /// 방정식 오픈
        /// </summary>
        /// <param name="filepath"></param>
        public List<Equation_Model> EquationsImportOpen(string filepath)
        {
            var equationModel = new List<Equation_Model>();
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Open(filepath);
                IWorksheet worksheet = workbook.Worksheets[0];

                for (int i = 1; i < worksheet.Columns[0].Cells.Length; i++)
                {
                    var list = new List<string>();
                    for (int j = 2; j <= 6; j++)
                    {
                        if (worksheet.Columns[j].Cells[i].Value == String.Empty)
                            break;
                        list.Add(worksheet.Columns[j].Cells[i].Value);
                    }

                    var eq = new Equation_Model()
                    {
                        Eqiation_Num = $"{list.Count-1}차 방정식",
                        Value = list.ToArray(),
                    };

                    eq.Formula = eq.ValueTostring();
                    equationModel.Add(eq);
                }
            }
            return equationModel;
        }

        /// <summary>
        /// 방정식 데이터 값
        /// </summary>
        /// <param name="equationModels"></param>
        void CalculationSet(List<Equation_Model> equationModels)
        {
            for (int i = 0; i < ExcelDatas.Count; i++)
            {
                int index = 0;
                DataRow data = Calculation_result_table.NewRow();
                foreach (var equationModel in equationModels)
                {
                    double[] value = Array.ConvertAll( equationModel.Value, s => double.Parse(s));
                    var calculation = FirstorderfunctionTodouble(ExcelDatas[i].T_NC, value);
                    data[index++] = calculation;
                }
                Calculation_result_table.Rows.Add(data);
            }
        }
        
        double FirstorderfunctionTodouble(double x, double[] value)
        {
            var Len_value = value.Length;
            var Calculation = 0.0;
            for (int i = 0; i < value.Length; i++)
            {
                Calculation += (Math.Pow(x, --Len_value)) * value[i];
            }
            return Calculation;
        }
    }
}