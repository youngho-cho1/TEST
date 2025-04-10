using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LGCNS.iPharmMES.Common;
using C1.Silverlight.Data;

namespace 보령
{
    public class CampaignProduction
    {
        /// <summary>
        /// 오더목록을 조회하는 함수
        /// </summary>
        /// <param name="recipeistguid"></param>
        /// <param name="poid"></param>
        /// <returns></returns>
        public static async Task<BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection> GetProductionOrderList(string recipeistguid, string poid)
        {
            BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection orders = new BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection();

            var br = new BR_BRS_SEL_ProductionOrder_RECIPEISTGUID();
            br.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.INDATA { RECIPEISTGUID = recipeistguid, POID = poid });
            await br.Execute();

            return br.OUTDATAs;
        }
    }
    public class CampaignOrderXML
    {
        private string _PoId;
        public string PoId
        {
            get { return _PoId; }
            set { _PoId = value; }
        }
        private DataTable _XML;
        public DataTable XML
        {
            get { return _XML; }
            set { _XML = value; }
        }
    }
}
