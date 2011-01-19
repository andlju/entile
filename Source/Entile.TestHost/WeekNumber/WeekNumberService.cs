using System;
using System.Globalization;
using System.IO;
using System.ServiceModel.Activation;
using System.Web.Hosting;
using Entile.Service;

namespace Entile.TestHost.WeekNumber
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WeekNumberService : IWeekNumberService
    {
        enum WeekNumberingMethod
        {
            Iso,
            UsAndCanada
        }

        private readonly IRegistrator _registrator;

        public WeekNumberService(IRegistrator registrator)
        {
            _registrator = registrator;
        }

        public Stream GetWeekNumberImage(string uniqueId)
        {
            var registration = _registrator.GetRegistration(uniqueId);

            if (registration == null)
                throw new ArgumentException("Client with this unique Id is not subscribed to the Week Number service", "uniqueId");

            var extraInfo = _registrator.GetExtraInfo(uniqueId);
            string timeZoneOffsetValue;
            string numberingMethodValue;
            extraInfo.TryGetValue("TimeZoneOffset", out timeZoneOffsetValue);
            extraInfo.TryGetValue("NumberingMethod", out numberingMethodValue);

            int timeZoneOffset;
            int.TryParse(timeZoneOffsetValue, out timeZoneOffset);

            var weekNumber = GetWeekNumber(timeZoneOffset, numberingMethodValue, null);

            var path =
                HostingEnvironment.MapPath(string.Format("~/WeekNumber/Images/WeekNumberTileBackground_{0:00}.png",
                                                         weekNumber));
            return File.OpenRead(path);
        }

        private int GetWeekNumber(int timeZoneOffset, string methodName, string selectedDate)
        {
            DateTime selectedDateTmp;
            if (!DateTime.TryParse(selectedDate,
                                   CultureInfo.InvariantCulture.DateTimeFormat,
                                   DateTimeStyles.None,
                                   out selectedDateTmp))
            {
                selectedDateTmp = DateTime.UtcNow.AddHours(timeZoneOffset);
            }

            WeekNumberingMethod method;
            if (!Enum.TryParse(methodName, true, out method))
            {
                method = WeekNumberingMethod.Iso;
            }
            return SelectedWeekNumber(method, selectedDateTmp);
        }

        private int SelectedWeekNumber(WeekNumberingMethod method, DateTime selectedDate)
        {
            if (method == WeekNumberingMethod.Iso)
                return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    selectedDate,
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
            if (method == WeekNumberingMethod.UsAndCanada)
                return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    selectedDate,
                    CalendarWeekRule.FirstDay,
                    DayOfWeek.Sunday);
            return 0;
        }

    }
}