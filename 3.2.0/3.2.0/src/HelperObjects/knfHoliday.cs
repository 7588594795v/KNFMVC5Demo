using System;

namespace KNFMVC5Demo.HelperObjects
{
    public static class knfHoliday
    {
        public static bool IsDateAWeekWorkDay(DateTime adtDateFrom)
        {
            var lblnIsDateAWeekWorkDay = false;
            
            switch (adtDateFrom.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    lblnIsDateAWeekWorkDay = false;
                    break;
                case DayOfWeek.Monday:
                    lblnIsDateAWeekWorkDay = true;
                    break;
                case DayOfWeek.Tuesday:
                    lblnIsDateAWeekWorkDay = true;
                    break;
                case DayOfWeek.Wednesday:
                    lblnIsDateAWeekWorkDay = true;
                    break;
                case DayOfWeek.Thursday:
                    lblnIsDateAWeekWorkDay = true;
                    break;
                case DayOfWeek.Friday:
                    lblnIsDateAWeekWorkDay = true;
                    break;
                case DayOfWeek.Saturday:
                    lblnIsDateAWeekWorkDay = false;
                    break;
                default:
                    lblnIsDateAWeekWorkDay = false;
                    break;
            }
            //ToDo Add Compare date with Declared Holidays in DataBase.

            return lblnIsDateAWeekWorkDay;
        }

        public static bool IsWeekend(DateTime adtDate)
        {
            var lblnIsWeekend = false;

            switch (adtDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    lblnIsWeekend = true;
                    break;
                case DayOfWeek.Monday:
                    lblnIsWeekend = false;
                    break;
                case DayOfWeek.Tuesday:
                    lblnIsWeekend = false;
                    break;
                case DayOfWeek.Wednesday:
                    lblnIsWeekend = false;
                    break;
                case DayOfWeek.Thursday:
                    lblnIsWeekend = false;
                    break;
                case DayOfWeek.Friday:
                    lblnIsWeekend = false;
                    break;
                case DayOfWeek.Saturday:
                    lblnIsWeekend = true;
                    break;
                default:
                    lblnIsWeekend = false;
                    break;
            }
            
            return lblnIsWeekend;
        }

        public static bool IsHoliday(DateTime adtDate)
        {
            var lblnIsHoliday = false;

            return lblnIsHoliday;
        }

        public static int GetNoOfWorkingDays(DateTime adtStartDate, DateTime adtEndDate)
        {
            var lintNoOfWorkingDays = knfConstant.ZERO;

            return lintNoOfWorkingDays;
        }
    }
}