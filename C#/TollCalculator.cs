using System;
using System.Globalization;
using TollFeeCalculator;

public class TollCalculator
{
    private const int MAX_TOLL_FEE_PER_DAY = 60; // Max fee per day is 60
    /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

    public int GetTollFee(Vehicle vehicle, DateTime[] dates)
    {
        // Check for null
        if (vehicle == null || dates == null) throw new ArgumentNullException();

        // Check if the dates array is empty
        if (dates.Length == 0) throw new ArgumentException("Dates array is empty");

        // If we have only one date, just return the fee for that one
        if (dates.Length == 1) return GetTollFee(dates[0], vehicle);

        // Check that all dates are on the same day, by comparing the first date to the rest. (Trust but verify)
        for (int i = 1; i < dates.Length; i++)
        {
            if (!dates[i].Date.Equals(dates[0].Date)) throw new ArgumentException("Dates are not on the same day");
        }

        // Sort the dates
        Array.Sort(dates);

        // Calculating the dayly total fee for the day, using a sliding window of 1 hour
        DateTime windowstart = dates[0];
        int windowfee = 0;
        int totalfee = 0;

        foreach (DateTime date in dates)
        {
            int fee = GetTollFee(date, vehicle);

            if ((date - windowstart).TotalMinutes <= 60)
            {
                windowfee = Math.Max(windowfee, fee);
            }
            else
            {
                totalfee += windowfee;
                windowstart = date;
                windowfee = fee;
            }
        }

        totalfee += windowfee;

        if (totalfee > MAX_TOLL_FEE_PER_DAY) return MAX_TOLL_FEE_PER_DAY;

        return totalfee;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return false;
        String vehicleType = vehicle.GetVehicleType();
        return vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
               vehicleType.Equals(TollFreeVehicles.Military.ToString());
    }

    private int GetTollFee(DateTime date, Vehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

        int hour = date.Hour;
        int minute = date.Minute;

        if (hour == 6 && minute >= 0 && minute <= 29) return 8;
        else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
        else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
        else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
        else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
        else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
        else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
        else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
        else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
        else return 0;
    }

    private Boolean IsTollFreeDate(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        int day = date.Day;

        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

        if (year == 2013)
        {
            if (month == 1 && day == 1 ||
                month == 3 && (day == 28 || day == 29) ||
                month == 4 && (day == 1 || day == 30) ||
                month == 5 && (day == 1 || day == 8 || day == 9) ||
                month == 6 && (day == 5 || day == 6 || day == 21) ||
                month == 7 ||
                month == 11 && day == 1 ||
                month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
            {
                return true;
            }
        }
        return false;
    }

    private enum TollFreeVehicles
    {
        Motorbike = 0,
        Tractor = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5
    }
}