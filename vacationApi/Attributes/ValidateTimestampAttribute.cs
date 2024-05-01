using System;
using System.ComponentModel.DataAnnotations;

public class ValidTimestampAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
      // Sjekk om verdien er en byte-array
      if (value is byte[] timestamp)
      {
          // Legg til valideringslogikk her
          // For eksempel: Sjekk om timestamp er gyldig og ikke null
          if (timestamp != null && timestamp.Length > 0)
          {
              return ValidationResult.Success!;
          }
          else
          {
              return new ValidationResult("Timestamp-verdien er ugyldig.");
          }
      }

      return new ValidationResult("Timestamp-verdien er ugyldig.");
  }
}