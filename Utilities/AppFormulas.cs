using oft_winform.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static oft_winform.DTO.DamageItemConfig;

namespace oft_winform.Utilities;

public static class AppFormulas
{
    static AgeCoefficientConfiguration ageConfg => AgeCoefficientConfiguration.Instance;
    static DamageCoefficientConfiguration damageConfig => DamageCoefficientConfiguration.Instance;

    static PriceDropConstraint priceDropConstraint => PriceDropConstraintConfiguration.Instance;

    static int? _currentYear;
    static int currentYear
    {
        get
        {
            if (_currentYear == null)
                _currentYear = new PersianCalendar()
                              .GetYear(DateTime.Now);
            return _currentYear.Value;
        }
    }
    public static float GetPriceDrop(Dictionary<string, DamageLevel> input, long carPrice, int manufacturedYear, long conventionalCarPrice)
    {
        bool changedRoom = input["room_change"] > DamageLevel.فاقد_خسارت;

        carPrice = Math.Min(carPrice, conventionalCarPrice);

        if (changedRoom && currentYear - manufacturedYear <= 4)
            return .1f * carPrice;

        float damageCoefficient = GetDamageCoefficient(input, manufacturedYear);

        float ageCoefficient = GetAgeCoefficient(manufacturedYear);

        float priceDrop = ageCoefficient * carPrice * damageCoefficient / 400;

        priceDrop = Math.Min(priceDrop, .2f * carPrice);

        return priceDrop;
    }

    private static float GetDamageCoefficient(Dictionary<string, DamageLevel> input, int manufacturedYear)
    {
        bool changedCylinderBlock = input["cylinder_block"] != DamageLevel.فاقد_خسارت;

        if (changedCylinderBlock && currentYear - manufacturedYear <= 4)
        {
            return 5 * input.Count(x => x.Value != DamageLevel.فاقد_خسارت && x.Key != "room_change");
        }

        int sum = 0;
        foreach(var item in input)
            if(item.Value > DamageLevel.فاقد_خسارت &&
               (changedCylinderBlock || item.Key != "cylinder_block") &&
               item.Key != "room_change")
                sum += damageConfig.GetItem(item.Key)
                                   .GetLevelValue((int)item.Value);
            
        return sum;
    }

    private static float GetAgeCoefficient(int manufacturedYear)
    {
        int model = currentYear - manufacturedYear;
        if (!ageConfg.TryGetItem(model, out float ageCoefficient))
            throw new ApplicationException("افت قیمت به دلیل سن بیشتر از 10 سال قابل محاسبه نیست.");
        return ageCoefficient;
    }
}
