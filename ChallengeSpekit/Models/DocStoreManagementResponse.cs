using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ChallengeSpekit.Models
{


    //public class RequireTransactioCD : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var _CDrequestList = (CD_RequestData)validationContext.ObjectInstance;
    //        var _value = value as string;


    //        if (validationContext.DisplayName == "COMPANY_ID" && (_CDrequestList.COMPANY_ID == null))
    //        {

    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "CNIC" && ( _CDrequestList.CNIC == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "Mobile_Number" && ( _CDrequestList.Mobile_Number == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }

    //        else if (validationContext.DisplayName == "ISSUANCE_DATE" && (_CDrequestList.ISSUANCE_DATE == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "AMOUNT" && ( _CDrequestList.AMOUNT == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "PROGRAM_CODE" && (_CDrequestList.PROGRAM_CODE == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "CORE_ACCOUNT_NUMBER" && (_CDrequestList.CORE_ACCOUNT_NUMBER == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "BANK_CODE" && ( _CDrequestList.CORE_ACCOUNT_NUMBER == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "IBAN_ACCOUNT_NUMBER" && ( _CDrequestList.IBAN_ACCOUNT_NUMBER == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "IS_IBFT" && ( _CDrequestList.IS_IBFT == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "ConversationID" && ( _CDrequestList.ConversationID == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "REF_FIELD_1" && ( _CDrequestList.REF_FIELD_1 == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "REF_FIELD_2" && (_CDrequestList.REF_FIELD_2 == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "REF_FIELD_3" && (_CDrequestList.REF_FIELD_3 == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "REF_FIELD_4" && (_CDrequestList.REF_FIELD_4 == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
    //        else if (validationContext.DisplayName == "REF_FIELD_5" && (_CDrequestList.REF_FIELD_5 == null))
    //        {
    //            return new ValidationResult(validationContext.MemberName + " is required.");
    //        }
            

    //            return ValidationResult.Success;
           



    //    }
    //}
    public class DocStoreManagementResponse
    {

        public string ResponseCode { get; set; }
        public List<string> Document { get; set; }

    }
}