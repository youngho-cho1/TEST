using LGCNS.iPharmMES.Common;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using C1.Silverlight.Excel;
using System.Globalization;
using System.Collections.Generic;


namespace Board
{
    public class Mockup_classes
    {
        #region[Property]
        public class Mockup_OrderSummary_item : BizActorDataSetBase
        {
            public Boolean CHECKBOX { get; set; } // Checkbox default = N
            public DateTime PeriodSTDTTM { get; set; } // 생산계획시작일
            public DateTime PeriodEDDTTM { get; set; } //생산계획종료일
            public string EQPTID { get; set; }      //라인코드
            public string EQPTNAME { get; set; }   //라인명
            public string POID { get; set; }    //오더번호
            public string LOTID { get; set; }   //제조번호
            public string MTRLID { get; set; }   //제품코드
            public string MTRLNAME { get; set; }   //제품명
            public string ORDERTYPEID { get; set; }   //오더유형코드
            public string ORDERTYPENAME { get; set; }   //오더유형명
            public string POSTAT { get; set; }   //상태코드
            public string POSTATNAME { get; set; }   //상태명

        }


        public class Mockup_OperationCheck_item : BizActorDataSetBase
        {          
            public string POID { get; set; }    //오더번호
            public string LOTID { get; set; }   //제조번호
            public string MTRLID { get; set; }   //제품코드
            public string MTRLNAME { get; set; }   //제품명
            public string OPSGGUID { get; set; } //공정코드
            public string OPSGNAME { get; set; } //공정명
            public string ITEM { get; set; } //항목
            public string SPEC { get; set; } //기준
            public string RANGEVALUE { get; set; } //범위
            public string RESULT { get; set; } //값

        }


        public class Mockup_CriticalParameter_item : BizActorDataSetBase
        {
            public string POID { get; set; }    //오더번호
            public string LOTID { get; set; }   //제조번호
            public string MTRLID { get; set; }   //제품코드
            public string MTRLNAME { get; set; }   //제품명
            public string OPSGGUID { get; set; } //공정코드
            public string OPSGNAME { get; set; } //공정명
            public string EQCLID { get; set; } //설비코드
            public string EQCLNAME { get; set; } //설비명
            public string ITEM { get; set; } //항목
            public string SPEC { get; set; } //기준
            public string RANGEVALUE { get; set; } //범위
            public string RESULT { get; set; } //값

        }

        public class Mockup_LogbookReview_item : BizActorDataSetBase
        {
            public string EQCLID { get; set; } //설비코드
            public string EQCLNAME { get; set; } //설비명
            public string EQCLSTAT { get; set; } //상태
            public string LOGBOOKID { get; set; } //로그북ID
            public string LOGBOOKNAME { get; set; } //로그북명
            public string AUTHORITY { get; set; } //검토자격
            public int PERIOD { get; set; } //검토주기
            public string PERIODUOM { get; set; } //검토주기단위

        }

        public class Mockup_LineCBO_item : BizActorDataSetBase
        {
            public bool CHECKBOX { get; set; }
            public string LINENAME { get; set; } //라인명

        }

        #endregion


        public class Mockup_OrderSummary_items : BufferedObservableCollection<Mockup_OrderSummary_item> { }
        public class Mockup_OperationCheck_items : BufferedObservableCollection<Mockup_OperationCheck_item> { }
        public class Mockup_CriticalParameter_items : BufferedObservableCollection<Mockup_CriticalParameter_item> { }

        public class Mockup_LogbookReview_items : BufferedObservableCollection<Mockup_LogbookReview_item> { }
        public class Mockup_LineCBO_items : BufferedObservableCollection<Mockup_LineCBO_item> { }
    }
}
