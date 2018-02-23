#region [NameSpaces]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using System.Web;
using Dapper;
using System.Security.Cryptography;
#endregion


namespace KNFMVC5Demo.HelperObjects
{
    public static class knfGlobalFunctions
    {
        const string _istrImageTagRegex = "<img ([a-zA-Z0-9_\\-/=;'\"\r\n\t ]+) src=['\"]([a-zA-Z0-9_\\-/=;\r\n\t.'\" ]+)['\"]";
        const string _istrImageTagReconstruct = "<img {0} src=\"cid:{1}\" ";
        const string _istrHtmlTagRegex = "<.*?>|&.*?;";

        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static TimeZoneInfo India_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        #region [Delegates]

        //public delegate utlError SendTemplateEmailNotificationDelegate(busCorTemplates abusCommTemplate, busCorTracking abusCorTracking, string astrCorGeneratedPath, utlPassInfo autlPassInfo);
        //public delegate utlError SendEmailDelegate(string lstrRecipientEmailAddress, string lstrSubjectLine, string lstrBody, System.Text.Encoding lenmEncoding, bool lblnIsBodyHtml, Collection<AttachmentBase> acollAttachments, utlPassInfo autlPassInfo, int aintRecipientID = 0);

        #endregion [Delegates]

        #region Properties

        public static DateTime ApplicationDateTime() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, India_Standard_Time);

        #endregion

        #region [Transaction]
        /// <summary>
        /// Get the OrgId from the orgcodef organization
        /// </summary>
        /// <param name="astrOrgCode"></param>
        /// <returns></returns>
        //public static int GetOrgIdFromBusinessUnitCode(string astrBusinessUnitCode)
        //{
        //    int lintOrgId = 0;
        //    if (String.IsNullOrEmpty(astrBusinessUnitCode)) return lintOrgId;
        //    DataTable ldtbOrganization = busBase.Select<cdoOrganization>(new string[1] { enmOrganization.business_unit_code.ToString() },
        //          new object[1] { astrBusinessUnitCode }, null, null);
        //    if (ldtbOrganization.Rows.Count > 0)
        //    {
        //        lintOrgId = Convert.ToInt32(ldtbOrganization.Rows[0]["org_id"]);
        //    }
        //    return lintOrgId;
        //}

        ///// <summary>
        ///// Get Org Name From Org Id
        ///// </summary>
        ///// <param name="aintOrgID"></param>
        ///// <returns></returns>
        //public static string GetOrgNameFromOrgID(int aintOrgID)
        //{
        //    string lstrOrgName = string.Empty;
        //    DataTable ldtbOrganization = busBase.Select<cdoOrganization>(new string[1] { "org_id" },
        //          new object[1] { aintOrgID }, null, null);
        //    if (ldtbOrganization.Rows.Count > 0)
        //    {
        //        lstrOrgName = ldtbOrganization.Rows[0]["org_name"].ToString();
        //    }
        //    return lstrOrgName;
        //}

        /// <summary>
        /// Get Person Id By SSN
        /// </summary>
        /// <param name="astrSSN">SSN</param>
        /// <returns></returns>
        //public static int GetPersonIdFromSSN(string astrSSN)
        //{
        //    int lintPersonId = 0;
        //    DataTable ldtbSSN = busBase.Select<cdoPerson>(
        //        new string[1] { enmPerson.ssn.ToString() },
        //        new object[1] { astrSSN }, null, null);
        //    if (ldtbSSN.IsNotNull() && ldtbSSN.Rows.Count > 0)
        //    {
        //        lintPersonId = Convert.ToInt32(ldtbSSN.Rows[0][enmPerson.person_id.ToString()].ToString());
        //    }
        //    return lintPersonId;
        //}

        /// <summary>
        /// Determines whether the string is not null or empty.
        /// </summary>
        /// <returns>Boolean indicating whether the string is not null or not empty</returns>
        public static bool IsNotNullOrEmpty(this string astrText)
        {
            return !String.IsNullOrEmpty(astrText);
        }

        /// <summary>
        /// Determines whether a string is null or empty
        /// </summary>
        /// <returns>True if string is null or empty. False if not null or not empty</returns>
        public static bool IsNullOrEmpty(this string astrText)
        {
            return string.IsNullOrEmpty(astrText);
        }

        /// <summary>
        /// Get the org short name from the org id from organization
        /// </summary>
        /// <param name="astrOrgCode"></param>
        /// <returns></returns>
        //public static string GetOrgShortNameFromOrgId(int aintOrgid)
        //{
        //    string lstrResult = string.Empty;
        //    if (aintOrgid == 0)
        //        return lstrResult;
        //    DataTable ldtbOrganization = busBase.Select<cdoOrganization>(new string[1] { enmOrganization.org_id.ToString() },
        //          new object[1] { aintOrgid }, null, null);
        //    if (ldtbOrganization.Rows.Count > 0)
        //    {
        //        lstrResult = Convert.ToString(ldtbOrganization.Rows[0]["ORG_SHORT_NAME"]);
        //    }
        //    return lstrResult;
        //}

        #endregion

        #region Common Methods
        /// <summary>
        /// Converts string to Proper case
        /// Ex. mcdonald --> McDonald, neil o'brien --> Neil O'Brien
        /// PIR: 2772 , Dev: Sagar
        /// </summary>
        /// <param name="astrTextToFormat"></param>
        /// <returns></returns>
        public static string ConvertToProperCase(string astrTextToFormat)
        {
            if (astrTextToFormat.IsNullOrEmpty())
                return astrTextToFormat;
            TextInfo lobjTextInfo = new CultureInfo("en-US", false).TextInfo;
            astrTextToFormat = lobjTextInfo.ToTitleCase(astrTextToFormat.ToLower());
            StringBuilder lstrbProperCaseWords = new StringBuilder();
            char[] larrProperWord;
            string lstrWord = "";
            foreach (string lstrText in astrTextToFormat.Split(" "))
            {
                lstrWord = lstrText;
                larrProperWord = lstrText.ToCharArray();
                if (lstrWord.Contains("'") && lstrWord.IndexOf("'") == 1 && lstrWord.Length >= 3)
                {
                    larrProperWord[lstrWord.IndexOf("'") + 1] = Char.ToUpper(larrProperWord[lstrWord.IndexOf("'") + 1]);
                    lstrWord = new string(larrProperWord);
                }
                if (larrProperWord.Length > 3 && (char.ToUpper(larrProperWord[0]) == 'M' && char.ToLower(larrProperWord[1]) == 'c' && !String.IsNullOrEmpty(larrProperWord[2].ToString())))
                {
                    larrProperWord[0] = char.ToUpper(larrProperWord[0]);
                    larrProperWord[1] = char.ToLower(larrProperWord[1]);
                    larrProperWord[2] = char.ToUpper(larrProperWord[2]);
                }

                lstrbProperCaseWords.Append(new string(larrProperWord) + " ");
            }

            return lstrbProperCaseWords.ToString().Substring(0, lstrbProperCaseWords.Length - 1);
        }

        //public static string GetMSSEntrustExecutionErrorMessage(IdentityGuardAdminServiceV10API.ErrorCode aErrorCode)
        //{
        //    if (aErrorCode.IsNull())
        //    {
        //        return null;
        //    }

        //    int lintMessageID = 9501;

        //    if (aErrorCode == IdentityGuardAdminServiceV10API.ErrorCode.INVALID_USERID_PASSWORD)
        //    {
        //        lintMessageID = 9515;
        //    }

        //    return utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageText(lintMessageID);
        //}

        //public static string GetMSSRegistrationErrorMessage(MSSUserType iMSSUserType, MSSRegistrationCode aErrorCode)
        //{
        //    object[] lobjFormatParms = null;

        //    if (aErrorCode.IsNull())
        //    {
        //        return null;
        //    }

        //    int lintMessageID = 9501;

        //    if (aErrorCode == MSSRegistrationCode.INVALID_FORGOT_USER_NAME)
        //    {
        //        lintMessageID = 9530;
        //        lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrCustomerSupportURL };
        //    }
        //    else if (aErrorCode == MSSRegistrationCode.INVALID_PASSWORD)
        //    {
        //        lintMessageID = 9515;
        //    }
        //    else if (aErrorCode == MSSRegistrationCode.INVALID_USER_NAME)
        //    {
        //        lintMessageID = 9529;
        //    }
        //    else if (aErrorCode == MSSRegistrationCode.REGISTRATION_INVALID_SSN)
        //    {
        //        //DS1300-ACT0001-UI0002-BR0022
        //        if (iMSSUserType == MSSUserType.PCG)
        //        {
        //            lintMessageID = 9526;
        //        }
        //        else
        //        {
        //            lintMessageID = 9521;
        //        }
        //    }
        //    //DS1300-ACT0001-UI0002-BR0018
        //    //DS1300-ACT0001-UI0002-BR0017
        //    else if (aErrorCode == MSSRegistrationCode.ALREADY_REGISTERED)
        //    {
        //        lintMessageID = 9522;
        //        lobjFormatParms = new object[2] { busMemberPortalAccountHelper.istrMSSExternalLoginURL, busMemberPortalAccountHelper.istrMSSShortName };
        //    }
        //    //DS1300-ACT0001-UI0002-BR0020
        //    //DS1300-ACT0001-UI0002-BR0021
        //    else if (aErrorCode == MSSRegistrationCode.REGISTRATION_LOCKED)
        //    {
        //        lintMessageID = 9523;
        //        lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrCustomerSupportURL };
        //    }
        //    //DS1300-ACT0001-UI0002-BR0005
        //    //DS1300-ACT0001-UI0002-BR0009
        //    else if (aErrorCode == MSSRegistrationCode.NOT_ELIGIBLE)
        //    {
        //        lintMessageID = 9524;
        //        lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrCustomerSupportURL };
        //    }
        //    else if (aErrorCode == MSSRegistrationCode.INVALID_CONTACT_ID)
        //    {
        //        lintMessageID = 9525;
        //    }
        //    else if (aErrorCode == MSSRegistrationCode.CONTACT_NO_AUTHORITY)
        //    {
        //        lintMessageID = 9527;
        //    }

        //    string lstrMessage = utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageText(lintMessageID);

        //    if (!lobjFormatParms.IsNullOrEmpty())
        //    {
        //        lstrMessage = lstrMessage.Format(lobjFormatParms);
        //    }

        //    return lstrMessage;
        //}

        //public static bool SendSMS(string astrRecipientPhoneNo, string astrBody)
        //{
        //    string lstrAccountSid = knfGlobalFunctions.GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.Common.Twilio.CONFIG_TWILIO_ACCOUNT_SID);
        //    string lstrAuthToken = knfGlobalFunctions.GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.Common.Twilio.CONFIG_TWILIO_AUTH_TOKEN);
        //    string lstrFromPhone = knfGlobalFunctions.GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.Common.Twilio.CONFIG_TWILIO_FROM_PHONE_NO);

        //    try
        //    {
        //        var twilio = new TwilioRestClient(lstrAccountSid, lstrAuthToken);
        //        Twilio.Message msg = twilio.SendMessage(lstrFromPhone, astrRecipientPhoneNo, astrBody);

        //        if (msg.RestException.IsNotNull())
        //        {
        //            ExceptionManager.Publish(new Exception(msg.RestException.Message));
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionManager.Publish(e);
        //        return false;
        //    }
        //}

        //public static string GetMSSEntrustExecutionErrorMessage(MSSUserType aMSSUserType, ErrorCode aErrorCode,
        //                                                         MSSPrimaryAuthenticationType aMSSPrimaryAuthenticationType,
        //                                                         MSSSecondaryAuthenticationType aMSSSecondaryAuthenticationType,
        //                                                         string astrAdditionalMessageParam = null)
        //{
        //    object[] lobjFormatParms = null;

        //    if (aErrorCode.IsNull())
        //    {
        //        return null;
        //    }

        //    int lintMessageID = 9501;

        //    if (aErrorCode == ErrorCode.USER_DOES_NOT_EXIST)
        //    {
        //        lintMessageID = 9500;
        //    }
        //    else if (aErrorCode == ErrorCode.USER_LOCKED || aErrorCode == ErrorCode.USER_SUSPENDED)
        //    {
        //        lintMessageID = 9502;
        //    }
        //    else if (aErrorCode == ErrorCode.INVALID_IP_ADDRESS || aErrorCode == ErrorCode.NOT_SUPPORTED)
        //    {
        //        lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrCustomerSupportURL };
        //        lintMessageID = 9503;
        //    }
        //    else if (aErrorCode == ErrorCode.PIN_EXPIRED)
        //    {
        //        if (aMSSSecondaryAuthenticationType == MSSSecondaryAuthenticationType.KBA)
        //        {
        //            if (aMSSPrimaryAuthenticationType == MSSPrimaryAuthenticationType.ExternalESign)
        //            {
        //                lobjFormatParms = new object[1] { string.Empty };
        //            }
        //            else
        //            {
        //                lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrMSSExternalLoginURL };
        //            }

        //            lintMessageID = 9514;
        //        }
        //    }
        //    else if (aErrorCode == ErrorCode.NO_ACTIVE_CARDS)
        //    {
        //        lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrCustomerSupportURL };
        //        lintMessageID = 9519;
        //    }
        //    else if (aErrorCode == ErrorCode.INVALID_RESPONSE)
        //    {
        //        if (aMSSPrimaryAuthenticationType == MSSPrimaryAuthenticationType.ForgotPasswordAlternate)
        //        {
        //            lintMessageID = 9517;
        //        }
        //        else if (aMSSPrimaryAuthenticationType == MSSPrimaryAuthenticationType.ForgotPassword)
        //        {
        //            if (aMSSUserType == MSSUserType.PCG)
        //            {
        //                lintMessageID = 9518;
        //            }
        //            else
        //            {
        //                lintMessageID = 9516;
        //            }
        //        }
        //        else if (aMSSSecondaryAuthenticationType == MSSSecondaryAuthenticationType.KBA)
        //        {
        //            if (aMSSPrimaryAuthenticationType == MSSPrimaryAuthenticationType.ExternalESign)
        //            {
        //                lobjFormatParms = new object[1] { string.Empty };
        //            }
        //            else
        //            {
        //                lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrMSSExternalLoginURL };
        //            }
        //            //TODO set the registration URL

        //            if (aMSSPrimaryAuthenticationType == MSSPrimaryAuthenticationType.Registration)
        //            {
        //                if (aMSSUserType == MSSUserType.PCG)
        //                {
        //                    lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrPCGRegisterURL };
        //                }

        //                lintMessageID = 9528;
        //            }
        //            else
        //            {
        //                lintMessageID = 9513;
        //            }
        //        }
        //        else if (aMSSSecondaryAuthenticationType == MSSSecondaryAuthenticationType.Enhanced)
        //        {
        //            lintMessageID = 9506;
        //        }
        //        else if (aMSSPrimaryAuthenticationType == MSSPrimaryAuthenticationType.OTP)
        //        {
        //            if (aMSSUserType == MSSUserType.PCG)
        //            {
        //                lintMessageID = 9511;
        //            }
        //            else
        //            {
        //                lintMessageID = 9510;
        //            }
        //        }
        //        else if (aMSSPrimaryAuthenticationType == MSSPrimaryAuthenticationType.AYSO)
        //        {
        //            lintMessageID = 9512;
        //        }
        //        else
        //        {
        //            lintMessageID = 9504;
        //        }
        //    }
        //    else if (aErrorCode == ErrorCode.OTP_EXPIRED)
        //    {
        //        if (aMSSSecondaryAuthenticationType == MSSSecondaryAuthenticationType.Enhanced)
        //        {
        //            lintMessageID = 9507;
        //        }
        //        lintMessageID = 9508;
        //    }
        //    else if (aErrorCode == ErrorCode.AUTH_FAILED_USER_LOCKED)
        //    {
        //        lobjFormatParms = new object[1] { busMemberPortalAccountHelper.istrCustomerSupportURL };
        //        lintMessageID = 9505;
        //    }
        //    else if (aErrorCode == ErrorCode.TRANSACTION_VALIDATION_ERROR_USER_LOCKED)
        //    {
        //        lobjFormatParms = new object[1] { astrAdditionalMessageParam };
        //        lintMessageID = 9509;
        //    }

        //    string lstrMessage = utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageText(lintMessageID);

        //    if (!lobjFormatParms.IsNullOrEmpty())
        //    {
        //        lstrMessage = lstrMessage.Format(lobjFormatParms);
        //    }

        //    return lstrMessage;
        //}

        /// <summary>
        /// Check whether the e-mail is having valid 
        /// </summary>
        /// <param name="astrEmail"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string astrEmail)
        {
            Regex lrexEmail = new Regex(@"(\w[-._\w]*\w@\w[-._\w]*\w\.\w{2,3})");
            return (lrexEmail.IsMatch(astrEmail));
        }

        /// <summary>
        /// Checks whether the email address is valid(Should not have white spaces).
        /// </summary>
        /// <param name="astrEmailID">The email address that is to be validated.</param>
        /// <returns>Returns true if the email address is valid.</returns>
        public static bool IsValidateEmail(string astrEmailID)
        {
            string lstrPattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            return astrEmailID.ValidateRegularExpression(lstrPattern);
        }

        /// <summary>
        /// Validates the string with given regular expression.
        /// </summary>
        /// <param name="astrStringValue">The string that is to be validated.</param>
        /// <param name="astrPattern">The regular expression pattern from which the string is to be validated.</param>
        /// <returns>Returns true if the regular expression matches the given pattern, else false.</returns>
        public static bool ValidateRegularExpression(this string astrStringValue, string astrPattern)
        {
            if (string.IsNullOrWhiteSpace(astrStringValue) || astrPattern == null)
            {
                return false;
            }
            return ((new Regex(astrPattern)).IsMatch(astrStringValue));
        }

        /// <summary>
        /// Check whether two date range overlaps each other.
        /// </summary>
        /// <param name="adtStart1"></param>
        /// <param name="adtEnd1"></param>
        /// <param name="adtStart2"></param>
        /// <param name="adtEnd2"></param>
        /// <returns></returns>
        public static bool IsDateRangeOverlaps(DateTime adtStart1, DateTime adtEnd1, DateTime adtStart2, DateTime adtEnd2)
        {
            bool lblnResult = false;
            DateTime ldtStart = adtStart1 > adtStart2 ? adtStart1 : adtStart2; // max of starts 
            DateTime ldtSend = adtEnd1 < adtEnd2 ? adtEnd1 : adtEnd2; // min of ends 
            if (ldtStart <= ldtSend)
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        /// <summary>
        /// Convert string to ProperCase
        /// </summary>
        /// <param name="astrInput">Input String</param>
        /// <returns>Propercase string</returns>
        public static string ToProperCase(this string astrInput)
        {
            if (string.IsNullOrWhiteSpace(astrInput))
                return string.Empty;
            else
            {
                CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                TextInfo lobjTextInfo = cultureInfo.TextInfo;
                return lobjTextInfo.ToTitleCase(astrInput.ToLower());
            }
        }

        /// <summary>
        /// Format String input to remove SQL Injection charaacters.
        /// </summary>
        /// <param name="astrInput">Input String</param>
        /// <returns>Safe String</returns>
        public static string FormatStringtoPreventSQLInjection(string astrInput)
        {
            string lstrOutput = astrInput.Trim().Replace("'", "''");
            return lstrOutput;
        }
        /// <summary>
        /// Calculates the Current Age of person.
        /// </summary>
        /// <param name="adtDateOfBirth">Date Of Birth</param>
        /// <param name="adtAgeCalculationDate">Age Calcualtion Date</param>
        /// <returns>DateTime</returns>
        public static int CalculateAge(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate)
        {
            int lintYears = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
            // subtract another year if we're before the
            // birth day in the current year
            if ((adtAgeCalculationDate.Month < adtDateOfBirth.Month) ||
                 (adtAgeCalculationDate.Month == adtDateOfBirth.Month && adtAgeCalculationDate.Day < adtDateOfBirth.Day))
            {
                lintYears--;
            }

            return lintYears;
        }

        /// <summary>
        /// Calculates the current Age of person in year and months
        /// </summary>
        /// <param name="adtDateOfBirth">Person Date Of Birth</param>
        /// <param name="adtAgeCalculationDate">Age Calculation Date</param>
        /// <returns>Age in year and months</returns>
        public static String CalculateAgeInYearAndMonths(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate)
        {
            int lintMonths = 0;
            int lintYears = 0;
            int lintDaysInBdayMonth = DateTime.DaysInMonth(adtDateOfBirth.Year, adtDateOfBirth.Month);
            int lintDaysRemain = adtAgeCalculationDate.Day + (lintDaysInBdayMonth - adtDateOfBirth.Day);
            if (adtAgeCalculationDate.Month > adtDateOfBirth.Month)
            {
                lintYears = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
                lintMonths = adtAgeCalculationDate.Month - (adtDateOfBirth.Month + 1) + Math.Abs(lintDaysRemain / lintDaysInBdayMonth);
            }
            else if (adtAgeCalculationDate.Month == adtDateOfBirth.Month)
            {
                if (adtAgeCalculationDate.Day >= adtDateOfBirth.Day)
                {
                    lintYears = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
                    lintMonths = knfConstant.ZERO;
                }
                else
                {
                    lintYears = (adtAgeCalculationDate.Year - 1) - adtDateOfBirth.Year;
                    lintMonths = knfConstant.ELEVEN;
                }
            }
            else
            {
                lintYears = (adtAgeCalculationDate.Year - 1) - adtDateOfBirth.Year;
                lintMonths = adtAgeCalculationDate.Month + (11 - adtDateOfBirth.Month) + Math.Abs(lintDaysRemain / lintDaysInBdayMonth);
            }
            return string.Format("{0} Year{1} {2} Month{3} ", lintYears, (lintYears == 1) ? "" : "(s)", lintMonths, (lintMonths == 1) ? "" : "(s)");
        }

        /// <summary>
        /// Calculates the current Age of person in year and months
        /// </summary>
        /// <param name="adtDateOfBirth"></param>
        /// <param name="adtAgeCalculationDate"></param>
        /// <param name="lintYears"></param>
        /// <param name="lintMonths"></param>
        public static void CalculateAgeInYearAndMonths(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate, out int lintYears, out int lintMonths)
        {
            lintMonths = 0;
            lintYears = 0;
            int lintDaysInBdayMonth = DateTime.DaysInMonth(adtDateOfBirth.Year, adtDateOfBirth.Month);
            int lintDaysRemain = adtAgeCalculationDate.Day + (lintDaysInBdayMonth - adtDateOfBirth.Day);
            if (adtAgeCalculationDate.Month > adtDateOfBirth.Month)
            {
                lintYears = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
                lintMonths = adtAgeCalculationDate.Month - (adtDateOfBirth.Month + 1) + Math.Abs(lintDaysRemain / lintDaysInBdayMonth);
            }
            else if (adtAgeCalculationDate.Month == adtDateOfBirth.Month)
            {
                if (adtAgeCalculationDate.Day >= adtDateOfBirth.Day)
                {
                    lintYears = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
                    lintMonths = knfConstant.ZERO;
                }
                else
                {
                    lintYears = (adtAgeCalculationDate.Year - 1) - adtDateOfBirth.Year;
                    lintMonths = knfConstant.ELEVEN;
                }
            }
            else
            {
                lintYears = (adtAgeCalculationDate.Year - 1) - adtDateOfBirth.Year;
                lintMonths = adtAgeCalculationDate.Month + (11 - adtDateOfBirth.Month) + Math.Abs(lintDaysRemain / lintDaysInBdayMonth);
            }
        }
        /// <summary>
        /// Append a string with the specified character and string
        /// </summary>
        /// <param name="astrOriginal">Original String</param>
        /// <param name="astrAppendString">String to be appended</param>
        /// <param name="aenmAppendCharacter">Append Character</param>
        /// <returns>Appended String</returns>
        public static string AppendStringWithChar(string astrOriginal, string astrAppendString, enmAppendCharacter aenmAppendCharacter)
        {
            if (string.IsNullOrEmpty(astrOriginal))
                return astrAppendString;
            else if (string.IsNullOrEmpty(astrAppendString))
                return astrOriginal;
            else
            {
                switch (aenmAppendCharacter)
                {
                    case enmAppendCharacter.Comma:
                        return string.Format("{0},{1}", astrOriginal, astrAppendString);
                    case enmAppendCharacter.CommaSpace:
                        return string.Format("{0}, {1}", astrOriginal, astrAppendString);
                    case enmAppendCharacter.Space:
                        return string.Format("{0} {1}", astrOriginal, astrAppendString);
                    case enmAppendCharacter.FullStopSpace:
                        return string.Format("{0}. {1}", astrOriginal, astrAppendString);
                    case enmAppendCharacter.Newline:
                        return string.Format("{0}\n{1}", astrOriginal, astrAppendString);
                    case enmAppendCharacter.Dash:
                        return string.Format("{0}-{1}", astrOriginal, astrAppendString);
                    case enmAppendCharacter.QuestionSpace:
                        return string.Format("{0}? {1}", astrOriginal, astrAppendString);
                    case enmAppendCharacter.ExclamationSpace:
                        return string.Format("{0}! {1}", astrOriginal, astrAppendString);
                    default:
                        return string.Format("{0}{1}", astrOriginal, astrAppendString);
                }
            }
        }

        /// <summary>
        /// Get Working Date
        /// </summary>
        /// <param name="adtDateFrom">From Date</param>
        /// <param name="aintNoOfDays">No of Days afer From Date</param>
        /// <returns>Working Dates</returns>
        public static DateTime AddWorkingDays(this DateTime adtDateFrom, int aintNoOfDays)
        {
            int lintIncrementOrDecrement = aintNoOfDays >= 0 ? 1 : -1;

            while (aintNoOfDays != 0)
            {
                adtDateFrom = adtDateFrom.AddDays(lintIncrementOrDecrement);

                if (knfHoliday.IsDateAWeekWorkDay(adtDateFrom))
                    aintNoOfDays -= lintIncrementOrDecrement;
            }

            return adtDateFrom;
        }

        /// <summary>
        /// Checks whether given date is first business day of the month.
        /// </summary>
        /// <param name="adtGivenDate"></param>
        /// <returns></returns>
        public static bool IsFirstBusinessDayOftheMonth(DateTime adtGivenDate)
        {
            return (adtGivenDate.Date == GetFirstBusinessDayOftheMonth(adtGivenDate).Date);
        }

        /// <summary>
        /// This method gets first business day of the month.
        /// </summary>
        /// <param name="adtGivenDate"></param>
        /// <returns></returns>
        public static DateTime GetFirstBusinessDayOftheMonth(DateTime adtGivenDate)
        {
            DateTime ldtDate = new DateTime(adtGivenDate.Year, adtGivenDate.Month, 1);
            DateTime ldtLastDayOfMonth = new DateTime(ldtDate.Year, ldtDate.Month, DateTime.DaysInMonth(ldtDate.Year, ldtDate.Month));
            DateTime ldtFirstBusinessDay = ldtDate;

            do
            {
                if (!IsDateAWeekWorkDay(ldtDate))
                {
                    ldtDate = ldtDate.AddDays(1);
                }
                else
                {
                    ldtFirstBusinessDay = ldtDate;
                    break;
                }

            } while (ldtDate <= ldtLastDayOfMonth);

            return ldtFirstBusinessDay;
        }

        /// <summary>
        /// Get First Day of Month
        /// </summary>
        /// <param name="adtDateInGivenMonth">Date in the Month</param>
        /// <returns>First Day of the Month</returns>
        public static DateTime GetFirstDayOfMonth(this DateTime adtDateInGivenMonth)
        {
            return new DateTime(adtDateInGivenMonth.Year, adtDateInGivenMonth.Month, 1);
        }

        /// <summary>
        /// Get Last Day of Month
        /// </summary>
        /// <param name="adtDateInGivenMonth">Date in the Month</param>
        /// <returns>Last Day of the Month</returns>
        public static DateTime GetLastDayOfMonth(this DateTime adtDateInGivenMonth)
        {
            return (adtDateInGivenMonth.GetFirstDayOfMonth()).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Get First Day of next Month
        /// </summary>
        /// <param name="adtDateInGivenMonth">Date in the Month</param>
        /// <returns>First Day of the Month</returns>
        public static DateTime GetFirstDayOfNextMonth(this DateTime adtDateInGivenMonth)
        {
            return new DateTime(adtDateInGivenMonth.Year, adtDateInGivenMonth.AddMonths(1).Month, 1);
        }

        /// <summary>
        /// Get Last Day of previous Month
        /// </summary>
        /// <param name="adtDateInGivenMonth">Date in the Month</param>
        /// <returns>Last Day of the Month</returns>
        public static DateTime GetLastDayOfPreviousMonth(this DateTime adtDateInGivenMonth)
        {
            return (adtDateInGivenMonth.GetFirstDayOfMonth()).AddDays(-1);
        }

        /// <summary>
        /// Check if the given date is a work date
        /// </summary>
        /// <param name="adtGivenDate">Given Date</param>
        /// <returns>True if work day; False if not</returns>
        public static bool IsDateAWeekWorkDay(this DateTime adtGivenDate)
        {
            return knfHoliday.IsDateAWeekWorkDay(adtGivenDate);
        }

        /// <summary>
        /// This method returns true/false if given date is Weekend date.
        /// </summary>
        /// <param name="adtDate">Date</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime adtDate)
        {
            return knfHoliday.IsWeekend(adtDate);
        }

        /// <summary>
        /// This method returns true/false if given date is holiday date
        /// </summary>
        /// <param name="adtDate">Date</param>
        /// <returns></returns>
        public static bool IsHoliday(this DateTime adtDate)
        {
            return knfHoliday.IsHoliday(adtDate);
        }

        /// <summary>
        /// Returns the count of working days between two days
        /// </summary>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>Count of Working Days</returns>
        public static int CountOfWorkingDays(DateTime adtStartDate, DateTime adtEndDate)
        {
            return knfHoliday.GetNoOfWorkingDays(adtStartDate, adtEndDate);
        }

        /// <summary>
        /// Check if the Given date is in the current fiscal year
        /// </summary>
        /// <param name="adtGivenDate">Date</param>
        /// <returns>True if is in current fiscal year; False if not</returns>
        public static bool IsDateInCurrentFiscalYear(DateTime adtGivenDate)
        {
            DateTime ldtCurrentFiscalYearStart = DateTime.MinValue;
            DateTime ldtCurrentFiscalYearEnd = DateTime.MinValue;

            // Get current fiscal year range
            GetCurrentFisalYearRange(ref ldtCurrentFiscalYearStart, ref ldtCurrentFiscalYearEnd);

            return (ldtCurrentFiscalYearStart <= adtGivenDate && adtGivenDate <= ldtCurrentFiscalYearEnd);
        }

        /// <summary>
        /// Check if the Given date is Less than current fiscal year
        /// </summary>
        /// <param name="adtGivenDate">Date</param>
        /// <returns>True if Less than current fiscal year; False if not</returns>
        public static bool IsDateLessThanCurrentFiscalYear(DateTime adtGivenDate)
        {
            DateTime ldtCurrentFiscalYearStart = DateTime.MinValue;
            DateTime ldtCurrentFiscalYearEnd = DateTime.MinValue;

            // Get current fiscal year range
            GetCurrentFisalYearRange(ref ldtCurrentFiscalYearStart, ref ldtCurrentFiscalYearEnd);

            return (adtGivenDate < ldtCurrentFiscalYearStart);
        }

        /// <summary>
        /// This method returns the current fiscal year Date Start & End
        /// </summary>
        /// <param name="adtFiscalYearStart">Fiscal Year Start</param>
        /// <param name="adtFiscalYearEnd">Fiscal Year End</param>
        public static void GetCurrentFisalYearRange(ref DateTime adtFiscalYearStart, ref DateTime adtFiscalYearEnd)
        {
            GetFiscalYearRangeForGivenDate(knfGlobalFunctions.ApplicationDateTime().Date, ref adtFiscalYearStart, ref adtFiscalYearEnd);
        }

        /// <summary>
        /// This method provides Fiscal Year Start & End for given year
        /// Considering Fiscal Year as upper fiscal year i.e. Fiscal year = 2005-2006. In this case passed fiscal year is 2006 & start date would be "1 July 2005".
        /// </summary>
        /// <param name="aintYear"></param>
        /// <param name="adtFiscalYearStart"></param>        
        public static void GetFiscalYearStartDateForGivenYear(int aintYear, ref DateTime adtFiscalYearStart)
        {
            DateTime ldtDate = new DateTime(aintYear - 1, knfConstant.FISCAL_YEAR_START_MONTH, knfConstant.FISCAL_YEAR_START_DATE);
            DateTime ldtFiscalYearEnd = DateTime.MinValue;
            GetFiscalYearRangeForGivenDate(ldtDate, ref adtFiscalYearStart, ref ldtFiscalYearEnd);
        }

        /// <summary>
        /// This method provides Fiscal Year Start & End for given year
        /// Considering Fiscal Year as upper fiscal year i.e. Fiscal year = 2005-2006. In this case passed fiscal year is 2006 & end date would be "30 June 2006".
        /// </summary>
        /// <param name="aintYear"></param>        
        /// <param name="adtFiscalYearEnd"></param>
        public static void GetFiscalYearEndDateForGivenYear(int aintYear, ref DateTime adtFiscalYearEnd)
        {
            DateTime ldtDate = new DateTime(aintYear - 1, knfConstant.FISCAL_YEAR_START_MONTH, knfConstant.FISCAL_YEAR_START_DATE);
            DateTime ldtFiscalYearStart = DateTime.MinValue;
            GetFiscalYearRangeForGivenDate(ldtDate, ref ldtFiscalYearStart, ref adtFiscalYearEnd);
        }

        /// <summary>
        /// This method provides Fiscal Year Start & End for given year
        /// </summary>
        /// <param name="aintYear"></param>
        /// <param name="adtFiscalYearStart"></param>
        /// <param name="adtFiscalYearEnd"></param>
        public static void GetFiscalYearRangeForGivenYear(int aintYear, ref DateTime adtFiscalYearStart, ref DateTime adtFiscalYearEnd)
        {
            DateTime ldtDate = new DateTime(aintYear, 1, 1);
            GetFiscalYearRangeForGivenDate(ldtDate, ref adtFiscalYearStart, ref adtFiscalYearEnd);
        }

        /// <summary>
        /// THis method provides Fiscal Year Start & End for given date
        /// </summary>
        /// <param name="adtGivenDate">Given Date</param>
        /// <param name="adtFiscalYearStart">Fiscal Year Start</param>
        /// <param name="adtFiscalYearEnd">Fiscal Year End</param>
        public static void GetFiscalYearRangeForGivenDate(DateTime adtGivenDate, ref DateTime adtFiscalYearStart, ref DateTime adtFiscalYearEnd)
        {
            // Throw Exception if GivenDate is MinValue of MaxValue
            if (adtGivenDate == DateTime.MinValue || adtGivenDate == DateTime.MaxValue)
                throw new Exception("Invalid Date. Please pass a valid date to determine fiscal year range.");

            // Get First Day of Fiscal Year
            if (adtGivenDate.Month > knfConstant.FISCAL_YEAR_END_MONTH)
            {
                adtFiscalYearStart = new DateTime(adtGivenDate.Year, knfConstant.FISCAL_YEAR_START_MONTH, knfConstant.FISCAL_YEAR_START_DATE);
                adtFiscalYearEnd = new DateTime(adtGivenDate.Year + 1, knfConstant.FISCAL_YEAR_END_MONTH, knfConstant.FISCAL_YEAR_END_DATE);
            }
            else
            {
                adtFiscalYearStart = new DateTime(adtGivenDate.Year - 1, knfConstant.FISCAL_YEAR_START_MONTH, knfConstant.FISCAL_YEAR_START_DATE);
                adtFiscalYearEnd = new DateTime(adtGivenDate.Year, knfConstant.FISCAL_YEAR_END_MONTH, knfConstant.FISCAL_YEAR_END_DATE);
            }
        }

        /// <summary>
        /// Check if the given date is in a future fiscal year.
        /// </summary>
        /// <param name="adtGivenDate">Given Date</param>
        /// <returns>True if it is in the future; False if not.</returns>
        public static bool IsDateInFutureFiscalYear(DateTime adtGivenDate)
        {
            DateTime ldtCurrentFiscalYearStart = DateTime.MinValue;
            DateTime ldtCurrentFiscalYearEnd = DateTime.MinValue;

            // Get current fiscal year range
            GetCurrentFisalYearRange(ref ldtCurrentFiscalYearStart, ref ldtCurrentFiscalYearEnd);

            if (adtGivenDate > ldtCurrentFiscalYearEnd)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Fiscal Year for the DateTime, extension method
        /// </summary>
        /// <param name="adtGivenDate">this; DateTime</param>
        /// <returns>Fiscal Year</returns>
        public static int FiscalYear(this DateTime adtGivenDate)
        {
            if (adtGivenDate.Month > knfConstant.FISCAL_YEAR_END_MONTH)
            {
                return (adtGivenDate.Year + 1);
            }
            else
            {
                return (adtGivenDate.Year);
            }
        }

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="astrToAddress">To Address</param>
        /// <param name="astrFromAddress">From Address</param>
        /// <param name="astrBody">Message Body</param>
        /// <param name="astrSubject">Subject</param>
        /// <param name="astrSmtpServer">SMTP Server</param>
        /// <param name="astrUserName">User Name</param>
        /// <param name="astrPassword">Password</param>
        /// <param name="aintPort">Port</param>
        public static void SendMail(string astrToAddress, string astrFromAddress, string astrBody, string astrSubject, string astrSmtpServer, string astrUserName, string astrPassword, int aintPort)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(astrToAddress));
            mail.From = new MailAddress(astrFromAddress);
            mail.Subject = astrSubject;
            mail.Body = astrBody;

            SmtpClient lobjSmtpClient = new SmtpClient(astrSmtpServer, aintPort);
            lobjSmtpClient.Credentials = new NetworkCredential(astrUserName, astrPassword);
            lobjSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            lobjSmtpClient.Send(mail);
        }

        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="astrToAddress">To Address</param>
        /// <param name="astrFromAddress">From Address</param>
        /// <param name="astrBody">Message Body</param>
        /// <param name="astrSubject">Subject</param>
        //public static void SendMail(string astrToAddress, string astrFromAddress, string astrBody, string astrSubject)
        //{
        //    string lstrSmtpServer = knfGlobalFunctions.GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.SMTPSetting.SMTP_HOST_PATH);
        //    string lstrUserName = knfGlobalFunctions.GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.SMTPSetting.SMTP_USERNAME);
        //    string lstrPassword = knfGlobalFunctions.GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.SMTPSetting.SMTP_PASSWORD);
        //    int lintPort = Convert.ToInt32(knfGlobalFunctions.GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.SMTPSetting.SMTP_HOST_PORT));

        //    SendMail(astrToAddress, astrFromAddress, astrBody, astrSubject, lstrSmtpServer, lstrUserName, lstrPassword, lintPort);
        //}

        /// <summary>
        /// This method is used to send email.
        /// </summary>
        /// <param name="astrSenderEmailAddress">Sender Email ID</param>
        /// <param name="astrRecipientEmailAddress">Recipient Email ID</param>
        /// <param name="astrSubjectLine">Email Subject Line</param>
        /// <param name="astrBody">Email Body</param>
        /// <param name="aenmEncoding">Email Encode Method</param>
        /// <param name="ablnIsBodyHtml">Email Body in html format flag</param>
        /// <param name="acolAttachments">Email Attachements</param>
        /// <returns></returns>
        //public static utlError SendMail(string astrSenderEmailAddress, string astrRecipientEmailAddress, string astrSubjectLine, string astrBody, System.Text.Encoding aenmEncoding, bool ablnIsBodyHtml, Collection<AttachmentBase> acolAttachments, utlPassInfo autlPassInfo, int aintRecipientID = 0)
        //{
        //    utlError lutlError = new utlError();
        //    try
        //    {
        //        if (astrRecipientEmailAddress.IsNotNullOrEmpty())
        //        {
        //            string[] larrRecipientEmailAddress = astrRecipientEmailAddress.Split(";");
        //            string lstrSmtpServer = knfGlobalFunctions.GetAppConfigValue(autlPassInfo, knfConstant.SMTPSetting.SMTP_HOST_PATH);
        //            string lstrUserName = knfGlobalFunctions.GetAppConfigValue(autlPassInfo, knfConstant.SMTPSetting.SMTP_USERNAME);
        //            string lstrPassword = knfGlobalFunctions.GetAppConfigValue(autlPassInfo, knfConstant.SMTPSetting.SMTP_PASSWORD);

        //            //Default port for SMTP client authentication
        //            SmtpClient serv = new SmtpClient(lstrSmtpServer);
        //            serv.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            serv.UseDefaultCredentials = false;

        //            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
        //            serv.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            foreach (string lstrMaliId in larrRecipientEmailAddress)
        //            {
        //                msg.To.Add(lstrMaliId);
        //            }
        //            msg.Subject = astrSubjectLine;

        //            msg.From = new MailAddress(astrSenderEmailAddress);

        //            if (aintRecipientID != 0)
        //                msg.Headers.Add(knfConstant.Communication.RECIPIENT_COR_TRACKING_ID, aintRecipientID.ToString());

        //            AlternateView lobjView = AlternateView.CreateAlternateViewFromString(astrBody, aenmEncoding, "text/html");
        //            if (acolAttachments.IsNotNull())
        //            {
        //                foreach (AttachmentBase lobjAttachmentBase in acolAttachments)
        //                {
        //                    if (lobjAttachmentBase is LinkedResource)
        //                    {
        //                        lobjView.LinkedResources.Add(lobjAttachmentBase);
        //                    }
        //                    else if (lobjAttachmentBase is Attachment)
        //                    {
        //                        msg.Attachments.Add(lobjAttachmentBase);
        //                    }
        //                }
        //            }

        //            msg.AlternateViews.Add(lobjView);
        //            msg.IsBodyHtml = ablnIsBodyHtml;
        //            using (serv)
        //                serv.Send(msg);
        //            lutlError.istrErrorMessage = string.Empty;
        //            lutlError.istrErrorID = string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.Publish(ex);
        //        lutlError.istrErrorMessage = ex.ToString();
        //        lutlError.istrErrorID = "-1";
        //    }
        //    return lutlError;
        //}

        /// <summary>
        /// This method is used to attach the images used in the email body.
        /// </summary>
        /// <param name="astrBody"></param>
        /// <param name="astrCorGeneratedPath"></param>
        /// <returns></returns>
        public static Collection<AttachmentBase> AttachImages(ref string astrBody, string astrCorGeneratedPath)
        {
            Collection<AttachmentBase> lcolLinkedResources = new Collection<AttachmentBase>();

            try
            {
                if (string.IsNullOrEmpty(astrBody) == false && string.IsNullOrEmpty(astrCorGeneratedPath) == false && string.IsNullOrWhiteSpace(astrCorGeneratedPath) == false)
                {
                    Dictionary<string, string> ldicFindReplace = new Dictionary<string, string>();
                    astrCorGeneratedPath = (astrCorGeneratedPath.EndsWith("\\") == false) ? astrCorGeneratedPath + "\\" : astrCorGeneratedPath;
                    MatchCollection lobjMatches = Regex.Matches(astrBody, _istrImageTagRegex);
                    if (lobjMatches != null && lobjMatches.Count > 0)
                    {
                        foreach (Match lobjMatch in lobjMatches)
                        {
                            string lstrImgAttributes = lobjMatch.Groups[1].Value;
                            string lstrImgSrc = lobjMatch.Groups[2].Value;
                            string[] larrImgSrcSplitup = lstrImgSrc.Split("/");
                            lstrImgSrc = lstrImgSrc.Replace("/", "\\");
                            string lstrAttachmentImagePath = astrCorGeneratedPath + lstrImgSrc;
                            if (File.Exists(lstrAttachmentImagePath))
                            {
                                using (AttachmentBase lobjLinkedResource = new LinkedResource(lstrAttachmentImagePath))
                                {
                                    lobjLinkedResource.ContentId = System.Guid.NewGuid().ToString().Replace("-", "_");
                                    lcolLinkedResources.Add(lobjLinkedResource);
                                    string lstrImageTag = string.Format(_istrImageTagReconstruct, lstrImgAttributes, lobjLinkedResource.ContentId);
                                    ldicFindReplace.Add(lobjMatch.Value, lstrImageTag);
                                }
                            }
                        }
                    }
                    Dictionary<string, string>.Enumerator ldicFindReplaceEnum = ldicFindReplace.GetEnumerator();
                    StringBuilder lstrbBodyText = new StringBuilder(astrBody);
                    while (ldicFindReplaceEnum.MoveNext())
                    {
                        lstrbBodyText.Replace(ldicFindReplaceEnum.Current.Key, ldicFindReplaceEnum.Current.Value);
                    }
                    astrBody = lstrbBodyText.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
            }
            return lcolLinkedResources;
        }

        /// <summary>
        /// Returns the first date of the month of the given datetime value.
        /// </summary>
        /// <param name="adtDateTime">Datetime value.</param>
        /// <returns>First Day of the given datetime value.</returns>
        public static DateTime FirstDayOfMonthFromDateTime(DateTime adtDateTime)
        {
            return new DateTime(adtDateTime.Year, adtDateTime.Month, 1);
        }

        /// <summary>
        /// Returns the last daty of the month of the given datetime value.
        /// </summary>
        /// <param name="adtDateTime">Datetime value.</param>
        /// <returns>Last Day of the given datetime value.</returns>
        public static DateTime LastDayOfMonthFromDateTime(DateTime adtDateTime)
        {
            DateTime ldtFirstDayOfTheMonth = FirstDayOfMonthFromDateTime(adtDateTime);
            return ldtFirstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Toggle Flag
        /// </summary>
        /// <param name="astrFlag">Flag Value</param>
        /// <returns>Toggled Value</returns>
        public static string ToggleFlag(string astrFlag)
        {
            if (astrFlag == knfConstant.FLAG_YES)
                return knfConstant.FLAG_NO;
            else
                return knfConstant.FLAG_YES;
        }

        /// <summary>
        /// This method used to queue the job based on Job Schedule Code.
        /// </summary>
        /// <param name="astrJobScheduleCode"></param>
        //public static void QueueJob(string astrJobScheduleCode)
        //{
        //    if (astrJobScheduleCode.IsNull() || astrJobScheduleCode.Length <= 0)
        //        return;

        //    busJobSchedule lbusJobSchedule = new busJobSchedule() { icdoJobSchedule = new cdoJobSchedule() };
        //    if (lbusJobSchedule.FindJobScheduleByScheduleCode(astrJobScheduleCode))
        //    {
        //        if (lbusJobSchedule.icdoJobSchedule.active_flag == knfConstant.FLAG_YES)
        //        {
        //            if (lbusJobSchedule.iclbJobScheduleDetail.IsNull() ||
        //                    lbusJobSchedule.iclbJobScheduleDetail.Count <= 0)
        //                lbusJobSchedule.LoadJobScheduleDetails();

        //            lbusJobSchedule.CloneJobFromSchedule();
        //        }
        //    }
        //}

        /// <summary>
        /// Validate for strong password.        
        /// </summary>
        /// <param name="astrPassword"></param>
        /// <returns></returns>
        public static bool IsStrongPassword(string astrPassword)
        {
            //Changed the Regular Expression for Strong Password.
            //Changed for PIR 864.
            Regex lrexRegex = new Regex(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@'#.$;%^&+=!""()*,-/:<>?]).*$");
            return (lrexRegex.IsMatch(astrPassword));
        }

        /// <summary>
        /// Difference between two days
        /// </summary>
        /// <param name="aenmInterval">The interval.</param>
        /// <param name="adtFirstDate">First Date.</param>
        /// <param name="adtSecondDate">Second Date.</param>
        /// <returns>Difference</returns>
        public static int DateDiff(enmDateInterval aenmInterval, DateTime adtFirstDate, DateTime adtSecondDate)
        {
            int lintDifference = 0;

            bool lblnIsFirstDateGreater = adtFirstDate > adtSecondDate;
            DateTime ldtGreaterDate = lblnIsFirstDateGreater ? adtFirstDate : adtSecondDate;
            DateTime ldtLesserDate = lblnIsFirstDateGreater ? adtSecondDate : adtFirstDate;

            // Calculate difference
            switch (aenmInterval)
            {
                case enmDateInterval.Day:
                    lintDifference = (ldtGreaterDate - ldtLesserDate).Days;
                    break;
                case enmDateInterval.Month:
                    if ((ldtGreaterDate.Day - ldtLesserDate.Day) >= 0)
                    {
                        lintDifference = (ldtGreaterDate.Year - ldtLesserDate.Year) * knfConstant.MONTHS_IN_YEAR + (ldtGreaterDate.Month - ldtLesserDate.Month) + 1;
                    }
                    else
                    {
                        lintDifference = (ldtGreaterDate.Year - ldtLesserDate.Year) * knfConstant.MONTHS_IN_YEAR + (ldtGreaterDate.Month - ldtLesserDate.Month);
                    }
                    break;
                case enmDateInterval.Year:
                    TimeSpan ltsDifference = ldtGreaterDate - ldtLesserDate;
                    DateTime ldtDifferenceInDateTime = DateTime.MinValue.AddDays(ltsDifference.Days);
                    lintDifference = ldtDifferenceInDateTime.Year - 1;
                    break;
            }

            return lintDifference;
        }

        ///<summary>
        ///Returns the number of days for the month value.
        ///</summary>
        ///<param name="adtDate">Date.</param>
        public static int GetNoDaysForMonth(DateTime adtDate)
        {
            int lintMonth = adtDate.Month;
            int lintDaysInMonth = 0;

            switch (lintMonth)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    lintDaysInMonth = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    lintDaysInMonth = 30;
                    break;
                case 2:
                    lintDaysInMonth = DateTime.IsLeapYear(adtDate.Year) ? 29 : 28;
                    break;
            }

            return lintDaysInMonth;
        }

        /// <summary>
        /// Returns First date of calendar year
        /// </summary>
        /// <param name="aintYear"></param> 
        public static DateTime GetCalendarYearFirstDate(int aintYear)
        {
            return new DateTime(aintYear, 1, 1);
        }

        /// <summary>
        /// Returns Last date of calendar year
        /// </summary>
        /// <param name="aintYear"></param>
        /// <returns></returns>
        public static DateTime GetCalendarYearLastDate(int aintYear)
        {
            return new DateTime(aintYear, 12, 31);
        }

        /// <summary>
        /// This method is used to replace the Cor Tracking Recipient ID and Recipient Mailing Address
        /// in the htm; file which is refered as an Email Body.
        /// </summary>
        /// <param name="astrHtmlText">Email Html Body</param>
        /// <param name="astrStartPattern">Replace word start pattern</param>
        /// <param name="astrEndPattern">Replace word end pattern</param>
        /// <param name="astrReplaceValue">Replace Value</param>
        /// <returns></returns>
        //public static string ReplaceHtmlContent(string astrHtmlText, string astrStartPattern, string astrEndPattern, string astrReplaceValue)
        //{
        //    bool lblnStartPatternFoundFlag = false;
        //    int lintParaIndex = 0;
        //    HtmlDocument lobjHtmlDocument = new HtmlDocument();
        //    lobjHtmlDocument.LoadHtml(astrHtmlText);
        //    HtmlNodeCollection lclbHtmlNodeCollection = lobjHtmlDocument.DocumentNode.SelectNodes("//p");
        //    foreach (HtmlNode lobjParegraphNode in lclbHtmlNodeCollection)
        //    {
        //        if (lobjParegraphNode.InnerText.Contains(astrStartPattern))
        //        {
        //            lblnStartPatternFoundFlag = true;
        //            continue;
        //        }

        //        if (lblnStartPatternFoundFlag == true || lobjParegraphNode.InnerText.Contains(astrEndPattern))
        //        {
        //            HtmlNode lHtmlNodeAnchor = lobjParegraphNode.FirstChild;
        //            if (lintParaIndex == 0 && lHtmlNodeAnchor != null && lHtmlNodeAnchor.FirstChild != null)
        //                lHtmlNodeAnchor.FirstChild.InnerHtml = astrReplaceValue;
        //            else
        //                lobjParegraphNode.Remove();

        //            lintParaIndex++;
        //        }

        //        if (lblnStartPatternFoundFlag == true && lobjParegraphNode.InnerText.Contains(astrEndPattern))
        //        {
        //            break;
        //        }
        //    }
        //    lobjHtmlDocument.DocumentNode.InnerHtml = lobjHtmlDocument.DocumentNode.OuterHtml.Replace(astrStartPattern, string.Empty);
        //    return lobjHtmlDocument.DocumentNode.OuterHtml;
        //}

        /// <summary>
        /// This method is used to get the recipient cor tracking id from the Html Email body.
        /// </summary>
        /// <param name="astrHtmlText">Html Email body</param>
        /// <param name="astrStartPattern">Start Pattern</param>
        /// <param name="astrEndPattern">End Pattern</param>
        /// <returns></returns>
        //public static string GetHtmlContent(string astrHtmlText, string astrStartPattern, string astrEndPattern)
        //{
        //    string lstrResult = string.Empty;
        //    bool lblnStartPatternFoundFlag = false;
        //    int lintParaIndex = 0;
        //    HtmlDocument lobjHtmlDocument = new HtmlDocument();
        //    lobjHtmlDocument.LoadHtml(astrHtmlText);
        //    HtmlNodeCollection lclbHtmlNodeCollection = lobjHtmlDocument.DocumentNode.SelectNodes("//p");
        //    foreach (HtmlNode lobjParegraphNode in lclbHtmlNodeCollection)
        //    {
        //        if (lobjParegraphNode.InnerText.Contains(astrStartPattern))
        //        {
        //            lblnStartPatternFoundFlag = true;
        //            continue;
        //        }

        //        if (lblnStartPatternFoundFlag == true && !lobjParegraphNode.InnerText.Contains(astrEndPattern))
        //        {
        //            if (lintParaIndex == 0 && lobjParegraphNode.FirstChild != null)
        //            {
        //                lstrResult = lobjParegraphNode.FirstChild.InnerHtml;
        //                break;
        //            }
        //        }

        //        if (lblnStartPatternFoundFlag == true && lobjParegraphNode.InnerText.Contains(astrEndPattern))
        //        {
        //            break;
        //        }
        //    }
        //    return lstrResult;
        //}

        /// <summary>
        /// This method is used to send the email asynchronously
        /// </summary>
        /// <param name="astrRecipientEmailAddress">Recipient Email ID</param>
        /// <param name="astrSubjectLine">Subject Line</param>
        /// <param name="astrBody">Email Body</param>
        /// <param name="lenmEncoding">Encoding Method</param>
        /// <param name="ablnIsBodyHtml">Email Body as html flag</param>
        /// <param name="acolAttachment">Collection of attachements</param>
        /// <param name="aobjCallbackFunction">Callback Method</param>
        /// <param name="autlPassInfo">Pass info object</param>
        //public static void SendEmailAsync(string astrRecipientEmailAddress, string astrSubjectLine, string astrBody, System.Text.Encoding lenmEncoding, bool ablnIsBodyHtml, Collection<AttachmentBase> acolAttachment, utlPassInfo autlPassInfo, int aintRecipientID = 0)
        //{
        //    SendEmailDelegate ldlgtSendEmailAsync = new SendEmailDelegate(SendEmail);
        //    ldlgtSendEmailAsync.BeginInvoke(astrRecipientEmailAddress, astrSubjectLine, astrBody, lenmEncoding, ablnIsBodyHtml, acolAttachment, autlPassInfo, aintRecipientID, new AsyncCallback(AsyncEmailResult), ldlgtSendEmailAsync);
        //}

        /// <summary>
        /// This method is used as the AsuncCall back method for sending an email.
        /// </summary>
        /// <param name="ar"></param>
        //public static void AsyncEmailResult(IAsyncResult ar)
        //{
        //    if (ar.IsCompleted)
        //    {
        //        SendEmailDelegate dlg = (SendEmailDelegate)ar.AsyncState;
        //        utlError lutlError = dlg.EndInvoke(ar);
        //        if (string.IsNullOrWhiteSpace(lutlError.istrErrorID) == false)
        //        {
        //            ExceptionManager.Publish(new Exception(lutlError.istrErrorMessage));
        //        }
        //    }
        //}

        /// <summary>
        /// This method is used to send the email for the main template and send the enclosures as attachements.
        /// </summary>
        /// <param name="astrRecipientEmailAddress"></param>
        /// <param name="astrSubjectLine"></param>
        /// <param name="astrBody"></param>
        /// <param name="aenmEncoding"></param>
        /// <param name="ablnIsBodyHtml"></param>
        /// <param name="acolAttachments"></param>
        /// <param name="autlPassInfo"></param>
        /// <returns></returns>
        //public static utlError SendEmail(string astrRecipientEmailAddress, string astrSubjectLine, string astrBody, System.Text.Encoding aenmEncoding, bool ablnIsBodyHtml, Collection<AttachmentBase> acolAttachments, utlPassInfo autlPassInfo, int aintRecipientID = 0)
        //{
        //    string lstrSenderEmailAddress = autlPassInfo.isrvDBCache.GetConstantValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SENDER_EMAIL_ADDRESS).ToString();
        //    return knfGlobalFunctions.SendMail(lstrSenderEmailAddress, astrRecipientEmailAddress, astrSubjectLine, astrBody, aenmEncoding, ablnIsBodyHtml, acolAttachments, autlPassInfo, aintRecipientID);
        //}

        #region [Job Scheduler Email Methods]
        /// <summary>
        /// Sends the Email to the Recipient Lists with the batch errors
        /// </summary>
        /// <param name="aintJobScheduleId"></param>
        /// <param name="astrError"></param>
        //public static void SendBatchErrorEmails(int aintJobScheduleId, utlPassInfo iobjPassInfo, string astrError = "")
        //{
        //    busJobSchedule lbusJobSchedule = new busJobSchedule();
        //    if (lbusJobSchedule.FindJobSchedule(aintJobScheduleId))
        //    {
        //        string[] larrErrEmailRecipientGroups = null;
        //        if (lbusJobSchedule.icdoJobSchedule != null && string.IsNullOrWhiteSpace(lbusJobSchedule.icdoJobSchedule.batch_error_email_recipient_groups) == false)
        //        {
        //            const string lstrErrorTemplate = "The following error was received: {1}.";
        //            const string lstrSubjectLineTemplate = "Job Schedule \"{0}\" has encountered errors while processing";
        //            const string lstrMessageTemplate = "The \"{0}\" job schedule has encountered errors while processing.";

        //            string lstrSubjectLine = string.Format(lstrSubjectLineTemplate, lbusJobSchedule.icdoJobSchedule.schedule_name);
        //            string lstrMessage = string.Format(lstrMessageTemplate, lbusJobSchedule.icdoJobSchedule.schedule_name);
        //            if (astrError.IsNotNullOrEmpty())
        //            {
        //                lstrMessage = string.Format(lstrMessageTemplate + " " + lstrErrorTemplate, lbusJobSchedule.icdoJobSchedule.schedule_name, astrError);
        //            }

        //            larrErrEmailRecipientGroups = lbusJobSchedule.icdoJobSchedule.batch_error_email_recipient_groups.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        //            if (iobjPassInfo.isrvDBCache.GetConstantValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANT_USE_SYSTEM_EMAIL) == knfConstant.FLAG_YES)
        //            {
        //                SendEmailAsync(iobjPassInfo.isrvDBCache.GetConstantValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANT_RECIPIENT_PERSONAL_EMAIL), lstrSubjectLine, lstrMessage, System.Text.Encoding.UTF8, false, null, utlPassInfo.iobjPassInfo);
        //            }
        //            else if (larrErrEmailRecipientGroups != null && larrErrEmailRecipientGroups.Length > 0)
        //            {
        //                foreach (string lstrRecipient in larrErrEmailRecipientGroups)
        //                {
        //                    SendEmailAsync(lstrRecipient, lstrSubjectLine, lstrMessage, System.Text.Encoding.UTF8, false, null, utlPassInfo.iobjPassInfo);
        //                }
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// Sends the Email to User for respective Job ScheduleId
        /// </summary>
        /// <param name="aintJobScheduleId"></param>
        //public static void SendBatchUserCancellationEmails(int aintJobScheduleId, utlPassInfo iobjPassInfo)
        //      {
        //          busJobSchedule lbusJobSchedule = new busJobSchedule();
        //          if (lbusJobSchedule.FindJobSchedule(aintJobScheduleId))
        //          {
        //              string[] larrErrEmailRecipientGroups = null;
        //              if (lbusJobSchedule.icdoJobSchedule != null && string.IsNullOrWhiteSpace(lbusJobSchedule.icdoJobSchedule.batch_error_email_recipient_groups) == false)
        //              {
        //                  const string lstrMessageTemplate = "Job Schedule \"{0}\" cancelled by user request";
        //                  const string lstrSubjectLine = "Job Schedule Cancelled";
        //                  string lstrMessage = string.Format(lstrMessageTemplate, lbusJobSchedule.icdoJobSchedule.schedule_name);

        //                  larrErrEmailRecipientGroups = lbusJobSchedule.icdoJobSchedule.batch_error_email_recipient_groups.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        //                  if (iobjPassInfo.isrvDBCache.GetConstantValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANT_USE_SYSTEM_EMAIL) == knfConstant.FLAG_YES)
        //                  {
        //                      SendEmailAsync(iobjPassInfo.isrvDBCache.GetConstantValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANT_RECIPIENT_PERSONAL_EMAIL), lstrSubjectLine, lstrMessage, System.Text.Encoding.UTF8, false, null, utlPassInfo.iobjPassInfo);
        //                  }
        //                  else if (larrErrEmailRecipientGroups != null && larrErrEmailRecipientGroups.Length > 0)
        //                  {
        //                      foreach (string lstrRecipient in larrErrEmailRecipientGroups)
        //                      {
        //                          SendEmailAsync(lstrRecipient, lstrSubjectLine, lstrMessage, System.Text.Encoding.UTF8, false, null, utlPassInfo.iobjPassInfo);
        //                      }
        //                  }
        //              }
        //          }
        //      }
        /// <summary>
        /// Sends Batch Completion Email to User
        /// </summary>
        /// <param name="aintJobScheduleId"></param>
        /// <param name="aintProcessedRecordCount"></param>
        //public static void SendBatchCompletionEmails(int aintJobScheduleId, utlPassInfo iobjPassInfo, int aintProcessedRecordCount = 0)
        //      {
        //          busJobSchedule lbusJobSchedule = new busJobSchedule();
        //          if (lbusJobSchedule.FindJobSchedule(aintJobScheduleId))
        //          {
        //              string[] larrEmailRecipientGroups = null;
        //              if (lbusJobSchedule.icdoJobSchedule != null && string.IsNullOrWhiteSpace(lbusJobSchedule.icdoJobSchedule.batch_status_email_recipient_groups) == false)
        //              {
        //                  const string lstrRecordCountTemplate = "The job schedule processed {2} records.";
        //                  const string lstrSubjectLineTemplate = "Processing has completed for the \"{0}\" job schedule";
        //                  const string lstrMessageTemplate = "The \"{0}\" job schedule has completed processing at {1}.";

        //                  //Format the email subject line and body.
        //                  //TODO: Replace the number of records processed in the job.
        //                  string lstrSubjectLine = string.Format(lstrSubjectLineTemplate, lbusJobSchedule.icdoJobSchedule.schedule_name);
        //                  string lstrMessage = string.Format(lstrMessageTemplate, lbusJobSchedule.icdoJobSchedule.schedule_name, knfGlobalFunctions.ApplicationDateTime.ToString());

        //                  //PIR 466 : 
        //                  //if (aintProcessedRecordCount > 0)
        //                  //{
        //                  lstrMessage = string.Format(lstrMessageTemplate + " " + lstrRecordCountTemplate, lbusJobSchedule.icdoJobSchedule.schedule_name, knfGlobalFunctions.ApplicationDateTime.ToString(), aintProcessedRecordCount.ToString());
        //                  //}

        //                  larrEmailRecipientGroups = lbusJobSchedule.icdoJobSchedule.batch_status_email_recipient_groups.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        //                  if (iobjPassInfo.isrvDBCache.GetConstantValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANT_USE_SYSTEM_EMAIL) == knfConstant.FLAG_YES)
        //                  {
        //                      SendEmailAsync(iobjPassInfo.isrvDBCache.GetConstantValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANT_RECIPIENT_PERSONAL_EMAIL), lstrSubjectLine, lstrMessage, System.Text.Encoding.UTF8, false, null, utlPassInfo.iobjPassInfo);
        //                  }
        //                  else if (larrEmailRecipientGroups != null && larrEmailRecipientGroups.Length > 0)
        //                  {
        //                      foreach (string lstrRecipient in larrEmailRecipientGroups)
        //                      {
        //                          SendEmailAsync(lstrRecipient, lstrSubjectLine, lstrMessage, System.Text.Encoding.UTF8, false, null, utlPassInfo.iobjPassInfo);
        //                      }
        //                  }
        //              }
        //          }
        //      }
        /// <summary>
        /// Sends the Email to User once the File is processed 
        /// </summary>
        /// <param name="acdoFileHdr"></param>
        //public static void SendFileProcessedEmails(cdoFileHdr acdoFileHdr)
        //{
        //    busFile lbusFile = new busFile();

        //    if (lbusFile.FindFile(acdoFileHdr.file_id))
        //    {
        //        string[] larrEmailRecipientGroups = null;
        //        if (lbusFile.icdoFile != null && string.IsNullOrWhiteSpace(lbusFile.icdoFile.email_notification) == false)
        //        {
        //            const string lstrSubjectLineTemplate = "\"{0}\" processed with a file status of {1}";
        //            const string lstrMessageTemplate = "The \"{0}\" with the file name of \"{1}\" has completed processing at {2} with a file status of {3}.";

        //            //Format the email subject line and body.
        //            //TODO: Replace the number of records processed in the job.
        //            string lstrSubjectLine = string.Format(lstrSubjectLineTemplate, lbusFile.icdoFile.description, acdoFileHdr.status_description);
        //            string lstrMessage = string.Format(lstrMessageTemplate, lbusFile.icdoFile.description, acdoFileHdr.processed_file_name, knfGlobalFunctions.ApplicationDateTime.ToString(), acdoFileHdr.status_description);

        //            larrEmailRecipientGroups = lbusFile.icdoFile.email_notification.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        //            if (larrEmailRecipientGroups != null && larrEmailRecipientGroups.Length > 0)
        //            {
        //                foreach (string lstrRecipient in larrEmailRecipientGroups)
        //                {
        //                    SendEmailAsync(lstrRecipient, lstrSubjectLine, lstrMessage, System.Text.Encoding.UTF8, false, null, utlPassInfo.iobjPassInfo);
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion [Methods Used For Sending Emails thru job scheduler]

        /// <summary>        
        /// This method is used to encrypt the source file using PGP Cryptographt and return the File Info object
        /// of encrypted source file
        /// </summary>
        /// <param name="astrSourceFile">Source file path</param>
        /// <param name="astrEncryptedFile">Encrypted File path</param>
        /// <returns></returns>
        //public static FileInfo EncryptFile(string astrSourceFile, string astrEncryptedFile)
        //{
        //    string lstrKeyUserId = GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.GNUPgpSetting.GNUPGP_KEY_USER_ID);
        //    string lstrAppPath = GetAppConfigValue(utlPassInfo.iobjPassInfo, knfConstant.GNUPgpSetting.GNUPGP_APP_PATH);

        //    //Create PgpEncryptionService object and encrypt the source file into the destination file.
        //    clsEncryptionService lobjPgpEncryptionService = new clsEncryptionService(lstrAppPath);
        //    return lobjPgpEncryptionService.EncryptFile(lstrKeyUserId, astrSourceFile, astrEncryptedFile);
        //}

        /// <summary>
        /// This method is used to decrypt the source file [Encrypted using the PGP Cryptography] and return the FileInfo object for the
        /// Decrypted file
        /// </summary>
        /// <param name="astrEncryptedSourceFile">Encrypted Source File path</param>
        /// <param name="astrDecryptedFile">Decrypted File path</param>
        /// <returns></returns>
        //public static FileInfo DecryptFile(string astrEncryptedSourceFile, string astrDecryptedFile)
        //{
        //    string lstrAppPath = HelperFunction.GetAppSettings(knfConstant.GNUPgpSetting.GNUPGP_APP_PATH);

        //    //Create PgpEncryptionService object and descypt the source file into the destination file.
        //    clsEncryptionService lobjPgpEncryptionService = new clsEncryptionService(lstrAppPath);
        //    return lobjPgpEncryptionService.DecryptFile(astrEncryptedSourceFile, astrDecryptedFile);
        //}

        /// <summary>
        /// This method is used to encrypt the source file using AES Cryptography and return the File Info object of encrypted source file.
        /// </summary>
        /// <param name="astrSourceFile"></param>
        /// <param name="astrEncryptedFile"></param>
        /// <returns>FileInfo</returns>
        //public static FileInfo AesEncryptFile(string astrSourceFile, string astrEncryptedFile)
        //{
        //    if (HelperUtil.GetData1ByCodeValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANTS_AND_VARIABLES_CODE_ID, knfConstant.Common.Codes.SystemConstantsAndVariables.AES_ENCRYPTION_FLAG) != knfConstant.FLAG_YES)
        //    {
        //        clsAesEncryptionService lobjAesEncryptionService = new clsAesEncryptionService();
        //        return lobjAesEncryptionService.EncryptFile(astrSourceFile, astrEncryptedFile);
        //    }

        //    return new FileInfo(astrEncryptedFile);
        //}

        /// <summary>
        /// This method is used to decrypt the source file [Encrypted using the AES Cryptography] and return the FileInfo object for the Decrypted file.
        /// </summary>
        /// <param name="astrEncryptedSourceFile"></param>
        /// <param name="astrDecryptedFile"></param>
        /// <returns>FileInfo</returns>
        //public static FileInfo AesDecryptFile(string astrEncryptedSourceFile, string astrDecryptedFile)
        //{
        //    if (HelperUtil.GetData1ByCodeValue(knfConstant.Common.Codes.SystemConstantsAndVariables.SYSTEM_CONSTANTS_AND_VARIABLES_CODE_ID, knfConstant.Common.Codes.SystemConstantsAndVariables.AES_ENCRYPTION_FLAG) != knfConstant.FLAG_YES)
        //    {
        //        clsAesEncryptionService lobjAesEncryptionService = new clsAesEncryptionService();

        //        return lobjAesEncryptionService.DecryptFile(astrEncryptedSourceFile, astrDecryptedFile);
        //    }

        //    return new FileInfo(astrDecryptedFile);
        //}

        /// <summary>
        /// Function to get Code Value data for the given codeId and codeValue combination
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <param name="astrCodeValue"></param>
        /// <returns>Code Value Object</returns>
        //public static cdoCodeValue GetCodeValueDetails(int aintCodeId, string astrCodeValue)
        //{
        //    cdoCodeValue lcdoCodeValue = null;
        //    busMainBase lbusMainbase = new busMainBase();
        //    DataTable ldtbList = lbusMainbase.iobjPassInfo.isrvDBCache.GetCacheData(knfConstant.SGS_CODE_VALUE, string.Format(knfConstant.CODE_ID_AND_VALUE_EQUALS, aintCodeId, astrCodeValue));
        //    if (ldtbList.Rows.Count > 0)
        //    {
        //        lcdoCodeValue = new cdoCodeValue();
        //        lcdoCodeValue.LoadData(ldtbList.Rows[0]);
        //    }
        //    return lcdoCodeValue;
        //}

        //public static Collection<cdoCodeValue> GetCodeValues(int aintCodeId)
        //{
        //    Collection<cdoCodeValue> lclcCodeValues = new Collection<cdoCodeValue>();
        //    busMainBase lbusMainbase = new busMainBase();
        //    DataTable ldtbList = lbusMainbase.iobjPassInfo.isrvDBCache.GetCodeValues(aintCodeId);

        //    if (ldtbList.Rows.Count > 0)
        //    {
        //        lclcCodeValues = doBase.GetCollection<cdoCodeValue>(ldtbList);
        //    }
        //    return lclcCodeValues;
        //}

        //public static Collection<busCodeValue> GetCodeValueData(int aintCodeId, string astrCodeValue)
        //{
        //    Collection<busCodeValue> lclbCodeValues = new Collection<busCodeValue>();
        //    busMainBase lbusMainbase = new busMainBase();
        //    //DataTable ldtbList = lbusMainbase.iobjPassInfo.isrvDBCache.GetCodeValues(aintCodeId);
        //    DataTable ldtbList = lbusMainbase.iobjPassInfo.isrvDBCache.GetCacheData(knfConstant.SGS_CODE_VALUE_DATA, string.Format(knfConstant.CODE_ID_AND_VALUE_EQUALS, aintCodeId, astrCodeValue));
        //    Collection<busCodeValue> lclbCodeValueData = new Collection<busCodeValue>();
        //    if (ldtbList.Rows.Count > 0)
        //    {
        //        lclbCodeValues = lbusMainbase.GetCollection<busCodeValue>(ldtbList);
        //        busCodeValue lbusCodeValue = new busCodeValue();
        //        lbusCodeValue = lclbCodeValues[0];

        //        foreach (busCodeValue lbusCodeVal in lclbCodeValues)
        //        {
        //            lbusCodeVal.istrPropertyName = Convert.ToString(ldtbList.Rows[0]["PROP_NAME"]);
        //            lbusCodeVal.istrPropertyValue = Convert.ToString(ldtbList.Rows[0]["PROP_VALUE"]);
        //            lclbCodeValueData.Add(lbusCodeVal);
        //        }

        //        //DataTable ldtbList1 = lbusMainbase.iobjPassInfo.isrvDBCache.GetCacheData(knfConstant.SGS_CODE_VALUE_DATA, string.Format(knfConstant.CODE_SERIAL_ID_EQUALS, lbusCodeValue.icdoCodeValue.code_serial_id));
        //        //Collection<busCodeValueData> lclbCodeValueData = new Collection<busCodeValueData>();
        //        //lclbCodeValueData = lbusMainbase.GetCollection<busCodeValueData>(ldtbList1);
        //        //lbusCodeValue.iclbCodeValueData.AddRange(lclbCodeValueData);
        //    }
        //    return lclbCodeValueData;
        //}

        /// <summary>
        /// To Calculate Exact Date Diffrence in Months and days
        /// </summary>
        /// <param name="adtDateValue1"></param>
        /// <param name="adtDateValue2"></param>
        /// <returns></returns>
        public static void DateDiffInMonthsAndDays(DateTime adtDateValue1, DateTime adtDateValue2, out int aintMonths, out int aintDays)
        {
            aintDays = 0;
            aintMonths = 0;
            DateTime adteStartDate, adteEndDate;

            if (adtDateValue1 > adtDateValue2)
            {
                adteEndDate = adtDateValue1;
                adteStartDate = adtDateValue2;
            }
            else if (adtDateValue1 < adtDateValue2)
            {
                adteEndDate = adtDateValue2;
                adteStartDate = adtDateValue1;
            }
            else
            {
                adteEndDate = DateTime.MinValue;
                adteStartDate = DateTime.MinValue;
            }


            int iintStartDay = adteStartDate.Day;
            aintMonths = 0;
            while (true)
            {
                if (adteStartDate.Day < iintStartDay)
                {
                    if (DateTime.DaysInMonth(adteStartDate.Year, adteStartDate.Month) < iintStartDay)
                    {
                        adteStartDate = new DateTime(adteStartDate.Year, adteStartDate.Month, DateTime.DaysInMonth(adteStartDate.Year, adteStartDate.Month));
                    }
                    else if (DateTime.DaysInMonth(adteStartDate.Year, adteStartDate.Month) > iintStartDay)
                    {
                        adteStartDate = new DateTime(adteStartDate.Year, adteStartDate.Month, iintStartDay);
                    }
                }

                if ((adteStartDate = adteStartDate.AddMonths(1)) <= adteEndDate)
                {
                    aintMonths++;
                }

                else
                {
                    adteStartDate = adteStartDate.AddMonths(-1);
                    break;
                }
            }
            aintDays = (adteEndDate - adteStartDate).Days;
        }


        #region [PGP Crptography]
        #endregion


        /// <summary>        
        /// Fills the mailbox, staging, process and error pathcode dropdowns
        /// </summary>
        /// <returns></returns>       
        //public static string GetAppConfigValue(utlPassInfo autlPassInfo, string astrConfigKey)
        //{
        //    string lstrResult = string.Empty;
        //    DataTable ldtbAppConfig = autlPassInfo.isrvDBCache.GetCacheData("sgt_app_config", "config_key = '" + astrConfigKey + "'");
        //    if (ldtbAppConfig.IsNotNull() && ldtbAppConfig.Rows.Count > 0)
        //    {
        //        lstrResult = Convert.ToString(ldtbAppConfig.Rows[0][enmAppConfig.config_value.ToString()]);
        //    }
        //    return lstrResult;
        //}

        /// <summary>
        /// Get private key file for SFTP server from configuration table
        /// </summary>
        /// <param name="autlPassInfo"></param>
        /// <param name="astrConfigKey"></param>
        /// <returns></returns>
        //public static byte[] GetAssociatedFile(utlPassInfo autlPassInfo, string astrConfigKey)
        //{
        //    byte[] larrFile = null;
        //    DataTable ldtbAppConfig = autlPassInfo.isrvDBCache.GetCacheData("sgt_app_config", "config_key = '" + astrConfigKey + "'");
        //    if (ldtbAppConfig.IsNotNull() && ldtbAppConfig.Rows.Count > 0)
        //    {
        //        larrFile = (byte[])ldtbAppConfig.Rows[0][enmAppConfig.associated_file.ToString()];
        //    }
        //    return larrFile;
        //}

        /// <summary>
        /// Moving the file in to SFTP server
        /// </summary>
        /// <param name="astrUploadFile"> Upload file path</param>
        /// <param name="aintSFTPPort"> SFTP server port no </param>
        /// <param name="astrSFTPHost">SFTP server host name</param>
        /// <param name="astrSFTPUserName"> SFTP server user name</param>
        /// <param name="astrSFTPWorkingDirectory">SFTP server direcctory in to which the upload file need to move</param>
        /// <returns></returns>
        //public static bool UploadSFTPFile(string astrUploadFile, int aintSFTPPort, string astrSFTPHost, string astrSFTPUserName, string astrSFTPWorkingDirectory)
        //{
        //    bool lblnResult = false;

        //    Stream lstmStream = new MemoryStream(GetAssociatedFile(utlPassInfo.iobjPassInfo, knfConstant.SFTPServerSettings.SFTP_SERVER_KEY_FILE));
        //    PrivateKeyFile lPrivateKeyFile = new PrivateKeyFile(lstmStream);
        //    SftpClient client = new SftpClient(astrSFTPHost, aintSFTPPort, astrSFTPUserName, lPrivateKeyFile);
        //    using (client)
        //    {
        //        client.Connect();
        //        client.ChangeDirectory(astrSFTPWorkingDirectory);
        //        var listDirectory = client.ListDirectory(astrSFTPWorkingDirectory);
        //        using (var fileStream = new FileStream(astrUploadFile, FileMode.Open))
        //        {
        //            client.BufferSize = 4 * 1024; // bypass Payload error large files
        //            client.UploadFile(fileStream, Path.GetFileName(astrUploadFile));
        //        }
        //    }
        //    return lblnResult;
        //}

        /// <summary>
        /// Method to clear Mailbox files
        /// </summary>
        /// <param name="astrSystemPathCode"></param>
        //public static void ClearMailboxFiles(string astrSystemPathCode)
        //{
        //    busMainBase lbusMainbase = new busMainBase();
        //    string lstrMailBoxFileName = lbusMainbase.iobjPassInfo.isrvDBCache.GetPathInfo(astrSystemPathCode);

        //    DirectoryInfo ldMailbox = new DirectoryInfo(lstrMailBoxFileName);

        //    foreach (FileInfo file in ldMailbox.GetFiles())
        //    {
        //        file.Delete();
        //    }
        //}

        /// <summary>
        /// Queue job with its correspoinding parameter values
        /// </summary>
        /// <param name="astrScheduleCode"></param>
        /// <param name="ahstParams"></param>

        //public static void QueueJobWithParameters(string astrScheduleCode, Hashtable ahstParams)
        //{
        //    busJobSchedule lbusJobSchedule = new busJobSchedule();
        //    if (lbusJobSchedule.FindJobScheduleByScheduleCode(astrScheduleCode))
        //    {
        //        Collection<clsJobScheduleDetailsParameters> lcolJobScheduleDetailsParameters = lbusJobSchedule.GetScheduleParameters();

        //        foreach (clsJobScheduleDetailsParameters lclsJobScheduleDetailsParameters in lcolJobScheduleDetailsParameters)
        //        {
        //            foreach (busJobParameters lbusJobParameters in lclsJobScheduleDetailsParameters.iclbParameters)
        //            {
        //                foreach (DictionaryEntry ldictParamName in ahstParams)
        //                {
        //                    if (lbusJobParameters.icdoJobParameters.param_name == ldictParamName.Key.ToString())
        //                    {
        //                        lbusJobParameters.icdoJobParameters.param_value = ldictParamName.Value.ToString();
        //                    }
        //                }
        //            }
        //        }
        //        lbusJobSchedule.RunJobInImmediateMode(lcolJobScheduleDetailsParameters);
        //    }
        //}

        /// <summary>
        /// Get Current on going payroll schedule
        /// </summary>
        /// <param name="astrPayrollType"></param>
        /// <returns></returns>
        //public static busPayrollSchedule GetCurrentPayrollSchedule(string astrPayrollType)
        //{
        //    busPayrollSchedule lbusPayrollSchedule = new busPayrollSchedule { icdoPayrollSchedule = new doPayrollSchedule() };

        //    DataTable ldtbPayroll = busBase.Select("entPayrollSchedule.GetCurrentPayroll", new object[1] { astrPayrollType });
        //    if (ldtbPayroll.IsNotNull() && ldtbPayroll.Rows.Count > 0)
        //    {
        //        lbusPayrollSchedule.icdoPayrollSchedule.LoadData(ldtbPayroll.Rows[0]);
        //    }
        //    return lbusPayrollSchedule;

        //}

        /// <summary>
        /// Check whether Health Dependant Age reaching 65 or not
        /// </summary>
        /// <param name="adtDateOfBirth"></param>
        /// <param name="adtAgeCalculationDate"></param>
        /// <returns></returns>
        public static bool IsHealthDependentReaching65(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate)
        {
            int lintMonths = 0;
            int lintYears = 0;
            int lintDaysInBdayMonth = DateTime.DaysInMonth(adtDateOfBirth.Year, adtDateOfBirth.Month);
            int lintDaysRemain = adtAgeCalculationDate.Day + (lintDaysInBdayMonth - adtDateOfBirth.Day);
            if (adtAgeCalculationDate.Month > adtDateOfBirth.Month)
            {
                lintYears = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
                lintMonths = adtAgeCalculationDate.Month - (adtDateOfBirth.Month + 1) + Math.Abs(lintDaysRemain / lintDaysInBdayMonth);
            }
            else if (adtAgeCalculationDate.Month == adtDateOfBirth.Month)
            {
                if (adtAgeCalculationDate.Day >= adtDateOfBirth.Day)
                {
                    lintYears = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
                    lintMonths = knfConstant.ZERO;
                }
                else
                {
                    lintYears = (adtAgeCalculationDate.Year - 1) - adtDateOfBirth.Year;
                    lintMonths = knfConstant.ELEVEN;
                }
            }
            else
            {
                lintYears = (adtAgeCalculationDate.Year - 1) - adtDateOfBirth.Year;
                lintMonths = adtAgeCalculationDate.Month + (11 - adtDateOfBirth.Month) + Math.Abs(lintDaysRemain / lintDaysInBdayMonth);
            }

            if (lintYears == 64 && lintMonths >= 6)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if user has access to Payment Administrator Resource
        /// </summary>
        /// <returns></returns>
        //public static bool IsUserHaveAccessToResourcePaymentAdministrator(utlPassInfo iobjPassInfo)
        //{
        //    bool lblnResult = false;
        //    int lintSecurityValue = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoSecurity.GetUserAccessByResource",
        //                                         new object[2] { iobjPassInfo.iintUserSerialID, knfConstant.RESOURCE_ID_PAYMENT_ADMINISTRATOR },
        //                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
        //    if (lintSecurityValue == 5)
        //    {
        //        return lblnResult = true;
        //    }
        //    return lblnResult;
        //}

        /// <summary>
        /// Checks whether the SSN is valid or not.
        /// </summary>
        /// <param name="astrSSN">SSN to validate</param>
        /// <returns>True, if valid otherwise false</returns>
        public static bool IsValidSSN(string astrSSN)
        {
            Regex lrexSSN = new Regex("^[0-9]*$");
            return astrSSN.IsNotNullOrEmpty() && astrSSN.Length == 9 && lrexSSN.IsMatch(astrSSN);
        }
        #endregion

        #region Extension Methods

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
               

        /// <summary>
        /// To Remove Html Tags from string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string source)
        {
            return Regex.Replace(source, "<.*?>|&.*?;", string.Empty);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data, string tableName)
        {
            DataTable table = new DataTable(tableName);

            //special handling for value types and string
            if (typeof(T).IsValueType || typeof(T).Equals(typeof(string)))
            {

                DataColumn dc = new DataColumn("Value");
                table.Columns.Add(dc);
                foreach (T item in data)
                {
                    DataRow dr = table.NewRow();
                    dr[0] = item;
                    table.Rows.Add(dr);
                }
            }
            else
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                {
                    table.Columns.Add(prop.Name,
                    Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        try
                        {
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        }
                        catch (Exception ex)
                        {
                            row[prop.Name] = DBNull.Value;
                        }
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public static DataTable ToDataTable<T>(this Collection<T> data, string tableName)
        {
            DataTable table = new DataTable(tableName);

            //special handling for value types and string
            if (typeof(T).IsValueType || typeof(T).Equals(typeof(string)))
            {

                DataColumn dc = new DataColumn("Value");
                table.Columns.Add(dc);
                foreach (T item in data)
                {
                    DataRow dr = table.NewRow();
                    dr[0] = item;
                    table.Rows.Add(dr);
                }
            }
            else
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                {
                    table.Columns.Add(prop.Name,
                    Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        try
                        {
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        }
                        catch (Exception ex)
                        {
                            row[prop.Name] = DBNull.Value;
                        }
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> GetSlices<T>(this IEnumerable<T> source, int n)
        {
            IEnumerable<T> it = source;
            T[] slice = it.Take(n).ToArray();
            it = it.Skip(n);
            while (slice.Length != 0)
            {
                yield return slice;
                slice = it.Take(n).ToArray();
                it = it.Skip(n);
            }
        }

        /// <summary>
        /// Adding Collection To Other Collection Of Same type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oc"></param>
        /// <param name="collection"></param>
        public static void AddRange<T>(this Collection<T> oc, Collection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            foreach (var item in collection)
            {
                oc.Add(item);
            }
        }
        /// <summary>
        /// Formats a string with a list of literal placeholders.
        /// </summary>
        /// <param name="astrText">The extension text</param>
        /// <param name="aarrArgs">The argument list</param>
        /// <returns>The formatted string</returns>
        public static string Format(this string astrText, params object[] aarrArgs)
        {
            return string.Format(astrText, aarrArgs);
        }

        /// <summary>
        /// Determines whether a substring exists within a string.
        /// </summary>
        /// <param name="astrText">String to search.</param>
        /// <param name="astrSubString">Substring to match when searching.</param>
        /// <param name="ablnCaseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>Indicator of substring presence within the string.</returns>
        public static bool Contains(this string astrText, string astrSubString, bool ablnCaseSensitive)
        {
            if (ablnCaseSensitive)
            {
                return astrText.Contains(astrSubString);
            }
            else
            {
                return astrText.ToLower().IndexOf(astrSubString.ToLower(), 0) >= 0;
            }
        }

        /// <summary>
        /// Detects if a string can be parsed to a valid date.
        /// </summary>
        /// <param name="astrText">Value to inspect.</param>
        /// <returns>Whether or not the string is formatted as a date.</returns>
        public static bool IsDate(this string astrText)
        {
            try
            {
                System.DateTime dtDateTime = System.DateTime.Parse(astrText);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Function checks whether the given date is valid as per the specified format.
        /// </summary>
        /// <param name="adtDateString">Date to be validated</param>
        /// <param name="format">Date format</param>
        /// <returns></returns>
        public static bool IsValidDate(this string adtDateString, string format)
        {
            DateTime ldtDateString;
            return DateTime.TryParseExact(adtDateString, format, new CultureInfo("en-US"), DateTimeStyles.None, out ldtDateString)
                   && ldtDateString != DateTime.MinValue && ldtDateString != DateTime.MaxValue;
        }

        public static bool IsDefault(this DateTime adtGivenDate)
        {
            if (adtGivenDate.Equals(default(DateTime)))
            {
                return true;
            }
            return false;
        }


        public static void RemoveRange<T>(this Collection<T> list, IEnumerable aList)
        {
            if (!aList.IsNullOrEmpty())
            {
                foreach (T obj in aList)
                {
                    list.Remove(obj);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified string is null or empty.
        /// </summary>
        /// <param name="astrText">The string value to check.</param>
        /// <returns>Boolean indicating whether the string is null or empty.</returns>
        public static bool IsEmpty(this string astrText)
        {
            return (astrText == null) || (astrText.Length == 0);
        }

        /// <summary>
        /// Determines whether the specified string is not null or empty.
        /// </summary>
        /// <param name="astrText">The string value to check.</param>
        /// <returns>Boolean indicating whether the string is not empty</returns>
        public static bool IsNotEmpty(this string astrText)
        {
            return (!astrText.IsEmpty());
        }

        /// <summary>
        /// Checks whether the string is null and returns a default value in case.
        /// </summary>
        /// <param name="astrDefaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfNull(this string astrText, string astrDefaultValue)
        {
            return (astrText.IsNull() ? astrText : astrDefaultValue);
        }

        /// <summary>
        /// Determines whether the string is empty and returns a default value in case.
        /// </summary>
        /// <param name="astrDefaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfEmpty(this string astrText, string astrDefaultValue)
        {
            return (astrText.IsNotEmpty() ? astrText : astrDefaultValue);
        }

        /// <summary>
        /// Determines whether the specified object is null
        /// </summary>
        /// <returns>Boolean indicating whether the object is null</returns>
        public static bool IsNull(this object aobjObject)
        {
            return object.ReferenceEquals(aobjObject, null);
        }

        /// <summary>
        /// Determines whether the specified object is not null
        /// </summary>
        /// <returns>Boolean indicating whether the object is not null</returns>
        public static bool IsNotNull(this object aobjObject)
        {
            return !object.ReferenceEquals(aobjObject, null);
        }


        /// <summary>
        /// Creates a type from the given name
        /// </summary>
        /// <typeparam name="T">The type being created</typeparam>      
        /// <param name="aarrArgs">Arguments to pass into the constructor</param>
        /// <returns>An instance of the type</returns>
        public static T CreateType<T>(this string astrTypeName, params object[] aarrArgs)
        {
            Type ltypType = Type.GetType(astrTypeName, true, true);
            return (T)Activator.CreateInstance(ltypType, aarrArgs);
        }

        /// <summary>
        /// Determines if a string can be converted to an integer.
        /// </summary>
        /// <returns>True if the string is numeric.</returns>
        public static bool IsNumeric(this string astrText)
        {
            System.Text.RegularExpressions.Regex regularExpression = new System.Text.RegularExpressions.Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(astrText).Success;
        }

        public static bool IsLenghtGreaterThanInt32(this string astrText)
        {
            try
            {
                Convert.ToInt32(astrText);
            }
            catch (OverflowException ex)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Detects whether this instance is a valid email address.
        /// </summary>
        /// <returns>True if instance is valid email address</returns>
        public static bool IsValidEmailAddress(this string astrText)
        {
            return IsValidEmail(astrText);
        }

        /// <summary>
        /// Detects whether the supplied string is a valid IP address.
        /// </summary>
        /// <returns>True if the string is valid IP address.</returns>
        public static bool IsValidIPAddress(this string astrText)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(astrText, @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

        /// <summary>
        /// Checks if url is valid.
        /// </summary>
        /// <returns>True if the url is valid.</returns>
        public static bool IsValidUrl(this string astrURL)
        {
            string lstrRegex = "^(https?://)"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" // user@
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                + "|" // allows either IP or domain
                + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]" // second level domain
                + @"(\.[a-z]{2,6})?)" // first level domain- .com or .museum is optional
                + "(:[0-9]{1,5})?" // port number- :80
                + "((/?)|" // a slash isn't required if there is no file name
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";

            return new System.Text.RegularExpressions.Regex(lstrRegex).IsMatch(astrURL);
        }

        /// <summary>
        /// Retrieves the left x characters of a string.
        /// </summary>
        /// <param name="aintCount">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        public static string Left(this string astrText, int aintCount)
        {
            return astrText.Substring(0, aintCount);
        }

        /// <summary>
        /// Retrieves the right x characters of a string.
        /// </summary>
        /// <param name="aintCount">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        public static string Right(this string astrText, int aintCount)
        {
            return astrText.Substring(astrText.Length - aintCount, aintCount);
        }

        /// <summary>
        /// Capitalizes the first letter of a string
        /// </summary>      
        public static string Capitalize(this string astrText)
        {
            if (astrText.Length == 0)
            {
                return astrText;
            }
            if (astrText.Length == 1)
            {
                return astrText.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            }
            return astrText.Substring(0, 1).ToUpper(System.Globalization.CultureInfo.InvariantCulture) + astrText.Substring(1);
        }

        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var isMatching = s.IsMatchingTo(@"^\d+$");
        /// </code>
        /// </example>
        public static bool IsMatchingTo(this string astrText, string astrRegexPattern)
        {
            return IsMatchingTo(astrText, astrRegexPattern, System.Text.RegularExpressions.RegexOptions.None);
        }

        public static bool AnyOfThis(this string astrText, params string[] larrayString)
        {
            foreach (var item in larrayString)
            {
                if (item.ToUpper().Equals(astrText.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool NoneOfThis(this string astrText, params string[] larrayString)
        {
            foreach (var item in larrayString)
            {
                if (item.ToUpper().Equals(astrText.ToUpper()))
                {
                    return false;
                }
            }
            return true;
        }        

        public static bool AnyOfThis(this int aintValue, params int[] larrayint)
        {
            foreach (var item in larrayint)
            {
                if (item == aintValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool NoneOfThis(this int aintValue, params int[] larrayint)
        {
            foreach (var item in larrayint)
            {
                if (item == aintValue)
                {
                    return false;
                }
            }
            return true;
        }

       
        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var isMatching = s.IsMatchingTo(@"^\d+$");
        /// </code>
        /// </example>
        public static bool IsMatchingTo(this string astrText, string astrRegexPattern, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(astrText, astrRegexPattern, options);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <param name="astrReplaceValue">The replacement value.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// </code>
        /// </example>
        public static string ReplaceWith(this string astrValue, string astrRegexPattern, string astrReplaceValue)
        {
            return ReplaceWith(astrValue, astrRegexPattern, astrReplaceValue, System.Text.RegularExpressions.RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <param name="astrReplaceValue">The replacement value.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// </code>
        /// </example>
        public static string ReplaceWith(this string astrText, string astrRegexPattern, string astrReplaceValue, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.Replace(astrText, astrRegexPattern, astrReplaceValue, options);
        }

        /// <summary>
        /// A case insenstive replace function.
        /// </summary>
        /// <param name="astrText">The string to examine.</param>
        /// <param name="astrOldString">The new value to be inserted.</param>
        /// <param name="astrNewString">The value to replace.</param>
        /// <param name="ablnCaseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>The resulting string.</returns>
        public static string Replace(this string astrText, string astrOldString, string astrNewString, bool ablnCaseSensitive)
        {
            if (ablnCaseSensitive)
            {
                return astrText.Replace(astrOldString, astrNewString);
            }
            else
            {
                System.Text.RegularExpressions.Regex aRegex = new System.Text.RegularExpressions.Regex(astrOldString, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

                return aRegex.Replace(astrText, astrNewString);
            }
        }

        /// <summary>
        /// Reverses a string.
        /// </summary>
        /// <param name="astrText">The string to reverse.</param>
        /// <returns>The resulting string.</returns>
        public static string Reverse(this string astrText)
        {
            char[] larrChar = astrText.ToCharArray();
            Array.Reverse(larrChar);
            return new string(larrChar);
        }

        /// <summary>
        /// Splits a string into an array by delimiter.
        /// </summary>
        /// <param name="astrText">String to split.</param>
        /// <param name="astrDelimiter">Delimiter string.</param>
        /// <returns>Array of strings.</returns>
        public static string[] Split(this string astrText, string astrDelimiter)
        {
            return astrText.Split(astrDelimiter.ToCharArray());
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters until the next whitespace on or after 
        /// the total character count has been reached for that line.  
        /// Uses the environment new line symbol for the break text.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aintCount">The number of characters per line.</param>
        /// <returns>The resulting string.</returns>
        public static string WordWrap(this string astrText, int aintCount)
        {
            return WordWrap(astrText, aintCount, false, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aintCount">The number of characters per line.</param>
        /// <param name="ablnCutOff">If true, will break in the middle of a word.</param>
        /// <returns>The resulting string.</returns>
        public static string WordWrap(this string astrText, int aintCount, bool ablnCutOff)
        {
            return WordWrap(astrText, aintCount, ablnCutOff, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the supplied breakText
        /// for line breaks.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aintCount">The number of characters per line.</param>
        /// <param name="ablnCutOff">If true, will break in the middle of a word.</param>
        /// <param name="astrBreakText">The line break text to use.</param>
        /// <returns>The resulting string</returns>
        public static string WordWrap(this string astrText, int aintCount, bool ablnCutOff, string astrBreakText)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(astrText.Length + 100);
            int lintCounter = 0;

            if (ablnCutOff)
            {
                while (lintCounter < astrText.Length)
                {
                    if (astrText.Length > lintCounter + aintCount)
                    {
                        sb.Append(astrText.Substring(lintCounter, aintCount));
                        sb.Append(astrBreakText);
                    }
                    else
                    {
                        sb.Append(astrText.Substring(lintCounter));
                    }

                    lintCounter += aintCount;
                }
            }
            else
            {
                string[] larrStrings = astrText.Split(' ');

                for (int lintIndex = 0; lintIndex < larrStrings.Length; lintIndex++)
                {
                    lintCounter += larrStrings[lintIndex].Length + 1; // the added one is to represent the inclusion of the space.

                    if (lintIndex != 0 && lintCounter > aintCount)
                    {
                        sb.Append(astrBreakText);
                        lintCounter = 0;
                    }

                    sb.Append(larrStrings[lintIndex] + ' ');
                }
            }

            return sb.ToString().TrimEnd(); // to get rid of the extra space at the end.
        }

        /// <summary>
        /// Converts String to Any Other Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrText">The input.</param>
        /// <returns>Return converted type</returns>
        public static T? ConvertTo<T>(this string astrText) where T : struct
        {
            T? ret = null;

            if (!string.IsNullOrEmpty(astrText))
            {
                ret = (T)Convert.ChangeType(astrText, typeof(T));
            }

            return ret;
        }

        /// <summary>
        /// Converts String to Any Other Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrText">The input.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static T? ConvertTo<T>(this string astrText, IFormatProvider provider) where T : struct
        {
            T? ret = null;

            if (!string.IsNullOrEmpty(astrText))
            {
                ret = (T)Convert.ChangeType(astrText, typeof(T), provider);

            }

            return ret;
        }

        /// <summary>
        /// Returns string converted from char.
        /// </summary>
        /// <param name="achrText"></param>
        /// <returns>Return string</returns>
        public static string ToString(this char? achrText)
        {
            return achrText.HasValue ? achrText.Value.ToString() : String.Empty;
        }

        /// <summary>
        /// Returns a Boolean value indicating whether a variable is of the indicated type.
        /// </summary>
        /// <param name="aobjObject">Object instance.</param>
        /// <param name="atypType">The Type to check the object against.</param>
        /// <returns>Result of the comparison.</returns>
        public static bool IsType(this object aobjObject, Type atypType)
        {
            return aobjObject.GetType().Equals(atypType);
        }

        /// <summary>
        /// Creates an instance of the generic type specified using the default constructor.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="atypType">The System.Type being instantiated.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <example>
        /// typeof(MyObject).CreateInstance();
        /// </example>
        public static T CreateInstance<T>(this System.Type atypType) where T : new()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Determines whether an expression evaluates to the DBNull class.
        /// </summary>
        /// <param name="aobjObject">Object instance.</param>
        /// <returns>Returns true if the object is DBNull.</returns>
        public static bool IsDBNull(this object aobjObject)
        {
            return aobjObject.IsType(typeof(DBNull));
        }

        /// <summary>
        /// Rounds the supplied decimal to the specified amount of decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <param name="aintDecimalPoints">The number of decimal points to round the output value to.</param>
        /// <returns>A rounded decimal.</returns>
        public static decimal RoundDecimalPoints(this decimal adecValue, int aintDecimalPoints)
        {
            return Math.Round(adecValue, aintDecimalPoints, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds the supplied decimal value to two decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <returns>A decimal value rounded to two decimal points.</returns>
        public static decimal RoundToTwoDecimalPoints(this decimal adecValue)
        {
            return Math.Round(adecValue, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds the supplied decimal value to four decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <returns>A decimal value rounded to four decimal points.</returns>
        public static decimal RoundToFourDecimalPoints(this decimal adecValue)
        {
            return Math.Round(adecValue, 4, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds the supplied decimal value to six decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <returns>A decimal value rounded to six decimal points.</returns>
        public static decimal RoundToSixDecimalPoints(this decimal adecValue)
        {
            return Math.Round(adecValue, 6, MidpointRounding.AwayFromZero);
        }


        /// <summary>
        /// Determine whether the collection/list is null or empty;
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Returns true if the list is null or empty.</returns>
        public static bool IsNullOrEmpty(this System.Collections.IEnumerable list)
        {
            return list == null ? true : list.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Determine whether the collection/list is empty
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Returns true if the list is empty.</returns>
        public static bool IsEmpty(this System.Collections.IEnumerable list)
        {
            return list == null ? true : list.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Checks a System.Type to see if it implements a given interface.
        /// </summary>
        /// <param name="source">The System.Type to check.</param>
        /// <param name="iface">The System.Type interface to check for.</param>
        /// <returns>True if the source implements the interface type, false otherwise.</returns>
        public static bool IsImplementationOf(this Type atypSource, Type atypInterfaceType)
        {
            if (atypSource == null)
                throw new ArgumentNullException("source");

            return atypSource.GetInterface(atypInterfaceType.FullName) != null;
        }

        /// <summary>
        /// Get age in Years for the given birth date
        /// </summary>
        /// <param name="adtDateOfBirth">Date of Birth</param>
        /// <returns>Age In Years</returns>
        public static int AgeInYears(this DateTime adtDateOfBirth)
        {
            return adtDateOfBirth.AgeInYearsAsOfDate(knfGlobalFunctions.ApplicationDateTime().Date);
        }

        /// <summary>
        /// Get age in Years for the given birth date
        /// </summary>
        /// <param name="adtDateOfBirth">Date of Birth</param>
        /// <param name="adtAsOfDate">As of Date</param>
        /// <returns>Age in Years</returns>
        public static int AgeInYearsAsOfDate(this DateTime adtDateOfBirth, DateTime adtAsOfDate)
        {
            // find the difference in days, months and years
            int lintNoOfDays = adtAsOfDate.Day - adtDateOfBirth.Day;
            int lintNoOfMonths = adtAsOfDate.Month - adtDateOfBirth.Month;
            int lintNoOfYears = adtAsOfDate.Year - adtDateOfBirth.Year;

            if (lintNoOfDays < 0)
            {
                lintNoOfDays += DateTime.DaysInMonth(adtAsOfDate.Year, adtAsOfDate.Month);
                lintNoOfMonths--;
            }

            if (lintNoOfMonths < 0)
            {
                lintNoOfMonths += 12;
                lintNoOfYears--;
            }

            return lintNoOfYears;
        }

        /// <summary>
        /// Checksum validation for the Bank Routing Number.
        /// </summary>
        /// <param name="iargRoutingNo">Routing Number</param>
        /// <returns>TRUE - If valid, FALSE - if Invalid</returns>
        public static bool IsValidCheckSum(string astrRoutingNo)
        {
            // If there is no CheckSum Entered, No Need to validate. Dont throw any error.
            if (astrRoutingNo.IsNull())
                return true;

            string lstrRoutingNo = astrRoutingNo;
            // If the length of the routing number is not 9
            if (lstrRoutingNo.Trim().Length != 9)
                return false;

            //Leading and trailing whitespaces in the string trimmed
            lstrRoutingNo = lstrRoutingNo.Trim();

            // If characters are entered.
            int lintRoutingNumber;
            if (int.TryParse(lstrRoutingNo, out lintRoutingNumber) == false)
                return false;

            int[] larrWeight = new int[] { 3, 7, 1, 3, 7, 1, 3, 7 };
            int lintSum = 0;
            int lintTemp = 0;

            for (int lintCount = 0; lintCount < 8; lintCount++)
            {
                lintTemp = Convert.ToInt16(lstrRoutingNo.Substring(lintCount, 1)) * larrWeight[lintCount];
                lintSum += lintTemp;
            }

            int lintChecksumDigit = lintSum % 10;               // Identify the Last Digit

            int lintCompareDigit = 10 - lintChecksumDigit;      // Subtract by 10

            if (lintCompareDigit == 10)                         // Checksum is 0, Make the Compare digit to 0, instead of 10, Because 10 will always return false.
                lintCompareDigit = 0;

            if (Convert.ToInt16(lstrRoutingNo.Substring(8, 1)) == lintCompareDigit) // The value should be the Last digit of the Routing Number
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the date difference between two date values in months.
        /// </summary>
        /// <param name="adtDateValue1">Date value to compare.</param>
        /// <param name="adtDateValue2">Date value to compare.</param>
        /// <returns>Date difference in months.</returns>
        public static int DateDiffInMonths(DateTime adtDateValue1, DateTime adtDateValue2)
        {
            DateTime ldtLowestDate, ldtHighestDate;

            if (adtDateValue1 > adtDateValue2)
            {
                ldtHighestDate = adtDateValue1;
                ldtLowestDate = adtDateValue2;
            }
            else if (adtDateValue1 < adtDateValue2)
            {
                ldtHighestDate = adtDateValue2;
                ldtLowestDate = adtDateValue1;
            }
            else
            {
                return 0;
            }

            int lintMonths = 0;

            while (ldtLowestDate < ldtHighestDate)
            {
                lintMonths++;
                ldtLowestDate = ldtLowestDate.AddMonths(1);

                if (ldtLowestDate > ldtHighestDate)
                {
                    lintMonths--;
                }
            }

            return Math.Abs(lintMonths);
        }

        /// <summary>
        /// Returns Formatted SSN.
        /// </summary>
        /// <param name="astrSSN">SSN</param>
        /// <returns></returns>
        public static string GetFormattedSSN(string astrSSN)
        {
            string lstrFormattedSSN = String.Empty;

            if (astrSSN.IsNotNullOrEmpty() && astrSSN.Length >= 9)
            {
                lstrFormattedSSN = string.Join("-", astrSSN.Substring(0, 3), astrSSN.Substring(3, 2), astrSSN.Substring(5, 4));
            }
            return lstrFormattedSSN;
        }
        /// <summary>
        /// Returns the Round Up Month Difference 
        /// </summary>
        /// <param name="lValue"></param>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static int MonthDifferenceRoundedUp(this DateTime adtFromDate, DateTime adtToDate)
        {
            return Convert.ToInt32(Math.Round(DateDiff(enmDateInterval.Day, adtToDate, adtFromDate) / (365.25 / 12), 0));
            // return Math.Abs((adtFromDate.Month - adtToDate.Month) + 12 * (adtFromDate.Year - adtToDate.Year));
        }

        /// <summary>
        /// Method to Trim the end of string with String Parameter.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trimChars"></param>
        /// <returns></returns>
        public static string TrimEnd(this string target, string trimChars)
        {
            return target.TrimEnd(trimChars.ToCharArray());
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) action(item);
            return collection;
        }

        //Add New Extention Methode Here

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                // Setting Column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    // Inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            // Put breakpoint here and check datatable
            return dataTable;
        }       

        public static string Crypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string Decrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }







        #region [Melissa Data Static Extension Methods]

        /// <summary>
        /// Method to get formatted person address from Melissa
        /// </summary>
        /// <param name="acdoPersonAddress">Person Address</param>
        /// <returns>Global Address</returns>
        //public static doGlobalAddress ToGlobalAddress(this cdoAddress acdoPersonAddress)
        //{
        //    doGlobalAddress ldoGlobalAddress = new doGlobalAddress();

        //    ldoGlobalAddress.address_line1 = string.IsNullOrEmpty(acdoPersonAddress.address_line_1) ? string.Empty : acdoPersonAddress.address_line_1;
        //    ldoGlobalAddress.address_line2 = string.IsNullOrEmpty(acdoPersonAddress.address_line_2) ? string.Empty : acdoPersonAddress.address_line_2;
        //    ldoGlobalAddress.address_line3 = string.IsNullOrEmpty(acdoPersonAddress.address_line_3) ? string.Empty : acdoPersonAddress.address_line_3;
        //    ldoGlobalAddress.locality = string.IsNullOrEmpty(acdoPersonAddress.istrCity) ? string.Empty : acdoPersonAddress.istrCity;
        //    ldoGlobalAddress.administrative_area = string.IsNullOrEmpty(acdoPersonAddress.istrStateValue) ? string.Empty : acdoPersonAddress.istrStateValue;
        //    ldoGlobalAddress.postal_code = string.IsNullOrEmpty(acdoPersonAddress.istrPostalCode) ? string.Empty : acdoPersonAddress.istrPostalCode;
        //    acdoPersonAddress.PopulateDescriptions();

        //    string istrCountryValue = HelperUtil.GetData1ByCodeValue(81, acdoPersonAddress.istrCountry);
        //    ldoGlobalAddress.country = string.IsNullOrEmpty(istrCountryValue) ? string.Empty : istrCountryValue;

        //    return ldoGlobalAddress;
        //}

        /// <summary>
        /// Check whether string Contains all digits 
        /// </summary>
        /// <param name="astrInputString"></param>
        /// <returns></returns>
        public static bool IsAllDigits(string astrInputString)
        {
            return astrInputString.All(char.IsDigit);
        }
        #endregion [Melissa Data Static Extension Methods]

        #endregion


    }
}