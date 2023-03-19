using GrossNetInC_.Constants;
using GrossNetInC_.ViewModels;
using System.Diagnostics;

namespace GrossNetInC_.Services
{
    public class GrossToNetService : IGrossToNetService
    {
        public void RegulationCheck(out double DefaultIncome, out double SelfAllowances, out double FamilyAllowances, out double LowestIncomeByRegion, GrossNetVM model)
        {
            if (model.Regulation == "new")
            {
                DefaultIncome = 1490000;
                SelfAllowances = 11000000;
                FamilyAllowances = 4400000;
                LowestIncomeByRegion = 0;
                switch (model.Region)
                {
                    case 1:
                        LowestIncomeByRegion = 4680000;
                        break;
                    case 2:
                        LowestIncomeByRegion = 4160000;
                        break;
                    case 3:
                        LowestIncomeByRegion = 3640000;
                        break;
                    case 4:
                        LowestIncomeByRegion = 3250000;
                        break;
                }

            }
            else
            {
                DefaultIncome = 1490000;
                SelfAllowances = 11000000;
                FamilyAllowances = 4400000;
                LowestIncomeByRegion = 0;
                switch (model.Region)
                {
                    case 1:
                        LowestIncomeByRegion = 4420000;
                        break;
                    case 2:
                        LowestIncomeByRegion = 3920000;
                        break;
                    case 3:
                        LowestIncomeByRegion = 3430000;
                        break;
                    case 4:
                        LowestIncomeByRegion = 3070000;
                        break;
                }
            }
        }

        public List<double[]> GetPersonalIncomeTaxesRange()
        {
            var ListRange = new List<double[]>
            {
                new double[] { 5000000, 0.05 },
                new double[] { 5000000, 0.1 },
                new double[] { 8000000, 0.15 },
                new double[] {14000000,0.2},
                new double[] {20000000,0.25},
                new double[] {28000000,0.3},
                new double[] {double.MaxValue,0.35}
            };
            return ListRange;
        }


        public double GetIncomeSufferInsuranceTax(GrossNetVM model)
        {
            if (model.IsCustomInsuranceType)
            {
                return model.InsuranceTax;
            }
            else
            {
                return model.Income;
            }
        }

        public GrossInsuranceVM CalculateIncomeAfterInsuranceTax(double income, double sufferTaxes)
        {
            var return_model = new GrossInsuranceVM();
            double SocialTax;
            double PublicHealthTax;
            double UnemploymentTax;
            double incomeAfterTaxes;
            if (sufferTaxes <= 29800000)
            {
                SocialTax = sufferTaxes * InsuranceContants.CommunityInsurance;
                PublicHealthTax = sufferTaxes * InsuranceContants.PublicHealthInsurance;
                UnemploymentTax = sufferTaxes * InsuranceContants.UnemplymentInsurance;
                incomeAfterTaxes = income - SocialTax - PublicHealthTax - UnemploymentTax;
            }
            else if (sufferTaxes <= 93600000)
            {
                SocialTax = InsuranceContants.MaxSocialInsurance;
                PublicHealthTax = InsuranceContants.MaxPublicHealthInsurance;
                UnemploymentTax = sufferTaxes * InsuranceContants.UnemplymentInsurance;
                incomeAfterTaxes = income - SocialTax - PublicHealthTax - UnemploymentTax;
            }
            else
            {
                SocialTax = InsuranceContants.MaxSocialInsurance;
                PublicHealthTax = InsuranceContants.MaxPublicHealthInsurance;
                UnemploymentTax = InsuranceContants.MaxUnemplymentInsurance;
                incomeAfterTaxes = income - SocialTax - PublicHealthTax - UnemploymentTax;
            }
            return_model.IncomeAfterTaxes = incomeAfterTaxes;
            return_model.PublicHealthInsurance = PublicHealthTax;
            return_model.SocialInsurance = SocialTax;
            return_model.UnemploymentInsurance = UnemploymentTax;
            return return_model;
        }

        public CalculateIncomeAfterPersonalTaxes CalculateIncomeAfterPesonalTaxes(double income, double incomeSufferTaxes)
        {
            var listRanges = GetPersonalIncomeTaxesRange();
            var tmp_income = incomeSufferTaxes;
            var list_tax = new List<double>();
            foreach (var range in listRanges)
            {
                var real_tmp = tmp_income;
                tmp_income -= range[0];
                if (tmp_income <= 0)
                {
                    var tax = real_tmp * range[1];
                    list_tax.Add(tax);
                    break;
                }
                else
                {
                    var tax = range[0] * range[1];
                    list_tax.Add(tax);
                }
            }
            foreach (var tax in list_tax)
            {
                income -= tax;
            }
            var returned_model = new CalculateIncomeAfterPersonalTaxes()
            {
                IncomeAfterTaxes = income,
                PersonalTaxes = list_tax
            };
            return returned_model;

        }


        public IncomeBeforePersonalTaxes CalculateIncomeBeforeTaxes(double income, double self, double family)
        {
            var range = PersonalTaxesConstants.PersonalTaxesRange;
            if(income <= self + family)
            {
                return new IncomeBeforePersonalTaxes()
                {
                    Range = null,
                    IncomeBefore = income
                };
            }
            foreach (var item in range)
            {
                var percent = item[2];
                var before_income = (income - percent * self - percent * family - item[3]) / (1 - percent);
                var sufferTax = before_income - family - self;
                if (sufferTax >= item[0] && sufferTax <= item[1])
                {
                    var tmp = CalculateIncomeAfterPesonalTaxes(before_income, before_income - self - family);
                    var returned_model = new IncomeBeforePersonalTaxes()
                    {
                        IncomeBefore = before_income,
                        Range = tmp.PersonalTaxes
                    };
                    return returned_model;
                }
            }
            return new IncomeBeforePersonalTaxes()
            {
                Range = null,
                IncomeBefore = 0
            };
        }

        public IncomeBeforeInsuranceTaxes CalculateIncomeBeforeInsuranceTaxes(double income, GrossNetVM model)
        {
            if(model.IsCustomInsuranceType)
            {
                var sufferTax = model.InsuranceTax;
                double social = sufferTax * InsuranceContants.CommunityInsurance;
                double publichealth = sufferTax * InsuranceContants.PublicHealthInsurance;
                double unemployment = sufferTax * InsuranceContants.UnemplymentInsurance;
                if(social >= InsuranceContants.MaxSocialInsurance )
                {
                    social = InsuranceContants.MaxSocialInsurance;
                }
                if(publichealth >= InsuranceContants.MaxPublicHealthInsurance)

                {
                    publichealth = InsuranceContants.MaxPublicHealthInsurance;
                }
                if(unemployment >= InsuranceContants.MaxUnemplymentInsurance)
                {
                    unemployment = InsuranceContants.MaxUnemplymentInsurance;
                }
                var before_income = income + social + unemployment + publichealth;
                var return_model = new IncomeBeforeInsuranceTaxes()
                {
                    Social = social,
                    PublicHealth = publichealth,
                    Unemployment = unemployment,
                    IncomeBefore = before_income
                };
                return return_model;
            }
            else
            {
                double before_income;
                double social;
                double publichealth;
                double unemployment;
                if(income <= InsuranceContants.MaxAfterInsurance1)
                {
                    before_income = income / 0.895;
                    social = before_income * InsuranceContants.CommunityInsurance;
                    publichealth = before_income * InsuranceContants.PublicHealthInsurance;
                    unemployment = before_income * InsuranceContants.UnemplymentInsurance;
                }
                else if(income <= InsuranceContants.MaxAfterInsurance2) {
                    social = InsuranceContants.MaxSocialInsurance;
                    publichealth = InsuranceContants.MaxPublicHealthInsurance;
                    before_income = (income + social + publichealth) / (1 - InsuranceContants.UnemplymentInsurance);
                    unemployment = before_income * InsuranceContants.UnemplymentInsurance;
                }
                else
                {
                    social = InsuranceContants.MaxSocialInsurance;
                    publichealth = InsuranceContants.MaxPublicHealthInsurance;
                    unemployment = InsuranceContants.MaxUnemplymentInsurance;
                    before_income = income + social + publichealth + unemployment;
                }
                var returned_model = new IncomeBeforeInsuranceTaxes()
                {
                    Social = social,
                    PublicHealth = publichealth,
                    Unemployment = unemployment,
                    IncomeBefore = before_income
                };
                return returned_model;
            }

        }
    }
}