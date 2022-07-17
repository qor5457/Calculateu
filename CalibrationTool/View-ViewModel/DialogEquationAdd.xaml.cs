using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CalibrationTool.Annotations;
using CalibrationToolLibrary;
using CalibrationToolLibrary.Model;

namespace CalibrationTool.View_ViewModel
{
    public partial class DialogEquationAdd : Window, INotifyPropertyChanged
    {
        #region Notify
        /// <summary>
        /// Equation_Model 모델
        /// </summary>
        public Equation_Model _em = new();
        public string EqiationNum
        {
            get { return _em.Eqiation_Num; }
            set 
            { 
                _em.Eqiation_Num = value;
                OnPropertyChanged("EqiationNum");
            }
        }
        public string[] Value
        {
            get { return _em.Value; }
            set
            {
                _em.Value = value;
                OnPropertyChanged("Value"); ;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region ICommend
        public ICommand BtnCancel { get; private set; }
        public ICommand BtnEquationAdd { get; private set; }
        public ICommand BtnRadioButtonClick { get; private set; }
        #endregion
        #region 생성자
        public DialogEquationAdd()
        {
            InitializeComponent();
            Rb4.IsChecked = true;
            var initRB = "5"; 
            RadioButtonClick(initRB);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = this;
            Value = new string[5];
            BtnCancel = new RelayCommand<object>(Cancel);
            BtnEquationAdd = new RelayCommand<object>(EquationAdd);
            BtnRadioButtonClick = new RelayCommand<object>(RadioButtonClick);
        }
        #endregion

        #region Event
        /// <summary>
        /// 라디오 버튼 클릭시 발생 이벤트
        /// </summary>
        /// <param name="obj"></param>
        private void RadioButtonClick(object obj)
        {
            var size = int.Parse((string)obj);
            EqiationNum = $"{size - 1}차 방정식";
            Value = new string[int.Parse((string)obj)];
        }
        /// <summary>
        /// 취소 버튼
        /// </summary>
        /// <param name="obj"></param>
        private void Cancel(object obj)
        {
            this.DialogResult = false;
            this.Close();
        }
        /// <summary>
        /// 추가 버튼
        /// </summary>
        /// <param name="obj"></param>
        private void EquationAdd(object obj)
        {
            if (InputDataCheack())
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("값을 입력해주세요.");
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 값 Null 체크
        /// </summary>
        /// <returns></returns>
        private bool InputDataCheack()
        {
            foreach (var value in Value)
            {
                if(string.IsNullOrEmpty(value))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
        
        
    }
}