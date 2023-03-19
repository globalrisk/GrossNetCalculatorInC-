using GrossNetInC_.Services;
using GrossNetInC_.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GrossNetInC_.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GrossNetController : Controller
    {
        private readonly IGrossToNetService service;

        public GrossNetController(IGrossToNetService service)
        {
            this.service = service;
        }
        [HttpPost]
        [Route("api/GrossToNet")]
        public IActionResult GrossToNet(GrossNetVM model)
        {
            var GrossIncome = model.Income;
            service.RegulationCheck(out double DefaultIncome, out double SelfAllowances, out double FamilyAllowances, out double LowestIncomeByRegion, model);
            var ListTaxRanges = service.GetPersonalIncomeTaxesRange();
            var IncomeSufferInsuranceTax = service.GetIncomeSufferInsuranceTax(model);
            var Calculated_list_after_insurance_taxes = service.CalculateIncomeAfterInsuranceTax(GrossIncome, IncomeSufferInsuranceTax);
            var GrossAfterInsuranceTaxes = Calculated_list_after_insurance_taxes.IncomeAfterTaxes;
            var SufferPersonalIncomeTaxes = GrossAfterInsuranceTaxes - SelfAllowances - FamilyAllowances * model.DependencePeople;
            if (SufferPersonalIncomeTaxes < 0)
            {
                SufferPersonalIncomeTaxes = 0;
            }
            var Calculated_list_after_personal_taxes = service.CalculateIncomeAfterPesonalTaxes(GrossAfterInsuranceTaxes, SufferPersonalIncomeTaxes);
            var returned_model = new ReturnedModelForGrossToNet()
            {
                modelInsurance = Calculated_list_after_insurance_taxes,
                modelPersonal = Calculated_list_after_personal_taxes
            };
            return Ok(returned_model);
        }

        [HttpPost]
        [Route("api/NetToGross")]
        public IActionResult NetToGross(GrossNetVM model)
        {
            var NetIncome = model.Income;
            service.RegulationCheck(out double DefaultIncome, out double SelfAllowances, out double FamilyAllowances, out double LowestIncomeByRegion, model);
            var returned_model = service.CalculateIncomeBeforeTaxes(NetIncome, SelfAllowances, model.DependencePeople * FamilyAllowances);
            var returned_model_2 = service.CalculateIncomeBeforeInsuranceTaxes(returned_model.IncomeBefore, model);
            var return_model_real = new ReturnedModelForNetToGross {
                model1 = returned_model_2,
                model2 = returned_model
            };
            return Ok(return_model_real);
        }
    }
}
