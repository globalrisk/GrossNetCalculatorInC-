namespace GrossNetInC_.Constants
{
    public class PersonalTaxesConstants
    {
        public static List<double[]> PersonalTaxesRange = new List<double[]>()
        {
            new double[] {0,5000000,0.05,0},
            new double[] {5000000,10000000,0.1,250000},
            new double[] {10000000,18000000,0.15,750000},
            new double[] {18000000,32000000,0.2,1650000},
            new double[] {32000000,52000000,0.25,3250000},
            new double[] {52000000,80000000,0.3,5850000},
            new double[] {80000000,double.MaxValue,0.35,9850000}

        };
    }
}
