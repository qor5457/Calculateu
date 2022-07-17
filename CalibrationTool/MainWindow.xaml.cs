using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CalibrationTool.View_ViewModel;
using CalibrationToolLibrary;
using CalibrationToolLibrary.Equation;
using CalibrationToolLibrary.Model;
using Microsoft.Win32;

namespace CalibrationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ICommand
        public ICommand BtnExcelOpen { get; private set; }
        public ICommand BtnEquationListCall { get; private set; }
        public ICommand BtnEquationListSave { get; private set; }
        public ICommand BtnEquationListAdd { get; private set; }
        public ICommand BtnListDelAll { get; private set; }
        public ICommand BtnSaveAnalysis { get; private set; }

        #endregion

        #region 생성자
        public MainWindow() 
        {
            InitializeComponent();
        }
        /// <summary>
        /// 이벤트 할당 및 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = this;
            
            BtnExcelOpen = new RelayCommand<object>(ExcelOpen);
            BtnEquationListCall = new RelayCommand<object>(EquationListCall);
            BtnEquationListSave = new RelayCommand<object>(EquationListSave);
            BtnEquationListAdd = new RelayCommand<object>(EquationListAdd);
            BtnListDelAll = new RelayCommand<object>(ListDelAll);
            BtnSaveAnalysis = new RelayCommand<object>(SaveAnalysis);
            
        }
        #endregion

        #region Feild
        public ObservableCollection<Equation_Model> EquationList { get; set; } = new();
        private Excel _excel = new Excel();
        private Json_Save _jsonSave = new Json_Save();
        #endregion
        
        #region Event
        /// <summary>
        /// Excel Open
        /// </summary>
        /// <param name="obj"></param>
        private void ExcelOpen(object obj)
        {
            var openfile = new OpenFileDialog();
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            openfile.InitialDirectory = "C:\\";
            if (openfile.ShowDialog() == true)
            {
                TXT_File_Path.Text = openfile.FileName;
                _excel.OpenFile(openfile.FileName);
            }
        }
        /// <summary>
        /// 방정식 목록 추가
        /// </summary>
        /// <param name="obj"></param>
        private void EquationListCall(object obj)
        {
            var openfile = new OpenFileDialog();
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            openfile.InitialDirectory = "C:\\";
            if (openfile.ShowDialog() == true)
            {
                var list = _excel.EquationsImportOpen(openfile.FileName);
                EquationList.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    EquationList.Add(list[i]);
                }
                MessageBox.Show("완료");
            }
        }
        /// <summary>
        /// 방정식 목록 저장
        /// </summary>
        /// <param name="obj"></param>
        private void EquationListSave(object obj)
        {
            var saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            saveFile.InitialDirectory = "C:\\";
            if (saveFile.ShowDialog() == true)
            {
                _excel.EquationsImportSave(saveFile.FileName,EquationList.ToList());
                MessageBox.Show("완료");
            }
        }
        /// <summary>
        /// 방정식 추가
        /// </summary>
        /// <param name="obj"></param>
        private void EquationListAdd(object obj)
        {
            var dialogEquationAdd = new DialogEquationAdd()
            {
                Owner = this
            };
            dialogEquationAdd.ShowDialog();
            if (dialogEquationAdd.DialogResult.HasValue && dialogEquationAdd.DialogResult.Value)
            {
                var equationModel = new Equation_Model()
                {
                    Eqiation_Num = dialogEquationAdd.EqiationNum,
                    Value = dialogEquationAdd.Value,
                    Formula = dialogEquationAdd._em.ValueTostring()
                };
                var item = EquationList.ToList().Find(x => x.Formula.Equals(equationModel.Formula));
                if (item == null)
                    EquationList.Add(equationModel);
                else
                    MessageBox.Show("값이 중복 되었습니다.");
            }
        }
        /// <summary>
        /// 방정식 리스트 삭제
        /// </summary>
        /// <param name="obj"></param>
        private void ListDelAll(object obj)
        {
            EquationList.Clear();
        }
        /// <summary>
        /// 엑셀 저장
        /// </summary>
        /// <param name="obj"></param>
        private void SaveAnalysis(object obj)
        {
            if (_excel.Filepath == String.Empty)
            {
                MessageBox.Show("Failed to load Excel.");
                return;
            }
            BtnAnalysis.Content = "저장중";
            
            var saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            saveFile.InitialDirectory = "C:\\";
            if (saveFile.ShowDialog() == true)
            {
                _excel.Savefile(saveFile.FileName, EquationList.ToList());
                MessageBox.Show("완료");
            }
            BtnAnalysis.Content = "분석 및 저장하기";
        }
        /// <summary>
        /// 리스트 한줄 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Data_Row_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            if (button.DataContext is Equation_Model model)
            {
                EquationList.Remove(model);
            }
        }
        #endregion
    }
}