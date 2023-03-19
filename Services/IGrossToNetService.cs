using GrossNetInC_.ViewModels;

namespace GrossNetInC_.Services
{
    public interface IGrossToNetService
    {
        public void RegulationCheck(out double DefaultIncome, out double SelfAllowances, out double FamilyAllowances, out double LowestIncomeByRegion, GrossNetVM model);
        public List<double[]> GetPersonalIncomeTaxesRange();
        public double GetIncomeSufferInsuranceTax(GrossNetVM model);
        public GrossInsuranceVM CalculateIncomeAfterInsuranceTax(double income, double sufferTaxes);
        public CalculateIncomeAfterPersonalTaxes CalculateIncomeAfterPesonalTaxes(double income, double incomeSufferTaxes);
        public IncomeBeforePersonalTaxes CalculateIncomeBeforeTaxes(double income, double self, double family);
        public IncomeBeforeInsuranceTaxes CalculateIncomeBeforeInsuranceTaxes(double income, GrossNetVM model);
    }
}
