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

namespace 보령.DataModel.ContainerModel
{
    public class StdSourceContainerStrategy : ISourceContainerStrategy
    {

        public decimal? CalculateSourceStrategy(decimal? bizRuleWeight, decimal? currentWeight, decimal? totalUsedWeight)
        {
            return bizRuleWeight - (currentWeight - totalUsedWeight);
        }
    }
}
