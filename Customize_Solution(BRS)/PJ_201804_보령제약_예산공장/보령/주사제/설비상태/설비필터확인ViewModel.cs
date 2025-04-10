using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Linq;

namespace 보령
{
    public class 설비필터확인ViewModel : ViewModelBase
    {
        #region [Property]
        public 설비필터확인ViewModel()
        {
            _BR_BRS_GET_FILTERINFO = new BR_BRS_GET_FILTERINFO();
            _FilterList = new BR_BRS_GET_FILTERINFO.OUTDATACollection();
        }

        private 설비필터확인 _mainWnd;
        private string _FilterId;
        public string FilterId
        {
            get { return _FilterId; }
            set
            {
                _FilterId = value;
                OnPropertyChanged("FilterId");
            }
        }
        private BR_BRS_GET_FILTERINFO.OUTDATACollection _FilterList;
        public BR_BRS_GET_FILTERINFO.OUTDATACollection FilterList
        {
            get { return _FilterList; }
            set
            {
                _FilterList = value;
                OnPropertyChanged("FilterList");
            }
        }

        #endregion

        #region [BizRule]
        private BR_BRS_GET_FILTERINFO _BR_BRS_GET_FILTERINFO;
        public BR_BRS_GET_FILTERINFO BR_BRS_GET_FILTERINFO
        {
            get { return _BR_BRS_GET_FILTERINFO; }
            set
            {
                _BR_BRS_GET_FILTERINFO = value;
                OnPropertyChanged("BR_BRS_GET_FILTERINFO");
            }
        }

        #endregion

        #region [Command]
        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = false;
                            CommandResults["LoadedCommandAsync"] = false;

                            ///
                            if (arg != null && arg is 설비필터확인)
                            {
                                _mainWnd = arg as 설비필터확인;

                                IsBusy = true;

                                _FilterList.Clear();

                                // 조회 파라미터 세팅
                                //var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                //if(inputValues.Count > 0)
                                //{
                                //    foreach (var item in inputValues)
                                //    {
                                //        if(!string.IsNullOrWhiteSpace(item.Raw.EQPTID))
                                //        {

                                //        }
                                //    }
                                //}

                                //if(!string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.EQPTID))
                                //{

                                //}


                            }

                            IsBusy = false;
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        public ICommand SelfilterCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SelfilterCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SelfilterCommand"] = false;
                            CommandCanExecutes["SelfilterCommand"] = false;

                            ///
                            if (arg is string && !string.IsNullOrWhiteSpace(arg as string))
                            {
                                FilterId = (arg as string).ToUpper().Trim();

                                _BR_BRS_GET_FILTERINFO.INDATAs.Clear();
                                _BR_BRS_GET_FILTERINFO.OUTDATAs.Clear();
                                _BR_BRS_GET_FILTERINFO.INDATAs.Add(new BR_BRS_GET_FILTERINFO.INDATA
                                {
                                    EQPTID = FilterId
                                });

                                // 필터 상태 조회
                                if (await _BR_BRS_GET_FILTERINFO.Execute() == true && _BR_BRS_GET_FILTERINFO.OUTDATAs.Count > 0)
                                {
                                    FilterList.Add(_BR_BRS_GET_FILTERINFO.OUTDATAs[0]);
                                    FilterId = "";
                                }   
                            }

                            IsBusy = false;
                            ///

                            CommandResults["SelfilterCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["SelfilterCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SelfilterCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SelfilterCommand") ?
                        CommandCanExecutes["SelfilterCommand"] : (CommandCanExecutes["SelfilterCommand"] = true);
                });
            }
        }
        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ComfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = false;
                            CommandResults["ComfirmCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSDTTM.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("기록값을 변경합니다."),
                                    string.Format("기록값 변경"),
                                    true,
                                    "OM_ProductionOrder_SUI",
                                    "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "설비필터확인",
                                "설비필터확인",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("관리번호"));
                            dt.Columns.Add(new DataColumn("교체일자"));
                            dt.Columns.Add(new DataColumn("사용기한"));
                            dt.Columns.Add(new DataColumn("기록"));

                            foreach (var item in _FilterList)
                            {
                                var row = dt.NewRow();

                                row["자재명"] = item.EQPTNAME ?? "";
                                row["관리번호"] = item.EQPTID ?? "";
                                row["교체일자"] = item.CHANGEDTTM ?? "";
                                row["사용기한"] = item.EXPIREDTTM ?? "";
                                row["기록"] = item.STATUS ?? "";

                                dt.Rows.Add(row);
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);


                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            ///

                            CommandResults["ComfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ComfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        #endregion       
        #region [User Define]

        #endregion
    }
}
